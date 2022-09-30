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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.Integrations.GenericModConfigMenu;
using Leclair.Stardew.Common.Types;

using Leclair.Stardew.BetterCrafting;

using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using Newtonsoft.Json.Linq;

namespace Leclair.Stardew.BCBuildings;

public class RuleData : IDynamicRuleData {

	public RuleData(string id) {
		Id = id;
	}

	public string Id { get; }

	public IDictionary<string, JToken> Fields { get; } = new Dictionary<string, JToken>();

}

public class ModEntry : ModSubscriber, IRecipeProvider {

	public static readonly string CategoryID = "leclair.bcbuildings/buildings";

#nullable disable
	internal ModConfig Config;
#nullable enable

	private IEnumerable<IIngredient>? CachedAdditionalCost;
	private bool HasCachedAdditionalCost = false;

	private GMCMIntegration<ModConfig, ModEntry>? GMCMIntegration;

	internal ModApi? API;

	internal IBetterCrafting? BCAPI;

	internal CaseInsensitiveDictionary<Rectangle?> BuildingSources = new();

	internal CaseInsensitiveDictionary<(BluePrint, string?, string?)> ApiBlueprints = new();

	public override void Entry(IModHelper helper) {
		base.Entry(helper);

		API = new ModApi(this);

		SpriteHelper.SetHelper(Helper);
		I18n.Init(Helper.Translation);

		// Read Config
		Config = Helper.ReadConfig<ModConfig>();
	}

	public override object? GetApi() {
		return API;
	}

	[Subscriber]
	private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
		RegisterConfig();

		BCAPI = Helper.ModRegistry.GetApi<IBetterCrafting>("leclair.bettercrafting");
		if (BCAPI is null)
			return;

		BCAPI.AddRecipeProvider(this);
		BCAPI.RegisterRuleHandler(ModManifest, "Building", new BuildingRuleHandler(this));
		BCAPI.CreateDefaultCategory(
			cooking: false,
			categoryId: CategoryID,
			Name: I18n.Category_Name,
			iconRecipe: "blueprint:Shed",
			useRules: true,
			rules: new IDynamicRuleData[] {
				new RuleData("leclair.bcbuildings/Building")
			}
		);

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

	#region Configuration

	public void SaveConfig() {
		Helper.WriteConfig(Config);
		BCAPI?.InvalidateRecipeCache();
		HasCachedAdditionalCost = false;
	}

	public void ResetConfig() {
		Config = new();
		HasCachedAdditionalCost = false;
	}

	[MemberNotNullWhen(true, nameof(GMCMIntegration))]
	public bool HasGMCM() {
		return GMCMIntegration?.IsLoaded ?? false;
	}

	public void OpenGMCM() {
		if (HasGMCM())
			GMCMIntegration.OpenMenu();
	}

	internal void RegisterConfig() {
		GMCMIntegration = new(this, () => Config, ResetConfig, SaveConfig);
		if (!GMCMIntegration.IsLoaded)
			return;

		GMCMIntegration.Register(true);

		GMCMIntegration.Add(
			I18n.Setting_GreenhouseMove,
			I18n.Setting_GreenhouseMove_Tip,
			c => c.AllowMovingUnfinishedGreenhouse,
			(c, v) => c.AllowMovingUnfinishedGreenhouse = v
		);

		GMCMIntegration
			.AddLabel("")
			.AddLabel(I18n.Setting_Cost)
			.AddParagraph(I18n.Setting_Cost_Desc);

		GMCMIntegration
			.Add(
				name: I18n.Setting_CostMaterial,
				tooltip: I18n.Setting_CostMaterial_Tip,
				get: c => c.CostMaterial,
				set: (c, v) => c.CostMaterial = v,
				min: 0,
				max: 5000,
				interval: 10,
				format: v => $"{v}%"
			)
			.Add(
				name: I18n.Setting_CostCurrency,
				tooltip: I18n.Setting_CostCurrency_Tip,
				get: c => c.CostCurrency,
				set: (c, v) => c.CostCurrency = v,
				min: 0,
				max: 5000,
				interval: 10,
				format: v => $"{v}%"
			)
			.Add(
				name: I18n.Setting_CostAdditional,
				tooltip: I18n.Setting_CostAdditional_Tip,
				get: c => c.CostAdditional ?? string.Empty,
				set: (c, v) => {
					if (string.IsNullOrWhiteSpace(v))
						c.CostAdditional = null;
					else
						c.CostAdditional = v;
				}
			);

		GMCMIntegration
			.AddLabel("")
			.AddLabel(I18n.Setting_Refund)
			.AddParagraph(I18n.Setting_Refund_Desc);

		GMCMIntegration
			.Add(
				name: I18n.Setting_RefundMaterial,
				tooltip: I18n.Setting_RefundMaterial_Tip,
				get: c => c.RefundMaterial,
				set: (c, v) => c.RefundMaterial = v,
				min: 0,
				max: 100,
				interval: 10,
				format: v => $"{v}%"
			)
			.Add(
				name: I18n.Setting_RefundCurrency,
				tooltip: I18n.Setting_RefundCurrency_Tip,
				get: c => c.RefundCurrency,
				set: (c, v) => c.RefundCurrency = v,
				min: 0,
				max: 100,
				interval: 10,
				format: v => $"{v}%"
			);
	}

	#endregion

	#region Extra Cost

	public IEnumerable<IIngredient>? GetAdditionalCost() {
		if (BCAPI is null)
			return null;

		if (HasCachedAdditionalCost)
			return CachedAdditionalCost;

		HasCachedAdditionalCost = true;
		CachedAdditionalCost = null;

		if (string.IsNullOrWhiteSpace(Config.CostAdditional))
			return null;

		string[] parts = Config.CostAdditional.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		if (parts.Length == 0)
			return null;

		List<IIngredient> ingredients = new();
		CachedAdditionalCost = ingredients;

		foreach(string part in parts) {
			int idx = part.IndexOf(':');
			if ( idx == -1 ) {
				Log($"Invalid additional cost entry. No delimiter for quantity in: \"{part}\"", LogLevel.Warn);
				ingredients.Add(BCAPI.CreateErrorIngredient());
				continue;
			}

			if (!int.TryParse(part[..idx], out int amount)) {
				Log($"Invalid quantity parsing additional cost entry: \"{part}\"", LogLevel.Warn);
				ingredients.Add(BCAPI.CreateErrorIngredient());
				continue;
			}

			Item? item = InventoryHelper.CreateItemById(part[(idx + 1)..], amount, allow_null: true);
			if (item is null) {
				Log($"Invalid item in additional cost entry: \"{part}\"", LogLevel.Warn);
				ingredients.Add(BCAPI.CreateErrorIngredient());
				continue;
			}

			if (item is not StardewValley.Object sobj || sobj.bigCraftable.Value) {
				Log($"Unsupported item in additional cost entry: \"{part}\"", LogLevel.Warn);
				ingredients.Add(BCAPI.CreateErrorIngredient());
				continue;
			}

			ingredients.Add(BCAPI.CreateBaseIngredient(sobj.ParentSheetIndex, amount));
		}

		return CachedAdditionalCost;
	}

	#endregion

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

		recipes.Add(new ActionRecipe(ActionType.Move, this));
		recipes.Add(new ActionRecipe(ActionType.Paint, this));
		recipes.Add(new ActionRecipe(ActionType.Demolish, this));

		foreach (var bp in blueprints.Values)
			recipes.Add(new BPRecipe(bp, this));

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
