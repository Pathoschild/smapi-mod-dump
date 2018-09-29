using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.farmer;

namespace AnimalHusbandryMod.animals
{
    public class AnimalStatusController
    {
        protected static AnimalStatus GetAnimalStatus(long id)
        {
            AnimalStatus animalStatus = FarmerLoader.FarmerData.AnimalData.Find(s => s.Id == id);
            if (animalStatus == null)
            {
                animalStatus = new AnimalStatus(id);
                FarmerLoader.FarmerData.AnimalData.Add(animalStatus);
            }

            return animalStatus;
        }
    }
}
