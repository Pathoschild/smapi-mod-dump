/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using LinqFasterer;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Hashing;
using SpriteMaster.Metadata;
using SpriteMaster.Types;
using SpriteMaster.Types.Interlocking;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SpriteMaster;

/// <summary>
/// A wrapper during the resampling process that encapsulates the properties of the sprite itself
/// <para>Warning: <seealso cref="SpriteInfo">SpriteInfo</seealso> holds a reference to the reference texture's data in its <seealso cref="ReferenceDataInternal">ReferenceData field</seealso>.</para>
/// </summary>
internal sealed class SpriteInfo : IDisposable {
	[Flags]
	internal enum SpriteFlags {
		None = 0,
		IsWater = 1 << 0,
		IsFont = 1 << 1,
		BlendEnabled = 1 << 2,
		HashMask = IsWater | IsFont | BlendEnabled,
		WasCached = 1 << 3,
		Preview = 1 << 4,
		Animated = 1 << 5,
	}

	internal readonly XTexture2D Reference;
	internal readonly Texture2DMeta Meta;
	internal readonly Bounds Bounds;
	internal Vector2I ReferenceSize => Reference.Extent();
	internal readonly Vector2B Wrapped;
	internal readonly TextureType TextureType;
	internal readonly uint ExpectedScale;
	private readonly int RawOffset;
	private readonly int RawStride;
	internal readonly Resample.Scaler Scaler;
	internal readonly Resample.Scaler ScalerPortrait;
	internal readonly Resample.Scaler ScalerText;
	internal readonly Resample.Scaler ScalerGradient;
	internal readonly BlendState BlendState;
	internal readonly SpriteFlags Flags;
	private volatile bool Broken = false;

	internal bool BlendEnabled => Flags.HasFlag(SpriteFlags.BlendEnabled);
	internal bool IsWater => Flags.HasFlag(SpriteFlags.IsWater);
	internal bool IsFont => Flags.HasFlag(SpriteFlags.IsFont);
	internal bool IsAnimated => Flags.HasFlag(SpriteFlags.Animated);
	internal bool IsPreview => Flags.HasFlag(SpriteFlags.Preview);

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

			var result = GetDataHash(ReferenceDataInternal, Reference, Bounds, RawOffset, RawStride, doThrow: false);
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

	private static T? GetTarget<T>(WeakReference<T> reference) where T : class {
		if (reference.TryGetTarget(out var target)) {
			return target;
		}

		return null;
	}

	private static ulong? GetDataHash(byte[] data, XTexture2D reference, Bounds bounds, int rawOffset, int rawStride, bool doThrow) {
		var meta = reference.Meta();

		if (meta.TryGetSpriteHash(bounds, out ulong hash)) {
			return hash;
		}

		var format = reference.Format.IsCompressed() ? SurfaceFormat.Color : reference.Format;
		var actualWidth = format.SizeBytes(bounds.Extent.X);

		int pitch = rawStride - actualWidth;

		try {
			var spriteData = new ReadOnlySpan2D<byte>(
				array: data,
				offset: rawOffset,
				width: actualWidth,
				height: bounds.Extent.Y,
				// 'pitch' is the distance between the end of one row and the start of another
				// whereas 'stride' is the distance between the starts of rows
				// Ergo, 'pitch' is 'stride' - 'width'.
				pitch: pitch
			);

			hash = spriteData.Hash();
			meta.SetSpriteHash(bounds, hash);

			return hash;
		}
		catch (ArgumentOutOfRangeException ex) {
			[MethodImpl(MethodImplOptions.NoInlining)]
			void HandleException() {
				var errorBuilder = new StringBuilder();
				errorBuilder.AppendLine("SpriteInfo.ReferenceData: arguments out of range");

				(string Tag, object? Value) GetField(object? value, [CallerArgumentExpression("value")] string expression = "") {
					return (expression, value);
				}

				var fields = new (string Tag, object? Value)[] {
					("Reference", reference.NormalizedName()),
					GetField(reference.GetType().FullName),
					GetField(reference.Extent()),
					GetField(rawOffset),
					GetField(rawStride),
					GetField(doThrow),
					GetField(bounds.Offset),
					GetField(bounds.Extent),
					GetField(format),
					GetField(pitch),
					GetField(data.Length),
					GetField(meta.ExpectedByteSize),
					GetField(meta.ExpectedByteSizeRaw),
					GetField(meta.CachedData?.Length),
					GetField(meta.CachedRawData?.Length),
					GetField(GetTarget(meta.CachedDataInternal)?.Length),
					GetField(GetTarget(meta.CachedRawDataInternal)?.Length),
					GetField(GetTarget(meta.CachedDataInternal) == meta.CachedData),
					GetField(GetTarget(meta.CachedRawDataInternal) == meta.CachedRawData),
					GetField(meta.Flags),
					GetField(meta.InFlightTasks.Count),
					GetField(meta.Size),
					GetField(meta.Revision),
					GetField(meta.Validation),
					GetField(meta.IsCompressed),
					GetField(meta.Owner.TryGetTarget(out var target) && target == reference),
					GetField(meta.IsSystemRenderTarget),
					GetField(meta.IsAnimated(bounds)),
					GetField(meta.CachedData == data),
				};

				int maxFieldTagLen = fields.MaxF(field => field.Tag.Length);

				foreach (var field in fields) {
					errorBuilder.AppendLine($"  {field.Tag.PadLeft(maxFieldTagLen)}: {field.Value}");
				}

				Debug.Error(ex, errorBuilder.ToString());

				if (!doThrow) {
					Debug.Break();

					reference.Meta().CachedRawData = null;
				}
			}

			HandleException();
			
			if (doThrow) {
				throw;
			}
		}
		return null;
	}



