using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCrabPotsConfigUpdater.ModConfigs
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
