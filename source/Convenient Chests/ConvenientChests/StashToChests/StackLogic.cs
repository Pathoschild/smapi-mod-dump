/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using ConvenientChests.CategorizeChests.Framework;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace ConvenientChests.StashToChests {
    public static class StackLogic {
        internal static string StashCueName => Game1.soundBank.GetCue("pickUpItem").Name;

        public delegate bool AcceptingFunction(Chest c, Item i);

        public static IEnumerable<Chest> GetNearbyChests(this Farmer farmer, int radius)
            => GetNearbyChests(farmer.currentLocation, farmer.Tile, radius);

        public static void StashToChest(Chest chest, AcceptingFunction f) {
            if (!TryStashToChest(chest, f))
                return;

            Game1.playSound(StashCueName);
        }

        public static void StashToChests(IEnumerable<Chest> chests, AcceptingFunction f) {
            if (TryStashToChests(chests, f))
                Game1.playSound(StashCueName);
        }

        internal static bool TryStashToChests(IEnumerable<Chest> chests, AcceptingFunction f) {
            var movedAtLeastOne = false;

            foreach (var chest in chests)
                if (TryStashToChest(chest, f))
                    movedAtLeastOne = true;

            return !movedAtLeastOne;
        }

        public static void StashToNearbyChests(int radius, AcceptingFunction f)
            => StashToChests(Game1.player.GetNearbyChests(radius), f);

        internal static bool TryStashToChest(Chest chest, AcceptingFunction f) {
            // find items to be moved
            var toBeMoved = Game1.player.Items
                                 .Where(i => i != null && f(chest, i))
                                 .ToList();

            // try to move items to chest
            return toBeMoved.Any() && Game1.player.Items.DumpItemsToChest(chest, toBeMoved).Any();
        }


        private static IEnumerable<Chest> GetNearbyChests(GameLocation location, Vector2 point, int radius) {
            // chests
            foreach (var c in GetNearbyObjects<Chest>(location, point, radius))
                yield return c;

            // fridge
            if (location is FarmHouse { upgradeLevel: > 0 } farmHouse) {
                if (InRadius(radius, point,
                             farmHouse.getKitchenStandingSpot().X + 1,
                             farmHouse.getKitchenStandingSpot().Y - 2))
                    yield return farmHouse.fridge.Value;

                yield break;
            }

            // buildings
            var buildings = location.buildings
                                    .Where(building => InRadius(radius, point, building.tileX.Value, building.tileY.Value));

            foreach (var chest in buildings.SelectMany(building => building.buildingChests))
                yield return chest;
        }

        private static IEnumerable<T> GetNearbyObjects<T>(GameLocation location, Vector2 point, int radius) where T : Object =>
            location.Objects.Pairs
                    .Where(p => p.Value is T && InRadius(radius, point, p.Key))
                    .Select(p => (T) p.Value);

        private static bool InRadius(int radius, Vector2 a, Vector2 b) => Math.Abs(a.X - b.X) < radius && Math.Abs(a.Y - b.Y) < radius;
        private static bool InRadius(int radius, Vector2 a, int x, int y) => Math.Abs(a.X - x) < radius && Math.Abs(a.Y - y) < radius;
    }
}