using System;
using AnimalHusbandryMod.meats;

namespace AnimalHusbandryMod.meats
{
    internal class MeatAttribute : Attribute
    {
        public Meat Meat;

        public MeatAttribute(Meat meat)
        {
            this.Meat = meat;
        }
    }
}