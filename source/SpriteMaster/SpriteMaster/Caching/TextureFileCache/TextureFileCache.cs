/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using Pastel;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using SpriteMaster.Types.MemoryCache;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace SpriteMaster.Caching;

using ImageResultObject = System.Object;

internal static partial class TextureFileCache {
	[StructLayout(LayoutKind.Auto)]
	private readonly struct RawTextureData : IRawTextureData {
		private readonly Vector2I Size;
		private readonly XColor[] Data;

		[Pure]
		readonly int IRawTextureData.Width => Size.Width;
		[Pure]
		readonly int IRawTextureData.Height => Size.Height;
		[Pure]
		readonly XColor[] IRawTextureData.Data => Data;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal RawTextureData(Vector2I size, XColor[] data) {
			Size = size;
			Data = data;
		}
	}

	private static readonly IMemoryCache<string, XColor> Cache =
		AbstractMemoryCache<string, XColor>.Create(name: "File Cache", maxSize: SMConfig.TextureFileCache.MaxSize, compressed: true);

	private static readonly ConcurrentDictionary<string, Vector2I> TextureInfoCache = new();

	[MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool OnLoadRawImageData(LocalizedContentManager __instance, ref IRawTextureData __result, FileInfo file, bool forRawData) {
		if (!Stb.Enabled || !SMConfig.TextureFileCache.Enabled) {
			return true;
		}

		bool copyArray = forRawData;

		string path = file.FullName;
		string resolvedPath = Path.GetFullPath(path);

		if (TextureInfoCache.TryGetValue(resolvedPath, out var cachedSize)) {
			if (Cache.TryGet(resolvedPath, out var cachedValue)) {
				__result = new RawTextureData(
					size: cachedSize,
					copyArray ? cachedValue.CloneFast() : cachedValue
				);

				if (cachedSize.Area != cachedValue.Length) {
					Debugger.Break();
				}

				Debug.Trace($"Loading Texture '{resolvedPath}' from cache.".Pastel(DrawingColor.LightGreen));
				return false;
			}
		}

		Debug.Trace($"Loading Texture '{resolvedPath}' from file.");
		var rawData = File.ReadAllBytes(resolvedPath);
		try {
			var imageResult = new Stb.ImageResult(rawData);
			byte[] data = imageResult.Data;
			var colorData = data.AsSpan<Color8>();

			ProcessTexture(colorData);

			// TODO : Horribly unsafe
			XColor[] resultData = data.Convert<byte, XColor>();

			Vector2I resultSize = imageResult.Size;
			TextureInfoCache.AddOrUpdate(resolvedPath, resultSize, (_, _) => resultSize);
			Cache.Set(resolvedPath, resultData);

			__result = new RawTextureData(
				size: resultSize,
				copyArray ? resultData.CloneFast() : resultData
			);

			return false;
		}
		catch (Exception ex) {
			// If there is an exception, swallow it and just go back to the normal execution path.
			Debug.Error($"{nameof(OnLoadRawImageData)} exception while processing '{path}'", ex);
			return true;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static void ProcessTexture(Span<Color8> data) {
		if (UseAvx2) {
			ProcessTextureAvx2(data);
		}
		else if (UseSse2) {
			ProcessTextureSse2Unrolled(data);
		}
		else {
			ProcessTextureScalar(data);
		}
	}

	internal static void Purge() {
		var newCache = AbstractMemoryCache<string, XColor>.Create(name: "File Cache", maxSize: SMConfig.TextureFileCache.MaxSize, compressed: true);
		var oldCache = Interlocked.Exchange(ref Unsafe.AsRef(Cache), newCache);
		TextureInfoCache.Clear();
		oldCache?.Dispose();
	}

	internal static long Size => Cache.SizeBytes;
}
