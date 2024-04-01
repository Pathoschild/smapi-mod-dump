/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using DecidedlyShared.Logging;
using Force.DeepCloner;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using TerrainFeatureRefresh.src.Framework;

namespace TerrainFeatureRefresh.Framework;

public class FeatureProcessor
{
    private TfrSettings settings;
    private ProcessorAction action;
    private GameLocation location;
    private GameLocation generatedLocation;
    private Logger logger;

    public FeatureProcessor(TfrSettings settings, ProcessorAction action, Logger logger)
    {
        this.settings = settings;
        this.logger = logger;
        this.action = action;

        // int i = "Remember to fix up current spawned objects count on each map after processing.";
    }

    public void Execute()
    {
        if (this.settings.AffectAllLocations.On)
        {
            foreach (GameLocation location in Game1.locations)
            {
                this.ProcessLocation(location);
            }
        }
        else
            this.ProcessLocation(Game1.currentLocation);
    }

    private void ProcessLocation(GameLocation location)
    {
        this.location = location;
        this.generatedLocation =
            new GameLocation(location.mapPath.Value, location.Name);

        this.logger.Log($"Removal settings: \n{this.settings.ToString()}", LogLevel.Trace);
        this.logger.Log($"Spawned objects prior to processing: {location.numberOfSpawnedObjectsOnMap}", LogLevel.Trace);

        this.DoFences();
        this.DoWeeds();
        this.DoTwigs();
        this.DoStones();
        this.DoForage();
        this.DoArtifactSpots();
        this.DoGrass();
        this.DoWildTrees();
        this.DoFruitTrees();
        this.DoPaths();
        this.DoHoeDirt();
        this.DoCrops();
        this.DoBushes();
        this.DoStumps();
        this.DoLogs();
        this.DoBoulders();
        this.DoMeteorites();

        this.logger.Log($"Spawned objects after processing: {location.numberOfSpawnedObjectsOnMap}", LogLevel.Trace);
    }

    private void LogRemoval(SObject obj)
    {
        this.logger.Log($"Removed {obj.Name}:{obj.DisplayName} from tile {obj.TileLocation} in {this.location.Name}.", LogLevel.Trace);
    }

    private void LogAddition(SObject obj, Vector2 tile)
    {
        this.logger.Log($"Adding {obj.Name}:{obj.DisplayName} to {tile} in {this.location.Name}.", LogLevel.Trace);
    }

    private void LogAddition(TerrainFeature tf, Vector2 tile)
    {
        this.logger.Log($"Adding TerrainFeature to {tile} in {this.location.Name}.", LogLevel.Trace);
    }

    private void LogRemoval(TerrainFeature tf)
    {
        this.logger.Log($"Removed TerrainFeature at tile {tf.Tile} in {this.location.Name}.", LogLevel.Trace);
    }

    private List<SObject> GetSObjects(GameLocation location, Func<SObject, bool> predicate)
    {
        List<SObject> objects = new List<SObject>();

        foreach (SObject obj in location.Objects.Values.Where(predicate))
            objects.Add(obj);

        return objects;
    }

    private List<TerrainFeature> GetTerrainFeatures(GameLocation location, Func<TerrainFeature, bool> predicate)
    {
        List<TerrainFeature> objects = new List<TerrainFeature>();

        foreach (TerrainFeature tf in location.terrainFeatures.Values.Where(predicate))
            objects.Add(tf);

        return objects;
    }

    private List<LargeTerrainFeature> GetLargeTerrainFeatures(GameLocation location, Func<LargeTerrainFeature, bool> predicate)
    {
        List<LargeTerrainFeature> objects = new List<LargeTerrainFeature>();

        foreach (LargeTerrainFeature tf in location.largeTerrainFeatures.Where(predicate))
            objects.Add(tf);

        return objects;
    }

    private List<ResourceClump> GetResourceClumps(GameLocation location, Func<ResourceClump, bool> predicate)
    {
        List<ResourceClump> objects = new List<ResourceClump>();

        foreach (ResourceClump rc in location.resourceClumps.Where(predicate))
            objects.Add(rc);

        return objects;
    }

