/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using System.IO;
using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;

namespace ItemResearchSpawner.Components.UI
{
    public class ItemMoneyTooltip
    {
        private readonly Texture2D _coinTexture;

        public ItemMoneyTooltip(IContentHelper content, IMonitor monitor)
        {
            // _coinTexture = content.Load<Texture2D>("assets/images/coin-icon.png");
            _coinTexture = content.Load<Texture2D>(Path.Combine("assets", "images", "coin-icon.png"));
        }

        public void Draw(SpriteBatch spriteBatch, Item hoveredItem)
        {
            var prices = ModManager.Instance.GetItemPrices(hoveredItem);

            string costText;

            if (prices.buy == prices.sell)
            {
                costText = hoveredItem.Stack > 1 ? $"{prices.buy * hoveredItem.Stack}({prices.buy})" : $"{prices.buy}";
            }
            else
            {
                costText = hoveredItem.Stack > 1 ?
                    $"Buy {prices.buy * hoveredItem.Stack}({prices.buy}) \nSell {prices.sell * hoveredItem.Stack}({prices.sell})" :
                    $"Buy {prices.buy} \nSell {prices.sell}";
            }

            //var costText = "0";

            var mousePos = Game1.getMousePosition();
            var basePosition = new Vector2(mousePos.X, mousePos.Y) + new Vector2(-38, 0);

            var textOffsetX = _coinTexture.Width * Game1.pixelZoom + 5;
            var textWidth = Game1.smallFont.MeasureString(costText).X;

            var boxWidth = textWidth + UIConstants.BorderWidth * 2 + _coinTexture.Width;

            RenderHelpers.DrawTextMenuBox((int) (basePosition.X - boxWidth), (int) (basePosition.Y - 40),
                Game1.smallFont, costText, textOffsetX);

            Utility.drawWithShadow(spriteBatch, _coinTexture, basePosition + new Vector2(-boxWidth + 16, -24),
                _coinTexture.Bounds, Color.White, 0f, Vector2.Zero, shadowIntensity: 0f);
        }
    }
}