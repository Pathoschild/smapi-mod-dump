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
using static HarmonyLib.Code;

namespace FilteredChestHopper
{
    internal class Pipeline
    {
        public List<Chest> Hoppers;
        //location
        internal GameLocation Location;

        public Pipeline(Chest originHopper)
        {
            Location = originHopper.Location;

            originHopper.modData[Mod.ModDataFlag] = "1";

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
        public void AttemptTransfer()
        {
            List<Chest> inputChests = new List<Chest>();
            List<Chest[]> outputChests = new List<Chest[]>();
            for (int i = 0; i < Hoppers.Count; i++)
            {
                Chest inputChest = Mod.GetChestAt(Location, Hoppers[i].TileLocation);
                if (inputChest != null)
                {
                    inputChests.Add(inputChest);
                }

                Chest outputChest = Mod.GetChestAt(Location, Hoppers[i].TileLocation);
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
                        for (int j = filterItems.Count - 1; j >= 0; j--)
                        {
                            if (filterItems[j].ItemId == chestAboveItems[i].ItemId)
                            {
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
                            if (Utility.addItemToThisInventoryList(item, outputChest[i].Items) == null)
                                chestAboveItems.RemoveAt(i);
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
