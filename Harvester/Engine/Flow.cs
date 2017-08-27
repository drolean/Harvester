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
        private ConsumablesModule ConsumablesModule { get; }
        private Inventory Inventory { get; }
        private NodeScanModule NodeScanModule { get; }
        private ObjectManager ObjectManager { get; }
        private PathModule PathModule { get; }
        private Spell Spell { get; }

        public Flow(CMD cmd, CombatModule combatModule, ConsumablesModule consumablesModule, 
            Inventory inventory, NodeScanModule nodeScanModule, ObjectManager objectManager, 
            PathModule pathModule, Spell spell)
        {
            CMD = cmd;
            CombatModule = combatModule;
            Inventory = inventory;
            NodeScanModule = nodeScanModule;
            ObjectManager = objectManager;
            PathModule = pathModule;
            Spell = spell;
        }

        Logger logger = new Logger();
        WoWGameObject closestNode;
        WoWUnit nodeGuardian;

        public void ExecuteFlow()
        {
            if (ObjectManager.Player.IsInCombat && !ObjectManager.Player.IsMounted)
                CombatModule.Fight();

            if (!ObjectManager.Player.IsInCombat || ObjectManager.Player.IsMounted)
            {
                if (!CombatModule.IsReadyToFight())
                {
                    if (ObjectManager.Player.IsMounted)
                        Inventory.GetItem(CMD.mountName).Use();

                    if (ObjectManager.Player.HealthPercent < 45)
                        ConsumablesModule.Eat();

                    if (ObjectManager.Player.ManaPercent < 45)
                        ConsumablesModule.Drink();
                }
                if (CombatModule.IsReadyToFight())
                {
                    closestNode = NodeScanModule.ClosestNode();

                    if (closestNode == null)
                    {
                        if (ObjectManager.Player.CastingAsName == "Herb Gathering"
                            || ObjectManager.Player.CastingAsName == "Mining")
                            Spell.StopCasting();

                        if (!ObjectManager.Player.IsMounted
                            && Inventory.GetItemCount(CMD.mountName) > 0
                            && !ObjectManager.Player.IsSwimming)
                            Inventory.GetItem(CMD.mountName).Use();

                        if (ObjectManager.Player.IsMounted
                            || Inventory.GetItemCount(CMD.mountName) == 0
                            || ObjectManager.Player.IsSwimming)
                            PathModule.Traverse(PathModule.GetNextHotspot());
                    }

                    if (closestNode != null)
                    {
                        if (ObjectManager.Player.IsInCombat)
                        {
                            if (ObjectManager.Player.IsMounted)
                                Inventory.GetItem(CMD.mountName).Use();
                        }
                        nodeGuardian = NodeScanModule.NodeGuardian(closestNode);

                        if (nodeGuardian != null)
                        {
                            if (ObjectManager.Player.IsMounted)
                                Inventory.GetItem(CMD.mountName).Use();

                            ObjectManager.Player.SetTarget(nodeGuardian);

                            if (ObjectManager.Target == nodeGuardian)
                                CombatModule.Pull(ObjectManager.Target);

                            return;
                        }

                        if (closestNode.Position.DistanceToPlayer() > 3
                            && (ObjectManager.Player.CastingAsName == "Herb Gathering"
                            || ObjectManager.Player.CastingAsName == "Mining"))
                            Spell.StopCasting();

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

                        PathModule.Traverse(NodeScanModule.ClosestNode().Position);
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
}