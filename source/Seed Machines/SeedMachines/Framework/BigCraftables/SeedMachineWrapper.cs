/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mrveress/SDVMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedMachines.Framework.BigCraftables
{
    class SeedMachineWrapper : IBigCraftableWrapper
    {
        public SeedMachineWrapper()
        {
            this.name = "Seed Machine";
            this.price = ModEntry.settings.seedMachinePrice;
            this.availableOutdoors = true;
            this.availableIndoors = true;
            this.fragility = 0;
            this.typeAndCategory = "Crafting -9";

            this.ingredients = ModEntry.settings.seedMachineIngredients;
            this.unlockConditions = "null";

            this.dynamicObjectType = typeof(SeedMachine);
            this.maxAnimationIndex = 5;
            this.millisecondsBetweenAnimation = 1000;
        }
    }
}
