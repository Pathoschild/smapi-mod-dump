/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/oranisagu/SDV-FarmAutomation
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace FarmAutomation.Common
{
    public class LocationHelper
    {
        public static string GetName(GameLocation location)
        {
            return location.uniqueName ?? location.Name;
        }

        public static bool IsTileOnMap(GameLocation location, Vector2 position)
        {
            if (location.Objects.ContainsKey(position))
            {
                return true;
            }
            if (location.terrainFeatures.ContainsKey(position))
            {
                return true;
            }
            return false;
        }
    }
}
