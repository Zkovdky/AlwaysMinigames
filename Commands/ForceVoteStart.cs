using CommandSystem;
using LabApi.Features.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlwaysMinigames.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ForceVoteStart : ICommand
    {
        public string Command => "votestart";

        public string[] Aliases { get; }

        public string Description => "Принудительно запустить голосование за мини-игру";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.HasPermissions("alwaysminigames.management"))
            {
                response = "<color=red>Недостаточно прав для выполнения этой команды!</color>";
                return false;
            }

            if (InfernoPlugin.Instance.controller == null)
            {
                response = "<color=red>Контроллер является null, любые обращения к нему невозможны!</color>";
                return false;
            }

            InfernoPlugin.Instance.controller.BeginVotingSession();
            response = "<color=green>Голосование было запрошено!</color>";
            return true;
        }
    }
}
