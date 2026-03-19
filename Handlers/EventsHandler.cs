using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using LightContainmentZoneDecontamination;

namespace AlwaysMinigames.Handlers
{
    public class EventsHandler : CustomEventsHandler
    {
        public override void OnServerRoundStarted()
        {
            Logger.Info("OnServerRoundStarted");
            InfernoPlugin.Instance.controller.BeginVotingSession();
        }

        public override void OnServerWaitingForPlayers()
        {
            Round.IsLocked = true;
            Decontamination.Status = DecontaminationController.DecontaminationStatus.Disabled;
        }

        public override void OnPlayerJoined(PlayerJoinedEventArgs ev)
        {
            if (Round.IsRoundStarted && InfernoPlugin.Instance.controller.currentState == ECurrentState.NotEnoughPlayers && Player.Count > 1)
            {
                var controller = InfernoPlugin.Instance.controller;
                
                controller.BeginVotingSession();
            }
            // Server.PlayerCount;
        }
    }
}
