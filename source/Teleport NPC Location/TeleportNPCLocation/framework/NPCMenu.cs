/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chencrstu/TeleportNPCLocation
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using static System.Net.Mime.MediaTypeNames;

namespace TeleportNPCLocation.framework
{
	public class NPCMenu : IClickableMenu
    {
        /// <summary>The aspect ratio of the page background.</summary>
        private readonly Vector2 AspectRatio = new Vector2(320, 180);

        /// <summary>The blend state to use when rendering the content sprite batch.</summary>
        private readonly BlendState ContentBlendState = new()
        {
            AlphaBlendFunction = BlendFunction.Add,
            AlphaSourceBlend = Blend.Zero,
            AlphaDestinationBlend = Blend.One,

            ColorBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.SourceAlpha,
            ColorDestinationBlend = Blend.InverseSourceAlpha
        };

        /// <summary>The nps list.</summary>
        private List<NPC> npcList;

        /// <summary>Encapsulates logging and monitoring.</summary>
        private readonly IMonitor Monitor;

        /// <summary>The mod configuration.</summary>
        private readonly ModConfig Config;

        /// <summary>The amount to scroll long content on each up/down scroll.</summary>
        private readonly int ScrollAmount;

        /// <summary>The maximum pixels to scroll.</summary>
        private int MaxScroll;

        /// <summary>The number of pixels to scroll.</summary>
        private int CurrentScroll;

        /// <summary>The clickable 'scroll up' icon.</summary>
        private readonly ClickableTextureComponent ScrollUpButton;

        /// <summary>The clickable 'scroll down' icon.</summary>
        private readonly ClickableTextureComponent ScrollDownButton;

        /// <summary>The subjects available for searching indexed by name.</summary>
        private readonly ILookup<string, NPC> SearchLookup;

        /// <summary>The current search results.</summary>
        private IEnumerable<NPC> SearchResults = Enumerable.Empty<NPC>();

        /// <summary>The search input box.</summary>
        private readonly SearchTextBox SearchTextbox;

        /// <summary> icon.</summary>
        private List<ClickableTextureComponent>
            teleportComponents;

        /// <summary>Whether the game HUD was enabled when the menu was opened.</summary>
        private readonly bool WasHudEnabled;

        /// <summary>Whether to exit the menu on the next update tick.</summary>
        private bool ExitOnNextTick;

        /// <summary>The spacing around the scroll buttons.</summary>
        private readonly int ScrollButtonGutter = 15;

        /*********
        ** Fields
        *********/
        /// <summary>Whether to use dimensions which are guaranteed to fit within the screen.</summary>
        /// <remarks>This is enabled automatically when the menu detects a rare scissor rectangle error ("The scissor rectangle cannot be larger than or outside of the current render target bounds"). The menu will usually be pushed into the top-left corner when this is active, so it be disabled unless it's needed.</remarks>
        protected static bool UseSafeDimensions { get; set; }

        public NPCMenu(List<NPC> npcList, IMonitor monitor, ModConfig config, int scroll)
		{
            // save data
            this.npcList = npcList;
            this.Monitor = monitor;
            this.Config = config;
            this.ScrollAmount = scroll;
            this.WasHudEnabled = Game1.displayHUD;
            this.SearchLookup = npcList.ToLookup(p => p.displayName, StringComparer.OrdinalIgnoreCase);
            this.SearchResults = this.npcList;

            // add scroll buttons
            this.ScrollUpButton = new ClickableTextureComponent(Rectangle.Empty, CommonSprites.Icons.Sheet, CommonSprites.Icons.UpArrow, 1);
            this.ScrollDownButton = new ClickableTextureComponent(Rectangle.Empty, CommonSprites.Icons.Sheet, CommonSprites.Icons.DownArrow, 1);
            this.SearchTextbox = new SearchTextBox(Game1.smallFont, Color.Black);

            this.teleportComponents = new List<ClickableTextureComponent>();

            // update layout
            this.UpdateLayout();
            this.SearchTextbox.Select();
            this.SearchTextbox.OnChanged += (_, text) => this.ReceiveSearchTextboxChanged(text);

            // hide game HUD
            Game1.displayHUD = false;
        }

