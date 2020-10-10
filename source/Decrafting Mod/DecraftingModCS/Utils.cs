/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/iSkLz/DecraftingMod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using static StardewValley.Objects.ObjectFactory;
using SObject = StardewValley.Object;

namespace DecraftingModCS
{
    public static class Utils
    {
        public static Item ItemFromID(int ID, int Stack = 1)
        {
            //return new SObject(ID, Stack);
            return getItemFromDescription(0, ID, Stack);
        }

        public static int ItemID(Item Item)
        {
            return getDescriptionFromItem(Item).index;
        }
    }
}
