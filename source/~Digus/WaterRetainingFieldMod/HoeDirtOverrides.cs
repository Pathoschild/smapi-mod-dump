using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace WaterRetainingFieldMod
{
    public class HoeDirtOverrides
    {
        internal static Dictionary<Vector2, int> TileLocationState = new Dictionary<Vector2, int>();

        [HarmonyPriority(800)]
        public static bool DayUpdatePrefix(HoeDirt __instance, ref int __state)
        {
            __state = __instance.state.Value;
            return true;
        }

        [HarmonyPriority(800)]
        public static void DayUpdatePostfix(HoeDirt __instance, ref GameLocation environment, ref Vector2 tileLocation, ref int __state)
        {
            if (environment is Farm || environment.isGreenhouse.Value )
            {
                if ((!__instance.hasPaddyCrop() || !__instance.paddyWaterCheck(environment, tileLocation)) && __state == 1 && (__instance.fertilizer.Value == 370 || __instance.fertilizer.Value == 371))
                {
                    if (TileLocationState.ContainsKey(tileLocation))
                    {
                        __instance.state.Value = TileLocationState[tileLocation];
                        return;
                    }
                    else
                    {
                        TileLocationState[tileLocation] = __instance.state.Value;
                        AddStateAdjacentFertilizedTiles(environment, tileLocation, __instance.state.Value, __instance.fertilizer.Value);
                    }
                }
            }
        }

        private static void AddStateAdjacentFertilizedTiles(GameLocation environment, Vector2 tileLocation, int stateValue, int fertilizer)
        {
            Vector2[] adjasent = new Vector2[]
            {
                new Vector2(tileLocation.X, tileLocation.Y + 1)
                , new Vector2(tileLocation.X + 1, tileLocation.Y)
                , new Vector2(tileLocation.X - 1, tileLocation.Y)
                , new Vector2(tileLocation.X, tileLocation.Y - 1)
            };
            foreach (var adjacentTileLocation in adjasent)
            {
                if (!TileLocationState.ContainsKey(adjacentTileLocation) && environment.terrainFeatures.ContainsKey(adjacentTileLocation) && environment.terrainFeatures[adjacentTileLocation] is HoeDirt hoeDirt)
                {
                    if (hoeDirt.state.Value == 1 && hoeDirt.fertilizer.Value == fertilizer && (!hoeDirt.hasPaddyCrop() || !hoeDirt.paddyWaterCheck(environment, tileLocation)))
                    {
                        TileLocationState[adjacentTileLocation] = stateValue;
                        AddStateAdjacentFertilizedTiles(environment, adjacentTileLocation, stateValue, fertilizer);
                    }
                }
            }

        }
    }
}
