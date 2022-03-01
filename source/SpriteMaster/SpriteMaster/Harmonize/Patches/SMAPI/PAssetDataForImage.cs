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
using SpriteMaster.Extensions;
using SpriteMaster.Metadata;
using SpriteMaster.Types;
using SpriteMaster.Types.Fixed;
using StardewModdingAPI;
using System;
using System.Reflection;

namespace SpriteMaster.Harmonize.Patches.SMAPI;

static class PAssetDataForImage {
	private const int MaxStackSize = 256;

	private static readonly Assembly? ReferenceAssembly = typeof(StardewModdingAPI.Framework.ModLoading.RewriteFacades.AccessToolsFacade)?.Assembly;
	private static readonly Type? AssetDataForImageType = ReferenceAssembly?.GetType("StardewModdingAPI.Framework.Content.AssetDataForImage");
	private static readonly byte MinOpacity = ((byte?)AssetDataForImageType?.GetField("MinOpacity", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null)) ?? 5;

	[Harmonize(
		typeof(StardewModdingAPI.Framework.ModLoading.RewriteFacades.AccessToolsFacade),
		"StardewModdingAPI.Framework.Content.AssetDataForImage",
		"PatchImage",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static unsafe bool PatchImage(IAssetDataForImage __instance, Texture2D source, XNA.Rectangle? sourceArea, XNA.Rectangle? targetArea, PatchMode patchMode) {
		if (!Config.SMAPI.ApplyPatchEnabled) {
			return true;
		}

		// get texture
		if (source is null) {
			throw new ArgumentNullException(nameof(source), "Can't patch from a null source texture.");
		}
		Texture2D target = __instance.Data;

		// get areas
		Bounds sourceBounds = sourceArea ?? new Bounds(source.Extent());
		Bounds targetBounds = targetArea ?? new Bounds(source.Extent());

		// validate
		if (!new Bounds(source.Bounds).Contains(sourceBounds))
			throw new ArgumentOutOfRangeException(nameof(sourceArea), "The source area is outside the bounds of the source texture.");
		if (!new Bounds(target.Bounds).Contains(targetBounds))
			throw new ArgumentOutOfRangeException(nameof(targetArea), "The target area is outside the bounds of the target texture.");
		if (sourceBounds.Extent != targetBounds.Extent)
			throw new InvalidOperationException("The source and target areas must be the same size.");

		// get source data
		int texelCount = sourceBounds.Area;
		// TODO : make me a span when I add Span overrides for GetData and SetData
		//ReadOnlySpan<Color8> sourceData = (texelCount > MaxStackSize) ? SpanExt.MakeUninitialized<Color8>(texelCount) : stackalloc Color8[texelCount];

		static byte[] GetTextureData(Texture2D texture, in Bounds bounds, int count) {
			count *= sizeof(Color8);
			byte[] dataArray;
			if (Config.SMAPI.ApplyPatchUseCache && texture.TryMeta(out var sourceMeta) && sourceMeta.CachedData is byte[] cachedSourceData) {
				if (bounds == texture.Bounds) {
					dataArray = cachedSourceData;
				}
				else {
					// We need a subcopy
					dataArray = GC.AllocateUninitializedArray<byte>(count, pinned: Config.SMAPI.ApplyPatchPinMemory);
					var cachedData = cachedSourceData.AsReadOnlySpan<Color8>();
					var destData = dataArray.AsSpan<Color8>();
					int sourceStride = texture.Width;
					int destStride = bounds.Width;
					int sourceOffset = (bounds.Top * sourceStride) + bounds.Left;
					int destOffset = 0;
					for (int y = 0; y < bounds.Height; ++y) {
						cachedData.Slice(sourceOffset, destStride).CopyTo(destData.Slice(destOffset, destStride));
						sourceOffset += sourceStride;
						destOffset += destStride;
					}
				}
			}
			else {
				dataArray = GC.AllocateUninitializedArray<byte>(count, pinned: Config.SMAPI.ApplyPatchPinMemory);
				texture.GetData(0, bounds, dataArray, 0, count);
			}
			return dataArray;
		}

		byte[] sourceDataArray = GetTextureData(source, sourceBounds, texelCount);
		var sourceData = sourceDataArray.AsSpan<Color8>();

		// merge data in overlay mode
		if (patchMode == PatchMode.Overlay) {
			// get target data
			byte[] targetDataArray = GetTextureData(target, targetBounds, texelCount);
			var targetData = targetDataArray.AsReadOnlySpan<Color8>();

			// merge pixels
			for (int i = 0; i < texelCount; i++) {
				var above = sourceData[i];
				var below = targetData[i];

				// shortcut transparency
				if (above.A.Value < MinOpacity) {
					sourceData[i] = below;
					continue;
				}
				if (below.A.Value < MinOpacity) {
					sourceData[i] = above;
					continue;
				}

				// merge pixels
				// This performs a conventional alpha blend for the pixels, which are already
				// premultiplied by the content pipeline. The formula is derived from
				// https://shawnhargreaves.com/blog/premultiplied-alpha.html.
				Fixed8 alphaBelow = (byte)(byte.MaxValue - above.A.Value);
				sourceData[i] = new(
					r: above.R.AddClamped(below.R * alphaBelow),
					g: above.G.AddClamped(below.G * alphaBelow),
					b: above.B.AddClamped(below.B * alphaBelow),
					a: MathExt.Max(above.A, below.A)
				);
			}
		}

		// patch target texture
		target.SetData(0, targetBounds, sourceDataArray, 0, texelCount * sizeof(Color8));

		return false;
	}
}
