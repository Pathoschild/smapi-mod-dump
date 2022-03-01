/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Toolkit.HighPerformance;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Metadata;
using SpriteMaster.Types;
using SpriteMaster.Types.Interlocking;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace SpriteMaster;

/// <summary>
/// A wrapper during the resampling process that encapsulates the properties of the sprite itself
/// <para>Warning: <seealso cref="SpriteInfo">SpriteInfo</seealso> holds a reference to the reference texture's data in its <seealso cref="SpriteInfo._ReferenceData">ReferenceData field</seealso>.</para>
/// </summary>
sealed class SpriteInfo : IDisposable {
	internal readonly Texture2D Reference;
	internal readonly Bounds Bounds;
	internal Vector2I ReferenceSize => Reference.Extent();
	internal readonly Vector2B Wrapped;
	internal readonly TextureType TextureType;
	internal readonly uint ExpectedScale;
	private readonly int RawOffset;
	private readonly int RawStride;
	internal readonly XNA.Graphics.BlendState BlendState;
	internal readonly bool BlendEnabled;
	internal readonly bool IsWater;
	internal readonly bool IsFont;
	private volatile bool _Broken = false;

	public override string ToString() => $"SpriteInfo[Name: '{Reference.Name}', ReferenceSize: {ReferenceSize}, Size: {Bounds}]";

	internal ulong SpriteDataHash = 0;
	private byte[]? _ReferenceData = null;
	internal byte[]? ReferenceData {
		get => _ReferenceData;
		private set {
			if (_Broken) {
				return;
			}
			if (_ReferenceData == value) {
				return;
			}
			_ReferenceData = value;
			if (_ReferenceData == null) {
				SpriteDataHash = 0;
			}
			else {
				var result = GetDataHash(_ReferenceData, Reference, Bounds, RawOffset, RawStride);
				if (result.HasValue) {
					SpriteDataHash = result.Value;
				}
				else {
					_Broken = true;
					_ReferenceData = null;
				}
			}
		}
	}

	private static ulong? GetDataHash(byte[] data, Texture2D reference, in Bounds bounds, int rawOffset, int rawStride) {
		//var format = reference.Format.IsCompressed() ? SurfaceFormat.Color : reference.Format;
		int actualWidth = (int)reference.Format.SizeBytes(bounds.Extent.X);

		try {
			var spriteData = new Span2D<byte>(
				array: data,
				offset: rawOffset,
				width: actualWidth,
				height: bounds.Extent.Y,
				// 'pitch' is the distance between the end of one row and the start of another
				// whereas 'stride' is the distance between the starts of rows
				// Ergo, 'pitch' is 'stride' - 'width'.
				pitch: rawStride - actualWidth
			);

			return spriteData.Hash();
		}
		catch (ArgumentOutOfRangeException) {
			float realFormatSize = (float)reference.Format.SizeBytes(4) / 4.0f;

			var errorBuilder = new StringBuilder();
			errorBuilder.AppendLine("SpriteInfo.ReferenceData: arguments out of range");
			errorBuilder.AppendLine($"Reference: {reference.NormalizedName()}");
			errorBuilder.AppendLine($"Reference Extent: {reference.Extent()}");
			errorBuilder.AppendLine($"raw offset: {rawOffset}");
			errorBuilder.AppendLine($"offset: {bounds.Offset}");
			errorBuilder.AppendLine($"extent: {bounds.Extent}");
			errorBuilder.AppendLine($"Format: {reference.Format}");
			errorBuilder.AppendLine($"pitch: {rawStride - actualWidth}");
			errorBuilder.AppendLine($"referenceDataSize: {data.Length}");
			Debug.Error(errorBuilder.ToString());
		}
		return null;
	}

	private InterlockedULong _Hash = 0;
	internal ulong Hash {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		get {
			if (_ReferenceData == null) {
				throw new NullReferenceException(nameof(_ReferenceData));
			}

			ulong hash = _Hash;
			if (hash == 0) {
				hash = Hashing.Combine(
					SpriteDataHash,
					Bounds.Extent.GetLongHashCode(),
					BlendEnabled.GetLongHashCode(),
					ExpectedScale.GetLongHashCode(),
					IsWater.GetLongHashCode(),
					IsFont.GetLongHashCode(),
					Reference.Format.GetLongHashCode()
				);

			}
			_Hash = hash;
			return hash;// ^ (ulong)ExpectedScale.GetHashCode();
		}
	}

	// Attempt to update the bytedata cache for the reference texture, or purge if it that makes more sense or if updating
	// is not plausible.
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void Purge(Texture2D reference, in Bounds? bounds, in DataRef<byte> data) => reference.Meta().Purge(reference, bounds, data);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool IsCached(Texture2D reference) => reference.Meta().CachedDataNonBlocking is not null;

