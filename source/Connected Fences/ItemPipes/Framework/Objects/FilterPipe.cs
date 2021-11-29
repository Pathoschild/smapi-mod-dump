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
using ItemPipes.Framework;
using ItemPipes.Framework.Model;

namespace ItemPipes.Framework.Objects
{
    class FilterPipe : Input
    {
        public Chest Chest { get; set; }
        public FilterPipe(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {
            ConnectedContainer = null;
            Chest = new Chest(true, position, 130);
            Priority = 3;
        }

        public override void UpdateFilter()
        {
            if (ConnectedContainer != null)
            {
                Filter = ConnectedContainer.UpdateFilter(Chest.items);
            }
        }
    }
}
