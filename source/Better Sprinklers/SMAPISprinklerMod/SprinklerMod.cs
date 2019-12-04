using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BetterSprinklers.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace BetterSprinklers
{
    public class SprinklerMod : Mod, IAssetEditor
    {
        /*********
        ** Fields
        *********/
        /// <summary>The asset key for the crafting recipes.</summary>
        private const string RecipeDataKey = "Data/CraftingRecipes";

        /// <summary>The asset key for the object data.</summary>
        private const string ObjectDataKey = "Data/ObjectInformation";

        /// <summary>The maximum grid size.</summary>
        private readonly int MaxGridSize = 19;

        /// <summary>The mod configuration.</summary>
        private SprinklerModConfig Config;

        private Texture2D BuildingPlacementTiles;
        private int[,] ScarecrowGrid;

        /// <summary>Whether to show a grid overlay and highlight the coverage for sprinklers or scarecrows under the cursor.</summary>
        private bool ShowInfoOverlay;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // initialise
            this.Config = helper.ReadConfig<SprinklerModConfig>();
            this.ScarecrowGrid = this.GetScarecrowGrid();
            this.BuildingPlacementTiles = helper.Content.Load<Texture2D>("LooseSprites\\buildingPlacementTiles", ContentSource.GameContent);

            // set up events
            helper.Events.Display.RenderingHud += this.OnRenderingHud;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        /// <summary>Get an API that other mods can access. This is always called after <see cref="Entry" />.</summary>
        public override object GetApi()
        {
            return new BetterSprinklersApi(this.Config, this.MaxGridSize);
        }

        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return
                asset.AssetNameEquals(SprinklerMod.RecipeDataKey)
                || asset.AssetNameEquals(SprinklerMod.ObjectDataKey);
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            // recalculate sprinkler crafting resources
            if (asset.AssetNameEquals(SprinklerMod.RecipeDataKey))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                foreach (KeyValuePair<string, string> pair in data.ToArray())
                {
                    if (!pair.Key.Contains("prinkler"))
                        continue;

                    // get field info
                    string[] fields = pair.Value.Split('/');
                    int sprinklerID = int.Parse(fields[2]);
                    string[] ingredients = fields[0].Split(' ');
                    if (!this.Config.SprinklerPrices.TryGetValue(sprinklerID, out int multiplier))
                        continue;

                    // multiply ingredients
                    for (int i = 1; i < ingredients.Length; i += 2)
                        ingredients[i] = (int.Parse(ingredients[i]) * multiplier).ToString();
                    fields[0] = string.Join(" ", ingredients);

                    // save
                    data[pair.Key] = string.Join("/", fields);
                }
            }

            // recalculate sale price
            else if (asset.AssetNameEquals(SprinklerMod.ObjectDataKey))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
                foreach (KeyValuePair<int, string> pair in data.ToArray())
                {
                    if (!this.Config.SprinklerPrices.TryGetValue(pair.Key, out int multiplier))
                        continue;

                    // multiply cost
                    string[] fields = pair.Value.Split('/');
                    fields[1] = (int.Parse(fields[1]) * multiplier).ToString();

                    // save
                    data[pair.Key] = string.Join("/", fields);
                }
            }

            else
                throw new InvalidOperationException($"Unexpected asset {asset.AssetName}");
        }


        /*********
        ** Private methods
        *********/
        /****
        ** Event handlers
        ****/
        /// <summary>Raised before drawing the HUD (item toolbar, clock, etc) to the screen. The vanilla HUD may be hidden at this point (e.g. because a menu is open).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (Context.IsWorldReady && Game1.activeClickableMenu == null && Game1.CurrentEvent == null)
                this.RenderHighlight();
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            this.RunSprinklers();
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Game1.activeClickableMenu != null || Game1.CurrentEvent != null)
                return;

            // show menu
            if (e.Button == this.Config.ConfigKey)
                Game1.activeClickableMenu = new SprinklerShapeEditMenu(this.Config, this.SaveChanges);

            // toggle overlay
            else if (e.Button == this.Config.HighlightKey)
                this.ShowInfoOverlay = !this.ShowInfoOverlay;
        }

        /****
        ** Methods
        ****/
        /// <summary>Get the scarecrow layout grid.</summary>
        private int[,] GetScarecrowGrid()
        {
            int[,] grid = new int[this.MaxGridSize, this.MaxGridSize];
            int scarecrowCenterValue = this.MaxGridSize / 2;
            Vector2 scarecrowCenter = new Vector2(scarecrowCenterValue, scarecrowCenterValue);
            for (int x = 0; x < this.MaxGridSize; x++)
            {
                for (int y = 0; y < this.MaxGridSize; y++)
                {
                    Vector2 vector = new Vector2(x, y);
                    grid[x, y] = Vector2.Distance(vector, scarecrowCenter) < 9f ? 1 : 0;
                }
            }
            return grid;
        }

        /// <summary>Save changes to the mod configuration.</summary>
        private void SaveChanges()
        {
            this.Helper.WriteConfig(this.Config);
            Game1.addHUDMessage(new HUDMessage("Sprinkler Configurations Saved", Color.Green, 3500f));
            this.UpdatePrices();
        }

        /// <summary>Update the sprinkler crafting costs based on the current mod configuration.</summary>
        private void UpdatePrices()
        {
            this.Helper.Content.InvalidateCache(SprinklerMod.RecipeDataKey);
            this.Helper.Content.InvalidateCache(SprinklerMod.ObjectDataKey);
        }

        /// <summary>Run all sprinklers.</summary>
        private void RunSprinklers()
        {
            foreach (GameLocation location in this.GetLocations())
            {
                foreach (KeyValuePair<Vector2, SObject> objectPair in location.objects.Pairs)
                {
                    int targetID = objectPair.Value.ParentSheetIndex;
                    Vector2 targetTile = objectPair.Key;
                    if (this.Config.SprinklerShapes.ContainsKey(targetID))
                    {
                        int[,] grid = this.Config.SprinklerShapes[targetID];
                        GridHelper.ForCoveredTiles(targetTile, grid, tilePos =>
                        {
                            if (location.terrainFeatures.TryGetValue(tilePos, out TerrainFeature terrainFeature) && terrainFeature is HoeDirt dirt)
                                dirt.state.Value = 1;
                        });
                    }
                }
            }
        }

        /// <summary>Get all game location.</summary>
        private IEnumerable<GameLocation> GetLocations()
        {
            return Game1.locations
                .Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value
                );
        }

        /// <summary>Highlight coverage for sprinklers and scarecrows based on the current context.</summary>
        [SuppressMessage("ReSharper", "PossibleLossOfFraction", Justification = "The decimals are deliberately truncated during conversion to tile coordinates.")]
        private void RenderHighlight()
        {
            // get context
            Vector2 mouseTile = new Vector2((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize, (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize);
            SObject heldItem = Game1.player.ActiveObject;

            // highlight coverage for held item
            if (heldItem != null)
            {
                if (this.Config.SprinklerShapes.ContainsKey(heldItem.ParentSheetIndex))
                {
                    this.RenderSprinklerHighlight(heldItem.ParentSheetIndex, mouseTile);
                    return;
                }
                if (heldItem.bigCraftable.Value && heldItem.Name.Contains("arecrow"))
                {
                    this.RenderScarecrowHighlight(mouseTile);
                    return;
                }
            }

            // highlight coverage for item under cursor
            if (this.ShowInfoOverlay)
            {
                if (Game1.currentLocation.objects.TryGetValue(mouseTile, out SObject target))
                {
                    if (this.Config.SprinklerShapes.ContainsKey(target.ParentSheetIndex))
                        this.RenderSprinklerHighlight(target.ParentSheetIndex, mouseTile);
                    else if (target.bigCraftable.Value && target.Name.Contains("arecrow"))
                        this.RenderScarecrowHighlight(mouseTile);
                }
                this.RenderGrid();
            }
        }

        /// <summary>Highlight coverage for a sprinkler.</summary>
        /// <param name="sprinklerID">The sprinkler ID.</param>
        /// <param name="tile">The sprinkler tile.</param>
        private void RenderSprinklerHighlight(int sprinklerID, Vector2 tile)
        {
            this.RenderHighlight(tile, this.Config.SprinklerShapes[sprinklerID]);
        }

        /// <summary>Highlight coverage for a scarecrow.</summary>
        /// <param name="tile">The scarecrow tile.</param>
        private void RenderScarecrowHighlight(Vector2 tile)
        {
            this.RenderHighlight(tile, this.ScarecrowGrid);
        }

        /// <summary>Highlight coverage based on a given grid.</summary>
        /// <param name="centerTile">The tile at the center of the grid.</param>
        /// <param name="grid">The grid indicating which tiles to highlight.</param>
        private void RenderHighlight(Vector2 centerTile, int[,] grid)
        {
            GridHelper.ForCoveredTiles(centerTile, grid, tilePos =>
            {
                Game1.spriteBatch.Draw(this.BuildingPlacementTiles, Game1.GlobalToLocal(Game1.viewport, tilePos * Game1.tileSize), Game1.getSourceRectForStandardTileSheet(this.BuildingPlacementTiles, 0), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.999f);
            });
        }

        /// <summary>Render a grid showing the tile layout.</summary>
        private void RenderGrid()
        {
            int startingX = -Game1.viewport.X % Game1.tileSize;
            float startingY = -(float)Game1.viewport.Y % Game1.tileSize;
            for (int x = startingX; x < Game1.graphics.GraphicsDevice.Viewport.Width; x += Game1.tileSize)
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(x, (int)startingY, 1, Game1.graphics.GraphicsDevice.Viewport.Height), this.Config.GridColour);
            for (float y = startingY; y < Game1.graphics.GraphicsDevice.Viewport.Height; y += Game1.tileSize)
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(startingX, (int)y, Game1.graphics.GraphicsDevice.Viewport.Width, 1), this.Config.GridColour);
        }
    }
}
