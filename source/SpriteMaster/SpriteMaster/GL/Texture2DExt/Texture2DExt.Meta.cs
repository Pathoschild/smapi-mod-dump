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
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.GL;

internal static partial class Texture2DExt {
	internal sealed class Texture2DOpenGlMeta : IDisposable {
		private static readonly ConditionalWeakTable<Texture2D, Texture2DOpenGlMeta> Map = new();

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal static bool TryGet(Texture2D texture, out Texture2DOpenGlMeta meta) => Map.TryGetValue(texture, out meta!);
		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal static Texture2DOpenGlMeta Get(Texture2D texture) => Map.GetValue(texture, t => new(t));

		[Flags]
		internal enum Flag {
			None = 0,
			Initialized = 1 << 0,
			Storage = 1 << 1,
			Managed = 1 << 2,
		}

		internal Flag Flags = Flag.Initialized;

		[MethodImpl(Runtime.MethodImpl.Inline)]
		private Texture2DOpenGlMeta(Texture2D texture) {
			texture.Disposing += (_, _) => Dispose();
		}

		~Texture2DOpenGlMeta() {

		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		public void Dispose() => Dispose(true);

		[MethodImpl(Runtime.MethodImpl.Inline)]
		private void Dispose(bool disposing) {
			if (disposing) {
				GC.SuppressFinalize(this);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Texture2DOpenGlMeta GetGlMeta<TTexture>(this TTexture texture) where TTexture : Texture2D {
		if (texture is InternalTexture2D internalTexture) {
			return internalTexture.GlMeta;
		}
		return Texture2DOpenGlMeta.Get(texture);
	}
}
