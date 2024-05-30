/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using SkillPrestige.Framework;
using SkillPrestige.Logging;
using SkillPrestige.Professions;
using SkillPrestige.SkillTypes;
using StardewValley;

namespace SkillPrestige
{
    /// <summary>Represents a prestige for a skill.</summary>
    [Serializable]
    public class Prestige
    {
        /// <summary>The skill the prestige is for.</summary>
        public SkillType SkillType { get; set; }

        /// <summary>The total available prestige points, one is gained per skill reset.</summary>
        public int PrestigePoints { get; set; }

        /// <summary>Professions that have been chosen to be permanent using skill points.</summary>
        public IList<int> PrestigeProfessionsSelected { get; set; } = new List<int>();

        public Dictionary<string, int> CraftingRecipeAmountsToSave { get; set; } = new();

        public Dictionary<string, int> CookingRecipeAmountsToSave { get; set; } = new();

        public void FixDeserializedNulls()
        {
            this.CraftingRecipeAmountsToSave ??= new Dictionary<string, int>();
            this.CookingRecipeAmountsToSave ??= new Dictionary<string, int>();
        }

        /// <summary>Purchases a profession to be part of the prestige set.</summary>
        public static void AddPrestigeProfession(int professionId)
        {
            var skill = Skill.AllSkills.Single(x => x.Professions.Select(y => y.Id).Contains(professionId));
            var prestige = PrestigeSet.Instance.Prestiges.Single(x => x.SkillType == skill.Type);
            int originalPrestigePointsForSkill = prestige.PrestigePoints;
            if (skill.Professions.Where(x => x.LevelAvailableAt == 5).Select(x => x.Id).Contains(professionId))
            {
                prestige.PrestigePoints -= PerSaveOptions.Instance.CostOfTierOnePrestige;
                Logger.LogInformation($"Spent prestige point on {skill.Type.Name} skill.");
            }

            else if (skill.Professions.Where(x => x.LevelAvailableAt == 10).Select(x => x.Id).Contains(professionId))
            {
                prestige.PrestigePoints -= PerSaveOptions.Instance.CostOfTierTwoPrestige;
                Logger.LogInformation($"Spent 2 prestige points on {skill.Type.Name} skill.");
            }
            else
                Logger.LogError($"No skill found for selected profession: {professionId}");
            if (prestige.PrestigePoints < 0)
            {
                prestige.PrestigePoints = originalPrestigePointsForSkill;
                Logger.LogCritical($"Prestige amount for {skill.Type.Name} skill would have gone negative, unable to grant profession {professionId}. Prestige values reset.");
            }
            else
            {
                prestige.PrestigeProfessionsSelected.Add(professionId);
                Logger.LogInformation("Profession permanently added.");
                Profession.AddMissingProfessions();
            }
        }

