using System.Collections.Generic;
using StardewModdingAPI;

namespace LeFauxMatt.HelpForHire.Models
{
    public class ModConfig
    {
        /// <summary>Charge at a rate per work done.</summary>
        public bool PayPerUnit { get; set; } = true;

        /// <summary>The chores that will be available for purchase.</summary>
        public IDictionary<string, int> Chores { get; set; } = new Dictionary<string, int>();

        /// <summary>The button used to activate the shop menu.</summary>
        public SButton ShopMenuButton { get; set; } = SButton.P;

        public ModConfig()
        {
            Instance = this;

            Chores.Add("furyx639.FeedTheAnimals", 25);
            Chores.Add("furyx639.LoveThePets", 250);
            Chores.Add("furyx639.PetTheAnimals", 50);
            Chores.Add("furyx639.RepairTheFences", 10);
            Chores.Add("furyx639.WaterTheCrops", 10);
            Chores.Add("furyx639.WaterTheSlimes", 250);
        }

        public static ModConfig Instance { get; private set; }
    }
}
