using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.meats
{
    public class MeatItem
    {
        public int Price;
        public int Edibility;

        public MeatItem(int price, int edibility)
        {
            Price = price;
            Edibility = edibility;
        }
    }
}
