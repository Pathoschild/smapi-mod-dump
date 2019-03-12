using Newtonsoft.Json;
using StardewModdingAPI;
using System.Collections.Generic;

namespace DailyTasksReport
{
    internal class ModConfig
    {
        /// <summary> The input to open the report. </summary>
        public SButton OpenReportKey { get; set; } = SButton.Y;

        /// <summary> The input to open the settings menu. </summary>
        public SButton OpenSettings { get; set; } = SButton.None;

        /// <summary> The input to toggle the display of bubble notifications. </summary>
        public SButton ToggleBubbles { get; set; } = SButton.None;

        /// <summary> Show the open report button below quests button. </summary>
        public bool DisplayReportButton { get; set; } = true;

        /// <summary> Display bubbles for items in the report that normally does not display bubbles. </summary>
        public bool DisplayBubbles { get; set; } = false;

        /// <summary> Show detailed info on the next pages. </summary>
        public bool ShowDetailedInfo { get; set; } = true;

        /// <summary> Report if there is a new recipe being aired on the queen of sauce. </summary>
        public bool NewRecipeOnTv { get; set; } = true;

        /// <summary> Report if it's someone's birthday and the farmer did not sent a gift. </summary>
        public bool Birthdays { get; set; } = true;

        /// <summary> Report if the traveling merchant is in town and was not visited yet. </summary>
        public bool TravelingMerchant { get; set; } = true;

        /// <summary> Check or not for unwatered crops in farm and greenhouse. </summary>
        public bool UnwateredCrops { get; set; } = true;

        /// <summary> Check or not for unharvested crops in farm and greenhouse. </summary>
        public bool UnharvestedCrops { get; set; } = true;

        /// <summary> Check or not if there are dead crops. </summary>
        public bool DeadCrops { get; set; } = true;

        /// <summary> Report fruits in fruit trees starting from this number. </summary>
        public int FruitTrees { get; set; } = 1;

        /// <summary> Check or not for if you petted your pet. </summary>
        public bool UnpettedPet { get; set; } = true;

        /// <summary> Check or not if your pet's bowl is filled with water. </summary>
        public bool UnfilledPetBowl { get; set; } = true;

        /// <summary> Check or not for unpetted animals in your farm. </summary>
        public bool UnpettedAnimals { get; set; } = true;

