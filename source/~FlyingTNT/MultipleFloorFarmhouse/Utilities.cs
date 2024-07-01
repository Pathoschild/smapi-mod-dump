/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/

using xTile.ObjectModel;
using xTile;

namespace MultipleFloorFarmhouse
{
    internal class Utilities
    {
        public static void AddWarp(Map map, string targetLocation, int thisX, int thisY, int targetX, int targetY)
        {
            // The Warp property should be a series of items in this format, separated by spaces
            string warpString = $"{thisX} {thisY} {targetLocation} {targetX} {targetY}";
            
            if (map.Properties.TryGetValue("Warp", out PropertyValue property))
            {
                map.Properties["Warp"] = property + " " + warpString;
                return;
            }

            map.Properties["Warp"] = warpString;
            return;
        }

        public static void ClearWarps(Map map)
        {
            if(map.Properties.ContainsKey("Warp"))
            {
                map.Properties.Remove("Warp");
            }
        }
    }
}
