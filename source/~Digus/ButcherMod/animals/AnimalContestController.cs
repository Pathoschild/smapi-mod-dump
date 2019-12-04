using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals.data;
using AnimalHusbandryMod.common;
using AnimalHusbandryMod.farmer;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;

namespace AnimalHusbandryMod.animals
{
    public class AnimalContestController : AnimalStatusController
    {
        private static FarmAnimal _temporaryFarmAnimal = null;

        public static bool IsParticipant(FarmAnimal farmAnimal)
        {
            return GetAnimalStatus(farmAnimal.myID.Value).DayParticipatedContest != null;
        }

        public static bool IsParticipantPet()
        {
            SDate dayParticipatedContest = GetAnimalStatus(AnimalData.PetId).DayParticipatedContest;
            return dayParticipatedContest != null && dayParticipatedContest > SDate.Now();
        }

        public static bool HasParticipated(FarmAnimal farmAnimal)
        {
            SDate dayParticipatedContest = GetAnimalStatus(farmAnimal.myID.Value).DayParticipatedContest;
            return dayParticipatedContest != null && dayParticipatedContest <= SDate.Now();
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

        public static bool IsContestDate()
        {
            return DataLoader.AnimalContestData.ContestDays
                .Select(d => new SDate(Convert.ToInt32(d.Split(' ')[0]), d.Split(' ')[1]))
                .Any(d => d == SDate.Now());
        }

        public static SDate GetNextContestDate()
        {
            List<SDate> orderedDates = DataLoader.AnimalContestData.ContestDays
                .Select(d => new SDate(Convert.ToInt32(d.Split(' ')[0]), d.Split(' ')[1]))
                .OrderBy(d => d.DaysSinceStart).ToList();

            return orderedDates
                .Where(d => d > SDate.Now())
                .DefaultIfEmpty(orderedDates.First().AddDays(112))
                .FirstOrDefault();
        }

        public static String GetNextContestDateKey()
        {
            SDate date = GetNextContestDate();
            return $"{date.Year:00}{Utility.getSeasonNumber(date.Season)}{date.Day:00}";
        }

        public static long? GetNextContestParticipantId()
        {
            return ContestParticipantId(GetNextContestDate());
        }

        public static long? ContestParticipantId(SDate contestDate)
        {
            AnimalStatus animalStatus = FarmerLoader.FarmerData.AnimalData.Find(s => s.DayParticipatedContest == contestDate);
            return animalStatus?.Id;
        }

        public static void EndEvent(AnimalContestItem animalContestItem, bool participated = true)
        {
            if (Context.IsMainPlayer)
            {
                if (!participated)
                {
                    animalContestItem.Winner = "Marnie";
                    animalContestItem.Contenders = new List<string>(new[] { "Marnie" });
                    animalContestItem.VincentAnimal = null;
                    animalContestItem.MarnieAnimal = null;
                }
                if (animalContestItem.ParticipantId.HasValue)
                {
                    long participantIdValue = animalContestItem.ParticipantId.Value;
                    AnimalStatus animalStatus = GetAnimalStatus(participantIdValue);
                    animalStatus.HasWon = (animalStatus.HasWon ?? false) || animalContestItem.Winner == "Farmer";
                    if (participantIdValue != AnimalData.PetId)
                    {
                        if (participated)
                        {
                            _temporaryFarmAnimal.friendshipTowardFarmer.Value = Math.Min(1000, _temporaryFarmAnimal.friendshipTowardFarmer.Value + DataLoader.AnimalContestData.FarmAnimalFriendshipForParticipating);
                            _temporaryFarmAnimal.happiness.Value = 255;
                        }
                        else
                        {
                            _temporaryFarmAnimal.friendshipTowardFarmer.Value = Math.Max(0, _temporaryFarmAnimal.friendshipTowardFarmer.Value - DataLoader.AnimalContestData.FarmAnimalFriendshipForParticipating);
                            _temporaryFarmAnimal.happiness.Value = 0;
                        }
                        ReAddFarmAnimal(participantIdValue);
                    }
                    else
                    {
                        if (participated)
                        {
                            Pet pet = Game1.player.getPet();
                            pet.friendshipTowardFarmer.Value = Math.Min(Pet.maxFriendship, pet.friendshipTowardFarmer.Value + DataLoader.AnimalContestData.PetFriendshipForParticipating);
                        }
                        AnimalContestController.ReAddPet();
                    }
                }
            }
        }

        public static FarmAnimal GetAnimal(long id)
        {
            if (_temporaryFarmAnimal?.myID.Value == id)
            {
                return _temporaryFarmAnimal;
            }
            FarmAnimal animal = Utility.getAnimal(id);
            if (animal != null)
            {
                return animal;
            }
            else
            {
                AnimalHusbandryModEntry.monitor.Log($"The animal id '{id}' was not found in the game and its animal status data is being discarted.", LogLevel.Warn);
                RemoveAnimalStatus(id);
                return null;
            }
        }

        public static void TemporalyRemoveFarmAnimal(FarmAnimal farmAnimal)
        {
            if (farmAnimal.currentLocation is Farm farm)
            {
                farm.animals.Remove(farmAnimal.myID.Value);
                _temporaryFarmAnimal = farmAnimal;
            }
            else if (farmAnimal.currentLocation is AnimalHouse animalHouse)
            {
                animalHouse.animals.Remove(farmAnimal.myID.Value);
                _temporaryFarmAnimal = farmAnimal;
            }
        }

        public static void TemporalyRemovePet()
        {
            Game1.player.getPet().IsInvisible = true;
        }

        public static void ReAddFarmAnimal(long participantIdValue)
        {
            if (_temporaryFarmAnimal != null && _temporaryFarmAnimal.myID.Value == participantIdValue)
            {
                (_temporaryFarmAnimal.home.indoors.Value as AnimalHouse)?.animals.Add(_temporaryFarmAnimal.myID.Value, _temporaryFarmAnimal);
            }
            _temporaryFarmAnimal = null;
        }

        public static void ReAddPet()
        {
            Game1.player.getPet().IsInvisible = false;
        }

        public static void CleanTemporaryParticipant()
        {
            _temporaryFarmAnimal = null;
        }

        public static bool HasFertilityBonus(FarmAnimal farmAnimal)
        {
            return HasWon(farmAnimal) && new[]{ "spring" , "summer"}.Contains(GetParticipantDate(farmAnimal).Season);
        }

        public static bool HasProductionBonus(FarmAnimal farmAnimal)
        {
            return HasWon(farmAnimal) && new[] { "fall", "winter" }.Contains(GetParticipantDate(farmAnimal).Season);
        }
    }
}
