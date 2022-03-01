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
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Harmonize.Patches.PSpriteBatch;

[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Harmony")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony")]
static class Draw {
	private const bool Continue = true;
	private const bool Stop = false;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static bool GetIsSliced(in Bounds bounds, Texture2D reference, [NotNullWhen(true)] out Config.TextureRef? result) {
		foreach (var slicedTexture in Config.Resample.SlicedTexturesS) {
			if (!reference.NormalizedName().StartsWith(slicedTexture.Texture)) {
				continue;
			}
			if (slicedTexture.Bounds.IsEmpty) {
				result = slicedTexture;
				return true;
			}
			if (slicedTexture.Bounds.Contains(bounds)) {
				result = slicedTexture;
				return true;
			}
		}
		result = null;
		return false;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static bool Cleanup(this ref Bounds sourceBounds, Texture2D reference) {
		if (Config.ClampInvalidBounds && !sourceBounds.ClampToChecked(reference.Extent(), out var clampedBounds)) {
			//Debug.Warning($"Draw.Cleanup: '{reference.SafeName()}' bounds '{sourceBounds}' are not contained in reference bounds '{(Bounds)reference.Bounds}' - clamped ({(sourceBounds.Degenerate ? "degenerate" : "")})");
			sourceBounds = clampedBounds;
		}

		// Let's just skip potentially invalid draws since I have no idea what to do with them.
		return !sourceBounds.Degenerate;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static bool FetchScaledTexture(
		this Texture2D reference,
		uint expectedScale,
		ref Bounds source,
		[NotNullWhen(true)] out ManagedSpriteInstance? spriteInstance,
		bool create = false
	) {
		var invert = source.Invert;
		spriteInstance = reference.FetchScaledTexture(
			expectedScale: expectedScale,
			source: ref source,
			create: create
		);
		source.Invert = invert;
		return spriteInstance is not null;
	}

	private static ManagedSpriteInstance? FetchScaledTexture(
		this Texture2D reference,
		uint expectedScale,
		ref Bounds source,
		bool create = false
	) {
		var clampedSource = source;

		try {
			if (reference is InternalTexture2D) {
				return null;
			}

			// If the (potentially-clamped) source bounds are invalid, return null
			if (!clampedSource.Cleanup(reference)) {
				return null;
			}

			// If the reference texture's dimensions are invalid, return null
			if (reference.Width < 1 || reference.Height < 1) {
				return null;
			}

			// If the reference texture is too small to consider resampling, return null
			if (reference.Extent().MaxOf <= Config.Resample.MinimumTextureDimensions) {
				return null;
			}

			bool isSliced = false;
			if (GetIsSliced(clampedSource, reference, out var textureRef)) {
				clampedSource = textureRef.Value.Bounds;
				isSliced = true;
			}

			var spriteInstance = create ?
				ManagedSpriteInstance.FetchOrCreate(texture: reference, source: clampedSource, expectedScale: expectedScale, sliced: isSliced) :
				ManagedSpriteInstance.Fetch(texture: reference, source: clampedSource, expectedScale: expectedScale);

			if (spriteInstance is null || !spriteInstance.IsReady) {
				return null;
			}

			var t = spriteInstance.Texture!;

			if (!t.Validate()) {
				return null;
			}

			source = t.Dimensions;

			return spriteInstance;
		}
		catch (Exception ex) {
			ex.PrintError();
		}

		return null;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static bool Validate(this ManagedTexture2D texture) => !texture.IsDisposed;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static void GetDrawParameters(Texture2D texture, in XNA.Rectangle? source, out Bounds bounds, out float scaleFactor) {
		if (texture is not InternalTexture2D) {
			texture.Meta().UpdateLastAccess();
		}

		var sourceRectangle = (Bounds)source.GetValueOrDefault(new(0, 0, texture.Width, texture.Height));

		if (Config.Resample.TrimWater && SpriteOverrides.IsWater(sourceRectangle, texture)) {
			scaleFactor = 4.0f;
		}
		else {
			scaleFactor = 1.0f;
		}

		ReportOnceValidations.Validate(sourceRectangle, texture);
		bounds = sourceRectangle;
	}

	// Takes the arguments, and checks to see if the texture is padded. If it is, it is forwarded to the correct draw call, avoiding
	// intervening mods altering the arguments first.
	internal static bool OnDrawFirst(
		this SpriteBatch @this,
		ref Texture2D texture,
		ref XNA.Rectangle destination,
		ref XNA.Rectangle? source,
		XNA.Color color,
		float rotation,
		ref XNA.Vector2 origin,
		ref SpriteEffects effects,
		float layerDepth,
		ref ManagedTexture2D __state
	) {
		using var watchdogScoped = WatchDog.WatchDog.ScopedWorkingState;

		/*
		if (destination.Width < 0 || destination.Height < 0) {
			Debug.Trace("destination invert");
		}
		if (source is XNA.Rectangle sourceRect && (sourceRect.Width < 0 || sourceRect.Height < 0)) {
			Debug.Trace("source invert");
		}
		*/

		GetDrawParameters(
			texture: texture,
			source: source,
			bounds: out var sourceRectangle,
			scaleFactor: out var scaleFactor
		);

		var referenceRectangle = sourceRectangle;

		Bounds destinationBounds = destination;

		var expectedScale2D = destinationBounds.ExtentF / sourceRectangle.ExtentF;
		var expectedScale = EstimateScale(expectedScale2D, scaleFactor);

		if (!texture.FetchScaledTexture(
			expectedScale: expectedScale,
			source: ref sourceRectangle,
			spriteInstance: out var spriteInstance,
			create: true
		)) {
			return Continue;
		}
		spriteInstance.UpdateReferenceFrame();

		if (referenceRectangle.X < 0) {
			destinationBounds.Left -= referenceRectangle.X;
		}
		if (referenceRectangle.Y < 0) {
			destinationBounds.Top -= referenceRectangle.Y;
		}

		var resampledTexture = spriteInstance.Texture!;

		if (!spriteInstance.Padding.IsZero) {
			// Convert the draw into the other draw style. This has to be done because the padding potentially has
			// subpixel accuracy when scaled to the destination rectangle.

			var originalSize = referenceRectangle.ExtentF;
			var destinationSize = destinationBounds.ExtentF;
			var newScale = destinationSize / originalSize;
			var newPosition = destinationBounds.OffsetF;

			if ((destinationBounds.Invert.X || destinationBounds.Invert.Y) && DrawState.CurrentRasterizerState.CullMode == CullMode.CullCounterClockwiseFace) {
				// Winding order is invalid
				return Stop;
			}
			if (destinationBounds.Invert.X) {
				effects ^= SpriteEffects.FlipHorizontally;
			}
			if (destinationBounds.Invert.Y) {
				effects ^= SpriteEffects.FlipVertically;
			}

			// TODO handle culling here for inverted sprites?

			@this.Draw(
				texture: resampledTexture,
				position: newPosition,
				sourceRectangle: sourceRectangle,
				color: color,
				rotation: rotation,
				origin: origin,
				scale: newScale,
				effects: effects,
				layerDepth: layerDepth
			);
			return Stop;
		}
		__state = resampledTexture;
		return Continue;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool OnDraw(
		this SpriteBatch @this,
		ref Texture2D texture,
		ref XNA.Rectangle destination,
		ref XNA.Rectangle? source,
		XNA.Color color,
		float rotation,
		ref XNA.Vector2 origin,
		ref SpriteEffects effects,
		ref float layerDepth,
		ref ManagedTexture2D __state
	) {
		Bounds sourceRectangle;
		ManagedSpriteInstance? spriteInstance;
		ManagedTexture2D resampledTexture;

		Bounds destinationBounds = destination;

		var referenceSource = source.GetValueOrDefault();

		if (__state is null) {
			GetDrawParameters(
				texture: texture,
				source: source,
				bounds: out sourceRectangle,
				scaleFactor: out var scaleFactor
			);

			var expectedScale2D = new Vector2F(destinationBounds.Extent) / new Vector2F(sourceRectangle.Extent);
			var expectedScale = EstimateScale(expectedScale2D, scaleFactor);

			if (!texture.FetchScaledTexture(
				expectedScale: expectedScale,
				source: ref sourceRectangle,
				spriteInstance: out spriteInstance
			)) {
				return Continue;
			}

			if (spriteInstance.TexType == TextureType.SlicedImage) {
				sourceRectangle = source ?? spriteInstance.Texture!.Bounds;
			}

			spriteInstance.UpdateReferenceFrame();

			resampledTexture = spriteInstance.Texture!;
		}
		else {
			resampledTexture = __state;
			spriteInstance = resampledTexture.Texture;
			sourceRectangle = resampledTexture.Dimensions;
			if (spriteInstance.TexType == TextureType.SlicedImage) {
				sourceRectangle = source ?? resampledTexture.Bounds;
				if (source.HasValue) {
					sourceRectangle = new Bounds(
						(Vector2I)source.Value.Location - spriteInstance.OriginalSourceRectangle.Offset,
						source.Value.Size
					);
					sourceRectangle.Offset = (sourceRectangle.OffsetF * spriteInstance.Scale).NearestInt();
					sourceRectangle.Extent = (sourceRectangle.ExtentF * spriteInstance.Scale).NearestInt();
				}
			}
		}

		if (referenceSource.X < 0) {
			destination.X -= referenceSource.X;
		}
		if (referenceSource.Y < 0) {
			destination.Y -= referenceSource.Y;
		}

		var scaledOrigin = (Vector2F)origin / spriteInstance.Scale;

		if (source.HasValue) {
			sourceRectangle.Invert.X = (source.Value.Width < 0);
			sourceRectangle.Invert.Y = (source.Value.Height < 0);
		}

		source = sourceRectangle;
		origin = scaledOrigin;
		texture = resampledTexture;

		return Continue;
	}

	internal static uint EstimateScale(Vector2F scale, float scaleFactor) {
		float factoredScale = scale.MaxOf * scaleFactor;
		factoredScale += Config.Resample.ScaleBias;
		factoredScale = factoredScale.Clamp(2.0f, (float)Config.Resample.MaxScale);
		uint factoredScaleN = (uint)factoredScale.NextInt();
		return Resample.Scalers.IScaler.Current.ClampScale(factoredScaleN);
	}

	private static Vector2I test = (0, 1408);

	internal static bool OnDraw(
		this SpriteBatch @this,
		ref Texture2D texture,
		ref XNA.Vector2 position,
		ref XNA.Rectangle? source,
		XNA.Color color,
		float rotation,
		ref XNA.Vector2 origin,
		ref XNA.Vector2 scale,
		SpriteEffects effects,
		ref float layerDepth
	) {
		GetDrawParameters(
			texture: texture,
			source: source,
			bounds: out var sourceRectangle,
			scaleFactor: out var scaleFactor
		);

		var originalSourceRect = sourceRectangle;

		ManagedSpriteInstance? spriteInstance;
		ManagedTexture2D? resampledTexture;
		if (texture is ManagedTexture2D) {
			resampledTexture = (ManagedTexture2D)texture;
			spriteInstance = resampledTexture.Texture;
			sourceRectangle = resampledTexture.Dimensions;
		}
		else if (texture.FetchScaledTexture(
			expectedScale: EstimateScale(scale, scaleFactor),
			source: ref sourceRectangle,
			spriteInstance: out spriteInstance,
			create: true
		)) {
			spriteInstance.UpdateReferenceFrame();
			resampledTexture = spriteInstance.Texture!;
		}
		else {
			resampledTexture = null;
		}

		if (spriteInstance is null || resampledTexture is null) {
			return Continue;
		}

		if (originalSourceRect.X < 0) {
			position.X -= originalSourceRect.X * scale.X;
		}
		if (originalSourceRect.Y < 0) {
			position.Y -= originalSourceRect.Y * scale.Y;
		}

		var adjustedScale = (Vector2F)scale / (Vector2F)spriteInstance.Scale;
		var adjustedPosition = position;
		var adjustedOrigin = (Vector2F)origin;

		if (spriteInstance.TexType == TextureType.SlicedImage) {
			sourceRectangle = source ?? resampledTexture.Bounds;
			if (source is not null) {
				sourceRectangle = new Bounds(
					(Vector2I)source.Value.Location - spriteInstance.OriginalSourceRectangle.Offset,
					source.Value.Size
				);
				sourceRectangle.Offset = (sourceRectangle.OffsetF * spriteInstance.Scale).NearestInt();
				sourceRectangle.Extent = (sourceRectangle.ExtentF * spriteInstance.Scale).NearestInt();
			}
		}

		if (!spriteInstance.Padding.IsZero) {
			var textureSize = new Vector2F(sourceRectangle.Extent);
			var innerSize = (Vector2F)spriteInstance.UnpaddedSize;

			// This is the scale factor to bring the inner size to the draw size.
			var innerRatio = textureSize / innerSize; // spriteInstance.InnerRatio;

			// Scale the... scale by the scale factor.
			adjustedScale *= innerRatio;

			adjustedOrigin *= (Vector2F)spriteInstance.Scale;
			adjustedOrigin /= innerRatio;
			adjustedOrigin += (Vector2F)spriteInstance.Padding.Offset;
		}
		else {
			adjustedOrigin *= (Vector2F)spriteInstance.Scale;
		}

		if (source.HasValue) {
			sourceRectangle.Invert.X = (source.Value.Width < 0);
			sourceRectangle.Invert.Y = (source.Value.Height < 0);
		}

		texture = resampledTexture;
		source = sourceRectangle;
		origin = adjustedOrigin;
		scale = adjustedScale;
		position = adjustedPosition;
		return Continue;
	}
}
