using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace FishPreview
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config;

        private bool isCatching = false;
        private int fishId = 0;
        private Object fishSprite;
        private bool showFish;
        private bool showText;
        private string textValue;
        private IList<string> displayOrder;
        private Vector2 textSize;
        private int margin = 18; 
        private int spriteSize = 64;
        private float scale = 0.7f;
        private int boxwidth;
        private int boxheight;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.Display.RenderedActiveMenu += this.OnRenderMenu;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game is launched, right before the first update tick.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // read the Config for display position and get list priority for displayOrder
            RefreshConfig();
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (Game1.player == null || !Game1.player.IsLocalPlayer)
            {
                return;
            }

            if (e.NewMenu is BobberBar bar)
            {
                isCatching = true;
                this.Monitor.Log($"{Game1.player.Name} has started fishing.", LogLevel.Trace);

                fishId = 0;
            }
            else
            {
                isCatching = false;
            }

            if (e.OldMenu is BobberBar)
            {
                this.Monitor.Log($"{Game1.player.Name} is done fishing.", LogLevel.Trace);
            }
        }

        /// <summary>When a menu is open, raised after that menu is drawn to the sprite batch but before it's rendered to the screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnRenderMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (Game1.player == null || !Game1.player.IsLocalPlayer)
            {
                return;
            }

            if (Game1.activeClickableMenu is BobberBar bar && isCatching == true)
            {
                // stop drawing on fadeOut
                if (Helper.Reflection.GetField<bool>(bar, "fadeOut").GetValue())
                {
                    return;
                }

                // figure out which fish is being caught
                int newFishId = Helper.Reflection.GetField<int>(bar, "whichFish").GetValue();

                // check if fish has changed somehow. If yes, re-make the fish sprite and its display text.
                GetFishData(newFishId);

                // call a function to position and draw the box
                DrawFishDisplay(displayOrder, bar);

                /**
                // draw all 6 display positions (testing)
                IList<List<string>> allPositions = new List<List<string>>() { 
                    new List<string> { "Top" }, 
                    new List<string> { "UpperRight" }, 
                    new List<string> { "UpperLeft" }, 
                    new List<string> { "Bottom" },
                    new List<string> { "LowerRight" },
                    new List<string> { "LowerLeft" } 
                };
                foreach (IList<string> testOrder in allPositions)
                {
                    DrawFishDisplay(testOrder, bar);
                }**/
            }
        }

        private void GetFishData(int newFishId)
        {
            if (newFishId != fishId) // catching a new fish OR the fish species has changed mid-cast
            {
                fishId = newFishId;
                this.Monitor.Log($"Currently catching: {fishId}", LogLevel.Trace);

                // save fish object to use in drawing // check for errors?
                fishSprite = new Object(fishId, 1);

                // determine if species has been caught before
                bool caughtSpecies = Game1.player.fishCaught.ContainsKey(fishId) && Game1.player.fishCaught[fishId][0] > 0;

                // is it a legendary fish?
                bool isLegendary = this.Helper.Reflection.GetMethod(typeof(FishingRod), "isFishBossFish").Invoke<bool>(fishId);
                this.Monitor.Log($"Catching legendary fish? {isLegendary}", LogLevel.Trace);

                // determine value of showFish value
                showFish = Config.ShowUncaughtFishSpecies || caughtSpecies || (Config.AlwaysShowLegendaryFish && isLegendary);

                // determine value of showText value
                showText = Config.ShowFishName;

                // determine text to show if true
                if (showText && showFish)
                {
                    textValue = fishSprite.DisplayName;  // does it need translation help? (Seems to work with French already)
                    this.Monitor.Log($"Fish name: {textValue}", LogLevel.Trace);
                }
                else
                {
                    textValue = "???";
                }

                // determine width and height of display box
                boxwidth = 150; boxheight = 100;

                textSize = Game1.dialogueFont.MeasureString(textValue) * scale;

                if (showText && showFish) { boxwidth = Math.Max(150, (2 * margin) + (int)textSize.X); }
                if (showText) { boxheight += (int)textSize.Y; }
            }
        }

        private void DrawFishDisplay(IList<string> positions, BobberBar bar)
        {
            // call a function to determine the x and y coords
            DetermineCoordinates(positions, bar, out int x, out int y);
            // call a function to draw the box
            DrawAtCoordinates(x, y);
        }

        private void DetermineCoordinates(IList<string> positions, BobberBar bar, out int x, out int y)
        {
            // define offset values
            int xLeftOffset = -32; int xCenterOffset = 32; int xRightOffset = 80;
            int yTopOffset = -32; int yUpperOffset = 0; int yLowerOffset = -40; int yBottomOffset = -12; 

            // determine x and y positions
            foreach (string position in positions)
            {
                // set the correct display coordinates from position values in order of priority
                switch (position)
                {
                    case "Top":
                        x = bar.xPositionOnScreen + (bar.width / 2) - (boxwidth / 2) + xCenterOffset;
                        y = bar.yPositionOnScreen - boxheight + yTopOffset;
                        break;
                    case "UpperRight":
                        x = bar.xPositionOnScreen + bar.width + xRightOffset;
                        y = bar.yPositionOnScreen + yUpperOffset;
                        break;
                    case "UpperLeft":
                        x = bar.xPositionOnScreen - boxwidth + xLeftOffset;
                        y = bar.yPositionOnScreen + yUpperOffset;
                        break;
                    case "Bottom":
                        x = bar.xPositionOnScreen + (bar.width / 2) - (boxwidth / 2) + xCenterOffset;
                        y = bar.yPositionOnScreen + bar.height + yBottomOffset;
                        break;
                    case "LowerRight":
                        x = bar.xPositionOnScreen + bar.width + xRightOffset;
                        y = bar.yPositionOnScreen + bar.height - boxheight + yLowerOffset;
                        break;
                    case "LowerLeft":
                        x = bar.xPositionOnScreen - boxwidth +  xLeftOffset;
                        y = bar.yPositionOnScreen + bar.height - boxheight +yLowerOffset;
                        break;
                    default:
                        // default to UpperRight position
                        x = bar.xPositionOnScreen + bar.width + xRightOffset;
                        y = bar.yPositionOnScreen + yUpperOffset;
                        //this.Monitor.Log($"Invalid position {position} listed in displayOrder.", LogLevel.Debug);
                        break;
                }

                // if the box display is in bounds, break the loop. Otherwise proceed to alternative display position(s).
                if (x >= 0 && y >= 0 && x + boxwidth <= Game1.viewport.Width && y + boxheight <= Game1.viewport.Height)
                {
                    return;
                }
            }
            // if no suitable location found in foreach, default to UpperRight position
            x = bar.xPositionOnScreen + bar.width + xRightOffset;
            y = bar.yPositionOnScreen + yUpperOffset;
            //this.Monitor.Log($"No suitable coordinate position found in displayOrder.", LogLevel.Debug);
        }

        private void DrawAtCoordinates(int x, int y)
        {   
            // draw box of height and width at location
            IClickableMenu.drawTextureBox(Game1.spriteBatch, x, y, boxwidth, boxheight, Color.White);

            // if showFish, center the fish x
            if (showFish)
            {
                fishSprite.drawInMenu(Game1.spriteBatch, new Vector2(x + (boxwidth / 2) - (spriteSize / 2), y + margin), 1.0f, 1.0f, 1.0f, StackDrawType.Hide);
                    
                // if showFish and showText, center the text x below the fish
                if (showText)
                {
                    Game1.spriteBatch.DrawString(Game1.dialogueFont, textValue, new Vector2(x + (boxwidth / 2) - ((int)textSize.X / 2), y + spriteSize + margin), Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                }
            }
            // else (if not showFish), center the text x&y
            else
            {
                Game1.spriteBatch.DrawString(Game1.dialogueFont, textValue, new Vector2(x + (boxwidth / 2) - ((int)textSize.X / 2), y + (boxheight / 2) - ((int)textSize.Y / 2)), Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }

        private void RefreshConfig()
        {
            switch (Config.FishDisplayPosition)
            {
                case "Top":
                    displayOrder = new List<string>() { "Top", "UpperRight", "UpperLeft", "LowerRight" };
                    break;
                case "UpperRight":
                    displayOrder = new List<string>() { "UpperRight", "UpperLeft", "LowerRight" };
                    break;
                case "UpperLeft":
                    displayOrder = new List<string>() { "UpperLeft", "UpperRight", "LowerLeft" };
                    break;
                case "Bottom":
                    displayOrder = new List<string>() { "Bottom", "LowerRight", "LowerLeft", "UpperRight" };
                    break;
                case "LowerRight":
                    displayOrder = new List<string>() { "LowerRight", "LowerLeft", "UpperRight" };
                    break;
                case "LowerLeft":
                    displayOrder = new List<string>() { "LowerLeft", "LowerRight", "UpperLeft" };
                    break;
                default:
                    displayOrder = new List<string>() { "UpperRight", "UpperLeft", "LowerLeft" };
                    this.Monitor.Log($"Invalid config value {Config.FishDisplayPosition} for FishDisplayPosition. Valid entries include Top, Bottom, UpperRight, UpperLeft, LowerRight and LowerLeft.", LogLevel.Warn);
                    break;
            }
        }
    }
}