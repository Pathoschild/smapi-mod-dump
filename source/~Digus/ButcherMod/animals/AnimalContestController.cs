using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals.data;
using Harmony;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace AnimalHusbandryMod.animals
{
    public class AnimalContestController : AnimalStatusController
    {
        public static readonly IList<string> ContestDays = new ReadOnlyCollection<string>(new List<string>() { "26 spring", "26 fall" }) ;

        public static bool IsParticipant(FarmAnimal farmAnimal)
        {
            return GetAnimalStatus(farmAnimal.myID.Value).DayParticipatedContest != null;
        }

        public static bool HasParticipated(FarmAnimal farmAnimal)
        {
            return GetAnimalStatus(farmAnimal.myID.Value).HasWon != null;
        }

        public static bool HasWon(FarmAnimal farmAnimal)
        {
            return GetAnimalStatus(farmAnimal.myID.Value).HasWon??false;
        }

        public static bool CanChangeParticipant(FarmAnimal farmAnimal)
        {
            return CanChangeParticipant(farmAnimal.myID.Value);
        }

        public static bool CanChangeParticipantPet()
        {
            return CanChangeParticipant(AnimalData.PetId);
        }

        private static bool CanChangeParticipant(long id)
        {
            return GetAnimalStatus(id).DayParticipatedContest > SDate.Now();
        }

        public static SDate GetParticipantDate(FarmAnimal farmAnimal)
        {
            return GetAnimalStatus(farmAnimal.myID.Value).DayParticipatedContest;
        }

        public static void MakeAnimalParticipant(FarmAnimal farmAnimal)
        {
            MakeAnimalParticipant(farmAnimal.myID.Value);
        }

        public static void MakePetParticipant()
        {
            MakeAnimalParticipant(AnimalData.PetId);
        }

        private static void MakeAnimalParticipant(long id)
        {
            AnimalStatus animalStatus = GetAnimalStatus(id);
            animalStatus.DayParticipatedContest = GetNextContestDate();
        }

        public static void RemoveAnimalParticipant(FarmAnimal farmAnimal)
        {
            RemoveAnimalParticipant(farmAnimal.myID.Value);
        }

        public static void RemovePetParticipant()
        {
            RemoveAnimalParticipant(AnimalData.PetId);
        }

        private static void RemoveAnimalParticipant(long id)
        {
            AnimalStatus animalStatus = GetAnimalStatus(id);
            animalStatus.DayParticipatedContest = null;
        }

        public static SDate GetNextContestDate()
        {
            return ContestDays
                .Select(d => new SDate(Convert.ToInt32(d.Split(' ')[0]), d.Split(' ')[1]))
                .Where(d => d >= SDate.Now()).OrderBy(d => d)
                .DefaultIfEmpty(new SDate(Convert.ToInt32(ContestDays[0].Split(' ')[0]), ContestDays[0].Split(' ')[1]))
                .FirstOrDefault();
        }
    }
}
