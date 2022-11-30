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
using System.IO;
using System.Linq;
using System.Reflection;

using HarmonyLib;

using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.Integrations.GenericModConfigMenu;
using Leclair.Stardew.GiantCropTweaks.Integrations.GiantCropFertilizer;
using Leclair.Stardew.GiantCropTweaks.Models;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace Leclair.Stardew.GiantCropTweaks;

public class ModEntry : ModSubscriber {

	public const string GIANT_CROP_DATA = @"Data\GiantCrops";

	public const string MD_ID = "leclair.giantcroptweaks/Id";
	public const string MD_PROTECT = "leclair.giantcroptweaks/UnderCrop";

#nullable disable

	public static ModEntry Instance { get; private set; }
	public ModConfig Config { get; private set; }

#nullable enable

	internal readonly Dictionary<IManifest, ModApi> ApiInstances = new();

	internal readonly static MethodInfo OneTimeRandom_GetDouble = AccessTools.Method("StardewValley.OneTimeRandom:GetDouble");

	internal readonly Dictionary<string, Lazy<Texture2D>> PackTextures = new();

	internal Dictionary<string, GiantCrops>? CropData;

	internal Dictionary<string, IGiantCropData>? ApiData;

	internal Dictionary<int, int>? HarvestToCropMap;

	internal readonly PerScreen<bool> CanBreakCrops = new(() => true);

	internal Harmony? Harmony;

	// Integrations
	internal GMCMIntegration<ModConfig, ModEntry>? intGMCM;
	internal GCFIntegration? intGCF;

	// Entry
	public override void Entry(IModHelper helper) {
		base.Entry(helper);

		Instance = this;

		// Harmony
		Harmony = new Harmony(ModManifest.UniqueID);

		Patches.Crop_Patches.Patch(this);
		Patches.GameLocation_Patches.Patch(this);
		Patches.GiantCrop_Patches.Patch(this);
		Patches.HoeDirt_Patches.Patch(this);

		// Read Config
		Config = Helper.ReadConfig<ModConfig>();

		// Other Init
		I18n.Init(Helper.Translation);
	}

	public override object? GetApi(IModInfo mod) {
		if (!ApiInstances.TryGetValue(mod.Manifest, out var api)) {
			api = new ModApi(this, mod.Manifest);
			ApiInstances.Add(mod.Manifest, api);
		}

		return api;
	}

	#region Configuration

	public void SaveConfig() {
		Helper.WriteConfig(Config);
	}

	public void ResetConfig() {
		Config = new();
	}

	public void RegisterConfig() {
		intGMCM ??= new(this, () => Config, ResetConfig, SaveConfig);
		if (!intGMCM.IsLoaded)
			return;

		intGMCM.Unregister();
		intGMCM.Register(true);

		intGMCM
			.Add(
				I18n.Setting_AllowIslandWest,
				I18n.Setting_AllowIslandWest_Tooltip,
				c => c.AllowInIslandWest,
				(c, v) => c.AllowInIslandWest = v
			);
	}

	#endregion

	#region Events

	[Subscriber]
	private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
		intGCF = new(this);
		RegisterConfig();

		Helper.ConsoleCommands.Add("gct_update", "Reload our giant crop data from disk.", (_, _) => {
			Helper.GameContent.InvalidateCache(@"Data\GiantCrops");
			Helper.GameContent.InvalidateCache(asset => asset.Name.StartsWith(@"Mods/leclair.giantcroptweaks/Texture/"));

			LoadCropData();

			Log($"Invalidated cached content.", LogLevel.Info);
		});

