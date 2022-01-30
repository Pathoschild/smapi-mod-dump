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
    using HarmonyLib;
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.Characters;
    using System;
    using System.Linq;

    internal class Feeding
    {
        public static bool CheckHorseInteraction(HorseOverhaul mod, GameLocation currentLocation, int mouseX, int mouseY, bool ignoreMousePosition)
        {
            foreach (Horse horse in currentLocation.characters.OfType<Horse>())
            {
                // check if the interaction was a mouse click on a horse or a button press near a horse
                if (horse != null && !horse.IsTractor() && IsInRange(horse, mouseX, mouseY, ignoreMousePosition))
                {
                    HorseWrapper horseW = null;
                    mod.Horses.Where(h => h?.Horse?.HorseId == horse.HorseId).Do(h => horseW = h);

                    if (Game1.player.CurrentItem != null && mod.Config.Feeding)
                    {
                        Item currentItem = Game1.player.CurrentItem;

                        if (mod.Config.NewFoodSystem)
                        {
                            var potentialhorseFood = HorseFoodData.ClassifyHorseFood(currentItem);

                            if (potentialhorseFood.IsHorseFood)
                            {
                                int friendship = potentialhorseFood.FriendshipOnFeed;

                                FeedHorse(mod, horseW, currentItem, friendship);

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

                            FeedHorse(mod, horseW, currentItem, friendship);

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
            }

            return false;
        }

        public static void FeedHorse(HorseOverhaul mod, HorseWrapper horseW, Item currentItem, int friendship)
        {
            if (horseW.GotFed && !mod.Config.AllowMultipleFeedingsADay)
            {
                Game1.drawObjectDialogue(mod.Helper.Translation.Get("AteEnough", new { name = horseW.Horse.displayName }));
            }
            else
            {
                string translation = mod.Helper.Translation.Get("AteFood", new { name = horseW.Horse.displayName, foodName = currentItem.DisplayName });

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

                Game1.player.reduceActiveItemByOne();

                horseW.JustGotFood(friendship);
            }
        }

        public static bool CheckPetInteraction(HorseOverhaul mod, int mouseX, int mouseY, bool ignoreMousePosition)
        {
            if (!mod.Config.PetFeeding || !Game1.player.hasPet())
            {
                return false;
            }

            Pet pet = Game1.player.getPet();

            if (pet != null && IsInRange(pet, mouseX, mouseY, ignoreMousePosition))
            {
                if (Game1.player.CurrentItem != null)
                {
                    Item currentItem = Game1.player.CurrentItem;

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

                            FeedPet(mod, pet, currentItem, friendship);

                            return true;
                        }
                    }
                    else if (FoodData.IsGenericEdible(currentItem))
                    {
                        int friendship = FoodData.CalculateGenericFriendshipGain(currentItem, pet.friendshipTowardFarmer.Value);

                        FeedPet(mod, pet, currentItem, friendship);

                        return true;
                    }
                }
            }

            return false;
        }

        public static void FeedPet(HorseOverhaul mod, Pet pet, Item currentItem, int friendship)
        {
            if (pet?.modData?.ContainsKey($"{mod.ModManifest.UniqueID}/gotFed") == true && !mod.Config.AllowMultipleFeedingsADay)
            {
                Game1.drawObjectDialogue(mod.Helper.Translation.Get("AteEnough", new { name = pet.displayName }));
            }
            else
            {
                pet.modData.Add($"{mod.ModManifest.UniqueID}/gotFed", "fed");

                Game1.drawObjectDialogue(mod.Helper.Translation.Get("AteFood", new { name = pet.displayName, foodName = currentItem.DisplayName }));

                pet.doEmote(Character.happyEmote);

                Game1.player.reduceActiveItemByOne();

                pet.friendshipTowardFarmer.Value = Math.Min(1000, pet.friendshipTowardFarmer.Value + friendship);
            }
        }

        private static bool IsInRange(Character chara, int mouseX, int mouseY, bool ignoreMousePosition)
        {
            if (Utility.withinRadiusOfPlayer((int)chara.Position.X, (int)chara.Position.Y, 1, Game1.player))
            {
                if (ignoreMousePosition)
                {
                    var playerPos = Game1.player.getStandingXY();
                    var charaPos = chara.getStandingXY();

                    int xDistance = Math.Abs(playerPos.X - charaPos.X);
                    int yDistance = Math.Abs(playerPos.Y - charaPos.Y);

                    return Game1.player.FacingDirection switch
                    {
                        Game1.up => playerPos.Y > charaPos.Y && xDistance < 48,
                        Game1.down => playerPos.Y < charaPos.Y && xDistance < 48,
                        Game1.right => playerPos.X < charaPos.X && yDistance < 48,
                        Game1.left => playerPos.X > charaPos.X && yDistance < 48,
                        _ => false,
                    };
                }
                else
                {
                    return Utility.distance(mouseX, chara.Position.X, mouseY, chara.Position.Y) <= 70;
                }
            }

            return false;
        }
    }
}