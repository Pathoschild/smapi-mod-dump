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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.IO;

namespace MapTeleport
{
    public partial class ModEntry
    {
 
        [HarmonyPatch(typeof(MapPage), nameof(MapPage.receiveLeftClick))]
        public class MapPage_receiveLeftClick_Patch
        {
            public static bool Prefix(MapPage __instance, int x, int y)
            {
                if (!Config.EnableMod)
                    return true;
                var coordinates = SHelper.GameContent.Load<CoordinatesList>(dictPath);
                bool added = false;
                bool found = false;
                foreach (ClickableComponent c in __instance.points)
                {
                    Coordinates co = coordinates.coordinates.Find(o => o.id == c.myID);
                    if (co == null)
                    {
                        coordinates.coordinates.Add(new Coordinates() { name = c.name, id = c.myID, enabled = false });
                        added = true;
                        continue;
                    }
                    if (c.containsPoint(x, y) && co.enabled)
                    {
                        SMonitor.Log($"Teleporting to {c.name} ({c.myID}), {co.mapName}, {co.x},{co.y}", LogLevel.Debug);
                        Game1.activeClickableMenu?.exitThisMenu(true);
                        Game1.warpFarmer(co.mapName, co.x, co.y, false);
                        found = true;
                    }
                }
                if (added)
                {
                    SHelper.Data.WriteJsonFile("coordinates.json", coordinates);
                }
                return !found;
            }
        }
   }
}