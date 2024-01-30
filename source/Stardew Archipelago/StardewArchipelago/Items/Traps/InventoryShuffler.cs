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

namespace StardewArchipelago.Items.Traps
{
    public class InventoryShuffler
    {
        private class ItemSlot
        {
            public IList<Item> Inventory { get; set; }
            public int SlotNumber { get; set; }

            public ItemSlot(IList<Item> inventory, int slotNumber)
            {
                Inventory = inventory;
                SlotNumber = slotNumber;
            }

            public void SetItem(Item item)
            {
                while (SlotNumber >= Inventory.Count)
                {
                    Inventory.Add(null);
                }
                Inventory[SlotNumber] = item;
            }
        }

        private const int CRAFTING_CATEGORY = -9;
        private const string CRAFTING_TYPE = "Crafting";
        private IMonitor _monitor;
        private GiftSender _giftSender;

        public InventoryShuffler(IMonitor monitor, GiftSender giftSender)
        {
            _monitor = monitor;
            _giftSender = giftSender;
        }

        public void ShuffleInventories(double rate, double rateAsGifts)
        {
            if (rate <= 0)
            {
                return;
            }

            var slotsToShuffle = new Dictionary<ItemSlot, Item>();

            _monitor.Log($"Executing a Shuffle Trap...", LogLevel.Debug);

            AddItemSlotsFromPlayerInventory(slotsToShuffle);
            AddItemSlotsFromChestsInEntireWorld(slotsToShuffle);
            AddItemSlotsFromFridges(slotsToShuffle);
            // AddItemSlotsFromJunimoChest(slotsToShuffle);

            var numberItemsToShuffle = slotsToShuffle.Count;
            _monitor.Log($"Found {numberItemsToShuffle} items to shuffle", LogLevel.Debug);
            var random = new Random((int)(Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed));

            if (rateAsGifts > 0)
            {
                SendRandomGifts(slotsToShuffle, random, rate * rateAsGifts);
                _monitor.Log($"Sent {numberItemsToShuffle - slotsToShuffle.Count} items as random gifts", LogLevel.Debug);
            }

            slotsToShuffle = slotsToShuffle.Where(x => random.NextDouble() < rate).ToDictionary(x => x.Key, x => x.Value);
            var allSlots = slotsToShuffle.Keys.ToList();
            var allItems = slotsToShuffle.Values.ToList();
            var allItemsShuffled = allItems.Shuffle(random);

            _monitor.Log($"Shuffling {slotsToShuffle.Count} items locally", LogLevel.Debug);

            for (var i = 0; i < allSlots.Count; i++)
            {
                allSlots[i].SetItem(allItemsShuffled[i]);
            }

            foreach (var chest in FindAllChests())
            {
                chest.clearNulls();
            }

            _monitor.Log($"Finished the shuffle trap!", LogLevel.Debug);
        }

        private void SendRandomGifts(Dictionary<ItemSlot, Item> slotsToShuffle, Random random, double giftingRate)
        {
            var giftsToSend = new Dictionary<string, List<Object>>();
            var slotsToClear = new Dictionary<Object, ItemSlot>();
            var validTargets = _giftSender.GetAllPlayersThatCanReceiveShuffledItems();

            if (validTargets == null || !validTargets.Any())
            {
                _monitor.Log($"Found no players to gift to", LogLevel.Debug);
                return;
            }

            _monitor.Log($"Found {validTargets.Count} players that can receive random gifts!", LogLevel.Debug);
            foreach (var (itemSlot, item) in slotsToShuffle)
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

            var failedToSendGifts = _giftSender.SendShuffleGifts(giftsToSend);

            _monitor.Log($"Finished sending gifts, {failedToSendGifts.Count} failed to send and will stay local", LogLevel.Debug);

            foreach (var (gift, slot) in slotsToClear)
            {
                if (failedToSendGifts.Contains(gift))
                {
                    continue;
                }

                slotsToShuffle.Remove(slot);
                slot.SetItem(null);
            }
        }

        private static void AddItemSlotsFromPlayerInventory(Dictionary<ItemSlot, Item> slotsToShuffle)
        {
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
                slotsToShuffle.Add(slot, item);
            }
        }

        private void AddItemSlotsFromChestsInEntireWorld(Dictionary<ItemSlot, Item> slotsToShuffle)
        {
            foreach (var chest in FindAllChests())
            {
                AddItemSlotsFromChest(slotsToShuffle, chest);
            }
        }

        private static IEnumerable<Chest> FindAllChests()
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

            foreach (var gameLocation in locations)
            {
                foreach (var (tile, gameObject) in gameLocation.Objects.Pairs)
                {
                    if (gameObject is not Chest { SpecialChestType: Chest.SpecialChestTypes.None or Chest.SpecialChestTypes.AutoLoader, Category: CRAFTING_CATEGORY, Type: CRAFTING_TYPE } chest || chest.giftbox.Value)
                    {
                        continue;
                    }

                    yield return chest;
                }
            }
        }

        private static void AddItemSlotsFromChest(Dictionary<ItemSlot, Item> slotsToShuffle, Chest chest)
        {
            var capacity = chest.GetActualCapacity();
            for (var i = 0; i < capacity; i++)
            {
                Item item = null;
                if (chest.items.Count > i && chest.items[i] != null)
                {
                    item = chest.items[i];
                }

                var slot = new ItemSlot(chest.items, i);
                slotsToShuffle.Add(slot, item);
            }
        }

        private void AddItemSlotsFromFridges(Dictionary<ItemSlot, Item> slotsToShuffle)
        {
            AddItemSlotsFromFridge(slotsToShuffle);
            AddItemSlotsFromIslandFridge(slotsToShuffle);
        }

        private void AddItemSlotsFromFridge(Dictionary<ItemSlot, Item> slotsToShuffle)
        {
            try
            {
                if (Game1.getLocationFromName("FarmHouse") is not FarmHouse location)
                {
                    return;
                }

                var fridge = location.fridge.Value;
                if (fridge == null || fridge.items.All(x => x == null))
                {
                    return;
                }

                AddItemSlotsFromChest(slotsToShuffle, fridge);
            }
            catch (Exception ex)
            {
                _monitor.Log("Could not find a fridge in the farmhouse", LogLevel.Warn);
            }
        }

        private void AddItemSlotsFromIslandFridge(Dictionary<ItemSlot, Item> slotsToShuffle)
        {
            try
            {
                if (Game1.getLocationFromName("IslandFarmHouse") is not IslandFarmHouse location)
                {
                    return;
                }

                var fridge = location.fridge.Value;
                if (fridge == null || fridge.items.All(x => x == null))
                {
                    return;
                }

                AddItemSlotsFromChest(slotsToShuffle, fridge);
            }
            catch (Exception ex)
            {
                _monitor.Log("Could not find a fridge in the island farmhouse", LogLevel.Warn);
            }
        }

        private void AddItemSlotsFromJunimoChest(Dictionary<ItemSlot, Item> slotsToShuffle)
        {
            try
            {
                var capacity = Game1.player.team.junimoChest.Count;
                for (var i = 0; i < capacity; i++)
                {
                    Item item = null;
                    if (Game1.player.team.junimoChest.Count > i && Game1.player.team.junimoChest[i] != null)
                    {
                        item = Game1.player.team.junimoChest[i];
                    }

                    var slot = new ItemSlot(Game1.player.team.junimoChest, i);
                    slotsToShuffle.Add(slot, item);
                }
            }
            catch (Exception ex)
            {
                _monitor.Log("Could not update the junimo chest properly", LogLevel.Warn);
            }
        }
    }
}
