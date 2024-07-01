/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shivion/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using Force.DeepCloner;
using StardewValley.Menus;

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
                        filterItems.RemoveEmptySlots();
                        for (int j = filterItems.Count - 1; j >= 0; j--)
                        {
                            if(filterItems[j] != null && chestAboveItems[i] != null && filterItems[j].QualifiedItemId == chestAboveItems[i].QualifiedItemId && (!mod.Config.CompareQuality || filterItems[j].Quality == chestAboveItems[i].Quality))
                            {
                                if(mod.Config.CompareQuantity && chestAboveItems[i].TypeDefinitionId == ItemRegistry.type_object)
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
                                int amountToMove = filterCount;

                                //Calculate the amount to move to match the filter amount
                                foreach (var itemStack in outputChest[1].GetItemsForPlayer(inputChest.owner.Value))
                                {
                                    if (itemStack != null && itemStack.canStackWith(item))
                                    {
                                        //handle single items
                                        if (itemStack.Stack == 0)
                                        {
                                            amountToMove--;
                                        }
                                        amountToMove -= itemStack.Stack;
                                    }
                                }

                                //continue if the amount to move is already met
                                if (amountToMove < 1)
                                    continue;

                                //Make a new item stack
                                Item newItem = new StardewValley.Object(item.ItemId, item.Stack, item.IsRecipe, item.salePrice(), item.Quality);

                                //Limit the new stack size to the amount to Move
                                if (newItem.Stack > amountToMove)
                                {
                                    newItem.Stack = amountToMove;
                                }

                                //Attempt the addition
                                if (outputChest[1].addItem(newItem) == null)
                                {
                                    //clean up the old stack
                                    if (item.Stack == newItem.Stack)
                                    {
                                        chestAboveItems.RemoveAt(i);
                                    }
                                    //handle single items
                                    else if(newItem.Stack == 0)
                                    {
                                        item.Stack--;
                                    }
                                    //or just remove larger stacks
                                    else
                                    {
                                        item.Stack -= newItem.Stack;
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

        public class ChestLeftToRight : Comparer<Chest>
        {
            public override int Compare(Chest x, Chest y)
            {
                return x.TileLocation.X.CompareTo(y.TileLocation.X);
            }
        }
    }
}
