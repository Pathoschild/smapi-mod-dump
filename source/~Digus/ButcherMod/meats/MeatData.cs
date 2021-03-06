/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

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
        public MeatItem Ostrich;

        public MeatData()
        {
            Beef = new MeatItem(100,15);
            Pork = new MeatItem(1250, 30);
            Chicken = new MeatItem(250, 15);
            Duck = new MeatItem(400, 20);
            Rabbit = new MeatItem(2500, 20);
            Mutton = new MeatItem(650, 20);
            Ostrich = new MeatItem(10500, 60);
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
                case Meat.Ostrich:
                    return Ostrich;
                default:
                    throw new ArgumentException("Invalid Meat");
            }
        }
    }
}
