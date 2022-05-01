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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Metadata;

static class Metadata {
	private static readonly ConditionalWeakTable<Texture2D, Texture2DMeta> Texture2DMetaTable = new();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Texture2DMeta Meta(this Texture2D @this) {
#if DEBUG
		if (@this is InternalTexture2D) {
			Debugger.Break();
		}
#endif
		return Texture2DMetaTable.GetValue(@this, key => new(key));
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool TryMeta(this Texture2D @this, [NotNullWhen(true)] out Texture2DMeta? value) => Texture2DMetaTable.TryGetValue(@this, out value);

	internal static void Purge() {
		Texture2DMetaTable.Clear();
	}

	internal static void FlushValidations() {
		foreach (var p in Texture2DMetaTable) {
			p.Value.Validation = null;
		}
	}
}

