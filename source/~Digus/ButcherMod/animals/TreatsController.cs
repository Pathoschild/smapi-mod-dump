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

        public static bool IsLikedTreat(Object item)
        {
            return IsLikedTreat(DataLoader.AnimalData.Chicken, item)
                || IsLikedTreat(DataLoader.AnimalData.Duck, item)
                || IsLikedTreat(DataLoader.AnimalData.Rabbit, item)
                || IsLikedTreat(DataLoader.AnimalData.Cow, item)
                || IsLikedTreat(DataLoader.AnimalData.Goat, item)
                || IsLikedTreat(DataLoader.AnimalData.Sheep, item)
                || IsLikedTreat(DataLoader.AnimalData.Pig, item)
                || IsLikedTreat(DataLoader.AnimalData.Pet, item)
                || IsLikedTreat(DataLoader.AnimalData.Dinosaur, item)
                || DataLoader.AnimalData.CustomAnimals.Exists(c=> IsLikedTreat(c, item))
                ;
        }

        public static bool IsLikedTreat(Character character, Object item)
        {
            try
            {
                TreatItem treatItem = null;
                switch (character)
                {
                    case Pet _:
                        treatItem = DataLoader.AnimalData.Pet;
                        break;
                    case FarmAnimal farmAnimal:
                        treatItem = GetTreatItem(farmAnimal);
                        break;
                }
                if (treatItem != null)
                {
                    return IsLikedTreat(treatItem, item);
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return false;
        }

        private static bool IsLikedTreat(TreatItem treatItem, Object item)
        {
            return treatItem.LikedTreatsId.Contains(item.ParentSheetIndex) || treatItem.LikedTreatsId.Contains(item.Category) ||
                   (item.ParentSheetIndex == 1720 && DataLoader.DgaApi != null &&
                    treatItem.LikedTreats.Contains(DataLoader.DgaApi.GetDGAItemId(item)));
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
