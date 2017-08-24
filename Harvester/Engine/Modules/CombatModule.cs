using System.Linq;
using ZzukBot.Constants;
using ZzukBot.ExtensionFramework;
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

        public WoWUnit ClosestCombattableNPC()
        {
            return ObjectManager.Npcs.Where(x => !x.IsCritter && !x.IsDead && x.Flags == 0
                && (x.Reaction & Enums.UnitReaction.Friendly) != Enums.UnitReaction.Friendly
                && (x.NpcFlags & Enums.NpcFlags.UNIT_NPC_FLAG_GOSSIP) != Enums.NpcFlags.UNIT_NPC_FLAG_GOSSIP
                && (x.NpcFlags & Enums.NpcFlags.UNIT_NPC_FLAG_VENDOR) != Enums.NpcFlags.UNIT_NPC_FLAG_VENDOR)
                .OrderBy(x => ObjectManager.Player.Position.GetDistanceTo(x.Position))
                .FirstOrDefault();
        }

        public WoWUnit ClosestLootableNPC()
        {
            return ObjectManager.Npcs.Where(x => x.CanBeLooted)
                .OrderBy(x => ObjectManager.Player.Position.GetDistanceTo(x.Position))
                .FirstOrDefault();
        }

        public void Fight()
        {
            if (ObjectManager.Units.Count() > 0 && ObjectManager.Target != null)
                CustomClasses.Current.Fight(ObjectManager.Units);
        }

        public bool IsReadyToFight()
        {
            return CustomClasses.Current.IsReadyToFight(ObjectManager.Units);
        }

        public void Pull(WoWUnit target)
        {
            if (target != null)
                CustomClasses.Current.Pull(target);
        }
    }
}