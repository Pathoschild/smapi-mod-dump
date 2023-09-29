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
using StardewValley.Menus;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class RecipeLevelUpInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _helper = helper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        // public LevelUpMenu(int skill, int level)
        public static void LevelUpMenuConstructor_SendSkillRecipeChecks_Postfix(LevelUpMenu __instance, int skill, int level)
        {
            try
            {
                SendSkillRecipeChecks((Skill)skill, level);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(LevelUpMenuConstructor_SendSkillRecipeChecks_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static void SendSkillRecipeChecks(Skill skill, int level)
        {
            SendSkillCookingRecipeChecks(skill, level);
            // SendSkillCraftingRecipeChecks(skill, level);
        }

        private static void SendSkillCookingRecipeChecks(Skill skill, int level)
        {
            if (!_archipelago.SlotData.Chefsanity.HasFlag(Chefsanity.Skills) || !_cookingRecipesBySkill.ContainsKey(skill))
            {
                return;
            }

            var skillRecipes = _cookingRecipesBySkill[skill];
            for (var i = 0; i <= level; i++)
            {
                if (!skillRecipes.ContainsKey(i))
                {
                    continue;
                }

                var skillRecipesAtLevel = skillRecipes[i];
                foreach (var skillRecipe in skillRecipesAtLevel)
                {
                    _locationChecker.AddCheckedLocation($"{RecipePurchaseInjections.CHEFSANITY_LOCATION_PREFIX}{skillRecipe}");
                }
            }
        }

        private static void SendSkillCraftingRecipeChecks(Skill skill, int level)
        {
            throw new NotImplementedException();
        }

        private static readonly Dictionary<Skill, Dictionary<int, string[]>> _cookingRecipesBySkill = new()
        {
            {
                Skill.Farming, new Dictionary<int, string[]>()
                {
                    { 3, new[] { "Farmer's Lunch" } },
                }
            },
            {
                Skill.Fishing, new Dictionary<int, string[]>()
                {
                    { 3, new[] { "Dish O' The Sea" } },
                    { 9, new[] { "Seafoam Pudding" } },
                }
            },
            {
                Skill.Foraging, new Dictionary<int, string[]>()
                {
                    { 2, new[] { "Survival Burger" } },
                }
            },
            {
                Skill.Mining, new Dictionary<int, string[]>()
                {
                    { 3, new[] { "Miner's Treat" } },
                }
            },
            {
                Skill.Combat, new Dictionary<int, string[]>()
                {
                    { 3, new[] { "Roots Platter" } },
                    { 9, new[] { "Squid Ink Ravioli" } },
                }
            },
        };
    }
}
