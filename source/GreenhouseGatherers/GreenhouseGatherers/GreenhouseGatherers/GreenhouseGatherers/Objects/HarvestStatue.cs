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
    public class HarvestStatue : StardewValley.Objects.Chest
    {
        private IMonitor monitor = ModResources.GetMonitor();

        public bool isFull = false;
        public bool ateCrops = false;
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

        protected override void initNetFields()
        {
            base.initNetFields();
        }

        public override int maximumStackSize()
        {
            return 1;
        }

        public HarvestStatue()
        {

        }

        public HarvestStatue(Vector2 position, int itemID, bool enableHarvestMessage = true, bool doJunimosEatExcessCrops = true, bool doJunimosHarvestFromPots = true, bool doJunimosHarvestFromFruitTrees = true, bool doJunimosHarvestFromFlowers = true, bool doJunimosSowSeedsAfterHarvest = false, int minimumFruitOnTreeBeforeHarvest = 3) : base(true, position, itemID)
        {
            this.Name = "Harvest Statue";
            this.enableHarvestMessage = enableHarvestMessage;
            this.doJunimosEatExcessCrops = doJunimosEatExcessCrops;
            this.doJunimosHarvestFromPots = doJunimosHarvestFromPots;
            this.doJunimosHarvestFromFruitTrees = doJunimosHarvestFromFruitTrees;
            this.doJunimosHarvestFromFlowers = doJunimosHarvestFromFlowers;
            this.doJunimosSowSeedsAfterHarvest = doJunimosSowSeedsAfterHarvest;
            this.minimumFruitOnTreeBeforeHarvest = minimumFruitOnTreeBeforeHarvest;

            this.currentSheetIndex = itemID;

            base.type.Value = "Crafting";
            base.bigCraftable.Value = true;
            base.canBeSetDown.Value = true;

            // Setting SpecialChestType to -1 so we can bypass Automate's default chest logic
            // TODO: Make this only happen if Automate is installed
            this.SpecialChestType = (SpecialChestTypes)(-1);
        }

        public void SpawnJunimos(GameLocation location, int maxJunimosToSpawn = -1)
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

        public void HarvestCrops(GameLocation location)
        {
            string locationName = ModResources.SplitCamelCaseText(location.Name);
            // Check if we're at capacity and that Junimos aren't allowed to eat excess crops
            if (this.items.Count >= this.GetActualCapacity() && !doJunimosEatExcessCrops)
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
            if (ateCrops)
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
            if (this.items.Count >= this.GetActualCapacity() && !doJunimosEatExcessCrops)
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

                    if (this.addItem(new Object(fruitTree.indexOfFruit, 1, quality: fruitQuality)) != null)
                    {
                        ateCrops = true;
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
                this.addItem(item);
            }

            UpdateSprite();
        }

        private void UpdateSprite()
        {
            if (!this.items.Any())
            {
                this.currentSheetIndex = this.ParentSheetIndex;
            }
            else
            {
                this.currentSheetIndex = this.ParentSheetIndex + 1;
            }
        }

        public override void ShowMenu()
        {
            // Set source to 0 so recolor doesn't show up
            Game1.activeClickableMenu = new ItemGrabMenu(this.GetItemsForPlayer(Game1.player.UniqueMultiplayerID), reverseGrab: false, showReceivingMenu: true, InventoryMenu.highlightAllItems, grabItemFromInventory, null, grabItemFromChest, snapToBottom: false, canBeExitedWithKey: true, playRightClickSound: true, allowRightClick: true, showOrganizeButton: true, 0, null, -1, this);
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            base.tileLocation.Value = new Vector2(x / 64, y / 64);
            return true;
        }

        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            if (!Game1.didPlayerJustRightClick(ignoreNonMouseHeldInput: true))
            {
                return false;
            }

            this.GetMutex().RequestLock(delegate
            {
                this.frameCounter.Value = 1;
                Game1.playSound("stoneStep");
                Game1.player.Halt();
            });

            return true;
        }

        public override bool performToolAction(Tool t, GameLocation location)
        {
            if (t != null && t.getLastFarmerToUse() != null && t.getLastFarmerToUse() != Game1.player)
            {
                return false;
            }

            if (t == null)
            {
                return false;
            }

            if (t is MeleeWeapon || !t.isHeavyHitter())
            {
                return false;
            }

            Farmer player = t.getLastFarmerToUse();
            if (player != null)
            {
                Vector2 c = base.TileLocation;
                if (c.X == 0f && c.Y == 0f)
                {
                    bool found = false;
                    foreach (KeyValuePair<Vector2, StardewValley.Object> pair in location.objects.Pairs)
                    {
                        if (pair.Value == this)
                        {
                            c.X = (int)pair.Key.X;
                            c.Y = (int)pair.Key.Y;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        c = player.GetToolLocation() / 64f;
                        c.X = (int)c.X;
                        c.Y = (int)c.Y;
                    }
                }
                this.GetMutex().RequestLock(delegate
                {
                    this.clearNulls();
                    //monitor.Log(this.items.Count.ToString(), LogLevel.Debug);
                    if (this.isEmpty())
                    {
                        this.performRemoveAction(base.tileLocation, location);
                        if (location.Objects.Remove(c) && base.type.Equals("Crafting") && (int)base.fragility != 2)
                        {
                            location.debris.Add(new Debris(base.bigCraftable ? (-base.ParentSheetIndex) : base.ParentSheetIndex, player.GetToolLocation(), new Vector2(player.GetBoundingBox().Center.X, player.GetBoundingBox().Center.Y)));
                        }
                    }
                    else if (t != null && t.isHeavyHitter() && !(t is MeleeWeapon))
                    {
                        location.playSound("hammer");
                        base.shakeTimer = 100;
                        if (t != player.CurrentTool)
                        {
                            Vector2 zero = Vector2.Zero;
                            zero = ((player.FacingDirection == 1) ? new Vector2(1f, 0f) : ((player.FacingDirection == 3) ? new Vector2(-1f, 0f) : ((player.FacingDirection == 0) ? new Vector2(0f, -1f) : new Vector2(0f, 1f))));
                            if (base.TileLocation.X == 0f && base.TileLocation.Y == 0f && location.getObjectAtTile((int)c.X, (int)c.Y) == this)
                            {
                                base.TileLocation = c;
                            }
                            this.MoveToSafePosition(location, base.TileLocation, 0, zero);
                        }
                    }
                    this.GetMutex().ReleaseLock();
                });
            }

            return false;
        }

        public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
        {
            if (this.synchronized.Value)
            {
                this.openChestEvent.Poll();
            }
            if (this.localKickStartTile.HasValue)
            {
                if (Game1.currentLocation == environment)
                {
                    if (this.kickProgress == 0f)
                    {
                        if (Utility.isOnScreen((this.localKickStartTile.Value + new Vector2(0.5f, 0.5f)) * 64f, 64))
                        {
                            Game1.playSound("clubhit");
                        }

                        base.shakeTimer = 100;
                    }
                }
                else
                {
                    this.localKickStartTile = null;
                    this.kickProgress = -1f;
                }
                if (this.kickProgress >= 0f)
                {
                    float move_duration = 0.25f;
                    this.kickProgress += (float)(time.ElapsedGameTime.TotalSeconds / (double)move_duration);
                    if (this.kickProgress >= 1f)
                    {
                        this.kickProgress = -1f;
                        this.localKickStartTile = null;
                    }
                }
            }
            else
            {
                this.kickProgress = -1f;
            }
            this.mutex.Update(environment);
            if (base.shakeTimer > 0)
            {
                base.shakeTimer -= time.ElapsedGameTime.Milliseconds;
                if (base.shakeTimer <= 0)
                {
                    base.health = 10;
                }
            }

            if ((int)this.frameCounter > -1 && this.GetMutex().IsLockHeld())
            {
                this.ShowMenu();
                this.frameCounter.Value = -1;
            }
            else if ((int)this.frameCounter == -1 && Game1.activeClickableMenu == null && this.GetMutex().IsLockHeld())
            {
                this.GetMutex().ReleaseLock();
                this.frameCounter.Value = 1;
                environment.localSound("stoneStep");
            }

            UpdateSprite();
        }
        public override void actionOnPlayerEntry()
        {
            this.kickProgress = -1f;
            this.localKickStartTile = null;
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
            spriteBatch.Draw(Game1.bigCraftableSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(draw_x * 64f + (float)((base.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (draw_y - 1f) * 64f)), Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.currentSheetIndex, 16, 32), this.tint.Value * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, base_sort_order);
        }
    }
}
