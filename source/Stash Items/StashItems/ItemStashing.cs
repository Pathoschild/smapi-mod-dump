/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zack-hill/stardew-valley-stash-items
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace StashItems
{
    internal static class ItemStashing
	{
		internal static void StashItemsInNearbyChests(int radius)
		{
			var player = Game1.player;
			var stashedAny = false;

            foreach (var chest in GetChestsAroundCharacter(player, radius))
            {
				// - Player items are processed in reverse order to improve placement of gaps in inventory when
				// only able to partially transfer items.
				// - Player items are ToListed so that we can modify the player's inventory while iterating over it.
                var farmerItems = player.Items.Reverse().ToList();
                foreach (var playerItem in farmerItems)
                {
                    if (playerItem == null)
                        continue;

                    var foundMatch = false;

                    foreach (var chestItem in chest.Items)
                    {
                        if (chestItem == null)
                            continue;

                        if (!playerItem.canStackWith(chestItem))
                            continue;

                        foundMatch = true;

						// Merge with existing stack
                        if (chestItem.getRemainingStackSpace() > 0)
						{
							playerItem.Stack = chestItem.addToStack(playerItem);
                            stashedAny = true;
						}
					}

                    if (playerItem.Stack == 0)
                    {
                        player.removeItemFromInventory(playerItem);
                        continue;
                    }

					// Transfer remaining stack to open slot in chest
                    if (foundMatch && chest.Items.Count < chest.GetActualCapacity())
					{
                        chest.addItem(playerItem);
                        player.removeItemFromInventory(playerItem);
                        stashedAny = true;
					}
                }
            }

            Game1.playSound(stashedAny
				? Game1.soundBank.GetCue("pickUpItem").Name
				: Game1.soundBank.GetCue("breathout").Name);
		}

		private static IEnumerable<Chest> GetChestsAroundCharacter(Character character, int radius)
		{
			// Chests
			for (var dx = -radius; dx <= radius; dx++)
			{
				for (var dy = -radius; dy <= radius; dy++)
				{
					var checkLocation = character.TilePoint + new Point(dx, dy);
					var objectAtTile = character.currentLocation.getObjectAtTile(checkLocation.X, checkLocation.Y);
                    if (objectAtTile is Chest chest)
					{
						yield return chest;
					}
				}
			}

			// Fridge
			if (character.currentLocation is FarmHouse farmHouse && farmHouse.upgradeLevel >= 1) //  Check upgrade level to make sure player has fridge
            {
                var fridgeIsWithinRange = Math.Abs(character.Tile.X - farmHouse.fridgePosition.X) <= radius &&
                                          Math.Abs(character.Tile.Y - farmHouse.fridgePosition.Y) <= radius;
                if (fridgeIsWithinRange && farmHouse.fridge.Value != null)
				{
                    yield return farmHouse.fridge.Value;
				}
			}
		}
	} 
}
