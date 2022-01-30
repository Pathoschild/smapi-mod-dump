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
using System.Runtime.CompilerServices;

namespace SpriteMaster.Metadata;

static class Metadata {
	private static readonly ConditionalWeakTable<Texture2D, Texture2DMeta> Texture2DMetaTable = new();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Texture2DMeta Meta(this Texture2D @this) {
		return Texture2DMetaTable.GetValue(@this, key => new(key));
	}
}

