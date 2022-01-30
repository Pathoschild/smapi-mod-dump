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
using ItemPipes.Framework.Util;
using Netcode;
using System.Threading;


namespace ItemPipes.Framework.Nodes
{
    public class ChestContainerNode : ContainerNode
    {
        public Chest Chest { get; set; }
        public ChestContainerNode() { }
        public ChestContainerNode(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {
            if (obj is Chest)
            {
                Chest = (Chest)obj;
            }
            Type = "Chest";
        }

        public Item GetItemToShip(InputNode input)
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

        public Item TrySendItem(ChestContainerNode input, NetObjectList<Item> itemList, int index)
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

        public Item CanSendItem(ChestContainerNode input)
        {
            Item item = null;
            if (!IsEmpty() && input != null)
            {
                NetObjectList<Item> itemList = GetItemList();
                int index = itemList.Count - 1;
                while (index >= 0 && item == null)
                {

                    if (Globals.UltraDebug) { Printer.Info($"T[{Thread.CurrentThread.ManagedThreadId}][?] Trying to send: " +itemList[index].Name); }
                    if(itemList[index] != null)
                    {
                        if (input.HasFilter())
                        {
                            if (Globals.UltraDebug) { Printer.Info($"T[{Thread.CurrentThread.ManagedThreadId}][?] Input has filter" + input.Filter.Count.ToString()); }
                            if (input.Filter.Contains(itemList[index].Name))
                            {
                                item = TrySendItem(input, itemList, index);
                            }
                        }
                        else
                        {

                            item = TrySendItem(input, itemList, index);
                        }
                    }
                    index--;
                }
            }
            return item;
        }

        public bool InsertItem(Item item)
        {
            bool sent = false;
            if (CanStackItem(item))
            {
                ReceiveStack(item);
                sent = true;
            }
            else if (CanReceiveItems())
            {
                ReceiveItem(item);
                sent = true;
            }
            return sent;
        }

        public bool SendItem(ChestContainerNode input, Item item)
        {
            bool sent = false;
            if(input != null)
            {
                if (input.HasFilter() && input.Filter.Contains(item.Name))
                {
                    if (input.InsertItem(item))
                    {
                        sent = true;
                    }
                    else
                    {
                        InsertItem(item);
                        sent = false;
                    }
                }
                else if(!input.HasFilter())
                {
                    if (input.InsertItem(item))
                    {
                        sent = true;
                    }
                    else
                    {
                        InsertItem(item);
                        sent = false;
                    }
                }
            }
            return sent;
        }
        /*
        public bool SendItem(ChestContainerNode input, Item item)
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
            if (Globals.UltraDebug) { Printer.Info($"T[{Thread.CurrentThread.ManagedThreadId}][?] Item sent? " + sent.ToString()); }
            return sent;
        }
        */
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
