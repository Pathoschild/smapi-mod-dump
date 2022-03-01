/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using ItemPipes.Framework.Model;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using Netcode;
using System.Collections.Generic;
using System.Linq;
using ItemPipes.Framework.Util;
using System;

namespace ItemPipes.Framework.Nodes.ObjectNodes
{
    public class ShippingBinContainerNode : ContainerNode
    {
        public ShippingBin ShippingBin { get; set; }
        public Farm Farm { get; set; }
        public ShippingBinContainerNode() { }

        public ShippingBinContainerNode(Vector2 position, GameLocation location, StardewValley.Object obj, Building building) : base(position, location, obj)
        {

            Name = building.buildingType.ToString();
            if (building is ShippingBin)
            {
                ShippingBin = (ShippingBin)building;
            }
            Farm = Game1.getFarm();
            Filter = new NetObjectList<Item>();
            Type = "ShippingBin";

        }



        public bool ShipItem(Item item)
        {
            bool shipped = false;
            if (item != null && item is StardewValley.Object && Farm != null)
            {
                Farm.getShippingBin(Game1.MasterPlayer).Add(item);
                ShippingBin.showShipment(item as StardewValley.Object, playThrowSound: false);
                Farm.lastItemShipped = item;
                Printer.Info("SHIPPED ITEMS");
                for(int i=0;i<Farm.getShippingBin(Game1.MasterPlayer).Count;i++)
                {
                    Printer.Info(i+" "+Farm.getShippingBin(Game1.MasterPlayer)[i].Name);
                }
                shipped = true;
            }
            return shipped;
        }

        public override NetObjectList<Item> UpdateFilter(NetObjectList<Item> filteredItems)
        {
            Filter = new NetObjectList<Item>();
            if (filteredItems == null)
            {
                Filter.Add(Farm.lastItemShipped);
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

        public override bool CanSendItems()
        {
            return !IsEmpty();
        }
        public override bool CanRecieveItems()
        {
            return true;
        }
        public override bool CanRecieveItem(Item item)
        {
            return true;
        }
        public override bool CanStackItem(Item item)
        {
            return false;
        }
        public override bool InsertItem(Item item)
        {
            return ShipItem(item);
        }

        public override Item GetItemForInput(InputPipeNode input)
        {
            Item item = null;
            if (input != null)
            {
                NetCollection<Item> itemList = Farm.getShippingBin(Game1.MasterPlayer);
                int index = itemList.Count - 1;
                if (CanSendItems() && Farm.lastItemShipped != null)
                {
                    if (input.HasFilter())
                    {
                        if (input.Filter.Any(i => i.Name.Equals(itemList[index].Name)))
                        {
                            item = TryExtractItem(input.ConnectedContainer, itemList);
                        }
                    }
                    else
                    {

                        item = TryExtractItem(input.ConnectedContainer, itemList);
                    }
                }
            }
            return item;
        }
        public Item TryExtractItem(ContainerNode input, NetCollection<Item> itemList)
        {
            //Exception for multiple thread collisions
            Printer.Info("EXTRAATING");
            Item item = null;
            try
            {
                if (input.CanStackItem(item))
                {
                    item = itemList.Last();
                    itemList.Remove(itemList.Last());
                    //item.Stack = 20;
                    //itemList[index].Stack = itemList[index].Stack-20;
                    Farm.lastItemShipped = itemList.Last();
                    if (itemList.Count == 1)
                    {
                        Farm.lastItemShipped = null;
                    }
                    else
                    {
                        Farm.lastItemShipped = itemList.Last();
                    }
                }
                else if (input.CanRecieveItems())
                {
                    item = itemList.Last();
                    itemList.Remove(itemList.Last());
                    //item.Stack = 20;
                    //itemList[index].Stack = itemList[index].Stack-20;
                    if(itemList.Count == 1)
                    {
                        Farm.lastItemShipped = null;
                    }
                    else
                    {
                        Farm.lastItemShipped = itemList.Last();
                    }
                }
            }
            catch (Exception e)
            {

            }
            return item;
        }

        public override bool IsEmpty()
        {
            if (Farm.getShippingBin(Game1.MasterPlayer).Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        
    }
}
