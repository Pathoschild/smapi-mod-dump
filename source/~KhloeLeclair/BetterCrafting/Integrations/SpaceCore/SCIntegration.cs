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

namespace Leclair.Stardew.BetterCrafting.Integrations.SpaceCore;

public class SCIntegration : BaseAPIIntegration<IApi, ModEntry>, IRecipeProvider {

	private readonly ProxyManager<Nothing>? ProxyMan;

	private readonly AssemblyBuilder? Assembly;
	private readonly ModuleBuilder? Module;

	private readonly Type? CustomCraftingRecipe;
	private readonly IDictionary? CookingRecipes;
	private readonly IDictionary? CraftingRecipes;

	public SCIntegration(ModEntry mod)
	: base(mod, "spacechase0.SpaceCore", "1.8.1") {

		if (!IsLoaded)
			return;

		try {
			CustomCraftingRecipe = Type.GetType("SpaceCore.CustomCraftingRecipe, SpaceCore");

			CookingRecipes = mod.Helper.Reflection.GetField<IDictionary>(CustomCraftingRecipe!, "CookingRecipes", true).GetValue();
			CraftingRecipes = mod.Helper.Reflection.GetField<IDictionary>(CustomCraftingRecipe!, "CraftingRecipes", true).GetValue();

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
	}

	public int RecipePriority => 10;

	public bool CacheAdditionalRecipes => true;

	public void AddCustomSkillExperience(Farmer farmer, string skill, int amt) {
		if (!IsLoaded)
			return;

		API.AddExperienceForCustomSkill(farmer, skill, amt);
	}

	public IEnumerable<IRecipe>? GetAdditionalRecipes(bool cooking) {
		return null;
	}

	public int GetCustomSkillLevel(Farmer farmer, string skill) {
		if (!IsLoaded)
			return 0;

		return API.GetLevelForCustomSkill(farmer, skill);
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
				if (cls.Equals("SpaceCore.CustomCraftingRecipe+ObjectIngredientMatcher")) {
					matcher = ProxyMan.ObtainProxy<IIngredientMatcher>(ing);
					int? index = Self.Helper.Reflection.GetField<int>(ing, "objectIndex", false)?.GetValue();
					if (index.HasValue)
						result = new BaseIngredient(index.Value, matcher.Quantity);

				} else if (cls.Equals("DynamicGameAssets.DGACustomCraftingRecipe+DGAIngredientMatcher")) {
					result = new DGAIngredient(ing, Self);

				} else
					result = new SCIngredient(ing, Self);

			} catch (Exception ex) {
				Log($"An error occurred while handling a SpaceCore IngredientMatcher for the recipe {recipe.name}. This recipe will not be craftable.", LogLevel.Warn, ex);
				result = null;
			}

			if (result == null)
				result = new ErrorIngredient();

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
			return null;
		}
	}
}
