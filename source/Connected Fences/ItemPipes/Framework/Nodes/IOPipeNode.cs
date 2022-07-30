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
using ItemPipes.Framework.Model;
using ItemPipes.Framework.Util;
using ItemPipes.Framework.Nodes;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using SObject = StardewValley.Object;
using ItemPipes.Framework.Items;


namespace ItemPipes.Framework
{
    public abstract class IOPipeNode : PipeNode
    {
        public ContainerNode ConnectedContainer { get; set; }
        public string Signal { get; set; }

        public IOPipeNode() : base()
        {
            ConnectedContainer = null;
            Connecting = false;
            LoadSignal();
        }

        public IOPipeNode(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {
            ConnectedContainer = null;
            Connecting = false;
            LoadSignal();
        }

        public void LoadSignal()
        {
            SObject item;
            if(Location.objects.TryGetValue(Position, out item))
            {
                IOPipeItem pipe = (IOPipeItem)item;
                Signal = pipe.Signal;
            }
        }

        public virtual void UpdateSignal()
        {
            if(!Signal.Equals("off"))
            {
                if (ConnectedContainer == null)
                {
                    Signal = "nochest";
                }
                else if (ConnectedContainer != null)
                {
                    Signal = "on";
                }
            }
        }

        public virtual void ChangeSignal()
        {
            if(Signal.Equals("off"))
            {
                if (ConnectedContainer == null)
                {
                    Signal = "nochest";
                }
                else if (ConnectedContainer != null)
                {
                    Signal = "on";
                }
            }
            else
            {
                Signal = "off";
            }           
        }

        public bool AddConnectedContainer(Node node)
        {
            bool added = false;
            if (ConnectedContainer == null && node is ContainerNode)
            {
                ContainerNode container = (ContainerNode)node;
                if (container.IOPipes.Count < 4)
                {
                    ConnectedContainer = (ContainerNode)node;
                    ConnectedContainer.AddIOPipe(this);
                }
                else
                {
                }
            }
            else
            {
            }
            UpdateSignal();
            added = true;
            return added;
        }

        public bool RemoveConnectedContainer(Node node)
        {
            bool removed = false;
            if (ModEntry.config.DebugMode) { Printer.Info($"[?] Removing {node.Name} container "); }
            if (ConnectedContainer != null && node is ContainerNode)
            {
                ConnectedContainer.RemoveIOPipe(this);
                ConnectedContainer = null;
                if (ModEntry.config.DebugMode) { Printer.Info($"[?] CONNECTED CONTAINER REMOVED"); }
                removed = true;
            }
            UpdateSignal();
            return removed;
        }

        public override bool AddAdjacent(Side side, Node node)
        {
            bool added = false;
            if (Adjacents[side] == null)
            {
                added = true;
                Adjacents[side] = node;
                node.AddAdjacent(Sides.GetInverse(side), this);
                if(node is ContainerNode)
                {
                    AddConnectedContainer(node);
                }
            }
            else if (Adjacents[side] != null &&
                Adjacents[side].Adjacents[Sides.GetInverse(side)] != null &&
                Adjacents[side].Adjacents[Sides.GetInverse(side)].ParentNetwork != null &&
                Adjacents[side].ParentNetwork != null &&
                Adjacents[side].Adjacents[Sides.GetInverse(side)].ParentNetwork != Adjacents[side].ParentNetwork)
            {
                added = true;
                Adjacents[side].Adjacents[Sides.GetInverse(side)] = this;
            }
            return added;
        }

        public override bool RemoveAdjacent(Side side, Node node)
        {
            bool removed = false;
            if (Adjacents[side] != null)
            {
                removed = true;
                Adjacents[side] = null;
                node.RemoveAdjacent(Sides.GetInverse(side), this);
                if (node is ContainerNode)
                {
                    RemoveConnectedContainer(node);
                }
            }
            return removed;
        }


        public override bool RemoveAllAdjacents()
        {
            bool removed = false;
            foreach (KeyValuePair<Side, Node> adj in Adjacents.ToList())
            {
                if (adj.Value != null)
                {
                    removed = true;
                    RemoveAdjacent(adj.Key, adj.Value);
                }
            }
            return removed;
        }

    }
}