	private InterlockedULong HashInternal = 0;
	internal ulong Hash {
		get {
			if (ReferenceDataInternal is null) {
				return ThrowHelper.ThrowNullReferenceException<ulong>(nameof(ReferenceDataInternal));
			}

			ulong hash = HashInternal;
			if (hash == 0) {
				hash = HashUtility.Combine(
					SpriteDataHash,
					Bounds.Extent.GetLongHashCode(),
					(Flags & SpriteFlags.HashMask).GetLongHashCode(),
					ExpectedScale.GetLongHashCode(),
					Reference.Format.GetLongHashCode(),
					Scaler.GetLongHashCode(),
					ScalerPortrait.GetLongHashCode(),
					ScalerText.GetLongHashCode(),
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

	[StructLayout(LayoutKind.Auto)]
	internal readonly ref struct Initializer {

		internal readonly Bounds Bounds;
		internal readonly byte[]? ReferenceData;
		internal readonly XTexture2D Reference;
		internal readonly Texture2DMeta Meta;
		internal readonly BlendState BlendState;
		internal readonly SamplerState SamplerState;
		internal readonly ulong? Hash = null;
		internal readonly ulong? DataHash = null;
		internal readonly uint ExpectedScale;
		internal readonly TextureType TextureType;
		internal readonly Resample.Scaler Scaler;
		internal readonly Resample.Scaler ScalerPortrait;
		internal readonly Resample.Scaler ScalerText;
		internal readonly Resample.Scaler ScalerGradient;
		// For statistics and throttling
		internal readonly SpriteFlags Flags;

		private readonly record struct HashPair(ulong? SpriteHash, ulong? DataHash) {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static implicit operator HashPair((ulong? SpriteHash, ulong? DataHash) tuple) =>
				new(tuple.SpriteHash, tuple.DataHash);
		}

		internal ulong? HashForced {
			get {
				if (Hash is not { } hash) {
					var calculatedHashes = CalculateHashes(Flags, doThrow: false);
					if (calculatedHashes.DataHash is { } calculatedDataHash) {
						Unsafe.AsRef(DataHash) = calculatedDataHash;
					}
					if (calculatedHashes.SpriteHash is { } calculatedSpriteHash) {
						Unsafe.AsRef(Hash) = hash = calculatedSpriteHash;
					}
					else {
						return null;
					}
				}

				return hash;
			}
		}

		internal ulong? DataHashForced {
			get {
				if (DataHash is not {} dataHash ) {
					var calculatedHashes = CalculateHashes(Flags, doThrow: false);
					if (calculatedHashes.SpriteHash is { } calculatedSpriteHash) {
						Unsafe.AsRef(Hash) = calculatedSpriteHash;
					}
					if (calculatedHashes.DataHash is { } calculatedDataHash) {
						Unsafe.AsRef(DataHash) = dataHash = calculatedDataHash;
					}
					else {
						return null;
					}
				}

				return dataHash;
			}
		}

		internal readonly bool BlendEnabled => Flags.HasFlag(SpriteFlags.BlendEnabled);
		internal readonly bool IsWater => Flags.HasFlag(SpriteFlags.IsWater);
		internal readonly bool IsFont => Flags.HasFlag(SpriteFlags.IsFont);
		internal readonly bool IsAnimated => Flags.HasFlag(SpriteFlags.Animated);
		internal readonly bool IsPreview => Flags.HasFlag(SpriteFlags.Preview);
		internal readonly bool WasCached => Flags.HasFlag(SpriteFlags.WasCached);

		internal class InitializationException : Exception {
			internal InitializationException(string message) : base(message) {

			}

			internal InitializationException(string message, Exception innerException) : base(message, innerException) {

			}

			[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
			internal static void Throw(string message) =>
				throw new InitializationException(message);

			[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
			internal static void Throw(string message, Exception innerException) =>
				throw new InitializationException(message, innerException);
		}

		internal sealed class DataMismatchException : InitializationException {
			private static string MakeMessage(uint actual, uint expected) =>
				$"Data Array size mismatch: actual {actual:N0} != expected {expected:N0}";

			internal DataMismatchException(uint actual, uint expected) : base(MakeMessage(actual, expected)) {

			}

			internal DataMismatchException(uint actual, uint expected, Exception innerException) : base(MakeMessage(actual, expected), innerException) {

			}

			[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
			internal static void Throw(uint actual, uint expected) =>
				throw new DataMismatchException(actual, expected);

			[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
			internal static void Throw(uint actual, uint expected, Exception innerException) =>
				throw new DataMismatchException(actual, expected, innerException);
		}

		internal Initializer(XTexture2D reference, Bounds dimensions, uint expectedScale, TextureType textureType) {
			var flags = SpriteFlags.None;

			Reference = reference;
			var refMeta = Meta = reference.Meta();

			var blendState = BlendState = DrawState.CurrentBlendState;

			SamplerState = DrawState.CurrentSamplerState;
			ExpectedScale = expectedScale;

			// Truncate the bounds so that it fits if it wouldn't otherwise fit
			if (!dimensions.ClampToChecked(reference.Bounds, out var clampedBounds)) {
				if (clampedBounds.IsEmpty) {
					string errorMessage = $"Bounds exception: sprite info source bounds {dimensions} are not within reference bounds {reference.Bounds()}, and clamped value is degenerate";
					Debug.Error(errorMessage);
					InitializationException.Throw(errorMessage);
				}
				else
				{
					Debug.Warning(
						$"SpriteInfo for '{reference.NormalizedName()}' bounds '{dimensions}' are not contained in reference bounds '{(Bounds)reference.Bounds}'"
					);
				}
				dimensions = clampedBounds;
			}
			else if (dimensions.IsEmpty) {
				string errorMessage = $"Bounds exception: sprite info source bounds {dimensions} are degenerate";
				Debug.Error(errorMessage);
				InitializationException.Throw(errorMessage);
			}

			Bounds = dimensions;

			if (Configuration.Preview.Override.Instance is {} instance) {
				flags |= SpriteFlags.Preview;
				Scaler = instance.Scaler;
				ScalerPortrait = instance.ScalerPortrait;
				ScalerText = instance.ScalerText;
				ScalerGradient = instance.ScalerGradient;
			}
			else {
				Scaler = Config.Resample.Scaler;
				ScalerPortrait = Config.Resample.ScalerPortrait;
				ScalerText = Config.Resample.ScalerText;
				ScalerGradient = Config.Resample.ScalerGradient;
			}

			TextureType = textureType;
			var refData = refMeta.CachedData;

			if (refData is null) {
				if (refMeta.CachedRawData is null) {
					// TODO : Switch this around to use ReadOnlySequence so our hash is specific to the sprite
					var tempRefData = GC.AllocateUninitializedArray<byte>(reference.SizeBytes());
					Debug.Trace($"Reloading Texture Data (not in cache): {reference.NormalizedName(DrawingColor.LightYellow)}");
					reference.GetData(tempRefData);
					refMeta.CachedRawData = tempRefData;
					if (!refMeta.IsCompressed) {
						// we can only use uncompressed data at this stage.
						refData = tempRefData;
					}
				}
			}
			else {
				flags |= SpriteFlags.WasCached;
			}

			ReferenceData = refData;

			if (refData is not null) {
				uint refDataLen = (uint)refData.Length;
				uint expectedLength = refMeta.ExpectedByteSize;
				if (refDataLen != expectedLength) {
					ThrowMismatchException(refDataLen, expectedLength);
				}
			}

			if (TextureType == TextureType.Sprite) {
				if (SpriteOverrides.IsWater(Bounds, refMeta)) {
					flags |= SpriteFlags.IsWater;
				}
				else if (SpriteOverrides.IsFont(reference, Bounds.Extent, reference.Extent())) {
					flags |= SpriteFlags.IsFont;
				}
			}

			if (blendState.AlphaSourceBlend != Blend.One) {
				flags |= SpriteFlags.BlendEnabled;
			}

			if (refMeta.IsAnimated(dimensions)) {
				flags |= SpriteFlags.Animated;
				if (Config.SuspendedCache.Enabled) {
					(Hash, DataHash) = CalculateHashes(flags, doThrow: true);
				}
			}

			Flags = flags;
		}

		[DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
		private readonly void ThrowMismatchException(uint actualLength, uint expectedLength, Exception? innerException = null) {
			Debug.Error($"Texture cached data size mismatch: {actualLength} != {expectedLength}");
			Debug.Break();

			Meta.CachedRawData = null;
			if (innerException is not null) {
				DataMismatchException.Throw(actualLength, expectedLength, innerException);
			}
			else {
				DataMismatchException.Throw(actualLength, expectedLength);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private readonly HashPair CalculateHashes(SpriteFlags flags, bool doThrow) {
			try {
				return GetHash(flags, doThrow: doThrow);
			}
			catch (ArgumentOutOfRangeException ex) {
				uint refDataLen = (uint?)ReferenceData?.Length ?? uint.MaxValue;
				uint expectedLength = Meta.ExpectedByteSize;
				ThrowMismatchException(refDataLen, expectedLength, ex);

				return new(null, null);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private readonly HashPair GetHash(SpriteFlags flags, bool doThrow) {
			if (ReferenceData is not {} referenceData) {
				return (null, null);
			}

			var reference = Reference;
			var bounds = Bounds;

			var format = reference.Format.IsCompressed() ? SurfaceFormat.Color : reference.Format;
			int rawStride = format.SizeBytes(reference.Width);
			int rawOffset = (rawStride * bounds.Top) + format.SizeBytes(bounds.Left);

			if (GetDataHash(referenceData, reference, bounds, rawOffset, rawStride, doThrow) is not {} dataHash) {
				return (null, null);
			}

			var result = HashUtility.Combine(
				dataHash,
				bounds.Extent.GetLongHashCode(),
				(flags & SpriteFlags.HashMask).GetLongHashCode(),
				ExpectedScale.GetLongHashCode(),
				reference.Format.GetLongHashCode(),
				Scaler.GetLongHashCode(),
				ScalerPortrait.GetLongHashCode(),
				ScalerText.GetLongHashCode(),
				ScalerGradient.GetLongHashCode()
			);
			return (result, dataHash);
		}
	}

	internal SpriteInfo(in Initializer initializer) {
		Reference = initializer.Reference;
		Meta = initializer.Meta;
		BlendState = initializer.BlendState;
		ExpectedScale = initializer.ExpectedScale;
		Bounds = initializer.Bounds;
		TextureType = initializer.TextureType;

		var format = Reference.Format.IsCompressed() ? SurfaceFormat.Color : Reference.Format;
		var rawStride = RawStride = format.StrideBytes(ReferenceSize);
		RawOffset = (rawStride * Bounds.Top) + format.SizeBytes(Bounds.Left);

		if (initializer.ReferenceData is not { } referenceData) {
			ThrowHelper.ThrowArgumentNullException(nameof(initializer.ReferenceData));
			return;
		}

		ReferenceData = referenceData;

		Scaler = initializer.Scaler;
		ScalerPortrait = initializer.ScalerPortrait;
		ScalerText = initializer.ScalerText;
		ScalerGradient = initializer.ScalerGradient;

		Flags = initializer.Flags;
		var samplerState = initializer.SamplerState;
		Wrapped = new(
			samplerState.AddressU == TextureAddressMode.Wrap,
			samplerState.AddressV == TextureAddressMode.Wrap
		);

		if (initializer.DataHash is {} dataHash) {
			SpriteDataHashInternal = dataHash;
		}
		if (initializer.Hash is {} hash) {
			HashInternal = hash;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public void Dispose() {
		ReferenceData = null;
		Unsafe.AsRef(Meta) = null!;
		HashInternal = 0ul;
		SpriteDataHashInternal = 0ul;
	}
}
