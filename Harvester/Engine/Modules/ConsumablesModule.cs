using System.Collections.Generic;
using System.Linq;
using ZzukBot.Objects;
using ZzukBot.Game.Statics;

namespace Harvester.Engine.Modules
{
    public class ConsumablesModule
    {
        private Inventory Inventory { get; }
        private ObjectManager ObjectManager { get; }

        public ConsumablesModule(Inventory inventory, ObjectManager objectManager)
        {
            Inventory = inventory;
            ObjectManager = objectManager;
        }

        public List<string> foodNames = new List<string>(new string[]
        {
            "Tough Hunk of Bread", "Darnassian Bleu", "Slitherskin Mackeral", "Shiny Red Apple", "Forest Mushroom Cap", "Tough Jerky",
            "Freshly Baked Bread", "Dalaran Sharp", "Longjaw Mud Snapper", "Tel'Abim Banana", "Red-speckled Mushroom", "Haunch of Meat",
            "Moist Cornbread", "Dwarven Mild", "Bristle Whisker Catfish", "Snapvine Watermelon", "Spongy Morel", "Mutton Chop",
            "Mulgore Spice Bread", "Stormwind Brie", "Rockscale Cod", "Goldenbark Apple", "Delicious Cave Mold", "Wild Hog Shank",
            "Soft Banana Bread", "Fine Aged Cheddar", "Spotted Yellowtail", "Striped Yellowtail", "Moon Harvest Pumpkin", "Raw Black Truffle", "Cured Ham Steak",
            "Homemade Cherry Pie", "Alterac Swiss", "Spinefin Halibut", "Deep Fried Plantains", "Dried King Bolete", "Roasted Quail",
            "Conjured Sweet Roll", "Conjured Cinnamon Roll"
        });

        public string[] drinkNames =
        {
            "Refreshing Spring Water", "Conjured Water",
            "Ice Cold Milk", "Conjured Fresh Water",
            "Melon Juice", "Conjured Purified Water",
            "Moonberry Juice", "Conjured Mineral Water",
            "Sweet Nectar", "Conjured Sparkling Water",
            "Morning Glory Dew", "Conjured Crystal Water",
        };

        public WoWItem SelectedFood()
        {
            return Inventory.GetItem(foodNames.Where(x => Inventory.GetItemCount(x) > 0).FirstOrDefault());
        }

        public void Drink()
        {
            List<string> drinks = drinkNames.ToList();
            Inventory.GetItem(drinks.Where(x => Inventory.GetItemCount(x) > 0).FirstOrDefault()).Use();
        }
    }
}