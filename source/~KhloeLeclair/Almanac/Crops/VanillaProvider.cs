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
using System.Collections.Generic;
using System.Linq;

using Leclair.Stardew.Common;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.GameData;

using StardewModdingAPI;
using StardewValley.GameData.Crops;
using StardewValley.GameData.GiantCrops;

namespace Leclair.Stardew.Almanac.Crops;

public class VanillaProvider : ICropProvider {

	public readonly ModEntry Mod;

	public VanillaProvider(ModEntry mod) {
		Mod = mod;
	}

	public string Name => nameof(VanillaProvider);

	public int Priority => 0;

	public IEnumerable<CropInfo> GetCrops() {
		Dictionary<string, CropData> crops = Game1.content.Load<Dictionary<string, CropData>>(@"Data\Crops");
		List<CropInfo> result = new();
		foreach (var entry in crops) {
			try {
				CropInfo? info = GetCropInfo(entry.Key, entry.Value);
				if (info.HasValue)
					result.Add(info.Value);
			} catch (Exception ex) {
				ModEntry.Instance.Log($"Unable to process crop: {entry.Key}", LogLevel.Warn, ex);
			}
		}
		return result;
	}

	private CropInfo? GetCropInfo(string id, CropData data) {
		// TODO: Create Crop instances so we can
		// lean more on vanilla logic.
		int spriteRow = data.SpriteIndex;
		string harvestID = data.HarvestItemId;
		int regrow = data.RegrowDays;
		bool isTrellisCrop = data.IsRaised;
		bool isPaddyCrop = data.IsPaddyCrop;
		if (!Game1.objectData.ContainsKey(harvestID))
			return null;
		List<Season> seasons = data.Seasons;
		WorldDate? startDate = null;
		WorldDate? endDate = null;
		// TODO: Handle weird crops with a gap.
		foreach (Season season in seasons) {
			WorldDate start;
			WorldDate end;
			try {
				start = new(1, season, 1);
				end = new(1, season, ModEntry.DaysPerMonth);
				// Sanity check the seasons, just in case.
				string test = start.SeasonKey;
				test = end.SeasonKey;
			} catch (Exception) {
				ModEntry.Instance.Log($"Invalid season for crop {id} (harvest:{harvestID}): {season}", LogLevel.Warn);
				return null;
			}
			if (startDate == null || startDate > start)
				startDate = start;
			if (endDate == null || endDate < end)
				endDate = end;
		}
		// Skip crops that don't have any valid seasons.
		// Who knows what else isn't valid?
		if (startDate == null || endDate == null)
			return null;
		// Skip crops that are always active.
		if (startDate.SeasonIndex == 0 && startDate.DayOfMonth == 1 && endDate.SeasonIndex == (WorldDate.MonthsPerYear - 1) && endDate.DayOfMonth == ModEntry.DaysPerMonth)
			return null;
		// If the sprite is 23, it's a seasonal multi-seed
		// so we want to show that rather than the seed.
		Item item;
		if (spriteRow == 23)
			item = ItemRegistry.Create(id, 1);
		else
			item = ItemRegistry.Create(harvestID, 1);
		// Phases
		int[] phases = [.. data.DaysInPhase];
		// Phase Sprites
		// We need an extra sprite for the finished crop,
		// as well as one for regrow if that's enabled.
		SpriteInfo[] sprites = new SpriteInfo[phases.Length + 1 + (regrow > 0 ? 1 : 0)];
		// Are we dealing with colors?
		Color? color = null;
		string[]? colorbits = regrow <= 0 && data.TintColors.Count>0 ? data.TintColors.ToArray(): null;
		if (colorbits != null && colorbits.Length >= 4 && colorbits[0].Equals("true")) {
			// Technically there could be many colors, but we just use
			// the first one.
			color = new(
				Convert.ToByte(colorbits[1]),
				Convert.ToByte(colorbits[2]),
				Convert.ToByte(colorbits[3])
			);
		}
		for (int i = 0; i < sprites.Length; i++) {
			bool final = i == (sprites.Length - (regrow > 0 ? 2 : 1));
			sprites[i] = new(
				Game1.cropSpriteSheet,
				new Rectangle(
					Math.Min(240, (i + 1) * 16 + (spriteRow % 2 != 0 ? 128 : 0)),
					spriteRow / 2 * 16 * 2,
					16, 32
				),
				overlaySource: final && color.HasValue ? new Rectangle(
					Math.Min(240, (i + 2) * 16 + (spriteRow % 2 != 0 ? 128 : 0)),
					spriteRow / 2 * 16 * 2,
					16, 32
				) : null,
				overlayColor: final ? color : null
			);
		}
		bool isGiantCrop = false;
		SpriteInfo? giantSprite = null;
		// Giant Crops
		var giantCrops = Game1.content.Load<Dictionary<string, GiantCropData>>(@"Data\GiantCrops");
		if (giantCrops != null && giantCrops.TryGetValue(harvestID, out var gcdata) && gcdata != null) { 
			isGiantCrop = true;
			giantSprite = new(
				Game1.content.Load<Texture2D>(gcdata.Texture),
				new Rectangle(
					gcdata.TexturePosition.X, gcdata.TexturePosition.Y,
					gcdata.TileSize.X * 16,
					(gcdata.TileSize.Y + 1) * 16
				)
			);
		}
		SpriteInfo sprite = null;
		int offset;
		switch (harvestID) {
				case "Carrot": {
						offset = 0;
						break;
					}
				case "SummerSquash": {
						offset = 1;
						break;
					}
				case "Broccoli": {
						offset = 2;
						break;
					}
				case "Powdermelon": {
						offset = 3;
						break;
					}
				default: {
						offset = -1;
						break;
					}
			}
		if (offset!=-1) {
			sprite = new SpriteInfo(Game1.objectSpriteSheet_2, new Rectangle(offset * 16, 160, 16, 16));
		}
		return new(
			id,
			item,
			item.DisplayName,
			sprite==null? SpriteHelper.GetSprite(item): sprite,
			isTrellisCrop,
			isGiantCrop,
			giantSprite,
			new Item[] {
				new SObject(id, 1)
			},
			phases,
			sprites,
			regrow,
			isPaddyCrop,
			startDate,
			endDate
		);
	}
}
