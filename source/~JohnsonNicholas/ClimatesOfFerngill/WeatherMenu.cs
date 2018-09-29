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

        /// <summary>Encapsulates logging and monitoring.</summary>
        private readonly IMonitor Monitor;

        /// <summary>Simplifies access to private game code.</summary>
        private readonly IReflectionHelper Reflection;

        /// <summary> Configuration Options </summary>
        private WeatherConfig OurConfig;
        
        /// <summary>The aspect ratio of the page background.</summary>
        private readonly Vector2 AspectRatio = new Vector2(Sprites.Letter.Sprite.Width, Sprites.Letter.Sprite.Height);

        /// <summary> To da Moon, Princess!  </summary>
        private SDVMoon OurMoon;

        /// <summary> The current weather status </summary>
        private WeatherConditions CurrentWeather;

        /// <summary> Our Icons </summary>
        private Sprites.Icons IconSheet;

        ///<summary>This contains the text for various things</summary>
        private ITranslationHelper Helper;

        /// <summary>Whether the game's draw mode has been validated for compatibility.</summary>
        private bool ValidatedDrawMode;

        /// <summary> The text for the weather menu </summary>
        private string MenuText;

        /*********
        ** Public methods
        *********/
        /****
        ** Constructors
        ****/
        /// <summary>Construct an instance.</summary>
        /// <param name="monitor">Encapsulates logging and monitoring.</param>
        public WeatherMenu(IMonitor monitor, IReflectionHelper reflectionHelper, Sprites.Icons Icon, ITranslationHelper Helper, WeatherConditions weat, SDVMoon Termina, WeatherConfig ModCon, string text)
        {
            // save data
            this.MenuText = text;
            this.Monitor = monitor;
            this.Reflection = reflectionHelper;
            this.Helper = Helper;
            this.CurrentWeather = weat;
            this.IconSheet = Icon;
            this.OurMoon = Termina;
            this.OurConfig = ModCon;

            // update layout
            this.UpdateLayout();
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
            this.HandleLeftClick(x, y);
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
            this.UpdateLayout();
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
                    this.HandleLeftClick(p.X, p.Y);
                    break;

                // exit
                case Buttons.B:
                    this.exitThisMenu();
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
            if (!this.isWithinBounds(x, y))
                this.exitThisMenu();
        }

        /// <summary>Render the UI.</summary>
        /// <param name="spriteBatch">The sprite batch being drawn.</param>
        public override void draw(SpriteBatch spriteBatch)
        {
            // disable when game is using immediate sprite sorting
            if (!this.ValidatedDrawMode)
            {
                IReflectedField<SpriteSortMode> sortModeField =
                    this.Reflection.GetField<SpriteSortMode>(Game1.spriteBatch, "spriteSortMode", required: false) // XNA
                    ?? this.Reflection.GetField<SpriteSortMode>(Game1.spriteBatch, "_sortMode"); // MonoGame
                if (sortModeField.GetValue() == SpriteSortMode.Immediate)
                {
                    this.Monitor.Log("Aborted the weather draw because the game's current rendering mode isn't compatible with the mod's UI. This only happens in rare cases (e.g. the Stardew Valley Fair).", LogLevel.Warn);
                    this.exitThisMenu(playSound: false);
                    return;
                }
                this.ValidatedDrawMode = true;
            }

            // calculate dimensions
            int x = this.xPositionOnScreen;
            int y = this.yPositionOnScreen;
            const int gutter = 15;
            float leftOffset = gutter;
            float topOffset = gutter;
            float contentWidth = this.width - gutter * 2;
            float contentHeight = this.height - gutter * 2;
            //int tableBorderWidth = 1;

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
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
            spriteBatch.DrawSprite(Sprites.Letter.Sheet, Sprites.Letter.Sprite, x, y, scale: width / (float)Sprites.Letter.Sprite.Width);
            //spriteBatch.End();
            
            // begin draw
            
            // draw weather icon
            spriteBatch.Draw(IconSheet.WeatherSource, new Vector2(x + leftOffset, y + topOffset), IconSheet.GetWeatherSprite(CurrentWeather.GetCurrentConditions()), Color.White);
            leftOffset += 72;
            string weatherString = "";

            // draw text as sent from outside the menu
            float wrapWidth = this.width - leftOffset - gutter;
            {
                Vector2 textSize = spriteBatch.DrawTextBlock(font, MenuText, new Vector2(x + leftOffset, y + topOffset), wrapWidth);
                topOffset += textSize.Y;
                topOffset += lineHeight;

            }

            //draw moon info
            spriteBatch.Draw(IconSheet.MoonSource, new Vector2(x + 15, y + topOffset), 
                IconSheet.GetMoonSprite(OurMoon.CurrentPhase), Color.White);

            weatherString = Helper.Get("moon-desc.desc_moonphase", 
                new { moonPhase = SDVMoon.DescribeMoonPhase(OurMoon.CurrentPhase, Helper)});

            Vector2 moonText = spriteBatch.DrawTextBlock(font, 
                weatherString, new Vector2(x + leftOffset, y + topOffset), wrapWidth);

            topOffset += lineHeight; //stop moon from being cut off.

            this.drawMouse(Game1.spriteBatch);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Update the layout dimensions based on the current game scale.</summary>
        private void UpdateLayout()
        {
            // update size
            this.width = Math.Min(Game1.tileSize * 14, Game1.viewport.Width);
            this.height = Math.Min((int)(this.AspectRatio.Y / this.AspectRatio.X * this.width), Game1.viewport.Height);

            // update position
            Vector2 origin = Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height);
            this.xPositionOnScreen = (int)origin.X;
            this.yPositionOnScreen = (int)origin.Y;

            // update up/down buttons
            int x = this.xPositionOnScreen;
            int y = this.yPositionOnScreen;
            int gutter = 16;
            float contentHeight = this.height - gutter * 2;
        }

        /// <summary>The method invoked when an unhandled exception is intercepted.</summary>
        /// <param name="ex">The intercepted exception.</param>
        private void OnDrawError(Exception ex)
        {
            Monitor.Log($"handling an error in the draw code {ex}", LogLevel.Error);
        }
    }
}
