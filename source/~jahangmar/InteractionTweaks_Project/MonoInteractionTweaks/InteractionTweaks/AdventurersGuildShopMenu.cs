//Copyright (c) 2019 Jahangmar

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Lesser General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//GNU Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public License
//along with this program. If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

using StardewModdingAPI;
using Microsoft.Xna.Framework;
using Netcode;
using Microsoft.Xna.Framework.Graphics;

namespace InteractionTweaks
{
    public class AdventurersGuildShopMenu : ShopMenu
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;
        private readonly bool SlingshotChanges;

        private const int normalSlingshotPrice = 500;
        private const int masterSlingshotPrice = 1000;
        private const int galaxySlingshotPrice = 0;
        private const int stock = 10;

        //private readonly InventoryMenu oldInventory;

        private class ModSlingshot : Slingshot
        {
            public static ModSlingshot GetCopy(Slingshot slingshot, AdventurersGuildShopMenu inst)
            {
                return new ModSlingshot(slingshot.InitialParentTileIndex, slingshot.attachments, inst.Helper);
            }

            public ModSlingshot(int which) : base(which)
            {

            }

            public ModSlingshot(int which, NetObjectArray<Object> attachmentsParam, IModHelper helper) : base(which)
            {
                helper.Reflection.GetField<NetObjectArray<Object>>(this, "attachments").SetValue(attachmentsParam);
            }

            public override Item getOne()
            {
                return new ModSlingshot(InitialParentTileIndex);
            }

            public override int salePrice()
            {
                switch (InitialParentTileIndex)
                {
                    case Slingshot.basicSlingshot:
                        return normalSlingshotPrice;
                    case Slingshot.masterSlingshot:
                        return masterSlingshotPrice;
                    case Slingshot.galaxySlingshot:
                        return galaxySlingshotPrice;
                    default:
                        return -1;
                }
            }
        }

        private class ModInventoryMenu : InventoryMenu
        {
            private AdventurersGuildShopMenu inst;

            public static ModInventoryMenu GetCopy(InventoryMenu invMenu, AdventurersGuildShopMenu inst)
            {
                IList<Item> actualInventory = new List<Item>(invMenu.actualInventory);


                var result = new ModInventoryMenu(invMenu.xPositionOnScreen, invMenu.yPositionOnScreen, invMenu.playerInventory, actualInventory, inst,
                                            invMenu.capacity, invMenu.rows, invMenu.horizontalGap, invMenu.verticalGap, invMenu.drawSlots);

                return result.ModifySlinghots();
            }

            public ModInventoryMenu ModifySlinghots()
            {
                for (int i = 0; i < actualInventory.Count; i++)
                {
                    if (actualInventory[i] is Slingshot slingshot)
                        actualInventory[i] = ModSlingshot.GetCopy(slingshot, inst);
                }
                return this;
            }

            public ModInventoryMenu ResetSlingshots()
            {
                for (int i = 0; i < actualInventory.Count; i++)
                {
                    if (actualInventory[i] is ModSlingshot modSlingshot)
                    {
                        Slingshot slingshot = new Slingshot(modSlingshot.InitialParentTileIndex);
                        inst.Helper.Reflection.GetField<NetObjectArray<Object>>(slingshot, "attachments").SetValue(modSlingshot.attachments);
                        actualInventory[i] = slingshot;
                    }
                     
                }
                return this;
            }

            private ModInventoryMenu(int xPosition, int yPosition, bool playerInventory, IList<Item> actualInventory, AdventurersGuildShopMenu inst, int capacity, int rows, int horizontalGap, int verticalGap, bool drawSlots) : base(xPosition, yPosition, playerInventory, actualInventory, inst.HighlightItemToSell, capacity, rows, horizontalGap, verticalGap, drawSlots)
            {
                this.inst = inst;
            }

            public InventoryMenu GetBaseInstance()
            {
                return new InventoryMenu(xPositionOnScreen, yPositionOnScreen, playerInventory, actualInventory, inst.HighlightItemToSell, capacity, rows, horizontalGap, verticalGap, drawSlots);
            }
        }

