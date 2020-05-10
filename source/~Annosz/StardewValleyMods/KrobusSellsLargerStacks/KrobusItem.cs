using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrobusSellsLargerStacks
{
    public class KrobusItem
    {
        public int ItemQuantity { get; set; }
        public int ItemId { get; set; }
        public string Type { get; set; }

        public KrobusItem(int itemQuantity, int itemId = 0, string type = "")
        {
            ItemQuantity = itemQuantity;
            ItemId = itemId;
            Type = type;
        }
    }
}
