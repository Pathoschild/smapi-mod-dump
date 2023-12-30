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
using StardewArchipelago.Archipelago;
using StardewArchipelago.Stardew;

namespace StardewArchipelago.Items.Traps
{
    public class TrapDifficultyBalancer
    {
        public Dictionary<TrapItemsDifficulty, BuffDuration> FrozenDebuffDurations = new()
        {
            {TrapItemsDifficulty.NoTraps, BuffDuration.Zero},
            {TrapItemsDifficulty.Easy, BuffDuration.TenMinutes},
            {TrapItemsDifficulty.Medium, BuffDuration.HalfHour},
            {TrapItemsDifficulty.Hard, BuffDuration.OneHour},
            {TrapItemsDifficulty.Hell, BuffDuration.TwoHours},
            {TrapItemsDifficulty.Nightmare, BuffDuration.WholeDay},
        };

        public Dictionary<TrapItemsDifficulty, BuffDuration> DefaultDebuffDurations = new()
        {
            {TrapItemsDifficulty.NoTraps, BuffDuration.Zero},
            {TrapItemsDifficulty.Easy, BuffDuration.HalfHour},
            {TrapItemsDifficulty.Medium, BuffDuration.OneHour},
            {TrapItemsDifficulty.Hard, BuffDuration.TwoHours},
            {TrapItemsDifficulty.Hell, BuffDuration.FourHours},
            {TrapItemsDifficulty.Nightmare, BuffDuration.WholeDay},
        };

        public Dictionary<TrapItemsDifficulty, double> TaxRates = new()
        {
            {TrapItemsDifficulty.NoTraps, 0},
            {TrapItemsDifficulty.Easy, 0.1},
            {TrapItemsDifficulty.Medium, 0.2},
            {TrapItemsDifficulty.Hard, 0.4},
            {TrapItemsDifficulty.Hell, 0.8},
            {TrapItemsDifficulty.Nightmare, 1},
        };

        // TODO: Figure out a way to have different difficulties to random teleports
        public Dictionary<TrapItemsDifficulty, TeleportDestination> TeleportDestinations = new()
        {
            {TrapItemsDifficulty.NoTraps, TeleportDestination.None},
            {TrapItemsDifficulty.Easy, TeleportDestination.Nearby},
            {TrapItemsDifficulty.Medium, TeleportDestination.SameMap},
            {TrapItemsDifficulty.Hard, TeleportDestination.SameMapOrHome},
            {TrapItemsDifficulty.Hell, TeleportDestination.PelicanTown},
            {TrapItemsDifficulty.Nightmare, TeleportDestination.Anywhere},
        };


        public Dictionary<TrapItemsDifficulty, double> CrowAttackRate = new()
        {
            {TrapItemsDifficulty.NoTraps, 0},
            {TrapItemsDifficulty.Easy, 0.05},
            {TrapItemsDifficulty.Medium, 0.1},
            {TrapItemsDifficulty.Hard, 0.25},
            {TrapItemsDifficulty.Hell, 0.60},
            {TrapItemsDifficulty.Nightmare, 1},
        };


        public Dictionary<TrapItemsDifficulty, CrowTargets> CrowValidTargets = new()
        {
            {TrapItemsDifficulty.NoTraps, CrowTargets.None},
            {TrapItemsDifficulty.Easy, CrowTargets.Farm},
            {TrapItemsDifficulty.Medium, CrowTargets.Outside},
            {TrapItemsDifficulty.Hard, CrowTargets.Outside},
            {TrapItemsDifficulty.Hell, CrowTargets.Everywhere},
            {TrapItemsDifficulty.Nightmare, CrowTargets.Everywhere},
        };

        public const double SCARECROW_EFFICIENCY = 0.40;


        public Dictionary<TrapItemsDifficulty, int> NumberOfMonsters = new()
        {
            {TrapItemsDifficulty.NoTraps, 0},
            {TrapItemsDifficulty.Easy, 1},
            {TrapItemsDifficulty.Medium, 2},
            {TrapItemsDifficulty.Hard, 4},
            {TrapItemsDifficulty.Hell, 8},
            {TrapItemsDifficulty.Nightmare, 12},
        };

