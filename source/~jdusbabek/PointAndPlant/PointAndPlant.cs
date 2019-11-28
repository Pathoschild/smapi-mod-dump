using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PointAndPlant.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile.Dimensions;

namespace PointAndPlant
{
    public class PointAndPlant : Mod
    {
        /*********
        ** Properties
        *********/
        private SButton PlowKey;
        private SButton PlantKey;
        private SButton GrowKey;
        private SButton HarvestKey;
        private SButton GrassKey;

        private bool PlowEnabled;
        private bool PlantEnabled;
        private bool GrowEnabled;
        private bool HarvestEnabled;

        private int PlantRadius;
        private int GrowRadius;
        private int HarvestRadius;

        private int Fertilizer;

        private int PlowWidth;
        private int PlowHeight;

        private int MouseX;
        private int MouseY;
        private int TileX;
        private int TileY;

        private Vector2 Vector;
        private Texture2D BuildingTiles;

        private ModConfig Config;

        private Hoe CustomHoe;
        private ModSickle CustomSickle;
        private Axe PhantomAxe;
        private Pickaxe PhantomPick;

        private int ToolLevel = 4;

        private bool LoggingEnabled;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.Input.ButtonReleased += this.OnButtonReleased;
            helper.Events.Display.Rendered += this.OnRendered;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player loads a save slot.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();

            this.PlowEnabled = this.Config.PlowEnabled;
            this.PlantEnabled = this.Config.PlantEnabled;
            this.GrowEnabled = this.Config.GrowEnabled;
            this.HarvestEnabled = this.Config.HarvestEnabled;

            this.PlantRadius = this.Config.PlantRadius;
            this.GrowRadius = this.Config.GrowRadius;
            this.HarvestRadius = this.Config.HarvestRadius;

            this.PlowWidth = this.Config.PlowWidth;
            this.PlowHeight = this.Config.PlowHeight;

            this.Fertilizer = this.Config.Fertilizer;

            this.LoggingEnabled = this.Config.LoggingEnabled;

            if (this.Config.PlowWidth < 1)
            {
                this.PlowWidth = 1;
                if (this.PlowEnabled)
                    this.Monitor.Log($"Plow width must be at least 1; defaulted to {this.PlowWidth}.");
            }
            else
                this.PlowWidth = this.Config.PlowWidth;

            if (this.Config.PlowHeight < 1)
            {
                this.PlowHeight = 1;
                if (this.PlowEnabled)
                    this.Monitor.Log($"Plow height must be at least 1; defaulted to {this.PlowHeight}.");
            }
            else
                this.PlowHeight = this.Config.PlowHeight;

            if (this.Config.PlantRadius < 0)
            {
                this.PlantRadius = 0;
                if (this.Config.PlantEnabled)
                    this.Monitor.Log($"Plant Radius must be 0 or greater; defaulted to {this.PlantRadius}.");
            }
            else
                this.PlantRadius = this.Config.PlantRadius;

            if (this.Config.GrowRadius < 0)
            {
                this.GrowRadius = 0;
                if (this.Config.GrowEnabled)
                    this.Monitor.Log($"Grow Radius must be 0 or greater; defaulted to {this.GrowRadius}.");
            }
            else
                this.GrowRadius = this.Config.GrowRadius;

            if (this.Config.HarvestRadius < 0)
            {
                this.HarvestRadius = 0;
                if (this.Config.PlantEnabled)
                    this.Monitor.Log($"Harvest Radius must be 0 or greater; defaulted to {this.HarvestRadius}.");
            }
            else
                this.HarvestRadius = this.Config.HarvestRadius;

            if (!Enum.TryParse(this.Config.PlowKey, true, out this.PlowKey))
            {
                this.PlowKey = SButton.Z;
                if (this.PlowEnabled)
                    this.Monitor.Log($"Error parsing plow key; defaulted to {this.PlowKey}.");
            }

