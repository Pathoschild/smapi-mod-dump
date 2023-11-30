/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/futroo/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace ShippingBinSummary
{
    class ModEntry : Mod
    {
        private DataModel Data = null!;
        private bool showToolTip = false;
    
        string allItemsSellPriceString;
        int allItemsSellPrice;

        public override void Entry(IModHelper helper)
        {
            this.Data = helper.Data.ReadJsonFile<DataModel>("assets/data.json") ?? new DataModel(null);

            helper.Events.Input.CursorMoved += OnCursorMoved;
            helper.Events.Display.RenderedHud += OnPostRenderHudEvent;
        }

        private void OnCursorMoved(object sender, CursorMovedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            Building shippingBin = Game1.getFarm().buildings.Where(obj => obj.buildingType == "Shipping Bin").FirstOrDefault();
            List<Vector2> shippingBinTiles = new List<Vector2>();

            if (shippingBin != null)
            {
                int width = shippingBin.tilesWide;
                int height = shippingBin.tilesHigh + 1;
                for( int x = 0; x < width; x++ )
                {
                    for( int y = 0; y < height; y++ ) {
                        shippingBinTiles.Add(new Vector2(shippingBin.tileX + x, shippingBin.tileY - y));
                    }
                }
            }

            Vector2 mouseTile = e.NewPosition.Tile;
            if (mouseTile != null)
            {
                if (shippingBinTiles.Contains(mouseTile))
                {
                    allItemsSellPrice = 0;
                    foreach (var item in Game1.getFarm().getShippingBin(Game1.player))
                    {
                        int? price = GetSellPrice(item);
                        allItemsSellPrice += price != null ? price.Value : 0;
                    }
                    if (allItemsSellPrice > 0)
                    {
                        allItemsSellPriceString = allItemsSellPrice.ToString();
                        showToolTip = true;
                    } else
                    {
                        allItemsSellPriceString = "Empty";
                        showToolTip = true;
                    }
                } else
                {
                    showToolTip = false;
                }
            } else
            {
                showToolTip = false;
            }
        }
        private void OnPostRenderHudEvent(object sender, RenderedHudEventArgs e)
        {
            if (showToolTip)
            {
                const int border = 8;
                const int padding = 6;
                Rectangle CoinSource = new(5, 69, 6, 6);

                int coinSize = CoinSource.Width * Game1.pixelZoom;

                Vector2 priceTextSize = Game1.smallFont.MeasureString(allItemsSellPriceString);
                
                Vector2 innerSize = new(priceTextSize.X + (allItemsSellPriceString != "Empty" ? padding + coinSize : 0), priceTextSize.Y);
                Vector2 outerSize = innerSize + new Vector2((border + padding) * 2);

                float x = Game1.getMouseX() - innerSize.X/2;
                float y = Game1.getMouseY() + 46;

                if ((int)x + (int)outerSize.X > Game1.uiViewport.Width)
                    x = Game1.uiViewport.Width - (int)outerSize.X;
                if ((int)y + (int)outerSize.Y > Game1.uiViewport.Height)
                    y = Game1.uiViewport.Height - (int)outerSize.Y;

                IClickableMenu.drawTextureBox(Game1.spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), (int)x, (int)y, (int)outerSize.X + (allItemsSellPriceString == "Empty" ? 0 : 3), (int)outerSize.Y, Color.White);
                Utility.drawTextWithShadow(Game1.spriteBatch, allItemsSellPriceString, Game1.smallFont, new Vector2(x + border + padding, y + border + padding + 2), Game1.textColor);

                if (allItemsSellPriceString != "Empty")
                {
                    Game1.spriteBatch.Draw(Game1.debrisSpriteSheet, new Vector2(x + outerSize.X - border - padding - coinSize, y + border + padding + 5), CoinSource, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1f);
                }
                
            }
        }
    
        private int? GetSellPrice(Item item)
        {
            if (!CanBeSold(item, this.Data.ForceSellable))
                return null;

            int price = Utility.getSellToStorePriceOfItem(item, countStack: true);
            return price >= 0
                ? price
                : null;
        }
        public static bool CanBeSold(Item item, ISet<int> forceSellable)
        {
            return
                (item is SObject obj && obj.canBeShipped())
                || forceSellable.Contains(item.Category);
        }
    }
}
