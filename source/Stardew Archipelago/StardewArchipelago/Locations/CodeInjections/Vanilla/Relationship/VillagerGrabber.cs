/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using StardewArchipelago.Stardew;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla.Relationship
{
    public class VillagerGrabber
    {
        private StardewItemManager _itemManager;

        public Dictionary<string, Dictionary<StardewObject, int>> GrabberItems { get; private set; }

        public VillagerGrabber(StardewItemManager itemManager)
        {
            _itemManager = itemManager;
            InitializeGrabberItems();
        }

        private void InitializeGrabberItems()
        {
            GrabberItems = new Dictionary<string, Dictionary<StardewObject, int>>();

            AddGrabberItem("Abigail", new[] { "Amethyst", "Pumpkin" });
            AddGrabberItem("Alex", new[] { "Complete Breakfast", "Egg", "Egg (Brown)" });
            AddGrabberItem("Caroline", new[] { "Cauliflower", "Potato", "Parsnip" });
            AddGrabberItem("Clint", new[] { "Copper Bar", "Iron Bar", "Gold Bar" });
            AddGrabberItem("Demetrius", new[] { "Nautilus Shell", "Rainbow Shell", "Bream", "Amethyst" });
            AddGrabberItem("Dwarf", new[] { "Omni Geode", "Dwarf Scroll I", "Dwarf Scroll II", "Dwarf Scroll III", "Dwarf Scroll IV" });
            AddGrabberItem("Elliott", new[] { "Squid Ink", "Duck Feather", "Lobster" });
            AddGrabberItem("Emily", new[] { "Sea Urchin", "Cloth", "Wool" });
            AddGrabberItem("Evelyn", new[] { "Bread", "Chocolate Cake", "Cookie" /*Cookies?*/ });
            AddGrabberItem("George", "Stone", 35);
            AddGrabberItem("George", "Leek");
            AddGrabberItem("Gus", new[] { "Spaghetti", "Fish Taco", "Pancakes", "Bean Hotpot", "Vegetable Medley", "Omelet", "Baked Fish" });
            AddGrabberItem("Haley", new[] { "Coconut", "Sunflower", "Pink Cake" });
            AddGrabberItem("Harvey", new[] { "Coffee", "Muscle Remedy", "Energy Tonic" });
            AddGrabberItem("Jas", new[] { "Fairy Rose", "Clay", "Ancient Doll" });
            AddGrabberItem("Jodi", new[] { "Basic Fertilizer", "Quality Fertilizer", "Basic Retaining Soil" });
            AddGrabberItem("Kent", new[] { "Cherry Bomb", "Bomb", "Mega Bomb", "Battery Pack" });
            AddGrabberItem("Krobus", new[] { "Void Egg", "Wild Horseradish" });
            AddGrabberItem("Leah", new[] { "Salad", "Truffle" });
            AddGrabberItem("Leo", new[] { "Mango", "Ostrich Egg", "Duck Feather" });
            AddGrabberItem("Lewis", new[] { "Hot Pepper", "Green Tea" });
            AddGrabberItem("Linus", new[] { "Largemouth Bass", "Catfish", "Fried Calamari", "Sashimi", "Maki Roll" });
            AddGrabberItem("Marnie", new[] { "Hay" }, 30);
            AddGrabberItem("Marnie", new[] { "Farmer's Lunch", "Green Tea" });
            AddGrabberItem("Maru", new[] { "Battery Pack", "Iridium Bar" });
            AddGrabberItem("Pam", new[] { "Beer", "Energy Tonic", "Battery Pack", "Pale Ale", "Mead" });
            AddGrabberItem("Penny", new[] { "Melon", "Poppy" });
            AddGrabberItem("Pierre", new[] { "Fiber", "Grass Starter", "Daffodil", "Dandelion" });
            AddGrabberItem("Robin", new[] { "Stone", "Wood" }, 50);
            AddGrabberItem("Robin", "Hardwood");
            AddGrabberItem("Sam", new[] { "Cactus Fruit", "Pizza" });
            AddGrabberItem("Sandy", new[] { "Coconut", "Cactus Fruit", "Tom Kha Soup" });
            AddGrabberItem("Sebastian", new[] { "Frozen Tear", "Obsidian" });
            AddGrabberItem("Shane", new[] { "Pepper Poppers", "Pizza" });
            AddGrabberItem("Vincent", new[] { "Strange Doll", "Strange Doll (Green)", "Clay" });
            AddGrabberItem("Willy", new[] { "Bait" }, 25);
            AddGrabberItem("Willy", new[] { "Wild Bait" }, 5);
            AddGrabberItem("Willy", new[] { "Magic Bait" });
            AddGrabberItem("Willy", new[] { "Trout Soup", "Chub", "Carp" });
            AddGrabberItem("Wizard", new[] { "Purple Mushroom", "Fire Quartz", "Frozen Tear", "Jade" });
        }

        private void AddGrabberItem(string npc, IEnumerable<string> items, int amount = 1)
        {
            foreach (var item in items)
            {
                AddGrabberItem(npc, item, amount);
            }
        }

        private void AddGrabberItem(string npc, string item, int amount = 1)
        {
            if (!GrabberItems.ContainsKey(npc))
            {
                GrabberItems.Add(npc, new Dictionary<StardewObject, int>());
            }

            if (string.IsNullOrWhiteSpace(item))
            {
                return;
            }

            GrabberItems[npc].Add(_itemManager.GetObjectByName(item), amount);
        }
    }
}
