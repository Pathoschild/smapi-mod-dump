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
using SpriteMaster.Caching;
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Hashing;
using SpriteMaster.Metadata;
using SpriteMaster.Resample.Passes;
using SpriteMaster.Tasking;
using SpriteMaster.Types;
using SpriteMaster.Types.Fixed;
using SpriteMaster.Types.Spans;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace SpriteMaster.Resample;

internal sealed class Resampler {
	internal enum ResampleStatus {
		Unknown = -1,
		Success = 0,
		Failure = 1,
		DisabledGradient = 2,
		DisabledSolid = 3,
		Disabled = 4,
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void PurgeHash(XTexture2D reference) {
		reference.Meta().CachedRawData = null;
	}

	internal static ulong GetHash(SpriteInfo input, TextureType textureType) {
		// Need to make Hashing.CombineHash work better.
		var hash = input.Hash;

		if (Config.Resample.EnableDynamicScale) {
			hash = HashUtility.Combine(hash, HashUtility.Rehash(input.ExpectedScale));
		}

		if (textureType == TextureType.Sprite) {
			hash = HashUtility.Combine(hash, input.Bounds.Extent.GetLongHashCode());
		}
		return hash;
	}

	internal static ulong? GetHash(in SpriteInfo.Initializer input, TextureType textureType) {
		if (!input.Hash.HasValue) {
			return null;
		}

		// Need to make Hashing.CombineHash work better.
		var hash = input.Hash.Value;

		if (Config.Resample.EnableDynamicScale) {
			hash = HashUtility.Combine(hash, HashUtility.Rehash(input.ExpectedScale));
		}

		if (textureType == TextureType.Sprite) {
			hash = HashUtility.Combine(hash, input.Bounds.Extent.GetLongHashCode());
		}
		return hash;
	}

	private static readonly WeakSet<XTexture2D> GarbageMarkSet = new();

	private const int WaterBlock = 4;

	// TODO : use MemoryFailPoint class. Extensively.

	private enum GammaState {
		Linear,
		Gamma
	}

	//internal static readonly ArrayPool<Color16> ResamplerArrayPool = ArrayPool<Color16>.Shared;

