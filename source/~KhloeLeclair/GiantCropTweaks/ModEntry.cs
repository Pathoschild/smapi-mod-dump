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

using HarmonyLib;

using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.Integrations.GenericModConfigMenu;
using Leclair.Stardew.GiantCropTweaks.Models;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.GameData.GiantCrops;
using StardewValley.TerrainFeatures;

namespace Leclair.Stardew.GiantCropTweaks;

public class ModEntry : ModSubscriber {

	public const string CUSTOM_CROP_DATA = @"Mods/leclair.giantcroptweaks/Data";

	public const string MD_PROTECT = @"leclair.giantcroptweaks/UnderCrop";
	public const string MD_COLOR = @"leclair.giantcroptweaks/Color";

#nullable disable

	public static ModEntry Instance { get; private set; }
	public ModConfig Config { get; private set; }

#nullable enable

	internal Dictionary<string, ExtraGiantCropData>? CropData;
	internal HashSet<string>? CropsWithRegrowthRestriction;
	internal readonly Dictionary<string, KeyValuePair<string, CropData>?> UnderlyingCropCache = new();

	internal readonly Dictionary<string, Texture2D?> TextureCache = new();

	internal readonly PerScreen<bool> CanBreakCrops = new(() => true);

	internal Harmony? Harmony;

	// Integrations
	internal GMCMIntegration<ModConfig, ModEntry>? intGMCM;

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
		Patches.Crop_Patches.AllowedLocations = Config.AllowedLocations;