        public AdventurersGuildShopMenu(IModHelper helper, IMonitor monitor, Dictionary<Item, int[]> itemPriceAndStock, bool slingshotChanges, int currency = 0, string who = null) : base(itemPriceAndStock, currency, who)
        {
            Helper = helper;
            Monitor = monitor;
            SlingshotChanges = slingshotChanges;

            ModSlingshot normalSlingshot = new ModSlingshot(Slingshot.basicSlingshot);

            ModSlingshot masterSlingshot = new ModSlingshot(Slingshot.masterSlingshot);

            if (slingshotChanges)
            {
                if (Game1.player.deepestMineLevel >= 40)
                {
                    Monitor.Log("Adding normal slingshot to shop menu", LogLevel.Trace);
                    itemPriceAndStock.Add(normalSlingshot, new int[] { normalSlingshotPrice, stock });
                    Helper.Reflection.GetField<List<Item>>(this, "forSale").GetValue().Add(normalSlingshot);
                    if (Game1.player.deepestMineLevel >= 70)
                    {
                        Monitor.Log("Adding master slingshot to shop menu", LogLevel.Trace);
                        itemPriceAndStock.Add(masterSlingshot, new int[] { masterSlingshotPrice, stock });
                        Helper.Reflection.GetField<List<Item>>(this, "forSale").GetValue().Add(masterSlingshot);
                    }
                }
                ModifyInventoryMenu();
                exitFunction = ResetInventoryMenu;
            }
        }

        public override void emergencyShutDown()
        {
            ResetInventoryMenu();
            base.emergencyShutDown();
        }


        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            ModifyInventoryMenu();
        }

        private void ResetInventoryMenu()
        {
            if (inventory is ModInventoryMenu modInventoryMenu)
                inventory = modInventoryMenu.ResetSlingshots().GetBaseInstance();
            Game1.player.setInventory(new List<Item>(inventory.actualInventory));
        }

        private void ModifyInventoryMenu()
        {
            inventory = ModInventoryMenu.GetCopy(inventory, this);
        }

        public bool HighlightItemToSell(Item item)
        {
            return highlightItemToSell(item) || SlingshotChanges && item is Slingshot;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            Item item = inventory.getItemAt(x, y);
            //prevents the slingshot to be sold with attachments
            if (SlingshotChanges && item is Slingshot slingshot && HasAmmunition(slingshot))
            {
                Game1.playSound("cancel");
                Game1.showRedMessage(ModFeature.GetTrans("menu.slingshotempty"));
                return;
            }
            base.receiveLeftClick(x, y, playSound);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            //this prevents the standard inventory right-click action (removal of attachments) and prevents the slingshot to be sold with attachments
            if (SlingshotChanges && inventory.getItemAt(x, y) is Slingshot slingshot && HasAmmunition(slingshot))
            {
                Game1.playSound("cancel");
                Game1.showRedMessage(ModFeature.GetTrans("menu.slingshotempty"));
                return;
            }
            base.receiveRightClick(x, y, playSound);
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            Item heldItem = Helper.Reflection.GetField<Item>(this, "heldItem").GetValue();
            bool scrolling = Helper.Reflection.GetField<bool>(this, "scrolling").GetValue();
            if (!scrolling && heldItem == null && inventory.getItemAt(x, y) is Item item && HighlightItemToSell(item))
            {

                Helper.Reflection.GetField<string>(this, "boldTitleText").SetValue(item.DisplayName);
                Helper.Reflection.GetField<string>(this, "hoverText").SetValue(item.getDescription());
                Helper.Reflection.GetField<Item>(this, "hoveredItem").SetValue(item);

                if (item is Slingshot slingshot)
                {
                    if (HasAmmunition(slingshot))
                        Helper.Reflection.GetField<string>(this, "hoverText").SetValue(ModFeature.GetTrans("menu.slingshotempty"));
                    float sellPercentage = Helper.Reflection.GetField<float>(this, "sellPercentage").GetValue();
                    int sellToStorePrice = (int)((float)(slingshot.salePrice() / 2) * sellPercentage * slingshot.Stack);
                    //Helper.Reflection.GetField<Item>(this, "hoveredItem").SetValue(item);
                    Helper.Reflection.GetField<int>(this, "hoverPrice").SetValue(sellToStorePrice);
                }
            }
        }

        private bool HasAmmunition(Slingshot slingshot)
        {
            foreach (Object attachment in slingshot.attachments.Fields)
            {
                if (attachment != null && attachment.Stack > 0)
                    return true;
            }
            return false;
        }
    }
}
