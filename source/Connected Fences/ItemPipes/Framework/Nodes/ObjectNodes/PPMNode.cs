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
using ItemPipes.Framework.Nodes.ObjectNodes;
using ItemPipes.Framework.Util;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;


namespace ItemPipes.Framework.Nodes.ObjectNodes
{
    public class PPMNode : Node
    {
        public List<Network> AdjNetworks { get; set; }
        public PPMNode() { }
        public PPMNode(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {
            State = "off";
            AdjNetworks = new List<Network>();
        }

        public bool ChangeState()
        {
            if(State.Equals("on"))
            {
                State = "off";
                foreach(Network network in AdjNetworks)
                {
                    if (network != null)
                    {
                        network.Deinvisibilize(this);
                    }
                }
                return false;
            }
            else
            {
                State = "on";
                foreach (Network network in AdjNetworks)
                {
                    if(network != null)
                    {
                        network.Invisibilize(this);
                    }
                }
                return true;
            }
        }

        public override bool AddAdjacent(Side side, Node node)
        {
            bool added = false;
            if (Adjacents[side] == null)
            {
                added = true;
                Adjacents[side] = node;
                node.AddAdjacent(Sides.GetInverse(side), this);
                if (node is PipeNode && node.ParentNetwork != null)
                {
                    PipeNode pipeNode = (PipeNode)node;
                    AdjNetworks.Add(pipeNode.ParentNetwork);
                    pipeNode.ParentNetwork.AddNode(this);
                }
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
                if (node != null && node is PipeNode)
                {
                    PipeNode pipeNode = (PipeNode)node;
                    AdjNetworks.Remove(pipeNode.ParentNetwork);
                    if(pipeNode.ParentNetwork != null)
                    {
                        pipeNode.ParentNetwork.RemoveNode(this);
                    }
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
