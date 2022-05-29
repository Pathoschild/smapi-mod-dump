/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Configuration;
using SpriteMaster.GL;
using StardewModdingAPI;
using System;
using System.Reflection;

namespace SpriteMaster.Harmonize.Patches.SMAPI;

internal static class PAssetDataForImage {
	private static readonly Assembly ReferenceAssembly = typeof(StardewModdingAPI.Framework.ModLoading.RewriteFacades.AccessToolsFacade).Assembly;
	private static readonly Type? AssetDataForImageType = ReferenceAssembly.
		GetType("StardewModdingAPI.Framework.Content.AssetDataForImage");
	private static readonly byte MinOpacity = (byte?)AssetDataForImageType?.
		GetField("MinOpacity", BindingFlags.NonPublic | BindingFlags.Static)?.
		GetValue(null) ?? 5;

	[Harmonize(
		typeof(StardewModdingAPI.Framework.ModLoading.RewriteFacades.AccessToolsFacade),
		"StardewModdingAPI.Framework.Content.AssetDataForImage",
		"PatchImage",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static bool PatchImage(IAssetDataForImage __instance, XTexture2D source, XRectangle? sourceArea, XRectangle? targetArea, PatchMode patchMode) {
		if (!Config.SMAPI.ApplyPatchEnabled) {
			return true;
		}

		// get texture
		if (source is null) {
			throw new ArgumentNullException(nameof(source), "Can't patch from a null source texture.");
		}

		XTexture2D target = __instance.Data;

		// get areas
		sourceArea ??= new(0, 0, source.Width, source.Height);
		targetArea ??= new(0, 0, Math.Min(sourceArea.Value.Width, target.Width), Math.Min(sourceArea.Value.Height, target.Height));

		// validate
		if (!source.Bounds.Contains(sourceArea.Value))
			throw new ArgumentOutOfRangeException(nameof(sourceArea), "The source area is outside the bounds of the source texture.");
		if (!target.Bounds.Contains(targetArea.Value))
			throw new ArgumentOutOfRangeException(nameof(targetArea), "The target area is outside the bounds of the target texture.");
		if (sourceArea.Value.Size != targetArea.Value.Size)
			throw new InvalidOperationException("The source and target areas must be the same size.");

		if (GL.Texture2DExt.CopyTexture(source, sourceArea.Value, target, targetArea.Value, patchMode)) {
			return false;
		}

		// get source data
		int pixelCount = sourceArea.Value.Width * sourceArea.Value.Height;
		var sourceData = GC.AllocateUninitializedArray<XColor>(pixelCount);
		source.GetData(0, sourceArea, sourceData, 0, pixelCount);

		// merge data in overlay mode
		if (patchMode == PatchMode.Overlay) {
			// get target data
			var targetData = GC.AllocateUninitializedArray<XColor>(pixelCount);
			target.GetData(0, targetArea, targetData, 0, pixelCount);

			// merge pixels
			for (int i = 0; i < sourceData.Length; i++) {
				var above = sourceData[i];
				var below = targetData[i];

				// shortcut transparency
				if (above.A < MinOpacity) {
					sourceData[i] = below;
					continue;
				}
				if (below.A < MinOpacity) {
					sourceData[i] = above;
					continue;
				}

				// merge pixels
				// This performs a conventional alpha blend for the pixels, which are already
				// premultiplied by the content pipeline. The formula is derived from
				// https://shawnhargreaves.com/blog/premultiplied-alpha.html.
				float alphaBelow = 1 - (above.A / 255f);
				sourceData[i] = new XColor(
						(int)(above.R + (below.R * alphaBelow)), // r
						(int)(above.G + (below.G * alphaBelow)), // g
						(int)(above.B + (below.B * alphaBelow)), // b
						Math.Max(above.A, below.A) // a
				);
			}
		}

		// patch target texture
		target.SetData(0, targetArea, sourceData, 0, pixelCount);
		return false;
	}
}
