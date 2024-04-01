/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Richard2091/MapTeleport
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

        public static bool CheckClickableComponents(List<ClickableComponent> components, int topX, int topY, int x,
            int y)
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
            // Sort boundries so that the function will warp to the smallest overlapping area
            components.Sort(delegate(ClickableComponent a, ClickableComponent b)
            {
                return (a.bounds.Height * a.bounds.Width).CompareTo(b.bounds.Height * b.bounds.Width);
            });
            // 查找点击的组件
            foreach (ClickableComponent component in components)
            {
                // 判断点击区域且检查是否可见
                if (component.containsPoint(x, y) && component.visible)
                {
                    string componentName = component.name;
                    int underscoreIndex = componentName.IndexOf('_');
                    // 如果包含下划线，则进行截取
                    if (underscoreIndex != -1)
                    {
                        componentName = componentName.Substring(0, underscoreIndex);
                    }
                    SMonitor.Log($"Component Information：\nID:{component.myID}, name:{component.name}, label:{component.label}",
                        LogLevel.Debug);
                    // 获取具体坐标
                    foreach (Coordinates tpCoordinate in coordinates.coordinates)
                    {
                        if (componentName.Equals(tpCoordinate.displayName))
                        {
                            SMonitor.Log(
                                $"Teleporting to {componentName}\nCoordinate: {tpCoordinate.teleportName}({tpCoordinate.x},{tpCoordinate.y})",
                                LogLevel.Debug);
                            Game1.activeClickableMenu?.exitThisMenu(true);
                            Game1.warpFarmer(tpCoordinate.teleportName, tpCoordinate.x, tpCoordinate.y,
                                false);
                            found = true;
                            break;
                        }
                    }
                }
            }
            SMonitor.Log("No teleportation coordinate found.");

            return found;
        }

        [HarmonyPatch(typeof(MapPage), nameof(MapPage.receiveLeftClick))]
        public class MapPage_receiveLeftClick_Patch
        {
            public static bool Prefix(MapPage __instance, int x, int y)
            {
                List<ClickableComponent> clickableComponents = new List<ClickableComponent>(__instance.points.Values);
                bool found = CheckClickableComponents(clickableComponents, __instance.xPositionOnScreen,
                    __instance.yPositionOnScreen, x, y);
                return !found;
                //SMonitor.Log($"clicked x:{x} y:{y}", LogLevel.Debug);
                //return false;
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
                    // RSV uses component x,y's that are not offset, however they need to be offset to check for the mouse position
                    found = CheckClickableComponents(__instance.allClickableComponents, 0, 0,
                        x - __instance.xPositionOnScreen, y - __instance.yPositionOnScreen);
                }

                return !found;
            }
        }
    }
}