
using System.Collections.Generic;

namespace AutoAnimalDoors.StardewValleyWrapper.Buildings
{
    public enum AnimalDoorState { OPEN, CLOSED };

    class AnimalBuilding : Building
    {
        public AnimalBuilding(StardewValley.Buildings.Building building) :
            base(building)
        {

        }

        protected StardewValley.AnimalHouse Indoors
        {
            get
            {
                return this.building.indoors.Get() as StardewValley.AnimalHouse;
            }
        }

        public List<Animals.FarmAnimal> FarmAnimals
        {
            get
            {
                List<Animals.FarmAnimal> farmAnimals = new List<Animals.FarmAnimal>();
                foreach (StardewValley.FarmAnimal stardewValleyFarmAnimal in Indoors.animals.Values)
                {
                    farmAnimals.Add(new Animals.FarmAnimal(stardewValleyFarmAnimal));
                }
                return farmAnimals;
            }
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
    }
}
