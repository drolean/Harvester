using System.Collections.Generic;
using System.Linq;
using ZzukBot.Constants;
using ZzukBot.ExtensionFramework;
using ZzukBot.ExtensionFramework.Classes;
using ZzukBot.Game.Statics;
using ZzukBot.Objects;

namespace Harvester.Engine.Modules
{
    public class CombatModule
    {
        private CustomClasses CustomClasses { get; }
        private ObjectManager ObjectManager { get; }
        private PathModule PathModule { get; }

        public CombatModule(CustomClasses customClasses, ObjectManager objectManager, PathModule pathModule)
        {
            CustomClasses = customClasses;
            ObjectManager = objectManager;
            PathModule = pathModule;
        }

        public bool LootableExists()
        {
            List<WoWUnit> npcs = ObjectManager.Npcs;

            if (npcs.Exists(x => x.CanBeLooted))
                return true;

            return false;
        }

        public void GetMob(WoWUnit mob)
        {
            LocalPlayer player = ObjectManager.Player;
            WoWUnit target = ObjectManager.Target;

            player.SetTarget(mob);
        }

        public WoWUnit GetClosestLootable()
        {
            LocalPlayer player = ObjectManager.Player;
            List<WoWUnit> npcs = ObjectManager.Npcs;
            WoWUnit npc;
            bool lootableExists = npcs.Exists(x => x.CanBeLooted);

            if (lootableExists)
            {
                npcs = npcs.Where(x => x.CanBeLooted).ToList();
                npc = npcs.OrderBy(x => player.Position.GetDistanceTo(x.Position)).First();
                return npc;
            }

            return null;
        }

        public bool LootableInRange()
        {
            if (GetClosestLootable().DistanceToPlayer <= 3)
                return true;

            return false;
        }

        public void TraverseToClosestLootable()
        {
            PathModule.Traverse(PathModule.Path(GetClosestLootable().Position));
        }

        public WoWUnit GetClosestMob()
        {
            LocalPlayer player = ObjectManager.Player;
            List<WoWUnit> npcs = ObjectManager.Npcs;
            WoWUnit npc;
            bool npcExists = npcs.Exists(x => x.IsMob);

            if (npcExists)
            {
                npcs = npcs.Where(x => !x.IsCritter && x.Health > 0 && x.Flags == 0
                    && (x.Reaction & Enums.UnitReaction.Friendly) != Enums.UnitReaction.Friendly
                    && (x.NpcFlags & Enums.NpcFlags.UNIT_NPC_FLAG_GOSSIP) != Enums.NpcFlags.UNIT_NPC_FLAG_GOSSIP
                    && (x.NpcFlags & Enums.NpcFlags.UNIT_NPC_FLAG_VENDOR) != Enums.NpcFlags.UNIT_NPC_FLAG_VENDOR).ToList();
                npc = npcs.OrderBy(x => player.Position.GetDistanceTo(x.Position)).First();
                return npc;
            }

            return null;
        }

        public void PullMob(WoWUnit target)
        {
            LocalPlayer player = ObjectManager.Player;
            CustomClass cc = CustomClasses.Current;

            if (target != null)
                cc.Pull(target);
        }

        public void FightMob()
        {
            LocalPlayer player = ObjectManager.Player;
            WoWUnit target = ObjectManager.Target;
            List<WoWUnit> possibleTargets = ObjectManager.Units.ToList();
            CustomClass cc = CustomClasses.Current;

            if (target != null && possibleTargets.Count() > 0)
                cc.Fight(possibleTargets);
        }
    }
}