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
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Hashing;
using SpriteMaster.Metadata;
using SpriteMaster.Types;
using SpriteMaster.Types.Interlocking;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace SpriteMaster;

/// <summary>
/// A wrapper during the resampling process that encapsulates the properties of the sprite itself
/// <para>Warning: <seealso cref="SpriteInfo">SpriteInfo</seealso> holds a reference to the reference texture's data in its <seealso cref="ReferenceDataInternal">ReferenceData field</seealso>.</para>
/// </summary>
internal sealed class SpriteInfo : IDisposable {
	internal readonly XTexture2D Reference;
	internal readonly Bounds Bounds;
	internal Vector2I ReferenceSize => Reference.Extent();
	internal readonly Vector2B Wrapped;
	internal readonly TextureType TextureType;
	internal readonly uint ExpectedScale;
	private readonly int RawOffset;
	private readonly int RawStride;
	internal readonly bool IsPreview;
	internal readonly Resample.Scaler Scaler;
	internal readonly Resample.Scaler ScalerGradient;
	internal readonly BlendState BlendState;
	internal readonly bool BlendEnabled;
	internal readonly bool IsWater;
	internal readonly bool IsFont;
	private volatile bool Broken = false;

	public override string ToString() => $"SpriteInfo[Name: '{Reference.Name}', ReferenceSize: {ReferenceSize}, Size: {Bounds}]";

	private ulong? SpriteDataHashInternal = null;
	private ulong? SpriteDataHash {
		get {
			if (SpriteDataHashInternal.HasValue) {
				return SpriteDataHashInternal;
			}

			if (ReferenceDataInternal is null) {
				return null;
			}

			var result = GetDataHash(ReferenceDataInternal, Reference, Bounds, RawOffset, RawStride);
			if (result.HasValue) {
				SpriteDataHashInternal = result.Value;
				return result;
			}

			Broken = true;
			ReferenceDataInternal = null;
			return null;
		}
	}
	private byte[]? ReferenceDataInternal = null;
	internal byte[]? ReferenceData {
		get => ReferenceDataInternal;
		private set {
			if (Broken) {
				return;
			}
			if (ReferenceDataInternal == value) {
				return;
			}
			ReferenceDataInternal = value;
			if (ReferenceDataInternal is null) {
				SpriteDataHashInternal = null;
			}
		}
	}

