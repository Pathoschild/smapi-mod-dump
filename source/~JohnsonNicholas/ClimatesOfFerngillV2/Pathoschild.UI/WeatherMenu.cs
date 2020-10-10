/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using TwilightShards.ClimatesOfFerngillV2;

namespace Pathoschild.UI
{
    /// <summary>A UI which shows information about an item.</summary>
    internal class WeatherMenu : BaseMenu, IDisposable
    {
        /*********
        ** Fields
        *********/
        /// <summary>Encapsulates logging and monitoring.</summary>
        private readonly IMonitor Monitor;

        /// <summary>The aspect ratio of the page background.</summary>
        private readonly Vector2 AspectRatio = new Vector2(Letter.Sheet.Width, Letter.Sheet.Height);

        /// <summary>Simplifies access to private game code.</summary>
        private readonly IReflectionHelper Reflection;

        /// <summary>The amount to scroll long content on each up/down scroll.</summary>
        private readonly int ScrollAmount;

        /// <summary>The clickable 'scroll up' icon.</summary>
        private readonly ClickableTextureComponent ScrollUpButton;

        /// <summary>The clickable 'scroll down' icon.</summary>
        private readonly ClickableTextureComponent ScrollDownButton;

        /// <summary>The spacing around the scroll buttons.</summary>
        private readonly int ScrollButtonGutter = 15;

        /// <summary>The maximum pixels to scroll.</summary>
        private int MaxScroll;

        /// <summary>The number of pixels to scroll.</summary>
        private int CurrentScroll;

        /// <summary>Whether the game's draw mode has been validated for compatibility.</summary>
        private bool ValidatedDrawMode;

        /// <summary>Whether the game HUD was enabled when the menu was opened.</summary>
        private readonly bool WasHudEnabled;

        /// <summary>Whether to exit the menu on the next update tick.</summary>
        private bool ExitOnNextTick;

        /// <summary> The text for the weather menu </summary>
        private readonly string MenuText;

        /*********
        ** Public methods
        *********/
        /****
        ** Constructors
        ****/
        /// <summary>Construct an instance.</summary>
        /// <param name="monitor">Encapsulates logging and monitoring.</param>
        /// <param name="reflectionHelper">Simplifies access to private game code.</param>
        /// <param name="scroll">The amount to scroll long content on each up/down scroll.</param>
        /// <param name="text">The text being displayed in the helper.</param>
        public WeatherMenu(IMonitor monitor, IReflectionHelper reflectionHelper, int scroll, string text)
        {
            // save data
            this.Monitor = monitor;
            this.Reflection = reflectionHelper;
            this.ScrollAmount = scroll;
            this.WasHudEnabled = Game1.displayHUD;
            this.MenuText = text;

            // add scroll buttons
            this.ScrollUpButton = new ClickableTextureComponent(Rectangle.Empty, Icons.Sheet, Icons.UpArrow, 1);
            this.ScrollDownButton = new ClickableTextureComponent(Rectangle.Empty, Icons.Sheet, Icons.DownArrow, 1);

            // update layout
            this.UpdateLayout();

            // hide game HUD
            Game1.displayHUD = false;
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

        /// <summary>The method invoked when the player scrolls the mouse wheel on the lookup UI.</summary>
        /// <param name="direction">The scroll direction.</param>
        public override void receiveScrollWheelAction(int direction)
        {
            if (direction > 0)    // positive number scrolls content up
                this.ScrollUp();
            else
                this.ScrollDown();
        }

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

                // scroll up
                case Buttons.RightThumbstickUp:
                    this.ScrollUp();
                    break;

                // scroll down
                case Buttons.RightThumbstickDown:
                    this.ScrollDown();
                    break;
            }
        }

        /// <summary>Update the menu state if needed.</summary>
        /// <param name="time">The elapsed game time.</param>
        public override void update(GameTime time)
        {
            if (this.ExitOnNextTick && this.readyToClose())
                this.exitThisMenu();
            else
                base.update(time);
        }

