/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

// Thanks to Pathoschild's Lookup Anything, which provided the pattern code for this menu!

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Pathoschild.Stardew.UIF;

namespace ClimatesOfFerngillRebuild
{
    /// <summary>A UI which shows information about an item.</summary>
    internal class WeatherMenu : IClickableMenu
    {
        /*********
        ** Properties
        *********/
              
        /// <summary>The aspect ratio of the page background.</summary>
        private readonly Vector2 AspectRatio = new Vector2(Sprites.Letter.Sprite.Width, Sprites.Letter.Sprite.Height);

        /// <summary> The current weather status </summary>
        private readonly WeatherConditions CurrentWeather;

        /// <summary> Our Icons </summary>
        private readonly Sprites.Icons IconSheet;

        /// <summary>Whether the game's draw mode has been validated for compatibility.</summary>
        private bool ValidatedDrawMode;

        /// <summary> The text for the weather menu </summary>
        private readonly string MenuText;

        /*********
        ** Public methods
        *********/
        /****
        ** Constructors
        ****/
        /// <summary>Construct an instance.</summary>
        public WeatherMenu(Sprites.Icons Icon, WeatherConditions weat, string text)
        {
            // save data
            MenuText = text;
            CurrentWeather = weat;
            IconSheet = Icon;

            // update layout
            UpdateLayout();
        }

        /****
        ** Events
        ****/

        /// <summary>The method invoked when the player left-clicks on the lookup UI.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            HandleLeftClick(x, y);
        }

        /// <summary>The method invoked when the player right-clicks on the lookup UI.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveRightClick(int x, int y, bool playSound = true) { }

        /// <summary>The method called when the game window changes size.</summary>
        /// <param name="oldBounds">The former viewport.</param>
        /// <param name="newBounds">The new viewport.</param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            UpdateLayout();
        }

        /// <summary>The method called when the player presses a controller button.</summary>
        /// <param name="button">The controller button pressed.</param>
        public override void receiveGamePadButton(Buttons button)
        {
            switch (button)
            {
                // left click
                case Buttons.A:
                    Point p = Game1.getMousePosition();
                    HandleLeftClick(p.X, p.Y);
                    break;

                // exit
                case Buttons.B:
                    exitThisMenu();
                    break;
            }
        }

        /****
        ** Methods
        ****/

        /// <summary>Handle a left-click from the player's mouse or controller.</summary>
        /// <param name="x">The x-position of the cursor.</param>
        /// <param name="y">The y-position of the cursor.</param>
        public void HandleLeftClick(int x, int y)
        {
            // close menu when clicked outside
            if (!isWithinBounds(x, y))
                exitThisMenu();
        }

        /// <summary>Render the UI.</summary>
        /// <param name="spriteBatch">The sprite batch being drawn.</param>
        public override void draw(SpriteBatch spriteBatch)
        {
            // disable when game is using immediate sprite sorting
            if (!ValidatedDrawMode)
            {
                IReflectedField<SpriteSortMode> sortModeField =
                    ClimatesOfFerngill.Reflection.GetField<SpriteSortMode>(Game1.spriteBatch, "spriteSortMode", required: false) // XNA
                    ?? ClimatesOfFerngill.Reflection.GetField<SpriteSortMode>(Game1.spriteBatch, "_sortMode"); // MonoGame
                if (sortModeField.GetValue() == SpriteSortMode.Immediate)
                {
                    ClimatesOfFerngill.Logger.Log("Aborted the weather draw because the game's current rendering mode isn't compatible with the mod's UI. This only happens in rare cases (e.g. the Stardew Valley Fair).", LogLevel.Warn);
                    exitThisMenu(playSound: false);
                    return;
                }
                ValidatedDrawMode = true;
            }

            // calculate dimensions
            int x = xPositionOnScreen;
            int y = yPositionOnScreen;
            const int gutter = 15;
            float leftOffset = gutter;
            float topOffset = gutter;
            float contentWidth = width - gutter * 2;
            float contentHeight = height - gutter * 2;

            // get font
            SpriteFont font = Game1.smallFont;
            float lineHeight = font.MeasureString("ABC").Y;

            //at this point I'm going to manually put this in as I don't need in many places,
            // and I kinda want to have this where I can understand what it's for 
            float spaceWidth = DrawHelper.GetSpaceWidth(font);

            // draw background
            // (This uses a separate sprite batch because it needs to be drawn before the
            // foreground batch, and we can't use the foreground batch because the background is
            // outside the clipping area.)
            spriteBatch.DrawSprite(Sprites.Letter.Sheet, Sprites.Letter.Sprite, x, y, scale: width / (float)Sprites.Letter.Sprite.Width);
            
            // begin draw
            
            // draw weather icon
            spriteBatch.Draw(IconSheet.WeatherSource, new Vector2(x + leftOffset, y + topOffset), Sprites.Icons.GetWeatherSprite(CurrentWeather.GetCurrentConditions()), Color.White);
            leftOffset += 72;

            // draw text as sent from outside the menu
            
            float wrapWidth = width - leftOffset - gutter;
            {
                Vector2 textSize = spriteBatch.DrawTextBlock(font, MenuText, new Vector2(x + leftOffset, y + topOffset), wrapWidth);
                topOffset += textSize.Y + lineHeight;
            }
            

            drawMouse(Game1.spriteBatch);
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Update the layout dimensions based on the current game scale.</summary>
        private void UpdateLayout()
        {
            // update size
            width = Math.Min(Game1.tileSize * 14, Game1.viewport.Width);
            height = Math.Min((int)(AspectRatio.Y / AspectRatio.X * width), Game1.viewport.Height);

            // update position
            Vector2 origin = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
            xPositionOnScreen = (int)origin.X;
            yPositionOnScreen = (int)origin.Y;

            // update up/down buttons
            int x = xPositionOnScreen;
            int y = yPositionOnScreen;
            int gutter = 16;
            float contentHeight = height - gutter * 2;
        }

        /// <summary>The method invoked when an unhandled exception is intercepted.</summary>
        /// <param name="ex">The intercepted exception.</param>
#pragma warning disable IDE0051 // Remove unused private members
        private void OnDrawError(Exception ex)
#pragma warning restore IDE0051 // Remove unused private members
        {
            ClimatesOfFerngill.Logger.Log($"handling an error in the draw code {ex}", LogLevel.Error);
        }
    }
}
