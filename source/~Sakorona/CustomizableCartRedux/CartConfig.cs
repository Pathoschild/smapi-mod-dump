using System.Collections.Generic;

namespace CustomizableTravelingCart
{
    public class CartConfig
    {
        public double MondayChance { get; set; }
        public double TuesdayChance { get; set; }
        public double WednesdayChance { get; set; }
        public double ThursdayChance { get; set; }
        public double FridayChance { get; set; }
        public double SaturdayChance { get; set; }
        public double SundayChance { get; set; }

        public bool AppearOnlyAtStartOfSeason { get; set; }
        public bool AppearOnlyAtEndOfSeason { get; set; }
        public bool AppearOnlyAtStartAndEndOfSeason { get; set; }
        public bool AppearOnlyEveryOtherWeek { get; set; }

        public bool UseVanillaMax { get; set; }
        public int AmountOfItems { get; set; }
        public bool UseCheaperPricing { get; set; }
        public List<int> AllowedItems { get; set; }
        public List<int> BlacklistedItems { get; set; }
        public int OpeningTime { get; set; }
        public int ClosingTime { get; set; }
        public CartConfig()
        {
            AppearOnlyAtEndOfSeason = false;
            AppearOnlyAtStartAndEndOfSeason = false;
            AppearOnlyAtStartOfSeason = false;
            AppearOnlyEveryOtherWeek = false;
            BlacklistedItems = new List<int>();
            AllowedItems = new List<int>();
            UseCheaperPricing = false;
            UseVanillaMax = true;
            AmountOfItems = 12;

            OpeningTime = 0600;
            ClosingTime = 2000;

            MondayChance = .2;
            TuesdayChance = .2;
            WednesdayChance = .2;
            ThursdayChance = .2;
            FridayChance = 1;
            SaturdayChance = .4;
            SundayChance = 1;
        }
    }
}
