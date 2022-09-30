/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Types;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SpriteMaster.Metadata;

internal static class Metadata {
	private readonly struct CacheElement {
		internal readonly ComparableWeakReference<XTexture2D?> Reference = new(null);
		internal readonly WeakReference<Texture2DMeta?> Metadata = new(null);

		public CacheElement() {}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal readonly void Update(XTexture2D? reference, Texture2DMeta? meta) {
			Reference.SetTarget(reference);
			Metadata.SetTarget(meta);
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal readonly void Clear() {
			Reference.SetTarget(null);
			Metadata.SetTarget(null);
		}
	}

	private static readonly SharedLock InlineCacheLock = new();
	private static readonly ThreadLocal<CacheElement> InlineCache = new(() => new(), trackAllValues: true);

	private static readonly ConditionalWeakTable<XTexture2D, Texture2DMeta> Texture2DMetaTable = new();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static bool TryGetCached(XTexture2D @this, [NotNullWhen(true)] out Texture2DMeta? meta) {
		var inlineCache = InlineCache.Value;
		if (inlineCache.Reference == @this && inlineCache.Metadata.TryGetTarget(out meta)) {
			return true;
		}

		meta = null;
		return false;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static void UpdateCached(XTexture2D? reference, Texture2DMeta? meta) {
		using (InlineCacheLock.Read) {
			InlineCache.Value.Update(reference, meta);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static Texture2DMeta Meta(this XTexture2D @this) {
#if DEBUG
		if (@this is InternalTexture2D) {
			Debug.Break();
		}
#endif
		if (TryGetCached(@this, out var cachedMeta)) {
			return cachedMeta;
		}

		var newMeta = Texture2DMetaTable.GetValue(@this, key => new(key));

		UpdateCached(@this, newMeta);

		return newMeta;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool TryMeta(this XTexture2D @this, [NotNullWhen(true)] out Texture2DMeta? value) {
		if (TryGetCached(@this, out value)) {
			return true;
		}

		if (Texture2DMetaTable.TryGetValue(@this, out value)) {
			UpdateCached(@this, value);

			return true;
		}

		return false;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void Purge(bool recache = false) {
		if (recache && SMConfig.ResidentCache.Enabled) {
			foreach (var p in Texture2DMetaTable) {
				p.Value.PushToCache();
			}
		}
		using (InlineCacheLock.Write) {
			Texture2DMetaTable.Clear();
			foreach (var cacheElement in InlineCache.Values) {
				cacheElement.Clear();
			}
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void FlushValidations() {
		foreach (var p in Texture2DMetaTable) {
			p.Value.Validation = null;
		}
	}
}

