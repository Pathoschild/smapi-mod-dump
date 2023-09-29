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
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

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

            _allRecipes = Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes");
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
                // CheckCraftingRecipeLocations(__instance.Name, currentHearts);

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
                var unlockConditionFields = unlockConditions.Split(" ");
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

                _locationChecker.AddCheckedLocation($"{RecipePurchaseInjections.CHEFSANITY_LOCATION_PREFIX}{recipeName}");
            }
        }

        private static void CheckCraftingRecipeLocations(string friendName, int currentHearts)
        {
            throw new NotImplementedException();
        }
    }
}
