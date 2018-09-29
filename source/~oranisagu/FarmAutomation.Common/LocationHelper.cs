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
