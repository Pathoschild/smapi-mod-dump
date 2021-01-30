/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bpendragon/GreenhouseSprinklers
**
*************************************************/

using System;
using System.Collections.Generic;

namespace Bpendragon.GreenhouseSprinklers.Data
{
    public enum SprinklerType
    {
        Basic = 599,
        Quality = 621,
        Iridium = 645
    }

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Custom
    }

    class ModConfig
    {
        public Difficulty SelectedDifficulty { get; set; } = Difficulty.Medium;
        public bool ShowVisualUpgrades { get; set; } = true;
        public bool WaterSandOnBeachFarm { get; set; } = true;
        public int MaxNumberOfUpgrades { get; set; } = 3;

        public List<UpgradeCost> DifficultySettings = new List<UpgradeCost>()
        {
            new UpgradeCost(
                new SingleUpgradeCost(SprinklerType.Basic, 5, 10000, 0, 0),
                new SingleUpgradeCost(SprinklerType.Quality, 5, 15000, 1, 0),
                new SingleUpgradeCost(SprinklerType.Iridium, 10, 25000, 5, 0),
                Difficulty.Easy
            ),

             new UpgradeCost(
               new SingleUpgradeCost(SprinklerType.Quality, 5 , 20000, 1, 2),
               new SingleUpgradeCost(SprinklerType.Iridium, 5 , 30000, 5, 5),
               new SingleUpgradeCost(SprinklerType.Iridium, 20, 50000, 10, 10),
               Difficulty.Medium
           ),

            new UpgradeCost(
               new SingleUpgradeCost(SprinklerType.Iridium, 5, 30000, 5, 5),
               new SingleUpgradeCost(SprinklerType.Iridium, 10, 35000, 20, 10),
               new SingleUpgradeCost(SprinklerType.Iridium, 25, 70000, 25, 10),
               Difficulty.Hard
           ),
            new UpgradeCost(
               new SingleUpgradeCost(SprinklerType.Quality, 5 , 20000, 1, 2),
               new SingleUpgradeCost(SprinklerType.Iridium, 5 , 30000, 5, 5),
               new SingleUpgradeCost(SprinklerType.Iridium, 20, 50000, 10, 10),
               Difficulty.Custom
           ),
        };
    }

    class SingleUpgradeCost
    {
        public SprinklerType Sprinkler { get; set; }
        public int SprinklerCount { get; set; }
        public int Gold { get; set; }
        public int Batteries { get; set; }
        public int Hearts { get; set; }

        public SingleUpgradeCost(SprinklerType SprinklerType, int NumSprinklers, int GoldAmount, int NumBatteries, int Hearts)
        {
            Sprinkler = SprinklerType;
            SprinklerCount = NumSprinklers;
            Gold = GoldAmount;
            Batteries = NumBatteries;
            this.Hearts = Hearts;
        }
    }

    class UpgradeCost
    {
        public Difficulty Difficulty { get; set; }
        public SingleUpgradeCost FirstUpgrade { get; set; }
        public SingleUpgradeCost SecondUpgrade { get; set; }
        public SingleUpgradeCost FinalUpgrade { get; set; }

        public UpgradeCost(SingleUpgradeCost FirstUpgrade, SingleUpgradeCost SecondUpgrade, SingleUpgradeCost FinalUpgrade, Difficulty difficulty)
        {
            this.FirstUpgrade = FirstUpgrade;
            this.SecondUpgrade = SecondUpgrade;
            this.FinalUpgrade = FinalUpgrade;
            Difficulty = difficulty;
        }

    }
}
