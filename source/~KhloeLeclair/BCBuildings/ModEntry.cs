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

using Leclair.Stardew.BetterCrafting;

using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewValley.Network;

namespace Leclair.Stardew.BCBuildings;

public class ModEntry : ModSubscriber, IRecipeProvider {

	public static readonly string CategoryID = "leclair.bcbuildings/buildings";

	internal ModApi? API;

	internal IBetterCrafting? BCAPI;

	internal CaseInsensitiveDictionary<Rectangle?> BuildingSources = new();

	internal CaseInsensitiveDictionary<(BluePrint, string?, string?)> ApiBlueprints = new();

	public override void Entry(IModHelper helper) {
		base.Entry(helper);

		API = new ModApi(this);

		SpriteHelper.SetHelper(Helper);
		I18n.Init(Helper.Translation);
	}

	public override object? GetApi() {
		return API;
	}

	[Subscriber]
	private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
		BCAPI = Helper.ModRegistry.GetApi<IBetterCrafting>("leclair.bettercrafting");
		if (BCAPI is null)
			return;

		BCAPI.AddRecipeProvider(this);
		BCAPI.CreateDefaultCategory(false, CategoryID, I18n.Category_Name);

		CaseInsensitiveDictionary<Rectangle?>? buildings;
		try {
			buildings = Helper.Data.ReadJsonFile<CaseInsensitiveDictionary<Rectangle?>>(@"assets/source_overrides.json");
			if (buildings is null)
				throw new Exception("source_overrides.json is invalid or empty");

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
		if (cooking)
			return null;

		CaseInsensitiveDictionary<BluePrint> blueprints = new();

		GetBluePrintsFromMenu(blueprints, false);
		if (Game1.player.mailReceived.Contains("hasPickedUpMagicInk") || Game1.player.hasMagicInk)
			GetBluePrintsFromMenu(blueprints, true);

		foreach(var bp in ApiBlueprints) {
			if (blueprints.ContainsKey(bp.Key))
				continue;

			if (!string.IsNullOrEmpty(bp.Value.Item2) && !GameStateQuery.CheckConditions(bp.Value.Item2))
				continue;

			blueprints.Add(bp.Key, bp.Value.Item1);
		}

		API?.EmitBlueprintPopulation(blueprints);

		List<IRecipe> recipes = new();

		foreach (var bp in blueprints.Values)
			recipes.Add(new BPRecipe(bp, this));

		recipes.Add(new ActionRecipe(ActionType.Move, this));
		recipes.Add(new ActionRecipe(ActionType.Paint, this));
		recipes.Add(new ActionRecipe(ActionType.Demolish, this));

		BCAPI!.AddRecipesToDefaultCategory(false, CategoryID, recipes.Select(x => x.Name));

		return recipes;
	}

	public void GetBluePrintsFromMenu(CaseInsensitiveDictionary<BluePrint> store, bool magical) {
		CarpenterMenu menu = new(magical);
		List<BluePrint>? blueprints = Helper.Reflection.GetField<List<BluePrint>>(menu, "blueprints", false)?.GetValue();

		if (blueprints == null) {
			CommonHelper.YeetMenu(menu);
			return;
		}

		foreach (var bp in blueprints) {
			if (! store.ContainsKey(bp.name))
				store.Add(bp.name, bp);
		}

		if (store.ContainsKey("Coop") && !store.ContainsKey("Big Coop"))
			store["Big Coop"] = new BluePrint("Big Coop");

		if (store.ContainsKey("Coop") && !store.ContainsKey("Deluxe Coop"))
			store["Deluxe Coop"] = new BluePrint("Deluxe Coop");

		if (store.ContainsKey("Barn") && !store.ContainsKey("Big Barn"))
			store["Big Barn"] = new BluePrint("Big Barn");

		if (store.ContainsKey("Barn") && !store.ContainsKey("Deluxe Barn"))
			store["Deluxe Barn"] = new BluePrint("Deluxe Barn");

		if (store.ContainsKey("Shed") && !store.ContainsKey("Big Shed"))
			store["Big Shed"] = new BluePrint("Big Shed");

		CommonHelper.YeetMenu(menu);
	}


	#endregion

}
