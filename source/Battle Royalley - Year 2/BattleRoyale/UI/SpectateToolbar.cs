/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Menus
{
    public class SpectateToolbar : IClickableMenu
    {
        private readonly List<ClickableComponent> buttons = new List<ClickableComponent>();

        private new int yPositionOnScreen;

        public Farmer Farmer;

        private Item hoverItem;

        private float transparency = 1f;

        public string[] slotText = new string[12]
        {
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
            "-", "="
        };

        public Rectangle toolbarTextSource = new Rectangle(0, 256, 60, 60);

        public SpectateToolbar(Farmer farmer)
            : base(Game1.uiViewport.Width / 2 - 384 - 64, Game1.uiViewport.Height, 896, 208)
        {
            Farmer = farmer;
            for (int i = 0; i < 12; i++)
            {
                this.buttons.Add(new ClickableComponent(new Rectangle(Game1.uiViewport.Width / 2 - 384 + i * 64, this.yPositionOnScreen - 96 + 8, 64, 64), string.Concat(i)));
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            for (int i = 0; i < 12; i++)
            {
                this.buttons[i].bounds = new Rectangle(Game1.uiViewport.Width / 2 - 384 + i * 64, this.yPositionOnScreen - 96 + 8, 64, 64);
            }
        }

        public override bool isWithinBounds(int x, int y)
        {
            return new Rectangle(this.buttons.First().bounds.X, this.buttons.First().bounds.Y, this.buttons.Last().bounds.X - this.buttons.First().bounds.X + 64, 64).Contains(x, y);
        }

        public override void draw(SpriteBatch b)
        {
            if (Game1.activeClickableMenu != null)
            {
                return;
            }
            bool alignTop = false;
            Point playerGlobalPos = Game1.player.GetBoundingBox().Center;
            Vector2 playerLocalVec = Game1.GlobalToLocal(globalPosition: new Vector2(playerGlobalPos.X, playerGlobalPos.Y), viewport: Game1.viewport);
            if (Game1.options.pinToolbarToggle)
            {
                alignTop = false;
                this.transparency = Math.Min(1f, this.transparency + 0.075f);
                if (playerLocalVec.Y > (float)(Game1.viewport.Height - 192))
                {
                    this.transparency = Math.Max(0.33f, this.transparency - 0.15f);
                }
            }
            else
            {
                alignTop = playerLocalVec.Y > (Game1.viewport.Height / 2 + 64);
                this.transparency = 1f;
            }
            int margin = Utility.makeSafeMarginY(8);
            int num = yPositionOnScreen;
            if (!alignTop)
            {
                this.yPositionOnScreen = Game1.uiViewport.Height;
                this.yPositionOnScreen += 8;
                this.yPositionOnScreen -= margin;
            }
            else
            {
                this.yPositionOnScreen = 112;
                this.yPositionOnScreen -= 8;
                this.yPositionOnScreen += margin;
            }
            if (num != this.yPositionOnScreen)
            {
                for (int k = 0; k < 12; k++)
                {
                    this.buttons[k].bounds.Y = this.yPositionOnScreen - 96 + 8;
                }
            }
            drawTextureBox(b, Game1.menuTexture, this.toolbarTextSource, Game1.uiViewport.Width / 2 - 384 - 16, this.yPositionOnScreen - 96 - 8, 800, 96, Color.White * this.transparency, 1f, drawShadow: false);
            for (int j = 0; j < 12; j++)
            {
                Vector2 toDraw = new Vector2(Game1.uiViewport.Width / 2 - 384 + j * 64, this.yPositionOnScreen - 96 + 8);
                b.Draw(Game1.menuTexture, toDraw, Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, (Farmer.CurrentToolIndex == j) ? 56 : 10), Color.White * this.transparency);
                b.DrawString(Game1.tinyFont, this.slotText[j], toDraw + new Vector2(4f, -8f), Color.DimGray * this.transparency);
            }
            for (int i = 0; i < 12; i++)
            {
                this.buttons[i].scale = Math.Max(1f, this.buttons[i].scale - 0.025f);
                Vector2 toDraw2 = new Vector2(Game1.uiViewport.Width / 2 - 384 + i * 64, this.yPositionOnScreen - 96 + 8);
                if (Farmer.items.Count > i && Farmer.items.ElementAt(i) != null)
                {
                    Farmer.items[i].drawInMenu(b, toDraw2, (Farmer.CurrentToolIndex == i) ? 0.9f : (this.buttons.ElementAt(i).scale * 0.8f), this.transparency, 0.88f);
                }
            }
            if (this.hoverItem != null)
            {
                drawToolTip(b, this.hoverItem.getDescription(), this.hoverItem.DisplayName, this.hoverItem);
                this.hoverItem = null;
            }
        }
    }
}
