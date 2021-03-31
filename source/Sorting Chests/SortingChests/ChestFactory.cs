/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aRooooooba/SortingChests
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewModdingAPI.Utilities;
using StardewModdingAPI.Events;

namespace SortingChests
{
    class ChestFactory
    {
        private readonly IMultiplayerHelper multiplayer;
        public IDictionary<GameLocation, IDictionary<string, ItemChest>> contentDict;
        private IMonitor monitor;

        public ChestFactory(IMultiplayerHelper multiplayer, IMonitor monitor)
        {
            this.multiplayer = multiplayer;
            contentDict = new Dictionary<GameLocation, IDictionary<string, ItemChest>>();
            this.monitor = monitor;
        }

        private IEnumerable<GameLocation> GetAccessibleLocations()
        {
            if (Context.IsMainPlayer)
            {
                var locations = Game1.locations
                    .Concat
                    (
                        from location in Game1.locations.OfType<BuildableGameLocation>()
                        from building in location.buildings
                        where building.indoors.Value != null
                        select building.indoors.Value
                    );
                return locations;
            }
            return multiplayer.GetActiveLocations();
        }

        private IEnumerable<Chest> GetChests(GameLocation location)
        {
            IEnumerable<Chest> Search()
            {
                foreach (KeyValuePair<Vector2, StardewValley.Object> pair in location.Objects.Pairs)
                {
                    if (pair.Value is Chest chest && chest.playerChest.Value && chest.SpecialChestType == Chest.SpecialChestTypes.None)
                    {
                        yield return chest;
                    }
                }
            }
            return Search();
        }

        /// <summary>
        /// Sort the chests in the given location.
        /// </summary>
        /// <param name="location">The location where the chests to be sorted resides.</param>
        /// <returns>The number of chests operation.</returns>
        public int SortChests(GameLocation location)
        {
            int chestOperations = 0;
            if (!contentDict.ContainsKey(location))
                contentDict.Add(location, new Dictionary<string, ItemChest>());
            IDictionary<string, ItemChest> curContent = contentDict[location];
            foreach (Chest sourceChest in GetChests(location))
            {
                chestOperations += AddItems(curContent, sourceChest, sourceChest.GetItemsForPlayer(Game1.player.UniqueMultiplayerID));
            }
            return chestOperations;
        }

        public int SortChestsInAllLocations()
        {
            int chestOperations = 0;
            foreach (GameLocation location in GetAccessibleLocations())
                chestOperations += SortChests(location);
            return chestOperations;
        }

        public void DeleteNullItems(IList<Item> toBeDeleted, Chest sourceChest)
        {
            foreach (Item item in toBeDeleted)
            {
                item.Stack = 0;
                sourceChest.grabItemFromChest(item, Game1.player);
            }
        }

        public int AddItems(IDictionary<string, ItemChest> curContent, Chest sourceChest, IEnumerable<Item> toBeAdded)
        {
            IList<Item> toBeDeleted = new List<Item>();
            int chestOperations = 0;
            foreach (Item newItem in toBeAdded)
            {
                if (newItem.Stack == newItem.maximumStackSize())
                    continue;
                if (curContent.ContainsKey(newItem.Name) && curContent[newItem.Name].Item.Stack == newItem.maximumStackSize())
                    curContent.Remove(newItem.Name);
                if (!curContent.ContainsKey(newItem.Name))
                {
                    curContent.Add(newItem.Name, new ItemChest(newItem, sourceChest));
                    continue;
                }
                Item oldItem = curContent[newItem.Name].Item;
                Chest targetChest = curContent[newItem.Name].Chest;
                if (targetChest == sourceChest)
                    continue;
                chestOperations += 2;
                if (oldItem.Stack + newItem.Stack > oldItem.maximumStackSize())
                {
                    newItem.Stack -= oldItem.maximumStackSize() - oldItem.Stack;
                    oldItem.Stack = oldItem.maximumStackSize();
                    curContent[newItem.Name] = new ItemChest(newItem, sourceChest);
                }
                else
                {
                    oldItem.Stack += newItem.Stack;
                    toBeDeleted.Add(newItem);
                    if (oldItem.Stack == oldItem.maximumStackSize())
                        curContent.Remove(newItem.Name);
                }
            }
            DeleteNullItems(toBeDeleted, sourceChest);
            return chestOperations;
        }

        public void RemoveItems(IDictionary<string, ItemChest> curContent, IEnumerable<Item> toBeRemoved)
        {
            foreach (Item newItem in toBeRemoved)
            {
                if (newItem.Stack == newItem.maximumStackSize() || !curContent.ContainsKey(newItem.Name))
                    continue;
                curContent.Remove(newItem.Name);
            }
        }

        public int ChangeItemsQuantities(IDictionary<string, ItemChest> curContent, Chest sourceChest, IEnumerable<ItemStackSizeChange> toBeChanged)
        {
            IList<Item> toBeAdded = new List<Item>();
            foreach (ItemStackSizeChange it in toBeChanged)
            {
                if (it.NewSize == it.Item.maximumStackSize())
                    curContent.Remove(it.Item.Name);
                else
                    toBeAdded.Add(it.Item);
            }
            return AddItems(curContent, sourceChest, toBeAdded);
        }

        public int UpdateContent(Chest sourceChest, GameLocation location, IEnumerable<Item> added, IEnumerable<Item> removed, IEnumerable<ItemStackSizeChange> quantityChanged)
        {
            AddedRemoved addedRemoved = new AddedRemoved(added, removed);
            int chestOperations = 0;
            if (!contentDict.ContainsKey(location))
                contentDict.Add(location, new Dictionary<string, ItemChest>());
            IDictionary<string, ItemChest> curContent = contentDict[location];
            string keys = "";
            foreach (string name in curContent.Keys)
                keys += name + " ";
            // monitor.Log(keys, LogLevel.Debug);
            chestOperations += AddItems(curContent, sourceChest, addedRemoved.Added);
            RemoveItems(curContent, addedRemoved.Removed);
            chestOperations += ChangeItemsQuantities(curContent, sourceChest, quantityChanged);
            return chestOperations;
        }
    }
}