    private int RemoveSObjects(GameLocation location, Func<SObject, bool> predicate)
    {
        List<SObject> objectsToDestroy = this.GetSObjects(this.location, predicate);

        // Now, we destroy.
        foreach (SObject obj in objectsToDestroy)
        {
            this.LogRemoval(obj);
            this.location.Objects.Remove(obj.TileLocation);

            if (obj.IsSpawnedObject)
                location.numberOfSpawnedObjectsOnMap--;
        }

        return objectsToDestroy.Count;
    }

    private void RemoveTerrainFeatures(GameLocation location, Func<TerrainFeature, bool> predicate)
    {
        List<TerrainFeature> terrainFeaturesToDestroy = this.GetTerrainFeatures(this.location, predicate);

        // Destroy.
        foreach (TerrainFeature tf in terrainFeaturesToDestroy)
        {
            this.LogRemoval(tf);
            this.location.terrainFeatures.Remove(tf.Tile);
        }
    }

    private void RemoveLargeTerrainFeatures(GameLocation location, Func<LargeTerrainFeature, bool> predicate)
    {
        List<LargeTerrainFeature> terrainFeaturesToDestroy = this.GetLargeTerrainFeatures(this.location, predicate);

        // Destroy.
        foreach (LargeTerrainFeature tf in terrainFeaturesToDestroy)
        {
            this.LogRemoval(tf);
            this.location.largeTerrainFeatures.Remove(tf);
        }
    }

    private void RemoveResourceClumps(GameLocation location, Func<ResourceClump, bool> predicate)
    {
        List<ResourceClump> resourceClumpsToDestroy = this.GetResourceClumps(this.location, predicate);

        // Destroy.
        foreach (ResourceClump rc in resourceClumpsToDestroy)
        {
            this.LogRemoval(rc);
            this.location.resourceClumps.Remove(rc);
        }
    }

    private void GenerateNewTerrainFeatures(GameLocation location, Func<TerrainFeature, bool> predicate)
    {
        // Now we copy over to the main location.
        foreach (TerrainFeature tf in this.generatedLocation.terrainFeatures.Values)
        {
            // If our predicate isn't matched, we don't care about this TerrainFeature.
            if (!predicate.Invoke(tf))
                continue;

            if (this.location.terrainFeatures.ContainsKey(tf.Tile))
                continue;

            this.LogAddition(tf, tf.Tile);
            this.location.terrainFeatures.Add(tf.Tile, tf);
        }
    }

    private void GenerateNewLargeTerrainFeatures(GameLocation location, Func<LargeTerrainFeature, bool> predicate)
    {
        // Now we copy over to the main location.
        foreach (LargeTerrainFeature tf in this.generatedLocation.largeTerrainFeatures)
        {
            // If our predicate isn't matched, we don't care about this SObject.
            if (!predicate.Invoke(tf))
                continue;

            if (this.location.largeTerrainFeatures.Contains(tf))
                continue;

            this.LogAddition(tf, tf.Tile);
            this.location.largeTerrainFeatures.Add(tf);
        }
    }

    private void GenerateNewResourceClumps(GameLocation location, Func<ResourceClump, bool> predicate)
    {
        // Now we copy over to the main location.
        foreach (ResourceClump rc in this.generatedLocation.resourceClumps)
        {
            // If our predicate isn't matched, we don't care about this SObject.
            if (!predicate.Invoke(rc))
                continue;

            if (this.location.resourceClumps.Contains(rc))
                continue;

            this.LogAddition(rc, rc.Tile);
            this.location.resourceClumps.Add(rc);
        }
    }

