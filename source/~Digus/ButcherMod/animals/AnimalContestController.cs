/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

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
using StardewValley.Buildings;
using StardewValley.Characters;

namespace AnimalHusbandryMod.animals
{
    public class AnimalContestController
    {

        private const string AnimalContestCountKey = "DIGUS.ANIMALHUSBANDRYMOD/AnimalContest.Count";

        private static FarmAnimal _temporaryFarmAnimal = null;

        public static bool IsNextParticipant(Character character)
        {
            SDate dayParticipatedContest = character.GetDayParticipatedContest();
            return dayParticipatedContest != null && dayParticipatedContest > SDate.Now();
        }

        public static bool HasParticipated(Character character)
        {
            SDate dayParticipatedContest = character.GetDayParticipatedContest();
            return dayParticipatedContest != null && dayParticipatedContest <= SDate.Now();
        }

        public static bool HasWon(Character character)
        {
            return character.GetHasWon()??false;
        }

        public static bool CanChangeParticipant(Character character)
        {
            return character.GetDayParticipatedContest() > SDate.Now();
        }

        public static void MakeAnimalParticipant(Character character)
        {
            character.SetDayParticipatedContest(GetNextContestDate());
        }

        public static void RemoveAnimalParticipant(Character character)
        {
            character.ClearDayParticipatedContest();
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

        public static Character GetNextContestParticipant()
        {
            return ContestParticipant(GetNextContestDate());
        }

        public static Character ContestParticipant(SDate contestDate)
        {
            Pet pet = Game1.player.getPet();
            return pet.GetDayParticipatedContest() == contestDate
                ? (Character) pet
                : AnimalUtility.FindAnimals(a => a.GetDayParticipatedContest() == contestDate).FirstOrDefault();
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
                    Character participant = null;
                    if (participantIdValue != AnimalData.PetId)
                    {
                        if (_temporaryFarmAnimal != null)
                        {
                            participant = _temporaryFarmAnimal;
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
                    }
                    else
                    {
                        participant = Game1.player.getPet();
                        if (participated)
                        {
                            Pet pet = (Pet) participant;
                            pet.friendshipTowardFarmer.Value = Math.Min(Pet.maxFriendship, pet.friendshipTowardFarmer.Value + DataLoader.AnimalContestData.PetFriendshipForParticipating);
                        }
                        AnimalContestController.ReAddPet();
                    }
                    participant?.SetHasWon((participant.GetHasWon() ?? false) || animalContestItem.Winner == "Farmer");
                }
            }
        }

        public static void TemporallyRemoveFarmAnimal(FarmAnimal farmAnimal)
        {
            if(Context.IsMainPlayer)
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
        }

        public static void TemporallyRemovePet()
        {
            if (Context.IsMainPlayer)
            {
                Game1.player.getPet().IsInvisible = true;
            }
        }

        public static void ReAddFarmAnimal(long participantIdValue)
        {
            if (Context.IsMainPlayer)
            {
                if (_temporaryFarmAnimal != null && _temporaryFarmAnimal.myID.Value == participantIdValue)
                {
                    (_temporaryFarmAnimal.home.indoors.Value as AnimalHouse)?.animals.Add(
                        _temporaryFarmAnimal.myID.Value, _temporaryFarmAnimal);
                }

                _temporaryFarmAnimal = null;
            }
        }

        public static void ReAddPet()
        {
            if (Context.IsMainPlayer)
            {
                Game1.player.getPet().IsInvisible = false;
            }
        }

        public static void CleanTemporaryParticipant()
        {
            if (Context.IsMainPlayer)
            {
                _temporaryFarmAnimal = null;
            }
        }

        public static bool HasFertilityBonus(FarmAnimal farmAnimal)
        {
            return HasWon(farmAnimal) && new[]{ "spring" , "summer"}.Contains(farmAnimal.GetDayParticipatedContest()?.Season);
        }

        public static bool HasProductionBonus(FarmAnimal farmAnimal)
        {
            return HasWon(farmAnimal) && new[] { "fall", "winter" }.Contains(farmAnimal.GetDayParticipatedContest()?.Season);
        }

        public static void UpdateContestCount()
        {
            Game1.getFarm().modData[AnimalContestCountKey] = FarmerLoader.FarmerData?.AnimalContestData.Count.ToString();
        }

        public static int GetContestCount()
        {
            int result = 0;
            if (Game1.getFarm().modData.ContainsKey(AnimalContestCountKey))
            {
                int.TryParse(Game1.getFarm().modData[AnimalContestCountKey], out result);
            }
            return result;
            
        }
    }
}
