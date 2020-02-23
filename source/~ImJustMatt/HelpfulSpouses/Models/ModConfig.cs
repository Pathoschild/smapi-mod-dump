using System.Collections.Generic;

namespace LeFauxMatt.HelpfulSpouses.Models
{
    public class ModConfig
    {
        /// <summary>The maximum number of chores any spouse will perform in one day.</summary>
        public int DailyLimit { get; set; } = 1;

        /// <summary>The minimum number of hearts required for spouse to perform any chore.</summary>
        public int HeartsNeeded { get; set; } = 12;

        /// <summary>Overall chance that any chore gets performed.</summary>
        public double GlobalChance { get; set; } = 1;

        /// <summary>The spouses that will be able to perform chores.</summary>
        public IDictionary<string, IDictionary<string, double>> Spouses { get; set; } = new Dictionary<string, IDictionary<string, double>>();

        public ModConfig()
        {
            Instance = this;

            // Default Marriage Candidates (and Krobus)
            Spouses.Add("Alex", new Dictionary<string, double>()
            {
                { "furyx639.LoveThePets", 1 },
                { "furyx639.FeedTheAnimals", 0.6 },
                { "furyx639.PetTheAnimals", 0.6 },
            });

            Spouses.Add("Elliott", new Dictionary<string, double>()
            {
                { "furyx639.BirthdayGift", 0.6 },
                { "furyx639.MakeBreakfast", 0.7 },
                { "furyx639.WaterTheCrops", 0.6 },
            });

            Spouses.Add("Harvey", new Dictionary<string, double>()
            {
                { "furyx639.LoveThePets", 0.8 },
                { "furyx639.WaterTheCrops", 0.6 },
                { "furyx639.WaterTheSlimes", 0.6 },
            });

            Spouses.Add("Sam", new Dictionary<string, double>()
            {
                { "furyx639.BirthdayGift", 0.4 },
                { "furyx639.LoveThePets", 0.6 },
                { "furyx639.PetTheAnimals", 0.5 },
            });

            Spouses.Add("Sebastian", new Dictionary<string, double>()
            {
                { "furyx639.BirthdayGift", 0.4 },
                { "furyx639.LoveThePets", 0.6 },
                { "furyx639.WaterTheSlimes", 0.6 },
                });

            Spouses.Add("Shane", new Dictionary<string, double>()
            {
                { "furyx639.FeedTheAnimals", 0.8 },
                { "furyx639.LoveThePets", 1 },
                { "furyx639.PetTheAnimals", 0.8 },
            });

            Spouses.Add("Abigail", new Dictionary<string, double>()
            {
                { "furyx639.BirthdayGift", 0.5 },
                { "furyx639.PetTheAnimals", 0.7 },
                { "furyx639.WaterTheSlimes", 0.6 },
            });

            Spouses.Add("Emily", new Dictionary<string, double>()
            {
                { "furyx639.BirthdayGift", 0.8 },
                { "furyx639.MakeBreakfast", 0.8 },
                { "furyx639.PetTheAnimals", 0.8 },
            });

            Spouses.Add("Haley", new Dictionary<string, double>()
            {
                { "furyx639.BirthdayGift", 1.0 },
                { "furyx639.MakeBreakfast", 0.5 },
                { "furyx639.PetTheAnimals", 0.6 },
            });

            Spouses.Add("Leah", new Dictionary<string, double>()
            {
                { "furyx639.BirthdayGift", 0.7 },
                { "furyx639.RepairTheFences", 0.5 },
                { "furyx639.WaterTheCrops", 0.6 },
            });

            Spouses.Add("Maru", new Dictionary<string, double>()
            {
                { "furyx639.BirthdayGift", 0.7 },
                { "furyx639.RepairTheFences", 0.8 },
                { "furyx639.WaterTheCrops", 0.6 },
            });

            Spouses.Add("Penny", new Dictionary<string, double>()
            {
                { "furyx639.LoveThePets", 0.9 },
                { "furyx639.MakeBreakfast", 0.7 },
                { "furyx639.PetTheAnimals", 0.8 },
            });

            Spouses.Add("Krobus",new Dictionary<string, double>()
            {
                { "furyx639.MakeBreakfast", 0.7 },
                { "furyx639.WaterTheSlimes", 1 },
            });

            // Romanceable Rasmodius
            Spouses.Add("Wizard", new Dictionary<string, double>()
            {
                { "furyx639.BirthdayGift", 0.8 },
                { "furyx639.WaterTheCrops", 0.4 },
                { "furyx639.WaterTheSlimes", 0.7 },
            });

            // SVE Marriage Candidates (TBD)
            Spouses.Add("Olivia", new Dictionary<string, double>()
            {
                { "furyx639.BirthdayGift", 0.8 },
                { "furyx639.MakeBreakfast", 0.4 },
                { "furyx639.PetTheAnimals", 0.6 },
            });

            Spouses.Add("Victor", new Dictionary<string, double>()
            {
                { "furyx639.BirthdayGift", 0.8 },
                { "furyx639.PetTheAnimals", 0.6 },
                { "furyx639.WaterTheCrops", 0.4 },
            });

            Spouses.Add("Paul", new Dictionary<string, double>()
            {
                { "furyx639.BirthdayGift", 0.8 },
                { "furyx639.PetTheAnimals", 0.6 },
                { "furyx639.WaterTheCrops", 0.4 },
            });

            Spouses.Add("Sophia", new Dictionary<string, double>()
            {
                { "furyx639.PetTheAnimals", 0.7 },
                { "furyx639.RepairTheFences", 0.5 },
                { "furyx639.WaterTheCrops", 0.6 },
            });
        }

        public static ModConfig Instance { get; private set; }
    }
}
