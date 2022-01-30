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
using StardewValley.Buildings;

namespace ItemPipes.Framework
{
    public abstract class InputNode : IOPipeNode
    {
        public List<string> Filter { get; set; }
        public int Priority { get; set; }

        public InputNode() : base()
        {

        }

        public InputNode(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {
            ConnectedContainer = null;
            Filter = new List<string>();
        }

        public bool HasFilter()
        {
            bool hasFilter = false;
            if (Filter.Count > 0)
            {
                hasFilter = true;
            }
            return hasFilter;
        }
        public virtual void UpdateFilter()
        {
        }
    }
}
