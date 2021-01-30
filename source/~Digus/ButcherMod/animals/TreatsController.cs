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
using AnimalHusbandryMod.animals.data;
using AnimalHusbandryMod.common;
using AnimalHusbandryMod.farmer;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;
using Object = StardewValley.Object;

namespace AnimalHusbandryMod.animals
{
    public class TreatsController
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

        public static bool IsLikedTreat(Character character, int itemId)
        {
            try
            {
                if (character is Pet)
                {
                    return DataLoader.AnimalData.Pet.LikedTreatsId.Contains(itemId);
                }
                else if (character is FarmAnimal farmAnimal)
                {

                    return GetTreatItem(farmAnimal).LikedTreatsId.Contains(itemId);
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return false;
        }

        public static bool IsReadyForTreat(Character character)
        {
            return DaysUntilNextTreat(character) <= 0;
        }

        public static int DaysUntilNextTreat(Character character)
        {
            try
            {
                if (character is Pet)
                {
                    return DaysUntilNextTreat(character, DataLoader.AnimalData.Pet);
                }
                else if (character is FarmAnimal farmAnimal)
                {
                    return DaysUntilNextTreat(farmAnimal, GetTreatItem(farmAnimal));
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return 0;
        }

        public static int DaysUntilNextTreat(Character character, TreatItem treatItem)
        {
            SDate lastDayFeedTreat = character.GetLastDayFeedTreat();
            if (lastDayFeedTreat == null)
            {
                return 0;
            }
            return lastDayFeedTreat.DaysSinceStart + treatItem.MinimumDaysBetweenTreats - SDate.Now().DaysSinceStart;
        }

        public static void FeedAnimalTreat(Character character, Object treat)
        {
            character.SetLastDayFeedTreat(SDate.Now());

            int quantity = character.GetFeedTreatsQuantity(treat.ParentSheetIndex);
            character.SetFeedTreatsQuantity(treat.ParentSheetIndex, quantity+1);
        }

        public static TreatItem GetTreatItem(FarmAnimal farmAnimal)
        {
            return DataLoader.AnimalData.GetAnimalItem(farmAnimal) as TreatItem;
        }

    }
}
