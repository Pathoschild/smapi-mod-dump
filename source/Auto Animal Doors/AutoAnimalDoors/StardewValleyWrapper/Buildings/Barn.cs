/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/taggartaa/AutoAnimalDoors
**
*************************************************/

namespace AutoAnimalDoors.StardewValleyWrapper.Buildings
{
    public class Barn : AnimalBuilding
    {

        public Barn(StardewValley.Buildings.Building barn, Farm farm) :
            base(barn, farm)
        {
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
                    default:
                        return 4;
                }
            }
        }
    }
}
