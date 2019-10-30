using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.farmer;
using StardewModdingAPI;
using StardewValley;

namespace AnimalHusbandryMod.animals
{
    public class AnimalStatusController
    {
        public static AnimalStatus GetAnimalStatus(long id)
        {
            AnimalStatus animalStatus = FarmerLoader.FarmerData.AnimalData.Find(s => s.Id == id);
            if (animalStatus == null)
            {
                animalStatus = new AnimalStatus(id);
                FarmerLoader.FarmerData.AnimalData.Add(animalStatus);
            }

            return animalStatus;
        }

        public static void RemoveAnimalStatus(long farmAnimalId)
        {
            FarmerLoader.FarmerData.AnimalData.RemoveAll(f => f.Id == farmAnimalId);
        }
    }
}
