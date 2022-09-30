/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewRoguelike.Patches;

namespace StardewRoguelike.UI
{
    internal class RefreshableShopMenu : ShopMenu
    {
        private bool HasRefreshed = false;

        private Rectangle refreshSourceRect = new(0, 608, 16, 16);

        private ClickableTextureComponent refreshButton;

        public RefreshableShopMenu(Dictionary<ISalable, int[]> itemPriceAndStock, bool hasRefreshed, int currency = 0, string who = null, Func<ISalable, Farmer, int, bool> on_purchase = null, Func<ISalable, bool> on_sell = null, string context = null) : base(itemPriceAndStock, currency, who, on_purchase, on_sell, context)
        {
            refreshButton = new(new(xPositionOnScreen + 174, yPositionOnScreen + height - 150, 64, 64), Game1.mouseCursors, refreshSourceRect, 4f)
            {
                myID = 1000,
                rightNeighborID = inventory.GetBorder(InventoryMenu.BorderSide.Top)[0].myID
            };

            HasRefreshed = hasRefreshed;

            inventory.GetBorder(InventoryMenu.BorderSide.Top)[0].leftNeighborID = 1000;

            populateClickableComponentList();
            allClickableComponents.Add(refreshButton);
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            SetUpPositions();
        }

        public void SetUpPositions()
        {
            refreshButton.bounds = new(xPositionOnScreen + 174, yPositionOnScreen + height - 150, 64, 64);
        }

        public void DoRefresh()
        {
            Merchant.CurrentShop = new RefreshableShopMenu(Merchant.GetMerchantStock(), true, context: "Blacksmith", on_purchase: OpenShopPatch.OnPurchase);
            Game1.activeClickableMenu = Merchant.CurrentShop;
            Game1.playSound("sell");
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            if (refreshButton.containsPoint(x, y) && !HasRefreshed)
                DoRefresh();
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);

            if (HasRefreshed)
                return;

            refreshButton.tryHover(x, y);
        }

        public override void draw(SpriteBatch b)
        {
            if (HasRefreshed)
            {
                refreshButton.draw(b);
                base.draw(b);
            }
            else
            {
                base.draw(b);
                refreshButton.draw(b);
            }

            drawMouse(b);
        }
    }
}
