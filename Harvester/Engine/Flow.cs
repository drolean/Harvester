using Harvester.Debugger;
using Harvester.Engine.Modules;
using Harvester.GUI;
using System;
using ZzukBot.Game.Statics;
using ZzukBot.Objects;

namespace Harvester.Engine
{
    public class Flow
    {
        private CMD CMD { get; }
        private CombatModule CombatModule { get; }
        private Inventory Inventory { get; }
        private NodeScanModule NodeScanModule { get; }
        private ObjectManager ObjectManager { get; }
        private PathModule PathModule { get; }

        public Flow(CMD cmd, CombatModule combatModule, Inventory inventory, NodeScanModule nodeScanModule, ObjectManager objectManager, PathModule pathModule)
        {
            CMD = cmd;
            CombatModule = combatModule;
            Inventory = inventory;
            NodeScanModule = nodeScanModule;
            ObjectManager = objectManager;
            PathModule = pathModule;
        }

        Logger logger = new Logger();
        WoWGameObject closestNode;

        public void ExecuteFlow()
        {
            if (ObjectManager.Player.IsInCombat && !ObjectManager.Player.IsMounted)
                CombatModule.FightMob();

                if (!ObjectManager.Player.IsInCombat || ObjectManager.Player.IsMounted)
                {
                    closestNode = NodeScanModule.ClosestNode();

                if (closestNode == null)
                {
                    if (ObjectManager.Player.CastingAsName == "Herb Gathering"
                        || ObjectManager.Player.CastingAsName == "Mining")
                        ObjectManager.Player.Jump();

                    if (!ObjectManager.Player.IsMounted
                        && Inventory.GetItemCount(CMD.mountName) > 0)
                        Inventory.GetItem(CMD.mountName).Use();

                    if (ObjectManager.Player.IsMounted
                        || Inventory.GetItemCount(CMD.mountName) == 0)
                        PathModule.Traverse(PathModule.Path(PathModule.GetNextHotspot()));
                }

                if (closestNode != null)
                {
                    if (closestNode.Position.DistanceToPlayer() > 3
                        && (ObjectManager.Player.CastingAsName == "Herb Gathering"
                        || ObjectManager.Player.CastingAsName == "Mining"))
                        ObjectManager.Player.Jump();

                    if (closestNode.Position.DistanceToPlayer() <= 3)
                    {
                        if (ObjectManager.Player.IsMounted)
                            Inventory.GetItem(CMD.mountName).Use();

                        ObjectManager.Player.CtmStopMovement();

                        if (ObjectManager.Player.CastingAsName != "Herb Gathering" 
                            && ObjectManager.Player.CastingAsName != "Mining")
                            closestNode.Interact(true);

                        return;
                    }

                    PathModule.index = -1;
                    PathModule.Traverse(PathModule.Path(NodeScanModule.ClosestNode().Position));
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