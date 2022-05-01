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
using SpriteMaster.Harmonize;
using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;
using static SpriteMaster.Harmonize.Harmonize;

namespace SpriteMaster.Core;

static partial class OnDrawImpl {

	internal struct DrawInstance {
		internal readonly Vector2F Position { get; init; }
		internal readonly float Scale { get; init; }
		internal readonly float Rotation { get; init; }
		internal uint ExpectedScale;
	}

	[Harmonize(typeof(SpriteBatch), "Draw", fixation: Fixation.Reverse)]
	public static void RawDraw(SpriteBatch __instance, Texture2D texture, XNA.Vector2 position, XNA.Rectangle? sourceRectangle, XNA.Color color, float rotation, XNA.Vector2 origin, float scale, SpriteEffects effects, float layerDepth) {
		throw new NotImplementedException($"{nameof(RawDraw)} is a reverse patch");
	}

	[Harmonize(typeof(SpriteBatch), "Draw", fixation: Fixation.Reverse)]
	public static void RawDraw(SpriteBatch __instance, Texture2D texture, XNA.Vector2 position, XNA.Rectangle? sourceRectangle, XNA.Color color, float rotation, XNA.Vector2 origin, XNA.Vector2 scale, SpriteEffects effects, float layerDepth) {
		throw new NotImplementedException($"{nameof(RawDraw)} is a reverse patch");
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void DrawMulti(
		this SpriteBatch @this,
		Texture2D texture,
		in Bounds source,
		XNA.Color color,
		XNA.Vector2 origin,
		SpriteEffects effects,
		float layerDepth,
		Span<DrawInstance> instances
	) {
		GetDrawParameters(
			texture: texture,
			source: source,
			bounds: out var sourceRectangle,
			scaleFactor: out var scaleFactor
		);

		var originalSourceRect = sourceRectangle;

		ManagedSpriteInstance? spriteInstance = null;
		ManagedTexture2D? resampledTexture = null;
		if (texture is ManagedTexture2D) {
			resampledTexture = (ManagedTexture2D)texture;
			spriteInstance = resampledTexture.SpriteInstance;
			sourceRectangle = resampledTexture.Dimensions;
		}
		else {
			// Rapidly sort the inputs based upon scale estimates
			for (int i = 0; i < instances.Length; ++i) {
				ref var instance = ref instances[i];
				instance.ExpectedScale = EstimateScale((instance.Scale, instance.Scale), scaleFactor);
			}

			instances.Sort((a, b) => a.ExpectedScale.CompareTo(b.ExpectedScale));
		}

		uint fetchedScale = uint.MaxValue;
		foreach (var instance in instances) {
			XNA.Vector2 position = instance.Position;
			float scale = instance.Scale;
			float rotation = instance.Rotation;

			if (fetchedScale != instance.ExpectedScale && (spriteInstance is null || resampledTexture is null)) {
				if (texture.FetchScaledTexture(
					expectedScale: instance.ExpectedScale,
					source: ref sourceRectangle,
					spriteInstance: out spriteInstance,
					create: false
				)) {
					spriteInstance.UpdateReferenceFrame();
					resampledTexture = spriteInstance.Texture!;
				}
				else {
					resampledTexture = null;
				}
				fetchedScale = instance.ExpectedScale;
			}

			if (spriteInstance is null || resampledTexture is null) {
				@this.Draw(texture, position, source, color, rotation, origin, scale, effects, layerDepth);
				continue;
			}

			if (originalSourceRect.X < 0) {
				position.X -= originalSourceRect.X * scale;
			}
			if (originalSourceRect.Y < 0) {
				position.Y -= originalSourceRect.Y * scale;
			}

			var adjustedScale = new Vector2F(scale) / spriteInstance.Scale;
			var adjustedPosition = position;
			var adjustedOrigin = (Vector2F)origin;

			if (spriteInstance.TexType == TextureType.SlicedImage) {
				sourceRectangle = source;
				sourceRectangle = new Bounds(
					(Vector2I)source.Location - spriteInstance.OriginalSourceRectangle.Offset,
					source.Size
				);
				sourceRectangle.Offset = (sourceRectangle.OffsetF * spriteInstance.Scale).NearestInt();
				sourceRectangle.Extent = (sourceRectangle.ExtentF * spriteInstance.Scale).NearestInt();
			}

			if (!spriteInstance.Padding.IsZero) {
				var textureSize = new Vector2F(sourceRectangle.Extent);
				var innerSize = (Vector2F)spriteInstance.UnpaddedSize;

				// This is the scale factor to bring the inner size to the draw size.
				var innerRatio = textureSize / innerSize; // spriteInstance.InnerRatio;

				// Scale the... scale by the scale factor.
				adjustedScale *= innerRatio;

				adjustedOrigin *= spriteInstance.Scale;
				adjustedOrigin /= innerRatio;
				adjustedOrigin += (Vector2F)spriteInstance.Padding.Offset;
			}
			else {
				adjustedOrigin *= spriteInstance.Scale;
			}

			sourceRectangle.Invert.X = source.Width < 0;
			sourceRectangle.Invert.Y = source.Height < 0;

			if (Debug.Mode.RegisterDrawForSelect(
				instance: spriteInstance,
				texture: texture,
				position: adjustedPosition,
				source: sourceRectangle,
				color: color,
				rotation: rotation,
				origin: adjustedOrigin,
				scale: adjustedScale,
				effects: effects,
				layerDepth: layerDepth
			)) {
				color = XNA.Color.Red;
			}

			RawDraw(
				@this,
				resampledTexture,
				adjustedPosition,
				sourceRectangle,
				color,
				rotation,
				adjustedOrigin,
				adjustedScale,
				effects,
				layerDepth
			);
		}
	}
}
