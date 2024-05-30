/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Inventories;
using System;
using System.Collections;
using System.Collections.Generic;
using Object = StardewValley.Object;

namespace ResourceStorage.BetterCrafting
{
    internal class ResourceStorageInventory : IInventory
    {
        protected List<Item> items;
        public readonly long OwnerId;
        public readonly Farmer Owner;

        public ResourceStorageInventory(Farmer player) 
        { 
            items = new List<Item>();
            Owner = player;
            OwnerId = player.UniqueMultiplayerID;

            ReloadFromFarmerResources();
        }

        public Item this[int index] { get => items[index]; set => items[index] = value; }

        public long LastTickSlotChanged { get; private set;}

        public int Count => items.Count;

        public bool IsReadOnly => false;

        public void Add(Item item)
        {
            if (!ModEntry.CanStore(ItemRegistry.Create<Object>(item.QualifiedItemId)))
                throw new ArgumentException($"The item {item.DisplayName} with id {item.QualifiedItemId} cannot be stored in the resource storage!");

            ModEntry.ModifyResourceLevel(Owner, item.QualifiedItemId, item.Stack, notifyBetterCraftingIntegration: false);
            items.Add(item);
        }

        public void AddRange(ICollection<Item> collection)
        {
            foreach(Item item in collection)
            {
                Add(item);
            }
        }

