using System;
using System.Text;
using CommandSystem;
using Unity.IO.LowLevel.Unsafe;

namespace AlwaysMinigames.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class VoteEventList : ICommand
    {
        public string Command { get; } = "eventlist";
        public string[] Aliases { get; }
        public string Description { get; }
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (InfernoPlugin.Instance.controller.currentState != ECurrentState.Voting)
            {
                response = "Сейчас не голосование";
                return false;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<color=red>Ивенты</color>");
            foreach (var minigame in InfernoPlugin.Instance.controller.VoteOptions)
            {
                sb.AppendLine($"<color=white>{minigame.Name} id {InfernoPlugin.Instance.controller.VoteOptions.IndexOf(minigame) + 1} </color>");
            }
            response = sb.ToString();
            return true;
        }
    }
}