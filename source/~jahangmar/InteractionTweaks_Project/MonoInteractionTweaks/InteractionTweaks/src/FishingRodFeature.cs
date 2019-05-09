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
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace InteractionTweaks
{
    public class FishingRodFeature : ModFeature
    {
        public static new void Enable()
        {
            Helper.Events.Display.MenuChanged += Display_MenuChanged;
        }

        public static new void Disable()
        {
            Helper.Events.Display.MenuChanged -= Display_MenuChanged;
        }

        private static void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu shopMenu && Game1.currentLocation.Name.Equals("FishShop"))
            {
                Monitor.Log("Adding FishShop changes", LogLevel.Trace);
                AddFishingRods(shopMenu);
                Helper.Events.Display.RenderingActiveMenu += Display_RenderingActiveMenu;
                Helper.Events.Input.ButtonPressed += Input_ButtonPressed;
                Helper.Events.Input.ButtonReleased += Input_ButtonReleased;
                ConvertFishingRods(shopMenu);
            }
            else if (e.OldMenu is ShopMenu shopMenuOld && Game1.currentLocation.Name.Equals("FishShop"))
            {
                Monitor.Log("Removing FishShop changes", LogLevel.Trace);
                Helper.Events.Display.RenderingActiveMenu -= Display_RenderingActiveMenu;
                Helper.Events.Input.ButtonPressed -= Input_ButtonPressed;
                Helper.Events.Input.ButtonReleased -= Input_ButtonReleased;
                ResetFishingRods(shopMenuOld);
            }
        }

        private static void AddFishingRods(ShopMenu shopMenu)
        {
            //change
            //Dictionary<Item, int[]> itemPriceAndStock = Helper.Reflection.GetField<Dictionary<Item, int[]>>(shopMenu, "itemPriceAndStock").GetValue();
            //List<Item> forSale = Helper.Reflection.GetField<List<Item>>(shopMenu, "forSale").GetValue();
            //List<int> categoriesToSellHere = Helper.Reflection.GetField<List<int>>(shopMenu, "categoriesToSellHere").GetValue();

            InventoryMenu inventoryMenu = Helper.Reflection.GetField<InventoryMenu>(shopMenu, "inventory").GetValue();
            InventoryMenu.highlightThisItem hightlightMethod = inventoryMenu.highlightMethod;
            bool NewHighlightMethod(Item item) {
                return hightlightMethod(item) || item is FishingRod;
            }
            inventoryMenu.highlightMethod = NewHighlightMethod;
        }

        static void Display_RenderingActiveMenu(object sender, RenderingActiveMenuEventArgs e)
        {
            ShopMenu shopMenu = (ShopMenu)Game1.activeClickableMenu;
            InventoryMenu inventoryMenu = Helper.Reflection.GetField<InventoryMenu>(shopMenu, "inventory").GetValue();
            Item heldItem = Helper.Reflection.GetField<Item>(shopMenu, "heldItem").GetValue();
            bool scrolling = Helper.Reflection.GetField<bool>(shopMenu, "scrolling").GetValue();
            int X = Game1.getMouseX();
            int Y = Game1.getMouseY();
            if (!scrolling && heldItem == null && inventoryMenu.getItemAt(X, Y) is FishingRod fishingRod)
            {
                Helper.Reflection.GetField<string>(shopMenu, "boldTitleText").SetValue(fishingRod.DisplayName);
                Helper.Reflection.GetField<string>(shopMenu, "hoverText").SetValue(fishingRod.getDescription());
                float sellPercentage = Helper.Reflection.GetField<float>(shopMenu, "sellPercentage").GetValue();
                int price = (int)(fishingRod.salePrice() / 2 * sellPercentage * fishingRod.Stack);
                Helper.Reflection.GetField<int>(shopMenu, "hoverPrice").SetValue(price);
                Helper.Reflection.GetField<Item>(shopMenu, "hoveredItem").SetValue(fishingRod);
                if (HasAttachment(fishingRod))
                    Helper.Reflection.GetField<string>(shopMenu, "hoverText").SetValue(ModFeature.GetTrans("menu.fishingrodempty"));
            }
        }


        static void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {

            ShopMenu shopMenu = (ShopMenu)Game1.activeClickableMenu;

            Item heldItem = Helper.Reflection.GetField<Item>(shopMenu, "heldItem").GetValue();
            InventoryMenu inventoryMenu = Helper.Reflection.GetField<InventoryMenu>(shopMenu, "inventory").GetValue();
            if ((e.Button.Equals(SButton.MouseLeft) || e.Button.Equals(SButton.MouseRight)) && heldItem == null && inventoryMenu.getItemAt((int)e.Cursor.ScreenPixels.X, (int)e.Cursor.ScreenPixels.Y) is FishingRod fishingRod)
            {
                if (HasAttachment(fishingRod))
                {
                    Game1.playSound("cancel");
                    Game1.showRedMessage(ModFeature.GetTrans("menu.fishingrodempty"));
                    Helper.Input.Suppress(e.Button);
                }
            }
        }

        static void Input_ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            ShopMenu shopMenu = (ShopMenu)Game1.activeClickableMenu;
            ConvertFishingRods(shopMenu);
        }



        private static bool HasAttachment(FishingRod fishingRod)
        {
            foreach (Object attachment in fishingRod.attachments.Fields)
            {
                if (attachment != null && attachment.Stack > 0)
                    return true;
            }
            return false;
        }

        private static void ConvertFishingRods(ShopMenu shopMenu)
        {
            InventoryMenu inventoryMenu = Helper.Reflection.GetField<InventoryMenu>(shopMenu, "inventory").GetValue();
            for (int i = 0; i < inventoryMenu.actualInventory.Count; i++)
                if (inventoryMenu.actualInventory[i] is FishingRod fishingRod)
                    inventoryMenu.actualInventory[i] = new ModFishingRod(fishingRod, shopMenu);
        }

        private static void ResetFishingRods(ShopMenu shopMenu)
        {
            InventoryMenu inventoryMenu = Helper.Reflection.GetField<InventoryMenu>(shopMenu, "inventory").GetValue();
            for (int i = 0; i < inventoryMenu.actualInventory.Count; i++)
                if (inventoryMenu.actualInventory[i] is ModFishingRod modFishingRod)
                    inventoryMenu.actualInventory[i] = modFishingRod.ToFishingRod();
        }

        private class ModFishingRod : FishingRod
        {
            private Dictionary<Item, int[]> itemPriceAndStock;

            public ModFishingRod(FishingRod fishingRod, ShopMenu shopMenu) : base(fishingRod.upgradeLevel)
            {
                Helper.Reflection.GetField <NetObjectArray<Object>> (this, "attachments").SetValue(fishingRod.attachments);
                itemPriceAndStock = Helper.Reflection.GetField<Dictionary<Item, int[]>>(shopMenu, "itemPriceAndStock").GetValue();
            }

            public FishingRod ToFishingRod()
            {
                FishingRod fishingRod = new FishingRod(upgradeLevel);
                Helper.Reflection.GetField<NetObjectArray<Object>>(fishingRod, "attachments").SetValue(this.attachments);
                return fishingRod;
            }

            public override int salePrice()
            {
                foreach (KeyValuePair<Item, int[]> pair in itemPriceAndStock)
                {
                    if (pair.Key is FishingRod fishingRod && fishingRod.upgradeLevel == this.upgradeLevel)
                    {
                        return pair.Value[0];
                    }
                }
                Monitor.Log($"Fishing Rod (upgradeLevel={upgradeLevel} not found in itemPriceAndStock", LogLevel.Error);
                return 0;
            }
        }
    }
}
