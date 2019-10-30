using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals.data;
using AnimalHusbandryMod.common;
using AnimalHusbandryMod.farmer;
using StardewModdingAPI.Utilities;
using StardewValley;
using Object = StardewValley.Object;

namespace AnimalHusbandryMod.animals
{
    public class TreatsController : AnimalStatusController
    {
        public static bool CanReceiveTreat(FarmAnimal farmAnimal)
        {
            try
            {
                return GetTreatItem(farmAnimal) != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsLikedTreat(int id)
        {
            return DataLoader.AnimalData.Chicken.LikedTreatsId.Contains(id)
                || DataLoader.AnimalData.Duck.LikedTreatsId.Contains(id)
                || DataLoader.AnimalData.Rabbit.LikedTreatsId.Contains(id)
                || DataLoader.AnimalData.Cow.LikedTreatsId.Contains(id)
                || DataLoader.AnimalData.Goat.LikedTreatsId.Contains(id)
                || DataLoader.AnimalData.Sheep.LikedTreatsId.Contains(id)
                || DataLoader.AnimalData.Pig.LikedTreatsId.Contains(id)
                || DataLoader.AnimalData.Pet.LikedTreatsId.Contains(id)
                || DataLoader.AnimalData.Dinosaur.LikedTreatsId.Contains(id)
                || DataLoader.AnimalData.CustomAnimals.Exists(c=> c.LikedTreatsId.Contains(id))
                ;
        }

        public static bool IsLikedTreatPet(int itemId)
        {
            return DataLoader.AnimalData.Pet.LikedTreatsId.Contains(itemId);
        }

        public static bool IsLikedTreat(FarmAnimal farmAnimal, int itemId)
        {
            try
            {
                return GetTreatItem(farmAnimal).LikedTreatsId.Contains(itemId);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsReadyForTreatPet()
        {
            return DaysUntilNextTreatPet() <= 0;
        }

        public static bool IsReadyForTreat(FarmAnimal farmAnimal)
        {
            return DaysUntilNextTreat(farmAnimal) <= 0;
        }

        public static int DaysUntilNextTreatPet()
        {
            return DaysUntilNextTreat(AnimalData.PetId, DataLoader.AnimalData.Pet);
        }

        public static int DaysUntilNextTreat(FarmAnimal farmAnimal)
        {
            try
            {
                return DaysUntilNextTreat(farmAnimal.myID.Value, GetTreatItem(farmAnimal));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static int DaysUntilNextTreat(long id, TreatItem treatItem)
        {
            if (GetAnimalStatus(id).LastDayFeedTreat == null)
            {
                return 0;
            }
            return GetAnimalStatus(id).LastDayFeedTreat.DaysSinceStart + treatItem.MinimumDaysBetweenTreats - SDate.Now().DaysSinceStart;
        }

        public static void FeedAnimalTreat(FarmAnimal farmAnimal, Object treat)
        {
            FeedAnimalTreat(farmAnimal.myID.Value, treat);
        }

        public static void FeedPetTreat(Object treat)
        {
            FeedAnimalTreat(AnimalData.PetId, treat);
        }

        private static void FeedAnimalTreat(long id, Object treat)
        {
            AnimalStatus animalStatus = GetAnimalStatus(id);
            animalStatus.LastDayFeedTreat = SDate.Now();
            if (!animalStatus.FeedTreatsQuantity.ContainsKey(treat.ParentSheetIndex))
            {
                animalStatus.FeedTreatsQuantity[treat.ParentSheetIndex] = 0;
            }
            animalStatus.FeedTreatsQuantity[treat.ParentSheetIndex]++;
        }

        public static TreatItem GetTreatItem(FarmAnimal farmAnimal)
        {
            return DataLoader.AnimalData.GetAnimalItem(farmAnimal) as TreatItem;
        }

    }
}