    private int GenerateNewSObjects(GameLocation location, Func<SObject, bool> predicate)
    {
        int spawnCount = default;

        // Now we copy over to the main location.
        foreach (SObject obj in this.generatedLocation.Objects.Values.Where(predicate))
        {
            // If our predicate isn't matched, we don't care about this SObject.
            if (!predicate.Invoke(obj))
                continue;

            if (this.location.Objects.ContainsKey(obj.TileLocation))
                continue;

            this.LogAddition(obj, obj.TileLocation);
            this.location.Objects.Add(obj.TileLocation, obj);

            if (obj.IsSpawnedObject)
                location.numberOfSpawnedObjectsOnMap++;

            spawnCount++;
        }

        return spawnCount;
    }

    #region SObjects

    private void DoFences()
    {
        if (this.settings.Fences.actionToTake == TfrAction.Ignore)
            return;

        if (this.action == ProcessorAction.Generate) // Fences just don't get generated, so we skip this.
            return;

        Func<SObject, bool> predicate = (SObject o) => o is Fence;

        this.RemoveSObjects(this.location, predicate);

    }

    private void DoWeeds()
    {
        if (this.settings.Weeds.actionToTake == TfrAction.Ignore)
            return;

        Func<SObject, bool> predicate = (SObject o) => o.Type.Equals("Litter") && o.Name.Equals("Weeds");

        // If we're regenerating or clearing, we strip the location.
        if (this.action == ProcessorAction.ClearOnly || this.action == ProcessorAction.Regenerate)
            this.RemoveSObjects(this.location, predicate);

        // And, if we're generating or regenerating, we generate and copy over new items.
        if (this.action == ProcessorAction.Generate || this.action == ProcessorAction.Regenerate)
            this.GenerateNewSObjects(this.location, predicate);
    }

    private void DoTwigs()
    {
        if (this.settings.Twigs.actionToTake == TfrAction.Ignore)
            return;

        Func<SObject, bool> predicate = (SObject o) => o.Type.Equals("Litter") && o.Name.Equals("Twig");

        // If we're regenerating or clearing, we strip the location.
        if (this.action == ProcessorAction.ClearOnly || this.action == ProcessorAction.Regenerate)
            this.RemoveSObjects(this.location, predicate);

        // And, if we're generating or regenerating, we generate and copy over new items.
        if (this.action == ProcessorAction.Generate || this.action == ProcessorAction.Regenerate)
            this.GenerateNewSObjects(this.location, predicate);
    }

    private void DoStones()
    {
        if (this.settings.Stones.actionToTake == TfrAction.Ignore)
            return;


        Func<SObject, bool> predicate = (SObject o) => o.Type.Equals("Litter") && o.Name.Equals("Stone");

        // If we're regenerating or clearing, we strip the location.
        if (this.action == ProcessorAction.ClearOnly || this.action == ProcessorAction.Regenerate)
            this.RemoveSObjects(this.location, predicate);

        // And, if we're generating or regenerating, we generate and copy over new items.
        if (this.action == ProcessorAction.Generate || this.action == ProcessorAction.Regenerate)
            this.GenerateNewSObjects(this.location, predicate);

    }

    private void DoForage()
    {
        if (this.settings.Forage.actionToTake == TfrAction.Ignore)
            return;

        Func<SObject, bool> predicate = (SObject o) => o.IsSpawnedObject;
        int removalCount;
        int spawnCount;

        // If we're regenerating or clearing, we strip the location.
        if (this.action == ProcessorAction.ClearOnly || this.action == ProcessorAction.Regenerate)
            removalCount = this.RemoveSObjects(this.location, predicate);

        // And, if we're generating or regenerating, we generate and copy over new items.
        if (this.action == ProcessorAction.Generate || this.action == ProcessorAction.Regenerate)
            spawnCount = this.GenerateNewSObjects(this.location, predicate);
    }

    private void DoArtifactSpots()
    {
        if (this.settings.ArtifactSpots.actionToTake == TfrAction.Ignore)
            return;

        Func<SObject, bool> predicate = (SObject o) => o.Name.Equals("Artifact Spot");

        // If we're regenerating or clearing, we strip the location.
        if (this.action == ProcessorAction.ClearOnly || this.action == ProcessorAction.Regenerate)
            this.RemoveSObjects(this.location, predicate);

        // And, if we're generating or regenerating, we generate and copy over new items.
        if (this.action == ProcessorAction.Generate || this.action == ProcessorAction.Regenerate)
            this.GenerateNewSObjects(this.location, predicate);
    }

