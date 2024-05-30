/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JonathanFeenstra/Auto-StackBait
**
*************************************************/

/* Auto-Stack Bait
 *
 * SMAPI mod that automatically adds any new bait to fishing poles that
 * have the same type of bait attached.
 * 
 * Copyright (C) 2024 Jonathan Feenstra
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Tools;

namespace AutoStackBait;

internal sealed class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        helper.Events.Player.InventoryChanged += OnInventoryChanged;
        helper.Events.World.ChestInventoryChanged += OnChestInventoryChanged;
    }

    private static void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        foreach (var addedItem in e.Added)
        {
            if (addedItem.Category == StardewValley.Object.baitCategory)
            {
                AddBaitToRods(Game1.player.Items, addedItem);
            }
        }
    }
    
    private static void OnChestInventoryChanged(object? sender, ChestInventoryChangedEventArgs e)
    {
        foreach (var addedItem in e.Added)
        {
            if (addedItem.Category == StardewValley.Object.baitCategory)
            {
                AddBaitToRods(e.Chest.Items, addedItem);
            }
        }
    }

    private static void AddBaitToRods(Inventory inventory, Item newBait)
    {
        foreach (var inventoryItem in inventory)
        {
            if (inventoryItem is not FishingRod rod) continue;
            var currentBait = rod.GetBait();
            if (currentBait is null || !AreEqual(currentBait, newBait)) continue;
            var newStackSize = currentBait.Stack + newBait.Stack;
            var maxSize = currentBait.maximumStackSize();
            if (newStackSize > maxSize)
            {
                currentBait.Stack = maxSize;
                newBait.Stack = newStackSize - maxSize;
            }
            else
            {
                currentBait.Stack = newStackSize;
                inventory.RemoveButKeepEmptySlot(newBait);
                return;
            }
        }
    }

    private static bool AreEqual(ISalable item1, ISalable item2) =>
        item1.QualifiedItemId == item2.QualifiedItemId && item1.Name == item2.Name;
}