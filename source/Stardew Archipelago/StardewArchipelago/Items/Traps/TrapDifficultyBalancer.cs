/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            {TrapItemsDifficulty.Hard, BuffDuration.TwoHours},
            {TrapItemsDifficulty.Hell, BuffDuration.FourHours},
            {TrapItemsDifficulty.Nightmare, BuffDuration.WholeDay},
        };

        public Dictionary<TrapItemsDifficulty, BuffDuration> DefaultDebuffDurations = new()
        {
            {TrapItemsDifficulty.NoTraps, BuffDuration.Zero},
            {TrapItemsDifficulty.Easy, BuffDuration.OneHour},
            {TrapItemsDifficulty.Medium, BuffDuration.TwoHours},
            {TrapItemsDifficulty.Hard, BuffDuration.FourHours},
            {TrapItemsDifficulty.Hell, BuffDuration.WholeDay},
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
            {TrapItemsDifficulty.Easy, 0.1},
            {TrapItemsDifficulty.Medium, 0.25},
            {TrapItemsDifficulty.Hard, 0.5},
            {TrapItemsDifficulty.Hell, 0.75},
            {TrapItemsDifficulty.Nightmare, 1},
        };


        public Dictionary<TrapItemsDifficulty, CrowTargets> CrowValidTargets = new()
        {
            {TrapItemsDifficulty.NoTraps, CrowTargets.None},
            {TrapItemsDifficulty.Easy, CrowTargets.Farm},
            {TrapItemsDifficulty.Medium, CrowTargets.Island},
            {TrapItemsDifficulty.Hard, CrowTargets.Island},
            {TrapItemsDifficulty.Hell, CrowTargets.Everywhere},
            {TrapItemsDifficulty.Nightmare, CrowTargets.Everywhere},
        };


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
            {TrapItemsDifficulty.Easy, 25},
            {TrapItemsDifficulty.Medium, 50},
            {TrapItemsDifficulty.Hard, 100},
            {TrapItemsDifficulty.Hell, 200},
            {TrapItemsDifficulty.Nightmare, 400},
        };

        public Dictionary<TrapItemsDifficulty, ShuffleInventoryTarget> ShuffleInventoryTargets = new()
        {
            {TrapItemsDifficulty.NoTraps, ShuffleInventoryTarget.None},
            {TrapItemsDifficulty.Easy, ShuffleInventoryTarget.Hotbar},
            {TrapItemsDifficulty.Medium, ShuffleInventoryTarget.FullInventory},
            {TrapItemsDifficulty.Hard, ShuffleInventoryTarget.FullInventory},
            {TrapItemsDifficulty.Hell, ShuffleInventoryTarget.InventoryAndChests},
            {TrapItemsDifficulty.Nightmare, ShuffleInventoryTarget.InventoryAndChests},
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
    }

    public enum TeleportDestination
    {
        None,
        Nearby,
        SameMap,
        SameMapOrHome,
        PelicanTown,
        Anywhere
    }

    public enum CrowTargets
    {
        None,
        Farm,
        Island,
        Everywhere,
    }

    public enum ShuffleInventoryTarget
    {
        None,
        Hotbar,
        FullInventory,
        InventoryAndChests,
    }

    public enum DroughtTarget
    {
        None = 0,
        Soil = 1,
        Crops = 2,
        CropsIncludingInside = 3,
        CropsIncludingWateringCan = 4
    }
}
