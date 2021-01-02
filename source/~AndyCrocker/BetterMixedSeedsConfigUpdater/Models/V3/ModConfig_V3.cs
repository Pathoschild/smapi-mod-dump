/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace BetterMixedSeedsConfigUpdater.Models.V3
{
    /// <summary>The mod configuration.</summary>
    public class ModConfig_V3
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The percent chance that a mixed seed is dropped when cutting weeds (when fiber isn't dropped).</summary>
        public float PercentDropChanceForMixedSeedsWhenNotFiber { get; set; } = 5;

        /// <summary>Whether mixed seeds can only plant seeds if the seed's year requirement is met.</summary>
        public bool UseSeedYearRequirement { get; set; } = true;

        /// <summary>Whether trellis crops can be planted.</summary>
        public bool EnableTrellisCrops { get; set; } = true;

        /// <summary>The seed config for the base game crops.</summary>
        public CropMod_V3 StardewValley { get; set; }

        /// <summary>The seed config for each currently installed crop mod.</summary>
        /// <remarks>Key: UniqueId of mod that added it, Value: crops the mod adds.</remarks>
        public Dictionary<string, CropMod_V3> CropModSettings { get; set; } = new Dictionary<string, CropMod_V3>();
    }
}
