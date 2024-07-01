/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewArchipelago.Archipelago.Gifting;
using StardewArchipelago.Extensions;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace StardewArchipelago.Items.Traps.Shuffle
{
    public class InventoryShuffler
    {
        private const int CRAFTING_CATEGORY = -9;
        private const string CRAFTING_TYPE = "Crafting";
        private IMonitor _monitor;
        private GiftSender _giftSender;

        public InventoryShuffler(IMonitor monitor, GiftSender giftSender)
        {
            _monitor = monitor;
            _giftSender = giftSender;
        }

        public void ShuffleInventories(ShuffleTarget targets, double rate, double rateAsGifts)
        {
            if (targets == ShuffleTarget.None || rate <= 0)
            {
                return;
            }

            var groupsToShuffle = new InventoryCollection();

            _monitor.Log($"Executing a Shuffle Trap...", LogLevel.Debug);

            groupsToShuffle.Add(GetItemSlotsFromPlayerInventory());
            groupsToShuffle.AddRange(GetItemSlotsFromChestsInEntireWorld());
            groupsToShuffle.AddRange(GetItemSlotsFromFridges());
            // GetItemSlotsFromJunimoChest(slotsToShuffle);
            groupsToShuffle.RemoveInvalidInventories();

            var numberInventoriesToShuffle = groupsToShuffle.Count;
            var numberItemsToShuffle = groupsToShuffle.Sum(x => x.Content.Count);
            _monitor.Log($"Found {numberInventoriesToShuffle} inventoryGroups to shuffle, containing a total of {numberItemsToShuffle} items", LogLevel.Debug);

            var mergedGroups = MergeGroups(groupsToShuffle, targets);
            _monitor.Log($"Separated inventoryGroups in {mergedGroups.Count} groups to shuffle among themselves", LogLevel.Debug);

            var random = new Random((int)(Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed));

            if (rateAsGifts > 0)
            {
                var numberItemsSent = SendRandomGifts(mergedGroups, random, rate * rateAsGifts);
                _monitor.Log($"Sent {numberItemsSent} items as random gifts", LogLevel.Debug);
            }

            foreach (var groupToShuffle in mergedGroups)
            {
                var slotsToShuffle = groupToShuffle.SelectMany(x => x.Content.Content).ToDictionary(x => x.Key, x => x.Value);
                _monitor.Log($"Shuffling a group of {groupToShuffle.Inventories.Count} inventoryGroups on map {groupToShuffle.Inventories.First().Info.Map} with {slotsToShuffle.Count} items...", LogLevel.Debug);
                slotsToShuffle = slotsToShuffle.Where(x => random.NextDouble() < rate).ToDictionary(x => x.Key, x => x.Value);
                var allSlots = slotsToShuffle.Keys.ToList();
                var allItems = slotsToShuffle.Values.ToList();
                var allItemsShuffled = allItems.Shuffle(random);

                _monitor.Log($"Shuffling {slotsToShuffle.Count} items locally", LogLevel.Debug);

                for (var i = 0; i < allSlots.Count; i++)
                {
                    allSlots[i].SetItem(allItemsShuffled[i]);
                }
            }

            foreach (var chest in FindAllChests().Values)
            {
                chest.clearNulls();
            }

            _monitor.Log($"Finished the shuffle trap!", LogLevel.Debug);
        }

        private GroupedInventoryCollection MergeGroups(InventoryCollection groupsToShuffle, ShuffleTarget targets)
        {
            var groupedInventories = new GroupedInventoryCollection(groupsToShuffle);
            if (targets <= ShuffleTarget.Self)
            {
                return groupedInventories;
            }

            while (TryGetOneMergeCandidate(targets, groupedInventories, out var mergeIndex1, out var mergeIndex2))
            {
                MergeGroups(groupedInventories, mergeIndex1, mergeIndex2);
            }

            return groupedInventories;
        }

        private static bool TryGetOneMergeCandidate(ShuffleTarget targets, GroupedInventoryCollection groupedInventories, out int mergeIndex1, out int mergeIndex2)
        {
            for (var i = 0; i < groupedInventories.Count - 1; i++)
            {
                var group1 = groupedInventories[i];
                foreach (var inventory1 in group1)
                {
                    var info1 = inventory1.Info;
                    for (var j = i + 1; j < groupedInventories.Count; j++)
                    {
                        var group2 = groupedInventories[j];

                        foreach (var inventory2 in group2)
                        {
                            var info2 = inventory2.Info;
                            if (targets < ShuffleTarget.AllMaps && info1.Map != info2.Map)
                            {
                                continue;
                            }

                            var distance = Math.Abs(info1.Tile.X - info2.Tile.X) + Math.Abs(info1.Tile.Y - info2.Tile.Y);
                            if (distance > (int)targets)
                            {
                                continue;
                            }

                            mergeIndex1 = i;
                            mergeIndex2 = j;
                            return true;
                        }
                    }
                }
            }

            mergeIndex1 = -1;
            mergeIndex2 = -1;
            return false;
        }

        private void MergeGroups(GroupedInventoryCollection groupedInventories, int mergeIndex1, int mergeIndex2)
        {
            if (mergeIndex1 == mergeIndex2 || mergeIndex1 < 0 || mergeIndex2 < 0 || mergeIndex1 >= groupedInventories.Count || mergeIndex2 >= groupedInventories.Count)
            {
                return;
            }

            var inventoryGroup1 = groupedInventories[mergeIndex1];
            var inventoryGroup2 = groupedInventories[mergeIndex2];

            groupedInventories.Remove(mergeIndex1, mergeIndex2);

            var mergedInventory = inventoryGroup1.GetMergedWith(inventoryGroup2);
            groupedInventories.Add(mergedInventory);
        }

        private int SendRandomGifts(GroupedInventoryCollection inventoryGroups, Random random, double giftingRate)
        {
            var validTargets = _giftSender.GetAllPlayersThatCanReceiveShuffledItems();

            if (validTargets == null || !validTargets.Any())
            {
                _monitor.Log($"Found no players to gift to", LogLevel.Debug);
                return 0;
            }

            _monitor.Log($"Found {validTargets.Count} players that can receive random gifts!", LogLevel.Debug);
            var giftsToSend = SelectGiftingTargets(inventoryGroups, random, giftingRate, validTargets, out var slotsToClear);

            var failedToSendGifts = _giftSender.SendShuffleGifts(giftsToSend);
            if (failedToSendGifts.Any())
            {
                _monitor.Log($"Finished sending gifts, {failedToSendGifts.Count} failed to send and will stay local", LogLevel.Debug);
            }
            else
            {
                _monitor.Log($"Finished sending gifts", LogLevel.Debug);
            }

            RemoveGiftedItemsFromPoolToShuffle(inventoryGroups, slotsToClear, failedToSendGifts);
            return giftsToSend.Sum(x => x.Value.Count) - failedToSendGifts.Count;
        }

        private Dictionary<string, List<Object>> SelectGiftingTargets(GroupedInventoryCollection inventoryGroups, Random random, double giftingRate, List<string> validTargets, out Dictionary<Object, ItemSlot> slotsToClear)
        {
            var giftsToSend = new Dictionary<string, List<Object>>();
            slotsToClear = new Dictionary<Object, ItemSlot>();
            foreach (var inventoryGroup in inventoryGroups)
            {
                foreach (var inventory in inventoryGroup)
                {
                    foreach (var (itemSlot, item) in inventory.Content)
                    {
                        if (item is not Object objectToGift || !_giftSender.CanGiftObject(objectToGift) || random.NextDouble() > giftingRate)
                        {
                            continue;
                        }

                        var chosenTarget = validTargets[random.Next(validTargets.Count)];
                        if (!giftsToSend.ContainsKey(chosenTarget))
                        {
                            giftsToSend.Add(chosenTarget, new List<Object>());
                        }

                        _monitor.Log($"{chosenTarget} has been chosen as a recipient for {objectToGift.Stack} {objectToGift.Name}", LogLevel.Trace);
                        giftsToSend[chosenTarget].Add(objectToGift);
                        slotsToClear.Add(objectToGift, itemSlot);
                    }
                }
            }

            return giftsToSend;
        }

        private static void RemoveGiftedItemsFromPoolToShuffle(GroupedInventoryCollection inventoryGroups, Dictionary<Object, ItemSlot> slotsToClear, List<Object> failedToSendGifts)
        {
            foreach (var (gift, slot) in slotsToClear)
            {
                RemoveGiftedItemFromPoolToShuffle(inventoryGroups, failedToSendGifts, gift, slot);
            }
        }

        private static void RemoveGiftedItemFromPoolToShuffle(GroupedInventoryCollection inventoryGroups, List<Object> failedToSendGifts, Object gift, ItemSlot slot)
        {
            if (failedToSendGifts.Contains(gift))
            {
                return;
            }

            foreach (var inventoryGroup in inventoryGroups)
            {
                foreach (var inventory in inventoryGroup)
                {
                    if (inventory.Content.ContainsKey(slot))
                    {
                        inventory.Content.Remove(slot);
                        slot.SetItem(null);
                        return;
                    }
                }
            }

            slot.SetItem(null);
        }

        private static Inventory GetItemSlotsFromPlayerInventory()
        {
            var inventorySlots = new InventoryContent();
            var player = Game1.player;
            for (var i = 0; i < player.MaxItems; i++)
            {
                Item item = null;
                if (player.Items.Count > i && player.Items[i] != null)
                {
                    item = player.Items[i];
                }

                if (item is FishingRod rod && (rod.isReeling || rod.isFishing || rod.pullingOutOfWater))
                {
                    continue;
                }

                var slot = new ItemSlot(player.Items, i);
                inventorySlots.Add(slot, item);
            }

            var inventoryInfo = new InventoryInfo(player.currentLocation.Name, player.Tile, player.Items, player.MaxItems);
            return new Inventory(inventoryInfo, inventorySlots);
        }

        private IEnumerable<Inventory> GetItemSlotsFromChestsInEntireWorld()
        {
            foreach (var chestInfo in FindAllChests().Keys)
            {
                var itemSlots = GetItemSlotsFromChest(chestInfo);
                yield return new Inventory(chestInfo, itemSlots);
            }
        }

        private static Dictionary<InventoryInfo, Chest> FindAllChests()
        {
            var locations = Game1.locations.ToList();
            foreach (var building in Game1.getFarm().buildings)
            {
                if (building?.indoors.Value == null)
                {
                    continue;
                }
                locations.Add(building.indoors.Value);
            }

            var allChests = new Dictionary<InventoryInfo, Chest>();

            foreach (var gameLocation in locations)
            {
                foreach (var (tile, gameObject) in gameLocation.Objects.Pairs)
                {
                    if (gameObject is not Chest { SpecialChestType: Chest.SpecialChestTypes.None or Chest.SpecialChestTypes.AutoLoader, Category: CRAFTING_CATEGORY, Type: CRAFTING_TYPE } chest || chest.giftbox.Value)
                    {
                        continue;
                    }

                    allChests.Add(new InventoryInfo(gameLocation.Name, tile, chest.Items, chest.GetActualCapacity()), chest);
                }
            }

            return allChests;
        }

        private static InventoryContent GetItemSlotsFromChest(InventoryInfo chestInfo)
        {
            var inventorySlots = new InventoryContent();
            var capacity = chestInfo.Capacity;
            for (var i = 0; i < capacity; i++)
            {
                Item item = null;
                if (chestInfo.Content.Count > i && chestInfo.Content[i] != null)
                {
                    item = chestInfo.Content[i];
                }

                var slot = new ItemSlot(chestInfo.Content, i);
                inventorySlots.Add(slot, item);
            }

            return inventorySlots;
        }

        private IEnumerable<Inventory> GetItemSlotsFromFridges()
        {
            var houseFridge = GetItemSlotsFromFridge();
            var islandFridge = GetItemSlotsFromIslandFridge();
            yield return houseFridge;
            yield return islandFridge;
        }

        private Inventory GetItemSlotsFromFridge()
        {
            try
            {
                if (Game1.getLocationFromName("FarmHouse") is not FarmHouse location)
                {
                    return null;
                }

                var fridge = location.fridge.Value;
                if (fridge == null || fridge.Items.All(x => x == null))
                {
                    return null;
                }

                var info = new InventoryInfo(location.Name, fridge.TileLocation, fridge.Items, fridge.GetActualCapacity());
                var fridgeSlots = GetItemSlotsFromChest(info);
                return new Inventory(info, fridgeSlots);
            }
            catch (Exception ex)
            {
                _monitor.Log("Could not find a fridge in the farmhouse", LogLevel.Warn);
                return null;
            }
        }

        private Inventory GetItemSlotsFromIslandFridge()
        {
            try
            {
                if (Game1.getLocationFromName("IslandFarmHouse") is not IslandFarmHouse location)
                {
                    return null;
                }

                var fridge = location.fridge.Value;
                if (fridge == null || fridge.Items.All(x => x == null))
                {
                    return null;
                }

                var info = new InventoryInfo(location.Name, fridge.TileLocation, fridge.Items, fridge.GetActualCapacity());
                var fridgeSlots = GetItemSlotsFromChest(info);
                return new Inventory(info, fridgeSlots);
            }
            catch (Exception ex)
            {
                _monitor.Log("Could not find a fridge in the island farmhouse", LogLevel.Warn);
                return null;
            }
        }
    }
}