	private static unsafe ReadOnlyPinnedSpan<byte> CreateNewTexture(
		ManagedSpriteInstance texture,
		bool async,
		SpriteInfo input,
		string hashString,
		ref Vector2B wrapped,
		ref uint scale,
		out Vector2I size,
		out TextureFormat format,
		out PaddingQuad padding,
		out Vector2I blockPadding,
		out IScalerInfo? scalerInfo,
		out bool isGradient,
		out ResampleStatus result
	) {
		if (input.ReferenceData is null) {
			throw new ArgumentNullException(nameof(input.ReferenceData));
		}

		scalerInfo = null;
		padding = PaddingQuad.Zero;
		blockPadding = Vector2I.Zero;
		isGradient = false;

		string MakeDumpPath(in Analysis.LegacyResults? analysis = null, PaddingQuad? padding = null, string? subPath = null, string[]? modifiers = null) {
			var normalizedName = input.Reference.NormalizedName().Replace('\\', '.');
			var dumpPath = new StringBuilder();
			dumpPath.Append($"{normalizedName}.{hashString}");
			if (analysis.HasValue) {
				static string SimplifyBools(Vector2B vec) {
					return $"{vec.X.ToInt()}{vec.Y.ToInt()}";
				}

				dumpPath.Append($"-wrap[{SimplifyBools(analysis.Value.Wrapped)}]-repeat[{SimplifyBools(analysis.Value.RepeatX)},{SimplifyBools(analysis.Value.RepeatY)}]");
			}
			if (padding.HasValue) {
				dumpPath.Append($"-pad[{padding.Value.X},{padding.Value.Y}]");
			}
			if (modifiers is not null && modifiers.Length > 0) {
				dumpPath.Append($".{string.Join(',', modifiers)}");
			}
			if (subPath is not null) {
				return FileCache.GetDumpPath(subPath, $"{dumpPath}.png");
			}
			else {
				return FileCache.GetDumpPath($"{dumpPath}.png");
			}
		}

		if (Config.Debug.Sprite.DumpReference) {
			Textures.DumpTexture(
				source: input.ReferenceData,
				sourceSize: input.ReferenceSize,
				destBounds: input.Bounds,
				path: MakeDumpPath(modifiers: new[] { "reference" })
			);
		}

		var initialGammaState = GammaState.Gamma;
		var currentGammaState = initialGammaState;

		bool directImage = input.TextureType == TextureType.SlicedImage;
		Bounds inputBounds;
		switch (input.TextureType) {
			case TextureType.Sprite:
				inputBounds = input.Bounds;
				break;
			case TextureType.Image:
				inputBounds = input.ReferenceSize;
				break;
			case TextureType.SlicedImage:
				inputBounds = input.Bounds;
				break;
			default:
				throw new NotImplementedException("Unknown Texture Type provided");
		}

		// Water in the game is pre-upscaled by 4... which is weird.
		int blockSize = 1;
		if (input.IsWater || input.Reference == StardewValley.Game1.rainTexture) {
			blockSize = WaterBlock;
		}
		/*
		else if (input.IsFont && FontBlock != 1) {
			blockSize = FontBlock;
			scale = Config.Resample.MaxScale;
		}
		*/
		else if (SMConfig.Resample.FourXTextures.AnyF(prefix => input.Reference.NormalizedName().StartsWith(prefix))) {
			blockSize = 4;
		}
		else if (SMConfig.Resample.TwoXTextures.AnyF(prefix => input.Reference.NormalizedName().StartsWith(prefix))) {
			blockSize = 2;
		}
		else if (SMConfig.Resample.BlockMultipleAnalysis.Enabled) {
			blockSize = BlockMultipleAnalysis.Analyze(
				data: input.ReferenceData.AsSpan<Color8>(),
				textureBounds: input.Reference.Bounds,
				spriteBounds: inputBounds,
				stride: input.ReferenceSize.Width
			);
		}

		if (blockSize == Math.Max(inputBounds.Width, inputBounds.Height)) {
			if (!Config.Resample.Recolor.Enabled) {
				result = ResampleStatus.DisabledSolid;
				size = default;
				format = default;
				return PinnedSpan<byte>.Empty;
			}
		}

		// TODO : handle inverted input.Bounds
		ReadOnlySpan<Color8> spriteRawData8;
		Vector2I spriteRawExtent;
		if (blockSize <= 1 && inputBounds == input.Reference.Bounds) {
			spriteRawData8 = input.ReferenceData.AsReadOnlySpan<Color8>();
			spriteRawExtent = inputBounds.Extent;
		}
		else {
			spriteRawData8 = ExtractSprite.Extract(
				data: input.ReferenceData.AsSpan<Color8>(),
				textureBounds: input.Reference.Bounds,
				spriteBounds: inputBounds,
				stride: input.ReferenceSize.Width,
				block: blockSize,
				newExtent: out spriteRawExtent
			);
		}

		if (Config.Debug.Sprite.DumpReference) {
			Textures.DumpTexture(
				source: spriteRawData8,
				sourceSize: spriteRawExtent,
				destBounds: spriteRawExtent,
				path: MakeDumpPath(modifiers: new[] { "reference", "reduced" })
			);
		}

		// At this point, rawData includes just the sprite's raw data.

		var analysis = Analysis.AnalyzeLegacy(
			reference: input.Reference,
			data: spriteRawData8,
			bounds: spriteRawExtent,
			wrapped: input.Wrapped
		);

		isGradient = analysis.MaxChannelShades >= Config.Resample.Analysis.MinimumGradientShades && (analysis.GradientDiagonal.Any || analysis.GradientAxial.Any);

		if (isGradient) {
			foreach (var blacklistPattern in Config.Resample.GradientBlacklistPatterns) {
				if (blacklistPattern.IsMatch(input.Reference.NormalizedName())) {
					isGradient = false;
					break;
				}
			}
		}

		if (isGradient) {
			if (Config.Debug.Sprite.DumpReference) {
				Textures.DumpTexture(
					source: input.ReferenceData,
					sourceSize: input.ReferenceSize,
					destBounds: input.Bounds,
					path: MakeDumpPath(analysis: analysis, subPath: "gradient", modifiers: new[] { "reference" })
				);
			}
		}

		var scalerType = isGradient ? input.ScalerGradient : input.Scaler;

		if (
			!Config.Resample.Recolor.Enabled &&
			!Config.Resample.IsEnabled ||
			scalerType == Scaler.None
		) {
			result = isGradient ? ResampleStatus.DisabledGradient : ResampleStatus.Disabled;
			size = default;
			format = default;
			return PinnedSpan<byte>.Empty;
		}

		if (analysis.MaxChannelShades <= 1) {
			// If the sprite only has _one_ shade, resampling makes zero sense.
			if (Config.Debug.Sprite.DumpReference) {
				Textures.DumpTexture(
					source: input.ReferenceData,
					sourceSize: input.ReferenceSize,
					destBounds: input.Bounds,
					path: MakeDumpPath(analysis: analysis, subPath: "shadeless", modifiers: new[] { "reference" })
				);
			}
			if (!Config.Resample.Recolor.Enabled) {
				result = ResampleStatus.DisabledSolid;
				size = default;
				format = default;
				return PinnedSpan<byte>.Empty;
			}
		}

		bool resamplingAllowed = Config.Resample.IsEnabled || (Configuration.Preview.Override.Instance?.Scaler ?? Config.Resample.Scaler) == Scaler.None;

		if (Config.Resample.EnableWrappedAddressing) {
			wrapped = analysis.Wrapped;
		}
		else {
			wrapped = (false, false);
		}

		var scaledSize = resamplingAllowed ? spriteRawExtent * scale : spriteRawExtent;
		var scaledSizeClamped = scaledSize.Min(Config.ClampDimension);

		// Widen data.
		var spriteRawData = Color16.Convert(spriteRawData8);

		// Apply recolor
		if (Config.Resample.Recolor.Enabled) {
			for (int i = 0; i < spriteRawData.Length; ++i) {
				ref Color16 color = ref spriteRawData[i];
				float r = Math.Clamp((float)(color.R.RealF * Config.Resample.Recolor.RScalar), 0.0f, 1.0f);
				float g = Math.Clamp((float)(color.G.RealF * Config.Resample.Recolor.GScalar), 0.0f, 1.0f);
				float b = Math.Clamp((float)(color.B.RealF * Config.Resample.Recolor.BScalar), 0.0f, 1.0f);
				color.R = Fixed16.FromReal(r);
				color.G = Fixed16.FromReal(g);
				color.B = Fixed16.FromReal(b);
			}
		}

		Span<Color16> bitmapDataWide = spriteRawData;

		scalerInfo = Scalers.IScaler.GetScalerInfo(scalerType);

		if (scalerInfo is not null) {
			scale *= (uint)blockSize;
			scale = Math.Clamp(scale, (uint)scalerInfo.MinScale, (uint)scalerInfo.MaxScale);

			// Adjust the scale value so that it is within the preferred dimensional limits
			if (Config.Resample.Scale) {
				var originalScale = scale;
				scale = 2;
				for (uint s = originalScale; s > 2U; --s) {
					var newDimensions = spriteRawExtent * s;
					if (newDimensions.MaxOf <= Config.PreferredMaxTextureDimension) {
						scale = s;
						break;
					}
				}
			}

			bool premultiplyAlpha = Config.Resample.PremultiplyAlpha && scalerInfo.PremultiplyAlpha;
			bool gammaCorrect = Config.Resample.AssumeGammaCorrected && scalerInfo.GammaCorrect; // There is no reason to perform this pass with EPX, as EPX does not blend.

			bool handlePadding = !directImage;

			if (handlePadding) {
				if (Padding.IsBlacklisted(inputBounds, input.Reference)) {
					handlePadding = false;
				}
			}

			// Apply padding to the sprite if necessary
			if (handlePadding) {
				spriteRawData = Padding.Apply(
					data: spriteRawData,
					spriteSize: spriteRawExtent,
					scale: scale,
					input: input,
					forcePadding: input.IsFont || input.Reference.Format.IsCompressed(),
					analysis: analysis,
					padding: out padding,
					paddedSize: out spriteRawExtent
				);
			}

			scaledSize = spriteRawExtent * scale;
			scaledSizeClamped = scaledSize.Min(Config.ClampDimension);

			bitmapDataWide = SpanExt.Make<Color16>(scaledSize.Area);

			try {
				var doWrap = wrapped | input.IsWater;

				if (gammaCorrect && currentGammaState == GammaState.Gamma) {
					GammaCorrection.Linearize(spriteRawData, spriteRawExtent);
					currentGammaState = GammaState.Linear;
				}

				if (premultiplyAlpha) {
					PremultipliedAlpha.Reverse(spriteRawData, spriteRawExtent);
				}

				if (Config.Resample.Deposterization.PreEnabled) {
					spriteRawData = Deposterize.Enhance(spriteRawData, spriteRawExtent, doWrap);

					if (Config.Debug.Sprite.DumpReference) {
						Textures.DumpTexture(
							source: spriteRawData,
							sourceSize: spriteRawExtent,
							adjustGamma: 2.2,
							path: MakeDumpPath(analysis: analysis, modifiers: new[] { "reference", "deposter" })
						);
					}
				}

				var scaler = scalerInfo.Interface;

				var scalerConfig = scaler.CreateConfig(
					wrapped: doWrap,
					hasAlpha: true,
					gammaCorrected: currentGammaState == GammaState.Gamma
				);

				bitmapDataWide = scaler.Apply(
					configuration: scalerConfig,
					scaleMultiplier: scale,
					sourceData: spriteRawData,
					sourceSize: spriteRawExtent,
					targetSize: scaledSize,
					targetData: bitmapDataWide
				);

				if (Config.Resample.Deposterization.PostEnabled) {
					bitmapDataWide = Deposterize.Enhance(bitmapDataWide, scaledSize, doWrap);
				}

				if (Config.Resample.UseColorEnhancement) {
					bitmapDataWide = Recolor.Enhance(bitmapDataWide, scaledSize);
				}

				if (premultiplyAlpha) {
					PremultipliedAlpha.Apply(bitmapDataWide, scaledSize);
				}

				if (gammaCorrect && currentGammaState == GammaState.Linear) {
					GammaCorrection.Delinearize(bitmapDataWide, scaledSize);
					currentGammaState = GammaState.Gamma;
				}
			}
			catch (Exception ex) {
				ex.PrintError();
				throw;
			}
			//ColorSpace.ConvertLinearToSRGB(bitmapData, Texel.Ordering.ARGB);
		}
		{
			scale = 1;
		}

		if (!padding.IsZero) {
			// Trim excess padding

			// Check initial rows
			// Check ending rows

			// Check initial columns
			// Check ending columns
		}

		if (Config.Debug.Sprite.DumpResample) {
			Textures.DumpTexture(
				source: bitmapDataWide,
				sourceSize: scaledSize,
				//swap: (2, 1, 0, 4),
				path: MakeDumpPath(analysis: analysis, padding: padding, modifiers: new[] { "resample", })
			);
		}

		if (scaledSize != scaledSizeClamped) {
			if (scaledSize.Width < scaledSizeClamped.Width || scaledSize.Height < scaledSizeClamped.Height) {
				throw new Exception($"Resampled texture size {scaledSize} is smaller than expected {scaledSizeClamped}");
			}

			Debug.Trace($"Sprite {texture.NormalizedName()} requires rescaling");
			// This should be incredibly rare - we very rarely need to scale back down.
			// I don't actually have a solution for this case.
			scaledSizeClamped = scaledSize;
		}

		format = TextureFormat.Color;

		if (currentGammaState != initialGammaState) {
			throw new Exception("Gamma State Mismatch");
		}

		// Narrow
		var bitmapData = Color8.ConvertPinned(bitmapDataWide);
		var resultData = bitmapData.AsBytes();

		if (Config.Debug.Sprite.DumpResample) {
			Textures.DumpTexture(
				source: bitmapData,
				sourceSize: scaledSize,
				//swap: (2, 1, 0, 4),
				path: MakeDumpPath(analysis: analysis, padding: padding, modifiers: new[] { "resample", "narrowed" })
			);
		}

		bool doRecompress = true;

		// if the texture was originally compressed, do not recompress it for now. There seems to be a bug in the compressor in these cases.
		//if (input.Reference.Format.IsCompressed()) {
		//	doRecompress = false;
		//}

		// TODO : For some reason, block compression gives incorrect results with EPX. I need to investigate this at some point.
		if (Config.Resample.BlockCompression.Enabled && doRecompress && scaledSizeClamped.MinOf >= 4 && (scalerInfo?.BlockCompress ?? true)) {
			// TODO : We can technically allocate the block padding before the scaling phase, and pass it a stride
			// so it will just ignore the padding areas. That would be more efficient than this.

			// Check for special cases
			bool hasAlpha = true;
			bool isPunchThroughAlpha = false;
			bool isMasky = false;
			bool hasR = true;
			bool hasG = true;
			bool hasB = true;
			{
				const int maxShades = byte.MaxValue + 1;

				Span<int> alpha = stackalloc int[maxShades];
				Span<int> blue = stackalloc int[maxShades];
				Span<int> green = stackalloc int[maxShades];
				Span<int> red = stackalloc int[maxShades];
				for (int i = 0; i < maxShades; ++i) {
					alpha[i] = 0;
					blue[i] = 0;
					green[i] = 0;
					red[i] = 0;
				}
				
				for (int i = 0; i < bitmapData.Length; ++i) {
					var color = bitmapData[i];
					alpha[color.A.Value]++;
					blue[color.B.Value]++;
					green[color.G.Value]++;
					red[color.R.Value]++;
				}

				hasR = red[0] != bitmapData.Length;
				hasG = green[0] != bitmapData.Length;
				hasB = blue[0] != bitmapData.Length;

				//Debug.Warning($"Punch-through Alpha: {intData.Length}");
				isPunchThroughAlpha = isMasky = alpha[0] + alpha[maxShades - 1] == bitmapData.Length;
				hasAlpha = alpha[maxShades - 1] != bitmapData.Length;

				if (hasAlpha && !isPunchThroughAlpha) {
					var alphaDeviation = Statistics.StandardDeviation(alpha, maxShades, 1, maxShades - 2);
					isMasky = alphaDeviation < Config.Resample.BlockCompression.HardAlphaDeviationThreshold;
				}
			}

			ReadOnlySpan<Color8> uncompressedBitmapData = bitmapData;
			var originalScaledSizeClamped = scaledSizeClamped;
			var originalBlockPadding = blockPadding;

			if (!Decoder.BlockDecoderCommon.IsBlockMultiple(scaledSizeClamped)) {
				var blockPaddedSize = scaledSizeClamped + 3 & ~3;

				var spanDst = SpanExt.Make<Color8>(blockPaddedSize.Area);
				var spanSrc = bitmapData;

				int y;
				// Copy data
				for (y = 0; y < scaledSizeClamped.Y; ++y) {
					var newBufferOffset = y * blockPaddedSize.X;
					var bitmapOffset = y * scaledSizeClamped.X;
					int x;
					for (x = 0; x < scaledSizeClamped.X; ++x) {
						spanDst[newBufferOffset + x] = spanSrc[bitmapOffset + x];
					}
					// Extent X across
					int lastX = x - 1;
					for (; x < blockPaddedSize.X; ++x) {
						spanDst[newBufferOffset + x] = spanSrc[bitmapOffset + lastX];
					}
				}
				// Extend existing data
				var lastY = y - 1;
				var sourceOffset = lastY * blockPaddedSize.X;
				for (; y < blockPaddedSize.Y; ++y) {
					int newBufferOffset = y * blockPaddedSize.X;
					for (int x = 0; x < blockPaddedSize.X; ++x) {
						spanDst[newBufferOffset + x] = spanDst[sourceOffset + x];
					}
				}

				uncompressedBitmapData = spanDst;
				blockPadding += blockPaddedSize - scaledSizeClamped;
				scaledSizeClamped = blockPaddedSize;

				if (Config.Debug.Sprite.DumpResample) {
					Textures.DumpTexture(
						source: uncompressedBitmapData,
						sourceSize: blockPaddedSize,
						// swap: (2, 1, 0, 4),
						path: MakeDumpPath(analysis: analysis, padding: padding, modifiers: new[] { "resample", "blockpadded" })
					);
				}
			}

			if (!TextureEncode.Encode(
					data: uncompressedBitmapData,
					format: ref format,
					dimensions: scaledSizeClamped,
					hasAlpha: hasAlpha,
					isPunchthroughAlpha: isPunchThroughAlpha,
					isMasky: isMasky,
					hasR: hasR,
					hasG: hasG,
					hasB: hasB,
					result: out resultData
				)) {
				resultData = bitmapData.AsBytes();
				blockPadding = originalBlockPadding;
				scaledSizeClamped = originalScaledSizeClamped;
			}
		}

		size = scaledSizeClamped;
		result = ResampleStatus.Success;
		return resultData;
	}

