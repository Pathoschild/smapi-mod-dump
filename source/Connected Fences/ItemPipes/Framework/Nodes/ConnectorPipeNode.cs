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
            bool added = false;
            if (Adjacents[side] == null)
            {
                if(!(node is ConnectorPipeNode) || (node is ConnectorPipeNode && node.GetType().Equals(this.GetType())))
                {
                    added = true;
                    Adjacents[side] = node;
                    node.AddAdjacent(Sides.GetInverse(side), this);
                }
            }
            return added;
        }
    }
}