        /// <summary> Check or not if that are animal products ready to collect. </summary>
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Auto)]
        public Dictionary<string, bool> AnimalProducts { get; set; } = new Dictionary<string, bool>
        {
            {"Cow milk", true},
            {"Goat milk", true},
            {"Sheep wool", true},
            {"Chicken egg", true},
            {"Dinosaur egg", true},
            {"Duck egg", true},
            {"Duck feather", true},
            {"Rabbit's wool", true},
            {"Rabbit's foot", true},
            {"Truffle", true},
            {"Slime ball", true}
        };

        /// <summary> Check or not if the feeding benches are not full. </summary>
        public bool MissingHay { get; set; } = true;

        /// <summary> Check or not if there is something in the farm cave. </summary>
        public bool FarmCave { get; set; } = true;

        /// <summary> Check or not if you have uncollected crabpots. </summary>
        public bool UncollectedCrabpots { get; set; } = true;

        /// <summary> Check or not if you have not baited crabpots. </summary>
        public bool NotBaitedCrabpots { get; set; } = true;

        /// <summary> Check or not if there are BigCraftables (tapper, machines, ...) ready to collect. </summary>
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Auto)]
        public Dictionary<string, bool> Machines { get; set; } = new Dictionary<string, bool>
        {
            {"Bee House", true},
            {"Charcoal Kiln", true},
            {"Cheese Press", true},
            {"Crystalarium", true},
            {"Furnace", true},
            {"Keg", true},
            {"Lightning Rod", true},
            {"Loom", true},
            {"Mayonnaise Machine", true},
            {"Oil Maker", true},
            {"Preserves Jar", true},
            {"Recycling Machine", true},
            {"Seed Maker", true},
            {"Slime Egg-Press", true},
            {"Soda Machine", true},
            {"Statue Of Endless Fortune", true},
            {"Statue Of Perfection", true},
            {"Tapper", true},
            {"Worm Bin", true}
        };

        // <summary> Product quality to check in casks. </summary>
        public int Cask { get; set; } = 4;

        /// <summary> Draw bubbles for unwatered crops. </summary>
        public bool DrawBubbleUnwateredCrops { get; set; } = true;

        /// <summary> Draw bubbles for unharvested crops. </summary>
        public bool DrawBubbleUnharvestedCrops { get; set; } = true;

        /// <summary> Draw bubbles for dead crops. </summary>
        public bool DrawBubbleDeadCrops { get; set; } = true;

        /// <summary> Draw bubble if pet was not petted. </summary>
        public bool DrawBubbleUnpettedPet { get; set; } = true;

        /// <summary> Draw bubbles for unpetted animals. </summary>
        public bool DrawBubbleUnpettedAnimals { get; set; } = true;

        /// <summary> Draw bubbles for animals with produce. </summary>
        public bool DrawBubbleAnimalsWithProduce { get; set; } = true;

        /// <summary> Draw bubbles for buildings with produce inside. </summary>
        public bool DrawBubbleBuildingsWithProduce { get; set; } = true;

        /// <summary> Draw bubbles for buildings with any hay missing. </summary>
        public bool DrawBubbleBuildingsMissingHay { get; set; } = true;

        /// <summary> Draw bubbles for truffles </summary>
        public bool DrawBubbleTruffles { get; set; } = true;

        /// <summary> Draw bubbles for unbaited crabpots. </summary>
        public bool DrawBubbleCrabpotsNotBaited { get; set; } = true;

        /// <summary> Draw bubbles for cask if configuration for lower quality items. </summary>
        public bool DrawBubbleCask { get; set; } = true;

        internal bool Check(IMonitor monitor)
        {
            var changed = false;

            if (FruitTrees < 0 || FruitTrees > 3)
            {
                monitor.Log("Wrong configuration for Fruit Trees, setting to 3 fruits...", LogLevel.Warn);
                FruitTrees = 3;
                changed = true;
            }

            if (Cask < 0 || Cask > 4)
            {
                monitor.Log("Wrong configuration for Casks, setting to iridium quality...", LogLevel.Warn);
                Cask = 4;
                changed = true;
            }

            if (AnimalProducts.TryGetValue("Rabit's wool", out var enabled))
            {
                AnimalProducts.Remove("Rabit's wool");
                AnimalProducts["Rabbit's wool"] = enabled;
                changed = true;
            }

            if (AnimalProducts.TryGetValue("Rabit's foot", out enabled))
            {
                AnimalProducts.Remove("Rabit's foot");
                AnimalProducts["Rabbit's foot"] = enabled;
                changed = true;
            }

            return changed;
        }

        internal bool ProductFromAnimal(int produceIndex)
        {
            return produceIndex > 0 && AnimalProducts[LookupProductFromAnimal[produceIndex]];
        }

        internal bool ProductToCollect(int objectIndex)
        {
            return AnimalProducts[LookupProductToCollect[objectIndex]];
        }

        private static readonly Dictionary<int, string> LookupProductFromAnimal = new Dictionary<int, string>
        {
            {184, "Cow milk"},
            {186, "Cow milk"},
            {436, "Goat milk"},
            {438, "Goat milk"},
            {440, "Sheep wool"}
        };

        private static readonly Dictionary<int, string> LookupProductToCollect = new Dictionary<int, string>
        {
            {174, "Chicken egg"},
            {176, "Chicken egg"},
            {180, "Chicken egg"},
            {182, "Chicken egg"},
            {107, "Dinosaur egg"},
            {442, "Duck egg"},
            {444, "Duck feather"},
            {440, "Rabbit's wool"},
            {446, "Rabbit's foot"},
            {430, "Truffle"},
            {56, "Slime ball"},
            {57, "Slime ball"},
            {58, "Slime ball"},
            {59, "Slime ball"},
            {60, "Slime ball"},
            {61, "Slime ball"}
        };
    }
}