/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/GreenhouseGatherers
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace GreenhouseGatherers.GreenhouseGatherers.Objects
{
    public class HarvestStatue
    {
        private IMonitor monitor = ModResources.GetMonitor();

        // Storage related
        private Chest chest;
        private GameLocation location;

        // Statue related
        public bool isFull = false;
        public bool harvestedToday = false;
        public bool hasSpawnedJunimos = false;
        public List<Vector2> harvestedTiles = new List<Vector2>();

        // Config related
        private bool enableHarvestMessage = true;
        private bool doJunimosEatExcessCrops = true;
        private bool doJunimosHarvestFromPots = true;
        private bool doJunimosHarvestFromFruitTrees = true;
        private bool doJunimosHarvestFromFlowers = true;
        private bool doJunimosSowSeedsAfterHarvest = true;
        private int minimumFruitOnTreeBeforeHarvest = 3;

        // Graphic related
        private int currentSheetIndex;

        public HarvestStatue()
        {

        }

        public HarvestStatue(Chest storage, GameLocation gameLocation)
        {
            chest = storage;
            location = gameLocation;
        }

        public void SpawnJunimos(int maxJunimosToSpawn = -1)
        {
            if (!harvestedToday || harvestedTiles.Count == 0)
            {
                return;
            }

            if (maxJunimosToSpawn == -1)
            {
                maxJunimosToSpawn = harvestedTiles.Count / 2;
            }

            int junimosToSpawnUpper = System.Math.Min(harvestedTiles.Count, maxJunimosToSpawn);
            for (int x = 0; x < Game1.random.Next(junimosToSpawnUpper / 4, junimosToSpawnUpper); x++)
            {
                Vector2 tile = location.getRandomTile();

                if (!location.isTileLocationTotallyClearAndPlaceable(tile) || !(location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Type", "Back") == "Wood" || location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Type", "Back") == "Stone"))
                {
                    continue;
                }

                Junimo j = new Junimo(tile * 64f, 6, false);
                if (!location.isCollidingPosition(j.GetBoundingBox(), Game1.viewport, j))
                {
                    location.characters.Add(j);
                }

                //monitor.Log($"Spawning some Junimos at {location.Name}: {tile.X}, {tile.Y}.", LogLevel.Debug);
            }
            Game1.playSound("tinyWhip");

            hasSpawnedJunimos = true;
        }

        public void HarvestCrops(GameLocation location, bool enableHarvestMessage = true, bool doJunimosEatExcessCrops = true, bool doJunimosHarvestFromPots = true, bool doJunimosHarvestFromFruitTrees = true, bool doJunimosHarvestFromFlowers = true, bool doJunimosSowSeedsAfterHarvest = false, int minimumFruitOnTreeBeforeHarvest = 3)
        {
            // Set configs
            this.enableHarvestMessage = enableHarvestMessage;
            this.doJunimosEatExcessCrops = doJunimosEatExcessCrops;
            this.doJunimosHarvestFromPots = doJunimosHarvestFromPots;
            this.doJunimosHarvestFromFruitTrees = doJunimosHarvestFromFruitTrees;
            this.doJunimosHarvestFromFlowers = doJunimosHarvestFromFlowers;
            this.doJunimosSowSeedsAfterHarvest = doJunimosSowSeedsAfterHarvest;
            this.minimumFruitOnTreeBeforeHarvest = minimumFruitOnTreeBeforeHarvest;

            string locationName = ModResources.SplitCamelCaseText(location.Name);
            // Check if we're at capacity and that Junimos aren't allowed to eat excess crops
            if (chest.items.Count >= chest.GetActualCapacity() && !doJunimosEatExcessCrops)
            {
                Game1.showRedMessage($"The Junimos at the {locationName} couldn't harvest due to lack of storage!");
                return;
            }

            // Look and harvest for crops & forage products on the ground
            monitor.Log("Searching for crops and forage products on ground...", LogLevel.Trace);
            SearchForGroundCrops(location);

            // Look and harvest for crops & forage products inside IndoorPots
            if (doJunimosHarvestFromPots)
            {
                monitor.Log("Searching for crops and forage products within Garden Pots...", LogLevel.Trace);
                SearchForIndoorPots(location);
            }

            if (doJunimosHarvestFromFruitTrees)
            {
                monitor.Log("Searching for fruits from Fruit Trees...", LogLevel.Trace);
                SearchForFruitTrees(location);
            }

            if (isFull)
            {
                Game1.showRedMessage($"The Junimos at the {locationName} couldn't harvest due to lack of storage!");
                return;
            }

            // Check if the Junimos ate the crops due to no inventory space
            if (bool.Parse(chest.modData[ModEntry.ateCropsFlag]))
            {
                Game1.showRedMessage($"The Junimos at the {locationName} ate harvested crops due to lack of storage!");
                return;
            }

            if (harvestedToday && enableHarvestMessage)
            {
                // Let the player know we harvested
                Game1.addHUDMessage(new HUDMessage($"The Junimos at the {locationName} have harvested crops.", 2));
                return;
            }
        }

        private bool HasRoomForHarvest()
        {
            if (chest.items.Count >= chest.GetActualCapacity() && !doJunimosEatExcessCrops)
            {
                return false;
            }

            return true;
        }

        private void AttemptSowSeed(int seedIndex, HoeDirt hoeDirt, Vector2 tile)
        {
            // -74 == Object.SeedsCategory
            Item seedItem = chest.items.FirstOrDefault(i => i != null && i.Category == -74 && i.ParentSheetIndex == seedIndex);
            if (seedItem != null)
            {
                // Remove one seed from the stack, or the whole item if it is the last seed of the stack
                seedItem.Stack -= 1;

                if (seedItem.Stack == 0)
                {
                    chest.items.Remove(seedItem);
                }

                // Plant the seed on the ground
                //hoeDirt.crop = new Crop(seedIndex, (int)tile.X, (int)tile.Y);
                hoeDirt.plant(seedIndex, (int)tile.X, (int)tile.Y, Game1.MasterPlayer, false, hoeDirt.currentLocation);
            }
        }

        private void SearchForGroundCrops(GameLocation location)
        {
            // Search for crops
            foreach (KeyValuePair<Vector2, TerrainFeature> tileToHoeDirt in location.terrainFeatures.Pairs.Where(p => p.Value is HoeDirt && (p.Value as HoeDirt).crop != null))
            {
                if (!HasRoomForHarvest())
                {
                    isFull = true;
                    return;
                }

                Vector2 tile = tileToHoeDirt.Key;
                HoeDirt hoeDirt = (tileToHoeDirt.Value as HoeDirt);

                Crop crop = hoeDirt.crop;
                if (!hoeDirt.readyForHarvest())
                {
                    // Crop is either not fully grown or it has not regrown since last harvest
                    //monitor.Log($"Crop at ({tile.X}, {tile.Y}) is not ready for harvesting: {crop.forageCrop} | {crop.regrowAfterHarvest} | {crop.dayOfCurrentPhase}, {crop.currentPhase}", LogLevel.Debug);
                    continue;
                }
                //monitor.Log($"Harvesting crop ({tile.X}, {tile.Y}): {crop.forageCrop} | {crop.regrowAfterHarvest} | {crop.dayOfCurrentPhase}, {crop.currentPhase}", LogLevel.Debug);

                if (!doJunimosHarvestFromFlowers && new Object(tile, crop.indexOfHarvest, 0).Category == -80)
                {
                    // Crop is flower and config has been set to skip them
                    continue;
                }

                // Crop exists and is fully grown, attempt to harvest it
                crop.harvest((int)tile.X, (int)tile.Y, hoeDirt, null);
                harvestedToday = true;
                harvestedTiles.Add(tile);

                // Clear any non-renewing crop
                if (crop.regrowAfterHarvest == -1)
                {
                    int seedIndex = crop.netSeedIndex;
                    hoeDirt.crop = null;

                    // Attempt to replant, if it is enabled and has valid seed
                    if (doJunimosSowSeedsAfterHarvest)
                    {
                        AttemptSowSeed(seedIndex, hoeDirt, tile);
                    }
                }
            }

            // Search for forage products
            List<Vector2> tilesToRemove = new List<Vector2>();
            foreach (KeyValuePair<Vector2, Object> tileToForage in location.objects.Pairs.Where(p => p.Value.isForage(location)))
            {
                if (!HasRoomForHarvest())
                {
                    isFull = true;
                    return;
                }

                Vector2 tile = tileToForage.Key;
                if (chest.addItem(tileToForage.Value.getOne()) != null)
                {
                    chest.modData[ModEntry.ateCropsFlag] = true.ToString();
                }

                tilesToRemove.Add(tile);
                harvestedToday = true;
                harvestedTiles.Add(tile);
            }

            // Clean up the harvested forage products
            tilesToRemove.ForEach(t => location.removeObject(t, false));
        }

        private void SearchForIndoorPots(GameLocation location)
        {
            // Search for IndoorPots with crops
            foreach (KeyValuePair<Vector2, Object> tileToIndoorPot in location.objects.Pairs.Where(p => p.Value is IndoorPot))
            {
                if (!HasRoomForHarvest())
                {
                    isFull = true;
                    return;
                }

                Vector2 tile = tileToIndoorPot.Key;
                IndoorPot pot = tileToIndoorPot.Value as IndoorPot;
                HoeDirt hoeDirt = pot.hoeDirt.Value;

                // HoeDirt seems to be missing its currentLocation when coming from IndoorPots, which is problematic for Crop.harvest()
                if (hoeDirt.currentLocation is null)
                {
                    hoeDirt.currentLocation = location;
                }

                if (hoeDirt.readyForHarvest())
                {
                    if (!doJunimosHarvestFromFlowers && new Object(tile, hoeDirt.crop.indexOfHarvest, 0).Category == -80)
                    {
                        // Crop is flower and config has been set to skip them
                        continue;
                    }

                    hoeDirt.crop.harvest((int)tile.X, (int)tile.Y, hoeDirt, null);
                    harvestedToday = true;

                    // Clear any non-renewing crop
                    if (hoeDirt.crop.regrowAfterHarvest == -1)
                    {
                        int seedIndex = hoeDirt.crop.netSeedIndex;
                        hoeDirt.crop = null;

                        // Attempt to replant, if it is enabled and has valid seed
                        if (doJunimosSowSeedsAfterHarvest)
                        {
                            AttemptSowSeed(seedIndex, hoeDirt, tile);
                        }
                    }
                }
            }

            // Search for IndoorPots with forage items
            foreach (KeyValuePair<Vector2, Object> tileToIndoorPot in location.objects.Pairs.Where(p => p.Value is IndoorPot))
            {
                if (!HasRoomForHarvest())
                {
                    isFull = true;
                    return;
                }

                Vector2 tile = tileToIndoorPot.Key;
                IndoorPot pot = tileToIndoorPot.Value as IndoorPot;

                if (pot.heldObject.Value != null && pot.heldObject.Value.isForage(location))
                {
                    if (chest.addItem(pot.heldObject.Value.getOne()) != null)
                    {
                        chest.modData[ModEntry.ateCropsFlag] = true.ToString();
                    }

                    pot.heldObject.Value = null;
                    harvestedToday = true;
                }
            }
        }

        private void SearchForFruitTrees(GameLocation location)
        {
            // Search for fruit trees
            if (minimumFruitOnTreeBeforeHarvest > 3)
            {
                minimumFruitOnTreeBeforeHarvest = 3;
            }

            foreach (KeyValuePair<Vector2, TerrainFeature> tileToFruitTree in location.terrainFeatures.Pairs.Where(p => p.Value is FruitTree && (p.Value as FruitTree).fruitsOnTree >= minimumFruitOnTreeBeforeHarvest).ToList())
            {
                if (!HasRoomForHarvest())
                {
                    isFull = true;
                    return;
                }

                Vector2 tile = tileToFruitTree.Key;
                FruitTree fruitTree = (tileToFruitTree.Value as FruitTree);

                // Determine fruit quality per the game's original code
                int fruitQuality = 0;
                if (fruitTree.daysUntilMature <= -112)
                {
                    fruitQuality = 1;
                }
                if (fruitTree.daysUntilMature <= -224)
                {
                    fruitQuality = 2;
                }
                if (fruitTree.daysUntilMature <= -336)
                {
                    fruitQuality = 4;
                }

                for (int j = 0; j < fruitTree.fruitsOnTree; j++)
                {
                    Vector2 offset = new Vector2(0f, 0f);
                    switch (j)
                    {
                        case 0:
                            offset.X = -64f;
                            break;
                        case 1:
                            offset.X = 64f;
                            offset.Y = -32f;
                            break;
                        case 2:
                            offset.Y = 32f;
                            break;
                    }

                    if (chest.addItem(new Object(fruitTree.indexOfFruit, 1, quality: fruitQuality)) != null)
                    {
                        chest.modData[ModEntry.ateCropsFlag] = true.ToString();
                    }
                }

                fruitTree.fruitsOnTree.Value = 0;

                harvestedToday = true;
            }
        }

        public void AddItems(NetObjectList<Item> items)
        {
            foreach (var item in items)
            {
                chest.addItem(item);
            }
        }
    }
}
