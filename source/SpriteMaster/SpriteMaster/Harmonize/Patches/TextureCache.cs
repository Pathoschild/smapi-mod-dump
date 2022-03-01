/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nito.Collections;
using Pastel;
using SpriteMaster.Extensions;
using SpriteMaster.GL;
using SpriteMaster.Types;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using static SpriteMaster.Harmonize.Harmonize;

namespace SpriteMaster.Harmonize.Patches;

static class TextureCache {
	private const int MaxDequeItems = 20;
	private static readonly Deque<XTexture2D> TextureCacheDeque = new(MaxDequeItems);
	private static readonly ConcurrentDictionary<string, WeakReference<XTexture2D>> TextureCacheTable = new();
	private static readonly ConditionalWeakTable<XTexture2D, string> TexturePaths = new();
	private static readonly WeakSet<XTexture2D> PremultipliedTable = new();

	[Harmonize(
		typeof(XTexture2D),
		"FromStream",
		Harmonize.Fixation.Prefix,
		PriorityLevel.Last,
		platform: Harmonize.Platform.MonoGame,
		instance: false,
		critical: false
	)]
	public static bool FromStreamPre(ref XTexture2D __result, GraphicsDevice graphicsDevice, Stream stream) {
		if (!Config.SMAPI.TextureCacheEnabled) {
			return true;
		}

		if (stream is not FileStream fileStream) {
			return true;
		}

		using var watchdogScoped = WatchDog.WatchDog.ScopedWorkingState;

		if (graphicsDevice is not null && fileStream is not null) {
			var path = fileStream.Name;
			if (TextureCacheTable.TryGetValue(path, out var textureRef)) {
				XTexture2D? texture = null;
				if (textureRef is WeakReference<XTexture2D> weakRef) {
					weakRef.TryGetTarget(out texture);
				}
				if (texture is not null) {
					if (texture.IsDisposed || texture.GraphicsDevice != graphicsDevice) {
						TextureCacheTable.TryRemove(path, out var _);
						TexturePaths.Remove(texture);
					}
					else {
						Debug.Trace($"Found Texture2D for '{path}' in cache!".Pastel(System.Drawing.Color.LightCyan));
						__result = texture;
						return false;
					}
				}
			}
		}

		return true;
	}

	[Harmonize(typeof(XTexture2D), "FromStream", Harmonize.Fixation.Postfix, PriorityLevel.Last, platform: Harmonize.Platform.MonoGame, instance: false)]
	public static void FromStreamPost(ref XTexture2D __result, GraphicsDevice graphicsDevice, Stream stream) {
		if (!Config.SMAPI.TextureCacheEnabled) {
			return;
		}

		using var watchdogScoped = WatchDog.WatchDog.ScopedWorkingState;

		if (__result is null || stream is null) {
			return;
		}

		if (stream is not FileStream fileStream) {
			return;
		}

		var result = __result;
		PremultipliedTable.Remove(result);
		if (Config.SMAPI.TextureCacheHighMemoryEnabled) {
			lock (TextureCacheDeque) {
				int dequeIndex = TextureCacheDeque.IndexOf(result);
				if (dequeIndex != -1) {
					TextureCacheDeque.RemoveAt(dequeIndex);
				}

				while (TextureCacheDeque.Count >= MaxDequeItems) {
					TextureCacheDeque.RemoveFromBack();
				}
				TextureCacheDeque.AddToFront(result);
			}
		}
		TextureCacheTable.AddOrUpdate(fileStream.Name, result.MakeWeak(), (name, original) => result.MakeWeak());
		TexturePaths.AddOrUpdate(result, fileStream.Name);
	}

	private static readonly ThreadLocal<WeakReference<XTexture2D>> CurrentPremultiplyingTexture = new();

	[Harmonize(
		typeof(StardewModdingAPI.Framework.ModLoading.RewriteFacades.AccessToolsFacade),
		"StardewModdingAPI.Framework.ContentManagers.ModContentManager",
		"PremultiplyTransparency",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.First
	)]
	public static bool PremultiplyTransparencyPre(ContentManager __instance, ref XTexture2D __result, XTexture2D texture) {
		if (!Config.SMAPI.TextureCacheEnabled) {
			return true;
		}

		if (PremultipliedTable.Contains(texture)) {
			__result = texture;
			CurrentPremultiplyingTexture.Value = null!;
			return false;
		}

		CurrentPremultiplyingTexture.Value = texture.MakeWeak();
		return true;
	}

	[Harmonize(
		typeof(StardewModdingAPI.Framework.ModLoading.RewriteFacades.AccessToolsFacade),
		"StardewModdingAPI.Framework.ContentManagers.ModContentManager",
		"PremultiplyTransparency",
		Harmonize.Fixation.Postfix,
		Harmonize.PriorityLevel.First
	)]
	public static void PremultiplyTransparencyPost(ContentManager __instance, ref XTexture2D __result, XTexture2D texture) {
		if (!Config.SMAPI.TextureCacheEnabled) {
			return;
		}

		PremultipliedTable.AddOrIgnore(texture);
		CurrentPremultiplyingTexture.Value = null!;
	}

	internal static void Remove(XTexture2D texture) {
		if (!Config.SMAPI.TextureCacheEnabled) {
			return;
		}

		// Prevent an annoying circular logic problem
		if (CurrentPremultiplyingTexture.Value?.TryGetTarget(out var currentTexture) ?? false && currentTexture == texture) {
			return;
		}

		PremultipliedTable.Remove(texture);
		if (TexturePaths.TryGetValue(texture, out var path)) {
			TextureCacheTable.TryRemove(path, out var _);
			TexturePaths.Remove(texture);
		}
		lock (TextureCacheDeque) {
			int dequeIndex = TextureCacheDeque.IndexOf(texture);
			if (dequeIndex != -1) {
				TextureCacheDeque.RemoveAt(dequeIndex);
			}
		}
	}
}
