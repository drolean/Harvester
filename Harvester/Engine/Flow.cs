using Harvester.Debugger;
using Harvester.Engine.Modules;
using Harvester.GUI;
using System;
using ZzukBot.Constants;
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
        private Lua Lua { get; }
        private NodeScanModule NodeScanModule { get; }
        private ObjectManager ObjectManager { get; }
        private PathModule PathModule { get; }
        private Spell Spell { get; }

        public Flow(CMD cmd, CombatModule combatModule, ConsumablesModule consumablesModule, 
            Inventory inventory, Lua lua, NodeScanModule nodeScanModule, 
            ObjectManager objectManager, PathModule pathModule, Spell spell)
        {
            CMD = cmd;
            CombatModule = combatModule;
            ConsumablesModule = consumablesModule;
            Inventory = inventory;
            Lua = lua;
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
            closestNode = NodeScanModule.ClosestNode();

            if (ObjectManager.Player.IsInCombat)
            {
                if (closestNode == null)
                {
                    if (ObjectManager.Player.IsMounted)
                        PathModule.Traverse(PathModule.GetNextHotspot());

                    if (!ObjectManager.Player.IsMounted)
                    {
                        if (CombatModule.EliteInCombatNPC() == null)
                            CombatModule.Fight();

                        if (CombatModule.EliteInCombatNPC() != null)
                            PathModule.Traverse(PathModule.GetNextHotspot());
                    }
                }

                if (closestNode != null)
                {
                    nodeGuardian = NodeScanModule.NodeGuardian(closestNode);

                    if (nodeGuardian != null)
                    {
                        if ((nodeGuardian.CreatureRank & Enums.CreatureRankTypes.Elite) == Enums.CreatureRankTypes.Elite
                            || (nodeGuardian.CreatureRank & Enums.CreatureRankTypes.RareElite) == Enums.CreatureRankTypes.RareElite)
                        {
                            NodeScanModule.blacklist.Add(closestNode.Guid);

                            return;
                        }
                    }

                    if (ObjectManager.Player.IsMounted)
                    {
                        if (CombatModule.EliteInCombatNPC() == null)
				            Inventory.GetItem(CMD.mountName).Use();

                        if (CombatModule.EliteInCombatNPC() != null)
                            PathModule.Traverse(NodeScanModule.ClosestNode().Position);
                    }

                    if (!ObjectManager.Player.IsMounted)
                    {
                        if (CombatModule.EliteInCombatNPC() == null)
                            CombatModule.Fight();

                        if (CombatModule.EliteInCombatNPC() != null)
                            PathModule.Traverse(NodeScanModule.ClosestNode().Position);
                    }
                }
            }

            if (!ObjectManager.Player.IsInCombat)
            {
                if (!CombatModule.IsReadyToFight())
                {
                    if (ObjectManager.Player.IsMounted)
                        Inventory.GetItem(CMD.mountName).Use();

                    if (ConsumablesModule.Food() != null
                        && ObjectManager.Player.HealthPercent < 60
                        && !ObjectManager.Player.GotAura("Food"))
                        Inventory.GetItem(ConsumablesModule.Food().Name).Use();

                    if (ConsumablesModule.Drink() != null
                        && ObjectManager.Player.ManaPercent < 60
                        && !ObjectManager.Player.GotAura("Drink"))
                        Inventory.GetItem(ConsumablesModule.Drink().Name).Use();
                }

                if (CombatModule.IsReadyToFight())
                {
                    if (closestNode == null)
                    {
                        if (ObjectManager.Player.CastingAsName == "Herb Gathering"
                            || ObjectManager.Player.CastingAsName == "Mining")
                            Spell.StopCasting();

                        if (!ObjectManager.Player.IsMounted
                            && Inventory.GetItemCount(CMD.mountName) > 0
                            && !ObjectManager.Player.IsSwimming)
                        {
                            Lua.Execute("DoEmote('stand')");
                            Inventory.GetItem(CMD.mountName).Use();
                        }

                        if (ObjectManager.Player.IsMounted
                            || Inventory.GetItemCount(CMD.mountName) == 0
                            || ObjectManager.Player.IsSwimming)
                            PathModule.Traverse(PathModule.GetNextHotspot());
                    }

                    if (closestNode != null)
                    {
                        nodeGuardian = NodeScanModule.NodeGuardian(closestNode);

                        if (nodeGuardian != null)
                        {
                            if ((nodeGuardian.CreatureRank & Enums.CreatureRankTypes.Elite) == Enums.CreatureRankTypes.Elite
                            || (nodeGuardian.CreatureRank & Enums.CreatureRankTypes.RareElite) == Enums.CreatureRankTypes.RareElite)
                            {
                                NodeScanModule.blacklist.Add(closestNode.Guid);

                                return;
                            }

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
                            {
                                Lua.Execute("DoEmote('stand')");
                                closestNode.Interact(true);
                            }

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
                            PathModule.playerPositions.Clear();
                            ObjectManager.Player.Jump();
                        }
                    }
                }
            }
        }
    }
}