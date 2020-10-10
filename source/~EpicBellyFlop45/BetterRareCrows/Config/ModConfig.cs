/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

namespace BetterRarecrows.Config
{
    /// <summary>The mod configuration.</summary>
    public class ModConfig
    {
        /// <summary>The number of distinct rarecrows the player needs to have placed on their farm.</summary>
        public int NumberOfRequiredRareCrows { get; set; } = 8;

        /// <summary>Whether each placed rarecrow should slightly lower the chances of crops from spawning.</summary>
        public bool EnableProgressiveMode { get; set; } = true;

        /// <summary>The chance that each rarecrow has at stopping crows coming, with progressive mode (At a chance of 10, 5 rarecrows would sheild the farm 50% or time etc).</summary>
        public int ProgressivePercentPerRarecrow { get; set; } = 10;
    }
}
