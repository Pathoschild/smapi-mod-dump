/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/6135/StardewValley.ProfitCalculator
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProfitCalculator.main;
using ProfitCalculator.ui;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace ProfitCalculator.menus
{
    /// <summary>
    /// A menu that displays the results of the profit calculator.
    /// </summary>
    public class ProfitCalculatorResultsList : IClickableMenu
    {
        private static readonly int widthOnScreen = 632 + borderWidth * 2;
        private static readonly int heightOnScreen = 600 + borderWidth * 2 + Game1.tileSize;

        private readonly List<BaseOption> Options = new();
        private readonly List<Vector4> OptionSlots = new();

        private ClickableTextureComponent upArrow;
        private ClickableTextureComponent downArrow;
        private Rectangle scrollBarBounds;
        private ClickableTextureComponent scrollBar;

        private int currentItemIndex = 0;
        private readonly int maxOptions = 6;
        private bool scrolling = false;

        /// <summary> Tracks whether the menu is open or not. </summary>
        public bool IsResultsListOpen { get; set; } = false;

        /// <summary>
        /// Creates a new instance of the ProfitCalculatorResultsList class.
        /// </summary>
        /// <param name="_cropInfos"> The list of crop infos to display in the menu. </param>
        public ProfitCalculatorResultsList(List<CropInfo> _cropInfos) :
            base(
                (int)GetAppropriateMenuPosition().X,
                (int)GetAppropriateMenuPosition().Y,
                widthOnScreen,
                heightOnScreen
            )
        {
            for (int i = 0; i < maxOptions; i++)
            {
                OptionSlots.Add(
                    new(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + 10,
                        this.yPositionOnScreen + (spaceToClearTopBorder + 5) + (Game1.tileSize / 2) - Game1.tileSize / 4 + ((Game1.tileSize + Game1.tileSize / 2) * i),
                        widthOnScreen - ((spaceToClearSideBorder + borderWidth + 10) * 2),
                        Game1.tileSize + Game1.tileSize / 2
                   )
                );
            }
            foreach (CropInfo cropInfo in _cropInfos)
            {
                Options.Add(
                    new CropBox(
                        0,
                        0,
                        0,
                        0,
                        cropInfo
                    )
                );
            }

            behaviorBeforeCleanup = delegate
            {
                IsResultsListOpen = false;
            };

            this.xPositionOnScreen = (int)GetAppropriateMenuPosition().X;
            this.yPositionOnScreen = (int)GetAppropriateMenuPosition().Y;

            int scrollbar_x = xPositionOnScreen + width;
            this.upArrow = new ClickableTextureComponent(
                new Rectangle(scrollbar_x, yPositionOnScreen + Game1.tileSize + Game1.tileSize / 3, 44, 48),
                Game1.mouseCursors,
                new Rectangle(421, 459, 11, 12),
                4f);
            this.downArrow = new ClickableTextureComponent(
                new Rectangle(scrollbar_x, yPositionOnScreen + height - 64, 44, 48),
                Game1.mouseCursors,
                new Rectangle(421, 472, 11, 12),
                4f);
            this.scrollBarBounds = default;
            this.scrollBarBounds.X = this.upArrow.bounds.X + 12;
            this.scrollBarBounds.Width = 24;
            this.scrollBarBounds.Y = this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4;
            this.scrollBarBounds.Height = this.downArrow.bounds.Y - 4 - this.scrollBarBounds.Y;
            this.scrollBar = new ClickableTextureComponent(new Rectangle(this.scrollBarBounds.X, this.scrollBarBounds.Y, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
        }

        #region Draw Methods

        /// <inheritdoc/>
        public override void draw(SpriteBatch b)

        {
            //draw bottom up

            if (!Game1.options.showMenuBackground)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);

            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, widthOnScreen, heightOnScreen, speaker: false, drawOnlyBox: true);

            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);

            int maxIndex = Math.Min(maxOptions, Options.Count);
            for (int i = 0; i < maxIndex; i++)
            {
                if (currentItemIndex + i >= Options.Count)
                    break;
                Options[currentItemIndex + i].ClickableComponent = new(
                    new(
                    (int)OptionSlots[i].X,
                    (int)OptionSlots[i].Y,
                    (int)OptionSlots[i].Z,
                    (int)OptionSlots[i].W
                ), Options[currentItemIndex + i].Name());
                Options[currentItemIndex + i].Draw(b);
            }

            this.upArrow.draw(b, Color.White, 0.6f);
            this.downArrow.draw(b, Color.White, 0.6f);

            IClickableMenu.drawTextureBox(
                b,
                Game1.mouseCursors,
                new Rectangle(403, 383, 6, 6),
                this.scrollBarBounds.X,
                this.scrollBarBounds.Y,
                this.scrollBarBounds.Width,
                this.scrollBarBounds.Height,
                Color.White,
                4f,
                drawShadow: false,
                draw_layer: 0.6f
                );
            this.scrollBar.draw(
                b,
                Color.White,
                0.65f
            );

            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            if (shouldDrawCloseButton())
                base.draw(b);
            if (!Game1.options.hardwareCursor)
                b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
        }

        #endregion Draw Methods

        #region Event Handling

        private void SetScrollBarToCurrentIndex()
        {
            if (this.Options.Count > 0)
            {
                //devide the height of the scroll bar by the number of options minus the displayed options, then multiply by the current index to get the position of the scroll bar without going out of bounds. //804 is max y for bar
                int numberOfSteps = Math.Max(1, this.Options.Count - maxOptions);
                double sizeOfStep = Math.Floor((this.scrollBarBounds.Height - (scrollBar.bounds.Height / 2.0)) / numberOfSteps);
                double barPosition = this.scrollBarBounds.Y + (sizeOfStep * this.currentItemIndex);
                this.scrollBar.bounds.Y =
                    (int)Math.Floor(barPosition);
                if (this.currentItemIndex == this.Options.Count - maxOptions)
                {
                    this.scrollBar.bounds.Y = this.downArrow.bounds.Y - this.scrollBar.bounds.Height - 7;
                }
            }
            else
            {
                this.scrollBar.bounds.Y = this.scrollBarBounds.Y;
            }
        }

        /// <summary>
        /// Handles mouse scroll wheel actions received by the menu. Goes up or down a page depending on the direction of the scroll.
        /// </summary>
        /// <param name="direction"> The direction of the scroll. 1 for down and -1 for up </param>
        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            if (direction > 0 && this.currentItemIndex > 0)
            {
                this.ArrowPressed(-1);
                Game1.playSound("shiny4");
            }
            else if (direction < 0 && this.currentItemIndex < Math.Max(0, this.Options.Count - maxOptions))
            {
                ArrowPressed();
                Game1.playSound("shiny4");
            }
            if (Game1.options.SnappyMenus)
            {
                this.snapCursorToCurrentSnappedComponent();
            }
        }

        /// <summary>
        /// Handles key presses received by the menu.
        /// </summary>
        /// <param name="key"> The key that was pressed. </param>
        public override void receiveKeyPress(Keys key)
        {
            switch (key)
            {
                case Keys.Escape:
                    exitThisMenu();
                    break;

                case Keys.Down:
                    if (currentItemIndex + maxOptions < Options.Count)
                    {
                        ArrowPressed();
                        Game1.playSound("shwip");
                    }
                    break;

                case Keys.Up:
                    if (currentItemIndex - maxOptions >= 0)
                    {
                        ArrowPressed(-1);
                        Game1.playSound("shwip");
                    }
                    break;
            }
        }

        /// <summary>
        /// Handles mouse hovers received by the menu.
        /// </summary>
        /// <param name="x"> The x position of the mouse. </param>
        /// <param name="y"> The y position of the mouse. </param>
        public override void performHoverAction(int x, int y)
        {
            for (int i = 0; i < this.OptionSlots.Count; i++)
            {
                if (this.currentItemIndex >= 0 && this.currentItemIndex + i < this.Options.Count && this.Options[this.currentItemIndex + i].ClickableComponent.bounds.Contains(x - this.OptionSlots[i].X, y - this.OptionSlots[i].Y))
                {
                    Game1.SetFreeCursorDrag();
                    break;
                }
            }
            if (this.scrollBarBounds.Contains(x, y))
            {
                Game1.SetFreeCursorDrag();
            }
            if (GameMenu.forcePreventClose)
            {
                return;
            }

            //if hover over any of the optionSlots, print to log "HoveringOver it"
            int maxIndex = Math.Min(this.OptionSlots.Count, Options.Count);
            for (int i = 0; i < maxIndex; i++)
            {
                BaseOption option = (CropBox)Options[currentItemIndex + i];
                option.PerformHoverAction(x, y);
            }

            this.upArrow.tryHover(x, y);
            this.downArrow.tryHover(x, y);
            this.scrollBar.tryHover(x, y);
            _ = this.scrolling;
        }

        /// <summary>
        /// Handles scroll bar movement received by the menu according to the mouse y position.
        /// </summary>
        /// <param name="y"> The y position of the mouse. </param>
        public virtual void SetScrollFromY(int y)
        {
            int y2 = this.scrollBar.bounds.Y;
            float percentage = (float)(y - this.scrollBarBounds.Y) / (float)this.scrollBarBounds.Height;
            float currentItemIndexFloat =
                Utility.Lerp(
                    t: Utility.Clamp(percentage, 0f, 1f),
                    a: 0f,
                    b: this.Options.Count - maxOptions);
            this.currentItemIndex = (int)Math.Round(currentItemIndexFloat);
            this.SetScrollBarToCurrentIndex();
            if (y2 != this.scrollBar.bounds.Y)
            {
                Game1.playSound("shiny4");
            }
        }

        /// <summary>
        /// Handles mouse clicks held received by the menu.
        /// </summary>
        /// <param name="x"> The x position of the mouse. </param>
        /// <param name="y"> The y position of the mouse. </param>
        public override void leftClickHeld(int x, int y)
        {
            if (!GameMenu.forcePreventClose)
            {
                base.leftClickHeld(x, y);
                if (this.scrolling)
                {
                    this.SetScrollFromY(y);
                }
            }
        }

        /// <summary>
        /// Handles mouse clicks released received by the menu.
        /// </summary>
        /// <param name="x"> The x position of the mouse. </param>
        /// <param name="y"> The y position of the mouse. </param>
        public override void releaseLeftClick(int x, int y)
        {
            if (!GameMenu.forcePreventClose)
            {
                base.releaseLeftClick(x, y);
                this.scrolling = false;
            }
        }

        /// <summary>
        /// Handles mouse clicks received by the menu.
        /// </summary>
        /// <param name="x"> The x position of the mouse. </param>
        /// <param name="y"> The y position of the mouse. </param>
        /// <param name="playSound"> Whether to play a sound when the click is received. </param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (GameMenu.forcePreventClose)
            {
                return;
            }
            if (this.downArrow.containsPoint(x, y) && this.currentItemIndex < Math.Max(0, this.Options.Count - maxOptions))
            {
                this.ArrowPressed(1);
                Game1.playSound("shwip");
            }
            else if (this.upArrow.containsPoint(x, y) && this.currentItemIndex > 0)
            {
                this.ArrowPressed(-1);
                Game1.playSound("shwip");
            }
            else if (this.scrollBar.containsPoint(x, y))
            {
                this.scrolling = true;
            }
            else if (!this.downArrow.containsPoint(x, y) && x > base.xPositionOnScreen + base.width && x < base.xPositionOnScreen + base.width + 128 && y > base.yPositionOnScreen && y < base.yPositionOnScreen + base.height)
            {
                this.scrolling = true;
                this.leftClickHeld(x, y);
                this.releaseLeftClick(x, y);
            }
        }

        /// <summary>
        /// Handles arrow key presses received by the menu. Causing the menu to scroll up or down.
        /// </summary>
        /// <param name="direction"> The direction of the arrow key press. 1 for down and -1 for up </param>
        private void ArrowPressed(int direction = 1)
        {
            if (direction == 1)
            {
                this.downArrow.scale = this.downArrow.baseScale;
            }
            else
            {
                this.upArrow.scale = this.upArrow.baseScale;
            }
            this.currentItemIndex += direction;
            this.SetScrollBarToCurrentIndex();
        }

        /// <summary>
        /// Updates the menu. Refreshes the positions of the buttons and options.
        /// </summary>
        public override void update(GameTime time)
        {
            base.update(time);
            Vector2 defaultPosition = new(
                (Game1.viewport.Width * Game1.options.zoomLevel * (1 / Game1.options.uiScale)) / 2 - (widthOnScreen / 2),
                (Game1.viewport.Height * Game1.options.zoomLevel * (1 / Game1.options.uiScale)) / 2 - (heightOnScreen / 2)
            );
            //update all the options and labels and buttons
            foreach (BaseOption option in Options)
            {
                option.Update();
            }
        }

        /// <summary>
        /// Gets the appropriate position for the menu to be in.
        /// </summary>
        /// <returns> The appropriate position for the menu to be in, in Vector2 format </returns>
        public static Vector2 GetAppropriateMenuPosition()
        {
            Vector2 defaultPosition = new(
                (Game1.viewport.Width * Game1.options.zoomLevel * (1 / Game1.options.uiScale)) / 2 - (widthOnScreen / 2),
                (Game1.viewport.Height * Game1.options.zoomLevel * (1 / Game1.options.uiScale)) / 2 - (heightOnScreen / 2)
            );

            //Force the viewport into a position that it should fit into on the screen???
            if (defaultPosition.X + widthOnScreen > Game1.viewport.Width)
            {
                defaultPosition.X = 0;
            }

            if (defaultPosition.Y + heightOnScreen > Game1.viewport.Height)
            {
                defaultPosition.Y = 0;
            }
            return defaultPosition;
        }

        /// <summary>The method called when the game window changes size.</summary>
        /// <param name="oldBounds">The former viewport.</param>
        /// <param name="newBounds">The new viewport.</param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);

            this.xPositionOnScreen = (int)GetAppropriateMenuPosition().X;
            this.yPositionOnScreen = (int)GetAppropriateMenuPosition().Y;
            OptionSlots.Clear();
            for (int i = 0; i < maxOptions; i++)
            {
                OptionSlots.Add(
                    new(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + 10,
                        this.yPositionOnScreen + (spaceToClearTopBorder + 5) + (Game1.tileSize / 2) - Game1.tileSize / 4 + ((Game1.tileSize + Game1.tileSize / 2) * i),
                        widthOnScreen - ((spaceToClearSideBorder + borderWidth + 10) * 2),
                        Game1.tileSize + Game1.tileSize / 2
                   )
                );
            }

            int scrollbar_x = xPositionOnScreen + width;
            this.upArrow = new ClickableTextureComponent(
                new Rectangle(scrollbar_x, yPositionOnScreen + Game1.tileSize + Game1.tileSize / 3, 44, 48),
                Game1.mouseCursors,
                new Rectangle(421, 459, 11, 12),
                4f);
            this.downArrow = new ClickableTextureComponent(
                new Rectangle(scrollbar_x, yPositionOnScreen + height - 64, 44, 48),
                Game1.mouseCursors,
                new Rectangle(421, 472, 11, 12),
                4f);
            this.scrollBarBounds = default;
            this.scrollBarBounds.X = this.upArrow.bounds.X + 12;
            this.scrollBarBounds.Width = 24;
            this.scrollBarBounds.Y = this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4;
            this.scrollBarBounds.Height = this.downArrow.bounds.Y - 4 - this.scrollBarBounds.Y;
            this.scrollBar = new ClickableTextureComponent(new Rectangle(this.scrollBarBounds.X, this.scrollBarBounds.Y, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
        }

        #endregion Event Handling
    }
}