        // TODO: Figure out a way to have different difficulties to entrance reshuffle
        //public Dictionary<TrapItemsDifficulty, > EntranceReshuffles = new()
        //{
        //    {TrapItemsDifficulty.NoTraps, },
        //    {TrapItemsDifficulty.Easy, },
        //    {TrapItemsDifficulty.Medium, },
        //    {TrapItemsDifficulty.Hard, },
        //    {TrapItemsDifficulty.Hell, },
        //    {TrapItemsDifficulty.Nightmare, },
        //};

        public Dictionary<TrapItemsDifficulty, int> AmountOfDebris = new()
        {
            {TrapItemsDifficulty.NoTraps, 0},
            {TrapItemsDifficulty.Easy, 20},
            {TrapItemsDifficulty.Medium, 50},
            {TrapItemsDifficulty.Hard, 200},
            {TrapItemsDifficulty.Hell, 400},
            {TrapItemsDifficulty.Nightmare, 800},
        };

        public Dictionary<TrapItemsDifficulty, ShuffleInventoryTarget> ShuffleInventoryTargets = new()
        {
            {TrapItemsDifficulty.NoTraps, ShuffleInventoryTarget.None},
            {TrapItemsDifficulty.Easy, ShuffleInventoryTarget.Hotbar},
            {TrapItemsDifficulty.Medium, ShuffleInventoryTarget.FullInventory},
            {TrapItemsDifficulty.Hard, ShuffleInventoryTarget.FullInventory},
            {TrapItemsDifficulty.Hell, ShuffleInventoryTarget.InventoryAndChests},
            {TrapItemsDifficulty.Nightmare, ShuffleInventoryTarget.InventoryAndChestsAndFriends},
        };

        // TODO: Figure out a way to have different difficulties for temporary winter
        //public Dictionary<TrapItemsDifficulty, > EntranceReshuffles = new()
        //{
        //    {TrapItemsDifficulty.NoTraps, },
        //    {TrapItemsDifficulty.Easy, },
        //    {TrapItemsDifficulty.Medium, },
        //    {TrapItemsDifficulty.Hard, },
        //    {TrapItemsDifficulty.Hell, },
        //    {TrapItemsDifficulty.Nightmare, },
        //};


        public Dictionary<TrapItemsDifficulty, int> PariahFriendshipLoss = new()
        {
            {TrapItemsDifficulty.NoTraps, -0},
            {TrapItemsDifficulty.Easy, -10},
            {TrapItemsDifficulty.Medium, -20},
            {TrapItemsDifficulty.Hard, -40},
            {TrapItemsDifficulty.Hell, -100},
            {TrapItemsDifficulty.Nightmare, -400},
        };


        public Dictionary<TrapItemsDifficulty, DroughtTarget> DroughtTargets = new()
        {
            {TrapItemsDifficulty.NoTraps, DroughtTarget.None},
            {TrapItemsDifficulty.Easy, DroughtTarget.Soil},
            {TrapItemsDifficulty.Medium, DroughtTarget.Crops},
            {TrapItemsDifficulty.Hard, DroughtTarget.CropsIncludingInside},
            {TrapItemsDifficulty.Hell, DroughtTarget.CropsIncludingInside},
            {TrapItemsDifficulty.Nightmare, DroughtTarget.CropsIncludingWateringCan},
        };

        public Dictionary<TrapItemsDifficulty, TimeFliesDuration> TimeFliesDurations = new()
        {
            {TrapItemsDifficulty.NoTraps, TimeFliesDuration.Zero},
            {TrapItemsDifficulty.Easy, TimeFliesDuration.OneHour},
            {TrapItemsDifficulty.Medium, TimeFliesDuration.TwoHours},
            {TrapItemsDifficulty.Hard, TimeFliesDuration.SixHours},
            {TrapItemsDifficulty.Hell, TimeFliesDuration.TwelveHours},
            {TrapItemsDifficulty.Nightmare, TimeFliesDuration.TwoDays},
        };

