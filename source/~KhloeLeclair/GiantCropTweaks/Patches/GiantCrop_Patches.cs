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
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

using HarmonyLib;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Extensions;
using Leclair.Stardew.GiantCropTweaks.Models;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData.GiantCrops;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace Leclair.Stardew.GiantCropTweaks.Patches;

public static class GiantCrop_Patches {

	private static IMonitor? Monitor;

	public static void Patch(ModEntry mod) {
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(GiantCrop), nameof(GiantCrop.draw)),
				postfix: new HarmonyMethod(AccessTools.Method(typeof(GiantCrop_Patches), nameof(GiantCrop_draw__Postfix)))
			);

			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(GiantCrop), nameof(GiantCrop.performToolAction)),
				postfix: new HarmonyMethod(typeof(GiantCrop_Patches), nameof(GiantCrop_performToolAction__Postfix))
			);

			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(GiantCrop), "TryGetDrop"),
				postfix: new HarmonyMethod(typeof(GiantCrop_Patches), nameof(GiantCrop_TryGetDrop__Postfix))
			);

		} catch(Exception ex) {
			mod.Log($"An error occurred while registering a harmony patch for giant crops.", LogLevel.Error, ex);
		}
	}

	public static void GiantCrop_performToolAction__Postfix(GiantCrop __instance, bool __result) {
		try {
			if (__result)
				ModEntry.Instance.OnGiantCropRemoved(__instance);
		} catch (Exception ex) {
			Monitor?.Log($"An error occurred while attempting to interact with a GiantCrop:\n{ex}", LogLevel.Warn);
		}
	}


	public static void GiantCrop_TryGetDrop__Postfix(GiantCrop __instance, ref Item? __result) {
		try {
			if (__result != null && ModEntry.Instance.TryGetExtraData(__instance.Id, out var extraData) && __instance.GetData() is GiantCropData data) {

				var list = extraData.HarvestItemsToColor;
				if (list != null && (list.Contains(__result.ItemId) || list.Contains(__result.QualifiedItemId))) {
					// Time to color it!
					Color color = GetColorFor(
						extraData.RandomizeHarvestItemColors ? null : __instance,
						extraData,
						data
					);

					__result = new ColoredObject(__result.ItemId, __result.Stack, color);
				}
			}
		} catch(Exception ex) {
			Monitor?.LogOnce($"Error in GiantCrop.TryGetDrop Postfix: {ex}", LogLevel.Error);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color GetColorFor(GiantCrop? crop, ExtraGiantCropData extraData, GiantCropData data) {
		// If we have a crop instance, and it has a cached color, use it.
		if (crop != null && crop.modData.TryGetValue(ModEntry.MD_COLOR, out string? cstring) && uint.TryParse(cstring, out uint val))
			return new Color(val);

		// If we don't, pick a color.
		Color result;
		if (extraData.UseBaseCropTintColors)
			result = PickColorFromBaseCrop(data);
		else if (extraData.Colors != null)
			result = Game1.random.ChooseFrom(extraData.Colors);
		else
			// Do not cache the color if it isn't random.
			return Color.White;

		// If we have a crop, save the result.
		if (crop != null)
			crop.modData[ModEntry.MD_COLOR] = result.PackedValue.ToString();
		return result;
	}

	public static Color PickColorFromBaseCrop(GiantCropData cropData) {
		var pair = ModEntry.Instance.GetUnderlyingCrop(cropData);
		if (pair?.Value?.TintColors is null || pair.Value.Value.TintColors.Count == 0)
			return Color.White;

		return CommonHelper.ParseColor(Game1.random.ChooseFrom(pair.Value.Value.TintColors)) ?? Color.White;
	}

	public static void GiantCrop_draw__Postfix(GiantCrop __instance, SpriteBatch spriteBatch) {
		try {
			if (ModEntry.Instance.TryGetExtraData(__instance.Id, out var extraData) && extraData.OverlayTexture != null && __instance.GetData() is GiantCropData data) {
				Texture2D tex = Game1.content.Load<Texture2D>(extraData.OverlayTexture);

				Point source = extraData.OverlayPosition ?? data.TexturePosition;
				Point size = extraData.OverlaySize ?? data.TileSize;
				Point offset = extraData.OverlayOffset;

				Vector2 tileLocation = new(__instance.Tile.X + offset.X, __instance.Tile.Y + offset.Y);

				Color color;
				if (extraData.OverlayPrismatic)
					color = Utility.GetPrismaticColor(__instance.GetHashCode(), 2);
				else
					color = GetColorFor(__instance, extraData, data);

				spriteBatch.Draw(
					tex,
					Game1.GlobalToLocal(
						Game1.viewport,
						tileLocation * 64f - new Vector2(
							(__instance.shakeTimer > 0f) ? ((float) Math.Sin(Math.PI * 2.0 / __instance.shakeTimer) * 2f) : 0f,
							64f
						)
					),
					new Rectangle(source.X, source.Y, 16 * size.X, 16 * (size.Y + 1)),
					color,
					0f,
					Vector2.Zero,
					4f,
					SpriteEffects.None,
					(tileLocation.Y + (float) size.Y) * 64f / 10000f + 0.001f
				);
			}

		} catch(Exception ex) {
			Monitor?.LogOnce($"An error occurred while attempting to draw a GiantCrop instance:\n{ex}", LogLevel.Warn);
		}
	}

}
