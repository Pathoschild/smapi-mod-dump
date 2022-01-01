/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using ItemPipes.Framework.Model;
using Netcode;


namespace ItemPipes.Framework.Objects
{
    public class ChestContainer : Container
    {
        public Chest Chest { get; set; }
        public ChestContainer(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {
            if (obj is Chest)
            {
                Chest = (Chest)obj;
                Printer.Info(obj.name);
                Printer.Info(Chest.fridge.ToString());
            }
            Type = "Chest";
        }

        public Item GetItemToShip(Input input)
        {
            Item item = null;
            if (!IsEmpty() && input != null)
            {
                NetObjectList<Item> itemList = GetItemList();
                int index = itemList.Count - 1;
                while (index >= 0 && item == null)
                {
                    if (input.HasFilter())
                    {
                        if (input.Filter.Contains(itemList[index].Name))
                        {
                            item = itemList[index];
                            itemList.RemoveAt(index);
                            Chest.clearNulls();
                        }
                    }
                    else
                    {
                        item = itemList[index];
                        itemList.RemoveAt(index);
                        Chest.clearNulls();
                    }
                    index--;
                }
            }
            return item;
        }

        public Item TrySendItem(ChestContainer input, NetObjectList<Item> itemList, int index)
        {
            Item item = null;
            if (input.CanStackItem(item))
            {
                item = itemList[index];
                itemList.RemoveAt(index);
                Chest.clearNulls();
            }
            else if (input.CanReceiveItems())
            {
                item = itemList[index];
                itemList.RemoveAt(index);
                Chest.clearNulls();
            }
            return item;
        }

        public Item CanSendItem(ChestContainer input)
        {
            Item item = null;
            if (!IsEmpty() && input != null)
            {
                NetObjectList<Item> itemList = GetItemList();
                int index = itemList.Count - 1;
                while (index >= 0 && item == null)
                {
                    if (Globals.Debug) { Printer.Info("Trying to send: "+itemList[index].Name); }
                    if (input.HasFilter())
                    {
                        if (Globals.Debug) { Printer.Info("Input has filter" + input.Filter.Count.ToString()); }
                        if (input.Filter.Contains(itemList[index].Name))
                        {
                            item = TrySendItem(input, itemList, index);
                        }
                    }
                    else
                    {
                        item = TrySendItem(input, itemList, index);
                    }
                    index--;
                }
            }
            return item;
        }

        public bool SendItem(ChestContainer input, Item item)
        {
            bool sent = false;
            if (!IsEmpty() && input != null && input.HasFilter())
            {
                if (input.Filter.Contains(item.Name))
                {
                    if (input.CanStackItem(item))
                    {
                        input.ReceiveStack(item);
                        sent = true;
                    }
                    else if (input.CanReceiveItems())
                    {
                        input.ReceiveItem(item);
                        sent = true;
                    }
                }
                else
                {
                    if (CanStackItem(item))
                    {
                        ReceiveStack(item);
                        sent = false;
                    }
                    else if (CanReceiveItems())
                    {
                        ReceiveItem(item);
                        sent = false;
                    }
                    else
                    {
                        //Drop item
                        sent = false;
                    }
                }

            }
            else
            {
                if (input.CanStackItem(item))
                {
                    input.ReceiveStack(item);
                    sent = true;
                }
                else if (input.CanReceiveItems())
                {
                    input.ReceiveItem(item);
                    sent = true;
                }
                else
                {
                    if (CanStackItem(item))
                    {
                        ReceiveStack(item);
                        sent = false;
                    }
                    else if (CanReceiveItems())
                    {
                        ReceiveItem(item);
                        sent = false;
                    }
                    //If returning chest is full
                    else
                    {
                        //Drop item
                        //Game1.currentLocation.dropObject(item);
                        sent = false;
                    }
                }

            }
            if (Globals.Debug) { Printer.Info("Item sent? " + sent.ToString()); }
            return sent;
        }

        public bool CanStack()
        {
            bool canStack = false;
            NetObjectList<Item> itemList = GetItemList();
            int index = itemList.Count - 1;
            while (index >= 0 && !canStack)
            {
                if (itemList[index] != null)
                {
                    if (itemList[index].getRemainingStackSpace() > 0)
                    {
                        canStack = true;
                    }
                }
                index--;
            }

            return canStack;
        }
        public bool CanStackItem(Item item)
        {
            bool canStack = false;
            NetObjectList<Item> itemList = GetItemList();
            if (itemList.Contains(item))
            {
                int index = itemList.IndexOf(item);
                if (itemList[index].canStackWith(item))
                {
                    canStack = true;
                }
            }
            return canStack;
        }

        public bool CanReceiveItems()
        {
            bool canReceive = false;
            NetObjectList<Item> itemList = GetItemList();
            if (itemList.Count < Chest.GetActualCapacity())
            {
                canReceive = true;
            }
            return canReceive;
        }
        public void ReceiveStack(Item item)
        {
            Chest.addToStack(item);
        }

        public void ReceiveItem(Item item)
        {
            Chest.addItem(item);
        }

        public override List<string> UpdateFilter(NetObjectList<Item> filteredItems)
        {
            Filter = new List<string>();
            if (filteredItems == null)
            {
                NetObjectList<Item> itemList = GetItemList();
                foreach (Item item in itemList.ToList())
                {
                    Filter.Add(item.Name);
                }
            }
            else
            {
                foreach (Item item in filteredItems.ToList())
                {
                    Filter.Add(item.Name);
                }
            }
            return Filter;

        }
        public bool HasFilter()
        {
            bool hasFilter = false;
            if (Filter.Count > 0)
            {
                hasFilter = true;
            }
            return hasFilter;
        }

        public override bool IsEmpty()
        {
            bool isEmpty = false;
            NetObjectList<Item> itemList = GetItemList();
            if (itemList.Count < 1)
            {
                isEmpty = true;
            }
            return isEmpty;
        }

        public NetObjectList<Item> GetItemList()
        {
            NetObjectList<Item> itemList;
            if (Chest.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin || Chest.SpecialChestType == Chest.SpecialChestTypes.JunimoChest)
            {
                itemList = Chest.GetItemsForPlayer(Game1.player.UniqueMultiplayerID);
            }
            else
            {
                itemList = Chest.items;
            }
            return itemList;
        }
    }
}
