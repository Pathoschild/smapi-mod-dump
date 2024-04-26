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
    public class Coop : AnimalBuilding
    {
        public Coop(StardewValley.Buildings.Building coop, Farm farm) :
            base(coop, farm)
        {
        }

        public override AnimalBuildingType Type => AnimalBuildingType.COOP;

        public override int UpgradeLevel
        {
            get
            {
                switch (building.buildingType.Value.ToLower())
                {
                    case "coop":
                        return 1;
                    case "big coop":
                        return 2;
                    case "deluxe coop":
                        return 3;
                }

                return 4;
            }
        }
    }
}
