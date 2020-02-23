using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Metadata;
using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster {
	internal sealed class SpriteInfo : IDisposable {
		internal readonly Texture2D Reference;
		internal readonly Vector2I ReferenceSize;
		internal readonly Bounds Size;
		internal readonly Vector2B Wrapped;
		internal readonly bool BlendEnabled;
		internal readonly uint ExpectedScale;
		// For statistics and throttling
		internal readonly bool WasCached;

		internal byte[] Data = default;
		private Volatile<ulong> _Hash = Hashing.Default;
		public ulong Hash {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				ulong hash = _Hash;
				if (hash == Hashing.Default) {
					hash = Data.Hash();
				}
				_Hash = hash;
				return hash;// ^ unchecked((ulong)ExpectedScale.GetHashCode());
			}
		}

		// Attempt to update the bytedata cache for the reference texture, or purge if it that makes more sense or if updating
		// is not plausible.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static unsafe void Purge(Texture2D reference, Bounds? bounds, DataRef<byte> data) {
			reference.Meta().Purge(reference, bounds, data);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsCached(Texture2D reference) {
			return reference.Meta().CachedDataNonBlocking != null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal SpriteInfo (Texture2D reference, in Bounds dimensions, uint expectedScale) {
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

			Data = reference.Meta().CachedDataNonBlocking;

			if (Data == null) {
				Data = new byte[reference.SizeBytes()];
				Debug.TraceLn($"Reloading Texture Data: {reference.SafeName()}");
				reference.GetData(Data);
				reference.Meta().CachedData = Data;
				WasCached = false;
			}
			else if (Data == MTexture2D.BlockedSentinel) {
				Data = null;
				WasCached = false;
			}
			else {
				WasCached = true;
			}

			BlendEnabled = DrawState.CurrentBlendSourceMode != Blend.One;
			Wrapped = new Vector2B(
				DrawState.CurrentAddressModeU == TextureAddressMode.Wrap,
				DrawState.CurrentAddressModeV == TextureAddressMode.Wrap
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose () {
			Data = default;
			_Hash = Hashing.Default;
		}
	}
}
