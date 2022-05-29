/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Services
{
    partial class FishingApi
    {
        /// <inheritdoc/>
        public override void RequestReload()
        {
            // Lazily reload the fishing content
            this.reloadRequested = true;
        }

        private void ReloadIfRequested()
        {
            // Check if a reload was requested
            if (!this.reloadRequested)
            {
                return;
            }

            this.reloadRequested = false;

            // Reset fishing data
            foreach (var manager in this.fishingEffectManagers)
            {
                manager.Effect.UnapplyAll();
            }

            this.fishTraits.Clear();
            this.fishManagers.Clear();
            this.trashManagers.Clear();
            this.treasureManagers.Clear();
            this.fishingEffectManagers.Clear();

            // Create new fishing data
            var newFishTraits =
                new Dictionary<NamespacedKey, (IManifest sourceMod, FishTraits traits)>();
            var newFishEntries = new List<(IManifest sourceMod, FishEntry fish)>();
            var newTrashEntries = new List<(IManifest sourceMod, TrashEntry trash)>();
            var newTreasureEntries = new List<(IManifest sourceMod, TreasureEntry treasure)>();

            // Reload fishing data topologically
            var contentSources = this.contentSourcesFactory();
            var remainingContent =
                new Queue<FishingContent>(contentSources.SelectMany(source => source.Reload()));
            var unloadedByModId = remainingContent.GroupBy(content => content.ModManifest.UniqueID)
                .ToDictionary(group => group.Key, group => group.Count());
            while (remainingContent.TryDequeue(out var content))
            {
                // Get dependencies (including TFO if it's not TFO)
                var dependencies = content.ModManifest.Dependencies
                    .Select(dependency => dependency.UniqueID)
                    .ToHashSet();
                if (content.ModManifest.UniqueID != this.manifest.UniqueID)
                {
                    dependencies.Add(this.manifest.UniqueID);
                }

                // Check if dependencies are loaded
                var areDependenciesLoaded = dependencies.All(
                    dependency => !unloadedByModId.TryGetValue(dependency, out var unloaded)
                        || unloaded <= 0
                );
                if (!areDependenciesLoaded)
                {
                    // Add back to queue
                    remainingContent.Enqueue(content);
                    continue;
                }

                // Update unloaded count
                unloadedByModId[content.ModManifest.UniqueID]--;

                // Remove fish traits
                foreach (var key in content.RemoveFishTraits)
                {
                    // Check if the traits are already loaded
                    if (newFishTraits.TryGetValue(key, out var oldEntry))
                    {
                        // Check if the old traits are from a dependency
                        if (dependencies.Contains(oldEntry.sourceMod.UniqueID))
                        {
                            // Remove the old traits
                            newFishTraits.Remove(key);
                        }
                        else
                        {
                            // Ignore the removed traits
                            this.monitor.Log(
                                $"Ignoring removed fish traits for {key} from {content.ModManifest.UniqueID} because they are loaded from {oldEntry.sourceMod.UniqueID} which is not a dependency.",
                                LogLevel.Warn
                            );
                            this.monitor.Log(
                                $"To override the old traits, add a dependency on {oldEntry.sourceMod.UniqueID} to {content.ModManifest.UniqueID} (even if it's an optional dependency).",
                                LogLevel.Warn
                            );
                        }
                    }
                    else
                    {
                        // Warn if no fish traits were matched
                        this.monitor.Log(
                            $"No fish traits were removed by a 'RemoveFishTraits' entry in {content.ModManifest.UniqueID} because no trait entries were registered for {key} and were added by a dependency of the mod.",
                            LogLevel.Warn
                        );
                    }
                }

                // Set fish traits
                foreach (var (key, newTraits) in content.SetFishTraits)
                {
                    // Check if the traits are already loaded
                    if (newFishTraits.TryGetValue(key, out var oldEntry))
                    {
                        // Check if the old traits are from a dependency
                        if (dependencies.Contains(oldEntry.sourceMod.UniqueID))
                        {
                            // Replace the old traits
                            newFishTraits[key] = (content.ModManifest, newTraits);
                        }
                        else
                        {
                            // Ignore the new traits
                            this.monitor.Log(
                                $"Ignoring fish traits for {key} from {content.ModManifest.UniqueID} because they are already loaded from {oldEntry.sourceMod.UniqueID}.",
                                LogLevel.Warn
                            );
                            this.monitor.Log(
                                $"To override the old traits, add a dependency on {oldEntry.sourceMod.UniqueID} to {content.ModManifest.UniqueID} (even if it's an optional dependency).",
                                LogLevel.Warn
                            );
                        }
                    }
                    else
                    {
                        // Add the new traits
                        newFishTraits.Add(key, (content.ModManifest, newTraits));
                    }
                }

                // Remove fish
                foreach (var filter in content.RemoveFish)
                {
                    // Select the fish that match the filter and are added by a dependency
                    var matchingFish = newFishEntries
                        .Where(
                            entry => filter.Matches(entry.fish)
                                && dependencies.Contains(entry.sourceMod.UniqueID)
                        )
                        .Select(entry => entry.fish)
                        .ToHashSet();

                    if (!matchingFish.Any())
                    {
                        // Warn if no fish were matched
                        this.monitor.Log(
                            $"No fish were removed by a 'RemoveFish' entry in {content.ModManifest.UniqueID} because no fish entries matched the filter and were added by a dependency of the mod.",
                            LogLevel.Warn
                        );
                    }
                    else
                    {
                        // Log the fish that were removed
                        this.monitor.Log(
                            $"{content.ModManifest.UniqueID} removed {matchingFish.Count} fish."
                        );
                    }

                    // Remove the fish
                    newFishEntries.RemoveAll(entry => matchingFish.Contains(entry.fish));
                }

                // Add fish
                newFishEntries.AddRange(
                    content.AddFish.Select(entry => (content.ModManifest, entry))
                );

                // Remove trash
                foreach (var filter in content.RemoveTrash)
                {
                    // Select the trash that match the filter and are added by a dependency
                    var matchingTrash = newTrashEntries
                        .Where(
                            entry => filter.Matches(entry.trash)
                                && dependencies.Contains(entry.sourceMod.UniqueID)
                        )
                        .Select(entry => entry.trash)
                        .ToHashSet();

                    if (!matchingTrash.Any())
                    {
                        // Warn if no trash were matched
                        this.monitor.Log(
                            $"No trash were removed by a 'RemoveTrash' entry in {content.ModManifest.UniqueID} because no trash entries matched the filter and were added by a dependency of the mod.",
                            LogLevel.Warn
                        );
                    }
                    else
                    {
                        // Log the trash that were removed
                        this.monitor.Log(
                            $"{content.ModManifest.UniqueID} removed {matchingTrash.Count} trash."
                        );
                    }

                    // Remove the trash
                    newTrashEntries.RemoveAll(entry => matchingTrash.Contains(entry.trash));
                }

                // Add trash
                newTrashEntries.AddRange(
                    content.AddTrash.Select(entry => (content.ModManifest, entry))
                );

                // Remove treasure
                foreach (var filter in content.RemoveTreasure)
                {
                    // Select the treasure that match the filter and are added by a dependency
                    var matchingTreasure = newTreasureEntries
                        .Where(
                            entry => filter.Matches(entry.treasure)
                                && dependencies.Contains(entry.sourceMod.UniqueID)
                        )
                        .Select(entry => entry.treasure)
                        .ToHashSet();

                    if (!matchingTreasure.Any())
                    {
                        // Warn if no treasure were matched
                        this.monitor.Log(
                            $"No treasure were removed by a 'RemoveTreasure' entry in {content.ModManifest.UniqueID} because no treasure entries matched the filter and were added by a dependency of the mod.",
                            LogLevel.Warn
                        );
                    }
                    else
                    {
                        // Log the treasure that were removed
                        this.monitor.Log(
                            $"{content.ModManifest.UniqueID} removed {matchingTreasure.Count} treasure."
                        );
                    }

                    // Remove the treasure
                    newTreasureEntries.RemoveAll(
                        entry => matchingTreasure.Contains(entry.treasure)
                    );
                }

                // Add treasure
                newTreasureEntries.AddRange(
                    content.AddTreasure.Select(entry => (content.ModManifest, entry))
                );

                // Add fishing effects
                this.fishingEffectManagers.AddRange(
                    content.AddEffects.Select(
                        entry => this.fishingEffectManagerFactory.Create(content.ModManifest, entry)
                    )
                );
            }

            // Update fishing data
            foreach (var (k, v) in newFishTraits)
            {
                this.fishTraits.Add(k, v.traits);
            }

            this.fishManagers.AddRange(
                newFishEntries.Select(
                    item => this.fishEntryManagerFactory.Create(item.sourceMod, item.fish)
                )
            );
            this.trashManagers.AddRange(
                newTrashEntries.Select(
                    item => this.trashEntryManagerFactory.Create(item.sourceMod, item.trash)
                )
            );
            this.treasureManagers.AddRange(
                newTreasureEntries.Select(
                    item => this.treasureEntryManagerFactory.Create(item.sourceMod, item.treasure)
                )
            );

            // Log the loaded content
            this.monitor.Log($"Loaded {this.fishTraits.Count} fish traits.", LogLevel.Info);
            this.monitor.Log($"Loaded {this.fishManagers.Count} fish.", LogLevel.Info);
            this.monitor.Log($"Loaded {this.trashManagers.Count} trash.", LogLevel.Info);
            this.monitor.Log($"Loaded {this.treasureManagers.Count} treasure.", LogLevel.Info);
            this.monitor.Log(
                $"Loaded {this.fishingEffectManagers.Count} fishing effects.",
                LogLevel.Info
            );
        }
    }
}
