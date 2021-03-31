/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace ImJustMatt.SlimeFramework.Framework.Models
{
    internal class SlimeModel
    {
        public int Health { get; set; }
        public int DamageToFarmer { get; set; }
        public int MinCoinsToDrop { get; set; }
        public int MaxCoinsToDrop { get; set; }

        public int DurationOfRandomMovements { get; set; } = 1000;

        // ObjectsToDrop
        // ExtraDropItems
        public int Resilience { get; set; }
        public double Jitteriness { get; set; } = .01;
        public int MoveTowardPlayer { get; set; } = 4;
        public int Speed { get; set; } = 2;
        public double MissChance { get; set; } = 0;
        public int ExperienceGained { get; set; }
        public string DisplayName { get; set; }

        public int Slipperiness { get; set; }

        // Color
        // Egg
    }
}