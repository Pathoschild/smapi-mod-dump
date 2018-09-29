using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGrabberMod.Models
{
    using StardewValley;
    using SVObject = StardewValley.Object;

    public class NextItem
    {
        public SVObject Item { get; set; }
        public Chest Chest { get; set; }       

        public NextItem(SVObject item, Chest chest)
        {
            Item = item;
            Chest = chest;
        }

        public void UseItem()
        {
            Item.Stack -= 1;
            if (Item.Stack <= 0) Chest.items.Remove(Item);
        }
    }
}
