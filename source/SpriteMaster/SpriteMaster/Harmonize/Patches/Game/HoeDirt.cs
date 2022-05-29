/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Types;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;

namespace SpriteMaster.Harmonize.Patches.Game;

internal static class HoeDirt {
	private static XSpriteBatch DirtBatch = new(DrawState.Device);
	private static XSpriteBatch FertBatch = new(DrawState.Device);

	[Harmonize(
		typeof(GameLocation),
		"drawAboveFrontLayer",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static bool DrawAboveFrontLayerPre(GameLocation __instance, XSpriteBatch b) {
		if (!Configuration.Config.Extras.EnableDirtDrawOptimizations || !Configuration.Config.IsEnabled) {
			return true;
		}

		if (Game1.isFestival()) {
			return true;
		}

		DirtBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp);
		FertBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp);

		try {
			Bounds gameViewport = Game1.viewport;
			Vector2I startTile = (gameViewport.Offset / 64) + 1;
			Vector2I maxTile = (gameViewport.End / 64) + (3, 7);
			for (int y = startTile.Y; y < maxTile.Y; ++y) {
				for (int x = startTile.X; x < maxTile.X; ++x) {
					XVector2 tile = new(x, y);
					if (!__instance.terrainFeatures.TryGetValue(tile, out var feat) || feat is Flooring) {
						continue;
					}

					if (feat is StardewValley.TerrainFeatures.HoeDirt dirtFeat) {
						dirtFeat.DrawOptimized(DirtBatch, FertBatch, b, tile);
					}
					else {
						feat.draw(b, tile);
					}
				}
			}
		}
		finally {
			DirtBatch.End();
			FertBatch.End();
		}

		if (__instance is not MineShaft) {
			foreach (var character in __instance.characters) {
				(character as Monster)?.drawAboveAllLayers(b);
			}
		}
		if (__instance.lightGlows.Count > 0) {
			__instance.drawLightGlows(b);
		}
		__instance.DrawFarmerUsernames(b);

		return false;
	}

#if false
	[Harmonize(
		typeof(StardewValley.GameLocation),
		"drawAboveFrontLayer",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static void DrawAboveFrontLayerPre(StardewValley.GameLocation __instance, XSpriteBatch b, ref bool __state) {
		DirtBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp);
		FertBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp);

		__state = true;
	}

	[Harmonize(
		typeof(StardewValley.GameLocation),
		"drawAboveFrontLayer",
		Harmonize.Fixation.Finalizer,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static void DrawAboveFrontLayerFinally (StardewValley.GameLocation __instance, XSpriteBatch b, bool __state) {
		if (!__state) {
			return;
		}

		DirtBatch.End();
		FertBatch.End();
	}

	[Harmonize(
		typeof(StardewValley.TerrainFeatures.HoeDirt),
		"DrawOptimized",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static void DrawOptimized(StardewValley.TerrainFeatures.HoeDirt __instance, ref XSpriteBatch dirt_batch, ref XSpriteBatch fert_batch, XSpriteBatch crop_batch, XVector2 tileLocation) {
		dirt_batch = DirtBatch;
		fert_batch = FertBatch;
	}
#endif

	internal static void OnNewGraphicsDevice(GraphicsDevice device) {
		if (device != DirtBatch.GraphicsDevice) {
			DirtBatch.Dispose();
			DirtBatch = new XSpriteBatch(device);
		}
		if (device != FertBatch.GraphicsDevice) {
			FertBatch.Dispose();
			FertBatch = new XSpriteBatch(device);
		}
	}
}
