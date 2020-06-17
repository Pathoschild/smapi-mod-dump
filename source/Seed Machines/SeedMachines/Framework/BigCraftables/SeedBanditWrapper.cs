using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedMachines.Framework.BigCraftables
{
    class SeedBanditWrapper : IBigCraftableWrapper
    {
        public SeedBanditWrapper()
        {
            this.relativeID = 6;
            this.name = "Seed Bandit";
            this.price = ModEntry.settings.seedBanditPrice;
            this.availableOutdoors = true;
            this.availableIndoors = true;
            this.fragility = 0;
            this.typeAndCategory = "Crafting -9";

            this.ingridients = ModEntry.settings.seedBanditIngredients;
            this.unlockConditions = "null";

            this.dynamicObjectType = typeof(SeedBandit);
            this.maxAnimationIndex = 3;
            this.millisecondsBetweenAnimation = 1000;
        }
    }
}
