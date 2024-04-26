/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.Integrations;

using SpaceCore;
using Nanoray.Pintail;

using StardewValley;
using System.Collections.Generic;
using StardewModdingAPI;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Leclair.Stardew.BetterCrafting.Models;
using StardewValley.ItemTypeDefinitions;
using StardewValley.GameData.Objects;

namespace Leclair.Stardew.BetterCrafting.Integrations.SpaceCore;

public class SCIntegration : BaseAPIIntegration<IApi, ModEntry>, IRecipeProvider {

	private readonly ProxyManager<Nothing>? ProxyMan;

	private readonly AssemblyBuilder? Assembly;
	private readonly ModuleBuilder? Module;

	private readonly Type? CustomCraftingRecipe;
	private readonly Type? CustomSkills;

	private readonly IDictionary? CookingRecipes;
	private readonly IDictionary? CraftingRecipes;
	private readonly IDictionary? SkillsByName;

	public static readonly string SKILL_BUFF_PREFIX = "spacechase.SpaceCore.SkillBuff.";

	public SCIntegration(ModEntry mod)
	: base(mod, "spacechase0.SpaceCore", "1.8.1") {

		if (!IsLoaded)
			return;

		try {
			CustomCraftingRecipe = Type.GetType("SpaceCore.CustomCraftingRecipe, SpaceCore");
			CustomSkills = Type.GetType("SpaceCore.Skills, SpaceCore");

			CookingRecipes = mod.Helper.Reflection.GetField<IDictionary>(CustomCraftingRecipe!, "CookingRecipes", true).GetValue();
			CraftingRecipes = mod.Helper.Reflection.GetField<IDictionary>(CustomCraftingRecipe!, "CraftingRecipes", true).GetValue();
			SkillsByName = mod.Helper.Reflection.GetField<IDictionary>(CustomSkills!, "SkillsByName", true).GetValue();

			var builder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName($"Leclair.Stardew.BetterCrafting.Proxies, Version={GetType().Assembly.GetName().Version}, Culture=neutral"), AssemblyBuilderAccess.Run);
			var module = builder.DefineDynamicModule($"Proxies");

			Assembly = builder;
			Module = module;

			ProxyMan = new ProxyManager<Nothing>(module, new ProxyManagerConfiguration<Nothing>(
				proxyObjectInterfaceMarking: ProxyObjectInterfaceMarking.MarkerWithProperty
			));
		} catch (Exception ex) {
			Log($"Unable to set up Pintail-based proxying of SpaceCore internals.", StardewModdingAPI.LogLevel.Warn, ex);
		}

		mod.Recipes.AddProvider(this);

