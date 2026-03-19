using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using System;
using AutoEvent;
using AutoEvent.Interfaces;
using AlwaysMinigames.Handlers;
using LabApi.Events.CustomHandlers;
using LabApi.Loader.Features.Plugins.Enums;

namespace AlwaysMinigames
{
    public class InfernoPlugin : Plugin
    {
        public override string Name { get; } = "AlwaysMinigames";

        public override string Description { get; } = "Makes your SCP:SL server auto controllable and startes minigames from autoevents plugin";

        public override string Author { get; } = "author";

        public override Version Version { get; } = new Version(1,0,0,0);

        public override Version RequiredApiVersion { get; } = new Version(LabApiProperties.CompiledVersion);

        public override LoadPriority Priority { get; } = LoadPriority.Lowest;
        // singleton 
        static public InfernoPlugin Instance { get; private set; }

        public Event lastEvent;

        private EventsHandler _eventsHandler;

        public AlwaysMinigamesController controller;

        public override void Enable()
        {
            Instance = this;
    
            _eventsHandler = new EventsHandler();
            controller = new AlwaysMinigamesController();
            
            CustomHandlersManager.RegisterEventsHandler(_eventsHandler);

            lastEvent = AutoEvent.AutoEvent.EventManager.CurrentEvent;
        }
        

        public override void Disable()
        {
            Instance = null;
            
            CustomHandlersManager.UnregisterEventsHandler(_eventsHandler);
            
            _eventsHandler = null;
            controller = null;

            lastEvent = null;
        }
    }
}
