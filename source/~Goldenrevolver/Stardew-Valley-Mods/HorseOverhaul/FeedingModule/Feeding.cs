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
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.Characters;
    using System;
    using System.Linq;

    internal class Feeding
    {
        public static bool CheckHorseInteraction(HorseOverhaul mod, Farmer who, int mouseX, int mouseY, bool ignoreMousePosition)
        {
            if (who.currentLocation == null)
            {
                return false;
            }

            foreach (Horse horse in who.currentLocation.characters.OfType<Horse>())
            {
                if (horse == null || horse.IsTractor())
                {
                    continue;
                }

                // check if the interaction was a mouse click on a horse or a button press near a horse
                if (!Utility.withinRadiusOfPlayer((int)horse.Position.X, (int)horse.Position.Y, 1, who)
                    || !horse.MouseOrPlayerIsInRange(who, mouseX, mouseY, ignoreMousePosition))
                {
                    continue;
                }

                var horseW = mod.Horses.Where(h => h?.Horse?.HorseId == horse.HorseId).FirstOrDefault();

                if (horseW == null)
                {
                    continue;
                }

                if (who.CurrentItem != null && mod.Config.Feeding)
                {
                    Item currentItem = who.CurrentItem;

                    if (currentItem.QualifiedItemId == "(O)Carrot")
                    {
                        // don't combine the if statements, so we can fall into the saddle bag case

                        // prevent feeding, use for speed boost instead
                        if (!horse.ateCarrotToday)
                        {
                            return horse.checkAction(who, who.currentLocation);
                        }
                    }
                    else if (mod.Config.NewFoodSystem)
                    {
                        var potentialhorseFood = HorseFoodData.ClassifyHorseFood(currentItem);

                        if (potentialhorseFood.IsHorseFood)
                        {
                            int friendship = potentialhorseFood.FriendshipOnFeed;

                            FeedHorse(mod, who, horseW, currentItem, friendship);

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

                        FeedHorse(mod, who, horseW, currentItem, friendship);

                        return true;
                    }
                }

                if (Context.IsWorldReady && Context.CanPlayerMove && Context.IsPlayerFree && mod.Config.SaddleBag)
                {
                    if (horseW.SaddleBag != null)
                    {
                        horseW.SaddleBag.ShowMenu();

                        return true;
                    }
                }
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

                if (mod.Config.ThinHorse)
                {
                    horseW.Horse.doEmote(Character.happyEmote);
                }

                who.reduceActiveItemByOne();

                horseW.JustGotFood(friendship);
            }
        }

        public static bool CheckPetInteraction(HorseOverhaul mod, Farmer who, int mouseX, int mouseY, bool ignoreMousePosition)
        {
            if (!mod.Config.PetFeeding)
            {
                return false;
            }

            if (who.currentLocation == null)
            {
                return false;
            }

            foreach (NPC npc in who.currentLocation.characters)
            {
                if (npc is not Pet pet)
                {
                    continue;
                }

                // check if the interaction was a mouse click on a pet or a button press near a pet
                if (!Utility.withinRadiusOfPlayer((int)pet.Position.X, (int)pet.Position.Y, 1, who)
                    || !pet.MouseOrPlayerIsInRange(who, mouseX, mouseY, ignoreMousePosition))
                {
                    continue;
                }

                return CheckPetFeeding(mod, who, pet, who.CurrentItem);
            }

            return false;
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

                pet.doEmote(Character.happyEmote);

                who.reduceActiveItemByOne();

                pet.friendshipTowardFarmer.Value = Math.Min(1000, pet.friendshipTowardFarmer.Value + friendship);
            }
        }
    }
}