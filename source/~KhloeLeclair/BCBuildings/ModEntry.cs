/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.Types;

#if IS_BETTER_CRAFTING
using Leclair.Stardew.Common.Crafting;
#endif
using Leclair.Stardew.BetterCrafting;

using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;

namespace Leclair.Stardew.BCBuildings;

public class ModEntry : ModSubscriber, IRecipeProvider {

	public static readonly string CategoryID = "leclair.bcbuildings/buildings";

	internal IBetterCrafting? API;

	public CaseInsensitiveDictionary<Rectangle?> BuildingSources = new();

	public override void Entry(IModHelper helper) {
		base.Entry(helper);

#if IS_BETTER_CRAFTING
#else
		SpriteHelper.SetHelper(Helper);
#endif

		I18n.Init(Helper.Translation);
	}

	[Subscriber]
	private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
		API = Helper.ModRegistry.GetApi<IBetterCrafting>("leclair.bettercrafting");
		if (API is null)
			return;

		API.AddRecipeProvider(this);
		API.CreateDefaultCategory(false, CategoryID, "Building");

		CaseInsensitiveDictionary<Rectangle?>? buildings;
		try {
			buildings = Helper.Data.ReadJsonFile<CaseInsensitiveDictionary<Rectangle?>>(@"assets/source_overrides.json");
			if (buildings is null)
				throw new ArgumentNullException(nameof(buildings));

		} catch (Exception ex) {
			Log($"source_overrides.json is missing or invalid.", LogLevel.Warn, ex);
			buildings = null;
		}

		if (buildings != null)
			BuildingSources = buildings;
	}

#region IRecipeProvider

	public int RecipePriority => 0;

	public IRecipe? GetRecipe(CraftingRecipe recipe) {
		return null;
	}

	public bool CacheAdditionalRecipes => false;

	public IEnumerable<IRecipe>? GetAdditionalRecipes(bool cooking) {
		Log("Called GetAdditionalRecipes");

		if (cooking)
			return null;

		List<IRecipe> recipes = new();

		recipes.AddRange(GetRecipesFromMenu(false));

		if (Game1.player.mailReceived.Contains("hasPickedUpMagicInk") || Game1.player.hasMagicInk)
			recipes.AddRange(GetRecipesFromMenu(true));

		recipes.Add(new ActionRecipe(ActionType.Move, this));
		recipes.Add(new ActionRecipe(ActionType.Paint, this));
		recipes.Add(new ActionRecipe(ActionType.Demolish, this));

		API!.AddRecipesToDefaultCategory(false, CategoryID, recipes.Select(x => x.Name));

		return recipes;
	}

	public IEnumerable<IRecipe> GetRecipesFromMenu(bool magical) {
		List<IRecipe> result = new();

		CarpenterMenu menu = new(magical);
		List<BluePrint>? blueprints = Helper.Reflection.GetField<List<BluePrint>>(menu, "blueprints", false)?.GetValue();

		if (blueprints == null) {
			CommonHelper.YeetMenu(menu);
			return result;
		}

		List<string> seen = new();

		foreach (var bp in blueprints) {
			result.Add(new BPRecipe(bp, this));
			seen.Add(bp.name);
		}

		if (seen.Contains("Coop") && !seen.Contains("Big Coop"))
			result.Add(new BPRecipe(new BluePrint("Big Coop"), this));

		if (seen.Contains("Coop") && !seen.Contains("Deluxe Coop"))
			result.Add(new BPRecipe(new BluePrint("Deluxe Coop"), this));

		if (seen.Contains("Barn") && !seen.Contains("Big Barn"))
			result.Add(new BPRecipe(new BluePrint("Big Barn"), this));

		if (seen.Contains("Barn") && !seen.Contains("Deluxe Barn"))
			result.Add(new BPRecipe(new BluePrint("Deluxe Barn"), this));

		if (seen.Contains("Shed") && !seen.Contains("Big Shed"))
			result.Add(new BPRecipe(new BluePrint("Big Shed"), this));

		CommonHelper.YeetMenu(menu);

		return result;
	}


#endregion

}
