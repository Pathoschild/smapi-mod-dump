/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCrabPots.Config
{
    class Item
    {
        public int Id { get; set; }
        public int Chance { get; set; }
        public int Quantity { get; set; }

        public Item(int id = -1, int chance = 1, int quantity = 1)
        {
            Id = id;
            Chance = chance;
            Quantity = quantity;
        }
    }
}
