/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shivion/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Objects;
namespace FilteredChestHopper
{
    internal class Pipeline
    {
        public List<Chest> Hoppers = new List<Chest>();
        //location
        internal GameLocation Location;

        public Pipeline(Chest originHopper)
        {
            Location = originHopper.Location;

            originHopper.modData[Mod.ModDataFlag] = "1";

            Hoppers.Add(originHopper);

            CheckSideHoppers(new Vector2(1, 0), originHopper);
            CheckSideHoppers(new Vector2(-1, 0), originHopper);

            Hoppers.Sort(new ChestLeftToRight());
        }

        //Checks adjacent hoppers for expansion
        private void CheckSideHoppers(Vector2 direction, Chest hopper)
        {
            //check for hopper in direction
            Chest chest = Mod.GetChestAt(Location, hopper.TileLocation + direction);
            if (chest == null || !Mod.TryGetHopper(chest, out hopper))
            {
                return;
            }

            ExpandPipeline(hopper);

            CheckSideHoppers(direction, hopper);
        }

        internal void ExpandPipeline(Chest hopper)
        {
            //Expand Pipeline
            Hoppers.Add(hopper);
            hopper.modData[Mod.ModDataFlag] = "1";
        }

        //Attempt to output with this hopper as a filter
        public void AttemptTransfer(Mod mod)
        {
            List<Chest> inputChests = new List<Chest>();
            List<Chest[]> outputChests = new List<Chest[]>();
            for (int i = 0; i < Hoppers.Count; i++)
            {
                Chest inputChest = Mod.GetChestAt(Location, Hoppers[i].TileLocation - new Vector2(0,1));
                if (inputChest != null)
                {
                    inputChests.Add(inputChest);
                }

                Chest outputChest = Mod.GetChestAt(Location, Hoppers[i].TileLocation + new Vector2(0, 1));
                if (outputChest != null)
                {
                    outputChests.Add(new Chest[] { Hoppers[i], outputChest});
                }
            }

            foreach (var inputChest in inputChests)
            {
                inputChest.clearNulls();
                var chestAboveItems = inputChest.GetItemsForPlayer(inputChest.owner.Value);
                foreach (var outputChest in outputChests)
                {
                    var filterItems = outputChest[0].GetItemsForPlayer(inputChest.owner.Value);
                    for (int i = chestAboveItems.Count - 1; i >= 0; i--)
                    {
                        bool match = true;
                        int filterCount = 0;
                        for (int j = filterItems.Count - 1; j >= 0; j--)
                        {
                            if(filterItems[j] != null && chestAboveItems[i] != null && filterItems[j].ItemId == chestAboveItems[i].ItemId && (!mod.Config.CompareQuality || filterItems[j].Quality == chestAboveItems[i].Quality))
                            {
                                if(mod.Config.CompareQuantity)
                                {
                                    filterCount = filterItems[j].Stack == 1 ? 0 : filterItems[j].Stack;
                                }
                                match = true;
                                break;
                            }
                            else
                            {
                                match = false;
                            }
                        }
                        if (match)
                        {
                            Item item = chestAboveItems[i];

                            if(filterCount > 0)
                            {
                                bool hasStack = false;
                                //Check for an existing stack
                                foreach (var itemStack in outputChest[1].GetItemsForPlayer(inputChest.owner.Value))
                                {
                                    if (itemStack.canStackWith(item))
                                    {
                                        hasStack = true;
                                        int amountToMove = filterCount - itemStack.Stack;
                                        if (amountToMove > filterCount)
                                        {
                                            amountToMove = filterCount;
                                        }
                                        if (amountToMove < 0)
                                        {
                                            amountToMove = 0;
                                        }
                                        itemStack.Stack += amountToMove;
                                        if (amountToMove > 0)
                                        {
                                            item.Stack -= amountToMove;
                                        }
                                    }
                                }
                                //or make a new one
                                if (!hasStack)
                                {
                                    Item newItem = new StardewValley.Object(item.ItemId, item.Stack, item.IsRecipe, item.salePrice(), item.Quality);
                                    if (newItem.Stack > filterCount)
                                    {
                                        newItem.Stack = filterCount;
                                    }
                                    if (outputChest[1].addItem(newItem) == null)
                                    {
                                        if (item.Stack == newItem.Stack)
                                        {
                                            chestAboveItems.RemoveAt(i);
                                        }
                                        else
                                        {
                                            item.Stack -= newItem.Stack;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //The Easy Way
                                if (outputChest[1].addItem(item) == null)
                                {
                                    chestAboveItems.RemoveAt(i);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RemoveFromStack(IInventory chest, int index, int amount)
        {
            if(chest[index].Stack <= amount)
            {
                chest.RemoveAt(index);
            }
            else
            {
                chest[index].Stack -= amount;
            }
        }

        public class ChestLeftToRight : Comparer<Chest>
        {
            public override int Compare(Chest x, Chest y)
            {
                return x.TileLocation.X.CompareTo(y.TileLocation.X);
            }
        }
    }
}