		mod.Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;

	}

	private void GameLoop_DayStarted(object? sender, StardewModdingAPI.Events.DayStartedEventArgs e) {
		// Get a list of all existing SC Buff rules.
		var old_handlers = Self.Recipes.GetRuleHandlers()
			.Where(pair => pair.Key.StartsWith("scbuff:") && pair.Value is SCBuffRuleHandler)
			.Select(pair => ((SCBuffRuleHandler) pair.Value).SkillId)
			.ToHashSet();

		// For each skill, set up a buff rule if one doesn't exist.
		foreach(var skill in GetSkills()) {
			if (string.IsNullOrEmpty(skill.Id))
				continue;

			// If we had an entry for the skill already, we don't
			// need to make a new one. Just remove it from the list
			// and skip.
			if (old_handlers.Remove(skill.Id))
				continue;

			Self.Recipes.RegisterRuleHandler($"scbuff:{skill.Id}", new SCBuffRuleHandler(skill));
		}

		// Unregister any skills that no longer exist.
		foreach(string handler in old_handlers)
			Self.Recipes.UnregisterRuleHandler($"scbuff:{handler}");
	}

	#region Skill Handling

	public string[]? GetSkillNames() {
		if (!IsLoaded)
			return null;

		return API.GetCustomSkills();
	}

	public IEnumerable<ISCSkill> GetSkills() {
		if (!IsLoaded || ProxyMan is null || SkillsByName is null)
			yield break;

		foreach(object value in SkillsByName.Values) {
			if (!ProxyMan.TryProxy<ISCSkill>(value, out var skill))
				continue;

			yield return skill;
		}
	}

	public ISCSkill? GetSkill(string name) {
		if (!IsLoaded || ProxyMan is null || SkillsByName is null || ! SkillsByName.Contains(name))
			return null;

		object? thing = SkillsByName[name];
		if (thing is not null && ProxyMan.TryProxy<ISCSkill>(thing, out var skill))
			return skill;

		return null;
	}

	public IEnumerable<(ISCSkill, float)> GetItemBuffs(ObjectData data) {
		if (data.Buffs is null)
			yield break;

		foreach(var buff in data.Buffs) {
			if (buff.CustomFields is null)
				continue;

			foreach(var pair in buff.CustomFields)
				if (pair.Key.StartsWith(SKILL_BUFF_PREFIX) && float.TryParse(pair.Value, out float value)) {
					string name = pair.Key[SKILL_BUFF_PREFIX.Length..];
					if (GetSkill(name) is ISCSkill skill)
						yield return (skill, value);
				}
		}
	}

	public void AddCustomSkillExperience(Farmer farmer, string skill, int amt) {
		if (!IsLoaded)
			return;

		API.AddExperienceForCustomSkill(farmer, skill, amt);
	}

	public int GetCustomSkillLevel(Farmer farmer, string skill) {
		if (!IsLoaded)
			return 0;

		return API.GetLevelForCustomSkill(farmer, skill);
	}

	#endregion

	#region Recipe

	public int RecipePriority => 10;

	public bool CacheAdditionalRecipes => true;

	public IEnumerable<IRecipe>? GetAdditionalRecipes(bool cooking) {
		return null;
	}

	public IRecipe? GetRecipe(CraftingRecipe recipe) {
		if (!IsLoaded || CookingRecipes is null || CraftingRecipes is null || ProxyMan is null)
			return null;

		IDictionary container = recipe.isCookingRecipe ? CookingRecipes : CraftingRecipes;

		if (!container.Contains(recipe.name))
			return null;

		object? value = container[recipe.name];
		if (value is null)
			return null;

		if (!ProxyMan.TryProxy<ICustomCraftingRecipe>(value, out var ccr))
			return null;

		object[]? ingredients = Self.Helper.Reflection.GetProperty<object[]>(value, "Ingredients", false)?.GetValue();
		if (ingredients is null)
			return null;

		List<IIngredient> ingreds = new();

		foreach(object ing in ingredients) {
			Type type = ing.GetType();
			string cls = type.FullName ?? type.Name;

			IIngredientMatcher? matcher;
			IIngredient? result = null;

			try {
				matcher = ProxyMan.ObtainProxy<IIngredientMatcher>(ing);

				if (cls.Equals("SpaceCore.CustomCraftingRecipe+ObjectIngredientMatcher")) {
					// Private stuff.
					string? index = Self.Helper.Reflection.GetField<string>(ing, "objectIndex", false)?.GetValue();
					if (!string.IsNullOrEmpty(index))
						result = new BaseIngredient(index, matcher.Quantity);

				} else
					result = new SCIngredient(matcher);

			} catch (Exception ex) {
				Log($"An error occurred while handling a SpaceCore IngredientMatcher for the recipe {recipe.name}. This recipe will not be craftable.", LogLevel.Warn, ex);
				result = null;
			}

			result ??= new ErrorIngredient();
			ingreds.Add(result);
		}

		try {
			return new SCRecipe(
				name: recipe.name,
				recipe: ccr,
				cooking: recipe.isCookingRecipe,
				ingredients: ingreds
			);

		} catch(Exception ex) {
			Log($"An error occurred while accessing a custom SpaceCore recipe. We cannot handle the recipe: {recipe.name}", LogLevel.Error, ex);

			// Make the recipe impossible to craft by adding an error ingredient.
			// That way it won't be crafted *incorrectly*.
			return new RecipeBuilder(recipe)
				.Texture(() => Game1.mouseCursors)
				.Source(() => ErrorIngredient.SOURCE)
				.AddIngredient(new ErrorIngredient())
				.Build();
		}
	}

	#endregion
}
