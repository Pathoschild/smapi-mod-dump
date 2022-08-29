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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Metadata;

internal static class Metadata {
	private static readonly ComparableWeakReference<XTexture2D> CacheReference = new((XTexture2D)null!);
	private static readonly WeakReference<Texture2DMeta> CacheMeta = new(null!);

	private static readonly ConditionalWeakTable<XTexture2D, Texture2DMeta> Texture2DMetaTable = new();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static Texture2DMeta Meta(this XTexture2D @this) {
#if DEBUG
		if (@this is InternalTexture2D) {
			Debugger.Break();
		}
#endif
		if (CacheReference == @this && CacheMeta.TryGetTarget(out var cachedMeta)) {
			return cachedMeta;
		}

		var newMeta = Texture2DMetaTable.GetValue(@this, key => new(key));

		CacheReference.SetTarget(@this);
		CacheMeta.SetTarget(newMeta);

		return newMeta;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool TryMeta(this XTexture2D @this, [NotNullWhen(true)] out Texture2DMeta? value) {
		if (CacheReference == @this && CacheMeta.TryGetTarget(out var cachedMeta)) {
			value = cachedMeta;
			return true;
		}

		if (Texture2DMetaTable.TryGetValue(@this, out value)) {
			CacheReference.SetTarget(@this);
			CacheMeta.SetTarget(value);

			return true;
		}

		return false;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void Purge() {
		Texture2DMetaTable.Clear();
		CacheReference.SetTarget(null!);
		CacheMeta.SetTarget(null!);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void FlushValidations() {
		foreach (var p in Texture2DMetaTable) {
			p.Value.Validation = null;
		}
	}
}

