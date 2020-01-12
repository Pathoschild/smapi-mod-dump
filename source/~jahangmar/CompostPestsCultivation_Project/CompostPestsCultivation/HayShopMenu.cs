// Copyright (c) 2019 Jahangmar
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using System.Collections.Generic;

namespace CompostPestsCultivation
{
    public class HayShopMenu : ShopMenu
    {
        public const int hay_index = 178;

        private const int silo_capacity = 240;

        private static int GetHayCount() => Game1.getFarm().piecesOfHay.Value;

        private static void SetHayCount(int count) => Game1.getFarm().piecesOfHay.Set(count);

        private static int GetCapacity() => Utility.numSilos() * silo_capacity;

        private static Item HayItem;

        private static Dictionary<ISalable, int[]> ItemsForSale()
        {
            HayItem = new Object(hay_index, GetHayCount());
            Dictionary<ISalable, int[]> itemsForSale = new Dictionary<ISalable, int[]>();
            itemsForSale.Add(HayItem, new int[]{0, GetHayCount()});
            return itemsForSale;
        }

        private string trans_silosfull;
        private string trans_silosempty;

        public HayShopMenu(IModHelper helper) : base(ItemsForSale())
        {
            this.inventory.highlightMethod = (Item item) => GetHayCount() < GetCapacity() && item?.ParentSheetIndex == hay_index;
            this.potraitPersonDialogue = Game1.parseText(helper.Translation.Get("siloshop.descr", new { capacity = GetCapacity().ToString() }), Game1.dialogueFont, 304);
            this.onSell = OnSell;
            this.onPurchase = OnPurchase;
            this.purchaseSound = "pickUpItem";

            trans_silosfull = helper.Translation.Get("siloshop.silosfull");
            trans_silosempty = helper.Translation.Get("siloshop.silosempty");
        }

        private bool OnSell(ISalable item)
        {
            if (GetHayCount() + item.Stack > GetCapacity())
            {
                int canAdd = GetCapacity() - GetHayCount();
                SetHayCount(GetCapacity());
                item.Stack -= canAdd;
                Game1.player.addItemToInventory(item as Item);
                Game1.showRedMessage(trans_silosfull);
            }
            else
            {
                SetHayCount(GetHayCount() + item.Stack);
            }

            return false;
        }

        private bool OnPurchase(ISalable item, Farmer farmer, int count)
        {
            SetHayCount(GetHayCount() - count);
            return false;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (GetHayCount() <= 0 && forSaleButtons.Exists((forSaleButton) => forSaleButton.containsPoint(x,y)))
            {
                Game1.showRedMessage(trans_silosempty);
            }
            else
                base.receiveLeftClick(x, y, playSound);

            setItemPriceAndStock(ItemsForSale());
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (GetHayCount() <= 0 && forSaleButtons.Exists((forSaleButton) => forSaleButton.containsPoint(x, y)))
            {
                Game1.showRedMessage(trans_silosempty);
            }
            else
                base.receiveRightClick(x, y, playSound);

            setItemPriceAndStock(ItemsForSale());
        }
    }
}
