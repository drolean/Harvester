using Harvester.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using ZzukBot.Constants;
using ZzukBot.Game.Statics;
using ZzukBot.Objects;

namespace Harvester.Engine.Modules
{
    public class NodeScanModule
    {
        private CMD CMD { get; }
        private ObjectManager ObjectManager { get; }
        private Skills Skills { get; }

        public NodeScanModule(CMD cmd, ObjectManager objectManager, Skills skills)
        {
            CMD = cmd;
            ObjectManager = objectManager;
            Skills = skills;
        }

        public int HerbLevel()
        {
            List<Skills.Skill> skills = Skills.GetAllPlayerSkills();
            Skills.Skill herb = skills.Where(x => x.Id == Enums.Skills.HERBALISM).First();

            return herb.CurrentLevel;
        }

        public int MineLevel()
        {
            List<Skills.Skill> skills = Skills.GetAllPlayerSkills();
            Skills.Skill mine = skills.Where(x => x.Id == Enums.Skills.MINING).First();

            return mine.CurrentLevel;
        }

        public WoWGameObject ClosestHerbNode()
        {
            List<WoWGameObject> nodes = ObjectManager.GameObjects.Where(x => x.GatherInfo.Type == Enums.GatherType.Herbalism).ToList();
            List<string> herbCheckedBoxes = CMD.herbCheckedBoxes;

            if (!nodes.Any() || herbCheckedBoxes?.Any() == false)
                return null;

            nodes = nodes.Where(x => x.GatherInfo.RequiredSkill <= HerbLevel() 
                && herbCheckedBoxes.Any(y => y == x.Name)).ToList();

            return nodes.OrderBy(x => ObjectManager.Player.Position.GetDistanceTo(x.Position)).FirstOrDefault();
        }
    }
}