using Harvester.Engine;
using Harvester.Engine.Loaders;
using Harvester.Engine.Loaders.Profile;
using Harvester.Engine.Modules;
using Harvester.GUI;
using Harvester.Map;
using System;
using System.ComponentModel.Composition;
using ZzukBot.ExtensionFramework;
using ZzukBot.ExtensionFramework.Interfaces;
using ZzukBot.Game.Statics;

namespace Harvester
{
    [Export(typeof(IBotBase))]
    public class Harvester : IBotBase
    {
        private Router r = new Router();

        public Harvester()
        {
            r.Add(this);
            r.Add(CustomClasses.Instance);
            r.Add(Inventory.Instance);
            r.Add(Lua.Instance);
            r.Add(Navigation.Instance);
            r.Add(ObjectManager.Instance);
            r.Add(Skills.Instance);
            r.Add(Spell.Instance);
            r.Add(new Loader());
            r.Add(new CCLoader(r.Get<CustomClasses>()));
            r.Add(new ProfileLoader(r.Get<Loader>()));
            r.Add(new CMD(r.Get<ProfileLoader>()));
            r.Add(new ConsumablesModule(r.Get<Inventory>(), r.Get<ObjectManager>()));
            r.Add(new PathModule(r.Get<Navigation>(), r.Get<ObjectManager>(), r.Get<ProfileLoader>()));
            r.Add(new CombatModule(r.Get<CustomClasses>(), r.Get<ObjectManager>(), r.Get<PathModule>()));
            r.Add(new NodeScanModule(r.Get<CMD>(), r.Get<ObjectManager>(), r.Get<Skills>()));
            r.Add(new Flow(r.Get<CMD>(), r.Get<CombatModule>(), r.Get<ConsumablesModule>(), 
                r.Get<Inventory>(), r.Get<Lua>(), r.Get<NodeScanModule>(), 
                r.Get<ObjectManager>(), r.Get<PathModule>(), r.Get<Spell>()));
            r.Add(new Controller(r.Get<Flow>(), r.Get<Inventory>(), r.Get<ObjectManager>(), 
                r.Get<PathModule>()));
            r.Add(new Manager(r.Get<CCLoader>(), r.Get<Controller>(), r.Get<ObjectManager>(), 
                r.Get<ProfileLoader>()));
        }

        public string Author { get; } = "krycess";
        public string Name { get; } = "Harvester";
        public Version Version { get; } = new Version(1, 0, 11, 63);

        public void ShowGui()
        {
            CMD settings = r.Get<CMD>();

            if (!settings.Visible)
                settings.Show();
        }

        public bool Start(Action stopCallback) => r.Get<Manager>().Start(stopCallback);
        public void Stop() => r.Get<Manager>().Stop();
        public void Dispose() => r.Get<Manager>().Dispose();
        public void PauseBotbase(Action onPauseCallback) => r.Get<Manager>().Pause();
        public bool ResumeBotbase() => r.Get<Manager>().Resume();
    }
}