	private static ulong? GetDataHash(byte[] data, XTexture2D reference, Bounds bounds, int rawOffset, int rawStride) {
		var meta = reference.Meta();

		if (meta.TryGetSpriteHash(bounds, out ulong hash)) {
			return hash;
		}

		var format = reference.Format.IsCompressed() ? SurfaceFormat.Color : reference.Format;
		var actualWidth = format.SizeBytes(bounds.Extent.X);

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

			hash = spriteData.Hash();
			meta.SetSpriteHash(bounds, hash);
			return hash;
		}
		catch (ArgumentOutOfRangeException) {
			var errorBuilder = new StringBuilder();
			errorBuilder.AppendLine("SpriteInfo.ReferenceData: arguments out of range");
			errorBuilder.AppendLine($"Reference: {reference.NormalizedName()}");
			errorBuilder.AppendLine($"Reference Extent: {reference.Extent()}");
			errorBuilder.AppendLine($"raw offset: {rawOffset}");
			errorBuilder.AppendLine($"offset: {bounds.Offset}");
			errorBuilder.AppendLine($"extent: {bounds.Extent}");
			errorBuilder.AppendLine($"Format: {format}");
			errorBuilder.AppendLine($"pitch: {rawStride - actualWidth}");
			errorBuilder.AppendLine($"referenceDataSize: {data.Length}");
			Debug.Error(errorBuilder.ToString());
		}
		return null;
	}

	private InterlockedULong HashInternal = 0;
	internal ulong Hash {
		get {
			if (ReferenceDataInternal is null) {
				throw new NullReferenceException(nameof(ReferenceDataInternal));
			}

			ulong hash = HashInternal;
			if (hash == 0) {
				hash = HashUtility.Combine(
					SpriteDataHash,
					Bounds.Extent.GetLongHashCode(),
					BlendEnabled.GetLongHashCode(),
					ExpectedScale.GetLongHashCode(),
					IsWater.GetLongHashCode(),
					IsFont.GetLongHashCode(),
					Reference.Format.GetLongHashCode(),
					Scaler.GetLongHashCode(),
					ScalerGradient.GetLongHashCode()
				);

			}
			HashInternal = hash;
			return hash;
		}
	}

	// Attempt to update the bytedata cache for the reference texture, or purge if it that makes more sense or if updating
	// is not plausible.
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void Purge(XTexture2D reference, Bounds? bounds, in DataRef<byte> data, bool animated) =>
		reference.Meta().Purge(reference, bounds, data, animated: animated);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool IsCached(XTexture2D reference) =>
		reference.Meta().CachedDataNonBlocking is not null;

	internal readonly ref struct Initializer {
		internal readonly Bounds Bounds;
		internal readonly byte[]? ReferenceData;
		internal readonly XTexture2D Reference;
		internal readonly BlendState BlendState;
		internal readonly SamplerState SamplerState;
		internal readonly ulong? Hash;
		internal readonly uint ExpectedScale;
		internal readonly TextureType TextureType;
		internal readonly Resample.Scaler Scaler;
		internal readonly Resample.Scaler ScalerGradient;
		// For statistics and throttling
		internal readonly bool WasCached;
		internal readonly bool IsPreview;

		internal Initializer(XTexture2D reference, Bounds dimensions, uint expectedScale, TextureType textureType, bool animated) {
			Reference = reference;
			BlendState = DrawState.CurrentBlendState;
			SamplerState = DrawState.CurrentSamplerState;
			ExpectedScale = expectedScale;
			Bounds = dimensions;
			IsPreview = Configuration.Preview.Override.Instance is not null;
			Scaler = Configuration.Preview.Override.Instance?.Scaler ?? Config.Resample.Scaler;
			ScalerGradient = Configuration.Preview.Override.Instance?.ScalerGradient ?? Config.Resample.ScalerGradient;

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
			if (Config.SuspendedCache.Enabled && animated) {
				Hash = GetHash();
			}
		}

		private ulong? GetHash() {
			if (ReferenceData is null) {
				return null;
			}

			var format = Reference.Format.IsCompressed() ? SurfaceFormat.Color : Reference.Format;
			int rawStride = format.SizeBytes(Reference.Width);
			int rawOffset = (rawStride * Bounds.Top) + format.SizeBytes(Bounds.Left);

			bool blendEnabled = BlendState.AlphaSourceBlend != Blend.One;
			bool isWater = TextureType == TextureType.Sprite && SpriteOverrides.IsWater(Bounds, Reference);
			bool isFont = !isWater && TextureType == TextureType.Sprite && SpriteOverrides.IsFont(Reference, Bounds.Extent, Reference.Extent());
			var dataHash = GetDataHash(ReferenceData, Reference, Bounds, rawOffset, rawStride);

			if (dataHash is null) {
				return null;
			}

			var result = HashUtility.Combine(
				dataHash,
				Bounds.Extent.GetLongHashCode(),
				blendEnabled.GetLongHashCode(),
				ExpectedScale.GetLongHashCode(),
				isWater.GetLongHashCode(),
				isFont.GetLongHashCode(),
				Reference.Format.GetLongHashCode(),
				Scaler.GetLongHashCode(),
				ScalerGradient.GetLongHashCode()
			);
			return result;
		}
	}

	internal SpriteInfo(in Initializer initializer) {
		Reference = initializer.Reference;
		BlendState = initializer.BlendState;
		ExpectedScale = initializer.ExpectedScale;
		Bounds = initializer.Bounds;
		TextureType = initializer.TextureType;
		var format = Reference.Format.IsCompressed() ? SurfaceFormat.Color : Reference.Format;
		RawStride = format.SizeBytes(ReferenceSize.Width);
		RawOffset = (RawStride * Bounds.Top) + format.SizeBytes(Bounds.Left);
		ReferenceData = initializer.ReferenceData;
		IsPreview = initializer.IsPreview;
		Scaler = initializer.Scaler;
		ScalerGradient = initializer.ScalerGradient;

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
			HashInternal = initializer.Hash.Value;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public void Dispose() {
		ReferenceData = null;
		HashInternal = 0;
	}
}
