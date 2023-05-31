/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omegasis.Revitalize.Framework.World.Buildings;
using Omegasis.Revitalize.Framework.World.Objects.Storage;
using Pathoschild.Stardew.Automate;
using StardewValley;

namespace Omegasis.RevitalizeAutomateCompatibility.Objects.Storage
{
    public class DimensionalStorageChestWrapper: IContainer
    {
        public DimensionalStorageChest revitalizeObject;

        public string Name => this.revitalizeObject.Name;

        public ModDataDictionary ModData => this.revitalizeObject.modData;

        public bool IsJunimoChest => false;

        public GameLocation Location => this.revitalizeObject.getCurrentLocation();

        public Rectangle TileArea { get; }


        public DimensionalStorageChestWrapper(DimensionalStorageChest revitalizeObject, GameLocation location, Vector2 tile)
        {
            this.revitalizeObject = revitalizeObject;
            this.TileArea = new Rectangle((int)tile.X, (int)tile.Y, 1, 1);
        }

        public ITrackedStack Get(Func<Item, bool> predicate, int count)
        {
            ITrackedStack[] stacks = this.GetImpl(predicate, count).ToArray();
            if (!stacks.Any())
                return null;
            return new TrackedItemCollection(stacks);
        }

        /// <summary>Find items in the pipe matching a predicate.</summary>
        /// <param name="predicate">Matches items that should be returned.</param>
        /// <param name="count">The number of items to find.</param>
        /// <remarks>If there aren't enough items in the pipe, it should return those it has.</remarks>
        private IEnumerable<ITrackedStack> GetImpl(Func<Item, bool> predicate, int count)
        {
            if (this.getDimensionalStorageUnitBuilding() == null) yield break;

            int countFound = 0;
            foreach (Item item in this.getDimensionalStorageUnitBuilding().items)
                if (item != null && predicate(item))
                {
                    ITrackedStack stack = this.GetTrackedItem(item);
                    if (stack == null)
                        continue;

                    countFound += item.Stack;
                    yield return stack;
                    if (countFound >= count)
                        yield break;
                }
        }

        public int GetCapacity()
        {
            if (this.getDimensionalStorageUnitBuilding() == null) return 0;

            return (int)this.getDimensionalStorageUnitBuilding().DimensionalStorageUnitMaxItems;
        }

        public IEnumerator<ITrackedStack> GetEnumerator()
        {
            if (this.getDimensionalStorageUnitBuilding() == null) yield break;

            foreach (Item item in this.getDimensionalStorageUnitBuilding().items.ToArray())
            {
                ITrackedStack stack = this.GetTrackedItem(item);
                if (stack != null)
                    yield return stack;
            }
        }

        public int GetFilled()
        {
            if (this.getDimensionalStorageUnitBuilding() == null) return 0;

            return this.getDimensionalStorageUnitBuilding().items.Count(p => p != null);
        }

        public void Store(ITrackedStack stack)
        {
            if (this.getDimensionalStorageUnitBuilding() == null) return;

            IList<Item> inventory = this.getDimensionalStorageUnitBuilding().items;

            // try stack into existing slot
            foreach (Item slot in inventory)
                if (slot != null && stack.Sample.canStackWith(slot))
                {
                    Item sample = stack.Sample.getOne();
                    sample.Stack = stack.Count;
                    int added = stack.Count - slot.addToStack(sample);
                    stack.Reduce(added);
                    if (stack.Count <= 0)
                        return;
                }

            // try add to empty slot
            long capacity = this.getDimensionalStorageUnitBuilding().DimensionalStorageUnitMaxItems;
            for (int i = 0; i < capacity && i < inventory.Count; i++)
                if (inventory[i] == null)
                {
                    inventory[i] = stack.Take(stack.Count);
                    return;
                }

            // try add new slot
            if (inventory.Count < capacity)
                inventory.Add(stack.Take(stack.Count));
        }

        /// <summary>Get a tracked item synced with the chest inventory.</summary>
        /// <param name="item">The item to track.</param>
        private ITrackedStack GetTrackedItem(Item item)
        {
            if (item == null || item.Stack <= 0)
                return null;

            if (this.getDimensionalStorageUnitBuilding() == null) return null;

            try
            {
                return new TrackedItem(item, onEmpty: i => this.getDimensionalStorageUnitBuilding().items.Remove(i));
            }
            catch (KeyNotFoundException)
            {
                return null; // invalid/broken item, silently ignore it
            }
            catch (Exception ex)
            {
                string error = $"Failed to retrieve item #{item.ParentSheetIndex} ('{item.Name}'";
                if (item is StardewValley.Object obj && obj.preservedParentSheetIndex.Value >= 0)
                    error += $", preserved item #{obj.preservedParentSheetIndex.Value}";
                error += $") from container '{this.revitalizeObject.Name}' at {this.Location.Name} (tile: {this.TileArea.X}, {this.TileArea.Y}).";

                throw new InvalidOperationException(error, ex);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public DimensionalStorageUnitBuilding getDimensionalStorageUnitBuilding()
        {
            return DimensionalStorageUnitBuilding.GetDimensionalStorageUnitBuilding();
        }
    }
}
