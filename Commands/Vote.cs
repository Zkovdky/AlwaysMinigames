using System;
using AutoEvent.Interfaces;
using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Plugins;

namespace AlwaysMinigames.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Vote : ICommand
    {
        public string Command { get; } = "eventvote";
        public string[] Aliases { get; } = { "v" };
        public string Description { get; } = "";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var controller = InfernoPlugin.Instance.controller;

            if (controller == null)
            {
                response = "<color=red>Контроллера не существует!</color>";
                return false;
            }

            if (controller.currentState != ECurrentState.Voting)
            {
                response = "<color=red>Сейчас не идет голосование!</color>";
                return false;
            }
            
            Player player = Player.Get(sender);
            if (player == null)
            {
                response = "";
                return false;
            }

            if (controller.VotesSet.Contains(player.UserId))
            {
                response = "<color=red>Вы уже проголосовали!</color>";
                return false;
            }

            if (arguments.Count < 1 || !int.TryParse(arguments.At(0), out int voteId))
            {
                response = "Использование: .eventvote [номер]";
                return false;
            }
            
            int index = voteId - 1;
            
            if (controller.AddVote(player, index)) 
            {
                if (sender.HasPermissions("alwaysevents.secondvote"))
                {
                    controller.AddVote(player, index);
                }
                controller.VotesSet.Add(player.UserId);
        
                response = $"<color=green>Вы успешно проголосовали за {controller.VoteOptions[index].Name}</color>";
                return true;
            }

            response = "<color=red>Такого номера нет в списке!</color>";
            return false;
        }
        
    }
}