        public void Clean()
        {
            for(int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i] is null || items[i].Stack <= 0)
                {
                    items.RemoveAt(i);
                }
            }
        }

        public void Clear()
        {
            // Better Crafting has no reason to do this
            throw new InvalidOperationException("Do not try to clear the resource storage!");
        }

        public bool Contains(Item item)
        {
            return items.Contains(item);
        }

        public bool ContainsId(string itemId)
        {
            itemId = ItemRegistry.QualifyItemId(itemId);

            foreach (Item item in items)
            {
                if(item.QualifiedItemId == itemId)
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsId(string itemId, int minimum)
        {
            int amount = 0;
            itemId = ItemRegistry.QualifyItemId(itemId);

            foreach (Item item in items)
            {
                if (item.QualifiedItemId != itemId)
                    continue;

                amount += item.Stack;
                if(amount >= minimum)
                {
                    return true;
                }
            }

            return false;
        }

        public void CopyTo(Item[] array, int arrayIndex)
        {
            for(int i = 0; i < items.Count && arrayIndex + i < array.Length; i++)
            {
                array[arrayIndex+i] = items[i];
            }
        }

        public int CountId(string itemId)
        {
            int amount = 0;
            itemId = ItemRegistry.QualifyItemId(itemId);

            foreach (Item item in items)
            {
                if (item.QualifiedItemId == itemId)
                {
                    amount += item.Stack;
                }
            }

            return amount;
        }

        public int CountItemStacks()
        {
            return items.Count;
        }

        public IEnumerable<Item> GetById(string itemId)
        {
            itemId = ItemRegistry.QualifyItemId(itemId);
            List<Item> list = new List<Item>();

            foreach(Item item in items)
            {
                if(item.QualifiedItemId == itemId)
                {
                    list.Add(item);
                }
            }

            return list;
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public IList<Item> GetRange(int index, int count)
        {
            List<Item> list = new List<Item>();

            for(int i = 0; i < count; i++)
            {
                list.Add(items[index + i]);
            }

            return list;
        }

        public bool HasAny()
        {
            foreach (Item item in items)
            {
                if (item is not null)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasEmptySlots()
        {
            // The concept of slots does not exist in the resource storage. I return true under the assumption this is being used to tell if there is room to add items.
            return true;
        }

        public int IndexOf(Item item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, Item item)
        {
            if (!ModEntry.CanStore(ItemRegistry.Create<Object>(item.QualifiedItemId)))
                throw new ArgumentException($"The item {item.DisplayName} with id {item.QualifiedItemId} cannot be stored in the resource storage!");

            ModEntry.ModifyResourceLevel(Owner, item.QualifiedItemId, item.Stack, notifyBetterCraftingIntegration: false);
            items.Insert(index, item);
        }

        public void OverwriteWith(IList<Item> list)
        {
            // Better Crafting has no reason to do this
            throw new InvalidOperationException("Do not try to overwrite the resource storage!");
        }

        public int ReduceId(string itemId, int count)
        {
            int amountRemoved = ReduceIdWithoutNotifyingResourceStorage(itemId, count);

            ModEntry.ModifyResourceLevel(Owner, itemId, -amountRemoved, notifyBetterCraftingIntegration: false);

            return amountRemoved;
        }

        public int ReduceIdWithoutNotifyingResourceStorage(string itemId, int count)
        {
            int amountRemoved = 0;

            try
            {
                itemId = ItemRegistry.QualifyItemId(itemId);

                for (int i = items.Count - 1; i >= 0; i--)
                {
                    if (items[i].QualifiedItemId != itemId)
                        continue;

                    if (items[i].Stack > count - amountRemoved)
                    {
                        items[i].Stack -= count - amountRemoved;

                        amountRemoved = count;
                        break;
                    }
                    else
                    {
                        amountRemoved += items[i].Stack;
                        items.RemoveAt(i);

                        if (amountRemoved == count)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BetterCraftingIntegration.SMonitor.Log($"Failed in {nameof(ReduceIdWithoutNotifyingResourceStorage)}:\n{ex}", LogLevel.Error);
            }

            return amountRemoved;
        }

        public bool Remove(Item item)
        {
            for(int i = 0; i < items.Count; i++)
            {
                if (items[i] == item)
                {
                    ModEntry.ModifyResourceLevel(Owner, item.QualifiedItemId, -item.Stack, notifyBetterCraftingIntegration: false);
                    items.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            if (items[index] is Item item)
            {
                ModEntry.ModifyResourceLevel(Owner, item.QualifiedItemId, -item.Stack, notifyBetterCraftingIntegration: false);
            }

            items.RemoveAt(index);
        }

        public bool RemoveButKeepEmptySlot(Item item)
        {
            // The concept of empty slots is not relevant to the resource storage
            return Remove(item);
        }

        public void RemoveEmptySlots()
        {
            for(int i = items.Count-1; i >= 0; i--)
            {
                if (items[i] is Item item) // If there's an item with an empty stack, remove it
                {
                    if(item.Stack == 0)
                    {
                        items.RemoveAt(i);
                    }
                }
                else // If the value at the index is null, remove it
                {
                    items.RemoveAt(i);
                }
            }
        }

        public void RemoveRange(int index, int count)
        {
            for(int i = 0; i < count; i++)
            {
                RemoveAt(i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        /// <summary>
        /// Reset the items list and recreate it based on the current contents of the player's storage.
        /// </summary>
        public void ReloadFromFarmerResources()
        {
            try
            {
                BetterCraftingIntegration.SMonitor.Log("Reloading farmer resources for the BC integration!");
                items.Clear();
                foreach (var kvp in ModEntry.GetFarmerResources(Owner))
                {
                    items.AddRange(Utilities.CreateItemsWithTotalAmount(kvp.Key, kvp.Value));
                }

                LastTickSlotChanged = DateTime.UtcNow.Ticks;
            }
            catch (Exception ex)
            {
                BetterCraftingIntegration.SMonitor.Log($"Failed in {nameof(ReloadFromFarmerResources)}:\n{ex}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Takes all changes to the items in this inventory and sends them on to the resource storage.
        /// </summary>
        /// <param name="afterRecipe"></param>
        public void SquareWithFarmerResources(IRecipe afterRecipe = null)
        {
            try
            {
                // The differences between the amount of each item in this storage and the amount in the resource storage
                Dictionary<string, long> discrepencies = new();

                foreach (var kvp in ModEntry.GetFarmerResources(Owner))
                {
                    discrepencies.Add(kvp.Key, -kvp.Value);
                }

                foreach (Item item in items)
                {
                    if (item is null)
                    {
                        continue;
                    }

                    if (discrepencies.ContainsKey(item.QualifiedItemId))
                    {
                        discrepencies[item.QualifiedItemId] += item.Stack;
                    }
                    else
                    {
                        discrepencies[item.QualifiedItemId] = item.Stack;
                    }

                    if (discrepencies[item.QualifiedItemId] == 0)
                    {
                        discrepencies.Remove(item.QualifiedItemId);
                    }
                }

                if (discrepencies.Count == 0)
                {
                    BetterCraftingIntegration.SMonitor.Log("No discrepencies found after crafting!");
                }

                foreach (var kvp in discrepencies)
                {
                    BetterCraftingIntegration.SMonitor.Log("Discrepency found after crafting!");
                    ModEntry.ModifyResourceLevel(Owner, kvp.Key, (int)kvp.Value, notifyBetterCraftingIntegration: false);
                }
            }
            catch (Exception ex)
            {
                BetterCraftingIntegration.SMonitor.Log($"Failed in {nameof(SquareWithFarmerResources)}:\n{ex}", LogLevel.Error);
            }
        }

        public void NotifyOfChangeInResourceStorage(string itemId, int changeAmount)
        {
            try
            {
                if (changeAmount > 0)
                {
                    items.AddRange(Utilities.CreateItemsWithTotalAmount(itemId, changeAmount));
                }
                else
                {
                    ReduceIdWithoutNotifyingResourceStorage(itemId, -changeAmount);
                }
            }
            catch (Exception ex)
            {
                BetterCraftingIntegration.SMonitor.Log($"Failed in {nameof(NotifyOfChangeInResourceStorage)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}
