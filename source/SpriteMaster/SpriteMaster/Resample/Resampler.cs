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
using Microsoft.Xna.Framework.Graphics;
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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

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

	[StructLayout(LayoutKind.Auto)]
	private readonly ref struct NewTextureResult {
		internal readonly ResampleStatus Status { get; init; } = default;
		internal readonly ReadOnlyPinnedSpan<byte> Data { get; init; } = default;
		internal readonly Vector2B Wrapped { get; init; } = default;
		internal readonly uint Scale { get; init; } = default;
		internal readonly Vector2I Size { get; init; } = default;
		internal readonly TextureFormat Format { get; init; } = default;
		internal readonly PaddingQuad Padding { get; init; } = default;
		internal readonly Vector2I BlockPadding { get; init; } = default;
		internal readonly IScalerInfo? ScalerInfo { get; init; } = default;
		internal readonly bool IsGradient { get; init; } = default;

		public NewTextureResult() { }

		private NewTextureResult(ResampleStatus status) {
			Status = status;
		}

		internal static NewTextureResult FromFailure(ResampleStatus status) => new(status);
	}

	private static unsafe NewTextureResult CreateNewTexture(
		ManagedSpriteInstance texture,
		bool async,
		SpriteInfo input,
		string hashString,
		Vector2B wrapped,
		uint scale
	) {
		if (input.ReferenceData is null) {
			throw new ArgumentNullException(nameof(input.ReferenceData));
		}

		string MakeDumpPath(in Analysis.LegacyResults? analysis = null, PaddingQuad? padding = null, string? subPath = null, string[]? modifiers = null) {
			var normalizedName = input.Reference.NormalizedName().Replace('\\', '.');
			var dumpPath = new StringBuilder();
			dumpPath.Append($"{normalizedName}.{hashString}");
			if (analysis.HasValue) {
				static string SimplifyBools(Vector2B vec) {
					return $"{vec.X.ToInt()}{vec.Y.ToInt()}";
				}

				dumpPath.Append($"-wrap[{SimplifyBools(analysis.Value.Wrapped)}]-repeat[{SimplifyBools(analysis.Value.Repeat.Horizontal)},{SimplifyBools(analysis.Value.Repeat.Vertical)}]");
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
				return NewTextureResult.FromFailure(ResampleStatus.DisabledSolid);
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

		int originalTexels = spriteRawExtent.Area;

		/*
		if (Config.Debug.Sprite.DumpReference) {
			Textures.DumpTexture(
				source: spriteRawData8,
				sourceSize: spriteRawExtent,
				destBounds: spriteRawExtent,
				path: MakeDumpPath(modifiers: new[] { "reference", "reduced" })
			);
		}
		*/

		// At this point, rawData includes just the sprite's raw data.

		var analysis = Analysis.AnalyzeLegacy(
			reference: input.Reference,
			data: spriteRawData8,
			bounds: spriteRawExtent,
			wrapped: input.Wrapped
		);

		bool isGradient = analysis.MaxChannelShades >= Config.Resample.Analysis.MinimumGradientShades && (analysis.GradientDiagonal.Any || analysis.GradientAxial.Any);

		bool fullPremultiplyAlpha = true;
		bool premultiplyAlpha = Config.Resample.PremultiplyAlpha;
		bool gammaCorrect = Config.Resample.AssumeGammaCorrected;

		double opaqueProportion = (double)analysis.OpaqueCount / spriteRawExtent.Area;
		if (!isGradient) {
			if (opaqueProportion <= Config.Resample.Analysis.MinimumPremultipliedOpaqueProportion) {
				fullPremultiplyAlpha = false;
				// TODO : I want to remove this line, but it's causing a single-line artifact in wet HoeDirt that I haven't resolved yet.
				//premultiplyAlpha = false;
				//gammaCorrect = false;
			}
		}
		else {
			if (opaqueProportion >= Config.Resample.Analysis.MaximumGradientOpaqueProportion) {
				isGradient = false;
			}
		}

		if (isGradient) {
			var blacklist = Config.Resample.GradientBlacklistPatterns;
			foreach (var blacklistPattern in blacklist) {
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

		Scaler scalerType;
		switch (input.Reference.Meta().Type) {
			default:
			case Texture2DMeta.SpriteType.Sprite:
				scalerType = isGradient ? input.ScalerGradient : input.Scaler;
				break;
			case Texture2DMeta.SpriteType.Portrait:
				scalerType = input.ScalerPortrait;
				break;
			case Texture2DMeta.SpriteType.LargeText:
			case Texture2DMeta.SpriteType.SmallText:
				scalerType = input.ScalerText;
				break;
		}

		if (
			!Config.Resample.Recolor.Enabled &&
			!Config.Resample.IsEnabled ||
			scalerType == Scaler.None
		) {
			return NewTextureResult.FromFailure(isGradient ? ResampleStatus.DisabledGradient : ResampleStatus.Disabled);
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
				return NewTextureResult.FromFailure(ResampleStatus.DisabledSolid);
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

		// Widen data.
		var spriteRawData = Color16.Convert(spriteRawData8);

		// Apply recolor
		if (Config.Resample.Recolor.Enabled) {
			float rScalar = (float)Config.Resample.Recolor.RScalar;
			float gScalar = (float)Config.Resample.Recolor.GScalar;
			float bScalar = (float)Config.Resample.Recolor.BScalar;

			for (int i = 0; i < spriteRawData.Length; ++i) {
				ref Color16 color = ref spriteRawData[i];
				float r = Math.Clamp(color.R.RealF * rScalar, 0.0f, 1.0f);
				float g = Math.Clamp(color.G.RealF * gScalar, 0.0f, 1.0f);
				float b = Math.Clamp(color.B.RealF * bScalar, 0.0f, 1.0f);
				color.R = Fixed16.FromReal(r);
				color.G = Fixed16.FromReal(g);
				color.B = Fixed16.FromReal(b);
			}
		}

		Span<Color16> bitmapDataWide = spriteRawData;

		var scalerInfo = Scalers.IScaler.GetScalerInfo(scalerType);

		PaddingQuad padding = default;

		if (scalerInfo is not null) {
			scale *= (uint)blockSize;
			scale = Math.Clamp(scale, (uint)scalerInfo.MinScale, (uint)scalerInfo.MaxScale);

			// Adjust the scale value so that it is within the preferred dimensional limits
			if (Config.Resample.Scale) {
				int preferredMaxDimension = Config.PreferredMaxTextureDimension;
				var originalScale = scale;
				scale = 2;
				for (uint s = originalScale; s > 2U; --s) {
					var newDimensions = spriteRawExtent * s;
					if (newDimensions.MaxOf <= preferredMaxDimension) {
						scale = s;
						break;
					}
				}
			}

			premultiplyAlpha = premultiplyAlpha && scalerInfo.PremultiplyAlpha;
			gammaCorrect = gammaCorrect && scalerInfo.GammaCorrect; // There is no reason to perform this pass with EPX, as EPX does not blend.

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

			bitmapDataWide = SpanExt.Make<Color16>(scaledSize.Area);

			try {
				var doWrap = wrapped | input.IsWater;

				if (gammaCorrect && currentGammaState == GammaState.Gamma) {
					GammaCorrection.Linearize(spriteRawData, spriteRawExtent);
					currentGammaState = GammaState.Linear;
				}

				if (premultiplyAlpha) {
					PremultipliedAlpha.Reverse(spriteRawData, spriteRawExtent, fullPremultiplyAlpha);
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
					PremultipliedAlpha.Apply(bitmapDataWide, scaledSize, fullPremultiplyAlpha);
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

		{
			var scaledSizeClamped = scaledSize.Min(Config.ClampDimension);
			if (scaledSize != scaledSizeClamped) {
				if (scaledSize.Width < scaledSizeClamped.Width || scaledSize.Height < scaledSizeClamped.Height) {
					throw new Exception($"Resampled texture size {scaledSize} is smaller than expected {scaledSizeClamped}");
				}

				Debug.Trace($"Sprite {texture.NormalizedName()} requires rescaling");
				// This should be incredibly rare - we very rarely need to scale back down.
				// I don't actually have a solution for this case.
				//scaledSizeClamped = scaledSize;
			}
		}

		TextureFormat format = TextureFormat.Color;

		if (currentGammaState != initialGammaState) {
			throw new Exception("Gamma State Mismatch");
		}

		// Narrow
		var bitmapData = Color8.ConvertPinned(bitmapDataWide);

		/*
		if (Config.Debug.Sprite.DumpResample) {
			Textures.DumpTexture(
				source: bitmapData,
				sourceSize: scaledSize,
				//swap: (2, 1, 0, 4),
				path: MakeDumpPath(analysis: analysis, padding: padding, modifiers: new[] { "resample", "narrowed" })
			);
		}
		*/

		int beforeTexels = scaledSize.Area;

		Vector2I blockPadding = default;

		// TODO : ref optimize
		if (Config.Resample.TrimExcessTransparency) {
			// Detect transparent rows/columns
			(Vector2I Start, Vector2I End) counts = (default, default);

			// Rows

			// From Start
			{
				bool anyTransparent = false;
				for (int y = 0; y < scaledSize.Y; ++y) {
					int offset = scaledSize.X * y;

					bool allTransparent = true;
					for (int x = 0; x < scaledSize.X; ++x) {
						if (bitmapData[offset + x].A != 0) {
							allTransparent = false;
							break;
						}
					}

					if (!allTransparent) {
						counts.Start.Y = y;
						break;
					}

					anyTransparent = true;
				}

				if (anyTransparent && counts.Start.Y == 0) {
					// the entire texture is somehow transparent?
					return NewTextureResult.FromFailure(ResampleStatus.DisabledSolid);
				}
			}

			// From End
			{
				bool anyTransparent = false;
				for (int y = scaledSize.Y - 1; y >= 0; --y) {
					int offset = scaledSize.X * y;

					bool allTransparent = true;
					for (int x = 0; x < scaledSize.X; ++x) {
						if (bitmapData[offset + x].A != 0) {
							allTransparent = false;
							break;
						}
					}

					if (!allTransparent) {
						counts.End.Y = y;
						break;
					}

					anyTransparent = true;
				}

				if (anyTransparent && counts.End.Y == 0) {
					// the entire texture is somehow transparent?
					return NewTextureResult.FromFailure(ResampleStatus.DisabledSolid);
				}
			}

			// Columns

			// From Start
			{
				bool anyTransparent = false;
				for (int x = 0; x < scaledSize.X; ++x) {
					bool allTransparent = true;
					for (int y = 0; y < scaledSize.Y; ++y) {
						if (bitmapData[(y * scaledSize.X) + x].A != 0) {
							allTransparent = false;
							break;
						}
					}

					if (!allTransparent) {
						counts.Start.X = x;
						break;
					}

					anyTransparent = true;
				}

				if (anyTransparent && counts.Start.X == 0) {
					// the entire texture is somehow transparent?
					return NewTextureResult.FromFailure(ResampleStatus.DisabledSolid);
				}
			}

			// From End
			{
				bool anyTransparent = false;
				for (int x = scaledSize.X - 1; x >= 0; --x) {
					bool allTransparent = true;
					for (int y = 0; y < scaledSize.Y; ++y) {
						if (bitmapData[(y * scaledSize.X) + x].A != 0) {
							allTransparent = false;
							break;
						}
					}

					if (!allTransparent) {
						counts.End.X = x;
						break;
					}

					anyTransparent = true;
				}

				if (anyTransparent && counts.End.X == 0) {
					// the entire texture is somehow transparent?
					return NewTextureResult.FromFailure(ResampleStatus.DisabledSolid);
				}
			}

			if (!counts.Start.IsZero || !counts.End.IsZero) {
				if (counts.End.X == 0) {
					counts.End.X = scaledSize.X;
				}
				else /*if ((scaledSize.X - counts.Start.X - (scaledSize.X - counts.End.X)) % 4 != 0)*/ {
					counts.End.X++;
				}
				if (counts.End.Y == 0) {
					counts.End.Y = scaledSize.Y;
				}
				else /*if ((scaledSize.Y - counts.Start.Y - (scaledSize.Y - counts.End.Y)) % 4 != 0)*/ {
					counts.End.Y++;
				}

				/*
				if (counts.Start.X != 0) {
					counts.Start.X--;
				}

				if (counts.Start.Y != 0) {
					counts.Start.Y--;
				}
				*/

				Vector2F xPadding = (counts.Start.X, 0);
				Vector2F yPadding = (counts.Start.Y, 0);

				padding.X -= xPadding;
				padding.Y -= yPadding;

				//blockPadding.X = -(scaledSize.X - counts.End.X);
				//blockPadding.Y = -(scaledSize.Y - counts.End.Y);

				padding.X.Y += -(scaledSize.X - counts.End.X);
				padding.Y.Y += -(scaledSize.Y - counts.End.Y);

				if (counts.Start.X == 0 && counts.End.X == scaledSize.X) {
					// If we're only reducing it in height, that makes this far simpler.
					int offset = counts.Start.Y * scaledSize.X;
					int extent = (counts.End.Y - counts.Start.Y) * scaledSize.X;
					bitmapData.Slice(offset, extent).CopyTo(bitmapData.Slice(0, extent));
					bitmapData = bitmapData.Slice(0, extent);
					scaledSize -= (0, counts.Start.Y + (scaledSize.Y - counts.End.Y));
				}
				else {
					// Source and target technically overlap, but there's no contention because we never read somewhere we wrote to. We are always at the same point or ahead.
					int targetWidth = counts.End.X - counts.Start.X;
					int targetHeight = counts.End.Y - counts.Start.Y;

					int sourceStride = scaledSize.X;
					int targetStride = targetWidth;

					int sourceOffset = (counts.Start.Y * sourceStride) + counts.Start.X;
					int targetOffset = 0;

					for (int y = 0; y < targetHeight; ++y) {
						var sourceSlice = bitmapData.Slice(sourceOffset, targetWidth);
						var targetSlice = bitmapData.Slice(targetOffset, targetWidth);
						sourceSlice.CopyTo(targetSlice);
						sourceOffset += sourceStride;
						targetOffset += targetStride;
					}

					bitmapData = bitmapData.Slice(0, targetWidth * targetHeight);
					scaledSize -= (counts.Start.X + (scaledSize.X - counts.End.X), counts.Start.Y + (scaledSize.Y - counts.End.Y));
				}
			}
		}

		int afterTexels = scaledSize.Area;

		var resultData = bitmapData.AsBytes();

		// TODO : For some reason, block compression gives incorrect results with EPX. I need to investigate this at some point.
		if (Config.Resample.BlockCompression.Enabled && scaledSize.MinOf >= 4 && (scalerInfo?.BlockCompress ?? true)) {
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

				int* alpha = stackalloc int[maxShades];
				Unsafe.InitBlockUnaligned(alpha, 0, sizeof(int) * maxShades);
				int* blue = stackalloc int[maxShades];
				Unsafe.InitBlockUnaligned(blue, 0, sizeof(int) * maxShades);
				int* green = stackalloc int[maxShades];
				Unsafe.InitBlockUnaligned(green, 0, sizeof(int) * maxShades);
				int* red = stackalloc int[maxShades];
				Unsafe.InitBlockUnaligned(red, 0, sizeof(int) * maxShades);

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
			var originalScaledSizeClamped = scaledSize;
			Vector2I originalBlockPadding = blockPadding;

			if (!Decoder.BlockDecoderCommon.IsBlockMultiple(scaledSize)) {
				var blockPaddedSize = scaledSize + 3 & ~3;

				var spanDst = SpanExt.Make<Color8>(blockPaddedSize.Area);
				var spanSrc = bitmapData;

				int y;
				// Copy data
				for (y = 0; y < scaledSize.Y; ++y) {
					var newBufferOffset = y * blockPaddedSize.X;
					var bitmapOffset = y * scaledSize.X;

					int rowSize = scaledSize.X;

					spanSrc.Slice(bitmapOffset, rowSize).CopyTo(
						spanDst.Slice(newBufferOffset, rowSize)
					);

					// Extend X across
					spanDst.Slice(newBufferOffset + rowSize, blockPaddedSize.X - rowSize).Fill(
						spanSrc[bitmapOffset + rowSize - 1]
					);
				}
				// Extend existing data
				var lastY = y - 1;
				var sourceOffset = lastY * blockPaddedSize.X;
				for (; y < blockPaddedSize.Y; ++y) {
					int newBufferOffset = y * blockPaddedSize.X;

					int padSize = blockPaddedSize.X;

					spanDst.Slice(sourceOffset, padSize).CopyTo(
						spanDst.Slice(newBufferOffset, padSize)
					);
				}

				uncompressedBitmapData = spanDst;
				blockPadding += blockPaddedSize - scaledSize;
				scaledSize = blockPaddedSize;

				/*
				if (Config.Debug.Sprite.DumpResample) {
					Textures.DumpTexture(
						source: uncompressedBitmapData,
						sourceSize: blockPaddedSize,
						// swap: (2, 1, 0, 4),
						path: MakeDumpPath(analysis: analysis, padding: padding, modifiers: new[] { "resample", "blockpadded" })
					);
				}
				*/
			}

			if (!TextureEncode.Encode(
					data: uncompressedBitmapData,
					format: ref format,
					dimensions: scaledSize,
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
				scaledSize = originalScaledSizeClamped;
			}
		}

		int afterAfterTexels = scaledSize.Area;
		int compressedSize = resultData.Length;

		return new() {
			Status = ResampleStatus.Success,
			Data = resultData,
			Wrapped = wrapped,
			Scale = scale,
			Size = scaledSize,
			Format = format,
			Padding = padding,
			BlockPadding = blockPadding,
			ScalerInfo = scalerInfo,
			IsGradient = isGradient
		};
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

					Debug.Trace($"Skipping resample of {spriteInstance.Name} {input.Bounds}: {result}");
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
					var newTextureResult = CreateNewTexture(
						texture: spriteInstance,
						async: async,
						input: input,
						hashString: hashString,
						wrapped: wrapped,
						scale: scale
					);
					bitmapData = pinnedBitmapData = newTextureResult.Data;
					wrapped = newTextureResult.Wrapped;
					scale = newTextureResult.Scale;
					newSize = newTextureResult.Size;
					spriteFormat = newTextureResult.Format;
					spriteInstance.Padding = newTextureResult.Padding;
					spriteInstance.BlockPadding = newTextureResult.BlockPadding;
					spriteInstance.ScalerInfo = newTextureResult.ScalerInfo;
					isGradient = newTextureResult.IsGradient;
					result = newTextureResult.Status;

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

			static ManagedTexture2D? CreateTexture(SpriteInfo input, ManagedSpriteInstance spriteInstance, Vector2I newSize, TextureFormat spriteFormat, ReadOnlyPinnedSpan<byte>.FixedSpan data) {
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

				static void SyncCallFixed(
					SpriteInfo input,
					Vector2I newSize,
					TextureFormat spriteFormat,
					Texture2D reference,
					ManagedSpriteInstance spriteInstance,
					ReadOnlyPinnedSpan<byte>.FixedSpan data
				) {
					if (reference.IsDisposed) {
						return;
					}
					if (spriteInstance.IsDisposed) {
						return;
					}
					ManagedTexture2D? newTexture = null;
					try {
						newTexture = CreateTexture(input, spriteInstance, newSize, spriteFormat, data);
						spriteInstance.Texture = newTexture;
						spriteInstance.Finish();
					}
					catch (Exception ex) {
						ex.PrintError();
						newTexture?.Dispose();
						spriteInstance.Dispose();
					}
				}

				static void SyncCallArray(
					SpriteInfo input,
					Vector2I newSize,
					TextureFormat spriteFormat,
					Texture2D reference,
					ManagedSpriteInstance spriteInstance,
					byte[] array
				) {
					if (reference.IsDisposed) {
						return;
					}
					if (spriteInstance.IsDisposed) {
						return;
					}

					unsafe {
						fixed (byte* ptr = array) {
							var pinnedSpan = new ReadOnlyPinnedSpan<byte>(array, ptr, array.Length);
							SyncCallFixed(input, newSize, spriteFormat, reference, spriteInstance, pinnedSpan.Fixed);
						}
					}
				}

				if (bitmapDataArray is null) {
					var fixedData = pinnedBitmapData.Fixed;
					spriteInstance.DeferredTask.SetTarget(SynchronizedTaskScheduler.Instance.QueueDeferred(
						() => SyncCallFixed(input, newSize, spriteFormat, reference, spriteInstance, fixedData),
						new(input.Reference.NormalizedName(), bitmapData.Length, new(reference), input.Bounds),
						priority: input.IsAnimated ? SynchronizedTaskScheduler.Priority.High : SynchronizedTaskScheduler.Priority.Normal
					)!);
				}
				else {
					spriteInstance.DeferredTask.SetTarget(SynchronizedTaskScheduler.Instance.QueueDeferred(
						() => SyncCallArray(input, newSize, spriteFormat, reference, spriteInstance, bitmapDataArray),
						new(input.Reference.NormalizedName(), bitmapData.Length, new(reference), input.Bounds),
						priority: input.IsAnimated ? SynchronizedTaskScheduler.Priority.High : SynchronizedTaskScheduler.Priority.Normal
					)!);
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
								newTexture = CreateTexture(input, spriteInstance, newSize, spriteFormat, pinnedSpan.Fixed);
							}
						}
					}
					else {
						newTexture = CreateTexture(input, spriteInstance, newSize, spriteFormat, pinnedBitmapData.Fixed);
					}

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
