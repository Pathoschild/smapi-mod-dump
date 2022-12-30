/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/taggartaa/AutoAnimalDoors
**
*************************************************/

using AutoAnimalDoors.Config;
using System.Collections.Generic;

namespace AutoAnimalDoors.StardewValleyWrapper.Buildings
{
    public enum AnimalDoorState { OPEN, CLOSED };

    public enum AnimalBuildingType { BARN, COOP, OTHER };

    public abstract class AnimalBuilding : Building
    {
        private Farm Farm { get; }

        public AnimalBuilding(StardewValley.Buildings.Building building, Farm farm) :
            base(building)
        {
            this.Farm = farm;
        }

        protected StardewValley.AnimalHouse Indoors
        {
            get
            {
                StardewValley.GameLocation indoors = this.building.indoors.Get();
                if (indoors is StardewValley.AnimalHouse)
                {
                    return indoors as StardewValley.AnimalHouse;
                }

                Logger.Instance.Log(
                    string.Format("Animal building {0} has invalid indoor type {1}, name of inddors {2}", this.building, indoors, this.building?.nameOfIndoors),
                    StardewModdingAPI.LogLevel.Warn);
                return new StardewValley.AnimalHouse();
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
                if (animal?.myID?.Value == null)
                {
                    Logger.Instance.Log("Animal is unrecognized, can't warp that one home!", StardewModdingAPI.LogLevel.Warn);
                }
                // Only warp home animals that are still on the farm
                else if (this.Farm.StardewValleyFarm.animals.ContainsKey(animal.myID.Value))
                {
                    animal.warpHome(this.Farm.StardewValleyFarm, animal);
                }
            }
        }

        public bool AreAllAnimalsHome()
        {
            if (Indoors?.animalsThatLiveHere?.Count == null || Indoors?.animals == null)
            {
                Logger.Instance.Log(string.Format("Something is wrong with this animal building, skipping it: "), StardewModdingAPI.LogLevel.Warn);
            }
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
            if (this.building?.animalDoor?.Value != null && !this.building.isUnderConstruction())
            {

                PlayDoorSound();

                building.animalDoorOpen.Value = !building.animalDoorOpen.Value;
                AnimateDoorStateChange();
            }
        }

        private void PlayDoorSound()
        {
            DoorSoundSetting doorSoundSetting = ModConfig.Instance.DoorSoundSetting;
            if (doorSoundSetting == DoorSoundSetting.ALWAYS_ON ||
                doorSoundSetting == DoorSoundSetting.ONLY_ON_FARM && StardewValley.Game1.player.currentLocation.IsFarm)
            {
                if (!building.animalDoorOpen.Value)
                {
                    StardewValley.Game1.player.currentLocation.playSound("doorCreak");
                }
                else
                {
                    StardewValley.Game1.player.currentLocation.playSound("doorCreakReverse");
                }
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
