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
using ItemPipes.Framework.Nodes;
using ItemPipes.Framework.Util;
using Netcode;

namespace ItemPipes.Framework
{
    public abstract class ContainerNode : Node
    {
        public string Type { get; set; }
        public List<IOPipeNode> IOPipes { get; set; }
        public NetObjectList<Item> Filter { get; set; }

        public ContainerNode() { }
        public ContainerNode(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {
            Type = "";
            IOPipes = new List<IOPipeNode>();
            Filter = new NetObjectList<Item>();
        }
        public abstract bool CanSendItems();
        public abstract bool CanRecieveItems();
        public abstract bool CanStackItems();
        public abstract bool CanRecieveItem(Item item);
        public abstract bool CanStackItem(Item item);
        public abstract bool InsertItem(Item item);
        public abstract bool IsEmpty();
        public abstract Item GetItemForInput(InputPipeNode input, int flux);

        public bool HasFilter()
        {
            bool hasFilter = false;
            if (Filter.Count > 0)
            {
                hasFilter = true;
            }

            return hasFilter;
        }

        public bool AddIOPipe(Node node)
        {
            bool added = false;

            if (IOPipes.Count < 4 && !IOPipes.Contains(node) && node is IOPipeNode)
            {
                added = true;
                IOPipes.Add(node as IOPipeNode);
                if (Globals.UltraDebug) { Printer.Info($"[?] IOPipe ADDED"); }
            }
            return added;
        }

        public bool RemoveIOPipe(Node node)
        {
            bool removed = false;
            if (IOPipes.Count > 0 && IOPipes.Contains(node) && node is IOPipeNode)
            {
                removed = true;
                IOPipes.Remove(node as IOPipeNode);
                if (Globals.UltraDebug) { Printer.Info($"[?] IOPipe REMOVED"); }
            }
            if (removed)
            {
                ScanMoreIOPipes();
            }
            return removed;
        }

        public void ScanMoreIOPipes()
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            int x = (int)Position.X;
            int y = (int)Position.Y;
            List<Node> nodes = DataAccess.LocationNodes[Location];
            Vector2 north = new Vector2(x, y - 1);
            Node northNode = nodes.Find(n => n.Position.Equals(north));
            if (northNode != null && northNode is IOPipeNode)
            {
                IOPipeNode northIOPipeNode = (IOPipeNode)northNode;
                northIOPipeNode.AddConnectedContainer(this);
            }
            Vector2 south = new Vector2(x, y + 1);
            Node southNode = nodes.Find(n => n.Position.Equals(south));
            if (southNode != null && northNode is IOPipeNode)
            {
                IOPipeNode southIOPipeNode = (IOPipeNode)southNode;
                southIOPipeNode.AddConnectedContainer(this);
            }
            Vector2 west = new Vector2(x + 1, y);
            Node westNode = nodes.Find(n => n.Position.Equals(west));
            if (westNode != null && northNode is IOPipeNode)
            {
                IOPipeNode westIOPipeNode = (IOPipeNode)westNode;
                westIOPipeNode.AddConnectedContainer(this);
            }
            Vector2 east = new Vector2(x - 1, y);
            Node eastNode = nodes.Find(n => n.Position.Equals(east));
            if (eastNode != null && northNode is IOPipeNode)
            {
                IOPipeNode eastIOPipeNode = (IOPipeNode)eastNode;
                eastIOPipeNode.AddConnectedContainer(this);
            }
        }

        public override bool AddAdjacent(Side side, Node node)
        {
            //Printer.Info($"ADDING ADJ: {node.Print()} to {Print()}");

            bool added = false;
            if (Adjacents[side] == null)
            {
                added = true;
                Adjacents[side] = node;
                node.AddAdjacent(Sides.GetInverse(side), this);
                if (node is IOPipeNode)
                {
                    IOPipeNode iOPipeNode = (IOPipeNode)node;
                    iOPipeNode.AddConnectedContainer(this);
                }
            }
            return added;
        }

        public override bool RemoveAdjacent(Side side, Node node)
        {
            //Printer.Info($"removing ADJ: {node.Print()} from {Print()}");

            bool removed = false;
            if (Adjacents[side] != null)
            {
                removed = true;
                Adjacents[side] = null;
                node.RemoveAdjacent(Sides.GetInverse(side), this);
                if (node is IOPipeNode)
                {
                    IOPipeNode iOPipeNode = (IOPipeNode)node;
                    iOPipeNode.RemoveConnectedContainer(this);
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



        public virtual NetObjectList<Item> UpdateFilter(NetObjectList<Item> filteredItems)
        {
            return null;
        }
    }
}
