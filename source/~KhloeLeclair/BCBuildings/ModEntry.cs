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

using Leclair.Stardew.BetterCrafting;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.Integrations.GenericModConfigMenu;

using Newtonsoft.Json.Linq;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

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
	internal BuildingRenderer Renderer;
#nullable enable

	private IEnumerable<IIngredient>? CachedAdditionalCost;
	private bool HasCachedAdditionalCost = false;

	private GMCMIntegration<ModConfig, ModEntry>? GMCMIntegration;

	internal IBetterCrafting? BCAPI;

	private Dictionary<string, BuildingRecipe>? RecipesById;


	public override void Entry(IModHelper helper) {
		base.Entry(helper);

		Renderer = new(this);

		SpriteHelper.SetHelper(Helper);
		I18n.Init(Helper.Translation);

		// Read Config
		Config = Helper.ReadConfig<ModConfig>();

	}

	#region Events

	[Subscriber]
	private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
		RegisterConfig();

		BCAPI = Helper.ModRegistry.GetApi<IBetterCrafting>("leclair.bettercrafting");
		if (BCAPI is null)
			return;

		BCAPI.AddRecipeProvider(this);
		BCAPI.RegisterRuleHandler("Building", new BuildingRuleHandler(this));
		BCAPI.CreateDefaultCategory(
			cooking: false,
			categoryId: CategoryID,
			Name: I18n.Category_Name,
			iconRecipe: "bcbuildings:Barn",
			useRules: true,
			rules: [
				new RuleData(BCAPI.GetAbsoluteRuleId("Building"))
			]
		);

		BCAPI.ReportRecipeType(typeof(BuildingRecipe));
		BCAPI.ReportRecipeType(typeof(ActionRecipe));
		BCAPI.ReportRecipeType(typeof(RenovateFarmhouseRecipe));
		BCAPI.ReportRecipeType(typeof(UpgradeFarmhouseRecipe));
	}

	[Subscriber]
	private void OnAssetInvalidated(object? sender, AssetsInvalidatedEventArgs e) {
		foreach (var name in e.Names) {
			if (name.IsEquivalentTo(@"Data/Buildings"))
				RecipesById = null;
		}
	}

	#endregion

	#region Configuration

	public void SaveConfig() {
		Helper.WriteConfig(Config);
		BCAPI?.InvalidateRecipeCache();
		HasCachedAdditionalCost = false;
		RecipesById = null;
	}

	public void ResetConfig() {
		Config = new();
		HasCachedAdditionalCost = false;
		RecipesById = null;
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

		GMCMIntegration
			.Add(
				I18n.Setting_GreenhouseMove,
				I18n.Setting_GreenhouseMove_Tip,
				c => c.AllowMovingUnfinishedGreenhouse,
				(c, v) => c.AllowMovingUnfinishedGreenhouse = v
			)
			.Add(
				I18n.Setting_AllowHouseUpgrades,
				I18n.Setting_AllowHouseUpgrades_Tip,
				c => c.AllowHouseUpgrades,
				(c, v) => c.AllowHouseUpgrades = v
			)
			.Add(
				I18n.Setting_AllowHouseRenovation,
				I18n.Setting_AllowHouseRenovation_Tip,
				c => c.AllowHouseRenovation,
				(c, v) => c.AllowHouseRenovation = v
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

		foreach (string part in parts) {
			int idx = part.IndexOf(':');
			if (idx == -1) {
				Log($"Invalid additional cost entry. No delimiter for quantity in: \"{part}\"", LogLevel.Warn);
				ingredients.Add(BCAPI.CreateErrorIngredient());
				continue;
			}

			if (!int.TryParse(part[..idx], out int amount)) {
				Log($"Invalid quantity parsing additional cost entry: \"{part}\"", LogLevel.Warn);
				ingredients.Add(BCAPI.CreateErrorIngredient());
				continue;
			}

			Item? item = ItemRegistry.Create(part[(idx + 1)..], amount, allowNull: true);
			if (item is null) {
				Log($"Invalid item in additional cost entry: \"{part}\"", LogLevel.Warn);
				ingredients.Add(BCAPI.CreateErrorIngredient());
				continue;
			}

			if (item is not SObject sobj) {
				Log($"Unsupported item in additional cost entry: \"{part}\"", LogLevel.Warn);
				ingredients.Add(BCAPI.CreateErrorIngredient());
				continue;
			}

			ingredients.Add(BCAPI.CreateBaseIngredient(sobj.QualifiedItemId, amount));
		}

		return CachedAdditionalCost;
	}

	#endregion

	#region Loading

	[MemberNotNull(nameof(RecipesById))]
	private void LoadRecipes() {
		if (RecipesById != null)
			return;

		var buildings = DataLoader.Buildings(Game1.content);
		RecipesById = new();

		foreach (var building in buildings) {
			RecipesById[building.Key] = new BuildingRecipe(this, building.Key, null, building.Value);

			if (building.Value.Skins != null)
				foreach (var skin in building.Value.Skins) {
					if (skin.ShowAsSeparateConstructionEntry)
						RecipesById[$"{building.Key}/{skin.Id}"] = new BuildingRecipe(this, building.Key, skin.Id, building.Value);
				}
		}
	}

	public bool TryGetRecipe(string buildingId, string? skinId, [NotNullWhen(true)] out BuildingRecipe? recipe) {
		LoadRecipes();

		if (!string.IsNullOrEmpty(skinId) && RecipesById.TryGetValue($"{buildingId}/{skinId}", out recipe))
			return true;

		return RecipesById.TryGetValue(buildingId, out recipe);
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
			yield break;

		//RecipesById = null;
		LoadRecipes();

		foreach (var recipe in RecipesById.Values) {
			string builder = recipe.Builder;
			bool okay = builder == "Robin";
			if (builder == "Wizard")
				okay = Game1.player.hasMagicInk || Game1.player.mailReceived.Contains("hasPickedUpMagicInk");

			if (!okay)
				continue;

			if (!string.IsNullOrEmpty(recipe.Data.BuildCondition) && !GameStateQuery.CheckConditions(recipe.Data.BuildCondition, Game1.currentLocation))
				continue;

			yield return recipe;
		}

		if (Config.AllowHouseUpgrades)
			yield return new UpgradeFarmhouseRecipe(this);

		yield return new ActionRecipe(ActionType.Move, this);
		yield return new ActionRecipe(ActionType.Paint, this);
		yield return new ActionRecipe(ActionType.Demolish, this);

		if (Config.AllowHouseRenovation)
			foreach (var renovation in HouseRenovation.GetAvailableRenovations())
				if (renovation is HouseRenovation hr)
					yield return new RenovateFarmhouseRecipe(this, hr);

	}

	#endregion

}
