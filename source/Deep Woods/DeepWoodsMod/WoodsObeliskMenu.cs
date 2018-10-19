using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using static DeepWoodsMod.DeepWoodsSettings;

namespace DeepWoodsMod
{
    class WoodsObeliskMenu : IClickableMenu
    {
        public List<ClickableComponent> levelButtons = new List<ClickableComponent>();

        public WoodsObeliskMenu()
          : base(0, 0, 0, 0, true)
        {
            exitFunction = OnExit;

            int num1 = DeepWoodsState.LowestLevelReached / 10;
            this.width = num1 > 50 ? 484 + IClickableMenu.borderWidth * 2 : Math.Min(220 + IClickableMenu.borderWidth * 2, num1 * 44 + 64 + IClickableMenu.borderWidth * 2);
            this.height = Math.Max(64 + IClickableMenu.borderWidth * 3, num1 * 44 / (this.width - IClickableMenu.borderWidth) * 44 + 64 + IClickableMenu.borderWidth * 3);
            this.xPositionOnScreen = Game1.viewport.Width / 2 - this.width / 2;
            this.yPositionOnScreen = Game1.viewport.Height / 2 - this.height / 2;
            // Game1.playSound(Sounds.CRYSTAL);
            Game1.playSound(Sounds.LEAFRUSTLE);
            int num2 = this.width / 44 - 1;
            int x1 = this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder * 3 / 4;
            int y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.borderWidth / 3;
            this.levelButtons.Add(new ClickableComponent(new Rectangle(x1, y, 44, 44), string.Concat(1))
            {
                myID = 0,
                rightNeighborID = 1,
                downNeighborID = num2
            });
            int x2 = x1 + 64 - 20;
            if (x2 > this.xPositionOnScreen + this.width - IClickableMenu.borderWidth)
            {
                x2 = this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder * 3 / 4;
                y += 44;
            }
            for (int index = 1; index <= num1; ++index)
            {
                this.levelButtons.Add(new ClickableComponent(new Rectangle(x2, y, 44, 44), string.Concat(index * 10))
                {
                    myID = index,
                    rightNeighborID = index % num2 == num2 - 1 ? -1 : index + 1,
                    leftNeighborID = index % num2 == 0 ? -1 : index - 1,
                    downNeighborID = index + num2,
                    upNeighborID = index - num2
                });
                x2 = x2 + 64 - 20;
                if (x2 > this.xPositionOnScreen + this.width - IClickableMenu.borderWidth)
                {
                    x2 = this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder * 3 / 4;
                    y += 44;
                }
            }
            this.initializeUpperRightCloseButton();
            if (Game1.options.snappyMenus && Game1.options.gamepadControls)
            {
                this.populateClickableComponentList();
                this.snapToDefaultClickableComponent();
            }
        }

        private void OnExit()
        {
            Game1.fadeIn = false;
            Game1.fadeToBlack = true;
            Game1.fadeToBlackAlpha = 0.99f;
            Game1.screenGlow = false;
            Game1.player.temporarilyInvincible = false;
            Game1.player.temporaryInvincibilityTimer = 0;
            Game1.displayFarmer = true;
        }

        public override void snapToDefaultClickableComponent()
        {
            this.currentlySnappedComponent = this.getComponentWithID(0);
            this.snapCursorToCurrentSnappedComponent();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.isWithinBounds(x, y))
            {
                foreach (ClickableComponent levelButton in this.levelButtons)
                {
                    if (levelButton.containsPoint(x, y))
                    {
                        Game1.playSound("smallSelect");
                        Game1.changeMusicTrack("none");
                        this.exitThisMenu(false);
                        DeepWoodsManager.WarpFarmerIntoDeepWoods(Convert.ToInt32(levelButton.name));
                    }
                }
                base.receiveLeftClick(x, y, true);
            }
            else
            {
                this.exitThisMenu(false); 
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            foreach (ClickableComponent levelButton in this.levelButtons)
            {
                levelButton.scale = !levelButton.containsPoint(x, y) ? 1f : 2f;
            }
        }

        public override void draw(SpriteBatch b)
        {
            Game1.spriteBatch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black);

            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen - 64 + 8, this.width + 21, this.height + 64, false, true, (string)null, false);
            foreach (ClickableComponent levelButton in this.levelButtons)
            {
                b.Draw(Game1.mouseCursors, new Vector2((float)(levelButton.bounds.X - 4), (float)(levelButton.bounds.Y + 4)), new Rectangle?(new Rectangle((double)levelButton.scale > 1.0 ? 267 : 256, 256, 10, 10)), Color.Black * 0.5f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.865f);
                b.Draw(Game1.mouseCursors, new Vector2((float)levelButton.bounds.X, (float)levelButton.bounds.Y), new Rectangle?(new Rectangle((double)levelButton.scale > 1.0 ? 267 : 256, 256, 10, 10)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.868f);
                Vector2 position = new Vector2((float)(levelButton.bounds.X + 16 + NumberSprite.numberOfDigits(Convert.ToInt32(levelButton.name)) * 6), (float)(levelButton.bounds.Y + 24 - NumberSprite.getHeight() / 4));
                NumberSprite.draw(Convert.ToInt32(levelButton.name), b, position, Game1.CurrentMineLevel == Convert.ToInt32(levelButton.name) ? Color.Gray * 0.75f : Color.Gold, 0.5f, 0.86f, 1f, 0, 0);
            }
            this.drawMouse(b);
            base.draw(b);
        }
    }
}
