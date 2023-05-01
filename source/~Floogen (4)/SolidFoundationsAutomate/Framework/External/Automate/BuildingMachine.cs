/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using SolidFoundations.Framework.Models.Backport;
using SolidFoundations.Framework.Models.ContentPack;
using SolidFoundations.Framework.Utilities.Backport;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidFoundationsAutomate.Framework.External.Automate
{
    public class BuildingMachine : IMachine
    {
        public GameLocation Location { get; }
        public Rectangle TileArea { get; }
        public string MachineTypeID { get; }

        private GenericBuilding Building { get; }
        private ExtendedBuildingModel Model { get; }
        private Chest InputChest { get; }
        private Chest OutputChest { get; }

        public BuildingMachine(GenericBuilding customBuilding, BuildableGameLocation location, Vector2 tile)
        {
            Location = location;
            TileArea = new Rectangle((int)tile.X, (int)tile.Y, customBuilding.tilesWide.Value, customBuilding.tilesHigh.Value);

            MachineTypeID = customBuilding.Id;
            Building = customBuilding;
            Model = customBuilding.Model;

            if (Model.Chests is not null)
            {
                // Get the input chest
                if (Model.Chests.FirstOrDefault(c => c.Type == BuildingChest.ChestType.Load) is var loadChestData && loadChestData is not null && Building.GetBuildingChest(loadChestData.Name) is Chest loadChest && loadChest is not null)
                {
                    InputChest = loadChest;
                }
                else if (Model.Chests.FirstOrDefault(c => c.Type == BuildingChest.ChestType.Chest) is var omniChestData && omniChestData is not null && Building.GetBuildingChest(omniChestData.Name) is Chest omniChest && omniChest is not null)
                {
                    InputChest = omniChest;
                }

                // Get the output chest
                if (Model.Chests.FirstOrDefault(c => c.Type == BuildingChest.ChestType.Collect) is var collectChestData && collectChestData is not null && Building.GetBuildingChest(collectChestData.Name) is Chest collectChest && collectChest is not null)
                {
                    OutputChest = collectChest;
                }
                else if (Model.Chests.FirstOrDefault(c => c.Type == BuildingChest.ChestType.Chest) is var omniChestData && omniChestData is not null && Building.GetBuildingChest(omniChestData.Name) is Chest omniChest && omniChest is not null)
                {
                    OutputChest = omniChest;
                }
            }
        }

        public ITrackedStack GetOutput()
        {
            var validOutputItem = OutputChest.items.FirstOrDefault();
            if (validOutputItem is null)
            {
                return null;
            }

            return new TrackedItem(validOutputItem, onEmpty: item =>
            {
                OutputChest.clearNulls();
                OutputChest.items.Remove(validOutputItem);
            });
        }

        public MachineState GetState()
        {
            if (Building.isUnderConstruction() || InputChest is null || OutputChest is null)
            {
                return MachineState.Disabled;
            }

            int processesConverting = Model.ItemConversions is not null ? Model.ItemConversions.Count(c => c.ShouldTrackTime) : 0;
            if ((Model.MaxConcurrentConversions == -1 && processesConverting == Model.ItemConversions.Count()) || (Model.MaxConcurrentConversions != -1 && processesConverting >= Model.MaxConcurrentConversions))
            {
                return MachineState.Processing;
            }
            else if ((processesConverting == 0 && OutputChest.items.Count(i => i is not null) > 0) || IsChestFull(OutputChest))
            {
                return MachineState.Done;
            }

            foreach (var item in InputChest.items.Where(i => i is not null))
            {
                if (Building.GetItemConversionForItem(item, InputChest) is var itemConversion && itemConversion is not null && itemConversion.RequiredCount <= item.Stack)
                {
                    return MachineState.Processing;
                }
            }
            return MachineState.Empty;
        }

        public bool SetInput(IStorage input)
        {
            InputChest.clearNulls();

            bool wasAnyItemAdded = false;
            foreach (var item in input.GetItems().Where(i => i.Count > 0))
            {
                if (IsChestFull(InputChest))
                {
                    Utility.consolidateStacks(InputChest.items);
                    return wasAnyItemAdded;
                }

                var sampleItem = item.Sample;
                sampleItem.Stack = item.Count;

                if (Building.IsValidObjectForChest(sampleItem, InputChest) is false)
                {
                    continue;
                }
                if (Building.GetItemConversionForItem(sampleItem, InputChest) is var itemConversion && itemConversion is not null)
                {
                    var restrictedCount = Building.GetMaxAllowedInChest(sampleItem, InputChest);
                    if (itemConversion.RequiredCount > restrictedCount)
                    {
                        continue;
                    }

                    sampleItem.Stack = restrictedCount;
                    if (itemConversion.RequiredCount < sampleItem.Stack)
                    {
                        sampleItem.Stack = sampleItem.Stack - (sampleItem.Stack % itemConversion.RequiredCount);
                    }
                }

                if (Building.IsObjectFilteredForChest(sampleItem, InputChest, performSilentCheck: true))
                {
                    continue;
                }

                int currentStack = sampleItem.Stack;
                Item returnedItem = Utility.addItemToThisInventoryList(sampleItem, InputChest.items, InputChest.GetActualCapacity());
                if (returnedItem is null || returnedItem.Stack != item.Count)
                {
                    wasAnyItemAdded = true;
                    item.Reduce(returnedItem is null ? currentStack : returnedItem.Stack);
                }
            }

            Utility.consolidateStacks(InputChest.items);
            return wasAnyItemAdded;
        }

        private bool IsChestFull(Chest chest)
        {
            if (chest.items.Count < chest.GetActualCapacity())
            {
                return false;
            }

            return chest.items.Any(i => i is null || i.Stack < i.maximumStackSize()) ? false : true;
        }
    }
}
