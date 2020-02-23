using System;
using System.Linq;
using JoysOfEfficiency.Core;
using JoysOfEfficiency.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace JoysOfEfficiency.Huds
{
    internal class ShippingEstimationInfoBox
    {
        private static Config Config => InstanceHolder.Config;
        private static ITranslationHelper Translation => InstanceHolder.Translation;
        public static void DrawShippingPrice(IClickableMenu menu, SpriteFont font)
        {
            if (!(menu is ItemGrabMenu grabMenu) || !(grabMenu.shippingBin || IsCaShippingBinMenu(grabMenu)))
            {
                return;
            }
            int shippingPrice = Game1.getFarm().getShippingBin(Game1.player).Sum(item => Util.GetTruePrice(item) / 2 * item.Stack);
            string title = Translation.Get("estimatedprice.title");
            string text = $" {shippingPrice}G";
            Vector2 sizeTitle = font.MeasureString(title) * 1.2f;
            Vector2 sizeText = font.MeasureString(text) * 1.2f;
            int width = Math.Max((int)sizeTitle.X, (int)sizeText.X) + 32;
            int height = 16 + (int)sizeTitle.Y + 8 + (int)sizeText.Y + 16;
            Vector2 basePos = new Vector2(Config.PriceBoxCoordinates.X, Config.PriceBoxCoordinates.Y);

            Util.DrawWindow((int)basePos.X, (int)basePos.Y, width, height);
            Utility.drawTextWithShadow(Game1.spriteBatch, title, font, basePos + new Vector2(16, 16), Color.Black, 1.2f);
            Utility.drawTextWithShadow(Game1.spriteBatch, text, font, basePos + new Vector2(16, 16 + (int)sizeTitle.Y + 8), Color.Black, 1.2f);
        }

        public static bool IsCaShippingBinMenu(ItemGrabMenu menu)
        {
            return !menu.reverseGrab && menu.showReceivingMenu && menu.context is Farm;
        }
    }
}
