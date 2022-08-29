/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Rafseazz/Ridgeside-Village-Mod
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI.Events;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace RidgesideVillage
{
    internal static class Greenhouses
    {
        static IModHelper Helper;
        static IMonitor Monitor;
        internal static void Initialize(IMod ModInstance)
        {
            Helper = ModInstance.Helper;
            Monitor = ModInstance.Monitor;

            TileActionHandler.RegisterTileAction("ShipmentRSV", ShipmentBin);
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        private static void OnSaveLoaded(object sender, EventArgs ex)
        {
            //mark greenhouses as greenhouses
            List<string> locations = new List<string>() { "Custom_Ridgeside_AguarCaveTemporary", "Custom_Ridgeside_RSVGreenhouse1", "Custom_Ridgeside_RSVGreenhouse2" };
            if (Game1.MasterPlayer.mailReceived.Contains(IanShop.CLIMATECONTROLLED))
            {
                locations.Add("Custom_Ridgeside_SummitFarm");
            }
            foreach (var name in locations)
            {
                GameLocation location = Game1.getLocationFromName(name);
                if (location == null)
                {
                    Log.Trace($"RSV: Greenhouse {name} could not be found");
                    continue;
                }
                location.isGreenhouse.Value = true;
                Log.Trace($"RSV: {name} set to greenhouse");
            }
        }

        private static void ShipmentBin(string tileActionString, Vector2 position)
        {
            MethodInfo method = typeof(Farm).GetMethod("shipItem");
            ItemGrabMenu itemGrabMenu = new ItemGrabMenu((List<Item>)null, true, false, new InventoryMenu.highlightThisItem(Utility.highlightShippableObjects), (ItemGrabMenu.behaviorOnItemSelect)Delegate.CreateDelegate(typeof(ItemGrabMenu.behaviorOnItemSelect), (object)Game1.getFarm(), method), "", (ItemGrabMenu.behaviorOnItemSelect)null, true, true, false, true, false, 0, (Item)null, -1, (object)null);
            itemGrabMenu.initializeUpperRightCloseButton();
            int num1 = 0;
            itemGrabMenu.setBackgroundTransparency((uint)num1 > 0U);
            int num2 = 1;
            itemGrabMenu.setDestroyItemOnClick((uint)num2 > 0U);
            itemGrabMenu.initializeShippingBin();
            Game1.activeClickableMenu = (IClickableMenu)itemGrabMenu;
        }

        
    }
  
}