    #endregion

    #region TerrainFeatures

    private void DoGrass()
    {
        if (this.settings.Grass.actionToTake == TfrAction.Ignore)
            return;


        Func<TerrainFeature, bool> predicate = (TerrainFeature o) => o is Grass;
        // If we're regenerating or clearing, we strip the location.
        if (this.action == ProcessorAction.ClearOnly || this.action == ProcessorAction.Regenerate)
            this.RemoveTerrainFeatures(this.location, predicate);

        // And, if we're generating or regenerating, we generate and copy over new items.
        if (this.action == ProcessorAction.Generate || this.action == ProcessorAction.Regenerate)
            this.GenerateNewTerrainFeatures(this.location, predicate);
    }

    private void DoWildTrees()
    {
        if (this.settings.WildTrees.actionToTake == TfrAction.Ignore)
            return;


        Func<TerrainFeature, bool> predicate = (TerrainFeature o) => o is Tree && (o is not FruitTree);
        // If we're regenerating or clearing, we strip the location.
        if (this.action == ProcessorAction.ClearOnly || this.action == ProcessorAction.Regenerate)
            this.RemoveTerrainFeatures(this.location, predicate);

        // And, if we're generating or regenerating, we generate and copy over new items.
        if (this.action == ProcessorAction.Generate || this.action == ProcessorAction.Regenerate)
            this.GenerateNewTerrainFeatures(this.location, predicate);

    }

    private void DoFruitTrees()
    {
        if (this.settings.FruitTrees.actionToTake == TfrAction.Ignore)
            return;


        Func<TerrainFeature, bool> predicate = (TerrainFeature o) => o is FruitTree;
        // If we're regenerating or clearing, we strip the location.
        if (this.action == ProcessorAction.ClearOnly || this.action == ProcessorAction.Regenerate)
            this.RemoveTerrainFeatures(this.location, predicate);

        // And, if we're generating or regenerating, we generate and copy over new items.
        if (this.action == ProcessorAction.Generate || this.action == ProcessorAction.Regenerate)
            this.GenerateNewTerrainFeatures(this.location, predicate);

    }

    private void DoPaths()
    {
        if (this.settings.Paths.actionToTake == TfrAction.Ignore)
            return;


        Func<TerrainFeature, bool> predicate = (TerrainFeature o) => o is Flooring;
        // If we're regenerating or clearing, we strip the location.
        if (this.action == ProcessorAction.ClearOnly || this.action == ProcessorAction.Regenerate)
            this.RemoveTerrainFeatures(this.location, predicate);

        // And, if we're generating or regenerating, we generate and copy over new items.
        if (this.action == ProcessorAction.Generate || this.action == ProcessorAction.Regenerate)
            this.GenerateNewTerrainFeatures(this.location, predicate);

    }

    private void DoHoeDirt()
    {
        if (this.settings.HoeDirt.actionToTake == TfrAction.Ignore)
            return;


        Func<TerrainFeature, bool> predicate = (TerrainFeature o) => o is HoeDirt hoeDirt && (hoeDirt.crop is null);
        // If we're regenerating or clearing, we strip the location.
        if (this.action == ProcessorAction.ClearOnly || this.action == ProcessorAction.Regenerate)
            this.RemoveTerrainFeatures(this.location, predicate);

        // And, if we're generating or regenerating, we generate and copy over new items.
        if (this.action == ProcessorAction.Generate || this.action == ProcessorAction.Regenerate)
            this.GenerateNewTerrainFeatures(this.location, predicate);

    }

