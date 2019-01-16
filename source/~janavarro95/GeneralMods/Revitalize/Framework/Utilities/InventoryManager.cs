using System.Collections.Generic;
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
        public int ItemCount => this.items.Count;

        /// <summary>The actual contents of the inventory.</summary>
        public List<Item> items;

        /// <summary>Checks if the inventory is full or not.</summary>
        public bool IsFull => this.ItemCount >= this.capacity;

        /// <summary>Checks to see if this core object actually has a valid inventory.</summary>
        public bool HasInventory => this.capacity > 0;

        public InventoryManager()
        {
            this.capacity = 0;
            this.setMaxLimit(0);
            this.items = new List<Item>();
        }

        /// <summary>Construct an instance.</summary>
        public InventoryManager(List<Item> items)
        {
            this.capacity = int.MaxValue;
            this.setMaxLimit(int.MaxValue);
            this.items = items;
        }

        /// <summary>Construct an instance.</summary>
        public InventoryManager(int capacity)
        {
            this.capacity = capacity;
            this.MaxCapacity = int.MaxValue;
            this.items = new List<Item>();
        }

        /// <summary>Construct an instance.</summary>
        public InventoryManager(int capacity, int MaxCapacity)
        {
            this.capacity = capacity;
            this.setMaxLimit(MaxCapacity);
            this.items = new List<Item>();
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
                foreach (Item self in this.items)
                {
                    if (self != null && self.canStackWith(item))
                    {
                        self.addToStack(item.Stack);
                        return true;
                    }
                }
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

            item.Stack = item.Stack - 1;
            return item.getOne();
        }

        /// <summary>Empty the inventory.</summary>
        public void clear()
        {
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
                this.capacity += Amount;
        }

        /// <summary>Sets the upper limity of the capacity size for the inventory.</summary>
        public void setMaxLimit(int amount)
        {
            this.MaxCapacity = amount;
        }
    }
}
