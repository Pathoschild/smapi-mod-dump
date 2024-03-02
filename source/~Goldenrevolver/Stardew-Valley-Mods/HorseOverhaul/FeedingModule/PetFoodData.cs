/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace HorseOverhaul
{
    using StardewValley;
    using StardewObject = StardewValley.Object;

    internal class PetFoodData : FoodData
    {
        protected const string SeaCucumberID = "(O)154";
        protected const string SuperCucumberID = "(O)155";

        // meat or fish
        public static bool IsPetFood(Item item)
        {
            return IsEdibleMeat(item) || IsEdibleFish(item);
        }

        public static int CalculatePetFriendshipGain(Item item)
        {
            if (IsEdibleFish(item))
            {
                if (item.HasContextTag("fish_legendary"))
                {
                    return 50;
                }
                else if (item.HasContextTag("fish_night_market"))
                {
                    return 22;
                }
                else if (item.HasContextTag("fish_semi_rare"))
                {
                    return 14;
                }
                else
                {
                    return 8;
                }
            }

            if (item.QualifiedItemId == BugMeatID)
            {
                return 8;
            }
            else if (item.Category == StardewObject.meatCategory)
            {
                return 16;
            }

            return 0;
        }

        // not toxic, not crab pot fish, not sea or super cucumber
        public static bool IsEdibleFish(Item item)
        {
            return item.healthRecoveredOnConsumption() > 0
                && item.Category == StardewObject.FishCategory
                && !item.HasContextTag("fish_crab_pot")
                && item.QualifiedItemId != SeaCucumberID && item.QualifiedItemId != SuperCucumberID;
        }
    }
}