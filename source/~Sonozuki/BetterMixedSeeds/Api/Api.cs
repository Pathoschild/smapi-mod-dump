/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BetterMixedSeeds
{
    /// <summary>Provides basic crop apis.</summary>
    public class Api : IApi
    {
        /*********
        ** Public Methods
        *********/
        /// <inheritdoc/>
        public string[] GetExcludedCrops() => ModEntry.Instance.CropsToExclude.ToArray();

        /// <inheritdoc/>
        public void ForceExcludeCrop(params string[] cropNames)
        {
            if (!Context.IsWorldReady)
                throw new InvalidOperationException("Cannot exclued crops when a save isn't loaded.");

            if (cropNames == null || cropNames.Length == 0)
                return;

            ModEntry.Instance.Monitor.Log($"A mod has forcibly excluded: {string.Join(", ", cropNames)}");
            ModEntry.Instance.CropsToExclude.AddRange(cropNames);

            // reload the seeds to refelect newly excluded crops
            ModEntry.Instance.LoadEnabledSeeds();
        }

        /// <inheritdoc/>
        public string[] ReincludeCrop(params string[] cropNames)
        {
            if (!Context.IsWorldReady)
                throw new InvalidOperationException("Cannot reinclude crops when a save isn't loaded.");

            if (cropNames == null || cropNames.Length == 0)
                return new string[0];

            var failedToRemoveCrops = new List<string>();

            // try to reinclude specified crops
            foreach (var cropName in cropNames)
            {
                var numberOfRemovedEntries = ModEntry.Instance.CropsToExclude.RemoveAll(name => name == cropName);
                if (numberOfRemovedEntries == 0)
                    failedToRemoveCrops.Add(cropName);
            }

            // log the crops that were successfully reincluded
            var reincludedCrops = cropNames.Except(failedToRemoveCrops);
            if (reincludedCrops.Any())
                ModEntry.Instance.Monitor.Log($"A mod has forcibly excluded: {string.Join(", ", reincludedCrops)}");

            // reload the seeds to refelect newly included crops
            ModEntry.Instance.LoadEnabledSeeds();

            return failedToRemoveCrops.ToArray();
        }
    }
}
