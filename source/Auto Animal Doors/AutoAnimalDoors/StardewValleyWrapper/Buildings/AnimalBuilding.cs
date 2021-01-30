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

    abstract class AnimalBuilding : Building
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

                if (!building.animalDoorOpen.Value)
                {
                    StardewValley.Game1.player.currentLocation.playSound("doorCreak");
                }
                else
                {
                    StardewValley.Game1.player.currentLocation.playSound("doorCreakReverse");
                }

                building.animalDoorOpen.Value = !building.animalDoorOpen.Value;
                AnimateDoorStateChange();
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

        abstract protected void AnimateDoorStateChange();

        abstract public AnimalBuildingType Type { get; }

        abstract public int UpgradeLevel { get; }

    }
}
