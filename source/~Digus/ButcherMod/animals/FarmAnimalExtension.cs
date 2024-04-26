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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace AnimalHusbandryMod.animals
{
    public static class FarmAnimalExtension
    {
        public const string DaysUntilBirthKey = "DIGUS.ANIMALHUSBANDRYMOD/Pregnancy.DaysUntilBirth";
        public const string AllowReproductionBeforeInseminationKey = "DIGUS.ANIMALHUSBANDRYMOD/Pregnancy.AllowReproductionBeforeInsemination";
        public const string LastDayFeedTreatKey = "DIGUS.ANIMALHUSBANDRYMOD/Feeding.LastDayFeedTreat";
        public const string FeedTreatsQuantityKey = "DIGUS.ANIMALHUSBANDRYMOD/Feeding.FeedTreatQuantity";
        public const string DayParticipatedContestKey = "DIGUS.ANIMALHUSBANDRYMOD/Contest.DayParticipatedContest";
        public const string HasWonKey = "DIGUS.ANIMALHUSBANDRYMOD/Contest.HasWon";

        public static int? GetDaysUntilBirth(this FarmAnimal farmAnimal)
        {
            if (int.TryParse(GetKey(farmAnimal, DaysUntilBirthKey), out int i)) return i;
            return null;
        }

        public static void SetDaysUntilBirth(this FarmAnimal farmAnimal, int value)
        {
            SetKey(farmAnimal, DaysUntilBirthKey, value);
        }

        public static bool ClearDaysUntilBirth(this FarmAnimal farmAnimal)
        {
            return ClearKey(farmAnimal, DaysUntilBirthKey); 
        }

        public static bool? GetAllowReproductionBeforeInsemination(this FarmAnimal farmAnimal)
        {
            if (bool.TryParse(GetKey(farmAnimal, AllowReproductionBeforeInseminationKey), out bool b)) return b;
            return null;
        }

        public static void SetAllowReproductionBeforeInsemination(this FarmAnimal farmAnimal, bool value)
        {
            SetKey(farmAnimal, AllowReproductionBeforeInseminationKey, value);
        }

        public static bool ClearAllowReproductionBeforeInsemination(this FarmAnimal farmAnimal)
        {
            return ClearKey(farmAnimal, AllowReproductionBeforeInseminationKey);
        }
        
        public static SDate GetLastDayFeedTreat(this Character character)
        {
            string date = GetKey(character, LastDayFeedTreatKey);
            return date == null ? null : new SDate(Convert.ToInt32(date.Split(' ')[0]), date.Split(' ')[1], Convert.ToInt32(date.Split(' ')[2].Replace("Y", "")));
  
        }

        public static void SetLastDayFeedTreat(this Character character, SDate value)
        {
            SetKey(character, LastDayFeedTreatKey, value);
        }

        public static int GetFeedTreatsQuantity(this Character character, int treatIndex)
        {
            if (int.TryParse(GetKey(character, FeedTreatsQuantityKey+$".{treatIndex}"), out int i)) return i;
            return 0;
        }

        public static void SetFeedTreatsQuantity(this Character character, int treatIndex, int value)
        {
            SetKey(character, FeedTreatsQuantityKey + $".{treatIndex}", value);
        }

        public static int GetFeedTreatsQuantityCount(this Character character)
        {
            return character.modData.Keys.Count(d => d.StartsWith(FeedTreatsQuantityKey));
        }

        public static int GetFeedTreatsQuantitySum(this Character character)
        {
            return character.modData.Pairs.Where(d=> d.Key.StartsWith(FeedTreatsQuantityKey)).Sum(d=>int.Parse(d.Value));
        }

        public static SDate GetDayParticipatedContest(this Character character)
        {
            string date = GetKey(character, DayParticipatedContestKey);
            return date == null ? null : new SDate(Convert.ToInt32(date.Split(' ')[0]), date.Split(' ')[1], Convert.ToInt32(date.Split(' ')[2].Replace("Y", "")));

        }

        public static void SetDayParticipatedContest(this Character character, SDate value)
        {
            SetKey(character, DayParticipatedContestKey, value);
        }

        public static bool ClearDayParticipatedContest(this Character character)
        {
            return ClearKey(character, DayParticipatedContestKey);
        }

        public static bool? GetHasWon(this Character farmAnimal)
        {
            if (bool.TryParse(GetKey(farmAnimal, HasWonKey), out bool b)) return b;
            return null;
        }

        public static void SetHasWon(this Character farmAnimal, bool value)
        {
            SetKey(farmAnimal, HasWonKey, value);
        }

        private static string GetKey(this Character character, string key)
        {
            if (character.modData != null && character.modData.TryGetValue(key, out string value))
                return value;
            else
                return null;
        }

        private static void SetKey(this Character character, string key, object value)
        {
            character.modData[key] = value.ToString();
        }

        private static bool ClearKey(Character character, string key )
        {
            return character.modData.Remove(key);
        }
    }
}
