/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kesesek/StardewValleyMapTeleport
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;

namespace MapTeleport
{
    public partial class ModEntry
    {
        protected static CoordinatesList addedCoordinates;

        public static bool CheckClickableComponents(List<ClickableComponent> components, int topX, int topY, int x, int y)
        {
            SMonitor.Log($"clicked x:{x} y:{y}", LogLevel.Debug);
            if (!Config.ModEnabled)
                return false;

            if (addedCoordinates == null)
            {
                addedCoordinates = SHelper.Data.ReadJsonFile<CoordinatesList>("coordinates.json");
                if (addedCoordinates == null) addedCoordinates = new CoordinatesList();
            }

            var coordinates = SHelper.GameContent.Load<CoordinatesList>(dictPath);
            bool found = false;

            Dictionary<string, Coordinates> coordinatesDict = new Dictionary<string, Coordinates>(StringComparer.OrdinalIgnoreCase);
            foreach (var coord in coordinates.coordinates)
            {
                if (!coordinatesDict.ContainsKey(coord.displayName))
                {
                    coordinatesDict[coord.displayName] = coord;
                }
            }

            foreach (ClickableComponent component in components)
            {
                if (coordinatesDict.TryGetValue(component.name, out Coordinates tpCoordinate))
                {
                    if (component.containsPoint(x, y) && component.visible)
                    {
                        SMonitor.Log($"Teleporting to {tpCoordinate.displayName}\nCoordinate: {tpCoordinate.teleportName}({tpCoordinate.x},{tpCoordinate.y})", LogLevel.Debug);
                        Game1.activeClickableMenu?.exitThisMenu(true);
                        Game1.warpFarmer(tpCoordinate.teleportName, tpCoordinate.x, tpCoordinate.y, false);
                        found = true;
                        break;
                    }
                }
            }

            if (!found)
            {
                SMonitor.Log("No teleportation coordinate found.", LogLevel.Debug);
            }

            return found;
        }

        [HarmonyPatch(typeof(MapPage), nameof(MapPage.receiveLeftClick))]
        public class MapPage_receiveLeftClick_Patch
        {
            public static bool Prefix(MapPage __instance, int x, int y)
            {
                List<ClickableComponent> clickableComponents = new List<ClickableComponent>(__instance.points.Values);
                bool found = CheckClickableComponents(clickableComponents, __instance.xPositionOnScreen, __instance.yPositionOnScreen, x, y);
                return !found;
            }
        }

        [HarmonyPatch(typeof(IClickableMenu), nameof(IClickableMenu.receiveLeftClick))]
        public class RSVMapPage_receiveLeftClick_Patch
        {
            public static bool Prefix(IClickableMenu __instance, int x, int y)
            {
                bool found = false;
                if (__instance.allClickableComponents != null && __instance.GetType().Name == "RSVWorldMap")
                {
                    found = CheckClickableComponents(__instance.allClickableComponents, 0, 0, x - __instance.xPositionOnScreen, y - __instance.yPositionOnScreen);
                }
                return !found;
            }
        }
    }
}