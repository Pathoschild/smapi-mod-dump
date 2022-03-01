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
using ItemPipes.Framework.Nodes.ObjectNodes;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using Netcode;

namespace ItemPipes.Framework
{
    public abstract class InputPipeNode : IOPipeNode
    {
        public NetObjectList<Item> Filter { get; set; }
        public int Priority { get; set; }

        public InputPipeNode() : base()
        {

        }

        public InputPipeNode(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {
            ConnectedContainer = null;
            Filter = new NetObjectList<Item>();
        }

        public bool CanRecieveItems()
        {
            bool canReceive = false;
            if (ConnectedContainer.CanRecieveItems())
            {
                canReceive = true;
            }
            return canReceive;
        }

        public bool TryInsertItem(Item item)
        {
            bool inserted = false;
            //Cambiar esto para que use method de container, y
            //luego override en subclasses
            if (ConnectedContainer is ChestContainerNode)
            {
                ChestContainerNode chest = (ChestContainerNode)ConnectedContainer;
                chest.InsertItem(item);
                inserted = true;
            }
            else if(ConnectedContainer is ShippingBinContainerNode)
            {

            }
            return inserted;
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
        public virtual void UpdateFilter()
        {
        }
    }
}
