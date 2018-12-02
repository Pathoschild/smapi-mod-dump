using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RemoteFridgeStorage;

namespace RemoteFridgeStorageTest
{
    [TestFixture]
    public class VirtualListTest
    {
        [Test]
        public void BasicFunctionalityTest()
        {
            VirtualList<int> list = new VirtualList<int>();
            List<int> numbers1 = new List<int> {0, 1, 2, 3};
            List<int> numbers2 = new List<int> {4, 5, 6};
            List<int> numbers3 = new List<int> {7, 8, 9, 10, 11};
            list.AddSuppliers(numbers1, numbers2, numbers3);
            Assert.AreEqual(list.Count, 12);
            for (int i = 0; i < 12; i++)
            {
                Assert.AreEqual(list[i], i);
                Assert.AreEqual(list.IndexOf(i), i);
            }
        }

        [Test]
        public void AddItems()
        {
            VirtualList<int> list = new VirtualList<int>();
            List<int> numbers1 = new List<int> {0, 1, 2, 3};
            List<int> numbers2 = new List<int> {5, 6, 7};
            List<int> numbers3 = new List<int> {9, 10, 11, 12, 13};
            list.AddSuppliers(numbers1, numbers2, numbers3);

            Assert.AreEqual(list.Count, 12);
            list.Add(14);

            Assert.AreEqual(list.Count, 13);
            Assert.AreEqual(list[12], 14);
        }

        [Test]
        public void InsertItems()
        {
            VirtualList<int> list = new VirtualList<int>();
            List<int> numbers1 = new List<int> {0, 1, 2, 3};
            List<int> numbers2 = new List<int> {5, 6, 7};
            List<int> numbers3 = new List<int> {9, 10, 12, 13, 14};
            list.AddSuppliers(numbers1, numbers2, numbers3);

            list.Add(15);
            list.Insert(4, 4);
            Assert.AreEqual(list.Count, 14);

            list.Insert(8, 8);
            Assert.AreEqual(list.Count, 15);

            list.Insert(11, 11);
            Assert.AreEqual(list.Count, 16);

            for (int i = 0; i < 16; i++)
            {
                Assert.AreEqual(list[i], i);
            }
        }

        [Test]
        public void InsertItemsOriginalLists()
        {
            VirtualList<int> list = new VirtualList<int>();
            List<int> numbers1 = new List<int> {0, 1, 2, 3};
            List<int> numbers2 = new List<int> {5, 6, 7};
            List<int> numbers3 = new List<int> {9, 10, 12, 13, 14};
            list.AddSuppliers(numbers1, numbers2, numbers3);

            list.Add(15);
            list.Insert(4, 4);
            list.Insert(8, 8);
            list.Insert(11, 11);

            Assert.AreEqual(numbers2[0], 4);
            Assert.AreEqual(numbers3[0], 8);
            Assert.AreEqual(numbers3[3], 11);
            Assert.AreEqual(numbers3[7], 15);
        }

        [Test]
        public void RemoveItems()
        {
            VirtualList<int> list = new VirtualList<int>();
            List<int> numbers1 = new List<int> {0, 20, 1, 2};
            List<int> numbers2 = new List<int> {3, 4, 5};
            List<int> numbers3 = new List<int> {6, 21, 7, 8, 9};
            list.AddSuppliers(numbers1, numbers2, numbers3);

            list.Remove(20);
            list.Remove(21);

            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(list[i], i);
            }
        }

        [Test]
        public void RemoveItemsOriginal()
        {
            VirtualList<int> list = new VirtualList<int>();
            List<int> numbers1 = new List<int> {0, 20, 1, 2};
            List<int> numbers2 = new List<int> {3, 4, 5};
            List<int> numbers3 = new List<int> {6, 21, 7, 8, 9};
            list.AddSuppliers(numbers1, numbers2, numbers3);

            list.Remove(20);
            list.Remove(21);

            Assert.False(numbers1.Contains(20));
            Assert.False(numbers3.Contains(21));
        }

