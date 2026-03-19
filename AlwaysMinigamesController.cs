#define DEBUG

using LabApi.Features.Wrappers;
using System.Linq;
using CommandSystem;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using AutoEvent.API.Season;
using AutoEvent.Interfaces;
using LabApi.Features.Permissions;
using MEC;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;
using Random = UnityEngine.Random;


namespace AlwaysMinigames
{
    public enum ECurrentState
    {
        Unknown = 0,
        NotEnoughPlayers = 1,
        Voting = 2,
        Active = 3,
        RoundNotStarted = 4,
    };

    public class AlwaysMinigamesController : IDisposable
    {
        //public bool 
        public ECurrentState currentState = ECurrentState.Unknown;
        private Event EventToStart;
        private CoroutineHandle _hintVoteCoroutineHandle;
        private CoroutineHandle _eventWatchdogCoroutineHandle;
        
        public List<Event> VoteOptions = new List<Event>();
        public Dictionary<Event, int> VoteEvents = new Dictionary<Event, int>();
        public HashSet<string> VotesSet = new HashSet<string>();
        

        public void BeginMinigamesRound()
        {
            var filteredList = Player.ReadyList.Where(p => p.Role != RoleTypeId.Overwatch).ToList();

            if (filteredList.Count < 2)
            {
                foreach(var player in Player.ReadyList)
                {
                    player.SendHint("<color=red>Недостаточно игроков для начала раунда мини-игр!</color>", 10f);
                }
                currentState = ECurrentState.NotEnoughPlayers;
                return;
            }

            Logger.Info("BeginMinigamesRound");
            
            // foreach (var player in filteredList)
            // {
            //     player.SetRole(RoleTypeId.ClassD);
            //     player.SendHint("Hint", 5f);
            // }
            currentState = ECurrentState.Active;
            
            var eventList = AutoEvent.AutoEvent.EventManager.Events;
            if (AutoEvent.AutoEvent.EventManager.CurrentEvent != null) AutoEvent.AutoEvent.EventManager.CurrentEvent.StopEvent();
            Logger.Info("StartEvent");
            
            var minigame = EventToStart;
            if (minigame == null)
            {
                minigame = eventList.RandomItem();
            }

            try
            {
                Logger.Info("Trying add to delegate and start the event");
                minigame.StartEvent();
                _eventWatchdogCoroutineHandle = Timing.RunCoroutine(EventWatchdog());
            }catch(Exception e)
            {
                Logger.Error($"Exception was handled when starting the minigame: {e}");
                Server.SendBroadcast($"Exception during start the event: {e}. Try start voting again!", 10);
                BeginVotingSession();
                if (_eventWatchdogCoroutineHandle.IsRunning)
                {
                    Timing.KillCoroutines(_eventWatchdogCoroutineHandle);
                }
            }
        }

        private IEnumerator<float> EventWatchdog()
        {
            yield return Timing.WaitForSeconds(3f);
            while (currentState == ECurrentState.Active && AutoEvent.AutoEvent.EventManager.CurrentEvent != null)
            {
                yield return Timing.WaitForSeconds(1f);
            }
            Logger.Debug("EventWatchdog was called!");
            BeginVotingSession();
        }
        
        public void BeginVotingSession()
        {
            Logger.Info("Trying to start the voting session");
            if (currentState == ECurrentState.Voting) return;
            if (Player.ReadyList.Where(p => p.Role != RoleTypeId.Overwatch).Count() < 2)
            {
                foreach(var player in Player.ReadyList)
                {
                    player.SendHint("<color=red>Недостаточно игроков для начала голосования!</color>", 10f);
                }
                currentState =  ECurrentState.NotEnoughPlayers;
                return;
             }
            
            currentState = ECurrentState.Voting;
            
            VoteOptions.Clear();
            VoteEvents.Clear();
            VotesSet.Clear();
            
            Timing.KillCoroutines(_hintVoteCoroutineHandle);
            
            Logger.Info("BeginVotingSession");
            
            foreach (var player in Player.ReadyList)
            {
                if (player.Role == RoleTypeId.Overwatch) continue;
                player.SetRole(RoleTypeId.ClassD);
                player.Position = new Vector3(39.139f, 314.112f, -32.79f);
            }
            
            var eventList = AutoEvent.AutoEvent.EventManager.Events.Where(ev => ev is not IHiddenCommand).OrderBy(x => x.Name)
                .ToList();
            
            var allEvents = AutoEvent.AutoEvent.EventManager.Events
                .Where(ev => ev is not IHiddenCommand)
                .OrderBy(x => Random.value)
                .Take(3)
                .ToList();
            
            foreach (var ev in allEvents)
            {
                VoteOptions.Add(ev);
                VoteEvents.Add(ev, 0);
            }
            Logger.Info("Starting coroutines");
            _hintVoteCoroutineHandle = Timing.RunCoroutine(HintVoteCoroutine());
            Timing.RunCoroutine(VoteTimer(_hintVoteCoroutineHandle));
        }

        private IEnumerator<float> VoteTimer(CoroutineHandle hintCoroutineHandle)
        {
            Logger.Info("Vote timer was started");
            yield return Timing.WaitForSeconds(30);
            
            Timing.KillCoroutines(hintCoroutineHandle);
            currentState = ECurrentState.Unknown;
            var winnerPair = VoteEvents.OrderByDescending(x => x.Value).FirstOrDefault();
            EventToStart = winnerPair.Key;
            BeginMinigamesRound();
            Logger.Info("Vote timer was finished");
        }
        

        private IEnumerator<float> HintVoteCoroutine()
        {
            Logger.Info("Hint coroutine was started");
            Logger.Debug($"Players: {Player.List.Count}");
            while (true)
            {
                Logger.Debug("Waiting...");
                yield return Timing.WaitForSeconds(1f);
                Logger.Debug("Sending hint to players...");

                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("\n\n<size=70%><color=green>ГОЛОСОВАНИЕ</color>\n");

                    for (int i = 0; i < VoteOptions.Count; i++)
                    {
                        var ev = VoteOptions[i];
                        int votes = VoteEvents[ev];
                        sb.AppendLine(
                            $"<color=yellow><b>{ev.Name} <color=white>-</color> [{i + 1}] номер. Голосов: {votes}</b></color>");
                    }

                    sb.AppendLine("\n<color=green>'.v [номер]'</color> в консоль <size=50%><color=grey>(~)</size></color></size>");

                    string finalHint = sb.ToString();
                    foreach (Player player in Player.ReadyList)
                    {
                        player.SendHint(finalHint, 1.1f);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error($"Hint error: {e}");
                }
            }
        }
        
        public void OnEventFinished(string eventName)
        {
            Logger.Info("PreOnEventFinished");
            AutoEvent.AutoEvent.EventManager.GetEvent(eventName).EventStopped -= OnEventFinished;
            Logger.Info("OnEventFinished");
            Timing.CallDelayed(2f, BeginVotingSession);
        }
        
        public bool AddVote(Player player, int index)
        {
            if (index < 0 || index >= VoteOptions.Count) return false;
            
            Event selected = VoteOptions[index];
            VoteEvents[selected]++;
            return true;
        }

        public void Dispose()
        {
            Timing.KillCoroutines(_hintVoteCoroutineHandle);
            Timing.KillCoroutines(_eventWatchdogCoroutineHandle);
             if (AutoEvent.AutoEvent.EventManager.CurrentEvent != null)
             {
                 AutoEvent.AutoEvent.EventManager.CurrentEvent.StopEvent();
             }
             currentState = ECurrentState.Unknown;
        }
    }
}
