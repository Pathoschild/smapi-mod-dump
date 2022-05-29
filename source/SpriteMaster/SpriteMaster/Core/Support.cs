/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Metadata;
using SpriteMaster.Types;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Core;

internal static partial class OnDrawImpl {
	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static bool GetIsSliced(Bounds bounds, XTexture2D reference, [NotNullWhen(true)] out Config.TextureRef? result) {
		return reference.Meta().CheckSliced(bounds, out result);
	}

	private static bool Cleanup(this ref Bounds sourceBounds, XTexture2D reference) {
		if (Config.ClampInvalidBounds && !sourceBounds.ClampToChecked(reference.Extent(), out var clampedBounds)) {
			//Debug.Warning($"Draw.Cleanup: '{reference.SafeName()}' bounds '{sourceBounds}' are not contained in reference bounds '{(Bounds)reference.Bounds}' - clamped ({(sourceBounds.Degenerate ? "degenerate" : "")})");
			sourceBounds = clampedBounds;
		}

		// Let's just skip potentially invalid draws since I have no idea what to do with them.
		return !sourceBounds.Degenerate;
	}

	// Odds are high that we will run into the same textures/sprites being drawn one after another.
	// Thus, if we cache the last one, we will more-often-than-not likely be able to avoid a lot of work.
	private static (XTexture2D? Reference, uint ExpectedScale, Bounds? Source, Bounds UpdatedSource) LastDrawParams = new(null, 0, null, new());
	private static ManagedSpriteInstance? LastDrawSpriteInstance = null;

	internal static void ResetLastDrawCache() {
		LastDrawParams = new(null, 0, null, new());
		LastDrawSpriteInstance = null;
	}

	private static bool FetchScaledTexture(
		this XTexture2D reference,
		uint expectedScale,
		ref Bounds source,
		[NotNullWhen(true)] out ManagedSpriteInstance? spriteInstance,
		bool create = false
	) {
		if (
			LastDrawSpriteInstance is not null &&
			LastDrawSpriteInstance.IsReady && !LastDrawSpriteInstance.IsDisposed && !LastDrawSpriteInstance.Suspended &&
			LastDrawParams.Reference == reference &&
			LastDrawParams.ExpectedScale == expectedScale &&
			LastDrawParams.Source == source
		) {
			source = LastDrawParams.UpdatedSource;
			spriteInstance = LastDrawSpriteInstance;
			return true;
		}

		var originalSource = source;
		var invert = source.Invert;
		spriteInstance = reference.FetchScaledTexture(
			expectedScale: expectedScale,
			source: ref source,
			create: create
		);
		source.Invert = invert;

		if (spriteInstance is not null) {
			LastDrawParams = new(reference, expectedScale, originalSource, source);
			LastDrawSpriteInstance = spriteInstance;
			return true;
		}

		return false;
	}

	private static ManagedSpriteInstance? FetchScaledTexture(
		this XTexture2D reference,
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
				clampedSource = textureRef.Value.Bounds.IsEmpty ? reference.Extent() : textureRef.Value.Bounds;
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

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static bool Validate(this ManagedTexture2D texture) => !texture.IsDisposed;

	private static void GetDrawParameters(XTexture2D texture, XRectangle? source, out Bounds bounds, out float scaleFactor) {
		if (texture is not InternalTexture2D) {
			texture.Meta().UpdateLastAccess();
		}

		var sourceRectangle = (Bounds)source.GetValueOrDefault(new(0, 0, texture.Width, texture.Height));

		scaleFactor = SpriteOverrides.IsWater(sourceRectangle, texture) ? 4.0f : 1.0f;

		ReportOnceValidations.Validate(sourceRectangle, texture);
		bounds = sourceRectangle;
	}
}
