/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/taggartaa/AutoAnimalDoors
**
*************************************************/

using Netcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoAnimalDoors.StardewValleyWrapper.Buildings
{
    class Barn : AnimalBuilding
    {
        private StardewValley.Buildings.Barn StardewBarn { get; set; }

        public Barn(StardewValley.Buildings.Barn barn, Farm farm) :
            base(barn, farm)
        {
            StardewBarn = barn;
        }

        public override AnimalBuildingType Type => AnimalBuildingType.BARN;

        public override int UpgradeLevel
        {
            get
            {
                switch (this.building.buildingType.Value.ToLower())
                {
                    case "barn":
                        return 1;
                    case "big barn":
                        return 2;
                    case "deluxe barn":
                        return 3;
                }

                return 4;
            }
        }

        protected override void AnimateDoorStateChange()
        {
            // Need to use reflection to animate the door changing state because it is private
            var prop = StardewBarn.GetType().GetField("animalDoorMotion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            NetInt animalDoorMotion = prop.GetValue(StardewBarn) as NetInt;
            animalDoorMotion.Value = StardewBarn.animalDoorOpen.Value ? (-3) : 2;
        }
    }
}
