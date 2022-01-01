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

namespace ItemPipes.Framework
{
    public class Connector : Node
    {
        public bool PassingItem { get; set; }
        public bool Connecting { get; set; }


        public Connector(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {
            PassingItem = false;
            Connecting = false;
        }

        public override string GetState()
        {
            if(PassingItem)
            {
                return "item";
            }
            else if (Connecting)
            {
                return "connecting";
            }
            else
            {
                return "default";
            }

        }

    }
}
