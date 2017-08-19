using Harvester.Debugger;
using Harvester.Engine.Modules;
using System;
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

        Logger logger = new Logger();

        public void ExecuteFlow()
        {
            if (ObjectManager.Player.IsInCombat)
                CombatModule.FightMob();

            if (!ObjectManager.Player.IsInCombat)
            {
                if (NodeScanModule.ClosestNode() == null)
                    PathModule.Traverse(PathModule.Path(PathModule.GetNextHotspot()));

                if (NodeScanModule.ClosestNode() != null)
                {
                    if (NodeScanModule.ClosestNode().Position.DistanceToPlayer() <= 2)
                    {
                        ObjectManager.Player.CtmStopMovement();

                        if (ObjectManager.Player.CastingAsName != "Herb Gathering" 
                            && ObjectManager.Player.CastingAsName != "Mining")
                            NodeScanModule.ClosestNode().Interact(true);

                        Wait.For("harvest", 5000, true);
                    }

                    if (NodeScanModule.ClosestNode().Position.DistanceToPlayer() > 2)
                    {
                        PathModule.Traverse(PathModule.Path(NodeScanModule.ClosestNode().Position));
                        PathModule.index = -1;
                        PathModule.playerPositions.Add(Convert.ToInt32(ObjectManager.Player.Position.X).ToString()
                            + Convert.ToInt32(ObjectManager.Player.Position.Y).ToString()
                            + Convert.ToInt32(ObjectManager.Player.Position.Z).ToString());

                        if (PathModule.Stuck())
                        {
                            NodeScanModule.blacklist.Add(NodeScanModule.ClosestNode().Guid);
                            logger.LogOne(NodeScanModule.ClosestNode().Guid.ToString());
                            PathModule.playerPositions.Clear();
                        }
                    }
                }
            }
        }
    }
}