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
