using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolGeodes.Overrides
{
    public class ToolTilesHook
    {
        public static bool Prefix(Tool __instance, Vector2 tileLocation, int power, Farmer who, ref List<Vector2> __result)
        {
            ++power;
            // Modified from Prismatic tools
            __result = new List<Vector2>();
            int radius = 0;
            int length = 1;
            switch ( power )
            {
                case 2:
                    radius = 0;
                    length = 3;
                    break;
                case 3:
                    radius = 0;
                    length = 5;
                    break;
                case 4:
                    radius = 1;
                    length = 3;
                    break;
                case 5:
                    radius = 1;
                    length = 6;
                    break;
                case 6:
                    // TODO: Pull these from the prismatic tools config file
                    radius = 2;
                    length = 7;
                    break;
            }

            if (power != 1)
            {
                radius += who.HasAdornment(__instance is WateringCan ? ToolType.WateringCan : ToolType.Hoe, Mod.Config.GEODE_WIDTH);
                length += 2 * who.HasAdornment(__instance is WateringCan ? ToolType.WateringCan : ToolType.Hoe, Mod.Config.GEODE_LENGTH);
            }

            Vector2 direction;
            Vector2 orth;
            switch (who.FacingDirection)
            {
                case 0: direction = new Vector2(0, -1); orth = new Vector2(1, 0); break;
                case 1: direction = new Vector2(1, 0); orth = new Vector2(0, 1); break;
                case 2: direction = new Vector2(0, 1); orth = new Vector2(-1, 0); break;
                case 3: direction = new Vector2(-1, 0); orth = new Vector2(0, -1); break;
                default: direction = new Vector2(0, 0); orth = new Vector2(0, 0); break;
            }
            for (int i = 0; i < length; i++)
            {
                __result.Add(direction * i + tileLocation);
                for (int j = 1; j <= radius; j++)
                {
                    __result.Add(direction * i + orth * j + tileLocation);
                    __result.Add(direction * i + orth * -j + tileLocation);
                }
            }

            return false;
        }
    }
}
