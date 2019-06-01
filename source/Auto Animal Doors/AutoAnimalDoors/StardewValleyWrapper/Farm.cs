using System.Collections.Generic;

namespace AutoAnimalDoors.StardewValleyWrapper
{
    class Farm
    {
        public StardewValley.Farm StardewValleyFarm { get; private set; }

        public Farm(StardewValley.Farm farm)
        {
            this.StardewValleyFarm = farm;
        }

        public List<Buildings.Building> Buildings
        {
            get
            {
                List<Buildings.Building> buildings = new List<Buildings.Building>();

                if (this.StardewValleyFarm != null)
                {
                    foreach (StardewValley.Buildings.Building stardewBuilding in this.StardewValleyFarm.buildings)
                    {
                        if (stardewBuilding is StardewValley.Buildings.Barn || stardewBuilding is StardewValley.Buildings.Coop)
                        {
                            buildings.Add(new Buildings.AnimalBuilding(stardewBuilding, this));
                        }
                        else
                        {
                            buildings.Add(new Buildings.Building(stardewBuilding));
                        }
                    }
                }

                return buildings;
            }
        }

        public List<Buildings.AnimalBuilding> AnimalBuildings
        {
            get
            {
                List<Buildings.AnimalBuilding> animalBuildings = new List<Buildings.AnimalBuilding>();
                foreach (Buildings.Building building in Buildings)
                {
                    Buildings.AnimalBuilding animalBuilding = building as Buildings.AnimalBuilding;
                    if (animalBuilding != null)
                    {
                        animalBuildings.Add(animalBuilding);
                    }
                }

                return animalBuildings;
            }
        }

        public bool AreAllAnimalsHome()
        {
            foreach (Buildings.AnimalBuilding building in this.AnimalBuildings)
            {
                if (!building.AreAllAnimalsHome())
                {
                    return false;
                }
            }

            return true;
        }

        public void SendAllAnimalsHome()
        {
            foreach (Buildings.AnimalBuilding building in this.AnimalBuildings)
            {
                building.SendAllAnimalsHome();
            }
        }

        public void SetAnimalDoorsState(Buildings.AnimalDoorState state)
        {
            foreach (Buildings.AnimalBuilding animalBuilding in this.AnimalBuildings)
            {
                animalBuilding.AnimalDoorState = state;
            }
        }
    }
}
