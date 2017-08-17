using ZzukBot.Game.Statics;
using ZzukBot.Objects;

namespace Harvester.Engine
{
    public class Controller
    {
        private Inventory Inventory { get; }
        private ObjectManager ObjectManager { get; }
        private Flow Flow { get; }

        public Controller(Inventory inventory, ObjectManager objectManager, Flow flow)
        {
            Inventory = inventory;
            ObjectManager = objectManager;
            Flow = flow;
        }

        public void Behavior()
        {
            Flow.ExecuteFlow();
        }

        public STATUS StateLogic()
        {
            LocalPlayer player = ObjectManager.Player;
            WoWUnit target = ObjectManager.Target;

            if (player.IsDead)
                return STATUS.DEAD;
            else if (player.InGhostForm)
                return STATUS.GHOST;
            return STATUS.ALIVE;
        }
    }

    public enum STATUS
    {
        ALIVE,
        DEAD,
        GHOST,
    }
}