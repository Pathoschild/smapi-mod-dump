/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
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
	private static readonly CachedDrawParams LastDrawParams = new();

	private sealed class CachedDrawParams {
		private readonly ComparableWeakReference<XTexture2D?> WeakReference = new(null!);
		private readonly WeakReference<ManagedSpriteInstance?> WeakInstance = new(null!);

		internal ManagedSpriteInstance? Instance {
			get => WeakInstance.TryGetTarget(out var target) ? target : null;
			set => WeakInstance.SetTarget(value);
		}

		internal XTexture2D? Reference {
			get => WeakReference.Target;
			set => WeakReference.Target = value;
		}
		internal uint ExpectedScale = 0;
		internal Bounds? Source = null;
		internal Bounds UpdatedSource = default;

		internal void Reset() {
			WeakReference.SetTarget(null);
			ExpectedScale = 0;
			Source = null;
			UpdatedSource = default;
		}
	}

	internal static void ResetLastDrawCache() {
		var lastDrawParams = LastDrawParams;
		lock (lastDrawParams) {
			lastDrawParams.Reset();
		}
	}

	private static bool FetchScaledTexture(
		this XTexture2D reference,
		uint expectedScale,
		ref Bounds source,
		[NotNullWhen(true)] out ManagedSpriteInstance? spriteInstance,
		bool create = false
	) {
		if (expectedScale <= 1U && !SMConfig.Resample.ForceOverScaling) {
			spriteInstance = null;
			return false;
		}

		var lastDrawParams = LastDrawParams;

		lock (lastDrawParams) {
			if (
				lastDrawParams.Instance is { } lastDrawInstance &&
				lastDrawInstance.IsReady && !lastDrawInstance.IsDisposed && !lastDrawInstance.Suspended &&
				lastDrawParams.Reference == reference &&
				lastDrawParams.ExpectedScale == expectedScale &&
				lastDrawParams.Source == source
			) {
				source = lastDrawParams.UpdatedSource;
				spriteInstance = lastDrawInstance;
				return true;
			}

			var originalSource = source;
			var invert = source.Invert;
			spriteInstance = reference.FetchScaledTexture(
				expectedScale: expectedScale,
				source: ref source,
				allowCache: out bool allowCache,
				create: create
			);
			source.Invert = invert;

			if (spriteInstance is not null) {
				if (allowCache) {
					lastDrawParams.Reference = reference;
					lastDrawParams.ExpectedScale = expectedScale;
					lastDrawParams.Source = originalSource;
					lastDrawParams.UpdatedSource = source;
					lastDrawParams.Instance = spriteInstance;
				}

				return true;
			}
		}

		return false;
	}

	private static ManagedSpriteInstance? FetchScaledTexture(
		this XTexture2D reference,
		uint expectedScale,
		ref Bounds source,
		out bool allowCache,
		bool create = false
	) {
		var clampedSource = source;

		allowCache = false;

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

			var extent = reference.Extent();

			// If the reference texture is too small to consider resampling, return null
			if (extent.MaxOf <= Config.Resample.MinimumTextureDimensions) {
				return null;
			}

			bool isSliced = false;
			if (GetIsSliced(clampedSource, reference, out var textureRef)) {
				clampedSource = textureRef.Value.Bounds.IsEmpty ? extent : textureRef.Value.Bounds;
				isSliced = true;
			}

			allowCache = true;

			var spriteInstance = create ?
				ManagedSpriteInstance.FetchOrCreate(texture: reference, source: clampedSource, expectedScale: expectedScale, sliced: isSliced, allowCache: out allowCache) :
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
		var sourceRectangle = (Bounds)source.GetValueOrDefault(new(0, 0, texture.Width, texture.Height));

		bool isWater;
		if (texture is not InternalTexture2D) {
			var meta = texture.Meta();
			meta.UpdateLastAccess();
			isWater = SpriteOverrides.IsWater(sourceRectangle, meta);
		}
		else {
			isWater = SpriteOverrides.IsWater(sourceRectangle, texture);
		}

		scaleFactor = isWater ? 4.0f : 1.0f;

		ReportOnceValidations.Validate(sourceRectangle, texture);
		bounds = sourceRectangle;
	}
}
