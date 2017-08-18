﻿using Harvester.Engine.Modules;
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
                if (NodeScanModule.ClosestNode() != null)
                {
                    PathModule.Traverse(PathModule.Path(NodeScanModule.ClosestNode().Position));

                    if (NodeScanModule.ClosestNode().Position.DistanceToPlayer() <= 2)
                    {
                        ObjectManager.Player.CtmStopMovement();

                        if (ObjectManager.Player.CastingAsName != "Gathering" 
                            && ObjectManager.Player.CastingAsName != "Mining")
                            NodeScanModule.ClosestNode().Interact(true);

                        Wait.For("harvest", 5000, true);
                    }
                }

                if (NodeScanModule.ClosestNode() == null)
                    PathModule.Traverse(PathModule.Path(PathModule.GetNextHotspot()));
            }
        }
    }
}