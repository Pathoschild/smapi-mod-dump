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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Caching;
using SpriteMaster.Extensions;
using SpriteMaster.Metadata;
using SpriteMaster.Tasking;
using SpriteMaster.Types;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample;

sealed class Resampler {
	internal enum Scaler : int {
		xBRZ = 0,
		Bilinear,
		Bicubic,
		ImageMagick
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void PurgeHash(Texture2D reference) {
		reference.Meta().CachedRawData = null;
	}

	// https://stackoverflow.com/a/12996028
	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static ulong HashULong(ulong x) {
		if (x == 0) {
			x = 0x9e3779b97f4a7c15UL; // ⌊2^64 / Φ⌋
		}
		x = (x ^ x >> 30) * 0xbf58476d1ce4e5b9UL;
		x = (x ^ x >> 27) * 0x94d049bb133111ebUL;
		x ^= x >> 31;
		return x;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong GetHash(SpriteInfo input, TextureType textureType) {
		// Need to make Hashing.CombineHash work better.
		ulong hash = input.Hash;

		if (Config.Resample.EnableDynamicScale) {
			hash = Hashing.Combine(hash, HashULong(input.ExpectedScale));
		}

		if (textureType == TextureType.Sprite) {
			hash = Hashing.Combine(hash, input.Bounds.Extent.GetLongHashCode());
		}
		return hash;
	}

	private static readonly WeakSet<Texture2D> GarbageMarkSet = Config.Garbage.CollectAccountUnownedTextures ? new() : null!;

	private const int WaterBlock = 4;
	private const int FontBlock = 1;

	// TODO : use MemoryFailPoint class. Extensively.

	private enum GammaState {
		Linear,
		Gamma,
		Unknown
	}

	private static unsafe Span<byte> CreateNewTexture(
		ManagedSpriteInstance texture,
		bool async,
		SpriteInfo input,
		string hashString,
		ref Vector2B wrapped,
		ref uint scale,
		out Vector2I size,
		out TextureFormat format,
		out PaddingQuad padding,
		out Vector2I blockPadding
	) {
		if (input.ReferenceData is null) {
			throw new ArgumentNullException(nameof(input.ReferenceData));
		}

		padding = PaddingQuad.Zero;
		blockPadding = Vector2I.Zero;

		if (Config.Debug.Sprite.DumpReference) {
			Textures.DumpTexture(
				source: input.ReferenceData,
				sourceSize: input.ReferenceSize,
				destBounds: input.Bounds,
				path: FileCache.GetDumpPath($"{input.Reference.SafeName().Replace('/', '.')}.{hashString}.reference.png")
			);
		}

		var initialGammaState = GammaState.Gamma;
		var currentGammaState = initialGammaState;

		Bounds inputBounds;
		switch (input.TextureType) {
			case TextureType.Sprite:
				inputBounds = input.Bounds;
				break;
			case TextureType.Image:
				inputBounds = input.ReferenceSize;
				break;
			case TextureType.SlicedImage:
				throw new NotImplementedException("Sliced Images not yet implemented");
			default:
				throw new NotImplementedException("Unknown Texture Type provided");
		}

		if (input.Reference.Format.IsCompressed()) {
			throw new InvalidOperationException($"Compressed texture '{input.Reference.SafeName()}' reached Resampler");
		}

		// Water in the game is pre-upscaled by 4... which is weird.
		int blockSize = 1;
		if ((input.IsWater || input.Reference == StardewValley.Game1.rainTexture) && WaterBlock != 1) {
			blockSize = WaterBlock;
		}
		else if (input.IsFont && FontBlock != 1) {
			blockSize = FontBlock;
			scale = Config.Resample.MaxScale;
		}
		// TODO : handle inverted input.Bounds
		var spriteRawData8 = Passes.ExtractSprite.Extract(
			data: input.ReferenceData.AsSpan<Color8>(),
			textureBounds: input.Reference.Bounds,
			spriteBounds: inputBounds,
			stride: input.ReferenceSize.Width,
			block: blockSize,
			newExtent: out Vector2I spriteRawExtent
		);
		var innerSpriteRawExtent = spriteRawExtent;

		// At this point, rawData includes just the sprite's raw data.

		// Adjust the scale value so that it is within the preferred dimensional limits
		if (Config.Resample.Scale) {
			var originalScale = scale;
			scale = 2;
			foreach (uint s in originalScale.RangeTo(2U)) {
				var newDimensions = spriteRawExtent * s;
				if (newDimensions.X <= Config.PreferredMaxTextureDimension && newDimensions.Y <= Config.PreferredMaxTextureDimension) {
					scale = s;
					break;
				}
			}
		}

		var scaledSize = spriteRawExtent * scale;
		var scaledSizeClamped = scaledSize.Min(Config.ClampDimension);

		var analysis = Passes.Analysis.AnalyzeLegacy(
			reference: input.Reference,
			data: spriteRawData8,
			bounds: spriteRawExtent,
			Wrapped: input.Wrapped
		);

		if (input.Reference.SafeName().StartsWith("Buildings/houses")) {
			input = input;
		}

		if (Config.Resample.EnableWrappedAddressing) {
			wrapped = analysis.Wrapped;
		}
		else {
			wrapped = (false, false);
		}

		// Widen data.
		var spriteRawData = Color16.Convert(spriteRawData8);

		Span<Color16> bitmapDataWide = spriteRawData;

		if (Config.Resample.Enabled) {
			// Apply padding to the sprite if necessary
			spriteRawData = Passes.Padding.Apply(
				data: spriteRawData,
				spriteSize: spriteRawExtent,
				scale: scale,
				input: input,
				analysis: analysis,
				padding: out padding,
				paddedSize: out spriteRawExtent
			);

			scaledSize = spriteRawExtent * scale;
			scaledSizeClamped = scaledSize.Min(Config.ClampDimension);

			bitmapDataWide = SpanExt.MakePinned<Color16>(scaledSize.Area);

			try {
				var doWrap = wrapped | input.IsWater;

				if (Config.Resample.AssumeGammaCorrected && currentGammaState == GammaState.Gamma) {
					Passes.GammaCorrection.Linearize(spriteRawData, spriteRawExtent);
					currentGammaState = GammaState.Linear;
				}

				if (!input.IsWater && (Config.Resample.PremultiplyAlpha && analysis.PremultipliedAlpha)) {
					Passes.PremultipliedAlpha.Reverse(spriteRawData, spriteRawExtent);
				}

				if (Config.Resample.Deposterization.PreEnabled) {
					spriteRawData = Deposterize.Enhance<Color16>(spriteRawData, spriteRawExtent, doWrap);

					if (Config.Debug.Sprite.DumpReference) {
						Textures.DumpTexture(
							source: spriteRawData,
							sourceSize: spriteRawExtent,
							adjustGamma: 2.2,
							path: FileCache.GetDumpPath($"{input.Reference.SafeName().Replace('/', '.')}.{hashString}.reference.deposter.png")
						);
					}
				}

				switch (Config.Resample.Scaler) {
					case Scaler.xBRZ: {
							var scalerConfig = Resample.Scalers.IScaler.Current.CreateConfig(
								wrapped: doWrap,
								hasAlpha: true
							);

							bitmapDataWide = Resample.Scalers.IScaler.Current.Apply(
								configuration: scalerConfig,
								scaleMultiplier: scale,
								sourceData: spriteRawData,
								sourceSize: spriteRawExtent,
								targetSize: scaledSize,
								targetData: bitmapDataWide
							);
						}
						break;
					case Scaler.ImageMagick: {
							throw new NotImplementedException("ImageMagick Scaling is not implemented");
						}
						break;
					case Scaler.Bilinear:
					case Scaler.Bicubic: {
							throw new NotImplementedException("Bilinear and Bicubic scaling are not implemented");
						}
						break;
					default:
						throw new InvalidOperationException($"Unknown Scaler Type: {Config.Resample.Scaler}");
				}

				if (Config.Resample.Deposterization.PostEnabled) {
					bitmapDataWide = Deposterize.Enhance<Color16>(bitmapDataWide, scaledSize, doWrap);
				}

				if (Config.Resample.UseColorEnhancement) {
					bitmapDataWide = Recolor.Enhance<Color16>(bitmapDataWide, scaledSize);
				}

				if (!input.IsWater && (Config.Resample.PremultiplyAlpha && analysis.PremultipliedAlpha)) {
					Passes.PremultipliedAlpha.Apply(bitmapDataWide, scaledSize);
				}

				if (Config.Resample.AssumeGammaCorrected && currentGammaState == GammaState.Linear) {
					Passes.GammaCorrection.Delinearize(bitmapDataWide, scaledSize);
					currentGammaState = GammaState.Gamma;
				}
			}
			catch (Exception ex) {
				ex.PrintError();
				throw;
			}
			//ColorSpace.ConvertLinearToSRGB(bitmapData, Texel.Ordering.ARGB);
		}

		if (Config.Debug.Sprite.DumpResample) {
			static string SimplifyBools(in Vector2B vec) {
				return $"{vec.X.ToInt()}{vec.Y.ToInt()}";
			}

			Textures.DumpTexture(
				source: bitmapDataWide,
				sourceSize: scaledSize,
				swap: (2, 1, 0, 4),
				path: FileCache.GetDumpPath($"{input.Reference.SafeName().Replace('/', '.')}.{hashString}.resample-wrap[{SimplifyBools(analysis.Wrapped)}]-repeat[{SimplifyBools(analysis.RepeatX)},{SimplifyBools(analysis.RepeatY)}]-pad[{padding.X},{padding.Y}].png")
			);
		}

		if (scaledSize != scaledSizeClamped) {
			if (scaledSize.Width < scaledSizeClamped.Width || scaledSize.Height < scaledSizeClamped.Height) {
				throw new Exception($"Resampled texture size {scaledSize} is smaller than expected {scaledSizeClamped}");
			}

			Debug.TraceLn($"Sprite {texture.SafeName()} requires rescaling");
			// This should be incredibly rare - we very rarely need to scale back down.
			// I don't actually have a solution for this case.
			scaledSizeClamped = scaledSize;
		}

		format = TextureFormat.Color;

		if (currentGammaState != initialGammaState) {
			throw new Exception("Gamma State Mismatch");
		}

		// Narrow
		var bitmapData = Color8.Convert(bitmapDataWide);
		var resultData = bitmapData.AsBytes();

		// We don't want to use block compression if asynchronous loads are enabled but this is not an asynchronous load... unless that is explicitly enabled.
		if (Config.Resample.BlockCompression.Enabled /*&& (Config.Resample.BlockCompression.Synchronized || !Config.AsyncScaling.Enabled || async)*/ && scaledSizeClamped.MinOf >= 4) {
			// TODO : We can technically allocate the block padding before the scaling phase, and pass it a stride
			// so it will just ignore the padding areas. That would be more efficient than this.

			// Check for special cases
			bool HasAlpha = true;
			bool IsPunchThroughAlpha = false;
			bool IsMasky = false;
			bool hasR = true;
			bool hasG = true;
			bool hasB = true;
			{
				const int MaxShades = 256;

				Span<int> alpha = stackalloc int[MaxShades];
				Span<int> blue = stackalloc int[MaxShades];
				Span<int> green = stackalloc int[MaxShades];
				Span<int> red = stackalloc int[MaxShades];
				for (int i = 0; i < MaxShades; ++i) {
					alpha[i] = 0;
					blue[i] = 0;
					green[i] = 0;
					red[i] = 0;
				}

				foreach (var color in bitmapData) {
					alpha[color.A.Value]++;
					blue[color.B.Value]++;
					green[color.G.Value]++;
					red[color.R.Value]++;
				}

				hasR = red[0] != bitmapData.Length;
				hasG = green[0] != bitmapData.Length;
				hasB = blue[0] != bitmapData.Length;

				//Debug.WarningLn($"Punch-through Alpha: {intData.Length}");
				IsPunchThroughAlpha = IsMasky = alpha[0] + alpha[MaxShades - 1] == bitmapData.Length;
				HasAlpha = alpha[MaxShades - 1] != bitmapData.Length;

				if (HasAlpha && !IsPunchThroughAlpha) {
					var alphaDeviation = Statistics.StandardDeviation(alpha, MaxShades, 1, MaxShades - 2);
					IsMasky = alphaDeviation < Config.Resample.BlockCompression.HardAlphaDeviationThreshold;
				}
			}

			if (!Decoder.BlockDecoderCommon.IsBlockMultiple(scaledSizeClamped)) {
				var blockPaddedSize = scaledSizeClamped + 3 & ~3;

				var spanDst = SpanExt.MakeUninitialized<Color8>(blockPaddedSize.Area);
				var spanSrc = bitmapData;

				int y;
				for (y = 0; y < scaledSizeClamped.Y; ++y) {
					var newBufferOffset = y * blockPaddedSize.X;
					var bitmapOffset = y * scaledSizeClamped.X;
					int x;
					for (x = 0; x < scaledSizeClamped.X; ++x) {
						spanDst[newBufferOffset + x] = spanSrc[bitmapOffset + x];
					}
					int lastX = x - 1;
					for (; x < blockPaddedSize.X; ++x) {
						spanDst[newBufferOffset + x] = spanSrc[bitmapOffset + lastX];
					}
				}
				var lastY = y - 1;
				var sourceOffset = lastY * scaledSizeClamped.X;
				for (; y < blockPaddedSize.Y; ++y) {
					int newBufferOffset = y * blockPaddedSize.X;
					for (int x = 0; x < blockPaddedSize.X; ++x) {
						spanDst[newBufferOffset + x] = spanDst[sourceOffset + x];
					}
				}

				bitmapData = spanDst;
				blockPadding += blockPaddedSize - scaledSizeClamped;
				scaledSizeClamped = blockPaddedSize;
			}

			resultData = TextureEncode.Encode(
				data: bitmapData,
				format: ref format,
				dimensions: scaledSizeClamped,
				hasAlpha: HasAlpha,
				isPunchthroughAlpha: IsPunchThroughAlpha,
				isMasky: IsMasky,
				hasR: hasR,
				hasG: hasG,
				hasB: hasB
			);
		}

		size = scaledSizeClamped;
		return resultData;
	}

	internal static ManagedTexture2D? Upscale(ManagedSpriteInstance spriteInstance, ref uint scale, SpriteInfo input, ulong hash, ref Vector2B wrapped, bool async) {
		try {
			// Try to process the texture twice. Garbage collect after a failure, maybe it'll work then.
			for (int i = 0; i < 2; ++i) {
				try {
					return UpscaleInternal(
						spriteInstance: spriteInstance,
						scale: ref scale,
						input: input,
						hash: hash,
						wrapped: ref wrapped,
						async: async
					);
				}
				catch (OutOfMemoryException) {
					Debug.WarningLn("OutOfMemoryException encountered during Upscale, garbage collecting and deferring.");
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

	internal static readonly Action<Texture2D, int, byte[], int, int>? PlatformSetData = typeof(Texture2D).GetMethods(
		BindingFlags.Instance | BindingFlags.NonPublic
	).SingleF(m => m.Name == "PlatformSetData" && m.GetParameters().Length == 4)?.MakeGenericMethod(new Type[] { typeof(byte) })?.CreateDelegate<Action<Texture2D, int, byte[], int, int>>();

	private static ManagedTexture2D? UpscaleInternal(ManagedSpriteInstance spriteInstance, ref uint scale, SpriteInfo input, ulong hash, ref Vector2B wrapped, bool async) {
		var spriteFormat = TextureFormat.Color;

		if (Config.Garbage.CollectAccountUnownedTextures && GarbageMarkSet.Add(input.Reference)) {
			Garbage.Mark(input.Reference);
			input.Reference.Disposing += (obj, _) => {
				Garbage.Unmark((Texture2D)obj!);
			};
		}

		var hashString = hash.ToString("x");
		var cachePath = FileCache.GetPath($"{hashString}.cache");

		var inputSize = input.TextureType switch {
			TextureType.Sprite => input.Bounds.Extent,
			TextureType.Image => input.ReferenceSize,
			TextureType.SlicedImage => throw new NotImplementedException("Sliced Images not yet implemented"),
			_ => throw new NotImplementedException("Unknown Image Type provided")
		};

		Span<byte> bitmapData;
		try {
			var newSize = Vector2I.Zero;

			try {
				if (FileCache.Fetch(
					path: cachePath,
					refScale: out var fetchScale,
					size: out newSize,
					format: out spriteFormat,
					wrapped: out wrapped,
					padding: out spriteInstance.Padding,
					blockPadding: out spriteInstance.BlockPadding,
					data: out bitmapData
				)) {
					scale = fetchScale;
				}
				else {
					bitmapData = null;
				}
			}
			catch (Exception ex) {
				ex.PrintWarning();
				bitmapData = null;
			}

			if (bitmapData == null) {
				try {
					bitmapData = CreateNewTexture(
						async: async,
						texture: spriteInstance,
						input: input,
						hashString: hashString,
						wrapped: ref wrapped,
						scale: ref scale,
						size: out newSize,
						format: out spriteFormat,
						padding: out spriteInstance.Padding,
						blockPadding: out spriteInstance.BlockPadding
					);
				}
				catch (OutOfMemoryException) {
					Debug.Error($"OutOfMemoryException thrown trying to create texture [texture: {spriteInstance.SafeName()}, bounds: {input.Bounds}, textureSize: {input.ReferenceSize}, scale: {scale}]");
					throw;
				}

				try {
					FileCache.Save(cachePath, scale, newSize, spriteFormat, wrapped, spriteInstance.Padding, spriteInstance.BlockPadding, bitmapData);
				}
				catch { }
			}

			spriteInstance.UnpaddedSize = newSize - (spriteInstance.Padding.Sum + spriteInstance.BlockPadding);
			spriteInstance.InnerRatio = (Vector2F)newSize / (Vector2F)spriteInstance.UnpaddedSize;

			ManagedTexture2D? CreateTexture(byte[] data) {
				if (input.Reference.GraphicsDevice.IsDisposed) {
					return null;
				}
				//var newTexture2 = GL.GLTexture.CreateTexture2D(newSize, false, spriteFormat);
				var newTexture = new ManagedTexture2D(
					texture: spriteInstance,
					reference: input.Reference,
					dimensions: newSize,
					format: spriteFormat
				);
				if (PlatformSetData is not null) {
					PlatformSetData(newTexture, 0, data, 0, data.Length);
				}
				else {
					newTexture.SetData(data);
				}

				return newTexture;
			}

			var isAsync = Config.AsyncScaling.Enabled && async;
			if (!isAsync || Config.AsyncScaling.ForceSynchronousStores) {
				var reference = input.Reference;
				var bitmapDataArray = bitmapData.ToArray();
				void syncCall() {
					if (reference.IsDisposed) {
						return;
					}
					if (spriteInstance.IsDisposed) {
						return;
					}
					ManagedTexture2D? newTexture = null;
					try {
						newTexture = CreateTexture(bitmapDataArray);
						spriteInstance.Texture = newTexture;
						spriteInstance.Finish();
					}
					catch (Exception ex) {
						ex.PrintError();
						if (newTexture != null) {
							newTexture.Dispose();
						}
						spriteInstance.Dispose();
					}
				}
				SynchronizedTaskScheduler.Instance.QueueDeferred(syncCall, new(bitmapData.Length));
				return null;
			}
			else {
				ManagedTexture2D? newTexture = null;
				try {
					newTexture = CreateTexture(bitmapData.ToArray());
					if (isAsync) {
						spriteInstance.Texture = newTexture;
						spriteInstance.Finish();
					}
					return newTexture;
				}
				catch (Exception ex) {
					ex.PrintError();
					if (newTexture != null) {
						newTexture.Dispose();
					}
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
