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
    using StardewValley.Characters;
    using System;

    internal class Feeding
    {
        public const int eatingEmote = Character.happyEmote;

        public static bool CheckHorseFeeding(HorseOverhaul mod, Farmer who, HorseWrapper horseW, Item currentItem)
        {
            if (currentItem == null)
            {
                return false;
            }

            if (mod.Config.NewFoodSystem)
            {
                var potentialhorseFood = HorseFoodData.ClassifyHorseFood(currentItem);

                if (potentialhorseFood.IsHorseFood)
                {
                    int friendship = potentialhorseFood.FriendshipOnFeed;

                    Feeding.FeedHorse(mod, who, horseW, currentItem, friendship);

                    return true;
                }
                else if (potentialhorseFood.ReplyOnFeed != null)
                {
                    Game1.drawObjectDialogue(mod.Helper.Translation.Get(potentialhorseFood.ReplyOnFeed));

                    return false;
                }

                // don't return here so we can fall into the saddle bag case
            }
            else if (FoodData.IsGenericEdible(currentItem))
            {
                int friendship = FoodData.CalculateGenericFriendshipGain(currentItem, horseW.Friendship);

                Feeding.FeedHorse(mod, who, horseW, currentItem, friendship);

                return true;
            }

            return false;
        }

        public static void FeedHorse(HorseOverhaul mod, Farmer who, HorseWrapper horseW, Item currentItem, int friendship)
        {
            if (horseW.GotFed && !mod.Config.AllowMultipleFeedingsADay)
            {
                Game1.drawObjectDialogue(mod.Helper.Translation.Get("AteEnough", new { name = horseW.Horse.GetNPCNameForDisplay(mod) }));
            }
            else
            {
                string translation = mod.Helper.Translation.Get("AteFood", new { name = horseW.Horse.GetNPCNameForDisplay(mod), foodName = currentItem.DisplayName });

                if (mod.Config.NewFoodSystem)
                {
                    string translationKey;
                    if (friendship <= 5)
                    {
                        translationKey = "AteFoodDislike";
                    }
                    else if (friendship >= 15)
                    {
                        translationKey = "AteFoodLove";
                    }
                    else
                    {
                        translationKey = "AteFoodLike";
                    }

                    translation += " " + mod.Helper.Translation.Get(translationKey);
                }

                Game1.drawObjectDialogue(translation);

                horseW.Horse.doEmote(eatingEmote);

                who.reduceActiveItemByOne();

                horseW.JustGotFood(friendship);
            }
        }

        public static bool CheckPetFeeding(HorseOverhaul mod, Farmer who, Pet pet, Item currentItem)
        {
            if (currentItem == null)
            {
                return false;
            }

            if (mod.Config.NewFoodSystem)
            {
                if (FoodData.IsDairyProduct(currentItem))
                {
                    Game1.drawObjectDialogue(mod.Helper.Translation.Get("LactoseIntolerantPets"));

                    return false;
                }
                else if (FoodData.IsChocolate(currentItem))
                {
                    Game1.drawObjectDialogue(mod.Helper.Translation.Get("DontFeedChocolate"));

                    return false;
                }
                else if (PetFoodData.IsPetFood(currentItem))
                {
                    int friendship = PetFoodData.CalculatePetFriendshipGain(currentItem);

                    FeedPet(mod, who, pet, currentItem, friendship);

                    return true;
                }
            }
            else if (FoodData.IsGenericEdible(currentItem))
            {
                int friendship = FoodData.CalculateGenericFriendshipGain(currentItem, pet.friendshipTowardFarmer.Value);

                FeedPet(mod, who, pet, currentItem, friendship);

                return true;
            }

            return false;
        }

        public static void FeedPet(HorseOverhaul mod, Farmer who, Pet pet, Item currentItem, int friendship)
        {
            if (pet?.modData?.ContainsKey($"{mod.ModManifest.UniqueID}/gotFed") == true && !mod.Config.AllowMultipleFeedingsADay)
            {
                Game1.drawObjectDialogue(mod.Helper.Translation.Get("AteEnough", new { name = pet.GetNPCNameForDisplay(mod) }));
            }
            else
            {
                pet.modData.Add($"{mod.ModManifest.UniqueID}/gotFed", "fed");

                Game1.drawObjectDialogue(mod.Helper.Translation.Get("AteFood", new { name = pet.GetNPCNameForDisplay(mod), foodName = currentItem.DisplayName }));

                pet.doEmote(eatingEmote);

                who.reduceActiveItemByOne();

                pet.friendshipTowardFarmer.Value = Math.Min(1000, pet.friendshipTowardFarmer.Value + friendship);
            }
        }
    }
}