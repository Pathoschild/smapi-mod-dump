/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace BetterRarecrows.Config
{
    /// <summary>The mod configuration.</summary>
    public class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The number of distinct rarecrows the player needs to have placed on their farm for crows to stop spawning.</summary>
        public int NumberOfRequiredRareCrows { get; set; } = 8;

        /// <summary>Whether each placed rarecrow should slightly lower the chances of crows spawning (when the current number of rarecrows is less than <see cref="NumberOfRequiredRareCrows"/>).</summary>
        public bool EnableProgressiveMode { get; set; } = true;

        /// <summary>The chance that each rarecrow has at stopping crows spawning, with progressive mode (e.g. at a chance of 10, 5 rarecrows would stop crows spawning 50% or time).</summary>
        public float ProgressivePercentPerRarecrow { get; set; } = 10;
    }
}
