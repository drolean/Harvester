using Harvester.GUI;
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

        public WoWGameObject ClosestNode()
        {
            List<WoWGameObject> herbNodes = ObjectManager.GameObjects
                .Where(x => x.GatherInfo.Type == Enums.GatherType.Herbalism).ToList();
            List<WoWGameObject> mineNodes = ObjectManager.GameObjects
                .Where(x => x.GatherInfo.Type == Enums.GatherType.Mining).ToList();

            List<string> herbCheckedBoxes = CMD.herbCheckedBoxes;
            List<string> mineCheckedBoxes = CMD.mineCheckedBoxes;

            if (herbNodes?.Any() == true && mineNodes?.Any() == false)
            {
                herbNodes = herbNodes.Where(x => /*x.GatherInfo.RequiredSkill <= HerbLevel() 
                    &&*/ herbCheckedBoxes.Any(y => y == x.Name)).ToList();

                return herbNodes.OrderBy(x => ObjectManager.Player.Position.GetDistanceTo(x.Position)).FirstOrDefault();
            }

            if (herbNodes?.Any() == false && mineNodes?.Any() == true)
            {
                mineNodes = mineNodes.Where(x => /*x.GatherInfo.RequiredSkill <= MineLevel() 
                    &&*/ mineCheckedBoxes.Any(y => y == x.Name)).ToList();

                return mineNodes.OrderBy(x => ObjectManager.Player.Position.GetDistanceTo(x.Position)).FirstOrDefault();
            }

            return herbNodes.Concat(mineNodes).OrderBy(x => ObjectManager.Player.Position.GetDistanceTo(x.Position)).FirstOrDefault();
        }
    }
}