        /// <summary>The method invoked when the player changes the search text.</summary>
        /// <param name="search">The new search text.</param>
        private void ReceiveSearchTextboxChanged(string? search)
        {
            // get search words
            string[] words = (search ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (!words.Any())
            {
                this.SearchResults = this.npcList;
                return;
            }

            // get matches
            this.SearchResults = this.SearchLookup
                .Where(entry => words.All(word => entry.Key.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0))
                .SelectMany(entry => entry)
                .OrderBy(npc => npc.displayName, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }


        /****
        ** Events
        ****/
        /// <summary>The method invoked when the player left-clicks on the npc menu.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            this.HandleLeftClick(x, y);
        }

        /// <summary>The method invoked when the player right-clicks on the npc menu.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveRightClick(int x, int y, bool playSound = true) { }

        /// <summary>The method invoked when the player scrolls the mouse wheel on the npc menu.</summary>
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

        /// <summary>The method invoked when the player presses an input button.</summary>
        /// <param name="key">The pressed input.</param>
        public override void receiveKeyPress(Keys key)
        {
            // deliberately avoid calling base, which may let another key close the menu
            if (key.Equals(Keys.Escape))
                this.exitThisMenu();
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


        /// <summary>Exit the menu at the next safe opportunity.</summary>
        /// <remarks>This circumvents an issue where the game may freeze in some cases like the load selection screen when the menu is exited at an arbitrary time.</remarks>
        public void QueueExit()
        {
            this.ExitOnNextTick = true;
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

        /// <summary>Clean up after the menu when it's disposed.</summary>
        public void Dispose()
        {
            this.ContentBlendState.Dispose();
            this.SearchTextbox.Dispose();

            this.CleanupImpl();
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
            this.SearchTextbox.Dispose();
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Update the layout dimensions based on the current game scale.</summary>
        private void UpdateLayout()
        {
            Point viewport = this.GetViewportSize();

            this.width = Math.Min(Game1.tileSize * 20, viewport.X);
            this.height = Math.Min((int)(this.AspectRatio.Y / this.AspectRatio.X * this.width), viewport.Y);

            Vector2 origin = new Vector2(viewport.X / 2 - this.width / 2, viewport.Y / 2 - this.height / 2); // derived from Utility.getTopLeftPositionForCenteringOnScreen, adjusted to account for possibly different GPU viewport size
            this.xPositionOnScreen = (int)origin.X;
            this.yPositionOnScreen = (int)origin.Y;


            // update up/down buttons
            int x = this.xPositionOnScreen;
            int y = this.yPositionOnScreen;
            int gutter = this.ScrollButtonGutter;
            float contentHeight = this.height - gutter * 2;
            this.ScrollUpButton.bounds = new Rectangle(x + gutter, (int)(y + contentHeight - CommonSprites.Icons.UpArrow.Height - gutter - CommonSprites.Icons.DownArrow.Height), CommonSprites.Icons.UpArrow.Height, CommonSprites.Icons.UpArrow.Width);
            this.ScrollDownButton.bounds = new Rectangle(x + gutter, (int)(y + contentHeight - CommonSprites.Icons.DownArrow.Height), CommonSprites.Icons.DownArrow.Height, CommonSprites.Icons.DownArrow.Width);
        }

        /*********
        ** Protected methods
        *********/
        /// <summary>Get the viewport size adjusted for compatibility.</summary>
        protected Point GetViewportSize()
        {
            Point viewport = new Point(Game1.uiViewport.Width, Game1.uiViewport.Height);

            if (NPCMenu.UseSafeDimensions && Game1.graphics.GraphicsDevice.Viewport.Width < viewport.X)
            {
                viewport = new Point(
                    x: Math.Min(viewport.X, Game1.graphics.GraphicsDevice.Viewport.Width),
                    y: Math.Min(viewport.Y, Game1.graphics.GraphicsDevice.Viewport.Height)
                );
            }

            return viewport;
        }

        /// <inheritdoc />
        public void ScrollUp(int? amount = null)
        {
            this.CurrentScroll -= amount ?? this.ScrollAmount;
        }

        /// <inheritdoc />
        public void ScrollDown(int? amount = null)
        {
            this.CurrentScroll += amount ?? this.ScrollAmount;
        }

        /// <summary>Handle a left-click from the player's mouse or controller.</summary>
        /// <param name="x">The x-position of the cursor.</param>
        /// <param name="y">The y-position of the cursor.</param>
        public void HandleLeftClick(int x, int y)
        {
            // close menu when clicked outside
            if (!this.isWithinBounds(x, y))
                this.exitThisMenu();

            // search box
            if (this.SearchTextbox.Bounds.Contains(x, y))
                this.SearchTextbox.Select();

            // scroll up or down
            else if (this.ScrollUpButton.containsPoint(x, y))
                this.ScrollUp();
            else if (this.ScrollDownButton.containsPoint(x, y))
                this.ScrollDown();

            // teleport to npc location
            int index = 0;
            foreach (ClickableTextureComponent component in this.teleportComponents)
            {
                if (component.containsPoint(x, y))
                {
                    TeleportHelper.teleportToNPCLocation(this.SearchResults.ElementAt(index));

                    // Close this menu
                    this.exitThisMenu();
                    break;
                }
                index++;
            }
        }


        /// <summary>Render the UI.</summary>
        /// <param name="spriteBatch">The sprite batch being drawn.</param>
        public override void draw(SpriteBatch spriteBatch)
        {
            this.Monitor.InterceptErrors("drawing the npc list menu", () =>
            {
                // calculate dimensions
                int x = this.xPositionOnScreen;
                int y = this.yPositionOnScreen;
                const int gutter = 15;
                float leftOffset = gutter;
                float topOffset = gutter;
                float contentWidth = this.width - gutter * 2;
                float contentHeight = this.height - gutter * 2;
                int tableBorderWidth = 1;

                // get font
                SpriteFont font = Game1.smallFont;
                float lineHeight = font.MeasureString("ABC").Y;
                float spaceWidth = font.MeasureString("A B").X - font.MeasureString("AB").X;

                // draw background
                // (This uses a separate sprite batch because it needs to be drawn before the
                // foreground batch, and we can't use the foreground batch because the background is
                // outside the clipping area.)
                using (SpriteBatch backgroundBatch = new SpriteBatch(Game1.graphics.GraphicsDevice))
                {
                    float scale = this.width >= this.height
                        ? this.width / 320
                        : this.height / 180;

                    backgroundBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp);
                    backgroundBatch.Draw(Game1.content.Load<Texture2D>("LooseSprites\\letterBG"), new Vector2(x, y), new Rectangle(0, 0, 320, 180), Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
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
                        contentBatch.Begin(SpriteSortMode.Deferred, this.ContentBlendState, SamplerState.PointClamp, null, new RasterizerState { ScissorTestEnable = true });

                        // scroll view
                        this.CurrentScroll = Math.Max(0, this.CurrentScroll); // don't scroll past top
                        this.CurrentScroll = Math.Min(this.MaxScroll, this.CurrentScroll); // don't scroll past bottom
                        topOffset -= this.CurrentScroll; // scrolled down == move text up

                        leftOffset += 72;
                        topOffset += 36;

                        // draw fields
                        float wrapWidth = this.width - leftOffset - gutter;
                        {
                            // draw title
                            {
                                Vector2 nameSize = contentBatch.DrawTextBlock(font, $"List of teleportable NPCs", new Vector2(x + leftOffset, y + topOffset), wrapWidth, bold: false);
                                topOffset += (nameSize.Y + lineHeight);
                            }

                            // draw search text box
                            {
                                this.SearchTextbox.Bounds = new Rectangle(x: x + (int)leftOffset, y: y + (int)topOffset, width: (int)wrapWidth, height: this.SearchTextbox.Bounds.Height);
                                this.SearchTextbox.Draw(contentBatch);
                                topOffset += (this.SearchTextbox.Bounds.Height + lineHeight);
                            }

                            // draw npc list
                            this.teleportComponents = new List<ClickableTextureComponent>();
                            if (this.SearchResults.Any())
                            {
                                float cellPadding = 3;
                                float portraitWidth = NPC.portrait_width;
                                float valueWidth = wrapWidth - portraitWidth - cellPadding * 4 - tableBorderWidth;
                                int index = 0;
                                foreach (NPC npc in this.SearchResults)
                                {
                                    // draw value label
                                    if (npc.IsInvisible || npc.currentLocation == null)
                                        continue;

                                    // draw Portrait
                                    Vector2 portraitPosition = new Vector2(x + leftOffset + cellPadding, y + topOffset + cellPadding);
                                    Vector2 portraitSize = new Vector2(NPC.portrait_width, NPC.portrait_height);
                                    ClickableTextureComponent teleportButton = new ClickableTextureComponent(Rectangle.Empty, npc.Portrait, new Rectangle(0, 0, NPC.portrait_width, NPC.portrait_height), 1);
                                    teleportButton.bounds = new Rectangle((int)portraitPosition.X, (int)portraitPosition.Y, (int)portraitSize.X, (int)portraitSize.Y);
                                    teleportButton.draw(contentBatch);
                                    this.teleportComponents.Add(teleportButton);

                                    // draw value label
                                    Vector2 valuePosition = new Vector2(x + leftOffset + portraitWidth + cellPadding * 3, y + topOffset + cellPadding);
                                    string value = npc.displayName ?? npc.Name;
                                    if (this.Config.showMoreInfo)
                                    {
                                        value += $"\nlocation:{npc.currentLocation.NameOrUniqueName}";
                                    }
                                    Vector2 valueSize = contentBatch.DrawTextBlock(font, value, valuePosition, valueWidth);
                                    Vector2 rowSize = new Vector2(portraitWidth + valueWidth + cellPadding * 4, Math.Max(portraitSize.Y + cellPadding * 2, valueSize.Y + cellPadding * 2));

                                    // draw table row
                                    Color lineColor = Color.Gray;
                                    contentBatch.DrawLine(x + leftOffset, y + topOffset, new Vector2(rowSize.X, tableBorderWidth), lineColor); // top
                                    contentBatch.DrawLine(x + leftOffset, y + topOffset + rowSize.Y, new Vector2(rowSize.X, tableBorderWidth), lineColor); // bottom
                                    contentBatch.DrawLine(x + leftOffset, y + topOffset, new Vector2(tableBorderWidth, rowSize.Y), lineColor); // left
                                    contentBatch.DrawLine(x + leftOffset + portraitWidth + cellPadding * 2, y + topOffset, new Vector2(tableBorderWidth, rowSize.Y), lineColor); // middle
                                    contentBatch.DrawLine(x + leftOffset + rowSize.X, y + topOffset, new Vector2(tableBorderWidth, rowSize.Y), lineColor); // right

                                    // update offset
                                    topOffset += Math.Max(portraitSize.Y, valueSize.Y) + cellPadding * 2;
                                    index++;
                                }
                            }
                        }

                        // update max scroll
                        this.MaxScroll = Math.Max(0, (int)(topOffset - contentHeight + this.CurrentScroll));

                        // draw scroll icons
                        if (this.MaxScroll > 0 && this.CurrentScroll > 0)
                            this.ScrollUpButton.draw(spriteBatch);
                        if (this.MaxScroll > 0 && this.CurrentScroll < this.MaxScroll)
                            this.ScrollDownButton.draw(spriteBatch);

                        // end draw
                        contentBatch.End();
                    }
                    catch (ArgumentException ex) when (ex.ParamName == "value" && ex.StackTrace?.Contains("Microsoft.Xna.Framework.Graphics.GraphicsDevice.set_ScissorRectangle") == true)
                    {
                        this.Monitor.Log("The viewport size seems to be inaccurate. Enabling compatibility mode; lookup menu may be misaligned.", LogLevel.Warn);
                        this.Monitor.Log(ex.ToString());
                        this.UpdateLayout();
                    }
                    finally
                    {
                        device.ScissorRectangle = prevScissorRectangle;
                    }
                }


                // draw cursor
                this.drawMouse(Game1.spriteBatch);
            }, this.OnDrawError);
        }


        /// <summary>The method invoked when an unhandled exception is intercepted.</summary>
        /// <param name="ex">The intercepted exception.</param>
        private void OnDrawError(Exception ex)
        {
            this.Monitor.InterceptErrors("handling an error in the lookup code", () => this.exitThisMenu());
        }

    }
}