        public Dictionary<TrapItemsDifficulty, int> NumberOfBabies = new()
        {
            {TrapItemsDifficulty.NoTraps, 0},
            {TrapItemsDifficulty.Easy, 4},
            {TrapItemsDifficulty.Medium, 8},
            {TrapItemsDifficulty.Hard, 16},
            {TrapItemsDifficulty.Hell, 32},
            {TrapItemsDifficulty.Nightmare, 128},
        };

        public Dictionary<TrapItemsDifficulty, int> MeowBarkNumber = new()
        {
            {TrapItemsDifficulty.NoTraps, 0},
            {TrapItemsDifficulty.Easy, 4},
            {TrapItemsDifficulty.Medium, 8},
            {TrapItemsDifficulty.Hard, 16},
            {TrapItemsDifficulty.Hell, 32},
            {TrapItemsDifficulty.Nightmare, 128},
        };

        public Dictionary<TrapItemsDifficulty, int> DepressionTrapDays = new()
        {
            {TrapItemsDifficulty.NoTraps, 0},
            {TrapItemsDifficulty.Easy, 2},
            {TrapItemsDifficulty.Medium, 3},
            {TrapItemsDifficulty.Hard, 7},
            {TrapItemsDifficulty.Hell, 14},
            {TrapItemsDifficulty.Nightmare, 28},
        };

        public Dictionary<TrapItemsDifficulty, int> UngrowthDays = new()
        {
            {TrapItemsDifficulty.NoTraps, 0},
            {TrapItemsDifficulty.Easy, 1},
            {TrapItemsDifficulty.Medium, 2},
            {TrapItemsDifficulty.Hard, 4},
            {TrapItemsDifficulty.Hell, 8},
            {TrapItemsDifficulty.Nightmare, 14},
        };

        public Dictionary<TrapItemsDifficulty, int> TreeUngrowthDays = new()
        {
            {TrapItemsDifficulty.NoTraps, 0},
            {TrapItemsDifficulty.Easy, 2},
            {TrapItemsDifficulty.Medium, 4},
            {TrapItemsDifficulty.Hard, 7},
            {TrapItemsDifficulty.Hell, 21},
            {TrapItemsDifficulty.Nightmare, 56},
        };

        public Dictionary<TrapItemsDifficulty, double> InflationAmount = new()
        {
            {TrapItemsDifficulty.NoTraps, 0},
            {TrapItemsDifficulty.Easy, 1.2},
            {TrapItemsDifficulty.Medium, 1.4}, // Vanilla Inflation at Clint's after a year is equivalent to 2 traps
            {TrapItemsDifficulty.Hard, 2.25}, // Vanilla Inflation at Robin's after a year is equivalent to 2 traps
            {TrapItemsDifficulty.Hell, 3.5},
            {TrapItemsDifficulty.Nightmare, 5},
        };

        public Dictionary<TrapItemsDifficulty, int> ExplosionSize = new()
        {
            {TrapItemsDifficulty.NoTraps, 0},
            {TrapItemsDifficulty.Easy, 1},
            {TrapItemsDifficulty.Medium, 3}, // Cherry Bomb
            {TrapItemsDifficulty.Hard, 5}, // Bomb
            {TrapItemsDifficulty.Hell, 7}, // Mega Bomb
            {TrapItemsDifficulty.Nightmare, 15}, // Good luck!
        };
    }

    public enum TeleportDestination
    {
        None,
        Nearby,
        SameMap,
        SameMapOrHome,
        PelicanTown,
        Anywhere,
    }

    public enum CrowTargets
    {
        None,
        Farm,
        Outside,
        Everywhere,
    }

    public enum ShuffleInventoryTarget
    {
        None,
        Hotbar,
        FullInventory,
        InventoryAndChests,
        InventoryAndChestsAndFriends,
    }

    public enum DroughtTarget
    {
        None = 0,
        Soil = 1,
        Crops = 2,
        CropsIncludingInside = 3,
        CropsIncludingWateringCan = 4,
    }

    public enum TimeFliesDuration
    {
        Zero = 0,
        OneHour = 6,
        TwoHours = 12,
        SixHours = 36,
        TwelveHours = 72,
        TwoDays = 240,
    }
}
