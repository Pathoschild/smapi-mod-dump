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
using StardewModdingAPI.Events;
using SObject = StardewValley.Object;
using StardewValley.Tools;
using System.Xml.Serialization;
using ItemPipes.Framework.Util;
using System.Threading;


namespace ItemPipes.Framework.Model
{
    public abstract class Node
    {
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public GameLocation Location { get; set; }
        public StardewValley.Object Obj { get; set; }
        public Dictionary<Side, Node> Adjacents { get; set; }
        public Network ParentNetwork { get; set; }
        public SideStruct Sides { get; set; }
        public bool Reached { get; set; }
        public bool Passable { get; set; }
        public string State { get; set; }

        public Node()
        {
            Sides = SideStruct.GetSides();

            Adjacents = new Dictionary<Side, Node>();
            Adjacents.Add(Sides.North, null);
            Adjacents.Add(Sides.South, null);
            Adjacents.Add(Sides.West, null);
            Adjacents.Add(Sides.East, null);
        }

        public Node(Vector2 position, GameLocation location, StardewValley.Object obj)
        {
            if (obj != null) { Name = obj.name; }
            Position = position;
            Location = location;
            Obj = obj;

            Sides = SideStruct.GetSides();

            Adjacents = new Dictionary<Side, Node>();
            Adjacents.Add(Sides.North, null);
            Adjacents.Add(Sides.South, null);
            Adjacents.Add(Sides.West, null);
            Adjacents.Add(Sides.East, null);

            ParentNetwork = null;
        }

        public List<Node> TraverseAll()
        {
            List<Node> looked = new List<Node>();
            Reached = false;
            if (Globals.UltraDebug) { Printer.Info("TRAVERSING"); }
            System.Object[] returns = TraverseAllRecursive(looked, false);
            List<Node> path = (List<Node>)returns[1];
            return path;
        }

        public System.Object[] TraverseAllRecursive(List<Node> looked, bool reached)
        {
            if (Globals.UltraDebug) { Print(); }
            System.Object[] returns = new System.Object[3];
            returns[2] = reached;
            looked.Add(this);
            Node adj;
            if (Adjacents.TryGetValue(Sides.North, out adj) && !(bool)returns[2])
            {
                if (adj != null && !looked.Contains(adj))
                {
                    returns = adj.TraverseAllRecursive(looked, reached);
                }
            }
            if (Adjacents.TryGetValue(Sides.South, out adj) && !(bool)returns[2])
            {
                if (adj != null && !looked.Contains(adj))
                {
                    returns = adj.TraverseAllRecursive(looked, reached);
                }
            }
            if (Adjacents.TryGetValue(Sides.West, out adj) && !(bool)returns[2])
            {
                if (adj != null && !looked.Contains(adj))
                {
                    returns = adj.TraverseAllRecursive(looked, reached);
                }
            }
            if (Adjacents.TryGetValue(Sides.East, out adj) && !(bool)returns[2])
            {
                if (adj != null && !looked.Contains(adj))
                {
                    returns = adj.TraverseAllRecursive(looked, reached);
                }
            }
            if (!(bool)returns[2])
            {
                looked.Remove(this);
            }
            return returns;
        }

        public List<Network> Scan()
        {
            List<Network> retList = new List<Network>();
            foreach (KeyValuePair<Side, Node> adj in Adjacents)
            {
                if (adj.Value != null && !retList.Contains(adj.Value.ParentNetwork))
                {
                    retList.Add(adj.Value.ParentNetwork);
                }
            }
            return retList;
        }

        public virtual bool AddAdjacent(Side side, Node node)
        {
            bool added = false;
            if (Adjacents[side] == null)
            {
                added = true;
                Adjacents[side] = node;
                node.AddAdjacent(Sides.GetInverse(side), this);
            }
            return added;
        }

        public virtual bool RemoveAdjacent(Side side, Node node)
        {
            bool removed = false;
            if (Adjacents[side] != null)
            {
                removed = true;
                Adjacents[side] = null;
                node.RemoveAdjacent(Sides.GetInverse(side), this);
            }
            return removed;
        }

        
        public virtual bool RemoveAllAdjacents()
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

        public bool Same(Node node)
        {
            if(this.Obj.name.Equals(node.Obj.name) && Position.Equals(node.Position))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetName()
        {
            if(Passable)
            {
                return Name+"_passable";
            }
            else
            {
                return Name;
            }
        }

        public string Print()
        {
            if (ParentNetwork != null)
            {
                return $"'[T{Thread.CurrentThread.ManagedThreadId}][N{ParentNetwork.ID}]{Name}({Position.X},{Position.Y}){GetHashCode()}'";
            }
            else
            {
                return $"'[T{Thread.CurrentThread.ManagedThreadId}][N?]{Name}({Position.X},{Position.Y}){GetHashCode()}'";
            }
        }
    }
}
