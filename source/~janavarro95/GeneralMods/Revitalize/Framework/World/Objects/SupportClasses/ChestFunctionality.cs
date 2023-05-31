/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Netcode;
using Omegasis.StardustCore.Networking;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace Omegasis.Revitalize.Framework.World.Objects.SupportClasses
{



    /// <summary>
    /// A simple, abstracted class which allows complete access to a simple chest functionality without the need of the actual chest object.
    /// </summary>
    [XmlType("Mods_Omegasis.Revitalize.Framework.World.SupportClasses.ChestFunctionality")]
    public class ChestFunctionality:NetObject
    {
        public readonly NetObjectList<Item> items = new NetObjectList<Item>();
        public NetRef<Chest> actualChest = new NetRef<Chest>();

        public ChestFunctionality()
        {
            this.actualChest.Value = new Chest();
        }

        public virtual void grabItemFromInventory(Item item, Farmer who)
        {
            if (item.Stack == 0)
            {
                item.Stack = 1;
            }
            Item tmp = this.addItem(item);
            if (tmp == null)
            {
                who.removeItemFromInventory(item);
            }
            else
            {
                tmp = who.addItemToInventory(tmp);
            }
            this.clearNulls();
            int oldID = ((Game1.activeClickableMenu.currentlySnappedComponent != null) ? Game1.activeClickableMenu.currentlySnappedComponent.myID : (-1));
            this.showMenu(this.actualChest.Value);
            (Game1.activeClickableMenu as ItemGrabMenu).heldItem = tmp;
            if (oldID != -1)
            {
                Game1.activeClickableMenu.currentlySnappedComponent = Game1.activeClickableMenu.getComponentWithID(oldID);
                Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
            }
        }

        public virtual void grabItemFromChest(Item item, Farmer who)
        {
            if (who.couldInventoryAcceptThisItem(item))
            {
                this.items.Remove(item);
                this.clearNulls();
                this.showMenu(this.actualChest.Value);
            }
        }

        public virtual Item addItem(Item item)
        {
            item.resetState();
            this.clearNulls();
            NetObjectList<Item> item_list = this.items;
            for (int i = 0; i < item_list.Count; i++)
            {
                if (item_list[i] != null && item_list[i].canStackWith(item))
                {
                    item.Stack = item_list[i].addToStack(item);
                    if (item.Stack <= 0)
                    {
                        return null;
                    }
                }
            }
            if (item_list.Count < 36)
            {
                item_list.Add(item);
                return null;
            }
            return item;
        }

        public virtual void clearNulls()
        {
            for (int j = this.items.Count - 1; j >= 0; j--)
            {
                if (this.items[j] == null)
                {
                    this.items.RemoveAt(j);
                }
            }
        }

        public virtual void showMenu(Item callingItem)
        {
            Game1.activeClickableMenu = new ItemGrabMenu(this.items, true, true, InventoryMenu.highlightAllItems, this.grabItemFromInventory, null, this.grabItemFromChest, snapToBottom: false, canBeExitedWithKey: true, playRightClickSound: true, allowRightClick: true, showOrganizeButton: true, 1, callingItem, -1, this);
        }


        protected override void initializeNetFields()
        {
            base.initializeNetFields();
            this.NetFields.AddField(this.items);
        }
    }
}
