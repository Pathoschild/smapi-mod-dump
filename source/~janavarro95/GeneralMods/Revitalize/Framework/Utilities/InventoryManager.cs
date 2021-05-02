/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using StardewValley;

namespace Revitalize.Framework.Utilities
{
    /// <summary>Handles dealing with objects.</summary>
    public class InventoryManager
    {
        /// <summary>How many items the inventory can hold.</summary>
        public int capacity;

        /// <summary>The hard uper limit for # of items to be held in case of upgrading or resizing.</summary>
        public int MaxCapacity { get; private set; }

        /// <summary>How many items are currently stored in the inventory.</summary>
        public int ItemCount => this.items.Where(i => i != null).Count();

        /// <summary>The actual contents of the inventory.</summary>
        public IList<Item> items;

        /// <summary>
        /// Items that are to be buffered into the inventory manager if possible.
        /// </summary>
        public IList<Item> bufferItems;

        /// <summary>Checks if the inventory is full or not.</summary>
        public bool IsFull => this.ItemCount >= this.capacity && this.items.Where(i=>i==null).Count()==0;

        /// <summary>Checks to see if this core object actually has a valid inventory.</summary>
        public bool HasInventory => this.capacity > 0;

        [JsonIgnore]
        public bool hasItemsInBuffer
        {
            get
            {
                return this.bufferItems.Count > 0;
            }
        }

        public int displayRows;
        public int displayColumns;

        [JsonIgnore]
        public bool requiresUpdate;
        public InventoryManager()
        {
            this.capacity = 0;
            this.setMaxLimit(0);
            this.items = new List<Item>();
            this.bufferItems = new List<Item>();
        }

        /// <summary>Construct an instance.</summary>
        public InventoryManager(List<Item> items,int DisplayRows=6,int DisplayColumns=6)
        {
            this.capacity = int.MaxValue;
            this.setMaxLimit(int.MaxValue);
            this.items = items;
            this.bufferItems = new List<Item>();
            this.displayRows = DisplayRows;
            this.displayColumns = DisplayColumns;
        }

        public InventoryManager(IList<Item> items, int Capacity= int.MaxValue, int DisplayRows = 6, int DisplayColumns = 6)
        {
            this.capacity = Capacity;
            this.setMaxLimit(int.MaxValue);
            this.items = items;
            this.bufferItems = new List<Item>();
            this.displayRows = DisplayRows;
            this.displayColumns = DisplayColumns;
        }

        /// <summary>Construct an instance.</summary>
        public InventoryManager(int capacity, int DisplayRows = 6, int DisplayColumns = 6)
        {
            this.capacity = capacity;
            this.MaxCapacity = int.MaxValue;
            this.items = new List<Item>();
            this.bufferItems = new List<Item>();
            this.displayRows = DisplayRows;
            this.displayColumns = DisplayColumns;
        }

        /// <summary>Construct an instance.</summary>
        public InventoryManager(int capacity, int MaxCapacity, int DisplayRows = 6, int DisplayColumns = 6)
        {
            this.capacity = capacity;
            this.setMaxLimit(MaxCapacity);
            this.items = new List<Item>();
            this.bufferItems = new List<Item>();
            this.displayRows = DisplayRows;
            this.displayColumns = DisplayColumns;
        }

        /// <summary>Add the item to the inventory.</summary>
        public bool addItem(Item item)
        {
            if (this.IsFull)
            {
                return false;
            }
            else
            {
                for(int i = 0; i < this.items.Count; i++)
                {
                    Item self = this.items[i];
                    if (self != null && self.canStackWith(item))
                    {
                        self.addToStack(item);
                        this.requiresUpdate = true;
                        return true;
                    }
                    if (self == null)
                    {
                        self = item;
                        this.requiresUpdate=true;
                        return true;
                    }
                }

                this.requiresUpdate = true;
                this.items.Add(item);
                return true;
            }
        }

        /// <summary>Gets a reference to the object IF it exists in the inventory.</summary>
        public Item getItem(Item item)
        {
            foreach (Item i in this.items)
            {
                if (item == i)
                    return item;
            }
            return null;
        }

        /// <summary>Get the item at the specific index.</summary>
        public Item getItemAtIndex(int index)
        {
            return this.items[index];
        }

        /// <summary>Gets only one item from the stack.</summary>
        public Item getSingleItemFromStack(Item item)
        {
            if (item.Stack == 1)
                return item;

            this.requiresUpdate = true;
            item.Stack = item.Stack - 1;
            return item.getOne();
        }

        /// <summary>Empty the inventory.</summary>
        public void clear()
        {
            this.requiresUpdate = true;
            this.items.Clear();
        }

        /// <summary>Empty the inventory.</summary>
        public void empty()
        {
            this.clear();
        }

        /// <summary>Resize how many items can be held by this object.</summary>
        public void resizeCapacity(int Amount)
        {
            if (this.capacity + Amount < this.MaxCapacity)
            {
                this.capacity += Amount;
                this.requiresUpdate = true;
            }
        }

        /// <summary>Sets the upper limity of the capacity size for the inventory.</summary>
        public void setMaxLimit(int amount)
        {
            this.MaxCapacity = amount;
            this.requiresUpdate = true;
        }

        public bool canReceieveThisItem(Item I)
        {
            if (this.IsFull) return false;
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Returns a new inventory manager without the items but with the capacity limits.
        /// </summary>
        /// <returns></returns>
        public InventoryManager Copy()
        {
            return new InventoryManager(this.capacity, this.MaxCapacity,this.displayRows,this.displayColumns);
        }

        public void dumpBufferToItems()
        {
            foreach(Item I in this.bufferItems)
            {
                this.addItem(I);
            }
            this.bufferItems.Clear();
        }
    }
}
