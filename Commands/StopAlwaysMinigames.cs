using System;
using CommandSystem;
using LabApi.Features.Permissions;

namespace AlwaysMinigames.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class StopAlwaysMinigames : ICommand
    {
        public string Command { get; } = "StopMinigames";
        public string[] Aliases { get; } = { "StopAlwaysMinigames" };
        public string Description { get; }
        
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.HasPermissions("alwaysminigames.management"))
            {
                response = "Недостаточно прав для выполнения этой команды!";
                return false;
            }
    
            InfernoPlugin.Instance.controller?.Dispose();
            InfernoPlugin.Instance.controller = null;
            AutoEvent.AutoEvent.EventManager?.CurrentEvent?.StopEvent();
            GC.Collect();
            response = "Контроллер был занулен и по идее должен очиститься гарбаж коллектором токсика";
            return false;
        }
    }
}