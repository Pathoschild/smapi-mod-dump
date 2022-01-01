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
using SpriteMaster.Extensions;
using SpriteMaster.Metadata;
using SpriteMaster.Types;
using SpriteMaster.Types.Interlocked;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster;

sealed class SpriteInfo : IDisposable {
	internal readonly Texture2D Reference;
	internal readonly Vector2I ReferenceSize;
	internal readonly Bounds Size;
	internal readonly Vector2B Wrapped;
	internal readonly bool BlendEnabled;
	internal readonly uint ExpectedScale;
	// For statistics and throttling
	internal readonly bool WasCached;

	public override string ToString() {
		return $"SpriteInfo[Name: '{Reference.Name}', ReferenceSize: {ReferenceSize}, Size: {Size}]";
	}

	internal byte[] Data = default;
	private InterlockedULong _Hash = Hashing.Default;
	internal ulong Hash {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		get {
			ulong hash = _Hash;
			if (hash == Hashing.Default) {
				hash = Data.Hash();
			}
			_Hash = hash;
			return hash;// ^ (ulong)ExpectedScale.GetHashCode();
		}
	}

	// Attempt to update the bytedata cache for the reference texture, or purge if it that makes more sense or if updating
	// is not plausible.
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static unsafe void Purge(Texture2D reference, in Bounds? bounds, DataRef<byte> data) => reference.Meta().Purge(reference, bounds, data);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool IsCached(Texture2D reference) => reference.Meta().CachedDataNonBlocking is not null;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal SpriteInfo(Texture2D reference, in Bounds dimensions, uint expectedScale) {
		ReferenceSize = new(reference);
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

		if (Data is null) {
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
		Wrapped = new(
			DrawState.CurrentAddressModeU == TextureAddressMode.Wrap,
			DrawState.CurrentAddressModeV == TextureAddressMode.Wrap
		);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public void Dispose() {
		Data = default;
		_Hash = Hashing.Default;
	}
}
