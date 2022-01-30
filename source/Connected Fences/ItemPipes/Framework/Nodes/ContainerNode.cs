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
    public class ContainerNode : PipeNode
    {
        public string Type { get; set; }
        public OutputNode Output { get; set; }
        public InputNode Input { get; set; }
        public List<string> Filter { get; set; }

        public ContainerNode() { }
        public ContainerNode(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {
            Type = "";
            Output = null;
            Input = null;
            Filter = new List<string>();
        }


        public bool AddIOPipe(Node entity)
        {
            bool added = false;
            if (Output == null && entity is OutputNode)
            {
                Output = (OutputNode)entity;
                added = true;
                if (Globals.UltraDebug) { Printer.Info($"[?] OUTPUT ADDED"); }
            }
            else if (Input == null && entity is InputNode)
            {
                Input = (InputNode)entity;
                added = true;
                if (Globals.UltraDebug) { Printer.Info($"[?] INPUT ADDED"); }
            }
            return added;
        }

        public bool RemoveIOPipe(Node entity)
        {
            bool removed = false;
            if (Output != null && entity is OutputNode)
            {
                Output = null;
                removed = true;
                if (Globals.UltraDebug) { Printer.Info($"[?] OUTPUT REMOVED"); }
            }
            else if (Input != null && entity is InputNode)
            {
                Input = null;
                removed = true;
                if (Globals.UltraDebug) { Printer.Info($"[?] INPUT REMOVED"); }
            }
            if(removed)
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


        public virtual bool IsEmpty()
        {
            return false;
        }

        public virtual List<string> UpdateFilter(NetObjectList<Item> filteredItems)
        {
            return null;
        }
    }
}