		Helper.ConsoleCommands.Add("gct_fix", "Fix the protected dirt tiles on the current map.", (_, _) => {
			if (Game1.currentLocation is null)
				return;

			int removed = 0;
			int added = 0;

			// Handle additions first.
			HashSet<Vector2> valid = new();

			foreach(var entry in Game1.currentLocation.resourceClumps) {
				if (entry is GiantCrop crop)
					added += ProtectGiantCropDirt(Game1.currentLocation, crop, valid);
			}

			// Now, check every HoeDirt to see if it should be removed.
			foreach(var entry in Game1.currentLocation.terrainFeatures.Pairs) {
				if (entry.Value is HoeDirt dirt && ! valid.Contains(entry.Key) && dirt.modData.ContainsKey(MD_PROTECT)) {
					removed++;
					dirt.modData.Remove(MD_PROTECT);
				}
			}

			Log($"Added protection to {added} tiles and removed protection from {removed} tiles.", LogLevel.Info);
		});
	}

	[Subscriber]
	private void OnAssetInvalidated(object? sender, AssetsInvalidatedEventArgs e) {
		foreach(var name in e.Names) {
			if (name.IsEquivalentTo(@"Data\GiantCrops")) {
				CropData = null;
				ApiData = null;
			}

			if (name.IsEquivalentTo(@"Data\Crops")) {
				HarvestToCropMap = null;
			}
		}
	}

	[Subscriber]
	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {
		if (e.Name.IsEquivalentTo(@"Data\GiantCrops"))
			e.LoadFrom(LoadAssetCrops, AssetLoadPriority.Low);

		if (e.Name.StartsWith(@"Mods/leclair.giantcroptweaks/PackTexture/")) {
			string key = e.Name.ToString()![41..];
			if (PackTextures.ContainsKey(key))
				e.LoadFrom(() => PackTextures[key].Value, AssetLoadPriority.Low);
		}

		if (e.Name.StartsWith(@"Mods/leclair.giantcroptweaks/Texture/"))
			e.LoadFromModFile<Texture2D>($"assets/{e.Name.ToString()![37..]}", AssetLoadPriority.Low);

	}

	[Subscriber]
	private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e) {
		CanBreakCrops.ResetAllScreens();
	}

	[Subscriber]
	private void OnTerrainFeaturesChanged(object? sender, TerrainFeatureListChangedEventArgs e) {
		var features = e.Location.terrainFeatures;
		int count = 0;
		foreach(var entry in e.Removed) {
			if (entry.Value is HoeDirt dirt && dirt.modData.ContainsKey(MD_PROTECT)) {
				// Surely this is too stupid to work.
				count++;
				if (!features.ContainsKey(entry.Key))
					features[entry.Key] = entry.Value;
			}
		}

		if (count > 0)
			Log($"Preserving {count} dirt tiles in {e.Location.NameOrUniqueName}.", LogLevel.Trace);
	}

	#endregion

	#region Loading

	[MemberNotNull(nameof(CropData))]
	[MemberNotNull(nameof(ApiData))]
	internal void LoadCropData() {
		if (CropData is null || ApiData is null) {
			CropData = Helper.GameContent.Load<Dictionary<string, GiantCrops>>(@"Data\GiantCrops");
			ApiData = new();
			foreach (var entry in CropData)
				ApiData[entry.Key] = entry.Value;
		}
	}

	private Dictionary<string, GiantCrops> LoadAssetCrops() {
		Dictionary<string, GiantCrops>? result = null;

		try {
			result = Helper.ModContent.Load<Dictionary<string, GiantCrops>>(@"assets\crops.json");

		} catch(Exception ex) {
			if (!ex.Message.Contains("path doesn't exist"))
				Log($"crops.json is missing or invalid.", LogLevel.Warn, ex);
		}

		if (result is not null) {
			foreach(var entry in result)
				entry.Value.ID = entry.Key;
		}

		PackTextures.Clear();

		foreach(var pack in Helper.ContentPacks.GetOwned()) {
			foreach(string absolute in Directory.EnumerateDirectories(pack.DirectoryPath)) {
				string folder = Path.GetRelativePath(pack.DirectoryPath, absolute);
				if (!pack.HasFile($"{folder}/crop.json"))
					continue;

				GiantCrops? data;

				try {
					data = pack.ReadJsonFile<GiantCrops>($"{folder}/crop.json");
				} catch(Exception ex) {
					Log($"Unable to load \"${folder}/crop.json\" from {pack.Manifest.Name} ({pack.Manifest.UniqueID}): {ex}", LogLevel.Warn);
					continue;
				}

				if (data is null || string.IsNullOrEmpty(data.ID))
					continue;

				if (string.IsNullOrEmpty(data.Texture)) {
					if (!pack.HasFile($"{folder}/crop.png")) {
						Log($"Crop at \"${folder}/crop.json\" has no texture. Skipping from {pack.Manifest.Name} ({pack.Manifest.UniqueID})", LogLevel.Error);
						continue;
					}

					PackTextures[data.ID] = new Lazy<Texture2D>(() => pack.ModContent.Load<Texture2D>($"{folder}/crop.png"));
					data.Texture = $"Mods/leclair.giantcroptweaks/PackTexture/{data.ID}";
				}

				result ??= new();
				result[data.ID] = data;
			}
		}

		return result ?? new();
	}

	#endregion

	#region Handler

	/// <summary>
	/// Get the <see cref="GiantCrops"/> data instance for a specific giant
	/// crop, if one is available. This is likely to return <c>null</c> for
	/// giant crops added by other mods.
	/// </summary>
	/// <param name="id">The harvest ID of the giant crop we want data on.</param>
	public GiantCrops? GetGiantCropData(string id) {
		LoadCropData();
		if (CropData.TryGetValue(id, out var data))
			return data;
		return null;
	}

	/// <summary>
	/// Look up which crop provides the harvest product we have. Used for turning
	/// giant crops back into not-so-big crops. This also only returns a value
	/// for crops with regrowth times, to simplify things.
	/// </summary>
	/// <param name="harvest">The harvest ID.</param>
	public int? GetCropIdFromHarvest(int harvest) {
		if (HarvestToCropMap is null) {
			HarvestToCropMap = new();
			var crops = Helper.GameContent.Load<Dictionary<int, string>>(@"Data\Crops");
			foreach (var entry in crops) {
				string[] parts = entry.Value.Split('/');
				if (parts.Length < 5 || !int.TryParse(parts[3], out int prod) || !int.TryParse(parts[4], out int days) || days < 0)
					continue;

				HarvestToCropMap.TryAdd(prod, entry.Key);
			}
		}

		if (HarvestToCropMap.TryGetValue(harvest, out int val))
			return val;

		return null;
	}

	/// <summary>
	/// Check to see if giant crops are allowed to grow in a given location.
	/// This uses the same logic as the 1.6 update.
	/// </summary>
	/// <param name="location">The location to check.</param>
	public GameLocation? IsGiantCropLocation(GameLocation location) {
		// Vanilla 1.5.6 Behavior
		if (location is Farm)
			return location;

		// Custom Behavior
		if (Config.AllowInIslandWest && location is IslandWest)
			return location;

		// Vanilla 1.6 Behavior
		if (!string.IsNullOrEmpty(location.getMapProperty("AllowGiantCrops")))
			return location;

		return null;
	}

	/// <summary>
	/// Check to see if a giant crop should grow at a specific position on
	/// a specific day in a specific seed with a specific fertilizer. This
	/// replicated the default logic, but with a customizable chance as
	/// well as with support for the Giant Crop Fertilizer mod.
	/// </summary>
	/// <param name="chance">The percentage chance of a giant crop spawning, from 0 to 1</param>
	/// <param name="fertilizer">The fertilizer being used on the <see cref="HoeDirt"/> where this crop is growing</param>
	/// <param name="uniqueId">The unique ID of the save</param>
	/// <param name="daysPlayed">The number of days played of the save</param>
	/// <param name="x">The x coordinate within the map of the <see cref="HoeDirt"/></param>
	/// <param name="y">The y coordinate within the map of the <see cref="HoeDirt"/></param>
	public ShouldGrow ShouldGiantCropGrow(double chance, int fertilizer, ulong uniqueId, ulong daysPlayed, ulong x, ulong y) {
		// Try to get the standard value.
		if (OneTimeRandom_GetDouble!.Invoke(null, new object[] { uniqueId, daysPlayed, x, y }) is not double dbl)
			return ShouldGrow.False;

		// Did we succeed?
		if (dbl < chance)
			return ShouldGrow.True;

		// Giant Crop Fertilizer Mod
		if (fertilizer != -1 && fertilizer == (intGCF?.GiantCropFertilizerID ?? -1)) {
			chance = Math.Max(chance, intGCF?.FertilizerChance ?? 0);

			// Did we succeed now?
			if (dbl < chance)
				return ShouldGrow.WithFertilizer;
		}

		// ... we did not.
		return ShouldGrow.False;
	}

	/// <summary>
	/// Try growing a crop into a giant crop. This is called by the Crop
	/// harmony patches.
	/// </summary>
	/// <param name="location">The location this is happening in</param>
	/// <param name="crop">The crop we're trying to grow out</param>
	/// <param name="state">Whether or not it was watered</param>
	/// <param name="fertilizer">The fertilizer used on the crop</param>
	/// <param name="xTile">The X tile position of the crop</param>
	/// <param name="yTile">The Y tile position of the crop</param>
	public void TryGrowingGiantCrop(GameLocation location, Crop crop, int state, int fertilizer, int xTile, int yTile, bool force = false) {
		// If a giant crop was spawned some other way in 1.5, flag its dirt too.
		if (location.resourceClumps.Count > 0 && location.resourceClumps.Last() is GiantCrop gc && gc.tile.X == xTile - 1 && gc.tile.Y == yTile - 1)
			ProtectGiantCropDirt(location, gc);

		// If the crop isn't in its last phase, or this isn't a giant crop location, don't continue. Skip if forced.
		if (!force && (crop.currentPhase.Value != crop.phaseDays.Count - 1 || IsGiantCropLocation(location) is null))
			return;

		// For regrowing crops, only do this on the day they regrow. Skip if forced.
		if (!force && crop.fullyGrown.Value && crop.dayOfCurrentPhase.Value > 0)
			return;

		// If there's no dirt there or it isn't our crop, don't continue.
		if (!location.terrainFeatures.TryGetValue(new Vector2(xTile, yTile), out var tile) || tile is not HoeDirt hd || hd.crop != crop)
			return;

		// Get the data for this crop.
		LoadCropData();
		string key = crop.indexOfHarvest.Value.ToString();
		if (!CropData.TryGetValue(key, out var data))
			return;

		// Determine if this crop *should* grow to giant size. Always grow if force.
		ShouldGrow should = force ? ShouldGrow.True : ShouldGiantCropGrow(data.Chance, fertilizer, Game1.uniqueIDForThisGame, Game1.stats.daysPlayed, (ulong) xTile, (ulong) yTile);
		if (should == ShouldGrow.False)
			return;

		// Check to see if we have a full grid of crops in the dimensions we need.
		for(int x = xTile; x < xTile + data.TileSize.X; x++) {
			for(int y = yTile; y < yTile + data.TileSize.Y; y++) {
				Vector2 v = new(x, y);
				if (!location.terrainFeatures.TryGetValue(v, out var tf) || tf is not HoeDirt hd2 || hd2.crop == null || hd2.crop.indexOfHarvest.Value != crop.indexOfHarvest.Value)
					return;
			}
		}

		// Still here? Time to grow!

		// Clear the existing crops from all tiles.
		for (int x = xTile; x < xTile + data.TileSize.X; x++) {
			for (int y = yTile; y < yTile + data.TileSize.Y; y++) {
				Vector2 v = new(x, y);
				if (location.terrainFeatures[v] is HoeDirt hd3) {
					hd3.crop = null;

					// Maybe remove our fertilizer?
					if (should == ShouldGrow.WithFertilizer && x == xTile && y == yTile)
						hd3.fertilizer.Value = HoeDirt.noFertilizer;
				}
			}
		}

		// And add the new GiantCrop
		GiantCrop giant = new(crop.indexOfHarvest.Value, new Vector2(xTile, yTile));

		// Set up appropriate data.
		giant.modData[MD_ID] = key;
		giant.width.Value = data.TileSize.X;
		giant.height.Value = data.TileSize.Y;

		location.resourceClumps.Add(giant);
		ProtectGiantCropDirt(location, giant);
	}

	/// <summary>
	/// Set mod data on each square of <see cref="HoeDirt"/> under a giant crop
	/// to prevent the dirt from being removed.
	/// </summary>
	/// <param name="location">The location where this is happening</param>
	/// <param name="crop">The giant crop to protect</param>
	/// <param name="tiles">A hashset of tile positions where crops are, used internally.</param>
	/// <returns>The number of tiles that had protection added.</returns>
	public int ProtectGiantCropDirt(GameLocation location, GiantCrop crop, HashSet<Vector2>? tiles = null) {
		// Where exactly did this happen?
		int originX = (int) crop.tile.Value.X;
		int originY = (int) crop.tile.Value.Y;

		int width = crop.width.Value;
		int height = crop.height.Value;

		int count = 0;

		for (int xOffset = 0; xOffset < width; xOffset++) {
			for (int yOffset = 0; yOffset < height; yOffset++) {
				int x = originX + xOffset;
				int y = originY + yOffset;
				Vector2 pos = new(x, y);

				if (location.terrainFeatures.TryGetValue(pos, out var feature) && feature is HoeDirt dirt) {
					tiles?.Add(pos);
					if (!dirt.modData.ContainsKey(MD_PROTECT)) {
						dirt.modData[MD_PROTECT] = "1";
						count++;
					}
				}
			}
		}

		return count;
	}

	/// <summary>
	/// Remove mod data on each square of <see cref="HoeDirt"/> under a giant
	/// crop to allow the dirt to be removed again.
	/// </summary>
	/// <param name="location">The location where this is happening</param>
	/// <param name="crop">The giant crop to protect</param>
	public void UnprotectGiantCropDirt(GameLocation location, GiantCrop crop) {
		// Where exactly did this happen?
		int originX = (int) crop.tile.Value.X;
		int originY = (int) crop.tile.Value.Y;

		int width = crop.width.Value;
		int height = crop.height.Value;

		for (int xOffset = 0; xOffset < width; xOffset++) {
			for (int yOffset = 0; yOffset < height; yOffset++) {
				int x = originX + xOffset;
				int y = originY + yOffset;
				if (location.terrainFeatures.TryGetValue(new Vector2(x, y), out var feature) && feature is HoeDirt dirt)
					dirt.modData.Remove(MD_PROTECT);
			}
		}
	}

	/// <summary>
	/// This method runs when a GiantCrop is removed, and is responsible for
	/// restoring the dirt to its previous condition.
	/// </summary>
	/// <param name="location">The location where this is happening</param>
	/// <param name="crop">The giant crop that was removed</param>
	public void OnGiantCropRemoved(GameLocation location, GiantCrop crop) {
		// Where exactly did this happen?
		int originX = (int) crop.tile.Value.X;
		int originY = (int) crop.tile.Value.Y;

		Log($"Removed crop at {originX},{originY} in {location.NameOrUniqueName}: {crop.parentSheetIndex.Value}: {crop.which.Value}", LogLevel.Trace);

		int? cval = GetCropIdFromHarvest(crop.parentSheetIndex.Value);
		int width = crop.width.Value;
		int height = crop.height.Value;

		int i = -1;

		for(int xOffset = 0; xOffset < width; xOffset++) {
			for(int yOffset = 0; yOffset < height; yOffset++) {
				i++;
				int x = originX + xOffset;
				int y = originY + yOffset;
				Vector2 pos = new(x, y);

				// Check to see if we have existing HoeDirt, or if there's some
				// other terrain feature at that position already.
				HoeDirt? dirt = null;
				if (location.terrainFeatures.TryGetValue(pos, out var feature)) {
					if (feature is HoeDirt hd)
						dirt = hd;
					else if (feature is not null) {
						// just ignore tiles that have some weird terrain feature
						continue;
					}
				}

				// If we don't have existing dirt, make new dirt.
				if (dirt is null) {
					dirt = new HoeDirt(0, location);
					if (Game1.IsRainingHere(location))
						dirt.state.Value = 1;

					Log($"Had to recreate dirt at {originX},{originY} in {location.NameOrUniqueName}.", LogLevel.Trace);
					location.terrainFeatures[pos] = dirt;
				}

				// Make sure it isn't flagged for protection anymore.
				dirt.modData.Remove(MD_PROTECT);

				// Restore Crop
				if (cval.HasValue && dirt.crop is null) {
					dirt.crop = new Crop(cval.Value, x, y);
					dirt.crop.fullyGrown.Value = true;
					dirt.crop.currentPhase.Value = dirt.crop.phaseDays.Count - 1;
					dirt.crop.dayOfCurrentPhase.Value = dirt.crop.regrowAfterHarvest.Value;
					dirt.crop.updateDrawMath(pos);

					CanBreakCrops.Value = false;
				}
			}
		}
	}

	#endregion
}

public enum ShouldGrow {
	False = 0,
	True = 1,
	WithFertilizer = 2
};
