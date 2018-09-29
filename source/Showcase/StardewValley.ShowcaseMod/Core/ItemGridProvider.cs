using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace Igorious.StardewValley.ShowcaseMod.Core
{
    internal class ItemGridProvider : ICollection<Item>, IReadOnlyCollection<Item>
    {
        private List<Item> Items { get; }
        private int InitialRows { get; }
        private int InitialColumns { get; }

        public ItemGridProvider(List<Item> items, int rows, int columns, int currentRotation)
        {
            Items = items;
            CurrentRotation = currentRotation;
            InitialRows = IsHorizontal? columns : rows;
            InitialColumns = IsHorizontal? rows : columns;
        }

        public int CurrentRotation { get; private set; }
        public bool IsHorizontal => (CurrentRotation != -1) && (CurrentRotation % 2 == 1);
        public int Rows => IsHorizontal? InitialColumns : InitialRows;
        public int Columns => IsHorizontal? InitialRows : InitialColumns;

        public ItemGridProvider Clone(int newRotation)
        {
            var provider = new ItemGridProvider(Items.ToList(), Rows, Columns, CurrentRotation);
            if (CurrentRotation != -1) provider.UpdateCurrentRotation(newRotation);
            return provider;
        }

        public void Recalculate()
        {
            var prefferedCount = InitialRows * InitialColumns;
            while (Items.Count > prefferedCount && Items.Remove(null)) { }
            while (Items.Count < prefferedCount)
            {
                Items.Add(null);
            }
        }

        public void UpdateCurrentRotation(int currentRotation)
        {
            if (currentRotation == CurrentRotation || currentRotation == -1) return;

            var listCopy = Items.ToList();
            var deltaRotation = CurrentRotation - currentRotation;
            CurrentRotation = currentRotation;

            for (var i = 0; i < Rows; ++i)
            {
                for (var j = 0; j < Columns; ++j)
                {
                    var itemIndex = GetItemIndex(i, j, deltaRotation);
                    if (itemIndex < listCopy.Count) this[i, j] = listCopy[itemIndex];
                }
            }           
        }

        public Item this[int i, int j]
        {
            get
            {
                var itemIndex = GetItemIndex(i, j);
                return (itemIndex < Items.Count)? Items[itemIndex] : null;
            }
            set
            {
                var itemIndex = GetItemIndex(i, j);
                if (itemIndex >= Items.Count) return;
                Items[itemIndex] = value;
            }
        }

        public List<Item> GetInternalList() => Items;

        private int GetItemIndex(int i, int j, int deltaRotation = 0)
        {
            int itemIndex;
            switch (AsRotation(deltaRotation))
            {
                case 1:
                    itemIndex = (Rows * Columns - 1) - Rows * j + (-Rows + 1 + i);
                    break;
                case 2:
                    itemIndex = (Columns * Rows - 1) - Columns * i - j;
                    break;
                case 3:
                    itemIndex = (Rows - 1 - i) + Rows * j;
                    break;
                default:
                    itemIndex = Columns * i + j;
                    break;
            }
            return itemIndex;
        }

        private int AsRotation(int r)
        {
            return (r + 8) % 4;
        }

        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
        IEnumerator<Item> IEnumerable<Item>.GetEnumerator() => Items.GetEnumerator();
        public bool Contains(Item item) => Items.Contains(item);
        public void CopyTo(Item[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);
        public int Count => Items.Count;
        public bool IsReadOnly => false;

        public void Add(Item item)
        {
            var index = Items.IndexOf(null);
            if (index != -1)
            {
                Items[index] = item;
            }
            else
            {
                Items.Add(item);
            }
        }

        public bool Remove(Item item)
        {
            var index = Items.IndexOf(item);
            if (index != -1)
            {
                Items[index] = null;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Clear()
        {
            Items.Clear();
            Items.AddRange(Enumerable.Repeat<Item>(null, Rows * Columns));
        }
    }
}