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
using Microsoft.Xna.Framework.Graphics;
using ItemPipes.Framework.Util;
using ItemPipes.Framework.Model;
using ItemPipes.Framework.Nodes;
using StardewValley;
using StardewValley.Tools;
using StardewValley.Objects;
using SObject = StardewValley.Object;
using System.Threading;
using System.Xml.Serialization;


namespace ItemPipes.Framework.Items
{
    public abstract class ConnectorPipeItem : PipeItem
    {
        public ConnectorPipeItem() : base()
        {
            
        }
        public ConnectorPipeItem(Vector2 position) : base(position)
        {
            
        }

        public override bool countsForDrawing(SObject adj)
        {
            if (adj is PipeItem && !(adj is ConnectorPipeItem))
            {
                return true;
            }
            else if(adj is PipeItem && adj is ConnectorPipeItem)
            {
                if(adj.GetType().Equals(this.GetType()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
