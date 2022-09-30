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
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

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

				Debug.Trace($"Loading Texture '{resolvedPath}' from cache.".Pastel(DrawingColor.LightGreen));
				return false;
			}
		}

		return !LoadFromFile(path: resolvedPath, copyArray: copyArray, swallowExceptions: false, out __result!);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static bool LoadFromFile(string path, bool copyArray, bool swallowExceptions, [NotNullWhen(true)] out IRawTextureData? result) {
		Debug.Trace($"Loading Texture '{path}' from file.");
		var rawData = File.ReadAllBytes(path);
		try {
			var imageResult = new Stb.ImageResult(rawData);
			byte[] data = imageResult.Data;
			var colorData = data.AsSpan<Color8>();

			ProcessTexture(colorData);

			// TODO : Horribly unsafe
			XColor[] resultData = data.Convert<byte, XColor>();

			Vector2I resultSize = imageResult.Size;
			TextureInfoCache.AddOrUpdate(path, resultSize, (_, _) => resultSize);
			Cache.Set(path, resultData);

			result = new RawTextureData(
				size: resultSize,
				copyArray ? resultData.CloneFast() : resultData
			);

			return true;
		}
		catch (Exception ex) {
			if (!swallowExceptions) {
				// If there is an exception, swallow it and just go back to the normal execution path.
				Debug.Error($"{nameof(OnLoadRawImageData)} exception while processing '{path}'", ex);
			}

			result = null;
			return false;
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

	private static string? GetModsPath() {
		const BindingFlags smapiBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

		// Try to use reflection first.
		Type apiConstants = typeof(StardewModdingAPI.Constants);
		if (apiConstants.GetProperty("ModsPath", smapiBindingFlags)?.GetValue(null) is string modsPath && Directory.Exists(modsPath)) {
			return modsPath;
		}
		if (apiConstants.GetProperty("DefaultModsPath", smapiBindingFlags)?.GetValue(null) is string defaultModsPath && Directory.Exists(defaultModsPath)) {
			return defaultModsPath;
		}

		string? rootDirectory = Path.GetDirectoryName(SpriteMaster.Assembly.Location);

		bool IsModsDirectory() {
			return File.Exists(Path.Combine(rootDirectory, "Stardew Valley.dll"));
		}

		string? previousDirectory = rootDirectory;
		while (rootDirectory is not null && rootDirectory.Length != 0 && !IsModsDirectory()) {
			previousDirectory = rootDirectory;
			rootDirectory = Path.GetDirectoryName(rootDirectory);
		}

		if (rootDirectory is not null) {
			return previousDirectory!;
		}

		return null;
	}

	internal static List<FileInfo> GetAllTextures(string root) {
		List<FileInfo> result = new();

		Queue<DirectoryInfo> pending = new();
		pending.Enqueue(new(root));

		while (pending.TryDequeue(out var directory)) {
			try {
				foreach (var child in directory.EnumerateFileSystemInfos()) {
					if (child is DirectoryInfo childDirectory) {
						pending.Enqueue(childDirectory);
					}
					else if (child is FileInfo childFile) {
						if (childFile.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase)) {
							result.Add(childFile);
						}
					}
				}
			}
			catch (Exception) {
				// swallow exceptions
			}
		}

		return result;
	}

	internal static void Precache() {
		if (!SMConfig.TextureFileCache.Enabled || !SMConfig.TextureFileCache.Precache) {
			return;
		}

		if (GetModsPath() is not {} rootDirectory) {
			// Could not derive Mods directory path :(
			return;
		}

		// Traverse all mods looking for '.png' files (and maybe '.tga' or '.dds'?)
		var allSpriteSheets = GetAllTextures(rootDirectory);

		Parallel.ForEach(
			allSpriteSheets, file => {
				var originalPriority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				try {
					LoadFromFile(path: file.FullName, copyArray: false, swallowExceptions: true, out _);
				}
				catch {
					// swallow exceptions
				}
				finally {
					Thread.CurrentThread.Priority = originalPriority;
				}
			}
		);
	}

	internal static long Size => Cache.SizeBytes;
}
