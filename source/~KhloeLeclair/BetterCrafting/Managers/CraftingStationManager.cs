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

using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.Events;

using StardewModdingAPI.Events;

using StardewValley;

namespace Leclair.Stardew.BetterCrafting.Managers;

public class CraftingStationManager : BaseManager {

	public static readonly string CRAFTING_STATIONS_PATH = @"Mods/leclair.bettercrafting/CraftingStations";

	private Dictionary<string, CraftingStation>? Stations = null;

	private HashSet<string>? PrivateCraftingRecipes;
	private HashSet<string>? PrivateCookingRecipes;

	public CraftingStationManager(ModEntry mod) : base(mod) {

	}


	#region Events

	[Subscriber]
	private void OnAssetInvalidated(object? sender, AssetsInvalidatedEventArgs e) {
		foreach (var name in e.NamesWithoutLocale)
			if (name.IsEquivalentTo(CRAFTING_STATIONS_PATH)) {
				Stations = null;
				break;
			}
	}

	[Subscriber]
	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {
		if (e.Name.IsEquivalentTo(CRAFTING_STATIONS_PATH))
			e.LoadFrom(LoadCraftingStationsFromFiles, AssetLoadPriority.Exclusive);
	}

	#endregion

	#region Loading

	public void Invalidate() {
		Stations = null;
	}

	private Dictionary<string, CraftingStation> LoadCraftingStationsFromFiles() {

		Dictionary<string, CraftingStation> result = new();

		CraftingStation[]? stations;

		foreach(var cp in Mod.Helper.ContentPacks.GetOwned()) {
			if (!cp.HasFile("stations.json"))
				continue;

			stations = null;
			try {
				stations = cp.ReadJsonFile<CraftingStation[]>("stations.json");
			} catch(Exception ex) {
				Log($"The stations.json file of {cp.Manifest.Name} is invalid.", StardewModdingAPI.LogLevel.Error, ex);
			}

			if (stations != null)
				foreach(var station in stations) {
					if (string.IsNullOrEmpty(station.Id)) {
						Log($"Station from {cp.Manifest.Name} is invalid: missing Id", StardewModdingAPI.LogLevel.Warn);
						continue;
					}

					if (result.ContainsKey(station.Id)) {
						Log($"Skipping duplicate station from {cp.Manifest.Name}: {station.Id}", StardewModdingAPI.LogLevel.Warn);
						continue;
					}

					result[station.Id] = station;
				}
		}

		return result;

	}

	[MemberNotNull(nameof(Stations))]
	[MemberNotNull(nameof(PrivateCraftingRecipes))]
	[MemberNotNull(nameof(PrivateCookingRecipes))]
	public void LoadStations() {
		if (Stations != null && PrivateCraftingRecipes != null && PrivateCookingRecipes != null)
			return;

		Stations = Game1.content.Load<Dictionary<string, CraftingStation>>(CRAFTING_STATIONS_PATH);

		PrivateCraftingRecipes = new();
		PrivateCookingRecipes = new();

		foreach(var station in Stations) {
			if (string.IsNullOrEmpty(station.Value.Id) || station.Value.Id != station.Key)
				station.Value.Id = station.Key;

			if (station.Value.Recipes is null)
				station.Value.Recipes = Array.Empty<string>();

			if (station.Value.AreRecipesExclusive) {
				var target = station.Value.IsCooking ? PrivateCookingRecipes : PrivateCraftingRecipes;

				foreach(string recipe in station.Value.Recipes)
					target.Add(recipe);
			}

			// TODO: Other stuff, probably.
		}
	}

	#endregion

	#region Access

	public IEnumerable<string> GetExclusiveRecipes(bool cooking) {
		LoadStations();

		var target = cooking ? PrivateCookingRecipes : PrivateCraftingRecipes;
		foreach(string recipe in target)
			yield return recipe;

		var ccs = Mod.intCCStation?.GetRemovedRecipes(cooking);
		if (ccs is not null)
			foreach (string recipe in ccs)
				yield return recipe;
	}

	public IEnumerable<IRecipe> GetNonExclusiveRecipes(bool cooking) {

		LoadStations();
		var target = cooking ? PrivateCookingRecipes : PrivateCraftingRecipes;
		var ccs = Mod.intCCStation?.GetRemovedRecipes(cooking);

		// Quick abort without a filter.
		if (target.Count == 0 && (ccs?.Count ?? 0) == 0)
			return Mod.Recipes.GetRecipes(cooking);

		return Mod.Recipes.GetRecipes(cooking).Where(recipe => {
			if (target.Contains(recipe.Name))
				return false;
			if (ccs != null && ccs.Contains(recipe.Name))
				return false;
			return true;
		});
	}

	public bool IsStation(string name) {
		LoadStations();
		return Stations.ContainsKey(name);
	}

	public IEnumerable<CraftingStation> GetStations() {
		LoadStations();
		return Stations.Values;
	}

	public bool TryGetStation(string name, Farmer who, [NotNullWhen(true)] out CraftingStation? station) {
		LoadStations();

		// TODO: Logic for saving categories for custom crafting stations.

		return Stations.TryGetValue(name, out station);
	}

	#endregion


}
