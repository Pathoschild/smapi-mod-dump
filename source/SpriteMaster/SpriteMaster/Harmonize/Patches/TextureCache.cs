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
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using static SpriteMaster.Harmonize.Harmonize;

namespace SpriteMaster.Harmonize.Patches;

internal static class TextureCache {
	private const int MaxDequeItems = 20;
	private static readonly Deque<XTexture2D> TextureCacheDeque = new(MaxDequeItems);
	private static readonly ConcurrentDictionary<string, WeakReference<XTexture2D>> TextureCacheTable = new();
	private static readonly ConditionalWeakTable<XTexture2D, string> TexturePaths = new();
	private static readonly WeakSet<XTexture2D> PremultipliedTable = new();
	private static readonly object Lock = new();

	private static readonly Type ModContentManagerType = typeof(StardewModdingAPI.Framework.ModLoading.RewriteFacades.AccessToolsFacade).Assembly.
		GetType("StardewModdingAPI.Framework.ContentManagers.ModContentManager") ?? throw new NullReferenceException("Could not find 'ModContentManager type");

	[Harmonize(
		typeof(XTexture2D),
		"FromStream",
		Fixation.Prefix,
		PriorityLevel.Last,
		platform: Platform.MonoGame,
		instance: false,
		critical: false
	)]
	public static bool FromStreamPre(ref XTexture2D? __result, GraphicsDevice? graphicsDevice, Stream? stream, ref bool __state) {
		lock (Lock) {

			if (!Config.IsUnconditionallyEnabled || !Config.TextureCache.Enabled) {
				__state = false;
				return true;
			}

			if (graphicsDevice is null) {
				__state = false;
				return true;
			}

			if (stream is not FileStream fileStream) {
				__state = false;
				return true;
			}

			bool isContentManager = false;
			var stackTrace = new StackTrace(fNeedFileInfo: false, skipFrames: 1);
			foreach (var frame in stackTrace.GetFrames()) {
				if (frame.GetMethod() is { } method) {
					if (method.DeclaringType == ModContentManagerType) {
						isContentManager = true;
						break;
					}
				}
			}

			if (!isContentManager) {
				__state = false;
				return true;
			}

			__state = true;

			using var watchdogScoped = WatchDog.WatchDog.ScopedWorkingState;

			var path = fileStream.Name;
			if (TextureCacheTable.TryGetValue(path, out var textureRef)) {
				if (textureRef.TryGet(out var texture)) {
					if (texture.IsDisposed || texture.GraphicsDevice != graphicsDevice) {
						TextureCacheTable.TryRemove(path, out var _);
						TexturePaths.Remove(texture);
					}
					else {
						Debug.Trace($"Found XTexture2D for '{path}' in cache!".Pastel(DrawingColor.LightCyan));
						__result = texture;
						__state = false;
						return false;
					}
				}
			}

			return true;
		}
	}

	[Harmonize(typeof(XTexture2D), "FromStream", Fixation.Postfix, PriorityLevel.Last, platform: Platform.MonoGame, instance: false)]
	public static void FromStreamPost(ref XTexture2D? __result, GraphicsDevice? graphicsDevice, Stream? stream, bool __state) {
		lock (Lock) {
			if (!Config.IsUnconditionallyEnabled || !Config.TextureCache.Enabled) {
				return;
			}

			if (!__state) {
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
		}
	}

	[Harmonize(typeof(XTexture2D), "FromStream", Fixation.Finalizer, PriorityLevel.Last, platform: Platform.MonoGame, instance: false)]
	public static void FromStreamFinal(ref XTexture2D? __result, GraphicsDevice? graphicsDevice, Stream? stream, bool __state) {
		lock (Lock) {
			if (!Config.IsUnconditionallyEnabled || !Config.TextureCache.Enabled) {
				return;
			}

			if (!__state) {
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
			if (Config.TextureCache.HighMemoryEnabled) {
				lock (TextureCacheDeque) {
					int dequeIndex = TextureCacheDeque.IndexOf(result);
					if (dequeIndex != -1) {
						TextureCacheDeque.RemoveAt(dequeIndex);
					}

					while (TextureCacheDeque.Count >= MaxDequeItems) {
						var texture = TextureCacheDeque.RemoveFromBack();
						PremultipliedTable.Remove(texture);
					}
					TextureCacheDeque.AddToFront(result);
				}
			}
			WeakReference<XTexture2D>? previousTexture = null;
			TextureCacheTable.AddOrUpdate(fileStream.Name, result.MakeWeak(), (name, original) => {
				previousTexture = original;
				return result.MakeWeak();
			});
			if (previousTexture.TryGet(out var previousTextureTarget)) {
				PremultipliedTable.Remove(previousTextureTarget);
			}
			TexturePaths.AddOrUpdate(result, fileStream.Name);
		}
	}

	private static readonly ThreadLocal<WeakReference<XTexture2D>> CurrentPremultiplyingTexture = new();

	[Harmonize(
		typeof(StardewModdingAPI.Framework.ModLoading.RewriteFacades.AccessToolsFacade),
		"StardewModdingAPI.Framework.ContentManagers.ModContentManager",
		"PremultiplyTransparency",
		Fixation.Prefix,
		PriorityLevel.First
	)]
	public static bool PremultiplyTransparencyPre(ContentManager __instance, ref XTexture2D __result, XTexture2D texture) {
		lock (Lock) {
			if (!Config.IsUnconditionallyEnabled || !Config.TextureCache.Enabled || !Config.TextureCache.PMAEnabled) {
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
	}

	[Harmonize(
		typeof(StardewModdingAPI.Framework.ModLoading.RewriteFacades.AccessToolsFacade),
		"StardewModdingAPI.Framework.ContentManagers.ModContentManager",
		"PremultiplyTransparency",
		Fixation.Finalizer,
		PriorityLevel.First
	)]
	public static void PremultiplyTransparencyPost(ContentManager __instance, XTexture2D __result, XTexture2D texture) {
		lock (Lock) {
			CurrentPremultiplyingTexture.Value = null!;

			if (!Config.IsUnconditionallyEnabled || !Config.TextureCache.Enabled || !Config.TextureCache.PMAEnabled) {
				return;
			}

			PremultipliedTable.AddOrIgnore(texture);
		}
	}

	internal static void Remove(XTexture2D texture) {
		lock (Lock) {
			// Prevent an annoying circular logic problem
			if (CurrentPremultiplyingTexture.Value.TryGet(out var currentTexture) && currentTexture == texture) {
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

	internal static void Flush(bool reset = false) {
		lock (Lock) {
			TextureCacheDeque.Clear();
			TextureCacheTable.Clear();
			TexturePaths.Clear();
			PremultipliedTable.Clear();
		}
	}
}
