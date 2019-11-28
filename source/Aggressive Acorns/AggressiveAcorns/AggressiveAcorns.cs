using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace AggressiveAcorns
{
    [UsedImplicitly]
    public class AggressiveAcorns : Mod
    {
        internal static IReflectionHelper ReflectionHelper;
        internal static IModConfig Config;


        private bool _manageTrees;

        private bool ManageTrees
        {
            get => _manageTrees;
            set
            {
                if (value == _manageTrees) return;

                Monitor.Log($"{(value ? "Started" : "Stopped")} watching for new trees/new areas.");
                _manageTrees = value;
            }
        }


        public override void Entry([NotNull] IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            ReflectionHelper = helper.Reflection;

            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.World.LocationListChanged += OnLocationListChanged;
            helper.Events.World.TerrainFeatureListChanged += OnTerrainFeatureListChanged;
            helper.Events.GameLoop.Saving += OnSaving;
        }


        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            Monitor.Log("Enraging trees in all available areas.");
            ReplaceTerrainFeatures<Tree, AggressiveTree>(EnrageTree, Common.Utilities.GetLocations(Helper));
            ManageTrees = true;
        }


        private void OnSaving(object sender, SavingEventArgs e)
        {
            ManageTrees = false;
            Monitor.Log("Calming trees in all available areas.");
            ReplaceTerrainFeatures<AggressiveTree, Tree>(CalmTree, Common.Utilities.GetLocations(Helper));
        }


        private void OnLocationListChanged(object sender, LocationListChangedEventArgs e)
        {
            if (!ManageTrees) return;
            Monitor.Log("Found new areas; enraging any trees.");
            ReplaceTerrainFeatures<Tree, AggressiveTree>(EnrageTree, e.Added);
        }


        private void OnTerrainFeatureListChanged(object sender, TerrainFeatureListChangedEventArgs e)
        {
            // NOTE: this causes changes to the terrain feature list, make sure that this doesn't get stuck forever.
            if (!ManageTrees) return;

            var toReplace = GetTerrainFeatures<Tree>(e.Added);
            if (!toReplace.Any()) return;

            var msg = ReplaceTerrainFeatures(EnrageTree, e.Location, toReplace);
            Monitor.Log("TerrainFeature list changed: " + msg);
        }


        [NotNull]
        private static AggressiveTree EnrageTree([NotNull] Tree tree)
        {
            return new AggressiveTree(tree);
        }


        [NotNull]
        private static Tree CalmTree([NotNull] AggressiveTree tree)
        {
            return tree.ToTree();
        }


        private void ReplaceTerrainFeatures<TOriginal, TReplacement>(
            Func<TOriginal, TReplacement> converter,
            [NotNull] IEnumerable<GameLocation> locations)
            where TReplacement : TerrainFeature
            where TOriginal : TerrainFeature
        {
            foreach (var location in locations)
            {
                var toReplace = GetTerrainFeatures<TOriginal>(location.terrainFeatures.Pairs);
                if (toReplace.Any())
                {
                    Monitor.Log(ReplaceTerrainFeatures(converter, location, toReplace));
                }
            }
        }


        [NotNull]
        private static IList<KeyValuePair<Vector2, T>> GetTerrainFeatures<T>(
            [NotNull] IEnumerable<KeyValuePair<Vector2, TerrainFeature>> items) where T : TerrainFeature
        {
            return items
                .Where(kvp => kvp.Value.GetType() == typeof(T))
                .Select(kvp => new KeyValuePair<Vector2, T>(kvp.Key, kvp.Value as T))
                .ToList();
        }


        [NotNull]
        private string ReplaceTerrainFeatures<TOriginal, TReplacement>(
            Func<TOriginal, TReplacement> converter,
            [NotNull] GameLocation location,
            [NotNull] ICollection<KeyValuePair<Vector2, TOriginal>> terrainFeatures)
            where TReplacement : TerrainFeature
            where TOriginal : class
        {
            foreach (var keyValuePair in terrainFeatures)
            {
                location.terrainFeatures[keyValuePair.Key] = converter(keyValuePair.Value);
            }

            return
                $"{location.Name} - replaced {terrainFeatures.Count} {typeof(TOriginal).Name} with {typeof(TReplacement).Name}.";
        }
    }
}
