using Harvester.Engine.Modules;
using Harvester.Debugger;
using ZzukBot.Game.Statics;
using ZzukBot.Helpers;

namespace Harvester.Engine
{
    public class Flow
    {
        private CombatModule CombatModule { get; }
        private NodeScanModule NodeScanModule { get; }
        private ObjectManager ObjectManager { get; }
        private PathModule PathModule { get; }

        public Flow(CombatModule combatModule, NodeScanModule nodeScanModule, ObjectManager objectManager, PathModule pathModule)
        {
            CombatModule = combatModule;
            NodeScanModule = nodeScanModule;
            ObjectManager = objectManager;
            PathModule = pathModule;
        }

        public void ExecuteFlow()
        {
            if (ObjectManager.Player.IsInCombat)
                CombatModule.FightMob();
            if (!ObjectManager.Player.IsInCombat)
            {
                if (NodeScanModule.ClosestHerbNode() != null)
                {
                    PathModule.Traverse(PathModule.Path(NodeScanModule.ClosestHerbNode().Position));
                    if (NodeScanModule.ClosestHerbNode().Position.DistanceToPlayer() <= 2)
                    {
                        ObjectManager.Player.CtmStopMovement();
                        NodeScanModule.ClosestHerbNode().Interact(true);
                        Wait.For("harvest", 500);
                    }
                }

            }
        }
    }
}