/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/doncollins/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;

namespace StardewValleyMods.CategorizeChests.Framework
{
    class ChestFiller : IChestFiller
    {
        private readonly IChestDataManager ChestDataManager;
        private readonly IMonitor Monitor;

        public ChestFiller(IChestDataManager chestDataManager, IMonitor monitor)
        {
            ChestDataManager = chestDataManager;
            Monitor = monitor;
        }

        /// <summary>
        /// Attempt to move as much as possible of the player's inventory
        /// into the given chest, subject to the chest's capacity and its
        /// configured list of items to accept.
        /// </summary>
        /// <param name="chest">The chest to put the items in.</param>
        public void DumpItemsToChest(Chest chest)
        {
            var chestData = ChestDataManager.GetChestData(chest);

            bool shouldPlaySound = false;
            foreach (var item in GetInventoryItems())
            {
                try
                {
                    if (chestData.Accepts(item))
                    {
                        bool movedSome = TryPutItemInChest(chest, item);
                        if (movedSome) shouldPlaySound = true;
                    }
                }
                catch (ItemNotImplementedException)
                {
                    // it's fine, just skip it
                }
            }

            if (shouldPlaySound)
                Game1.playSound("dwop");
        }

        /// <summary>
        /// Attempt to move as much as possible of the given item stack into the chest.
        /// </summary>
        /// <param name="chest">The chest to put the items in.</param>
        /// <param name="item">The items to put in the chest.</param>
        /// <returns>True if at least some of the stack was moved into the chest.</returns>
        private bool TryPutItemInChest(Chest chest, Item item)
        {
            // we'll use this to track whether at least one thing was successfully
            // put in the chest
            bool movedSome = false;

            // Items in the chest that can stack with the given item.
            var candidates = chest.items
                .Where(i => i != null)
                .Where(i => i.canStackWith(item));

            foreach (var recipient in candidates)
            {
                var spaceLeft = recipient.maximumStackSize() - recipient.Stack;

                if (spaceLeft >= item.Stack)
                {
                    // we can put all of it in this stack and we're done
                    chest.grabItemFromInventory(item, Game1.player);
                    return true;
                }
                else if (spaceLeft > 0)
                {
                    // Move what we can onto the chest stack.
                    item.Stack -= spaceLeft; //TODO: is there a better way to do this?
                    recipient.addToStack(spaceLeft);
                    movedSome = true;
                }
            }

            // if we got here, we should still have some left to put in

            if (ChestHasEmptySpaces(chest))
            {
                // there's an empty space, so just put it there
                chest.grabItemFromInventory(item, Game1.player);
                return true;
            }

            return movedSome;
        }

        /// <summary>
        /// Check whether the given chest has any completely empty slots.
        /// </summary>
        /// <returns>Whether at least one slot is empty.</returns>
        /// <param name="chest">The chest to check.</param>
        private bool ChestHasEmptySpaces(Chest chest)
        {
            return chest.items.Count < Chest.capacity
                   || chest.items.Any(i => i == null);
        }

        /// <summary>
        /// Return a list of the items in the player's inventory.
        /// </summary>
        private IEnumerable<Item> GetInventoryItems()
        {
            return Game1.player.Items.Where(i => i != null).ToList();
        }
    }
}