/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewDruid.Map
{
    static class WarpData
    {

        public static Dictionary<string, Vector2> WarpPoints()
        {

            int default_x = 48;
            int default_y = 7;

            if (Game1.whichFarm == 5)
            {
                default_x = 48;
                default_y = 39;
            }
            else if (Game1.whichFarm == 6)
            {
                default_x = 82;
                default_y = 29;
            }

            Point farmWarp = Game1.getFarm().GetMapPropertyPosition("WarpTotemEntry", default_x, default_y);

            Dictionary<string, Vector2> warpPoints = new()
            {
                ["Farm"] = new Vector2(farmWarp.X, farmWarp.Y),
                ["Mountain"] = new Vector2(31, 20),
                ["Beach"] = new Vector2(20, 4),
                ["Desert"] = new Vector2(35, 43),
                ["IslandSouth"] = new Vector2(11, 11)
            };

            return warpPoints;

        }

        public static Dictionary<string, int> WarpTotems()
        {
            Dictionary<string, int> warpTotems = new()
            {
                ["Farm"] = 688,
                ["Mountain"] = 689,
                ["Beach"] = 690,
                ["Desert"] = 261,
                ["IslandSouth"] = 886,
            };

            return warpTotems;

        }

    }
}
