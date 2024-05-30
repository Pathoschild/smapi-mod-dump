/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace WaterRetainingFieldMod
{
    public class HoeDirtOverrides
    {
        internal static Dictionary<Vector2, int> TileLocationState = new();

        public static bool DayUpdatePrefix(HoeDirt __instance, ref int __state)
        {
            __state = __instance.state.Value;
            return true;
        }

        public static void DayUpdatePostfix(HoeDirt __instance, ref int __state)
        {
            GameLocation environment = __instance.Location;
            Vector2 tileLocation = __instance.Tile;
            if (environment is Farm || environment.isGreenhouse.Value )
            {
                if ((!__instance.hasPaddyCrop() || !__instance.paddyWaterCheck()) && __state == 1 && __instance.fertilizer.Value is "(O)370" or "(O)371")
                {
                    if (TileLocationState.TryGetValue(tileLocation, out var value))
                    {
                        __instance.state.Value = value;
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

        private static void AddStateAdjacentFertilizedTiles(GameLocation environment, Vector2 tileLocation, int stateValue, string fertilizer)
        {
            Vector2[] adjacent = new Vector2[]
            {
                new Vector2(tileLocation.X, tileLocation.Y + 1)
                , new Vector2(tileLocation.X + 1, tileLocation.Y)
                , new Vector2(tileLocation.X - 1, tileLocation.Y)
                , new Vector2(tileLocation.X, tileLocation.Y - 1)
            };
            foreach (var adjacentTileLocation in adjacent)
            {
                if (!TileLocationState.ContainsKey(adjacentTileLocation) && environment.terrainFeatures.ContainsKey(adjacentTileLocation) && environment.terrainFeatures[adjacentTileLocation] is HoeDirt hoeDirt)
                {
                    if (hoeDirt.state.Value == 1 && hoeDirt.fertilizer.Value == fertilizer && (!hoeDirt.hasPaddyCrop() || !hoeDirt.paddyWaterCheck()))
                    {
                        TileLocationState[adjacentTileLocation] = stateValue;
                        AddStateAdjacentFertilizedTiles(environment, adjacentTileLocation, stateValue, fertilizer);
                    }
                }
            }

        }
    }
}
