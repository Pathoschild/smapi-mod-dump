/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace AtraShared.Utils.Extensions
{
    public static class CropExtensions
    {
        /// <summary>
        /// Copied from the game - gets if a crop is harvestable.
        /// </summary>
        /// <param name="crop">Crop in question.</param>
        /// <returns>True if harvestable.</returns>
        public static bool IsActuallyFullyGrown(this Crop? crop)
            => crop is not null && crop.currentPhase.Value >= crop.phaseDays.Count - 1 && (!crop.fullyGrown.Value || crop.dayOfCurrentPhase.Value <= 0);
    }
}