	internal ref struct Initializer {
		internal readonly byte[]? ReferenceData;
		internal readonly Bounds Bounds;
		internal readonly Texture2D Reference;
		internal readonly BlendState BlendState;
		internal readonly SamplerState SamplerState;
		internal readonly uint ExpectedScale;
		internal readonly TextureType TextureType;
		// For statistics and throttling
		internal readonly bool WasCached;
		internal readonly ulong? Hash;

		internal Initializer(Texture2D reference, in Bounds dimensions, uint expectedScale, TextureType textureType) {
			Reference = reference;
			BlendState = DrawState.CurrentBlendState;
			SamplerState = DrawState.CurrentSamplerState;
			ExpectedScale = expectedScale;
			Bounds = dimensions;

			TextureType = textureType;

			// Truncate the bounds so that it fits if it wouldn't otherwise fit
			if (!Bounds.ClampToChecked(Reference.Bounds, out var clampedBounds)) {
				Debug.Warning($"SpriteInfo for '{reference.NormalizedName()}' bounds '{dimensions}' are not contained in reference bounds '{(Bounds)reference.Bounds}'");
				Bounds = clampedBounds;
			}

			var refMeta = reference.Meta();
			var refData = refMeta.CachedData;

			if (refData is null) {
				// TODO : Switch this around to use ReadOnlySequence so our hash is specific to the sprite
				refData = new byte[reference.SizeBytes()];
				Debug.Trace($"Reloading Texture Data (not in cache): {reference.NormalizedName(DrawingColor.LightYellow)}");
				reference.GetData(refData);
				reference.Meta().CachedRawData = refData;
				if (refMeta.IsCompressed) {
					refData = null; // we can only use uncompressed data at this stage.
				}
				WasCached = false;
			}
			else if (refData == Texture2DMeta.BlockedSentinel) {
				refData = null;
				WasCached = false;
			}
			else {
				WasCached = true;
			}

			ReferenceData = refData;

			Hash = null;
			if (Config.SuspendedCache.Enabled) {
				Hash = GetHash();
			}
		}

		private readonly ulong? GetHash() {
			if (ReferenceData is null) {
				return null;
			}

			var format = Reference.Format.IsCompressed() ? SurfaceFormat.Color : Reference.Format;
			int rawStride = (int)format.SizeBytes(Reference.Width);
			int rawOffset = (rawStride * Bounds.Top) + (int)format.SizeBytes(Bounds.Left);

			bool blendEnabled = BlendState.AlphaSourceBlend != Blend.One;
			bool isWater = TextureType == TextureType.Sprite && SpriteOverrides.IsWater(Bounds, Reference);
			bool isFont = !isWater && TextureType == TextureType.Sprite && SpriteOverrides.IsFont(Reference, Bounds.Extent, Reference.Extent());
			var dataHash = GetDataHash(ReferenceData, Reference, Bounds, rawOffset, rawStride);

			if (dataHash is null) {
				return null;
			}

			var result = Hashing.Combine(
				dataHash,
				Bounds.Extent.GetLongHashCode(),
				blendEnabled.GetLongHashCode(),
				ExpectedScale.GetLongHashCode(),
				isWater.GetLongHashCode(),
				isFont.GetLongHashCode(),
				Reference.Format.GetLongHashCode()
			);
			return result;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal SpriteInfo(in Initializer initializer) {
		Reference = initializer.Reference;
		BlendState = initializer.BlendState;
		ExpectedScale = initializer.ExpectedScale;
		Bounds = initializer.Bounds;
		TextureType = initializer.TextureType;
		var format = Reference.Format.IsCompressed() ? SurfaceFormat.Color : Reference.Format;
		RawStride = (int)format.SizeBytes(ReferenceSize.Width);
		RawOffset = (RawStride * Bounds.Top) + (int)format.SizeBytes(Bounds.Left);
		ReferenceData = initializer.ReferenceData;

		if (ReferenceData is null) {
			throw new ArgumentNullException(nameof(initializer.ReferenceData));
		}

		BlendEnabled = initializer.BlendState.AlphaSourceBlend != Blend.One;
		Wrapped = new(
			initializer.SamplerState.AddressU == TextureAddressMode.Wrap,
			initializer.SamplerState.AddressV == TextureAddressMode.Wrap
		);

		IsWater = TextureType == TextureType.Sprite && SpriteOverrides.IsWater(Bounds, Reference);
		IsFont = !IsWater && TextureType == TextureType.Sprite && SpriteOverrides.IsFont(Reference, Bounds.Extent, ReferenceSize);

		if (initializer.Hash.HasValue) {
			_Hash = initializer.Hash.Value;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public void Dispose() {
		ReferenceData = null;
		_Hash = 0;
	}
}
