/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revitalize.Framework.Objects;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

namespace Revitalize.Framework.Hacks
{
    /// <summary>
    /// Deals with hijacking menus for custom logic.
    /// </summary>
    public class MenuHacks
    {
        /// <summary>
        /// Checks to see if the mod has had it's custom object processed at the end of the day.
        /// </summary>
        public static bool EndOfDay_HasProcessedModdedItems;
        /// <summary>
        /// Checks to see if the end of day menus are up and running.
        /// </summary>
        /// <returns></returns>
        public static bool EndOfDay_IsShowingEndOfNightMenus()
        {
            return Game1.showingEndOfNightStuff;
        }

        /// <summary>
        /// Checks to see if the current end of day menu is the shippping menu.
        /// </summary>
        /// <returns></returns>
        public static bool EndOfDay_IsEndOfDayMenuShippingMenu()
        {
            if (EndOfDay_IsShowingEndOfNightMenus())
            {
                if (Game1.activeClickableMenu.GetType() == typeof(StardewValley.Menus.ShippingMenu)) return true;
                if (Game1.endOfNightMenus.Count == 0 && Game1.activeClickableMenu==null) return false;
                if (Game1.endOfNightMenus.Peek().GetType() == typeof(StardewValley.Menus.ShippingMenu))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else return false;
        }

        /// <summary>
        /// Gets the shipping menu from the end of day menus.
        /// </summary>
        /// <returns></returns>
        public static ShippingMenu EndOfDay_GetShippingMenu()
        {
            if (EndOfDay_IsEndOfDayMenuShippingMenu())
            {
                if (Game1.activeClickableMenu.GetType() == typeof(StardewValley.Menus.ShippingMenu))
                {
                    return (ShippingMenu)Game1.activeClickableMenu;
                }
                return (ShippingMenu)Game1.endOfNightMenus.Peek();
            }
            return null;
        }

        /// <summary>
        /// Hijacks the shipping menu to process modded items.
        /// </summary>
        public static void EndOfDay_HackShipping()
        {
            if (EndOfDay_GetShippingMenu() != null)
            {
                //ModCore.log("Hooked shipping menu!");
                ShippingMenu menu = EndOfDay_GetShippingMenu();

                List<int> categoryTotals = new List<int>();
                List<MoneyDial> categoryDials = new List<MoneyDial>();
                List<List<Item>> categoryItems = new List<List<Item>>();

                var CT_R=ModCore.ModHelper.Reflection.GetField<List<int>>(menu, "categoryTotals", true);
                var CD_R= ModCore.ModHelper.Reflection.GetField<List<MoneyDial>>(menu, "categoryDials", true);
                var CI_R= ModCore.ModHelper.Reflection.GetField<List<List<Item>>>(menu, "categoryItems", true);

                categoryTotals = CT_R.GetValue();
                categoryDials = CD_R.GetValue();
                categoryItems = CI_R.GetValue();

                //Recalculate other category.
                foreach (CustomObject obj in categoryItems[4])
                {
                    ModCore.log(obj.Name);
                    if (obj is StardewValley.Object)
                    {
                        ModCore.log(obj.sellToStorePrice());
                        categoryTotals[4] += obj.sellToStorePrice() * obj.Stack;
                        Game1.stats.itemsShipped += (uint)obj.Stack;
                        /*
                        if (o.Category == -75 || o.Category == -79)
                            Game1.stats.CropsShipped += (uint)o.Stack;
                        if (o.countsForShippedCollection())
                            Game1.player.shippedBasic((int)((NetFieldBase<int, NetInt>)o.parentSheetIndex), (int)((NetFieldBase<int, NetInt>)o.stack));
                            */
                    }
                }
                categoryTotals[5] = 0;
                for (int index = 0; index < 5; ++index)
                {
                    categoryTotals[5] += categoryTotals[index];
                    categoryItems[5].AddRange((IEnumerable<Item>)categoryItems[index]);
                    categoryDials[index].currentValue = categoryTotals[index];
                    categoryDials[index].previousTargetValue = categoryDials[index].currentValue;
                }
                categoryDials[5].currentValue = categoryTotals[5];
                if (Game1.IsMasterGame)
                    Game1.player.Money += categoryTotals[5];
                Game1.setRichPresence("earnings", (object)categoryTotals[5]);

            }
        }

        /// <summary>
        /// Triggers 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="sender"></param>
        public static void EndOfDay_RenderCheck(object o, StardewModdingAPI.Events.RenderedEventArgs sender)
        {
            if (EndOfDay_IsShowingEndOfNightMenus() && EndOfDay_HasProcessedModdedItems==false)
            {
                EndOfDay_HackShipping();
                EndOfDay_HasProcessedModdedItems = true;
            }
        }

        public static void EndOfDay_CleanupForNewDay(object o, StardewModdingAPI.Events.SavedEventArgs sender)
        {
            EndOfDay_HasProcessedModdedItems = false;
        }


        /// <summary>
        /// Recreates the farmhand's inventory even when they are offline.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void RecreateFarmhandInventory(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            if (e.NewMenu != null)
            {
                ModCore.log(e.NewMenu.GetType());

                if (e.NewMenu.GetType() == typeof(StardewValley.Menus.ItemGrabMenu))
                {
                    if (Game1.player.currentLocation is Cabin)
                    {
                        //ModCore.log("Let's get processing!");
                        List<KeyValuePair<int, Item>> addition = new List<KeyValuePair<int, Item>>();
                        for (int i = 0; i < (Game1.activeClickableMenu as ItemGrabMenu).ItemsToGrabMenu.actualInventory.Count; i++)
                        {
                            Item I = (Game1.activeClickableMenu as ItemGrabMenu).ItemsToGrabMenu.actualInventory[i];
                            if (I is Chest && I.Name != "Chest")
                            {
                                //ModCore.log("Found a custom object!");
                                Item cObj = ModCore.Serializer.DeserializeFromFarmhandInventory(I.Name);
                                if (cObj == null) continue;
                                addition.Add(new KeyValuePair<int, Item>(i, cObj));
                            }
                        }

                        foreach (KeyValuePair<int, Item> pair in addition)
                        {
                            (Game1.activeClickableMenu as ItemGrabMenu).ItemsToGrabMenu.actualInventory[pair.Key] = pair.Value;
                        }

                        (Game1.activeClickableMenu as ItemGrabMenu).ItemsToGrabMenu = new InventoryMenu((Game1.activeClickableMenu as ItemGrabMenu).ItemsToGrabMenu.xPositionOnScreen, (Game1.activeClickableMenu as ItemGrabMenu).ItemsToGrabMenu.yPositionOnScreen, true, (Game1.activeClickableMenu as ItemGrabMenu).ItemsToGrabMenu.actualInventory, (Game1.activeClickableMenu as ItemGrabMenu).ItemsToGrabMenu.highlightMethod, (Game1.activeClickableMenu as ItemGrabMenu).ItemsToGrabMenu.capacity, (Game1.activeClickableMenu as ItemGrabMenu).ItemsToGrabMenu.rows, (Game1.activeClickableMenu as ItemGrabMenu).ItemsToGrabMenu.horizontalGap, (Game1.activeClickableMenu as ItemGrabMenu).ItemsToGrabMenu.verticalGap, (Game1.activeClickableMenu as ItemGrabMenu).ItemsToGrabMenu.drawSlots);
                        (Game1.activeClickableMenu as ItemGrabMenu).populateClickableComponentList();
                        (Game1.activeClickableMenu as ItemGrabMenu).ItemsToGrabMenu.populateClickableComponentList();
                    }
                }
            }
        }

    }
}
