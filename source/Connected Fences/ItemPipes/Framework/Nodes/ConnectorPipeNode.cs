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
using Microsoft.Xna.Framework;
using StardewValley;
using ItemPipes.Framework.Nodes;
using ItemPipes.Framework.Util;


namespace ItemPipes.Framework
{
    public abstract class ConnectorPipeNode : PipeNode
    {
        public ConnectorPipeNode() : base()
        {

        }
        public ConnectorPipeNode(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {

        }

        public override bool AddAdjacent(Side side, Node node)
        {
            //Printer.Info($"ADDING ADJ: {node.Print()} to {Print()}");
            //Printer.Info($"IS SIDE NULL?" + (Adjacents[side] == null).ToString());
            if(Adjacents[side] != null)
            {
                //Printer.Info($"SIDE: {side.Name} is {Adjacents[side].Print()}");
            }
            bool added = false;
            if (Adjacents[side] == null)
            {
                if (!(node is ConnectorPipeNode) || (node is ConnectorPipeNode && node.GetType().Equals(this.GetType())))
                {
                    added = true;
                    Adjacents[side] = node;
                    node.AddAdjacent(Sides.GetInverse(side), this);
                }
            }
            else if (Adjacents[side]  != null &&
                Adjacents[side].Adjacents[Sides.GetInverse(side)] != null &&
                Adjacents[side].Adjacents[Sides.GetInverse(side)].ParentNetwork != null &&
                Adjacents[side].ParentNetwork != null &&
                Adjacents[side].Adjacents[Sides.GetInverse(side)].ParentNetwork != Adjacents[side].ParentNetwork)
            {
                //Printer.Info($"ADDING ADJ adj: {Adjacents[side].Adjacents[Sides.GetInverse(side)].Print()} of {Adjacents[side].Print()}");
                //Printer.Info($"in wrong network of {Adjacents[side].Print()}");

                added = true;
                Adjacents[side].Adjacents[Sides.GetInverse(side)] = this;
                //Printer.Info($"ADDING ADJ adj: {Adjacents[side].Adjacents[Sides.GetInverse(side)].Print()} of {Adjacents[side].Print()}");
            }
            return added;
        }
    }
}
