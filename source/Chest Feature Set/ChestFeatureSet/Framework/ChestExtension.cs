/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zack20136/ChestFeatureSet
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI.Framework.ModLoading.Rewriters.StardewValley_1_6;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;

namespace ChestFeatureSet.Framework
{
    public static class ChestExtension
    {
        /// <summary>
        /// Search ContainsItem By ItemId & ItemType
        /// </summary>
        /// <param name="chest"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static bool ContainsItem(this Chest chest, Item i)
        {
            if (i == null)
                return false;
            return chest.Items.Any(item => item.ItemId == i.ItemId && item.TypeDefinitionId == i.TypeDefinitionId);
        }

        /// <summary>
        /// Search ContainsItem By Existing Stacks
        /// </summary>
        /// <param name="chest"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static bool ContainsItemByStack(this Chest chest, Item i) => chest.Items.Any(i.canStackWith);

        ///  <summary>
        ///  Attempt to move as much as possible of the player's inventory into the given chest
        ///  </summary>
        /// <param name="chest">The chest to put the items in.</param>
        /// <param name="sourceInventory"></param>
        /// <param name="items">Items to put in</param>
        /// <returns>List of Items that were successfully moved into the chest</returns>
        public static IEnumerable<Item> DumpItemsToChest(this Chest chest, IList<Item> sourceInventory, IEnumerable<Item> items)
            => items.Where(item => item != null).Where(item => TryMoveItemToChest(chest, sourceInventory, item)).ToList();

        /// <summary>
        /// Attempt to move as much as possible of the given item stack into the chest.
        /// </summary>
        /// <param name="chest">The chest to put the items in.</param>
        /// <param name="sourceInventory"></param>
        /// <param name="item">The items to put in the chest.</param>
        /// <returns>True if at least some of the stack was moved into the chest.</returns>
        public static bool TryMoveItemToChest(this Chest chest, IList<Item> sourceInventory, Item item)
        {
            var remainder = chest.addItem(item);

            // nothing remains -> remove item
            if (remainder == null)
            {
                sourceInventory[sourceInventory.IndexOf(item)] = null;
                return true;
            }

            // nothing changed
            if (remainder.Stack == item.Stack)
                return false;

            // update stack count
            item.Stack = remainder.Stack;
            return true;
        }

        /// <summary>
        /// Check whether the given chest has any completely empty slots.
        /// </summary>
        /// <returns>Whether at least one slot is empty.</returns>
        /// <param name="chest">The chest to check.</param>
        public static bool HasEmptySlots(this Chest chest)
            => chest.Items.Count < Chest.capacity || chest.Items.Any(i => i == null);

        /// <summary>
        /// Gets all the chests in the world with all locations
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ChestLocationPair> GetAllChests()
            => from location in LocationExtension.GetAllLocations()
               from @object in location.objects.Values
               where @object is Chest
               select new ChestLocationPair((Chest)@object, location);

        /// <summary>
        /// Gets all the chests in the area with all locations
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ChestLocationPair> GetAreaChests(IEnumerable<string> area)
            => GetAllChests().Where(chestPair => area.Contains(chestPair.Location.Name));

        /// <summary>
        /// Get the chests nearby farmer
        /// </summary>
        public static IEnumerable<Chest> GetNearbyChests(this Farmer farmer, int radius)
            => GetNearbyChests(farmer.currentLocation, farmer.TilePoint.ToVector2(), radius);

        /// <summary>
        /// Get the chests nearby the point
        /// </summary>
        public static IEnumerable<Chest> GetNearbyChests(GameLocation location, Vector2 point, int radius)
        {
            // chests
            foreach (Chest c in GetNearbyObjects<Chest>(location, point, radius))
                yield return c;

            switch (location)
            {
                // fridge
                case FarmHouse farmHouse when farmHouse.upgradeLevel > 0:
                    if (InRadius(radius, point, farmHouse.getKitchenStandingSpot().X + 1, farmHouse.getKitchenStandingSpot().Y - 2))
                        yield return farmHouse.fridge.Value;
                    break;

                // buildings
                case BuildableGameLocationFacade l:
                    foreach (var building in l.buildings.Where(b => InRadius(radius, point, b.tileX.Value, b.tileY.Value)))
                    {
                        if (building is JunimoHut junimoHut)
                            yield return junimoHut.GetOutputChest();
                        else if (building is Building b)
                        {
                            if (!b.buildingChests.Any())
                                continue;
                            foreach (var chest in b.buildingChests)
                            {
                                yield return chest;
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Get the objects nearby point
        /// </summary>
        public static IEnumerable<T> GetNearbyObjects<T>(GameLocation location, Vector2 point, int radius) where T : StardewValley.Object
            => location.Objects.Pairs.Where(p => p.Value is T && (InRadius(radius, point, p.Key) || radius == -1)).Select(p => (T)p.Value);

        private static bool InRadius(int radius, Vector2 a, Vector2 b) => Math.Abs(a.X - b.X) < radius && Math.Abs(a.Y - b.Y) < radius;
        private static bool InRadius(int radius, Vector2 a, int x, int y) => Math.Abs(a.X - x) < radius && Math.Abs(a.Y - y) < radius;
    }

    /// <summary>
    /// Pair of chest and their location
    /// </summary>
    public class ChestLocationPair
    {
        public readonly Chest Chest;
        public readonly GameLocation Location;

        public ChestLocationPair(Chest chest, GameLocation location)
        {
            Chest = chest;
            Location = location;
        }
    }
}
