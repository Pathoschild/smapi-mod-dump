/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/taggartaa/AutoAnimalDoors
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoAnimalDoors.StardewValleyWrapper.Buildings
{
    class UnrecognizedAnimalBuilding : AnimalBuilding
    {
        public UnrecognizedAnimalBuilding(StardewValley.Buildings.Building building, Farm farm) : base(building, farm)
        {
        }

        public override AnimalBuildingType Type => AnimalBuildingType.OTHER;

        public override int UpgradeLevel => 1;

        protected override void AnimateDoorStateChange()
        {

        }
    }
}