	internal static ManagedTexture2D? Upscale(ManagedSpriteInstance spriteInstance, ref uint scale, SpriteInfo input, ulong hash, ref Vector2B wrapped, bool async) {
		try {
			// Try to process the texture twice. Garbage collect after a failure, maybe it'll work then.
			for (int i = 0; i < 2; ++i) {
				try {
					var resultTexture = UpscaleInternal(
						spriteInstance: spriteInstance,
						scale: ref scale,
						input: input,
						hash: hash,
						wrapped: ref wrapped,
						async: async,
						result: out var result
					);

					if (result is not (ResampleStatus.DisabledGradient or ResampleStatus.DisabledSolid)) {
						return resultTexture;
					}

					Debug.Trace($"Skipping resample of {spriteInstance.Name} {input.Bounds}: NoResample");
					spriteInstance.NoResample = true;
					if (input.Reference is { } texture) {
						texture.Meta().AddNoResample(input.Bounds);
					}

					return null;

				}
				catch (OutOfMemoryException) {
					Debug.Warning("OutOfMemoryException encountered during Upscale, garbage collecting and deferring.");
					Garbage.Collect(compact: true, blocking: true, background: false);
				}
			}
		}
		catch (Exception ex) {
			Debug.Error($"Internal Error processing '{input}'", ex);
		}

		spriteInstance.Texture = null;
		return null;
	}

