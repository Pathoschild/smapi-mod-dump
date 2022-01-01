/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Linq;
using ContentPatcher;
using StardewModdingAPI;
using TehPers.Core.Api.Extensions;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Services
{
    internal class ChanceCalculator<T>
        where T : AvailabilityInfo
    {
        private readonly T availabilityInfo;
        private readonly IManagedConditions? managedConditions;

        public ChanceCalculator(
            IMonitor monitor,
            IContentPatcherAPI contentPatcherApi,
            IManifest fishingManifest,
            IManifest owner,
            T availabilityInfo
        )
        {
            _ = monitor ?? throw new ArgumentNullException(nameof(monitor));
            _ = contentPatcherApi ?? throw new ArgumentNullException(nameof(contentPatcherApi));
            _ = owner ?? throw new ArgumentNullException(nameof(owner));
            this.availabilityInfo = availabilityInfo
                ?? throw new ArgumentNullException(nameof(availabilityInfo));

            if (availabilityInfo.When.Any())
            {
                // Get Content Patcher version
                var version =
                    fishingManifest.Dependencies.FirstOrDefault(
                            dependency => dependency.UniqueID == "Pathoschild.ContentPatcher"
                        )
                        ?.MinimumVersion
                    ?? throw new ArgumentException(
                        "Fishing overhaul does not depend on Content Patcher",
                        nameof(fishingManifest)
                    );

                // Parse conditions
                this.managedConditions = contentPatcherApi.ParseConditions(
                    owner,
                    availabilityInfo.When,
                    version,
                    new[] { fishingManifest.UniqueID }
                );

                // Check if conditions are valid
                if (!this.managedConditions.IsValid)
                {
                    // Log error
                    monitor.Log(
                        $"Failed to parse conditions for one of {owner.UniqueID}'s entries: {this.managedConditions.ValidationError}",
                        LogLevel.Error
                    );
                }
            }
        }

        /// <summary>
        /// Calculates the chance of catching this entry.
        /// </summary>
        /// <param name="fishingInfo">The fishing info to calculate the chance for.</param>
        /// <returns>The chance of catching this entry, or <see langword="null"/> if the entry is not available.</returns>
        public double? GetWeightedChance(FishingInfo fishingInfo)
        {
            return this.availabilityInfo.GetWeightedChance(fishingInfo)
                .Where(
                    _ =>
                    {
                        if (this.managedConditions is not { } conditions)
                        {
                            return true;
                        }

                        conditions.UpdateContext();
                        return conditions.IsMatch;
                    }
                );
        }
    }
}