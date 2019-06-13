using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB_BQMS
{
    public class AllChestsValueContainer
    {
        public Item previousItem;
        public StardewValley.GameLocation location;
        public bool hasBeenChecked = false;

        public AllChestsValueContainer(Item item, StardewValley.GameLocation whereat, bool isChecked)
        {
            previousItem = item;
            location = whereat;
            hasBeenChecked = isChecked;
        }
    }
}
