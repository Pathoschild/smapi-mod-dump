using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AnimalHusbandryMod.meats.MeatExtension;

namespace AnimalHusbandryMod.meats
{
    public class MeatData
    {
        public MeatItem Beef;
        public MeatItem Pork;
        public MeatItem Chicken;
        public MeatItem Duck;
        public MeatItem Rabbit;
        public MeatItem Mutton;

        public MeatData()
        {
            Beef = new MeatItem(100,15);
            Pork = new MeatItem(1250, 30);
            Chicken = new MeatItem(250, 15);
            Duck = new MeatItem(800, 20);
            Rabbit = new MeatItem(2500, 20);
            Mutton = new MeatItem(650, 20);
        }

        public MeatItem getMeatItem(Meat meatEnum)
        {
            switch (meatEnum)
            {
                case Meat.Beef:
                    return Beef;
                case Meat.Pork:
                    return Pork;
                case Meat.Chicken:
                    return Chicken;
                case Meat.Duck:
                    return Duck;
                case Meat.Rabbit:
                    return Rabbit;
                case Meat.Mutton:
                    return Mutton;
                default:
                    throw new ArgumentException("Invalid Meat");
            }
        }
    }
}