		// Other Init
		I18n.Init(Helper.Translation);
	}

	public override object? GetApi(IModInfo mod) {
		return new ModApi(this, mod.Manifest);
	}

	#region Configuration

	public void SaveConfig() {
		Helper.WriteConfig(Config);
		Patches.Crop_Patches.AllowedLocations = Config.AllowedLocations;
	}

	public void ResetConfig() {
		Config = new();
		Patches.Crop_Patches.AllowedLocations = Config.AllowedLocations;
	}

	public void RegisterConfig() {
		intGMCM ??= new(this, () => Config, ResetConfig, SaveConfig);
		if (!intGMCM.IsLoaded)
			return;

		intGMCM.Unregister();
		intGMCM.Register(true);

		intGMCM
			.AddChoice(
				I18n.Setting_AllowedLocation,
				I18n.Setting_AllowedLocation_Tooltip,
				c => c.AllowedLocations,
				(c, v) => c.AllowedLocations = v,
				new Dictionary<AllowedLocations, Func<string>> {
					{ AllowedLocations.BaseGame, I18n.Setting_AllowedLocation_BaseGame },
					{ AllowedLocations.BaseAndIsland, I18n.Setting_AllowedLocation_BaseAndIsland },
					{ AllowedLocations.Anywhere, I18n.Setting_AllowedLocation_Anywhere }
				}
			);
	}

	#endregion

	#region Events

	[Subscriber]
	private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
		RegisterConfig();

		Helper.ConsoleCommands.Add("gct_update", "Reload our additional giant crop data.", (_, _) => {
			Helper.GameContent.InvalidateCache(CUSTOM_CROP_DATA);
			Log($"Invalidated cached content.", LogLevel.Info);
		});

		Helper.ConsoleCommands.Add("gct_newcolors", "Clear the colors on all giant crops on the current map.", (_, _) => {
			int removed = 0;

			if (Game1.currentLocation is not null)
				foreach (var entry in Game1.currentLocation.resourceClumps) {
					if (entry is GiantCrop crop && crop.modData.Remove(MD_COLOR))
						removed++;
				}

			Log($"Reset cached color on {removed} crops.", LogLevel.Info);
		});

		Helper.ConsoleCommands.Add("gct_fixdirt", "Fix the protected dirt tiles on the current map.", (_, _) => {
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
			if (name.IsEquivalentTo(CUSTOM_CROP_DATA))
				CropData = null;

			if (name.IsEquivalentTo(@"Data/Crops") || name.IsEquivalentTo(@"Data/GiantCrops"))
				UnderlyingCropCache.Clear();

			// Make sure we don't have a texture cached from this asset.
			TextureCache.Remove(name.Name);
		}
	}

	[Subscriber]
	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {
		if (e.Name.IsEquivalentTo(CUSTOM_CROP_DATA))
			e.LoadFrom(InternalLoadCropData, AssetLoadPriority.Exclusive);

		/*if (e.Name.IsEquivalentTo(@"Data/GiantCrops"))
			e.Edit(editor => {
				var data = editor.AsDictionary<string, GiantCropData>();

				foreach (var entry in data.Data.Values)
					entry.Chance = 1.1f;

			}, AssetEditPriority.Late);*/
	}

	[Subscriber]
	private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e) {
		CanBreakCrops.Value = true;
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
	[MemberNotNull(nameof(CropsWithRegrowthRestriction))]
	internal void LoadCropData() {
		if (CropData is null) {
			CropData = Helper.GameContent.Load<Dictionary<string, ExtraGiantCropData>>(CUSTOM_CROP_DATA);
			CropsWithRegrowthRestriction = null;
		}

		CropsWithRegrowthRestriction ??= CropData
			.Where(pair => !pair.Value.CanGrowWhenNotFullyRegrown)
			.Select(pair => pair.Key)
			.ToHashSet();
	}

	private Dictionary<string, ExtraGiantCropData> InternalLoadCropData() {
		Dictionary<string, ExtraGiantCropData> result = new();

		// We don't have any built-in stuff yet. No texture pack support.

		return result;
	}

	public bool TryGetExtraData(string id, [NotNullWhen(true)] out ExtraGiantCropData? data) {
		LoadCropData();
		return CropData.TryGetValue(id, out data);
	}

	#endregion

	#region Handler

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
		int originX = (int) crop.Tile.X;
		int originY = (int) crop.Tile.Y;

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
		int originX = (int) crop.Tile.X;
		int originY = (int) crop.Tile.Y;

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


	public KeyValuePair<string, CropData>? GetUnderlyingCrop(GiantCropData? crop) {
		string? idToMatch = crop?.FromItemId;
		if (string.IsNullOrEmpty(idToMatch))
			return null;

		if (UnderlyingCropCache.TryGetValue(idToMatch, out var matched))
			return matched;

		string qualified_id = ItemRegistry.QualifyItemId(idToMatch);
		foreach (var pair in DataLoader.Crops(Game1.content))
			if (ItemRegistry.QualifyItemId(pair.Value.HarvestItemId) == qualified_id) {
				UnderlyingCropCache[idToMatch] = pair;
				return pair;
			}

		UnderlyingCropCache[idToMatch] = null;
		return null;
	}


	/// <summary>
	/// This method runs when a GiantCrop is removed, and is responsible for
	/// restoring the dirt to its previous condition.
	/// </summary>
	/// <param name="location">The location where this is happening</param>
	/// <param name="crop">The giant crop that was removed</param>
	public void OnGiantCropRemoved(GiantCrop crop) {
		// Where exactly did this happen?
		int originX = (int) crop.Tile.X;
		int originY = (int) crop.Tile.Y;

		Log($"Removed crop at {originX},{originY} in {crop.Location.NameOrUniqueName}: {crop.Id}", LogLevel.Trace);

		// Check the behavior.
		ReplantBehavior behavior = ReplantBehavior.WhenRegrowing;
		LoadCropData();
		if (CropData.TryGetValue(crop.Id, out var extraData))
			behavior = extraData.ShouldReplant;

		string? matchingCrop = null;

		// Try to get a matching crop.
		if (behavior != ReplantBehavior.Never) {
			var pair = GetUnderlyingCrop(crop.GetData());
			if (pair != null && (behavior == ReplantBehavior.Always || (behavior == ReplantBehavior.WhenRegrowing && pair.Value.Value.RegrowDays > -1)))
				matchingCrop = pair.Value.Key;
		}

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
				if (crop.Location.terrainFeatures.TryGetValue(pos, out var feature)) {
					if (feature is HoeDirt hd)
						dirt = hd;
					else if (feature is not null) {
						// just ignore tiles that have some weird terrain feature
						continue;
					}
				}

				// If we don't have existing dirt, make new dirt.
				if (dirt is null) {
					dirt = new HoeDirt(0, crop.Location);
					if (Game1.IsRainingHere(crop.Location))
						dirt.state.Value = 1;

					Log($"Had to recreate dirt at {originX},{originY} in {crop.Location.NameOrUniqueName}.", LogLevel.Trace);
					crop.Location.terrainFeatures[pos] = dirt;
				}

				// Make sure it isn't flagged for protection anymore.
				dirt.modData.Remove(MD_PROTECT);

				// Restore Crop
				if (matchingCrop != null && dirt.crop is null) {
					// Create a new crop instance. Let it update once to see if it dies.
					Crop c = new(matchingCrop, x, y, crop.Location);
					int regrowDays = c.GetData()?.RegrowDays ?? -1;
					c.newDay(dirt.state.Value);

					if (!c.dead.Value) {
						// We did it! Let's recreate the planting logic.
						dirt.crop = c;
						dirt.applySpeedIncreases(Game1.MasterPlayer);
						dirt.nearWaterForPaddy.Value = -1;
						if (dirt.hasPaddyCrop() && dirt.paddyWaterCheck()) {
							dirt.state.Value = 1;
							dirt.updateNeighbors();
						}

						// Now, fast forward the crop so it's ready to regrow.
						if (regrowDays > -1) {
							c.growCompletely();
							c.dayOfCurrentPhase.Value = regrowDays;
							c.updateDrawMath(c.tilePosition);
						}
					}
				}
			}
		}

		// Finally, flag the player as being unable to break crops for the rest of
		// this tick so that they don't immediately break one of the crops we just
		// respawned.
		CanBreakCrops.Value = false;
	}

	#endregion
}
