using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Menus;

namespace FastTravel
{
    public class FastTravelUtils
    {
        /// <summary>
        /// Checks if a point exists within the config.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool PointExistsInConfig(ClickableComponent point)
        {
            return ModEntry.Config.FastTravelPoints.Any(t => point.name.StartsWith(t.MapName.Replace("{0}", Game1.player.farmName.Value)));
        }

        /// <summary>
        /// Gets a GameLocation for a corresponding Point(from the Map)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static GameLocation GetLocationForMapPoint(ClickableComponent point)
        {
            string pointName = point.name;
            return Game1.locations[ModEntry.Config.FastTravelPoints.First(t => pointName.StartsWith(t.MapName.Replace("{0}", Game1.player.farmName.Value))).GameLocationIndex];
        }

        /// <summary>
        /// Gets a FastTravelPoint(struct) for a corresponding Point(from the map)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static FastTravelPoint GetFastTravelPointForMapPoint(ClickableComponent point)
        {
            string pointName = point.name.Replace("{0}", Game1.player.farmName.Value);
            return ModEntry.Config.FastTravelPoints.First(t => pointName.StartsWith(t.MapName.Replace("{0}", Game1.player.farmName.Value)));
        }
    }
}