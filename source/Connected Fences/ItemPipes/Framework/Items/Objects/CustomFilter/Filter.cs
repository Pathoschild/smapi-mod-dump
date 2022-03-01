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
using System.Xml.Serialization;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using ItemPipes.Framework.Util;
using ItemPipes.Framework.Nodes;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;
using Netcode;


namespace ItemPipes.Framework.Items.CustomFilter
{
    public class Filter
	{
        public NetObjectList<Item> items { get; set; }
        public string message { get; set; }
        public int Cols { get; set; }
        public int Rows { get; set; }
        public int Capacity { get; set; }
        public Filter()
        {

        }
        public Filter(int capacity)
        {
            items = new NetObjectList<Item>();
            message = "Filter";
            Capacity = capacity;
            //Generate cols and rows based on capacity
            Cols = 9;
            Rows = 1;
        }

        public void ShowMenu()
        {
			Game1.activeClickableMenu = new FilterItemGrabMenu(this, items, Capacity, reverseGrab: false, showReceivingMenu: true, InventoryMenu.highlightAllItems,
                grabItemFromInventory, message, grabItemFromChest, snapToBottom: true, canBeExitedWithKey: true, playRightClickSound: true,
                allowRightClick: false, showOrganizeButton: false, 1, null, -80, this);
		}

        public void grabItemFromInventory(Item item, Farmer who)
        {
            addItem(item); 
            clearNulls();
        }

        public void grabItemFromChest(Item item, Farmer who)
        {
            items.Remove(item);
            clearNulls();
            ShowMenu();
        }

        public Item addItem(Item item)
        {
            item.resetState();
            this.clearNulls();
            if (items.Count < Capacity && !items.Any(i => i.Name.Equals(item.Name)))
            {
                items.Add(item.getOne());
                return null;
            }
            return item;
        }

        public void clearNulls()
        {
            for (int j = this.items.Count - 1; j >= 0; j--)
            {
                if (this.items[j] == null)
                {
                    this.items.RemoveAt(j);
                }
            }
        }

        public int GetActualCapacity()
        {
            return items.Capacity;
        }
    }
}
