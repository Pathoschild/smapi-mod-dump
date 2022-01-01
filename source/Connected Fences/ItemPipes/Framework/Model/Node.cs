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

namespace ItemPipes.Framework.Model
{
    public class Node
    {
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public GameLocation Location { get; set; }
        public StardewValley.Object Obj { get; set; }
        public Dictionary<Side, Node> Adjacents { get; set; }
        public Network ParentNetwork { get; set; }
        public SideStruct Sides { get; set; }
        public bool Reached { get; set; }
        public string State { get; set; }
        public bool Passable { get; set; }

        public Node(Vector2 position, GameLocation location, StardewValley.Object obj)
        {
            if (obj != null) { Name = obj.name; }
            Position = position;
            Location = location;
            Obj = obj;
            State = "default";
            Passable = false;

            Sides = SideStruct.GetSides();

            Adjacents = new Dictionary<Side, Node>();
            Adjacents.Add(Sides.North, null);
            Adjacents.Add(Sides.South, null);
            Adjacents.Add(Sides.West, null);
            Adjacents.Add(Sides.East, null);

            ParentNetwork = null;
        }

        public virtual string GetState()
        {
            return State;
        }

        public List<Node> TraverseAll()
        {
            List<Node> looked = new List<Node>();
            Reached = false;
            if (Globals.Debug) { Printer.Info("TRAVERSING"); }
            System.Object[] returns = TraverseAllRecursive(looked, false);
            List<Node> path = (List<Node>)returns[1];
            return path;
        }

        public System.Object[] TraverseAllRecursive(List<Node> looked, bool reached)
        {
            if (Globals.Debug) { Print(); }
            System.Object[] returns = new System.Object[3];
            returns[2] = reached;
            Node adj;
            looked.Add(this);
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

        public bool CanConnectedWith(Node target)
        {
            bool connected = false;
            List<Node> looked = new List<Node>();
            if ((bool)GetPathRecursive(target, looked, false)[2])
            {
                if (Globals.Debug) { Printer.Info("CAN CONNECT"); }
                connected = true;
            }
            return connected;
        }

        public List<Node> GetPath(Node target)
        {
            List<Node> looked = new List<Node>();
            Reached = false;
            System.Object[] returns = GetPathRecursive(target, looked, false);
            List<Node> path = (List<Node>)returns[1];
            return path;
        }

        public System.Object[] GetPathRecursive(Node target, List<Node> looked, bool reached)
        {
            if (Globals.Debug) { Print(); }
            System.Object[] returns = new System.Object[3];
            returns[2] = reached;
            Node adj;
            if (this.Equals(target))
            {
                reached = true;
                if (Globals.Debug) { Printer.Info("Reached"); Printer.Info(looked.Count.ToString()); }
                returns[0] = this;
                returns[1] = looked;
                returns[2] = reached;
                return returns;
            }
            else
            {
                looked.Add(this);
                if (Adjacents.TryGetValue(Sides.North, out adj) && !(bool)returns[2])
                {
                    if (adj != null && !looked.Contains(adj))
                    {
                        returns = adj.GetPathRecursive(target, looked, reached);
                    }
                }
                if (Adjacents.TryGetValue(Sides.South, out adj) && !(bool)returns[2])
                {
                    if (adj != null && !looked.Contains(adj))
                    {
                        returns = adj.GetPathRecursive(target, looked, reached);
                    }
                }
                if (Adjacents.TryGetValue(Sides.West, out adj) && !(bool)returns[2])
                {
                    if (adj != null && !looked.Contains(adj))
                    {
                        returns = adj.GetPathRecursive(target, looked, reached);
                    }
                }

                if (Adjacents.TryGetValue(Sides.East, out adj) && !(bool)returns[2])
                {
                    if (adj != null && !looked.Contains(adj))
                    {
                        returns = adj.GetPathRecursive(target, looked, reached);
                    }
                }
                if(!(bool)returns[2])
                {
                    looked.Remove(this);
                }
                return returns;
            }
        }

        public List<Network> Scan()
        {
            List<Network> retList = new List<Network>();
            foreach(KeyValuePair<Side, Node> adj in Adjacents)
            {
                if(adj.Value != null)
                {
                    retList.Add(adj.Value.ParentNetwork);
                }
            }
            return retList;
        }

        public Node GetAdjacent(Side side)
        {
            return Adjacents[side];
        }

        public virtual bool AddAdjacent(Side side, Node entity)
        {
            bool added = false;
            if (Adjacents[side] == null)
            {
                added = true;
                Adjacents[side] = entity;
                entity.AddAdjacent(Sides.GetInverse(side), this);
            }
            return added;
        }

        public virtual bool RemoveAdjacent(Side side, Node entity)
        {
            bool removed = false;
            if (Adjacents[side] != null)
            {
                removed = true;
                Adjacents[side] = null;
                entity.RemoveAdjacent(Sides.GetInverse(side), this);
            }
            return removed;
        }

        
        public virtual bool RemoveAllAdjacents()
        {
            bool removed = false;
            foreach(KeyValuePair<Side, Node> adj in Adjacents.ToList())
            {
                if(adj.Value != null)
                {
                    removed = true;
                    RemoveAdjacent(adj.Key, adj.Value);
                    Adjacents[adj.Key] = null;
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

        public void Print()
        {
            if (ParentNetwork != null)
            {
                Printer.Info($"[{ParentNetwork.ID}] " + Name + Position.X.ToString() + Position.Y.ToString() + " " + GetHashCode().ToString());
            }
            else
            {
                Printer.Info($"[?] " + Name + Position.X.ToString() + Position.Y.ToString() + " " + GetHashCode().ToString());
            }
        }
    }
}