        [Test]
        public void RemoveIfDuplicate()
        {
            VirtualList<int> list = new VirtualList<int>();
            List<int> numbers1 = new List<int> {0, 7, 1, 2};
            List<int> numbers2 = new List<int> {3, 4, 5};
            List<int> numbers3 = new List<int> {6, 21, 7, 8, 9};
            list.AddSuppliers(numbers1, numbers2, numbers3);

            list.Remove(7);
            list.Remove(21);

            Assert.False(numbers1.Contains(7));
            Assert.True(numbers3.Contains(7));
        }

        [Test]
        public void RemoveAt()
        {
            VirtualList<int> list = new VirtualList<int>();
            List<int> numbers1 = new List<int> {0, 20, 1, 2};
            List<int> numbers2 = new List<int> {3, 4, 5};
            List<int> numbers3 = new List<int> {6, 21, 7, 8, 9};
            list.AddSuppliers(numbers1, numbers2, numbers3);

            list.RemoveAt(1);
            list.RemoveAt(7);

            Assert.False(numbers1.Contains(20));
            Assert.False(numbers3.Contains(21));
        }

        [Test]
        public void Copy()
        {
            VirtualList<int> list = new VirtualList<int>();
            List<int> numbers1 = new List<int> {0, 1, 2};
            List<int> numbers2 = new List<int> {3, 4, 5};
            List<int> numbers3 = new List<int> {6, 7, 8, 9};
            list.AddSuppliers(numbers1, numbers2, numbers3);

            int[] ints = new int[12];
            list.CopyTo(ints, 2);
            for (int i = 2; i < 12; i++)
            {
                Assert.AreEqual(ints[i], i - 2);
            }
        }

        [Test]
        public void CopyBig()
        {
            VirtualList<int> list = new VirtualList<int>();
            List<int> numbers1 = new List<int> {0, 1, 2};
            List<int> numbers2 = new List<int> {3, 4, 5};
            List<int> numbers3 = new List<int> {6, 7, 8, 9};
            list.AddSuppliers(numbers1, numbers2, numbers3);

            int[] ints = new int[20];
            list.CopyTo(ints, 2);
            for (int i = 2; i < 12; i++)
            {
                Assert.AreEqual(ints[i], i - 2);
            }
        }

        [Test]
        public void BasicFunctionalityTestWithSupplier()
        {
            VirtualList<int> list = new VirtualList<int>();
            List<int> numbers1 = new List<int> {0, 1, 2, 3};
            List<int> numbers2 = new List<int> {4, 5, 6};
            List<int> numbers3 = new List<int> {7, 8, 9, 10, 11};
            list.AddSuppliers(numbers1, numbers2, numbers3);
            Assert.AreEqual(list.Count, 12);
            for (int i = 0; i < 12; i++)
            {
                Assert.AreEqual(list[i], i);
                Assert.AreEqual(list.IndexOf(i), i);
            }
        }

        [Test]
        public void InsertItemsOriginalListsWithSupplier()
        {
            VirtualListBase<int, IntSupplier> list = new VirtualListBase<int, IntSupplier>(intSupplier => intSupplier.Ints);
            IntSupplier numbers1 = new IntSupplier(new List<int> {0, 1, 2, 3});
            IntSupplier numbers2 = new IntSupplier(new List<int> {5, 6, 7});
            IntSupplier numbers3 = new IntSupplier(new List<int> {9, 10, 12, 13, 14});
            list.AddSuppliers(numbers1, numbers2, numbers3);

            list.Add(15);
            list.Insert(4, 4);
            list.Insert(8, 8);
            list.Insert(11, 11);

            Assert.AreEqual(numbers2.Ints[0], 4);
            Assert.AreEqual(numbers3.Ints[0], 8);
            Assert.AreEqual(numbers3.Ints[3], 11);
            Assert.AreEqual(numbers3.Ints[7], 15);
        }
    }

    internal class IntSupplier
    {
        public IntSupplier(List<int> ints)
        {
            Ints = ints;
        }

        public List<int> Ints { get; set; }
    }
}