            if (!Enum.TryParse(this.Config.PlantKey, true, out this.PlantKey))
            {
                this.PlantKey = SButton.A;
                if (this.PlantEnabled)
                    this.Monitor.Log($"Error parsing plant key; defaulted to {this.PlantKey}.");
            }

            if (!Enum.TryParse(this.Config.GrowKey, true, out this.GrowKey))
            {
                this.GrowKey = SButton.S;
                if (this.GrowEnabled)
                    this.Monitor.Log($"Error parsing grow key; defaulted to {this.GrowKey}.");
            }

            if (!Enum.TryParse(this.Config.HarvestKey, true, out this.HarvestKey))
            {
                this.HarvestKey = SButton.D;
                if (this.HarvestEnabled)
                    this.Monitor.Log($"Error parsing harvest key; defaulted to {this.HarvestKey}.");
            }

            this.GrassKey = SButton.Q;
            this.CustomSickle = new ModSickle(47, this.Config.HarvestRadius, this.Vector);
            this.CustomHoe = new Hoe { UpgradeLevel = this.ToolLevel };
        }

        /// <summary>Raised after the player releases a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            if (Game1.hasLoadedGame)
            {
                if (e.Button == this.PlowKey && this.Config.PlowEnabled)
                {
                    try
                    {
                        this.Plow();
                    }
                    catch (Exception ex)
                    {
                        if (this.LoggingEnabled)
                            this.Monitor.Log($"Plow (key handler) Exception: {ex}", LogLevel.Error);
                    }
                }
                else if (e.Button == this.PlantKey && this.Config.PlantEnabled)
                {
                    try
                    {
                        this.Plant();
                    }
                    catch (Exception ex)
                    {
                        if (this.LoggingEnabled)
                            this.Monitor.Log($"Plant (key handler) Exception: {ex}", LogLevel.Error);
                    }

                }
                else if (e.Button == this.GrowKey && this.Config.GrowEnabled)
                {
                    try
                    {
                        this.Grow();
                    }
                    catch (Exception ex)
                    {
                        if (this.LoggingEnabled)
                            this.Monitor.Log($"Grow (key handler) Exception: {ex}", LogLevel.Error);
                    }

                }
                else if (e.Button == this.HarvestKey && this.Config.HarvestEnabled)
                {
                    try
                    {
                        this.Harvest();
                    }
                    catch (Exception ex)
                    {
                        if (this.LoggingEnabled)
                            this.Monitor.Log($"Harvest (key handler) Exception: {ex}", LogLevel.Error);
                    }
                }
                else if (e.Button == this.GrassKey)
                {
                    try
                    {
                        if (Game1.player.CurrentTool != null && Game1.player.CurrentTool is MeleeWeapon)
                        {
                            if (((MeleeWeapon)Game1.player.CurrentTool).InitialParentTileIndex == 39)
                            {
                                this.Monitor.Log("You're holding Leah's Whittler! Chop away!!", LogLevel.Trace);
                                this.ChopTrees();
                            }
                            else if (((MeleeWeapon)Game1.player.CurrentTool).InitialParentTileIndex == 37)
                            {
                                this.Monitor.Log("You're holding Harvey's Mallet! Pound away!!", LogLevel.Trace);
                                this.BreakRocks();
                            }
                            else
                                this.PlantGrass();
                        }
                        else
                            this.PlantGrass();

                    }
                    catch (Exception ex)
                    {
                        if (this.LoggingEnabled)
                            this.Monitor.Log($"Plant Grass (key handler) Exception: {ex}", LogLevel.Error);
                    }
                }
            }
        }

        private void Plow()
        {
            Farmer player = Game1.player;
            List<Vector2> tiles = new List<Vector2>();

            if (player.currentLocation.Name.Equals("Farm") || player.currentLocation.Name.Contains("Greenhouse"))
            {
                for (int x = 0; x < this.PlowWidth; x++)
                {
                    for (int y = 0; y < this.PlowHeight; y++)
                        tiles.Add(new Vector2(this.TileX + x, this.TileY + y));
                }
            }
            else
            {
                for (int x = 0; x < 15; x++)
                {
                    for (int y = 0; y < 15; y++)
                        tiles.Add(new Vector2(this.TileX + x, this.TileY + y));
                }
            }


            GameLocation location = Game1.player.currentLocation;
            Vector2 vector2 = new Vector2(this.TileX, this.TileY);

            foreach (Vector2 index in tiles)
            {
                try
                {
                    if (location.terrainFeatures.ContainsKey(index))
                    {
                        if (location.terrainFeatures[index].performToolAction(this.CustomHoe, 0, index, location))
                            location.terrainFeatures.Remove(index);
                    }
                    else
                    {
                        if (location.objects.ContainsKey(index) && location.Objects[index].performToolAction(this.CustomHoe, location))
                        {
                            if (location.Objects[index].Type.Equals("Crafting") && location.Objects[index].Fragility != 2)
                                location.debris.Add(new Debris(location.Objects[index].bigCraftable.Value ? -location.Objects[index].ParentSheetIndex : location.Objects[index].ParentSheetIndex, Game1.player.GetToolLocation(), new Vector2(Game1.player.GetBoundingBox().Center.X, Game1.player.GetBoundingBox().Center.Y)));
                            location.Objects[index].performRemoveAction(index, location);
                            location.Objects.Remove(index);
                        }
                        if (location.doesTileHaveProperty((int)index.X, (int)index.Y, "Diggable", "Back") != null)
                        {
                            if (location.Name.Equals("UndergroundMine") && !location.isTileOccupied(index))
                            {
                                if (Game1.mine.mineLevel < 40 || Game1.mine.mineLevel >= 80)
                                {
                                    location.terrainFeatures.Add(index, new HoeDirt());
                                    Game1.playSound("hoeHit");
                                }
                                else if (Game1.mine.mineLevel < 80)
                                {
                                    location.terrainFeatures.Add(index, new HoeDirt());
                                    Game1.playSound("hoeHit");
                                }
                                Game1.removeSquareDebrisFromTile((int)index.X, (int)index.Y);
                                location.checkForBuriedItem((int)index.X, (int)index.Y, false, false, Game1.player);
                                location.temporarySprites.Add(new TemporaryAnimatedSprite(12, new Vector2(vector2.X * Game1.tileSize, vector2.Y * Game1.tileSize), Color.White, 8, Game1.random.NextDouble() < 0.5, 50f));
                                if (tiles.Count > 2)
                                    location.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2(index.X * Game1.tileSize, index.Y * Game1.tileSize), Color.White, 8, Game1.random.NextDouble() < 0.5, Vector2.Distance(vector2, index) * 30f));
                            }
                            else if (!location.isTileOccupied(index) && location.isTilePassable(new Location((int)index.X, (int)index.Y), Game1.viewport))
                            {
                                location.makeHoeDirt(index);
                                Game1.playSound("hoeHit");
                                Game1.removeSquareDebrisFromTile((int)index.X, (int)index.Y);
                                location.temporarySprites.Add(new TemporaryAnimatedSprite(12, new Vector2(index.X * Game1.tileSize, index.Y * Game1.tileSize), Color.White, 8, Game1.random.NextDouble() < 0.5, 50f));
                                if (tiles.Count > 2)
                                    location.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2(index.X * Game1.tileSize, index.Y * Game1.tileSize), Color.White, 8, Game1.random.NextDouble() < 0.5, Vector2.Distance(vector2, index) * 30f));
                                location.checkForBuriedItem((int)index.X, (int)index.Y, false, false, Game1.player);
                            }
                            ++Game1.stats.DirtHoed;
                        }

                    }
                }
                catch (Exception exception)
                {
                    if (this.LoggingEnabled)
                    {
                        this.Monitor.Log($"Plow Exception: {exception}", LogLevel.Error);
                    }
                }
            }
        }

        private void Plant()
        {
            Farmer player = Game1.player;
            List<Vector2> tiles = new List<Vector2>();

            int min = this.PlantRadius * -1;

            for (int x = min; x <= this.PlantRadius; x++)
            {
                for (int y = min; y <= this.PlantRadius; y++)
                {
                    tiles.Add(new Vector2(this.TileX + x, this.TileY + y));
                }
            }

            foreach (Vector2 index in tiles)
            {
                try
                {
                    if (player.currentLocation.terrainFeatures.ContainsKey(index))
                    {
                        if (player.currentLocation.terrainFeatures[index] is HoeDirt dirt)
                        {
                            if (player.currentLocation.IsFarm || player.currentLocation.Name.Contains("Greenhouse"))
                            {
                                if (Game1.player.ActiveObject != null && dirt.crop == null && (Game1.player.ActiveObject.Category == -74 || Game1.player.ActiveObject.Category == -19))
                                {
                                    //Quality Fertilizer: 369
                                    //Quality Retaining Soil: 371
                                    //Basic Fertilizer: 368
                                    //Basic Retaining Soil: 370
                                    //Deluxe Speed-Gro: 466
                                    //Speed - Gro: 465
                                    dirt.fertilizer.Value = this.Fertilizer;

                                    if (dirt.plant(Game1.player.ActiveObject.ParentSheetIndex, this.MouseX, this.MouseY, Game1.player, Game1.player.ActiveObject.Category == -19, player.currentLocation) && Game1.player.IsMainPlayer)
                                        Game1.player.reduceActiveItemByOne();
                                    Game1.haltAfterCheck = false;
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    if (this.LoggingEnabled)
                        this.Monitor.Log($"Planting Exception: {exception}", LogLevel.Error);
                }
            }
        }

        private void PlantGrass()
        {
            Farmer player = Game1.player;
            List<Vector2> tiles = new List<Vector2>();

            int min = this.PlantRadius * -1;

            for (int x = min; x <= this.PlantRadius; x++)
            {
                for (int y = min; y <= this.PlantRadius; y++)
                    tiles.Add(new Vector2(this.TileX + x, this.TileY + y));
            }

            foreach (Vector2 index in tiles)
            {
                try
                {
                    if (!player.currentLocation.objects.ContainsKey(index) && !player.currentLocation.terrainFeatures.ContainsKey(index))
                    {
                        this.Monitor.Log($"Adding grass to: {index}", LogLevel.Trace);
                        player.currentLocation.terrainFeatures.Add(index, new Grass(1, 4));
                        Game1.playSound("dirtyHit");
                    }
                }
                catch (Exception exception)
                {
                    if (this.LoggingEnabled)
                        this.Monitor.Log($"Planting Exception: {exception}", LogLevel.Error);
                }
            }
        }

        private void ChopTrees()
        {
            Farmer player = Game1.player;
            List<Vector2> tiles = new List<Vector2>();

            if (this.PhantomAxe == null)
                this.PhantomAxe = new Axe { UpgradeLevel = 4 };

            int min = this.PlantRadius * -1;

            for (int x = min; x <= this.PlantRadius; x++)
            {
                for (int y = min; y <= this.PlantRadius; y++)
                    tiles.Add(new Vector2(this.TileX + x, this.TileY + y));
            }

            foreach (Vector2 index in tiles)
            {
                try
                {
                    if (player.currentLocation.terrainFeatures.ContainsKey(index) && player.currentLocation.terrainFeatures[index] is Tree)
                        this.PhantomAxe.DoFunction(player.currentLocation, (int)(index.X * Game1.tileSize), (int)(index.Y * Game1.tileSize), 4, player);
                }
                catch (Exception exception)
                {
                    if (this.LoggingEnabled)
                        this.Monitor.Log($"Tree Chopping Exception: {exception}", LogLevel.Error);
                }
            }
        }

        private void BreakRocks()
        {
            Farmer player = Game1.player;
            List<Vector2> tiles = new List<Vector2>();

            if (this.PhantomPick == null)
                this.PhantomPick = new Pickaxe { UpgradeLevel = 4 };

            int min = this.PlantRadius * -1;

            for (int x = min; x <= this.PlantRadius; x++)
            {
                for (int y = min; y <= this.PlantRadius; y++)
                    tiles.Add(new Vector2(this.TileX + x, this.TileY + y));
            }

            foreach (Vector2 index in tiles)
            {
                try
                {
                    if (player.currentLocation.objects.ContainsKey(index))
                    {
                        StardewValley.Object o = Game1.player.currentLocation.Objects[index];
                        if (o != null && o.name.Equals("Stone"))
                        {
                            o.MinutesUntilReady = 0;
                            this.PhantomPick.DoFunction(player.currentLocation, (int)(index.X * Game1.tileSize), (int)(index.Y * Game1.tileSize), 4, player);
                        }
                    }
                }
                catch (Exception exception)
                {
                    if (this.LoggingEnabled)
                        this.Monitor.Log($"[Point-and-Plant] Rock Pounding Exception: {exception}", LogLevel.Error);
                }
            }
        }

        private void Grow()
        {
            Farmer player = Game1.player;
            List<Vector2> tiles = new List<Vector2>();

            int min = this.GrowRadius * -1;

            for (int x = min; x <= this.GrowRadius; x++)
            {
                for (int y = min; y <= this.GrowRadius; y++)
                    tiles.Add(new Vector2(this.TileX + x, this.TileY + y));
            }

            foreach (Vector2 index in tiles)
            {
                try
                {
                    if (player.currentLocation.terrainFeatures.ContainsKey(index))
                    {
                        if (player.currentLocation.terrainFeatures[index] is HoeDirt dirt)
                            dirt.crop?.growCompletely();
                    }
                }
                catch (Exception exception)
                {
                    if (this.LoggingEnabled)
                        this.Monitor.Log($"Grow Exception: {exception}", LogLevel.Error);
                }
            }
        }

        private void Harvest()
        {
            Farmer player = Game1.player;
            GameLocation currentLocation = Game1.currentLocation;
            currentLocation.terrainFeatures.TryGetValue(this.Vector, out var terrainFeature);

            try
            {
                if (player.currentLocation.IsFarm || player.currentLocation.Name.Contains("Greenhouse"))
                {
                    if (((HoeDirt)terrainFeature).crop != null || ((HoeDirt)terrainFeature).fertilizer.Value != 0)
                    {
                        if (((HoeDirt)terrainFeature).crop != null && (((HoeDirt)terrainFeature).crop.harvestMethod.Value == 1 && ((HoeDirt)terrainFeature).readyForHarvest() || ((HoeDirt)terrainFeature).crop.dead.Value))
                        {
                            this.CustomSickle.DoDamage(currentLocation, this.MouseX, this.MouseY, Game1.player.getFacingDirection(), 0, Game1.player);
                        }
                        else if (((HoeDirt)terrainFeature).crop != null && (((HoeDirt)terrainFeature).crop.harvestMethod.Value != 1 && ((HoeDirt)terrainFeature).readyForHarvest()))
                        {
                            List<Vector2> tiles = new List<Vector2>();
                            int min = this.HarvestRadius * -1;

                            for (int x = min; x <= this.HarvestRadius; x++)
                            {
                                for (int y = min; y <= this.HarvestRadius; y++)
                                    tiles.Add(new Vector2(this.TileX + x, this.TileY + y));
                            }

                            foreach (Vector2 index in tiles)
                            {
                                try
                                {
                                    if (player.currentLocation.terrainFeatures.ContainsKey(index))
                                    {
                                        if (player.currentLocation.terrainFeatures[index] is HoeDirt dirt)
                                        {
                                            if (dirt.crop?.harvestMethod.Value == 0)
                                                dirt.performUseAction(index, player.currentLocation);
                                        }
                                    }
                                }
                                catch (Exception exception)
                                {
                                    if (this.LoggingEnabled)
                                        this.Monitor.Log($"Harvest Inner Exception: {exception}", LogLevel.Error);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                if (this.LoggingEnabled)
                    this.Monitor.Log($"Harvest Outer Exception: {exception}", LogLevel.Error);
            }
        }

        /// <summary>When a menu is open (<see cref="Game1.activeClickableMenu"/> isn't null), raised before that menu is drawn to the screen. This includes the game's internal menus like the title screen. Content drawn to the sprite batch at this point will appear under the menu.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRendered(object sender, EventArgs e)
        {
            /**
             * Gets some values used by the plow.
             */
            try
            {
                if (this.BuildingTiles == null)
                    this.BuildingTiles = this.Helper.Content.Load<Texture2D>("LooseSprites\\buildingPlacementTiles", ContentSource.GameContent);

                if (!Context.IsPlayerFree)
                    return;

                if (!this.Helper.Input.IsDown(this.GrassKey) && !this.Helper.Input.IsDown(this.PlowKey) && !this.Helper.Input.IsDown(this.PlantKey) && !this.Helper.Input.IsDown(this.GrowKey) && !this.Helper.Input.IsDown(this.HarvestKey))
                    return;

                this.MouseX = Game1.getMouseX() + Game1.viewport.X;
                this.MouseY = Game1.getMouseY() + Game1.viewport.Y;

                this.TileX = this.MouseX / Game1.tileSize;
                this.TileY = this.MouseY / Game1.tileSize;
                this.Vector = new Vector2(this.TileX, this.TileY);

                if (this.Helper.Input.IsDown(this.PlowKey))
                {
                    foreach (Vector2 vector2 in this.TilesAffected(Game1.player))
                        Game1.spriteBatch.Draw(this.BuildingTiles, Game1.GlobalToLocal(Game1.viewport, vector2 * Game1.tileSize), Game1.getSourceRectForStandardTileSheet(this.BuildingTiles, 0), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.999f);
                }
                else if (this.Helper.Input.IsDown(this.GrassKey))
                {
                    foreach (Vector2 vector2 in this.TilesAffectedGrass())
                        Game1.spriteBatch.Draw(this.BuildingTiles, Game1.GlobalToLocal(Game1.viewport, vector2 * Game1.tileSize), Game1.getSourceRectForStandardTileSheet(this.BuildingTiles, 0), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.999f);
                }
            }
            catch (Exception exception)
            {
                if (this.LoggingEnabled)
                    this.Monitor.Log($"Drawing Plow Overlay: {exception}", LogLevel.Error);
            }
        }

        private List<Vector2> TilesAffected(Farmer who)
        {
            List<Vector2> tiles = new List<Vector2>();

            if (who.currentLocation.Name.Equals("Farm") || who.currentLocation.Name.Contains("Greenhouse"))
            {
                for (int x = 0; x < this.PlowWidth; x++)
                {
                    for (int y = 0; y < this.PlowHeight; y++)
                        tiles.Add(new Vector2(this.TileX + x, this.TileY + y));
                }
            }
            else
            {
                for (int x = 0; x < 15; x++)
                {
                    for (int y = 0; y < 15; y++)
                        tiles.Add(new Vector2(this.TileX + x, this.TileY + y));
                }
            }

            return tiles;
        }

        private List<Vector2> TilesAffectedGrass()
        {
            List<Vector2> tiles = new List<Vector2>();

            int min = this.PlantRadius * -1;

            for (int x = min; x <= this.PlantRadius; x++)
            {
                for (int y = min; y <= this.PlantRadius; y++)
                    tiles.Add(new Vector2(this.TileX + x, this.TileY + y));
            }

            return tiles;
        }
    }
}
