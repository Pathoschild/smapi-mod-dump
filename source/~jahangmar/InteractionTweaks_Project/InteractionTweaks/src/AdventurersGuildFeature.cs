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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Tools;

using Microsoft.Xna.Framework;
using Netcode;

namespace InteractionTweaks
{
    public class AdventurersGuildFeature : ModFeature
    {

        private static bool SlingshotChanges;

        private const int normalSlingshotPrice = 500;
        private const int masterSlingshotPrice = 1000;
        private const int galaxySlingshotPrice = 0;
        private const int stock = 10;

        public static new void Enable()
        {
            Helper.Events.Display.MenuChanged += Display_MenuChanged;
        }

        public static new void Disable()
        {
            Helper.Events.Display.MenuChanged -= Display_MenuChanged;
        }

        public static void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu shopMenu && Game1.currentLocation is AdventureGuild)
            {
                AddSlingshots(shopMenu);
                Helper.Events.Display.RenderingActiveMenu += Display_RenderingActiveMenu;
                //Helper.Events.Display.RenderedActiveMenu += Display_RenderedActiveMenu;
                Helper.Events.Input.ButtonPressed += Input_ButtonPressed;
                Helper.Events.Input.ButtonReleased += Input_ButtonReleased;
                ConvertSlingshots(shopMenu);
            }
            else if (e.OldMenu is ShopMenu oldShopMenu && Game1.currentLocation is AdventureGuild)
            {
                Helper.Events.Display.RenderingActiveMenu -= Display_RenderingActiveMenu;
                //Helper.Events.Display.RenderedActiveMenu -= Display_RenderedActiveMenu;
                Helper.Events.Input.ButtonPressed -= Input_ButtonPressed;
                Helper.Events.Input.ButtonReleased -= Input_ButtonReleased;
                ResetSlingshots(oldShopMenu);
            }
        }

        static void Display_RenderingActiveMenu(object sender, StardewModdingAPI.Events.RenderingActiveMenuEventArgs e)
        {
            ShopMenu shopMenu = (ShopMenu)Game1.activeClickableMenu;

            Item heldItem = Helper.Reflection.GetField<Item>(shopMenu, "heldItem").GetValue();
            bool scrolling = Helper.Reflection.GetField<bool>(shopMenu, "scrolling").GetValue();

            int X = Game1.getMouseX();
            int Y = Game1.getMouseY();
            if (!scrolling && heldItem == null && shopMenu.inventory.getItemAt(X, Y) is Item item && shopMenu.inventory.highlightMethod(item))
            {

                Helper.Reflection.GetField<string>(shopMenu, "boldTitleText").SetValue(item.DisplayName);
                Helper.Reflection.GetField<string>(shopMenu, "hoverText").SetValue(item.getDescription());
                Helper.Reflection.GetField<Item>(shopMenu, "hoveredItem").SetValue(item);

                if (item is Slingshot slingshot)
                {
                    if (HasAmmunition(slingshot))
                        Helper.Reflection.GetField<string>(shopMenu, "hoverText").SetValue(ModFeature.GetTrans("menu.slingshotempty"));
                    float sellPercentage = Helper.Reflection.GetField<float>(shopMenu, "sellPercentage").GetValue();
                    int sellToStorePrice = (int)((float)(slingshot.salePrice() / 2) * sellPercentage * slingshot.Stack);
                    //Helper.Reflection.GetField<Item>(this, "hoveredItem").SetValue(item);
                    Helper.Reflection.GetField<int>(shopMenu, "hoverPrice").SetValue(sellToStorePrice);
                }
            }
        }

        static void Display_RenderedActiveMenu(object sender, StardewModdingAPI.Events.RenderedActiveMenuEventArgs e)
        {
        }

        static void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            ShopMenu shopMenu = (ShopMenu)Game1.activeClickableMenu;

            Item heldItem = Helper.Reflection.GetField<Item>(shopMenu, "heldItem").GetValue();
            InventoryMenu inventoryMenu = Helper.Reflection.GetField<InventoryMenu>(shopMenu, "inventory").GetValue();
            if ((e.Button.Equals(SButton.MouseLeft) || e.Button.Equals(SButton.MouseRight)) && heldItem == null && inventoryMenu.getItemAt((int)e.Cursor.ScreenPixels.X, (int)e.Cursor.ScreenPixels.Y) is Slingshot slingshot)
            {
                if (HasAmmunition(slingshot))
                {
                    Game1.playSound("cancel");
                    Game1.showRedMessage(ModFeature.GetTrans("menu.slingshotempty"));
                    Helper.Input.Suppress(e.Button);
                }
            }
        }

        static void Input_ButtonReleased(object sender, StardewModdingAPI.Events.ButtonReleasedEventArgs e)
        {
            ShopMenu shopMenu = (ShopMenu)Game1.activeClickableMenu;
            ConvertSlingshots(shopMenu);
        }

        private static void ResetSlingshots(ShopMenu shopMenu)
        {
            InventoryMenu inventoryMenu = Helper.Reflection.GetField<InventoryMenu>(shopMenu, "inventory").GetValue();
            for (int i = 0; i < inventoryMenu.actualInventory.Count; i++)
                if (inventoryMenu.actualInventory[i] is ModSlingshot modSlingshot)
                    inventoryMenu.actualInventory[i] = modSlingshot.ToSlingshot();
        }

        private static void ConvertSlingshots(ShopMenu shopMenu)
        {
            InventoryMenu inventoryMenu = Helper.Reflection.GetField<InventoryMenu>(shopMenu, "inventory").GetValue();
            for (int i = 0; i < inventoryMenu.actualInventory.Count; i++)
                if (inventoryMenu.actualInventory[i] is Slingshot slingshot)
                    inventoryMenu.actualInventory[i] = new ModSlingshot(slingshot, shopMenu);
        }

        private static void AddSlingshots(ShopMenu shopMenu)
        {
            SlingshotChanges = Config.SellableItemsFeature;

            ModSlingshot normalSlingshot = new ModSlingshot(Slingshot.basicSlingshot, shopMenu);

            ModSlingshot masterSlingshot = new ModSlingshot(Slingshot.masterSlingshot, shopMenu);

            if (SlingshotChanges)
            {
                if (Game1.player.deepestMineLevel >= 40)
                {
                    Monitor.Log("Adding normal slingshot to shop menu", LogLevel.Trace);
                    shopMenu.itemPriceAndStock.Add(normalSlingshot, new int[] { normalSlingshotPrice, stock });
                    shopMenu.forSale.Add(normalSlingshot);
                    if (Game1.player.deepestMineLevel >= 70)
                    {
                        Monitor.Log("Adding master slingshot to shop menu", LogLevel.Trace);
                        shopMenu.itemPriceAndStock.Add(masterSlingshot, new int[] { masterSlingshotPrice, stock });
                        shopMenu.forSale.Add(masterSlingshot);
                    }
                }

                InventoryMenu inventoryMenu = Helper.Reflection.GetField<InventoryMenu>(shopMenu, "inventory").GetValue();
                InventoryMenu.highlightThisItem hightlightMethod = inventoryMenu.highlightMethod;
                bool NewHighlightMethod(Item item)
                {
                    return hightlightMethod(item) || item is Slingshot;
                }
                inventoryMenu.highlightMethod = NewHighlightMethod;

            }
        }

        private static bool HasAmmunition(Slingshot slingshot)
        {
            foreach (Object attachment in slingshot.attachments.Fields)
            {
                if (attachment != null && attachment.Stack > 0)
                    return true;
            }
            return false;
        }

        private class ModSlingshot : Slingshot
        {
            private readonly ShopMenu shopMenu;

            public ModSlingshot(int which, ShopMenu shopMenu) : base(which)
            {
                this.shopMenu = shopMenu;
            }

            public ModSlingshot(Slingshot slingshot, ShopMenu shopMenu) : base(slingshot.initialParentTileIndex)
            {
                this.shopMenu = shopMenu;
                Helper.Reflection.GetField<NetObjectArray<Object>>(this, "attachments").SetValue(slingshot.attachments);
            }

            public Slingshot ToSlingshot()
            {
                Slingshot slingshot = new Slingshot(this.InitialParentTileIndex);
                Helper.Reflection.GetField<NetObjectArray<Object>>(slingshot, "attachments").SetValue(this.attachments);
                return slingshot;
            }

            public override Item getOne()
            {
                return new ModSlingshot(this, shopMenu);
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

    }
}
