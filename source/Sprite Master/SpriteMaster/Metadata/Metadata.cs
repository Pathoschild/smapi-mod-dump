using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Metadata {
	internal static class Metadata {
		private static readonly ConditionalWeakTable<Texture2D, MTexture2D> Texture2DMetaTable = new ConditionalWeakTable<Texture2D, MTexture2D>();
		internal static MTexture2D Meta(this Texture2D @this) {
			return Texture2DMetaTable.GetOrCreateValue(@this);
		}
	}
}
