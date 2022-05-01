/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

namespace SpriteMaster.Harmonize.Patches.Game;

static class HoeDirt {
#if false
	private static SpriteBatch DirtBatch = new(DrawState.Device);
	private static SpriteBatch FertBatch = new(DrawState.Device);

	[Harmonize(
		typeof(StardewValley.GameLocation),
		"drawAboveFrontLayer",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static bool DrawAboveFrontLayerPre(StardewValley.GameLocation __instance, SpriteBatch b) {
		if (Game1.isFestival()) {
			return true;
		}

		DirtBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp);
		FertBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp);

		try {
			Vector2I MaxTile = (
				(Game1.viewport.X + Game1.viewport.Width) / 64 + 3,
				(Game1.viewport.Y + Game1.viewport.Height) / 64 + 7
			);
			for (int y = Game1.viewport.Y / 64 - 1; y < MaxTile.Y; ++y) {
				for (int x = Game1.viewport.X / 64 - 1; x < MaxTile.X; ++x) {
					XNA.Vector2 tile = new(x, y);
					if (__instance.terrainFeatures.TryGetValue(tile, out var feat) && feat is not Flooring) {
						if (feat is StardewValley.TerrainFeatures.HoeDirt dirtFeat) {
							dirtFeat.DrawOptimized(DirtBatch, FertBatch, b, tile);
						}
						else {
							feat.draw(b, tile);
						}
					}
				}
			}
		}
		finally {
			DirtBatch.End();
			FertBatch.End();
		}

		if (__instance is not MineShaft) {
			foreach (NPC character in __instance.characters) {
				(character as Monster)?.drawAboveAllLayers(b);
			}
		}
		if (__instance.lightGlows.Count > 0) {
			__instance.drawLightGlows(b);
		}
		__instance.DrawFarmerUsernames(b);

		return false;
	}

	/*
	[Harmonize(
		typeof(StardewValley.GameLocation),
		"drawAboveFrontLayer",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static void DrawAboveFrontLayerPre(StardewValley.GameLocation __instance, SpriteBatch b, ref bool __state) {
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
	public static void DrawAboveFrontLayerFinally (StardewValley.GameLocation __instance, SpriteBatch b, bool __state) {
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
	public static void DrawOptimized(StardewValley.TerrainFeatures.HoeDirt __instance, ref SpriteBatch dirt_batch, ref SpriteBatch fert_batch, SpriteBatch crop_batch, XNA.Vector2 tileLocation) {
		dirt_batch = DirtBatch;
		fert_batch = FertBatch;
	}
	*/

	internal static void OnNewGraphicsDevice(GraphicsDevice device) {
		if (device != DirtBatch?.GraphicsDevice) {
			DirtBatch?.Dispose();
			DirtBatch = new SpriteBatch(device);
		}
		if (device != FertBatch?.GraphicsDevice) {
			FertBatch?.Dispose();
			FertBatch = new SpriteBatch(device);
		}
	}
#endif
}
