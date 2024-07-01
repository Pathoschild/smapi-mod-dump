/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using System;
using System.Collections.Generic;

using ItemBags.Bags;

using Leclair.Stardew.BetterCrafting;

using Microsoft.Xna.Framework;

using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.Network;

namespace ItemBags
{
    public class BetterCraftingInventoryProvider : IEventedInventoryProvider
    {
        // Split-screen stuff.
        internal readonly static PerScreen<Dictionary<ItemBag, NetMutex>> BagMutexes = new(() => new());
        internal readonly static PerScreen<Dictionary<ItemBag, ItemBagCraftingInventory>> BagInventories = new(() => new());

        internal static bool IsValid(object thing)
        {
            return thing is ItemBag && thing is not BundleBag;
        }


        [EventPriority(EventPriority.Low)]
        internal static void PopulateContainers(ISimplePopulateContainersEvent evt)
        {
            IBetterCrafting API = ItemBagsMod.ModInstance.BetterCraftingAPI;
            if (API == null)
                return;

            // We use a temporary list to hold bag instances to
            // avoid any weird concurrent modification stuff while
            // iterating evt.Containers.
            List<ItemBag> bags = new();

            // Just to note, we use BagMutexes to keep track of the bags
            // we're expecting to work with. We need to store them either
            // way, so it lets us avoid having an extra HashSet.

            // First, check the player's inventory for bags.
            foreach (var item in Game1.player.Items)
            {
                if (item is ItemBag bag && IsValid(bag))
                {
                    bags.Add(bag);
                    // No mutexes for the player's inventory.
                    BagMutexes.Value[bag] = null;
                }
            }

            // Iterate over the existing containers this menu is using.
            foreach (var container in evt.Containers)
            {
                object thing = container.Item1;
                GameLocation location = container.Item2;

                // We need to use the API to get a provider for this container
                // since it could be anything, in theory.
                var provider = API.GetProvider(thing);
                if (provider != null && provider.IsValid(thing, location, Game1.player))
                {
                    // If it's a valid container, then try to get a NetMutex for it.
                    // If we can, then save the NetMutex so that anyone using the
                    // bag will need to lock it to modify the bag. This will help
                    // minimize de-sync issues on multiplayer.
                    var mutex = provider.GetMutex(thing, location, Game1.player);
                    if (mutex != null || !provider.IsMutexRequired(thing, location, Game1.player))
                    {
                        // If the provider has a valid mutex, or doesn't require
                        // one, then we can check its items for any ItemBag instances
                        // that we should be crafting with.
                        var items = provider.GetItems(thing, location, Game1.player);
                        if (items != null)
                        {
                            foreach (var item in items)
                            {
                                if (item is ItemBag bag && IsValid(bag))
                                {
                                    bags.Add(bag);
                                    // Also store the mutex for later.
                                    BagMutexes.Value[bag] = mutex;
                                }
                            }
                        }
                    }
                }
            }

            // Now add all the bags to the menu.
            foreach (var item in bags)
                evt.Containers.Add(new(item, null));
        }

        internal static void MenuClosing(IClickableMenu menu)
        {
            // Just clear everything. The state should have been
            // re-synced already in the EndExclusive handler.
            BagInventories.Value.Clear();
            BagMutexes.Value.Clear();
        }

        internal static void ResyncBag(ItemBagCraftingInventory inventory)
        {
            if (inventory != null)
            {
                // Close the inventory + clean the bag.
                inventory.Close();

                if (inventory.Source is OmniBag OB)
                {
                    foreach (var nested in OB.NestedBags)
                        nested.Contents.RemoveAll(x => x == null || x.Stack <= 0);
                }
                else
                {
                    inventory.Source.Contents.RemoveAll(x => x == null || x.Stack <= 0);
                }

                // Re-sync the bag.
                inventory.Source.Resync();
            }
        }


        private static ItemBagCraftingInventory GetBagInventory(object item, bool createIfMissing = true)
        {
            // Make sure the item is a bag, and that we have a mutex (or null) for it.
            if (item is not ItemBag bag || !BagMutexes.Value.ContainsKey(bag))
                return null;

            if (BagInventories.Value.TryGetValue(bag, out ItemBagCraftingInventory inventory))
                return inventory;

            if (!createIfMissing)
                return null;

            inventory = new ItemBagCraftingInventory(bag);
            BagInventories.Value[bag] = inventory;
            return inventory;
        }

        public bool CanExtractItems(object obj, GameLocation location, Farmer who)
        {
            return IsValid(obj, location, who);
        }

        public bool CanInsertItems(object obj, GameLocation location, Farmer who)
        {
            return false;
        }

        public void CleanInventory(object obj, GameLocation location, Farmer who)
        {
            // Do nothing. Save our cleaning for when we're done.
        }

        public int GetActualCapacity(object obj, GameLocation location, Farmer who)
        {
            return GetBagInventory(obj)?.CountItemStacks() ?? 0;
        }

        public IInventory GetInventory(object obj, GameLocation location, Farmer who)
        {
            /// Cannot return the inventory because Better Crafting may
            /// access methods that are only stubs throwing exceptions.
            /// Notably: <see cref="IInventory.LastTickSlotChanged"/>
            return null;
        }

        public IList<Item> GetItems(object obj, GameLocation location, Farmer who)
        {
            return GetBagInventory(obj);
        }

        public Rectangle? GetMultiTileRegion(object obj, GameLocation location, Farmer who)
        {
            return null;
        }

        public NetMutex GetMutex(object obj, GameLocation location, Farmer who)
        {
            if (obj is ItemBag bag && BagMutexes.Value.ContainsKey(bag))
                return BagMutexes.Value[bag];
            return null;
        }

        public bool IsMutexRequired(object obj, GameLocation location, Farmer who)
        {
            if (obj is ItemBag bag && BagMutexes.Value.ContainsKey(bag))
                return BagMutexes.Value[bag] != null;
            return false;
        }

        public Vector2? GetTilePosition(object obj, GameLocation location, Farmer who)
        {
            return null;
        }

        public bool IsValid(object obj, GameLocation location, Farmer who)
        {
            return obj is ItemBag bag && IsValid(bag) && BagMutexes.Value.ContainsKey(bag);
        }

        public bool? StartExclusive(object obj, GameLocation location, Farmer who, IEventedInventoryProvider.StartExclusiveCallback callback)
        {
            // We don't need any startup behavior, only end behavior.
            // Just immediately return true.
            return true;
        }

        public void EndExclusive(object obj, GameLocation location, Farmer who)
        {
            // Clean up this bag's inventory, if it is a bag with an inventory.
            var inventory = GetBagInventory(obj, createIfMissing: false);
            if (inventory != null)
            {
                ResyncBag(inventory);

                // Remove this inventory from the cache so it is recreated
                // for the next use.
                BagInventories.Value.Remove(inventory.Source);
            }
        }
    }
}
