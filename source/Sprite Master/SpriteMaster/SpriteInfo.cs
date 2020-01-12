using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Metadata;
using SpriteMaster.Types;
using System;

namespace SpriteMaster {
	internal sealed class SpriteInfo {
		internal readonly Texture2D Reference;
		internal readonly Vector2I ReferenceSize;
		internal readonly Bounds Size;
		internal readonly Vector2B Wrapped;
		internal readonly bool BlendEnabled;
		internal readonly int ExpectedScale;
		internal byte[] Data { get; private set; } = default;
		private ulong _Hash = default;
		public ulong Hash {
			get {
				if (_Hash == default) {
					_Hash = Data.Hash();
				}
				return _Hash;// ^ unchecked((ulong)ExpectedScale.GetHashCode());
			}
		}

		// Attempt to update the bytedata cache for the reference texture, or purge if it that makes more sense or if updating
		// is not plausible.
		internal static unsafe void Purge(Texture2D reference, Bounds? bounds, DataRef<byte> data) {
			reference.Meta().Purge(reference, bounds, data);
		}

		internal SpriteInfo (Texture2D reference, in Bounds dimensions, int expectedScale) {
			ReferenceSize = new Vector2I(reference);
			ExpectedScale = expectedScale;
			Size = dimensions;
			if (Size.Bottom > ReferenceSize.Height) {
				Size.Height -= (Size.Bottom - ReferenceSize.Height);
			}
			if (Size.Right > ReferenceSize.Width) {
				Size.Width -= (Size.Right - ReferenceSize.Width);
			}
			Reference = reference;

			Data = reference.Meta().CachedData;

			if (Data == null) {
				Data = new byte[reference.SizeBytes()];
				Debug.TraceLn($"Reloading Texture Data: {reference.SafeName()}");
				reference.GetData(Data);
				reference.Meta().CachedData = Data;
			}

			BlendEnabled = DrawState.CurrentBlendSourceMode != Blend.One;
			Wrapped = new Vector2B(
				DrawState.CurrentAddressModeU == TextureAddressMode.Wrap,
				DrawState.CurrentAddressModeV == TextureAddressMode.Wrap
			);
		}

		internal void Dispose () {
			Data = default;
		}
	}
}
