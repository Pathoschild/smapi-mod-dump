/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants.Locations;
using StardewArchipelago.Constants.Vanilla;
using StardewArchipelago.Stardew.Ids.Vanilla;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class RecipeFriendshipInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static Dictionary<string, string> _allRecipes;

        public static void Initialize(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _helper = helper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;

            _allRecipes = DataLoader.CookingRecipes(Game1.content);
        }

        // public void grantConversationFriendship(Farmer who, int amount = 20)
        public static void GrantConversationFriendship_SendFriendshipRecipeChecks_Postfix(NPC __instance, Farmer who, int amount = 20)
        {
            try
            {
                if (!who.friendshipData.ContainsKey(__instance.Name))
                {
                    return;
                }

                var friendship = who.friendshipData[__instance.Name];
                var currentHearts = friendship.Points / 250;
                CheckCookingRecipeLocations(__instance.Name, currentHearts);
                CheckCraftingRecipeLocations(__instance.Name, currentHearts);

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GrantConversationFriendship_SendFriendshipRecipeChecks_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static void CheckCookingRecipeLocations(string friendName, int currentHearts)
        {
            if (!_archipelago.SlotData.Chefsanity.HasFlag(Chefsanity.Friendship))
            {
                return;
            }

            foreach (var (recipeName, recipeData) in _allRecipes)
            {
                var recipeFields = recipeData.Split("/");
                if (recipeFields.Length < 4)
                {
                    continue;
                }

                var unlockConditions = recipeFields[3];
                var unlockConditionFields = unlockConditions.Split(":").Last().Split(" ");
                if (unlockConditionFields.Length != 3 ||
                    !unlockConditionFields[0].Equals("f", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var recipeFriendName = unlockConditionFields[1];
                if (!recipeFriendName.Equals(friendName, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var hearts = int.Parse(unlockConditionFields[2]);
                if (hearts > currentHearts)
                {
                    continue;
                }

                var aliasedRecipeName = GetAliased(recipeName);

                _locationChecker.AddCheckedLocation($"{aliasedRecipeName}{Suffix.CHEFSANITY}");
            }
        }

        // public static void AddCookingRecipe(Event @event, string[] args, EventContext context)
        public static bool AddCookingRecipe_SkipLearningCookies_Prefix(Event @event, string[] args, EventContext context)
        {
            try
            {
                if (!@event.eventCommands[@event.CurrentCommand].Contains("Cookies"))
                {
                    return true; // run original logic
                }

                ++@event.CurrentCommand;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AddCookingRecipe_SkipLearningCookies_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public void skipEvent()
        public static bool SkipEvent_CookiesRecipe_Prefix(Event __instance)
        {
            try
            {
                if (__instance.id != EventIds.COOKIES_RECIPE)
                {
                    return true; // run original logic
                }

                SkipCookiesRecipeEventArchipelago(__instance);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SkipEvent_CookiesRecipe_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void SkipCookiesRecipeEventArchipelago(Event cookiesEvent)
        {
            if (cookiesEvent.playerControlSequence)
            {
                cookiesEvent.EndPlayerControlSequence();
            }

            Game1.playSound("drumkit6");

            var actorPositionsAfterMoveField = _helper.Reflection.GetField<Dictionary<string, Vector3>>(cookiesEvent, "actorPositionsAfterMove");
            actorPositionsAfterMoveField.GetValue().Clear();

            foreach (var actor in cookiesEvent.actors)
            {
                var ignoreStopAnimation = actor.Sprite.ignoreStopAnimation;
                actor.Sprite.ignoreStopAnimation = true;
                actor.Halt();
                actor.Sprite.ignoreStopAnimation = ignoreStopAnimation;
                cookiesEvent.resetDialogueIfNecessary(actor);
            }

            cookiesEvent.farmer.Halt();
            cookiesEvent.farmer.ignoreCollisions = false;
            Game1.exitActiveMenu();
            Game1.dialogueUp = false;
            Game1.dialogueTyping = false;
            Game1.pauseTime = 0.0f;

            // Game1.player.cookingRecipes.TryAdd("Cookies", 0);
            _locationChecker.AddCheckedLocation($"Cookies{Suffix.CHEFSANITY}");
            cookiesEvent.endBehaviors();
        }

        private static string GetAliased(string recipeName)
        {
            var aliasedRecipeName = recipeName;

            foreach (var (internalName, realName) in NameAliases.RecipeNameAliases)
            {
                if (aliasedRecipeName.Contains(internalName))
                {
                    aliasedRecipeName = aliasedRecipeName.Replace(internalName, realName);
                }
            }

            return aliasedRecipeName;
        }

        private static void CheckCraftingRecipeLocations(string friendName, int currentHearts)
        {
            // There are no crafting recipe learning checks yet
        }
    }
}
