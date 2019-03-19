using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace AggressiveAcorns {

    [UsedImplicitly]
    public class AggressiveAcorns : Mod {


        public override void Entry([NotNull] IModHelper helper) {
            ModConfig.Instance = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.Saved += OnSaved;
        }


        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e) {
            Monitor.Log("Loading trees on save load:", LogLevel.Trace);
            var locations = GetLocations();
            EnrageTrees(locations);
        }


        private void OnSaving(object sender, SavingEventArgs e) {
            Monitor.Log("Reverting trees to vanilla before serialization:", LogLevel.Trace);
            var locations = GetLocations();
            CalmTrees(locations);
        }


        private void OnSaved(object sender, SavedEventArgs e) {
            Monitor.Log("Reloading trees after save:", LogLevel.Trace);
            var locations = GetLocations();
            EnrageTrees(locations);
        }


        private void EnrageTrees([NotNull] IEnumerable<GameLocation> locations) {
            ReplaceTerrainFeature(locations, (Tree tree) => new AggressiveTree(tree));
        }


        private void CalmTrees([NotNull] IEnumerable<GameLocation> locations) {
            ReplaceTerrainFeature(locations, (AggressiveTree tree) => tree.ToTree());
        }


        private void ReplaceTerrainFeature<TOriginal, TReplacement>(
            [NotNull] IEnumerable<GameLocation> locations,
            Func<TOriginal, TReplacement> converter) where TReplacement : TerrainFeature where TOriginal : class {

            Monitor.Log($"Replacing terrain features: {typeof(TOriginal).FullName} -> {typeof(TReplacement).FullName}",
                LogLevel.Trace);

            foreach (var location in locations) {
                var oldFeatures = location.terrainFeatures.Pairs.Where(kvp => kvp.Value.GetType() == typeof(TOriginal));
                var count = 0;

                foreach (var keyValuePair in oldFeatures) {
                    var old = keyValuePair.Value as TOriginal;
                    var replacement = converter(old);
                    location.terrainFeatures[keyValuePair.Key] = replacement;
                    count++;
                }

                if (count > 0) {
                    Monitor.Log($"{location.Name} - replaced {count} {typeof(TOriginal).Name}(s).", LogLevel.Trace);
                }
            }
        }


        // From https://stardewvalleywiki.com/Modding:Common_tasks#Get_all_locations on 2019/03/16
        /// <summary>Get all game locations.</summary>
        [NotNull]
        private static IEnumerable<GameLocation> GetLocations() {
            return Game1.locations
                        .Concat(
                            from location in Game1.locations.OfType<BuildableGameLocation>()
                            from building in location.buildings
                            where building.indoors.Value != null
                            select building.indoors.Value
                        );
        }
    }

}