        /****
        ** Methods
        ****/
        /// <summary>Exit the menu at the next safe opportunity.</summary>
        /// <remarks>This circumvents an issue where the game may freeze in some cases like the load selection screen when the menu is exited at an arbitrary time.</remarks>
        public void QueueExit()
        {
            this.ExitOnNextTick = true;
        }

        /// <summary>Scroll up the menu content by the specified amount (if possible).</summary>
        public void ScrollUp()
        {
            this.CurrentScroll -= this.ScrollAmount;
        }

        /// <summary>Scroll down the menu content by the specified amount (if possible).</summary>
        public void ScrollDown()
        {
            this.CurrentScroll += this.ScrollAmount;
        }

        /// <summary>Handle a left-click from the player's mouse or controller.</summary>
        /// <param name="x">The x-position of the cursor.</param>
        /// <param name="y">The y-position of the cursor.</param>
        public void HandleLeftClick(int x, int y)
        {
            // close menu when clicked outside
            if (!this.isWithinBounds(x, y))
                this.exitThisMenu();

            // scroll up or down
            else if (this.ScrollUpButton.containsPoint(x, y))
                this.ScrollUp();
            else if (this.ScrollDownButton.containsPoint(x, y))
                this.ScrollDown();
        }

        /// <summary>Render the UI.</summary>
        /// <param name="spriteBatch">The sprite batch being drawn.</param>
        public override void draw(SpriteBatch spriteBatch)
        {
                // disable when game is using immediate sprite sorting
                // (This prevents Lookup Anything from creating new sprite batches, which breaks its core rendering logic.
                // Fortunately this very rarely happens; the only known case is the Stardew Valley Fair, when the only thing
                // you can look up anyway is the farmer.)
                if (!this.ValidatedDrawMode)
                {
                    IReflectedField<SpriteSortMode> sortModeField =
                        this.Reflection.GetField<SpriteSortMode>(Game1.spriteBatch, "spriteSortMode", required: false) // XNA
                        ?? this.Reflection.GetField<SpriteSortMode>(Game1.spriteBatch, "_sortMode"); // MonoGame
                    if (sortModeField.GetValue() == SpriteSortMode.Immediate)
                    {
                        this.Monitor.Log("Aborted the lookup because the game's current rendering mode isn't compatible with the mod's UI. This only happens in rare cases (e.g. the Stardew Valley Fair).", LogLevel.Warn);
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

                // get font
                SpriteFont font = Game1.smallFont;
                float lineHeight = font.MeasureString("ABC").Y;
                //float spaceWidth = DrawHelper.GetSpaceWidth(font);

                // draw background
                // (This uses a separate sprite batch because it needs to be drawn before the
                // foreground batch, and we can't use the foreground batch because the background is
                // outside the clipping area.)
                using (SpriteBatch backgroundBatch = new SpriteBatch(Game1.graphics.GraphicsDevice))
                {
                    backgroundBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
                    backgroundBatch.DrawSprite(Letter.Sheet, Letter.Sprite, x, y, scale: this.width / (float)Letter.Sprite.Width);
                    backgroundBatch.End();
                }

                // draw foreground
                // (This uses a separate sprite batch to set a clipping area for scrolling.)
                using (SpriteBatch contentBatch = new SpriteBatch(Game1.graphics.GraphicsDevice))
                {
                    GraphicsDevice device = Game1.graphics.GraphicsDevice;
                    Rectangle prevScissorRectangle = device.ScissorRectangle;
                    try
                    {
                        // begin draw
                        device.ScissorRectangle = new Rectangle(x + gutter, y + gutter, (int)contentWidth, (int)contentHeight);
                        contentBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, new RasterizerState { ScissorTestEnable = true });

                        // scroll view
                        this.CurrentScroll = Math.Max(0, this.CurrentScroll); // don't scroll past top
                        this.CurrentScroll = Math.Min(this.MaxScroll, this.CurrentScroll); // don't scroll past bottom
                        topOffset -= this.CurrentScroll; // scrolled down == move text up

                        /*
                        // draw weather icon
                        spriteBatch.Draw(IconSheet.WeatherSource, new Vector2(x + leftOffset, y + topOffset), Sprites.Icons.GetWeatherSprite(CurrentWeather.GetCurrentConditions()), Color.White);
                        leftOffset += 72;
                        */

                        // draw text as sent from outside the menu
            
                        float wrapWidth = width - leftOffset - gutter;
                        {
                            Vector2 textSize = spriteBatch.DrawTextBlock(font, MenuText, new Vector2(x + leftOffset, y + topOffset), wrapWidth);
                            topOffset += textSize.Y + lineHeight;
                        }

                        // update max scroll
                        this.MaxScroll = Math.Max(0, (int)(topOffset - contentHeight + this.CurrentScroll));

                        // draw scroll icons
                        if (this.MaxScroll > 0 && this.CurrentScroll > 0)
                            this.ScrollUpButton.draw(contentBatch);
                        if (this.MaxScroll > 0 && this.CurrentScroll < this.MaxScroll)
                            this.ScrollDownButton.draw(spriteBatch);

                        // end draw
                        contentBatch.End();
                    }
                    catch (ArgumentException ex) when (!BaseMenu.UseSafeDimensions && ex.ParamName == "value" && ex.StackTrace.Contains("Microsoft.Xna.Framework.Graphics.GraphicsDevice.set_ScissorRectangle"))
                    {
                        this.Monitor.Log("The viewport size seems to be inaccurate. Enabling compatibility mode; lookup menu may be misaligned.", LogLevel.Warn);
                        this.Monitor.Log(ex.ToString());
                        BaseMenu.UseSafeDimensions = true;
                        this.UpdateLayout();
                    }
                    finally
                    {
                        device.ScissorRectangle = prevScissorRectangle;
                    }
                }

                // draw cursor
                this.drawMouse(Game1.spriteBatch);
        }

        /// <summary>Clean up after the menu when it's disposed.</summary>
        public void Dispose()
        {
            this.CleanupImpl();
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Update the layout dimensions based on the current game scale.</summary>
        private void UpdateLayout()
        {
            Point viewport = this.GetViewportSize();

            // update size
            this.width = Math.Min(Game1.tileSize * 20, viewport.X);
            this.height = Math.Min((int)(this.AspectRatio.Y / this.AspectRatio.X * this.width), viewport.Y);

            // update position
            Vector2 origin = new Vector2(viewport.X / 2 - this.width / 2, viewport.Y / 2 - this.height / 2); // derived from Utility.getTopLeftPositionForCenteringOnScreen, adjusted to account for possibly different GPU viewport size
            this.xPositionOnScreen = (int)origin.X;
            this.yPositionOnScreen = (int)origin.Y;

            // update up/down buttons
            int x = this.xPositionOnScreen;
            int y = this.yPositionOnScreen;
            int gutter = this.ScrollButtonGutter;
            float contentHeight = this.height - gutter * 2;
            this.ScrollUpButton.bounds = new Rectangle(x + gutter, (int)(y + contentHeight - Icons.UpArrow.Height - gutter - Icons.DownArrow.Height), Icons.UpArrow.Height, Icons.UpArrow.Width);
            this.ScrollDownButton.bounds = new Rectangle(x + gutter, (int)(y + contentHeight - Icons.DownArrow.Height), Icons.DownArrow.Height, Icons.DownArrow.Width);
        }

        /// <summary>Perform any cleanup needed when the menu exits.</summary>
        protected override void cleanupBeforeExit()
        {
            this.CleanupImpl();
            base.cleanupBeforeExit();
        }

        /// <summary>Perform cleanup specific to the lookup menu.</summary>
        private void CleanupImpl()
        {
            Game1.displayHUD = this.WasHudEnabled;
        }
    }
}

