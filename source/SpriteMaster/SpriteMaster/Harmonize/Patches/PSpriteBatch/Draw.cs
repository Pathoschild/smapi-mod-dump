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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Harmonize.Patches.PSpriteBatch;

[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Harmony")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony")]
static class Draw {
	private const bool Continue = true;
	private const bool Stop = false;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static bool Cleanup(this ref Bounds sourceRectangle, Texture2D reference) {
		if (Config.ClampInvalidBounds) {
			sourceRectangle = sourceRectangle.ClampTo(new(reference.Width, reference.Height));
		}

		// Let's just skip potentially invalid draws since I have no idea what to do with them.
		return !sourceRectangle.Degenerate;
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
		var newSource = source;

		try {
			// If the (potentially-clamped) source bounds are invalid, return null
			if (!newSource.Cleanup(reference)) {
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

			var spriteInstance = create ?
				ManagedSpriteInstance.FetchOrCreate(texture: reference, source: newSource, expectedScale: expectedScale) :
				ManagedSpriteInstance.Fetch(texture: reference, source: newSource, expectedScale: expectedScale);

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
		texture.Meta().UpdateLastAccess();

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
		using var _ = Performance.Track();

		if (destination.Width < 0 || destination.Height < 0) {
			Debug.Trace("destination invert");
		}
		if (source is XNA.Rectangle sourceRect && (sourceRect.Width < 0 || sourceRect.Height < 0)) {
			Debug.Trace("source invert");
		}

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

		var resampledTexture = spriteInstance.Texture!;

		if (!spriteInstance.Padding.IsZero) {
			// Convert the draw into the other draw style. This has to be done because the padding potentially has
			// subpixel accuracy when scaled to the destination rectangle.

			var originalSize = referenceRectangle.ExtentF;
			var destinationSize = destinationBounds.ExtentF;
			var newScale = destinationSize / originalSize;
			var newPosition = destinationBounds.OffsetF;

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
		using var _ = Performance.Track("OnDraw0");

		Bounds sourceRectangle;
		ManagedSpriteInstance? spriteInstance;
		ManagedTexture2D resampledTexture;

		Bounds destinationBounds = destination;

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
			spriteInstance.UpdateReferenceFrame();

			resampledTexture = spriteInstance.Texture!;
		}
		else {
			resampledTexture = __state;
			spriteInstance = resampledTexture.Texture;
			sourceRectangle = resampledTexture.Dimensions;
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
		using var _ = Performance.Track("OnDraw1");

		GetDrawParameters(
			texture: texture,
			source: source,
			bounds: out var sourceRectangle,
			scaleFactor: out var scaleFactor
		);

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

		var adjustedScale = (Vector2F)scale / (Vector2F)spriteInstance.Scale;
		var adjustedPosition = position;
		var adjustedOrigin = (Vector2F)origin;

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
