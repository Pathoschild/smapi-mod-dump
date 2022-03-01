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


namespace ItemPipes.Framework.Nodes.ObjectNodes
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

        public override bool CanSendItems()
        {
            bool canSend = false;
            if (!IsEmpty())
            {
                canSend = true;
            }
            return canSend;
        }

        public override bool CanRecieveItems()
        {
            bool canReceive = false;
            NetObjectList<Item> itemList = GetItemList();
            if (itemList.Count < Chest.GetActualCapacity())
            {
                canReceive = true;
            }
            return canReceive;
        }

        public override bool CanRecieveItem(Item item)
        {
            bool canReceive = false;

            return canReceive;
        }

        public override bool InsertItem(Item item)
        {
            bool sent = false;
            if (CanStackItem(item))
            {
                ReceiveStack(item);
                sent = true;
            }
            else if (CanRecieveItems())
            {
                RecieveItem(item);
                sent = true;
            }
            return sent;
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

        public override Item GetItemForInput(InputPipeNode input)
        {
            Item item = null;
            if (input != null)
            {
                NetObjectList<Item> itemList = GetItemList();
                int index = itemList.Count - 1;
                while (index >= 0 && item == null)
                {
                    if (Globals.UltraDebug) { Printer.Info($"T[{Thread.CurrentThread.ManagedThreadId}][?] Trying to send: " + itemList[index].Name); }
                    if (itemList[index] != null)
                    {
                        if (input.HasFilter())
                        {
                            if (Globals.UltraDebug) { Printer.Info($"T[{Thread.CurrentThread.ManagedThreadId}][?] Input has filter" + input.Filter.Count.ToString()); }
                            if (input.Filter.Any(i => i.Name.Equals(itemList[index].Name)))
                            {
                                item = TryExtractItem(input.ConnectedContainer, itemList, index);
                            }
                        }
                        else
                        {

                            item = TryExtractItem(input.ConnectedContainer, itemList, index);
                        }
                    }
                    index--;
                }
            }
            return item;
        }

        public Item TryExtractItem(ContainerNode input, NetObjectList<Item> itemList, int index)
        {
            //Exception for multiple thread collisions
            Item item = null;
            try
            {
                if (input.CanStackItem(item))
                {
                    item = itemList[index];
                    itemList.RemoveAt(index);
                    //item.Stack = 20;
                    //itemList[index].Stack = itemList[index].Stack-20;
                    Chest.clearNulls();
                }
                else if (input.CanRecieveItems())
                {
                    item = itemList[index];
                    itemList.RemoveAt(index);
                    //item.Stack = 20;
                    //itemList[index].Stack = itemList[index].Stack - 20;
                    Chest.clearNulls();
                }
            }
            catch (Exception e)
            {

            }
            return item;
        }

        public override bool CanStackItem(Item item)
        {
            bool canStack = false;
            NetObjectList<Item> itemList = GetItemList();
            if (itemList.Contains(item))
            {
                int index = itemList.IndexOf(item);
                if (index < itemList.Count && itemList[index].canStackWith(item))
                {
                    canStack = true;
                }
            }
            return canStack;
        }

        public void ReceiveStack(Item item)
        {
            Chest.addToStack(item);
        }

        public void RecieveItem(Item item)
        {
            Chest.addItem(item);
        }

        public override NetObjectList<Item> UpdateFilter(NetObjectList<Item> filteredItems)
        {
            Filter = new NetObjectList<Item>();
            if (filteredItems == null)
            {
                NetObjectList<Item> itemList = GetItemList();
                foreach (Item item in itemList.ToList())
                {
                    Filter.Add(item);
                }
            }
            else
            {
                foreach (Item item in filteredItems.ToList())
                {
                    Filter.Add(item);
                }
            }
            return Filter;

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

        /*
        public Item GetItemToShip(InputPipeNode input)
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
                        if (input.Filter.Any(i => i.Name.Equals(itemList[index].Name)))
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
                            //input.Filter.fin
                            if (input.Filter.Any(i => i.Name.Equals(itemList[index].Name)))
                            {
                                item = TryGetItem(input, itemList, index);
                            }
                        }
                        else
                        {

                            item = TryGetItem(input, itemList, index);
                        }
                    }
                    index--;
                }
            }
            return item;
        }



        public bool SendItem(ChestContainerNode input, Item item)
        {
            bool sent = false;
            if (input != null)
            {
                if (input.HasFilter() && input.Filter.Any(i => i.Name.Equals(item.Name)))
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
                else if (!input.HasFilter())
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




        */
    }
}
