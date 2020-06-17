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
            this.relativeID = 0;
            this.name = "Seed Machine";
            this.price = ModEntry.settings.seedMachinePrice;
            this.availableOutdoors = true;
            this.availableIndoors = true;
            this.fragility = 0;
            this.typeAndCategory = "Crafting -9";

            this.ingridients = ModEntry.settings.seedMachineIngredients;
            this.unlockConditions = "null";

            this.dynamicObjectType = typeof(SeedMachine);
            this.maxAnimationIndex = 5;
            this.millisecondsBetweenAnimation = 1000;
        }
    }
}
