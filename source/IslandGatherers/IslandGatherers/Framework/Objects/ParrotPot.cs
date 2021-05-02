/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/IslandGatherers
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IslandGatherers.Framework.Objects
{
    public class ParrotPot : Chest
    {
        internal bool isFull = false;
        internal bool ateCrops = false;
        internal bool harvestedToday = false;
        internal bool hasSpawnedParrots = false;
        internal List<Vector2> harvestedTiles = new List<Vector2>();

        // Config related
        private bool _enableHarvestMessage = true;
        private bool _doParrotsEatExcessCrops = true;
        private bool _doParrotsHarvestFromPots = true;
        private bool _doParrotsHarvestFromFruitTrees = true;
        private bool _doParrotsHarvestFromFlowers = true;
        private bool _doParrotsSowSeedsAfterHarvest = true;
        private int _minimumFruitOnTreeBeforeHarvest = 3;

        public ParrotPot()
        {

        }

        public ParrotPot(Vector2 position, int itemID, bool enableHarvestMessage = true, bool doParrotsEatExcessCrops = true, bool doParrotsHarvestFromPots = true, bool doParrotsHarvestFromFruitTrees = true, bool doParrotsHarvestFromFlowers = true, bool doParrotsSowSeedsAfterHarvest = false, int minimumFruitOnTreeBeforeHarvest = 3) : base(true, position, itemID)
        {
            this.Name = "Parrot Pot";
            this.modData[IslandGatherers.parrotPotFlag] = System.String.Empty;

            this._enableHarvestMessage = enableHarvestMessage;
            this._doParrotsEatExcessCrops = doParrotsEatExcessCrops;
            this._doParrotsHarvestFromPots = doParrotsHarvestFromPots;
            this._doParrotsHarvestFromFruitTrees = doParrotsHarvestFromFruitTrees;
            this._doParrotsHarvestFromFlowers = doParrotsHarvestFromFlowers;
            this._doParrotsSowSeedsAfterHarvest = doParrotsSowSeedsAfterHarvest;
            this._minimumFruitOnTreeBeforeHarvest = minimumFruitOnTreeBeforeHarvest;

            base.type.Value = "Crafting";
            base.bigCraftable.Value = true;
            base.canBeSetDown.Value = true;

            // Setting SpecialChestType to -1 so we can bypass Automate's default chest logic
            // TODO: Make this only happen if Automate is installed
            this.SpecialChestType = (SpecialChestTypes)(-1);
        }

        public void AddItems(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                this.addItem(item);
            }
        }

        internal void HarvestCrops(GameLocation location)
        {
            // Check if we're at capacity and that Parrots aren't allowed to eat excess crops
            if (this.items.Count >= this.GetActualCapacity() && !_doParrotsEatExcessCrops)
            {
                Game1.showRedMessage($"The Parrots at the Ginger Island farm couldn't harvest due to lack of storage!");
                return;
            }

            // Look and harvest for crops & forage products on the ground
            IslandGatherers.monitor.Log("Searching for crops and forage products on ground...", LogLevel.Trace);
            SearchForGroundCrops(location);

            // Look and harvest for crops & forage products inside IndoorPots
            if (_doParrotsHarvestFromPots)
            {
                IslandGatherers.monitor.Log("Searching for crops and forage products within Garden Pots...", LogLevel.Trace);
                SearchForIndoorPots(location);
            }

            if (_doParrotsHarvestFromFruitTrees)
            {
                IslandGatherers.monitor.Log("Searching for fruits from Fruit Trees...", LogLevel.Trace);
                SearchForFruitTrees(location);
            }

            if (isFull)
            {
                Game1.showRedMessage($"The Parrots at the Ginger Island farm couldn't harvest due to lack of storage!");
                return;
            }

            // Check if the Parrots ate the crops due to no inventory space
            if (ateCrops)
            {
                Game1.showRedMessage($"The Parrots at the Ginger Island farm ate harvested crops due to lack of storage!");
                return;
            }

            if (harvestedToday && _enableHarvestMessage)
            {
                // Let the player know we harvested
                Game1.addHUDMessage(new HUDMessage($"The Parrots at the Ginger Island farm have harvested crops.", 2));
                return;
            }
        }

        private bool HasRoomForHarvest()
        {
            if (this.items.Count >= this.GetActualCapacity() && !_doParrotsEatExcessCrops)
            {
                return false;
            }

            return true;
        }

        private void AttemptSowSeed(int seedIndex, HoeDirt hoeDirt, Vector2 tile)
        {
            // -74 == Object.SeedsCategory
            Item seedItem = this.items.FirstOrDefault(i => i != null && i.Category == -74 && i.ParentSheetIndex == seedIndex);
            if (seedItem != null)
            {
                // Remove one seed from the stack, or the whole item if it is the last seed of the stack
                seedItem.Stack -= 1;

                if (seedItem.Stack == 0)
                {
                    this.items.Remove(seedItem);
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
                    //IslandGatherers.monitor.Log($"Crop at ({tile.X}, {tile.Y}) is not ready for harvesting: {crop.forageCrop} | {crop.regrowAfterHarvest} | {crop.dayOfCurrentPhase}, {crop.currentPhase}", LogLevel.Debug);
                    continue;
                }
                //IslandGatherers.monitor.Log($"Harvesting crop ({tile.X}, {tile.Y}): {crop.forageCrop} | {crop.regrowAfterHarvest} | {crop.dayOfCurrentPhase}, {crop.currentPhase}", LogLevel.Debug);

                if (!_doParrotsHarvestFromFlowers && new Object(tile, crop.indexOfHarvest, 0).Category == -80)
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
                    if (_doParrotsSowSeedsAfterHarvest)
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
                if (this.addItem(tileToForage.Value.getOne()) != null)
                {
                    ateCrops = true;
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
                    if (!_doParrotsHarvestFromFlowers && new Object(tile, hoeDirt.crop.indexOfHarvest, 0).Category == -80)
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
                        if (_doParrotsSowSeedsAfterHarvest)
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
                    if (this.addItem(pot.heldObject.Value.getOne()) != null)
                    {
                        ateCrops = true;
                    }

                    pot.heldObject.Value = null;
                    harvestedToday = true;
                }
            }
        }

        private void SearchForFruitTrees(GameLocation location)
        {
            // Search for fruit trees
            if (_minimumFruitOnTreeBeforeHarvest > 3)
            {
                _minimumFruitOnTreeBeforeHarvest = 3;
            }

            foreach (KeyValuePair<Vector2, TerrainFeature> tileToFruitTree in location.terrainFeatures.Pairs.Where(p => p.Value is FruitTree && (p.Value as FruitTree).fruitsOnTree >= _minimumFruitOnTreeBeforeHarvest))
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

                    if (this.addItem(new Object(fruitTree.indexOfFruit, 1, quality: fruitQuality)) != null)
                    {
                        ateCrops = true;
                    }
                }

                fruitTree.fruitsOnTree.Value = 0;

                harvestedToday = true;
            }
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            base.tileLocation.Value = new Vector2(x / 64, y / 64);
            return true;
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            float draw_x = x;
            float draw_y = y;
            if (this.localKickStartTile.HasValue)
            {
                draw_x = Utility.Lerp(this.localKickStartTile.Value.X, draw_x, this.kickProgress);
                draw_y = Utility.Lerp(this.localKickStartTile.Value.Y, draw_y, this.kickProgress);
            }
            float base_sort_order = System.Math.Max(0f, ((draw_y + 1f) * 64f - 24f) / 10000f) + draw_x * 1E-05f;
            if (this.localKickStartTile.HasValue)
            {
                spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((draw_x + 0.5f) * 64f, (draw_y + 0.5f) * 64f)), Game1.shadowTexture.Bounds, Color.Black * 0.5f, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, 0.0001f);
                draw_y -= (float)System.Math.Sin((double)this.kickProgress * System.Math.PI) * 0.5f;
            }

            // Show a "filled" sprite or not, based on if the Harvest Statues has items
            spriteBatch.Draw(Game1.bigCraftableSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(draw_x * 64f + (float)((base.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (draw_y - 1f) * 64f)), Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.items.Any() ? this.ParentSheetIndex + 1 : this.ParentSheetIndex, 16, 32), this.tint.Value * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, base_sort_order);
        }
    }
}