    private void DoCrops()
    {
        if (this.settings.Crops.actionToTake == TfrAction.Ignore)
            return;

        // For crops, I think I want to give the player back any seeds/fertiliser that was in the soil?

        Func<TerrainFeature, bool> predicate = (TerrainFeature o) => o is HoeDirt hoeDirt && (hoeDirt.crop is not null);
        // If we're regenerating or clearing, we strip the location.
        if (this.action == ProcessorAction.ClearOnly || this.action == ProcessorAction.Regenerate)
            this.RemoveTerrainFeatures(this.location, predicate);

        // And, if we're generating or regenerating, we generate and copy over new items.
        if (this.action == ProcessorAction.Generate || this.action == ProcessorAction.Regenerate)
            this.GenerateNewTerrainFeatures(this.location, predicate);
    }

    private void DoBushes()
    {
        if (this.settings.Bushes.actionToTake == TfrAction.Ignore)
            return;


        Func<LargeTerrainFeature, bool> predicate = (LargeTerrainFeature o) => o is Bush;
        // If we're regenerating or clearing, we strip the location.
        if (this.action == ProcessorAction.ClearOnly || this.action == ProcessorAction.Regenerate)
            this.RemoveLargeTerrainFeatures(this.location, predicate);

        // And, if we're generating or regenerating, we generate and copy over new items.
        if (this.action == ProcessorAction.Generate || this.action == ProcessorAction.Regenerate)
            this.GenerateNewLargeTerrainFeatures(this.location, predicate);

    }

    #endregion

    #region ResourceClumps

    private void DoStumps()
    {
        if (this.settings.Stumps.actionToTake == TfrAction.Ignore)
            return;


        Func<ResourceClump, bool> predicate = (ResourceClump o) => o.parentSheetIndex.Equals(600);
        // If we're regenerating or clearing, we strip the location.
        if (this.action == ProcessorAction.ClearOnly || this.action == ProcessorAction.Regenerate)
            this.RemoveResourceClumps(this.location, predicate);

        // And, if we're generating or regenerating, we generate and copy over new items.
        if (this.action == ProcessorAction.Generate || this.action == ProcessorAction.Regenerate)
            this.GenerateNewResourceClumps(this.location, predicate);

    }

    private void DoLogs()
    {
        if (this.settings.Logs.actionToTake == TfrAction.Ignore)
            return;


        Func<ResourceClump, bool> predicate = (ResourceClump o) => o.parentSheetIndex.Equals(602);
        // If we're regenerating or clearing, we strip the location.
        if (this.action == ProcessorAction.ClearOnly || this.action == ProcessorAction.Regenerate)
            this.RemoveResourceClumps(this.location, predicate);

        // And, if we're generating or regenerating, we generate and copy over new items.
        if (this.action == ProcessorAction.Generate || this.action == ProcessorAction.Regenerate)
            this.GenerateNewResourceClumps(this.location, predicate);

    }

    private void DoBoulders()
    {
        if (this.settings.Boulders.actionToTake == TfrAction.Ignore)
            return;


        Func<ResourceClump, bool> predicate = (ResourceClump o) => o.parentSheetIndex.Equals(672);
        // If we're regenerating or clearing, we strip the location.
        if (this.action == ProcessorAction.ClearOnly || this.action == ProcessorAction.Regenerate)
            this.RemoveResourceClumps(this.location, predicate);

        // And, if we're generating or regenerating, we generate and copy over new items.
        if (this.action == ProcessorAction.Generate || this.action == ProcessorAction.Regenerate)
            this.GenerateNewResourceClumps(this.location, predicate);

    }

    private void DoMeteorites()
    {
        if (this.settings.Meteorites.actionToTake == TfrAction.Ignore)
            return;


        Func<ResourceClump, bool> predicate = (ResourceClump o) => o.parentSheetIndex.Equals(622);
        // If we're regenerating or clearing, we strip the location.
        if (this.action == ProcessorAction.ClearOnly || this.action == ProcessorAction.Regenerate)
            this.RemoveResourceClumps(this.location, predicate);

        // And, if we're generating or regenerating, we generate and copy over new items.
        if (this.action == ProcessorAction.Generate || this.action == ProcessorAction.Regenerate)
            this.GenerateNewResourceClumps(this.location, predicate);

    }

    #endregion
}
