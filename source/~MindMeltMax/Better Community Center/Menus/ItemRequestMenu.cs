/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using BCC.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Threading;

namespace BCC.Menus
{
    class ItemRequestMenu : IClickableMenu
    {
        private ClickableTextureComponent ItemTexture;
        private ClickableTextureComponent backButton;
        private ClickableTextureComponent leftButton;
        private ClickableTextureComponent rightButton;
        private ClickableTextureComponent okButton;

        private string MenuString1 = "";
        private string HoverText = "";

        private int Key;

        private TextBox numberSelectionBox;

        private int currentInputValue;
        private int maxRequestCount = 5;
        private int minRequestCount = 1;

        private IMonitor Monitor;

        public ItemRequestMenu(int itemKey, string itemName, IMonitor monitor) : base(0, 0, 0, 0, true) // Will be stackable, UB3R has spoken // Finish Request Filled behavior 
        {
            MenuString1 = itemName;
            Key = itemKey;
            Monitor = monitor;

            width = 700 + borderWidth * 2;
            height = 150 + borderWidth * 2;

            xPositionOnScreen = Game1.viewport.Width / 2 - (800 + borderWidth * 2) / 2;
            yPositionOnScreen = Game1.viewport.Height / 2 - (600 + borderWidth * 2) / 2;

            CollectionsPage.widthToMoveActiveTab = 12;
            upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 20, yPositionOnScreen, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
            backButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 48, yPositionOnScreen + height - 80, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
            {
                myID = 7603,
                rightNeighborID = -7777
            };
            ItemTexture = new ClickableTextureComponent(Util.getItemName(Key), new Rectangle(xPositionOnScreen + 48, yPositionOnScreen + 116, 64, 64), null, "", Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, itemKey, 16, 16), 4f, true);
            numberSelectionBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), (Texture2D)null, Game1.smallFont, Game1.textColor)
            {
                X = xPositionOnScreen + borderWidth*2 + 56,
                Y = yPositionOnScreen + 124,
                Text = string.Concat((object)currentInputValue),
                numbersOnly = true,
                textLimit = string.Concat((object)maxRequestCount).Length
            };
            numberSelectionBox.SelectMe();
            leftButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + borderWidth + numberSelectionBox.Width + ItemTexture.bounds.Width*2, this.yPositionOnScreen + 124, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
            {
                myID = 101,
                rightNeighborID = 102,
                upNeighborID = -99998
            };
            
            rightButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + borderWidth + numberSelectionBox.Width + ItemTexture.bounds.Width*2 + ItemTexture.bounds.Width, this.yPositionOnScreen + 124, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f)
            {
                myID = 102,
                leftNeighborID = 101,
                rightNeighborID = 103,
                upNeighborID = -99998
            };
            
            okButton = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 128, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 21, 64, 64), (string)null, (string)null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f)
            {
                myID = 103,
                leftNeighborID = 102,
                rightNeighborID = 104,
                upNeighborID = -99998
            };
            
            backButton = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 21, 64, 64), (string)null, (string)null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f)
            {
                myID = 104,
                leftNeighborID = 103,
                upNeighborID = -99998,
            };
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            if (leftButton.containsPoint(x, y))
            {
                int num = currentInputValue - 1;
                if(num >= minRequestCount)
                {
                    leftButton.scale = leftButton.baseScale;
                    currentInputValue = num;
                    numberSelectionBox.Text = string.Concat((object)currentInputValue);
                    Game1.playSound("smallSelect");
                }
            }
            if(rightButton.containsPoint(x, y))
            {
                int num = currentInputValue + 1;
                if(num <= maxRequestCount)
                {
                    rightButton.scale = rightButton.baseScale;
                    currentInputValue = num;
                    numberSelectionBox.Text = string.Concat((object)currentInputValue);
                    Game1.playSound("smallSelect");
                }
            }
            if(okButton.containsPoint(x, y))
            {
                if (currentInputValue > maxRequestCount || currentInputValue < minRequestCount)
                {
                    currentInputValue = Math.Max(minRequestCount, Math.Min(maxRequestCount, currentInputValue));
                    numberSelectionBox.Text = string.Concat((object)currentInputValue);
                }
                else
                    Util.createItemRequest(Key, currentInputValue);
                Game1.playSound("smallSelect");
            }
            if(backButton.containsPoint(x, y))
            {
                Game1.activeClickableMenu = (IClickableMenu)new ItemSelectionMenu("Select Item", Monitor);
            }
            numberSelectionBox.Update();
        }

        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);
            if (key != Keys.Enter)
                return;
            receiveLeftClick(okButton.bounds.Center.X, okButton.bounds.Center.Y, true);
        }

        public override void performHoverAction(int x, int y)
        {
            HoverText = "";
            if (okButton.containsPoint(x, y) && currentInputValue >= minRequestCount)
                okButton.scale = Math.Min(okButton.scale + 0.02f, okButton.baseScale + 0.2f);
            else
                okButton.scale = Math.Max(okButton.scale - 0.02f, okButton.baseScale);
            if (backButton.containsPoint(x, y))
                backButton.scale = Math.Min(backButton.scale + 0.02f, backButton.baseScale + 0.2f);
            else
                backButton.scale = Math.Max(backButton.scale - 0.02f, backButton.baseScale);
            if (leftButton.containsPoint(x, y))
                leftButton.scale = Math.Min(leftButton.scale + 0.02f, leftButton.baseScale + 0.2f);
            else
                leftButton.scale = Math.Max(leftButton.scale - 0.02f, leftButton.baseScale);
            if (rightButton.containsPoint(x, y))
                rightButton.scale = Math.Min(rightButton.scale + 0.02f, rightButton.baseScale + 0.2f);
            else
                rightButton.scale = Math.Max(rightButton.scale - 0.02f, rightButton.baseScale);
            if(ItemTexture.containsPoint(x, y))
            {
                ItemTexture.scale = Math.Min(ItemTexture.scale + 0.02f, ItemTexture.baseScale + 0.2f);
                HoverText = Util.getItemDescription(Key);
            }
            else
                ItemTexture.scale = Math.Max(ItemTexture.scale - 0.02f, ItemTexture.baseScale);

        }

        public override void draw(SpriteBatch b)
        {
            if (!Game1.options.showMenuBackground)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            else
                drawBackground(b);
            base.draw(b);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            SpriteText.drawStringWithScrollCenteredAt(b, MenuString1, Game1.viewport.Width / 2 - 50, Game1.viewport.Height / 2 - 310, MenuString1, 1f, -1, 0, 0.88f, false);
            okButton.draw(b);
            leftButton.draw(b);
            rightButton.draw(b);
            backButton.draw(b);
            numberSelectionBox.Draw(b);
            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
            ItemTexture.draw(b, Color.White, 0.86f, 0);
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            if (!HoverText.Equals(""))
                drawHoverText(b, HoverText, Game1.smallFont, 0, 0);
            drawMouse(b);
        }
    }
}
