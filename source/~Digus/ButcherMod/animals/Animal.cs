/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using AnimalHusbandryMod.meats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals
{
    public enum Animal
    {
        [Meat(Meat.Beef)]
        Cow,
        [Meat(Meat.Pork)]
        Pig,
        [Meat(Meat.Chicken)]
        Chicken,
        [Meat(Meat.Duck)]
        Duck,
        [Meat(Meat.Rabbit)]
        Rabbit,
        [Meat(Meat.Mutton)]
        Sheep,
        [Meat(Meat.Mutton)]
        Goat,
        Dinosaur,
        CustomAnimal

    }
}
