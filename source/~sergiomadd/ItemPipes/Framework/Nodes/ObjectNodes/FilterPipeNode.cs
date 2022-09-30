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
using ItemPipes.Framework.Util;
using ItemPipes.Framework.Items;
using StardewValley.Menus;
using Netcode;
using ItemPipes.Framework.Items.Objects;
using SObject = StardewValley.Object;
using MaddUtil;


namespace ItemPipes.Framework.Nodes.ObjectNodes
{
    public class FilterPipeNode : InputPipeNode
    {
        public FilterPipeNode() : base()
        {

        }
        public FilterPipeNode(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {
            Filter = new FilterNode(true);
            ConnectedContainer = null;
            Priority = 3;
            if(obj != null && obj is FilterPipeItem)
            {
                Filter.Items = (obj as FilterPipeItem).Filter.items;
            }
            ItemTimer = 1000;
            
            if(obj is FilterPipeItem)
            {
                Filter.UpdateOption("quality", (obj as FilterPipeItem).Filter.Options["quality"]);
            }
        }

        public override void UpdateFilter()
        {
            SObject item;
            if(Location.objects.TryGetValue(Position, out item))
            {
                if(item is FilterPipeItem)
                {
                    Filter.Items = (item as FilterPipeItem).Filter.items;
                }
            }
        }
    }
}
