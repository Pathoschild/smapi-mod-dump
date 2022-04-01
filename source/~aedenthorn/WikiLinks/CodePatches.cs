/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/


using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Object = StardewValley.Object;

namespace WikiLinks
{
    public partial class ModEntry
    {
        [HarmonyPatch(typeof(InventoryMenu), nameof(InventoryMenu.leftClick))]
        public class InventoryMenu_leftClick_Patch
        {
            public static bool Prefix(InventoryMenu __instance)
            {
                if (!Config.EnableMod || !SHelper.Input.IsDown(Config.LinkModButton))
                    return true;
                foreach(var i in __instance.inventory)
                {
                    if (i.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
                    {
                        int slotNumber = Convert.ToInt32(i.name);
                        if (slotNumber < __instance.actualInventory.Count)
                        {
                            if (__instance.actualInventory[slotNumber] != null)
                            {

                                OpenPage(__instance.actualInventory[slotNumber].Name);

                            }
                        }
                        return false;
                    }
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.checkAction))]
        public class GameLocation_checkAction_Patch
        {
            public static bool Prefix(GameLocation __instance, ref bool __result)
            {
                if (!Config.EnableMod || !SHelper.Input.IsDown(Config.LinkModButton))
                    return true;
                if (__instance.objects.TryGetValue(Game1.currentCursorTile, out Object obj))
                {
                    OpenPage(obj.Name);
                    __result = true;
                    return false;
                }
                if (__instance.terrainFeatures.TryGetValue(Game1.currentCursorTile, out TerrainFeature feature))
                {
                    if (feature is HoeDirt && (feature as HoeDirt).crop != null)
                    {
                        OpenPage(new Object((feature as HoeDirt).crop.indexOfHarvest.Value, 1).Name);
                        __result = true;
                        return false;
                    }
                }
                foreach (var c in __instance.characters)
                {
                    if (c.getTileLocation() == Game1.currentCursorTile)
                    {
                        if(c.isVillager())
                            OpenPage(c.Name);
                        else if(c is Pet)
                            OpenPage(c.GetType().Name);
                        __result = true;
                        return false;
                    }
                }
                if(__instance is AnimalHouse)
                {

                    foreach (var c in (__instance as AnimalHouse).animals.Values)
                    {
                        if (c.getTileLocation() == Game1.currentCursorTile)
                        {
                            OpenPage(c.GetType().Name);
                            __result = true;
                            return false;
                        }
                    }
                }
                return true;
            }
        }
    }
}