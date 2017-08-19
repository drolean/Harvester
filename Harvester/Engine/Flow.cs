using Harvester.Debugger;
using Harvester.Engine.Modules;
using System;
using ZzukBot.Game.Statics;
using ZzukBot.Objects;

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

        Logger logger = new Logger();
        WoWGameObject closestNode;

        public void ExecuteFlow()
        {
            if (ObjectManager.Player.IsInCombat)
                CombatModule.FightMob();

            if (!ObjectManager.Player.IsInCombat)
            {
                closestNode = NodeScanModule.ClosestNode();

                if (closestNode == null)
                    PathModule.Traverse(PathModule.Path(PathModule.GetNextHotspot()));

                if (closestNode != null)
                {
                    if (closestNode.Position.DistanceToPlayer() <= 3)
                    {
                        ObjectManager.Player.CtmStopMovement();

                        if (ObjectManager.Player.CastingAsName != "Herb Gathering" 
                            && ObjectManager.Player.CastingAsName != "Mining")
                            closestNode.Interact(true);

                        return;
                    }

                    PathModule.Traverse(PathModule.Path(NodeScanModule.ClosestNode().Position));
                    PathModule.index = -1;
                    PathModule.playerPositions.Add(Convert.ToInt32(ObjectManager.Player.Position.X).ToString()
                        + Convert.ToInt32(ObjectManager.Player.Position.Y).ToString()
                        + Convert.ToInt32(ObjectManager.Player.Position.Z).ToString());

                    if (PathModule.Stuck())
                    {
                        NodeScanModule.blacklist.Add(closestNode.Guid);
                        logger.LogOne(closestNode.Guid.ToString());
                        PathModule.playerPositions.Clear();
                    }
                }
            }
        }
    }
}