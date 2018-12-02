using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace RemoteFridgeStorage
{
    /// <summary>
    /// The list for using the remoteFridgeStorage.
    /// </summary>
    public class FridgeVirtualList : VirtualListBase<Item, Chest>
    {
        public FridgeVirtualList(FridgeHandler fridgeHandler) : base(ToList(fridgeHandler),
            chest => chest.items)
        {
        }

        private static List<Chest> ToList(FridgeHandler fridgeHandler)
        {
            var chests = fridgeHandler.Chests.ToList();
            if (Game1.getLocationFromName("farmHouse") is FarmHouse farm) chests.Add(farm.fridge.Value);
            return chests;
        }
    }


    /// <summary>
    /// A list of lists that acts as one list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class VirtualList<T> : VirtualListBase<T, IList<T>>
    {
        public VirtualList() : base(new List<IList<T>>(), list => list)
        {
        }

        public VirtualList(IList<IList<T>> suppliers) : base(suppliers, list => list)
        {
        }
    }

    /// <summary>
    /// A Virtual list will act on a list of collections as if it is one list.
    /// The list has two type parameters. The type of the list, and the type of the supplier.
    /// The supplier is either a list or an object that contains a list.
    /// You pass a list of suppliers as the first argument and a function that reveals the underlying list as second argument.
    ///
    /// Adding a item will add it to the last list. R
    /// Removing a item will remove the first occurence of the first list that contains the number.
    /// Clearing the list will clear the list of suppliers not the items in the list.
    /// use <see cref="ClearUnderlying"/> for clearing the underlying lists.
    ///<example>
    ///<code>
    /// class FavouriteNumbers {
    ///        public IList<int> myNumbers;
    ///  }
    ///
    /// main() {
    ///     FavouriteNumbers johnsNumbers = new FavouriteNumbers(1,2,3,4,5,6)
    ///     FavouriteNumbers henksNumbers = new FavouriteNumbers(10,100,1000)
    ///     IList<int> list = new List(){johnNumbers,henkNumbers}
    ///     VirtualListBase allNumbers = new VirtualListBase(list,favouriteNumbers => favouriteNumbers.myNumbers)
    /// 
    ///     allNumbers[0] // 1
    ///     allNumbers[1] // 2
    ///     allNumbers[7] // 100
    ///     allNumbers.indexOf(5) // 4
    ///     allNumbers.indexOf(10) // 6
    ///     allNumbers.Count = 9
    ///     
    /// }
    ///</code>
    ///</example>
    /// 
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TSupplier"></typeparam>
    public class VirtualListBase<T, TSupplier> : IList<T> where TSupplier : class
    {
        private readonly IList<TSupplier> _suppliers;
        private readonly Func<TSupplier, IList<T>> _converter;

        public VirtualListBase(Func<TSupplier, IList<T>> converter) : this(new List<TSupplier>(), converter)
        {
        }

        public VirtualListBase(IList<TSupplier> suppliers, Func<TSupplier, IList<T>> converter)
        {
            _suppliers = suppliers;
            _converter = converter;
//            _chests = fridgeHandler.Chests.ToList();
//            if (Game1.currentLocation is FarmHouse farm) _chests.Add(farm.fridge.Value);
        }

        public IList<TSupplier> Suppliers => _suppliers;

        public IEnumerator<T> GetEnumerator()
        {
            var list = new List<T>();
            foreach (var chest in _suppliers)
            {
                list.AddRange(_converter(chest));
            }

            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            if (_suppliers.Count == 0) return;
            _converter(_suppliers[_suppliers.Count - 1]).Add(item);
        }

        public void Clear()
        {
            _suppliers.Clear();
        }

        public bool Contains(T item)
        {
            return _suppliers.Any(c => _converter(c).Contains(item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array.Length - arrayIndex < Count)
                throw new IndexOutOfRangeException(
                    $"Not enough space: list size was {Count} but remaining size was {(array.Length - arrayIndex)}");
            for (var i = arrayIndex; i < arrayIndex + Count && i < array.Length; i++)
            {
                array[i] = this[i - arrayIndex];
            }
        }

        public bool Remove(T item)
        {
            foreach (var chest in _suppliers)
            {
                if (_converter(chest).Remove(item)) return true;
            }

            return false;
        }

        public int Count
        {
            get
            {
                var count = 0;
                foreach (var chest in _suppliers)
                {
                    count += _converter(chest).Count;
                }

                return count;
            }
        }

        public bool IsReadOnly { get; } = false;

        public int IndexOf(T item)
        {
            var searched = 0;
            foreach (var chest in _suppliers)
            {
                var ind = _converter(chest).IndexOf(item);
                if (ind != -1)
                {
                    return searched + ind;
                }

                searched += _converter(chest).Count;
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            var pair = LocalIndex(index);
            _converter(pair.Chest).Insert(pair.Index, item);
        }

        public void RemoveAt(int index)
        {
            var pair = LocalIndex(index);
            _converter(pair.Chest).RemoveAt(pair.Index);
        }

        public T this[int index]
        {
            get
            {
                var pair = LocalIndex(index);
                return _converter(pair.Chest)[pair.Index];
            }
            set
            {
                var pair = LocalIndex(index);
                _converter(pair.Chest)[pair.Index] = value;
            }
        }

        /// <summary>
        /// Gets the list with the index in that list that corresponds to the input index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        private ListIndexPair LocalIndex(int index)
        {
            if (index < 0 | index >= Count)
            {
                throw new IndexOutOfRangeException($"Index was {index} but size was {Count}");
            }

            var iterateIndex = 0;
            while (iterateIndex < _suppliers.Count)
            {
                if (index < _converter(_suppliers[iterateIndex]).Count)
                {
                    return new ListIndexPair(_suppliers[iterateIndex], index);
                }

                index -= _converter(_suppliers[iterateIndex]).Count;

                iterateIndex++;
            }

            //It should not reach this.
            return new ListIndexPair(null, -1);
        }

        private struct ListIndexPair
        {
            public ListIndexPair(TSupplier chest, int i)
            {
                Chest = chest;
                Index = i;
            }

            public TSupplier Chest { get; }
            public int Index { get; }
        }

        public void ClearUnderlying()
        {
            foreach (var supplier in _suppliers)
            {
                _converter(supplier).Clear();
            }
        }

        public void AddSupplier(TSupplier supplier)
        {
            _suppliers.Add(supplier);
        }

        public void AddSuppliersRange(IEnumerable<TSupplier> suppliers)
        {
            foreach (var supplier in suppliers)
            {
                AddSupplier(supplier);
            }
        }

        public void AddSuppliers(params TSupplier[] suppliers)
        {
            foreach (var supplier in suppliers)
            {
                AddSupplier(supplier);
            }
        }
    }
}