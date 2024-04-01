/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SpriteMaster.Core;

[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Harmony")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony")]
internal static partial class OnDrawImpl {
	private const bool Continue = true;
	private const bool Stop = false;

	// Takes the arguments, and checks to see if the texture is padded. If it is, it is forwarded to the correct draw call, avoiding
	// intervening mods altering the arguments first.
	internal static bool OnDrawFirst(
		this XSpriteBatch @this,
		ref XTexture2D? texture,
		ref XRectangle destination,
		ref XRectangle? source,
		XColor color,
		float rotation,
		ref XVector2 origin,
		ref SpriteEffects effects,
		float layerDepth,
		ref ManagedTexture2D? __state
	) {
		if (texture is null) {
			return false;
		}

		using var watchdogScoped = WatchDog.WatchDog.ScopedWorkingState;

		/*
		if (destination.Width < 0 || destination.Height < 0) {
			Debug.Trace("destination invert");
		}
		if (source is XRectangle sourceRect && (sourceRect.Width < 0 || sourceRect.Height < 0)) {
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
				//Debug.Trace("Winding Order Error");
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

	internal static bool OnDraw(
		this XSpriteBatch @this,
		ref XTexture2D? texture,
		ref XRectangle destination,
		ref XRectangle? source,
		ref XColor color,
		float rotation,
		ref XVector2 origin,
		ref SpriteEffects effects,
		ref float layerDepth,
		ref ManagedTexture2D? __state
	) {
		if (texture is null) {
			return false;
		}

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

			if (!spriteInstance.Padding.IsZero) {
				ResetLastDrawCache();
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
			spriteInstance = resampledTexture.SpriteInstance;

			if (!spriteInstance.Padding.IsZero) {
				Debug.Trace($"Non-padded Draw Implementation path taken for padded sprite! ({nameof(__state)} is not null)");
			}

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

		var scaledOrigin = (Vector2F)origin * spriteInstance.Scale;

		if (source.HasValue) {
			sourceRectangle.Invert.X = source.Value.Width < 0;
			sourceRectangle.Invert.Y = source.Value.Height < 0;
		}

		if (Debug.Mode.RegisterDrawForSelect(
			instance: spriteInstance,
			texture: texture,
			originalDestination: destinationBounds,
			destination: destination,
			source: sourceRectangle,
			color: color,
			rotation: rotation,
			originalOrigin: origin,
			origin: scaledOrigin,
			effects: effects,
			layerDepth: layerDepth
		)) {
			color = XColor.Red;
		}

		source = sourceRectangle;
		origin = scaledOrigin;
		texture = resampledTexture;

		return Continue;
	}

	internal static uint EstimateScale(Vector2F scale, float scaleFactor) {
		int minScale = Config.Resample.MinScale;
		int maxScale = Config.Resample.MaxScale;
		if (minScale > maxScale) {
			(minScale, maxScale) = (maxScale, minScale);
		}

		float factoredScale = scale.MaxOf * scaleFactor;
		factoredScale += SMConfig.Resample.OverScale;
		factoredScale = factoredScale.Clamp(minScale, maxScale);
		uint factoredScaleN = (uint)factoredScale.NextInt();
		return Resample.Scalers.IScaler.Current?.ClampScale(factoredScaleN) ?? 1u;
	}

	internal static bool OnDraw(
		this XSpriteBatch @this,
		ref XTexture2D? texture,
		ref XVector2 position,
		ref XRectangle? source,
		ref XColor color,
		float rotation,
		ref XVector2 origin,
		ref XVector2 scale,
		SpriteEffects effects,
		ref float layerDepth
	) {
		if (texture is null) {
			return false;
		}

		GetDrawParameters(
			texture: texture,
			source: source,
			bounds: out var sourceRectangle,
			scaleFactor: out var scaleFactor
		);

		var originalSourceRect = sourceRectangle;

		ManagedSpriteInstance? spriteInstance;
		ManagedTexture2D? resampledTexture;
		if (texture is ManagedTexture2D texture2D) {
			resampledTexture = texture2D;
			spriteInstance = resampledTexture.SpriteInstance;
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

		var adjustedScale = (Vector2F)scale / spriteInstance.Scale;
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
			var paddingX = spriteInstance.Padding.X;
			var paddingY = spriteInstance.Padding.Y;

			if (effects.HasFlag(SpriteEffects.FlipHorizontally)) {
				paddingX = (paddingX.Y, paddingX.X);
			}

			if (effects.HasFlag(SpriteEffects.FlipVertically)) {
				paddingY = (paddingY.Y, paddingY.X);
			}

			var padding = new PaddingQuad(paddingX, paddingY);

			var textureSize = new Vector2F(sourceRectangle.Extent);
			var innerSize = (Vector2F)spriteInstance.UnpaddedSize;

			// This is the scale factor to bring the inner size to the draw size.
			var innerRatio = textureSize / innerSize; // spriteInstance.InnerRatio;

			// Scale the... scale by the scale factor.
			adjustedScale *= innerRatio;

			adjustedOrigin *= spriteInstance.Scale;
			adjustedOrigin /= innerRatio;
			adjustedOrigin += (Vector2F)padding.Offset;
		}
		else {
			adjustedOrigin *= spriteInstance.Scale;
		}

		if (source.HasValue) {
			sourceRectangle.Invert.X = source.Value.Width < 0;
			sourceRectangle.Invert.Y = source.Value.Height < 0;
		}

		if (Debug.Mode.RegisterDrawForSelect(
			instance: spriteInstance,
			texture: texture,
			originalPosition: position,
			originalSource: source,
			position: adjustedPosition,
			source: sourceRectangle,
			color: color,
			rotation: rotation,
			originalOrigin: origin,
			origin: adjustedOrigin,
			scale: adjustedScale,
			effects: effects,
			layerDepth: layerDepth
		)) {
			color = XColor.Red;
		}

		texture = resampledTexture;
		source = sourceRectangle;
		origin = adjustedOrigin;
		scale = adjustedScale;
		position = adjustedPosition;
		return Continue;
	}
}
