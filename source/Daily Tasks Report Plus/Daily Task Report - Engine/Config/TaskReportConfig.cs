/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/DailyTaskReportPlus
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyTasksReport
{

    [Serializable]
    public class TaskReportConfig
    {
        /// <summary> The input to open the report. </summary>
        public SButton OpenReportKey { get; set; } = SButton.P;       /// <summary> The input to toggle the display of bubble notifications. </summary>
        public SButton ToggleBubbles { get; set; } = SButton.Multiply;
        /// <summary> Show the open report button below quests button. </summary>
        public bool DisplayReportButton { get; set; } = true;

        /// <summary> The input to open the settings menu. </summary>
        public SButton OpenSettings { get; set; } = SButton.Subtract;
        /// <summary> Display bubbles for items in the report that normally does not display bubbles. </summary>
        public bool DisplayBubbles { get; set; } = true;

        public bool BounceBubbles {  get; set; } = true;
        public bool DrawBubbleUnpettedAnimals { get; set; } = true;
        public bool DrawBubbleTruffles { get; set; } = true;
        public bool DrawBubbleAnimalsWithProduce { get; set; } = true;
        public bool DrawBubbleBuildingsMissingHay { get; set; } = true;
        public bool DrawBubbleBuildingsWithProduce { get; set; } = true;
        public bool DrawBubbleDeadCrops { get; set; } = true;
        public bool DrawBubbleUnharvestedCrops { get; set; } = true;
        public bool DrawBubbleUnwateredCrops { get; set; } = true;
        public bool DrawBubbleCask { get; set; } = true;
        public bool DrawBubbleCrabpotsNotBaited { get; set; } = true;
        public bool DrawBubbleUnpettedPet { get; set; } = true;
        public bool SkipFlowersInHarvest { get; set; } = true;
        public bool FlowerReportLastDay { get; set; } = true;
        public bool ShowDetailedInfo { get; set; } = true;
        public bool Birthdays { get; set; } = true;
        public bool NewRecipeOnTv { get; set; } = true;
        public bool TravelingMerchant { get; set; } = true;
        public bool UnpettedAnimals { get; set; } = true;
        public bool UnwateredCrops { get; set; } = true;
        public bool UnharvestedCrops { get; set; } = true;
        public bool DeadCrops { get; set; } = true;
        public int FruitTrees { get; set; } = 3;
        public bool MissingHay { get; set; } = true;
        public bool UnpettedPet { get; set; } = true;
        public bool UnfilledPetBowl { get; set; } = true;
        public bool FarmCave { get; set; } = true;
        public bool UncollectedCrabpots { get; set; } = true;
        public bool NotBaitedCrabpots { get; set; } = true;
        public bool BrokenFences { get; set; } = true;
        public bool PondsNeedingAttention { get; set; } = true;
        public bool PondsWithItems { get; set; } = true;
        public int SiloThreshold { get; set; } = 100;
        // <summary> Product quality to check in casks. </summary>
        public int Cask { get; set; } = 3;
        internal bool ProductToCollect(string objectIndex)
        {
            return AnimalProducts[LookupProductToCollect[objectIndex]];
        }
        internal bool ProductFromAnimal(string produceIndex)
        {
            if (LookupProductFromAnimal.ContainsKey(produceIndex))
            {
                return !string.IsNullOrEmpty(produceIndex) && AnimalProducts[LookupProductFromAnimal[produceIndex]];
            }
            else
            {
                return false;
            }
        }


        private static readonly Dictionary<string, string> LookupProductFromAnimal = new Dictionary<string, string>
        {
            {"184", "Cow milk"},
            {"186", "Cow milk"},
            {"436", "Goat milk"},
            {"438", "Goat milk"},
            {"440", "Sheep wool"}
        };
        private static readonly Dictionary<string, string> LookupProductToCollect = new Dictionary<string, string>
        {
            {"174", "Chicken egg"},
            {"176", "Chicken egg"},
            {"180", "Chicken egg"},
            {"182", "Chicken egg"},
            {"107", "Dinosaur egg"},
            {"442", "Duck egg"},
            {"444", "Duck feather"},
            {"440", "Rabbit's wool"},
            {"446", "Rabbit's foot"},
            {"430", "Truffle"},
            {"56", "Slime ball"},
            {"57", "Slime ball"},
            {"58", "Slime ball"},
            {"59", "Slime ball"},
            {"60", "Slime ball"},
            {"61", "Slime ball"},
            {"289", "Ostrich Egg" }
        };
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
            {"Slime ball", true},
            {"Ostrich Egg",true }
        };
        public Dictionary<string, bool> Machines { get; set; } = new Dictionary<string, bool>
        {
            {"Bee House", true},
            {"Charcoal Kiln", true},
            {"Cheese Press", true},
            {"Crystalarium", true},
            {"Dehydrator",true },
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

    }
}
