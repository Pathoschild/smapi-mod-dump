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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leclair.Stardew.Common.Events;

using Leclair.Stardew.Almanac;

using DynamicGameAssets;
using DynamicGameAssets.PackData;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

namespace Leclair.Stardew.AlmanacDGA {
	public class ModEntry : ModSubscriber {

		IAlmanacAPI API;

		[Subscriber]
		private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {

			API = Helper.ModRegistry.GetApi<IAlmanacAPI>("leclair.almanac");
			if (API == null)
				Log("Unable to get Almanac's API.", LogLevel.Error);

			API?.SetCropPriority(ModManifest, 10);
			API?.SetCropCallback(ModManifest, UpdateCrops);
		}

		[Subscriber]
		private void OnDayStart(object sender, DayStartedEventArgs e) {
			// Invalidate the crop cache every day because the valid
			// DGA crops could change every day.
			API?.InvalidateCrops();
		}

		private Tuple<WorldDate, WorldDate> ParseConditions(DynamicFieldData[] fields) {
			// TODO: Actually parse the conditions to see when
			// we can grow this crop.
			return null;
		}

		private void UpdateCrops() {
			Log("Called UpdateCrops", LogLevel.Debug);
			API.ClearCrops(ModManifest);

			foreach (var pack in DynamicGameAssets.Mod.GetPacks()) {
				foreach (var item in pack.GetItems()) {
					if (!item.Enabled)
						continue;

					if (item is not CropPackData crop)
						continue;

					var dates = ParseConditions(crop.DynamicFields);
					if (dates == null && !crop.CanGrowNow)
						continue;

					Item citem = null;

					List<int> phases = new();
					List<Texture2D> spriteTextures = new();
					List<Rectangle?> spriteSources = new();
					List<Texture2D> spriteOverlays = new();
					List<Rectangle?> spriteOverlaySources = new();

					IReflectedMethod GetMultiTexture = Helper.Reflection.GetMethod(pack, "GetMultiTexture");

					bool trellis = false;

					foreach (var phase in crop.Phases) {

						trellis |= phase.Trellis;

						// Make a note of the last harvest result.
						if (phase.HarvestedDrops.Count > 0) {
							var choices = phase.HarvestedDrops[0].Item;
							if (choices.Count > 0)
								citem = choices[0].Value.Create();
						}

						// Add the phase.
						phases.Add(phase.Length);

						// Add the sprite.
						TexturedRect tex = GetMultiTexture.Invoke<TexturedRect>(phase.TextureChoices, 0, 16, 32);
						TexturedRect colored = phase.TextureColorChoices != null && phase.TextureColorChoices.Length > 0 ? GetMultiTexture.Invoke<TexturedRect>(phase.TextureColorChoices, 0, 16, 32) : null;

						spriteTextures.Add(tex.Texture);
						spriteSources.Add(tex.Rect ?? tex.Texture.Bounds);

						spriteOverlays.Add(colored?.Texture);
						spriteOverlaySources.Add(colored?.Rect);
					}

					// Because Dynamic Game Assets and Content Patcher
					// are stupid and opaque, we can't query the crop
					// for information like, say, the date range over
					// which it grows.

					// So instead, we only show crops that are active
					// NOW and assume they're active for this entire
					// month.

					// This is stupid shit and I don't like it.

					WorldDate earliest = dates?.Item1 ?? new(1, Game1.currentSeason, 1);
					WorldDate latest = dates?.Item2 ?? new(1, Game1.currentSeason, WorldDate.DaysPerMonth);

					Log($"Registering: {crop.ID}", LogLevel.Debug);

					API.AddCrop(
						ModManifest,
						crop.ID,
						citem,
						citem?.DisplayName ?? "???",
						trellis,
						crop.GiantChance > 0,
						crop.Type == CropPackData.CropType.Paddy,
						phases,
						spriteTextures,
						spriteSources,
						null,
						spriteOverlays,
						spriteOverlaySources,
						null,
						0,
						earliest,
						latest
					);

				}
			}
		}
	}
}