        /// <summary>Prestiges a skill, resetting it to level 0, removing all recipes and effects of the skill at higher levels and grants one prestige point in that skill to the player.</summary>
        /// <param name="skill">the skill you wish to prestige.</param>
        public static void PrestigeSkill(Skill skill)
        {
            try
            {
                if (PerSaveOptions.Instance.PainlessPrestigeMode)
                {
                    Logger.LogInformation($"Prestiging skill {skill.Type.Name} via Painless Mode.");
                    skill.SetSkillExperience(skill.GetSkillExperience() - PerSaveOptions.Instance.ExperienceNeededPerPainlessPrestige);
                    Logger.LogInformation($"Removed {PerSaveOptions.Instance.ExperienceNeededPerPainlessPrestige} experience points from {skill.Type.Name} skill.");
                }
                else
                {
                    Logger.LogInformation($"Prestiging skill {skill.Type.Name}.");
                    skill.OnPrestige?.Invoke();
                    skill.SetSkillExperience(0);
                    skill.SetSkillLevel(0);
                    Logger.LogInformation($"Skill {skill.Type.Name} experience and level reset.");
                    if (PerSaveOptions.Instance.ResetRecipesOnPrestige)
                    {
                        var currentPrestige = PrestigeSet.Instance.Prestiges.Single(x => x.SkillType == skill.Type);
                        currentPrestige.CraftingRecipeAmountsToSave = RemovePlayerCraftingRecipesForSkill(skill.Type);
                        currentPrestige.CookingRecipeAmountsToSave = RemovePlayerCookingRecipesForSkill(skill.Type);
                        Logger.LogVerbose($"stored crafting recipe counts upon prestige of skill {currentPrestige.SkillType.Name}, count: {currentPrestige.CraftingRecipeAmountsToSave.Count}");
                        Logger.LogVerbose($"stored cooking recipe counts upon prestige of skill {currentPrestige.SkillType.Name}, count: {currentPrestige.CookingRecipeAmountsToSave.Count}");
                        RecipeHandler.ResetRecipes();
                        RecipeHandler.LoadRecipes();
                    }
                    Profession.RemoveProfessions(skill);
                    PlayerManager.CorrectStats(skill);
                    Profession.AddMissingProfessions();
                }
                PrestigeSet.Instance.Prestiges.Single(x => x.SkillType == skill.Type).PrestigePoints += PerSaveOptions.Instance.PointsPerPrestige;
                Logger.LogInformation($"{PerSaveOptions.Instance.PointsPerPrestige} Prestige point(s) added to {skill.Type.Name} skill.");

            }
            catch (Exception exception)
            {
                Logger.LogError(exception.Message + Environment.NewLine + exception.StackTrace);
            }
        }

        /// <summary>Removes all crafting recipes granted by levelling a skill.</summary>
        /// <param name="skillType">the skill type to remove all crafting recipes from.</param>
        private static Dictionary<string, int> RemovePlayerCraftingRecipesForSkill(SkillType skillType)
        {
            Logger.LogInformation($"Removing {skillType.Name} crafting recipes");
            var craftingAmountsToStore = new Dictionary<string, int>();
            foreach (
                var recipe in
                CraftingRecipe.craftingRecipes.Where(
                    x =>
                        x.Value.Split('/').Length > 4
                        && x.Value.Split('/')[4].Contains(skillType.Name)
                        && Game1.player.craftingRecipes.ContainsKey(x.Key)))
            {
                Logger.LogVerbose($"Removing {skillType.Name} crafting recipe {recipe.Key}");
                int craftCount = Game1.player.craftingRecipes[recipe.Key];
                //storing crafted amount of recipe
                if (craftCount > 0) craftingAmountsToStore.Add(recipe.Key, craftCount);
                Game1.player.craftingRecipes.Remove(recipe.Key);
            }
            Logger.LogInformation($"{skillType.Name} crafting recipes removed.");
            return craftingAmountsToStore;
        }

        /// <summary>Removes all cooking recipes granted by levelling a skill.</summary>
        /// <param name="skillType">the skill type to remove all cooking recipes from.</param>
        private static Dictionary<string, int> RemovePlayerCookingRecipesForSkill(SkillType skillType)
        {
            // if (skillType.Name.IsOneOf("Cooking", string.Empty))
            // {
            //     Logger.LogInformation($"Wiping skill cooking recipes for skill: {skillType.Name} could remove more than intended. Exiting skill cooking recipe wipe.");
            //     return null;
            // }
            Logger.LogInformation($"Removing {skillType.Name} cooking recipes.");
            var cookingAmountsToStore = new Dictionary<string, int>();
            foreach (
                var recipe in
                CraftingRecipe.cookingRecipes.Where(
                    x =>
                    {
                        string[] recipePieces = x.Value.Split('/');
                        return recipePieces.Length >=4
                               && recipePieces[3].Contains(skillType.Name)
                               && Game1.player.cookingRecipes.ContainsKey(x.Key);
                    }))
            {
                Logger.LogVerbose($"Removing {skillType.Name} cooking recipe {recipe.Key}");
                    cookingAmountsToStore.Add(recipe.Key, Game1.player.cookingRecipes[recipe.Key]);
                    Game1.player.cookingRecipes.Remove(recipe.Key);
            }
            Logger.LogInformation($"{skillType.Name} cooking recipes removed.");
            return cookingAmountsToStore;
        }
    }
}