	internal static readonly Action<XTexture2D, int, byte[], int, int>? PlatformSetData = typeof(XTexture2D).GetMethods(
		BindingFlags.Instance | BindingFlags.NonPublic
	).SingleF(m => m.Name == "PlatformSetData" && m.GetParameters().Length == 4).MakeGenericMethod(typeof(byte))
		.CreateDelegate<Action<XTexture2D, int, byte[], int, int>>();

	private static ManagedTexture2D? UpscaleInternal(ManagedSpriteInstance spriteInstance, ref uint scale, SpriteInfo input, ulong hash, ref Vector2B wrapped, bool async, out ResampleStatus result) {
		var spriteFormat = TextureFormat.Color;

		if (Config.Garbage.CollectAccountUnownedTextures && GarbageMarkSet.Add(input.Reference)) {
			Garbage.Mark(input.Reference);
			// TODO : this won't be hit if the object is finalized without disposal
			input.Reference.Disposing += (obj, _) => {
				Garbage.Unmark((XTexture2D)obj!);
			};
		}

		var hashString = hash.ToString("x");

		result = ResampleStatus.Unknown;

		try {
			var newSize = Vector2I.Zero;

			var cachePath = FileCache.GetPath($"{hashString}.cache");

			ReadOnlyPinnedSpan<byte> pinnedBitmapData = default;
			ReadOnlySpan<byte> bitmapData;
			try {
				if (FileCache.Fetch(
					path: cachePath,
					scale: out var fetchScale,
					size: out newSize,
					format: out spriteFormat,
					wrapped: out wrapped,
					padding: out spriteInstance.Padding,
					blockPadding: out spriteInstance.BlockPadding,
					scalerInfo: out spriteInstance.ScalerInfo,
					gradient: out bool gradient,
					data: out bitmapData
				)) {
					scale = fetchScale;
				}
				else {
					bitmapData = null;
				}

				var referenceScaler = gradient ? input.ScalerGradient : input.Scaler;
				if (spriteInstance.ScalerInfo?.Scaler != referenceScaler) {
					bitmapData = null;
				}
			}
			catch (Exception ex) {
				ex.PrintWarning();
				bitmapData = null;
			}

			if (bitmapData.IsEmpty) {
				bool isGradient = false;

				try {
					bitmapData = pinnedBitmapData = CreateNewTexture(
						async: async,
						texture: spriteInstance,
						input: input,
						hashString: hashString,
						wrapped: ref wrapped,
						scale: ref scale,
						size: out newSize,
						format: out spriteFormat,
						padding: out spriteInstance.Padding,
						blockPadding: out spriteInstance.BlockPadding,
						scalerInfo: out spriteInstance.ScalerInfo,
						isGradient: out isGradient,
						result: out result
					);

					if (result is (ResampleStatus.DisabledGradient or ResampleStatus.DisabledSolid or ResampleStatus.Disabled)) {
						return null;
					}
				}
				catch (OutOfMemoryException) {
					Debug.Error($"OutOfMemoryException thrown trying to create texture [texture: {spriteInstance.NormalizedName()}, bounds: {input.Bounds}, textureSize: {input.ReferenceSize}, scale: {scale}]");
					throw;
				}

				try {
					FileCache.Save(
						path: cachePath,
						scale: scale,
						size: newSize,
						format: spriteFormat,
						wrapped: wrapped,
						padding: spriteInstance.Padding,
						blockPadding: spriteInstance.BlockPadding,
						scalerInfo: spriteInstance.ScalerInfo,
						gradient: isGradient,
						data: pinnedBitmapData
					);
				}
				catch {
					// ignored
				}
			}

			spriteInstance.UnpaddedSize = newSize - (spriteInstance.Padding.Sum + spriteInstance.BlockPadding);
			spriteInstance.InnerRatio = (Vector2F)newSize / (Vector2F)spriteInstance.UnpaddedSize;

			ManagedTexture2D? CreateTexture(ReadOnlyPinnedSpan<byte>.FixedSpan data) {
				if (input.Reference.GraphicsDevice.IsDisposed) {
					return null;
				}
				//var newTexture2 = GL.GLTexture.CreateTexture2D(newSize, false, spriteFormat);
				var newTexture = new ManagedTexture2D(
					data: data,
					instance: spriteInstance,
					reference: input.Reference,
					dimensions: newSize,
					format: spriteFormat
				);

				return newTexture;
			}

			var isAsync = Config.AsyncScaling.Enabled && async;
			if (!isAsync || Config.AsyncScaling.ForceSynchronousStores || DrawState.ForceSynchronous) {
				var reference = input.Reference;

				byte[]? bitmapDataArray = null;
				if (pinnedBitmapData.IsEmpty) {
					bitmapDataArray = bitmapData.ToArray();
				}

				void SyncCallFixed(ReadOnlyPinnedSpan<byte>.FixedSpan data) {
					if (reference.IsDisposed) {
						return;
					}
					if (spriteInstance.IsDisposed) {
						return;
					}
					ManagedTexture2D? newTexture = null;
					try {
						newTexture = CreateTexture(data);
						spriteInstance.Texture = newTexture;
						spriteInstance.Finish();
					}
					catch (Exception ex) {
						ex.PrintError();
						newTexture?.Dispose();
						spriteInstance.Dispose();
					}
				}

				void SyncCallArray() {
					if (reference.IsDisposed) {
						return;
					}
					if (spriteInstance.IsDisposed) {
						return;
					}

					unsafe {
						fixed (byte* ptr = bitmapDataArray) {
							var pinnedSpan = new ReadOnlyPinnedSpan<byte>(bitmapDataArray, ptr, bitmapDataArray.Length);
							SyncCallFixed(pinnedSpan.Fixed);
						}
					}
				}

				if (bitmapDataArray is null) {
					var fixedData = pinnedBitmapData.Fixed;
					SynchronizedTaskScheduler.Instance.QueueDeferred(
						() => SyncCallFixed(fixedData),
						new(input.Reference.NormalizedName(), bitmapData.Length)
					);
				}
				else {
					SynchronizedTaskScheduler.Instance.QueueDeferred(
						SyncCallArray,
						new(input.Reference.NormalizedName(), bitmapData.Length)
					);
				}
				return null;
			}
			else {
				ManagedTexture2D? newTexture = null;
				try {
					// TODO : this is very inefficient
					if (pinnedBitmapData.IsEmpty) {
						var asArray = bitmapData.ToArray();
						unsafe {
							fixed (byte* ptr = asArray) {
								var pinnedSpan = new ReadOnlyPinnedSpan<byte>(asArray, ptr, asArray.Length);
								newTexture = CreateTexture(pinnedSpan.Fixed);
							}
						}
					}
					else {

					}
					newTexture = CreateTexture(pinnedBitmapData.Fixed);
					if (!isAsync) {
						return newTexture;
					}

					spriteInstance.Texture = newTexture;
					spriteInstance.Finish();
					return newTexture;
				}
				catch (Exception ex) {
					ex.PrintError();
					newTexture?.Dispose();
				}
			}
		}
		catch (Exception ex) {
			ex.PrintError();
		}

		//TextureCache.Add(hash, output);
		return null;
	}
}
