using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SMAPISprinklerMod;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace SprinklerMod
{
    public class SprinklerMod : Mod
    {
        /*********
        ** Properties
        *********/
        int[] validSprinklers;

        private static IModHelper Helper;
        private static Dictionary<string, string> oldCraftingRecipes;
        private static Dictionary<int, string> oldObjectInfo;
        private static Texture2D buildingPlacementTiles;
        private static int[,] scarecrowGrid;
        private static bool gridKeyHeldDown;


        /*********
        ** Accessors
        *********/
        public static SprinklerModConfig ModConfig { get; private set; }
        public static bool extraInfoActive; //deliberately public, so other mods can read it.


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            SprinklerMod.Helper = helper;
            ModConfig = helper.ReadConfig<SprinklerModConfig>();
            oldCraftingRecipes = null;
            oldObjectInfo = null;
            extraInfoActive = false;

            TimeEvents.DayOfMonthChanged += Event_ChangedDayOfMonth;
            GameEvents.LoadContent += Event_LoadContent;
            GameEvents.UpdateTick += Event_UpdateTick;
            GraphicsEvents.OnPreRenderHudEvent += Event_PreRenderHud;

            scarecrowGrid = new int[19, 19];
            int scarecrowCenterValue = 19 / 2;
            Vector2 scarecrowCenter = new Vector2(scarecrowCenterValue, scarecrowCenterValue);
            int x = 0;
            int y = 0;
            float maxX = 19f;
            float maxY = 19f;
            Vector2 vIterator = new Vector2(0, 0);
            while (vIterator.X < maxX)
            {
                vIterator.Y = 0;
                y = 0;
                while (vIterator.Y < maxY)
                {
                    if (Vector2.Distance(vIterator, scarecrowCenter) < 9f)
                        scarecrowGrid[x, y] = 1;
                    else
                        scarecrowGrid[x, y] = 0;

                    ++vIterator.Y;
                    ++y;
                }
                ++vIterator.X;
                ++x;
            }
        }

        public static void UpdatePrices()
        {
            string[] infoSplit;
            string[] ingredientsSplit;
            int counter;

            if (oldCraftingRecipes == null)
            {
                oldCraftingRecipes = CraftingRecipe.craftingRecipes;
                oldObjectInfo = Game1.objectInformation;
            }
            else
            {
                CraftingRecipe.craftingRecipes = oldCraftingRecipes;
                Game1.objectInformation = oldObjectInfo;
            }


            Dictionary<string, string> newCraftingRecipes = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> craftingRecipe in CraftingRecipe.craftingRecipes)
            {
                if (craftingRecipe.Key.Contains("prinkler"))
                {
                    //Log.Debug(String.Format("key {0} value {1}", craftingRecipe.Key, craftingRecipe.Value));
                    infoSplit = craftingRecipe.Value.Split('/');
                    int sprinklerSheet = infoSplit[2].AsInt32();
                    int multiplier = ModConfig.SprinklerPrices[sprinklerSheet];
                    ingredientsSplit = infoSplit[0].Split(' ');
                    counter = 1;
                    while (counter < ingredientsSplit.Length)
                    {
                        ingredientsSplit[counter] = (ingredientsSplit[counter].AsInt32() * multiplier).ToString();
                        counter += 2;
                    }
                    infoSplit[0] = string.Join(" ", ingredientsSplit);
                    newCraftingRecipes[craftingRecipe.Key] = string.Join("/", infoSplit);
                    //Log.Debug(String.Format("key {0} value {1}", craftingRecipe.Key, newCraftingRecipes[craftingRecipe.Key]));
                }
                else
                {
                    newCraftingRecipes[craftingRecipe.Key] = craftingRecipe.Value;
                }
            }

            Dictionary<int, string> newObjectInfo = new Dictionary<int, string>();
            foreach (KeyValuePair<int, string> objectInfo in Game1.objectInformation)
            {
                if (ModConfig.SprinklerPrices.ContainsKey(objectInfo.Key))
                {
                    int multiplier = ModConfig.SprinklerPrices[objectInfo.Key];
                    infoSplit = objectInfo.Value.Split('/');
                    //Log.Debug(String.Format("object index {0}, name {1}, old price {2}, new price {3}", objectInfo.Key, infoSplit[0], infoSplit[1], infoSplit[1].AsInt32() * multiplier));
                    infoSplit[1] = (infoSplit[1].AsInt32() * multiplier).ToString();
                    newObjectInfo[objectInfo.Key] = string.Join("/", infoSplit);
                }
                else
                {
                    newObjectInfo[objectInfo.Key] = objectInfo.Value;
                }
            }

            CraftingRecipe.craftingRecipes = newCraftingRecipes;
            Game1.objectInformation = newObjectInfo;
        }

        /// <summary>Save the current configuration settings.</summary>
        internal static void SaveConfig()
        {
            SprinklerMod.Helper.WriteConfig(SprinklerMod.ModConfig);
        }


        /*********
        ** Private methods
        *********/
        private static void RenderSprinklerHighlight(int objIndex, Vector2 mousePositionTile)
        {
            int[,] configGrid = ModConfig.SprinklerShapes[objIndex];

            Vector2 iterativeLocation = mousePositionTile;
            int arrayHalfSizeX = configGrid.GetLength(0) / 2;
            int arrayHalfSizeY = configGrid.GetLength(1) / 2;
            iterativeLocation.X -= arrayHalfSizeX;
            iterativeLocation.Y -= arrayHalfSizeY;
            float maxX = mousePositionTile.X + arrayHalfSizeX + 1;
            float maxY = mousePositionTile.Y + arrayHalfSizeY + 1;

            int counterX = 0;
            int counterY = 0;

            while (iterativeLocation.X < maxX)
            {
                iterativeLocation.Y = mousePositionTile.Y - arrayHalfSizeY;
                counterY = 0;
                while (iterativeLocation.Y < maxY)
                {
                    if (configGrid[counterX, counterY] > 0)
                    {
                        Game1.spriteBatch.Draw(buildingPlacementTiles, Game1.GlobalToLocal(Game1.viewport, iterativeLocation * (float)Game1.tileSize), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(buildingPlacementTiles, 0, -1, -1)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.999f);
                    }
                    ++iterativeLocation.Y;
                    ++counterY;
                }
                ++iterativeLocation.X;
                ++counterX;
            }
        }

        private static void RenderScarecrowHighlight(Vector2 mousePositionTile)
        {
            int[,] configGrid = scarecrowGrid;

            Vector2 iterativeLocation = mousePositionTile;
            int arrayHalfSizeX = configGrid.GetLength(0) / 2;
            int arrayHalfSizeY = configGrid.GetLength(1) / 2;
            iterativeLocation.X -= arrayHalfSizeX;
            iterativeLocation.Y -= arrayHalfSizeY;
            float maxX = mousePositionTile.X + arrayHalfSizeX + 1;
            float maxY = mousePositionTile.Y + arrayHalfSizeY + 1;

            int counterX = 0;
            int counterY = 0;

            while (iterativeLocation.X < maxX)
            {
                iterativeLocation.Y = mousePositionTile.Y - arrayHalfSizeY;
                counterY = 0;
                while (iterativeLocation.Y < maxY)
                {
                    if (configGrid[counterX, counterY] > 0)
                    {
                        Game1.spriteBatch.Draw(buildingPlacementTiles, Game1.GlobalToLocal(Game1.viewport, iterativeLocation * (float)Game1.tileSize), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(buildingPlacementTiles, 0, -1, -1)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.999f);
                    }
                    ++iterativeLocation.Y;
                    ++counterY;
                }
                ++iterativeLocation.X;
                ++counterX;
            }
        }

        private static void RenderGrid()
        {
            int startingX = -Game1.viewport.X % Game1.tileSize;
            float startingY = (float)(-(float)Game1.viewport.Y % Game1.tileSize);
            for (int x = startingX; x < Game1.graphics.GraphicsDevice.Viewport.Width; x += Game1.tileSize)
            {
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(x, (int)startingY, 1, Game1.graphics.GraphicsDevice.Viewport.Height), ModConfig.GridColour);
            }
            for (float y = startingY; y < (float)Game1.graphics.GraphicsDevice.Viewport.Height; y += (float)Game1.tileSize)
            {
                Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(startingX, (int)y, Game1.graphics.GraphicsDevice.Viewport.Width, 1), ModConfig.GridColour);
            }
        }

        private static void Event_PreRenderHud(object sender, EventArgs e)
        {
            if (buildingPlacementTiles == null) buildingPlacementTiles = Game1.content.Load<Texture2D>("LooseSprites\\buildingPlacementTiles");

            if (Game1.activeClickableMenu == null && Game1.CurrentEvent == null && Game1.gameMode == Game1.playingGameMode)
            {
                Vector2 mousePositionTile = new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize));

                if (Game1.player.ActiveObject != null)
                {
                    if (ModConfig.SprinklerShapes.ContainsKey(Game1.player.ActiveObject.parentSheetIndex))
                    {
                        RenderSprinklerHighlight(Game1.player.ActiveObject.parentSheetIndex, mousePositionTile);
                        return; //don't want mess with people trying to place things.
                    }

                    if (Game1.player.ActiveObject.bigCraftable && Game1.player.ActiveObject.Name.Contains("arecrow"))
                    {
                        RenderScarecrowHighlight(mousePositionTile);
                        return; //don't want to mess with people trying to place things.
                    }
                }

                if (extraInfoActive)
                {
                    if (Game1.currentLocation.objects.ContainsKey(mousePositionTile))
                    {
                        StardewValley.Object hoveredObject = Game1.currentLocation.objects[mousePositionTile];

                        if (ModConfig.SprinklerShapes.ContainsKey(hoveredObject.parentSheetIndex))
                        {
                            RenderSprinklerHighlight(hoveredObject.parentSheetIndex, mousePositionTile);
                        }

                        if (hoveredObject.bigCraftable && hoveredObject.Name.Contains("arecrow"))
                        {
                            RenderScarecrowHighlight(mousePositionTile);
                        }
                    }

                    RenderGrid();
                }
            }


        }

        private static void Event_ChangedDayOfMonth(object sender, EventArgs e)
        {
            foreach (GameLocation location in Game1.locations)
            {
                foreach (KeyValuePair<Vector2, StardewValley.Object> objectPair in location.objects)
                {
                    StardewValley.Object obj = objectPair.Value;
                    Vector2 centerLocation = objectPair.Key;
                    if (ModConfig.SprinklerShapes.ContainsKey(obj.parentSheetIndex))
                    {
                        int[,] configGrid = ModConfig.SprinklerShapes[obj.parentSheetIndex];

                        Vector2 iterativeLocation = centerLocation;
                        int arrayHalfSizeX = configGrid.GetLength(0) / 2;
                        int arrayHalfSizeY = configGrid.GetLength(1) / 2;
                        iterativeLocation.X -= arrayHalfSizeX;
                        iterativeLocation.Y -= arrayHalfSizeY;
                        float maxX = centerLocation.X + arrayHalfSizeX + 1;
                        float maxY = centerLocation.Y + arrayHalfSizeY + 1;

                        int counterX = 0;
                        int counterY = 0;

                        while (iterativeLocation.X < maxX)
                        {
                            iterativeLocation.Y = centerLocation.Y - arrayHalfSizeY;
                            counterY = 0;
                            while (iterativeLocation.Y < maxY)
                            {
                                if (configGrid[counterX, counterY] > 0 && location.terrainFeatures.ContainsKey(iterativeLocation))
                                {
                                    if (location.terrainFeatures[iterativeLocation] is HoeDirt)
                                    {
                                        (location.terrainFeatures[iterativeLocation] as HoeDirt).state = 1;
                                    }
                                }
                                ++iterativeLocation.Y;
                                ++counterY;
                            }
                            ++iterativeLocation.X;
                            ++counterX;
                        }
                    }
                }
            }
        }

        private static void Event_LoadContent(object sender, EventArgs e)
        {
            UpdatePrices();
        }

        private static void Event_UpdateTick(object sender, EventArgs e)
        {
            if (Game1.activeClickableMenu == null && Game1.CurrentEvent == null)
            {
                KeyboardState currentKeyboardState = Keyboard.GetState();
                if (currentKeyboardState.IsKeyDown(ModConfig.ConfigKey))
                {
                    Game1.activeClickableMenu = new SprinklerShapeEditMenu();
                }

                if (currentKeyboardState.IsKeyDown(ModConfig.HighlightKey))
                {
                    if (gridKeyHeldDown == false) extraInfoActive = !extraInfoActive;
                    gridKeyHeldDown = true;
                }
                else
                {
                    gridKeyHeldDown = false;
                }
            }
        }
    }
}
