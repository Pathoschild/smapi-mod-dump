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

namespace SpriteMaster.Metadata {
	internal static class Metadata {
		private static readonly ConditionalWeakTable<Texture2D, MTexture2D> Texture2DMetaTable = new();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static MTexture2D Meta(this Texture2D @this) {
			return Texture2DMetaTable.GetOrCreateValue(@this);
		}
	}
}
