/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/taggartaa/AutoAnimalDoors
**
*************************************************/

using System.Collections.Generic;

namespace AutoAnimalDoors.StardewValleyWrapper.Buildings
{
    public enum AnimalDoorState { OPEN, CLOSED };

    public enum AnimalBuildingType { BARN, COOP, OTHER };

    class AnimalBuilding : Building
    {
        private Farm Farm { get; set; }

        public AnimalBuilding(StardewValley.Buildings.Building building, Farm farm) :
            base(building)
        {
            this.Farm = farm;
        }

        protected StardewValley.AnimalHouse Indoors
        {
            get
            {
                return this.building.indoors.Get() as StardewValley.AnimalHouse;
            }
        }

        private List<StardewValley.FarmAnimal> FarmAnimals
        {
            get
            {
                List<StardewValley.FarmAnimal> farmAnimals = new List<StardewValley.FarmAnimal>();
                foreach (long id in Indoors.animalsThatLiveHere)
                {
                    farmAnimals.Add(StardewValley.Utility.getAnimal(id));
                }
                return farmAnimals;
            }
        }

        public void SendAllAnimalsHome()
        {
            this.CloseAnimalDoor();
            foreach (StardewValley.FarmAnimal animal in FarmAnimals)
            {
                // Only warp home animals that are still on the farm
                if (this.Farm.StardewValleyFarm.animals.ContainsKey(animal.myID.Value))
                {
                    animal.warpHome(this.Farm.StardewValleyFarm, animal);
                }
            }
        }

        public bool AreAllAnimalsHome()
        {
            return Indoors.animalsThatLiveHere.Count == Indoors.animals.Count();
        }

        public AnimalDoorState AnimalDoorState
        {
            get
            {
                return building.animalDoorOpen.Value ? AnimalDoorState.OPEN : AnimalDoorState.CLOSED;
            }

            set
            {
                if (value != AnimalDoorState)
                {
                    ToggleAnimalDoorState();
                }
            }
        }

        public void ToggleAnimalDoorState()
        {
            if (this.building.animalDoor.Value != null && !this.building.isUnderConstruction())
            {
                int xPositionOfAnimalDoor = this.building.animalDoor.X + this.building.tileX.Value;
                int yPositionOfAnimalDoor = this.building.animalDoor.Y + this.building.tileY.Value;

                this.building.doAction(new Microsoft.Xna.Framework.Vector2(xPositionOfAnimalDoor, yPositionOfAnimalDoor), StardewValley.Game1.player);
            }
        }

        public void OpenAnimalDoor()
        {
            if (!this.building.animalDoorOpen.Value)
            {
                this.ToggleAnimalDoorState();
            }
        }

        public void CloseAnimalDoor()
        {
            if (this.building.animalDoorOpen.Value)
            {
                this.ToggleAnimalDoorState();
            }
        }

        public AnimalBuildingType Type
        {
            get
            {
                string buildingTypeString = this.building.buildingType.Value.ToLower();
                if (buildingTypeString.Contains("coop"))
                {
                    return AnimalBuildingType.COOP;
                } else if (buildingTypeString.Contains("barn"))
                {
                    return AnimalBuildingType.BARN;
                }

                return AnimalBuildingType.OTHER;
            }
        }

        public int UpgradeLevel
        {
            get
            {
                switch (this.building.buildingType.Value.ToLower())
                {
                    case "coop":
                    case "barn":
                        return 1;
                    case "big coop":
                    case "big barn":
                        return 2;
                    case "deluxe coop":
                    case "deluxe barn":
                        return 3;
                }

                return 4;
            }
        }

    }
}
