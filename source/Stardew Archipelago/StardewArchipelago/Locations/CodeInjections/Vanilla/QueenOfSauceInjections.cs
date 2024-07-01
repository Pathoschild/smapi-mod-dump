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
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants.Locations;
using StardewArchipelago.Extensions;
using StardewArchipelago.GameModifications.CodeInjections.Television;
using StardewArchipelago.GameModifications.Seasons;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class QueenOfSauceInjections
    {
        private const string RERUN_DAY = "Wed";
        private const string NORMAL_DAY = "Sun";
        private const string INVERSE_DAY = "Sat";
        private const string ALREADY_KNOWN_KEY = "Strings\\StringsFromCSFiles:TV.cs.13151";
        private const string NEW_RECIPE_LEARNED_KEY = "Strings\\StringsFromCSFiles:TV.cs.13153";

        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static StardewItemManager _itemManager;
        private static QueenOfSauceManager _qosManager;

        private static Dictionary<long, int> _recipeChoiceCache;

        public static void Initialize(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago, LocationChecker locationChecker, StardewItemManager itemManager, QueenOfSauceManager qosManager)
        {
            _monitor = monitor;
            _helper = helper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _itemManager = itemManager;
            _qosManager = qosManager;
            _recipeChoiceCache = new Dictionary<long, int>();
        }

        // protected virtual string[] getWeeklyRecipe()
        public static bool GetWeeklyRecipe_UseArchipelagoSchedule_Prefix(TV __instance, ref string[] __result)
        {
            try
            {
                if (__instance == null)
                {
                    return false; // don't run original logic
                }

                // private TemporaryAnimatedSprite screen;
                var screenField = _helper.Reflection.GetField<TemporaryAnimatedSprite>(__instance, "screen");

                // private int currentChannel;
                var currentChannelField = _helper.Reflection.GetField<int>(__instance, "currentChannel");

                if (screenField.GetValue() == null || currentChannelField.GetValue() != 5)
                {
                    return false; // don't run original logic
                }

                var cookingChannelData = DataLoader.Tv_CookingChannel(Game1.temporaryContent);
                var recipeWeek = PickRecipeWeekToTeach(cookingChannelData);


                var recipeInfo = cookingChannelData[recipeWeek.ToString()].Split('/');
                var recipeName = recipeInfo[0];
                var recipeDetails = recipeInfo[1];
                var tvText = GetQueenOfSauceTvText(recipeName, recipeDetails, recipeInfo);

                if (_archipelago.SlotData.Chefsanity.HasFlag(Chefsanity.QueenOfSauce))
                {
                    _locationChecker.AddCheckedLocation($"{recipeName}{Suffix.CHEFSANITY}");
                }
                else if (!Game1.player.cookingRecipes.ContainsKey(recipeName))
                {
                    Game1.player.cookingRecipes.Add(recipeName, 0);
                }

                __result = tvText;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetWeeklyRecipe_UseArchipelagoSchedule_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static int PickRecipeWeekToTeach(Dictionary<string, string> cookingRecipes)
        {
            if (_recipeChoiceCache.ContainsKey(Game1.stats.DaysPlayed))
            {
                return _recipeChoiceCache[Game1.stats.DaysPlayed];
            }

            var season = Game1.currentSeason;
            _qosManager.GetCurrentDateComponents(out var year, out var week);

            var dayShortName = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
            var isRerunDay = dayShortName.Equals(RERUN_DAY);
            if (isRerunDay)
            {
                var rerunWeek = PickRerunRecipe(cookingRecipes, week);
                if (rerunWeek != -1)
                {
                    _recipeChoiceCache.Add(Game1.stats.DaysPlayed, rerunWeek);
                    return rerunWeek;
                }
            }

            if (dayShortName.Equals(INVERSE_DAY))
            {
                year = 1 - year; // On Saturday, we play the opposite year's recipe
            }

            var currentSeasonOffset = Array.IndexOf(SeasonsRandomizer.ValidSeasons, season.ToCapitalized());
            var recipeWeek = year * 16 + 1; // There are 32 recipes. Year 1 is 1-16, Year 2 is 17-32
            recipeWeek += (currentSeasonOffset * 4); // 4 per month
            recipeWeek += week; // 1 per week

            _recipeChoiceCache.Add(Game1.stats.DaysPlayed, recipeWeek);
            return recipeWeek;
        }

        private static string[] GetQueenOfSauceTvText(string recipeName, string recipeDetails, string[] recipeInfo)
        {
            var weeklyRecipe = new List<string>();
            weeklyRecipe.Add(recipeDetails);

            var isEnglish = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en;
            var alreadyKnowsRecipe = Game1.player.cookingRecipes.ContainsKey(recipeName);
            var alreadyKnownText = Game1.content.LoadString(ALREADY_KNOWN_KEY, recipeName);
            var newRecipeText = Game1.content.LoadString(NEW_RECIPE_LEARNED_KEY, recipeName);

            if (_archipelago.SlotData.Chefsanity.HasFlag(Chefsanity.QueenOfSauce))
            {
                return weeklyRecipe.ToArray();
            }

            var recipeLearnedInfoText = alreadyKnowsRecipe ? alreadyKnownText : newRecipeText;

            if (!isEnglish)
            {
                var translatedRecipeName = CraftingRecipe.cookingRecipes.ContainsKey(recipeName)
                    ? CraftingRecipe.cookingRecipes[recipeName].Split('/').Last()
                    : recipeInfo.Last();
                alreadyKnownText = Game1.content.LoadString(ALREADY_KNOWN_KEY, translatedRecipeName);
                newRecipeText = Game1.content.LoadString(NEW_RECIPE_LEARNED_KEY, translatedRecipeName);
                recipeLearnedInfoText = alreadyKnowsRecipe ? alreadyKnownText : newRecipeText;
            }

            weeklyRecipe.Add(recipeLearnedInfoText);
            return weeklyRecipe.ToArray();
        }

        private static int PickRerunRecipe(Dictionary<string, string> cookingRecipes, int currentWeek)
        {
            var allRerunRecipes = _qosManager.GetAllRerunRecipes(currentWeek);
            var missingRerunRecipes = GetMissingRerunRecipes(cookingRecipes, allRerunRecipes);
            var seed = (int)(Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed);
            var random = new Random(seed);
            if (!missingRerunRecipes.Any())
            {
                return -1;
            }
            var rerunRecipe = missingRerunRecipes[random.Next(missingRerunRecipes.Count)];
            return rerunRecipe;
        }

        /// <summary>
        ///     Returns all rerun recipes that the player hasn't seen yet, and if they have seen them all, then just returns all
        ///     rerun recipes so a random can be picked
        /// </summary>
        private static List<int> GetMissingRerunRecipes(Dictionary<string, string> cookingRecipes, List<int> allRerunRecipes)
        {
            List<int> missingRerunRecipes;
            if (_archipelago.SlotData.Chefsanity.HasFlag(Chefsanity.QueenOfSauce))
            {
                missingRerunRecipes = allRerunRecipes.Where(x => _locationChecker.IsLocationNotChecked($"{GetRecipeName(cookingRecipes, x)}{Suffix.CHEFSANITY}")).ToList();
            }
            else
            {
                missingRerunRecipes = allRerunRecipes.Where(x => Game1.player.cookingRecipes.ContainsKey(GetRecipeName(cookingRecipes, x))).ToList();
            }

            if (missingRerunRecipes.Any())
            {
                return missingRerunRecipes;
            }

            return allRerunRecipes;
        }

        private static string GetRecipeName(Dictionary<string, string> cookingRecipes, int week)
        {
            var recipeInfo = cookingRecipes[week.ToString()].Split('/');
            return recipeInfo[0];
        }
    }
}
