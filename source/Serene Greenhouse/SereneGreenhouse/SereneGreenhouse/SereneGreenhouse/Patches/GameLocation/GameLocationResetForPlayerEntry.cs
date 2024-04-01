/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SereneGreenhouse
**
*************************************************/

using HarmonyLib;
using StardewValley;
using System.Reflection;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System.Linq;
using xTile.Dimensions;
using xTile.Tiles;
using System.Collections.Generic;
using StardewValley.BellsAndWhistles;
using System;
using StardewValley.Characters;
using SereneGreenhouse.SereneGreenhouse.Extensions;

namespace SereneGreenhouse.Patches.GameLocation
{
    [HarmonyPatch]
    public class GameLocationResetForPlayerEntry
    {
        private static IMonitor monitor = ModEntry.monitor;

        private static int junimoSpawnChance = 10;
        private static List<Vector2> junimoSpawnLocations = new List<Vector2>()
        {
            new Vector2(9, 37),
            new Vector2(37, 31),
            new Vector2(37, 7),
            new Vector2(14, 20)
        };

        private static int maxButterflySpawnPerTile = 7;
        private static int butterflySpawnChance = 75;
        private static List<Vector2> butterflySpawnLocations = new List<Vector2>()
        {
            new Vector2(9, 39),
            new Vector2(37, 31),
            new Vector2(37, 7),
            new Vector2(14, 20),
            new Vector2(25, 37),
            new Vector2(37, 43),
            new Vector2(25, 46),
            new Vector2(8, 11),
            new Vector2(20, 8),
            new Vector2(45, 28),
            new Vector2(6, 46),
            new Vector2(9, 28),
            new Vector2(44, 15)
        };

        private static int maxFireflySpawnPerTile = 6;
        private static int fireflySpawnChance = 75;
        private static List<Vector2> fireflySpawnLocations = new List<Vector2>()
        {
            new Vector2(9, 39),
            new Vector2(37, 31),
            new Vector2(37, 7),
            new Vector2(14, 20),
            new Vector2(25, 37),
            new Vector2(37, 43),
            new Vector2(25, 46),
            new Vector2(8, 11),
            new Vector2(20, 8),
            new Vector2(45, 28),
            new Vector2(6, 46),
            new Vector2(9, 28),
            new Vector2(44, 15)
        };

        internal static MethodInfo TargetMethod()
        {
            return AccessTools.Method(typeof(StardewValley.GameLocation), nameof(StardewValley.GameLocation.resetForPlayerEntry));
        }

        internal static void Postfix(StardewValley.GameLocation __instance)
        {
            if (__instance.Name != "Greenhouse")
            {
                return;
            }

            if (Game1.isRaining)
            {
                Game1.changeMusicTrack("rain");

                butterflySpawnChance -= 50;
                fireflySpawnChance -= 25;

                maxButterflySpawnPerTile -= 3;
                maxFireflySpawnPerTile -= 1;
            }
            else if (Game1.timeOfDay < 1800)
            {
                Game1.changeMusicTrack("woodsTheme");
            }

            // Spawn the critters
            __instance.critters = new List<Critter>();
            if (Game1.isDarkOut(__instance))
            {
                foreach (Vector2 tile in fireflySpawnLocations)
                {
                    if (Game1.random.Next(100) < fireflySpawnChance)
                    {
                        SpawnFireflies(__instance, tile, maxFireflySpawnPerTile);
                    }
                }
            }
            else
            {
                foreach (Vector2 tile in butterflySpawnLocations)
                {
                    if (Game1.random.Next(100) < butterflySpawnChance)
                    {
                        SpawnButterflies(__instance, tile, maxButterflySpawnPerTile);
                    }
                }

                if (!Game1.isRaining)
                {
                    // Spawn the Junimos
                    foreach (Vector2 tile in junimoSpawnLocations)
                    {
                        if (Game1.random.Next(100) < junimoSpawnChance)
                        {
                            SpawnJunimo(__instance, tile);
                        }
                    }
                }
            }
        }

        internal static void SpawnButterflies(StardewValley.GameLocation location, Vector2 tile, int maxSpawnPerTile)
        {
            location.critters.Add(new Butterfly(location, tile));
            for (int x = 0; x < Game1.random.Next(1, maxSpawnPerTile); x++)
            {
                location.critters.Add(new Butterfly(location, tile + new Vector2(Game1.random.Next(-2, 3), Game1.random.Next(-2, 3))));
            }
        }

        internal static void SpawnFireflies(StardewValley.GameLocation location, Vector2 tile, int maxSpawnPerTile)
        {
            location.critters.Add(new Firefly(tile));
            for (int x = 0; x < Game1.random.Next(1, maxSpawnPerTile); x++)
            {
                location.critters.Add(new Firefly(tile + new Vector2(Game1.random.Next(-2, 3), Game1.random.Next(-2, 3))));
            }
        }

        internal static void SpawnJunimo(StardewValley.GameLocation location, Vector2 tile)
        {
            if (!location.isTileLocationTotallyClearAndPlaceable(tile))
            {
                return;
            }

            Junimo j = new Junimo(tile * 64f, 6, false);
            if (!location.isCollidingPosition(j.GetBoundingBox(), Game1.viewport, j))
            {
                location.characters.Add(j);
            }

            Game1.playSound("junimoMeep1");
        }
    }
}
