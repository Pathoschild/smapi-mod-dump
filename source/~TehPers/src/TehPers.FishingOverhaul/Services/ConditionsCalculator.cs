/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using ContentPatcher;
using StardewModdingAPI;
using System;
using System.Linq;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Services
{
    internal class ConditionsCalculator
    {
        private const string contentPatcherId = "Pathoschild.ContentPatcher";

        private readonly AvailabilityConditions conditions;
        private readonly IManagedConditions? managedConditions;

        public ConditionsCalculator(
            IMonitor monitor,
            IContentPatcherAPI contentPatcherApi,
            IManifest fishingManifest,
            IManifest owner,
            AvailabilityConditions conditions
        )
        {
            _ = monitor ?? throw new ArgumentNullException(nameof(monitor));
            _ = contentPatcherApi ?? throw new ArgumentNullException(nameof(contentPatcherApi));
            _ = owner ?? throw new ArgumentNullException(nameof(owner));
            this.conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));

            // Check if any CP conditions exist
            if (!conditions.When.Any())
            {
                return;
            }

            // Get CP version
            var version =
                owner.Dependencies.FirstOrDefault(
                        dependency => string.Equals(
                            dependency.UniqueID,
                            ConditionsCalculator.contentPatcherId,
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    ?.MinimumVersion
                ?? fishingManifest.Dependencies.FirstOrDefault(
                        dependency => string.Equals(
                            dependency.UniqueID,
                            ConditionsCalculator.contentPatcherId,
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    ?.MinimumVersion
                ?? throw new ArgumentException(
                    "TFO does not depend on Content Patcher",
                    nameof(fishingManifest)
                );

            // Parse conditions
            this.managedConditions = contentPatcherApi.ParseConditions(
                owner,
                conditions.When,
                version,
                new[] {fishingManifest.UniqueID}
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

        /// <summary>
        /// Calculates whether the entry is available.
        /// </summary>
        /// <param name="fishingInfo">The fishing info to calculate the availability for.</param>
        /// <returns>Whether the entry is available.</returns>
        public bool IsAvailable(FishingInfo fishingInfo)
        {
            if (!this.conditions.IsAvailable(fishingInfo))
            {
                return false;
            }

            if (this.managedConditions is not { } managedConditions)
            {
                return true;
            }

            managedConditions.UpdateContext();
            return managedConditions.IsMatch;
        }
    }
}
