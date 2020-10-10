/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jahangmar/StardewValleyMods
**
*************************************************/

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
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace InteractionTweaks
{
    public class MarniesItemShopFeature : ModFeature
    {
        private static Dictionary<ISalable, int[]> initialItemPriceAndStock;

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
            if (e.NewMenu is ShopMenu shopMenu && Game1.currentLocation.Name.Equals("AnimalShop"))
            {
                Monitor.Log("Adding Marnie's ranch shop changes", LogLevel.Trace);
                AddItems(shopMenu);
                Helper.Events.Display.RenderingActiveMenu += Display_RenderingActiveMenu;
                Helper.Events.Input.ButtonReleased += Input_ButtonReleased;
                ConvertItems(shopMenu);
            }
            else if (e.OldMenu is ShopMenu shopMenuOld && Game1.currentLocation.Name.Equals("AnimalShop"))
            {
                Monitor.Log("Removing Marnie's ranch shop changes", LogLevel.Trace);
                Helper.Events.Display.RenderingActiveMenu -= Display_RenderingActiveMenu;
                Helper.Events.Input.ButtonReleased -= Input_ButtonReleased;
                ResetItems(shopMenuOld);
            }
        }

        private static void Display_RenderingActiveMenu(object sender, RenderingActiveMenuEventArgs e)
        {
            ShopMenu shopMenu = (ShopMenu)Game1.activeClickableMenu;
            InventoryMenu inventoryMenu = Helper.Reflection.GetField<InventoryMenu>(shopMenu, "inventory").GetValue();
            Item heldItem = Helper.Reflection.GetField<Item>(shopMenu, "heldItem").GetValue();
            bool scrolling = Helper.Reflection.GetField<bool>(shopMenu, "scrolling").GetValue();
            int X = Game1.getMouseX();
            int Y = Game1.getMouseY();
            if (!scrolling && heldItem == null && inventoryMenu.getItemAt(X, Y) is Item item && item != null && NewSellableItem(item))
            {
                float sellPercentage = Helper.Reflection.GetField<float>(shopMenu, "sellPercentage").GetValue();
                int price = (int)((item is Object) ? ((float)(item as Object).sellToStorePrice() * sellPercentage) : ((float)(item.salePrice() / 2) * sellPercentage)) * item.Stack; //(int)(item.salePrice() / 2 * sellPercentage * fishingRod.Stack);
                string text = item.DisplayName + " x" + item.Stack;
                Helper.Reflection.GetField<int>(shopMenu, "hoverPrice").SetValue(price);
                Helper.Reflection.GetField<string>(shopMenu, "hoverText").SetValue(text);
            }
        }

        private static void Input_ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            ShopMenu shopMenu = (ShopMenu)Game1.activeClickableMenu;
            Item heldItem = Helper.Reflection.GetField<Item>(shopMenu, "heldItem").GetValue();
            int x = (int)e.Cursor.ScreenPixels.X;
            int y = (int)e.Cursor.ScreenPixels.Y;
            if (shopMenu.inventory.getItemAt(x,y) != null || heldItem != null)
                ConvertItems(shopMenu);
        }

        protected static void AddItems(ShopMenu shopMenu)
        {
            InventoryMenu inventoryMenu = Helper.Reflection.GetField<InventoryMenu>(shopMenu, "inventory").GetValue();
            InventoryMenu.highlightThisItem hightlightMethod = inventoryMenu.highlightMethod;
            bool NewHighlightMethod(Item item)
            {
                return hightlightMethod(item) || NewSellableItem(item);
            }
            inventoryMenu.highlightMethod = NewHighlightMethod;

            NetObjectList<Item> playerItems = Helper.Reflection.GetField<NetObjectList<Item>>(Game1.player, "items").GetValue();
            List<Item> itemsList = new List<Item>(playerItems);
            NetObjectList<Item> playerItemsCopy = new NetObjectList<Item>(itemsList);

            Helper.Reflection.GetField<NetObjectList<Item>>(Game1.player, "items").SetValue(playerItemsCopy);
            playerItemsCopy.Filter((Item item) => !(item is Shears || item is MilkPail));
            initialItemPriceAndStock = Utility.getAnimalShopStock();
            Helper.Reflection.GetField<NetObjectList<Item>>(Game1.player, "items").SetValue(playerItems);
        }

        private static bool NewSellableItem(Item item)
        {
            return (item.Name.Equals(ModHayObject.internalName) || item.Name.Equals(ModHayBaleObject.internalName) || item.Name.Equals(ModHeaterObject.internalName) || item is MilkPail || item is Shears || item.Name.Equals(ModAutoGrabberObject.internalName));
        }

        protected static void ConvertItems(ShopMenu shopMenu)
        {
            Monitor.Log("Converting items in inventory", LogLevel.Trace);
            InventoryMenu inventoryMenu = Helper.Reflection.GetField<InventoryMenu>(shopMenu, "inventory").GetValue();
            for (int i = 0; i < inventoryMenu.actualInventory.Count; i++)
            {
                if (inventoryMenu.actualInventory[i] is MilkPail milkPail && !(milkPail is ModMilkPail))
                    inventoryMenu.actualInventory[i] = new ModMilkPail(milkPail);
                if (inventoryMenu.actualInventory[i] is Shears shears && !(shears is ModShears))
                    inventoryMenu.actualInventory[i] = new ModShears(shears);
                if (inventoryMenu.actualInventory[i] is Object obj && obj.name.Equals(ModHayObject.internalName) && !(obj is ModHayObject))
                    inventoryMenu.actualInventory[i] = new ModHayObject(obj);
                if (inventoryMenu.actualInventory[i] is Object obj2 && obj2.name.Equals(ModHayBaleObject.internalName) && !(obj2 is ModHayBaleObject))
                    inventoryMenu.actualInventory[i] = new ModHayBaleObject(obj2);
                if (inventoryMenu.actualInventory[i] is Object obj3 && obj3.name.Equals(ModHeaterObject.internalName) && !(obj3 is ModHeaterObject))
                    inventoryMenu.actualInventory[i] = new ModHeaterObject(obj3);
                if (inventoryMenu.actualInventory[i] is Object obj4 && obj4.name.Equals(ModAutoGrabberObject.internalName) && !(obj4 is ModAutoGrabberObject))
                    inventoryMenu.actualInventory[i] = new ModAutoGrabberObject(obj4);
                //Monitor.Log($"inventory[{i}] is now {inventoryMenu.actualInventory[i]?.GetType().ToString()}", LogLevel.Trace);
            }
        }

        public static void ResetItems(ShopMenu shopMenu)
        {
            Monitor.Log("Resetting items in inventory", LogLevel.Trace);
            InventoryMenu inventoryMenu = Helper.Reflection.GetField<InventoryMenu>(shopMenu, "inventory").GetValue();
            for (int i = 0; i < inventoryMenu.actualInventory.Count; i++)
            {
                if (inventoryMenu.actualInventory[i] is ModMilkPail modMilkPail)
                    inventoryMenu.actualInventory[i] = modMilkPail.ToMilkPail();
                if (inventoryMenu.actualInventory[i] is ModShears modShears)
                    inventoryMenu.actualInventory[i] = modShears.ToShears();
                if (inventoryMenu.actualInventory[i] is ModHayObject obj)
                    inventoryMenu.actualInventory[i] = obj.ToObject();
                if (inventoryMenu.actualInventory[i] is ModHayBaleObject obj2)
                    inventoryMenu.actualInventory[i] = obj2.ToObject();
                if (inventoryMenu.actualInventory[i] is ModHeaterObject obj3)
                    inventoryMenu.actualInventory[i] = obj3.ToObject();
                if (inventoryMenu.actualInventory[i] is ModAutoGrabberObject obj4)
                    inventoryMenu.actualInventory[i] = obj4.ToObject();
                //Monitor.Log($"inventory[{i}] is now {inventoryMenu.actualInventory[i]?.GetType().ToString()}", LogLevel.Trace);
            }

        }

        private static int GetPrice(Item item)
        {
            foreach (KeyValuePair<ISalable, int[]> pair in initialItemPriceAndStock)
            {
                if (pair.Key is Item keyItem && keyItem.Name.Equals(item.Name))
                {
                    if (keyItem is Object)
                        return pair.Value[0] / 2;
                    else
                        return pair.Value[0];
                }
            }
            Monitor.Log($"Item ({item.Name}) not found in itemPriceAndStock of AnimalShop", LogLevel.Error);
            return 0;
        }

        private class ModMilkPail : MilkPail
        {
            private readonly MilkPail milkPail;

            public ModMilkPail(MilkPail milkPail) : base()
            {
                if (milkPail is ModMilkPail)
                    Monitor.Log("Bug: tried to create ModMilkPail from ModMilkPail", LogLevel.Error);
                this.milkPail = milkPail;
            }

            public MilkPail ToMilkPail() => milkPail;
            public override int salePrice() => GetPrice(milkPail);
        }

        private class ModShears : Shears
        {
            private readonly Shears shears;

            public ModShears(Shears shears) : base()
            {
                if (shears is ModShears)
                    Monitor.Log("Bug: tried to create ModShears from ModShears", LogLevel.Error);
                this.shears = shears;
            }

            public Shears ToShears() => shears;
            public override int salePrice() => GetPrice(shears);
        }

        private class ModHayObject : Object
        {
            private readonly Object obj;

            public const string internalName = "Hay";

            public ModHayObject(Object obj) : base(178, obj.Stack, false, -1, 0)
            {
                if (obj is ModHayObject)
                    Monitor.Log("Bug: tried to create ModHayObject from ModHayObject", LogLevel.Error);
                this.obj = obj;
            }

            public Object ToObject()
            {
                obj.Stack = this.Stack;
                return obj;
            }

            public override int salePrice() => GetPrice(obj);
            public override int sellToStorePrice(long specificPlayerID) => GetPrice(obj);
        }

        private class ModHeaterObject : Object
        {
            private readonly Object obj;

            public const string internalName = "Heater";

            public ModHeaterObject(Object obj) : base(Vector2.Zero, 104, false)
            {
                if (obj is ModHeaterObject)
                    Monitor.Log("Bug: tried to create ModHeaterObject from ModHeaterObject", LogLevel.Error);
                this.obj = obj;
            }

            public Object ToObject() => obj;

            public override int salePrice() => GetPrice(obj);
            public override int sellToStorePrice(long specificPlayerID) => GetPrice(obj);
        }

        private class ModAutoGrabberObject : Object
        {
            private readonly Object obj;

            public const string internalName = "Auto-Grabber";

            public ModAutoGrabberObject(Object obj) : base(Vector2.Zero, 165, false)
            {
                if (obj is ModAutoGrabberObject)
                    Monitor.Log("Bug: tried to create ModAutoGrabberObject from ModAutoGrabberObject", LogLevel.Error);
                this.obj = obj;
            }

            public Object ToObject() => obj;

            public override int salePrice() => GetPrice(obj);
            public override int sellToStorePrice(long specificPlayerID) => GetPrice(obj);
        }

        private class ModHayBaleObject : Object
        {
            private readonly Object obj;

            public const string internalName = "Ornamental Hay Bale";

            public ModHayBaleObject(Object obj) : base(Vector2.Zero, 45, false)
            {
                if (obj is ModHayBaleObject)
                    Monitor.Log("Bug: tried to create ModHayBaleObject from ModHayBaleObject", LogLevel.Error);
                this.obj = obj;
            }

            public Object ToObject() => obj;

            public override int salePrice() => GetPrice(obj);
            public override int sellToStorePrice(long specificPlayerID) => GetPrice(obj);
        }

    }
}
