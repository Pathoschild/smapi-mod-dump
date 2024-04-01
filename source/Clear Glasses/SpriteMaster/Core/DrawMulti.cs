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
using SpriteMaster.Extensions;
using SpriteMaster.Harmonize;
using SpriteMaster.Harmonize.Patches.Game;
using SpriteMaster.Types;
using SpriteMaster.Types.Exceptions;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static SpriteMaster.Harmonize.Harmonize;
using static StardewValley.BellsAndWhistles.PlayerStatusList;

namespace SpriteMaster.Core;

internal static partial class OnDrawImpl {

	[StructLayout(LayoutKind.Auto)]
	internal struct DrawInstance {
		internal readonly Vector2F Position { get; init; }
		internal readonly float Scale { get; init; }
		internal readonly float Rotation { get; init; }
		internal uint ExpectedScale = 0U;

		internal DrawInstance(Snow.SnowWeatherDebris debris) {
			Position = debris.position;
			Scale = debris.Scale;
			Rotation = debris.Rotation;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[Harmonize(typeof(XSpriteBatch), "Draw", fixation: Fixation.Reverse)]
	public static void RawDraw(XSpriteBatch __instance, XTexture2D texture, XVector2 position, XRectangle? sourceRectangle, XColor color, float rotation, XVector2 origin, float scale, SpriteEffects effects, float layerDepth) {
		Harmonize.Patches.PSpriteBatch.Patch.Draw.IsReverse.Value = true;
		try {
			__instance.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
		}
		finally {
			Harmonize.Patches.PSpriteBatch.Patch.Draw.IsReverse.Value = false;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[Harmonize(typeof(XSpriteBatch), "Draw", fixation: Fixation.Reverse)]
	public static void RawDraw(
		XSpriteBatch __instance,
		XTexture2D texture,
		XVector2 position,
		XRectangle? sourceRectangle,
		XColor color,
		float rotation,
		XVector2 origin,
		XVector2 scale,
		SpriteEffects effects,
		float layerDepth
	) {
		Harmonize.Patches.PSpriteBatch.Patch.Draw.IsReverse.Value = true;
		try {
			__instance.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
		}
		finally {
			Harmonize.Patches.PSpriteBatch.Patch.Draw.IsReverse.Value = false;
		}
	}

	/*
	private static void TestDraw(
		XSpriteBatch @this,
		XTexture2D texture,
		Vector2F position,
		Bounds? sourceRectangle,
		XColor color,
		float rotation,
		Vector2F origin,
		Vector2F scale,
		SpriteEffects effects,
		float layerDepth
	) {
		@this.CheckValid(texture);

		var item = @this._batcher.CreateBatchItem();
		item.Texture = texture;

		// set SortKey based on SpriteSortMode.
		item.SortKey = @this._sortMode switch {
			// Comparison of Texture objects.
			SpriteSortMode.Texture => texture.SortingKey,
			// Comparison of Depth
			SpriteSortMode.FrontToBack => layerDepth,
			// Comparison of Depth in reverse
			SpriteSortMode.BackToFront => -layerDepth,
			_ => item.SortKey
		};

		origin *= scale;

		float w, h;
		if (sourceRectangle.HasValue) {
			var srcRect = sourceRectangle.GetValueOrDefault();
			w = srcRect.Width * scale.X;
			h = srcRect.Height * scale.Y;
			@this._texCoordTL.X = srcRect.X * texture.TexelWidth;
			@this._texCoordTL.Y = srcRect.Y * texture.TexelHeight;
			@this._texCoordBR.X = (srcRect.X + srcRect.Width) * texture.TexelWidth;
			@this._texCoordBR.Y = (srcRect.Y + srcRect.Height) * texture.TexelHeight;
		}
		else {
			w = texture.Width * scale.X;
			h = texture.Height * scale.Y;
			@this._texCoordTL = XVector2.Zero;
			@this._texCoordBR = XVector2.One;
		}

		if ((effects & SpriteEffects.FlipVertically) != 0) {
			var temp = @this._texCoordBR.Y;
			@this._texCoordBR.Y = @this._texCoordTL.Y;
			@this._texCoordTL.Y = temp;
		}
		if ((effects & SpriteEffects.FlipHorizontally) != 0) {
			var temp = @this._texCoordBR.X;
			@this._texCoordBR.X = @this._texCoordTL.X;
			@this._texCoordTL.X = temp;
		}

		if (rotation == 0f) {
			item.Set(position.X - origin.X,
							position.Y - origin.Y,
							w,
							h,
							color,
							@this._texCoordTL,
							@this._texCoordBR,
							layerDepth);
		}
		else {
			item.Set(position.X,
							position.Y,
							-origin.X,
							-origin.Y,
							w,
							h,
							(float)Math.Sin(rotation),
							(float)Math.Cos(rotation),
							color,
							@this._texCoordTL,
							@this._texCoordBR,
							layerDepth);
		}

		@this.FlushIfNeeded();
	}
	*/

	[StructLayout(LayoutKind.Auto)]
	private struct SpriteInstanceData {
		internal Bounds OriginalSourceRectangle;
		internal PaddingQuad Padding;

		private Vector2F _unpaddedSize;
		internal Vector2F UnpaddedSize {
			get => _unpaddedSize;
			set {
				_unpaddedSize = value;
				UnpaddedSizeReciprocal = Vector2F.One / value;
			}
		}
		internal Vector2F UnpaddedSizeReciprocal { get; private set; }

		private Vector2F _scale;
		internal Vector2F Scale {
			get => _scale;
			set {
				_scale = value;
				ScaleReciprocal = Vector2F.One / value;
			}
		}
		internal Vector2F ScaleReciprocal { get; private set; }

		internal TextureType TextureType;
	}

	/*
	private readonly struct DrawData {
		internal readonly ManagedTexture2D Texture;
		internal readonly Vector2F Position;
		internal readonly Bounds Source;
		internal readonly XColor Color;
		internal readonly float Rotation;
		internal readonly Vector2F Origin;
		internal readonly Vector2F Scale;
		internal readonly SpriteEffects Effects;
	}
	*/

	internal static void DrawMulti(
		this XSpriteBatch @this,
		XTexture2D texture,
		Bounds source,
		XColor color,
		XVector2 origin,
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
		Vector2F positionModifier = Vector2F.Zero;
		if (originalSourceRect.X < 0) {
			positionModifier.X = -originalSourceRect.X;
		}
		if (originalSourceRect.Y < 0) {
			positionModifier.Y = -originalSourceRect.Y;
		}

		ManagedSpriteInstance? spriteInstance = null;
		ManagedTexture2D? resampledTexture = null;
		if (texture is ManagedTexture2D texture2D) {
			resampledTexture = texture2D;
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

		bool debugModeEnabled = Debug.Mode.IsEnabled;

		SpriteInstanceData spriteInstanceData = default;

		uint fetchedScale = uint.MaxValue;
		foreach (var instance in instances) {
			Vector2F position = instance.Position;
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

				if (spriteInstance is not null) {
					spriteInstanceData = new() {
						OriginalSourceRectangle = spriteInstance.OriginalSourceRectangle,
						Padding = spriteInstance.Padding,
						UnpaddedSize = spriteInstance.UnpaddedSize,
						Scale = spriteInstance.Scale,
						TextureType = spriteInstance.TexType
					};
				}
			}

			if (spriteInstance is null || resampledTexture is null) {
				@this.Draw(texture, position, source, color, rotation, origin, scale, effects, layerDepth);
				continue;
			}

			position += positionModifier * scale;

			var instanceScale = spriteInstanceData.Scale;
			var instanceScaleReciprocal = spriteInstanceData.ScaleReciprocal;

			var adjustedScale = instanceScaleReciprocal * scale;
			var adjustedPosition = position;
			var adjustedOrigin = (Vector2F)origin;

			if (spriteInstanceData.TextureType == TextureType.SlicedImage) {
				sourceRectangle = new Bounds(
					source.Location - spriteInstanceData.OriginalSourceRectangle.Offset,
					source.Size
				);
				sourceRectangle.Offset = (sourceRectangle.OffsetF * instanceScale).NearestInt();
				sourceRectangle.Extent = (sourceRectangle.ExtentF * instanceScale).NearestInt();
			}

			if (!spriteInstanceData.Padding.IsZero) {
				var textureSize = new Vector2F(sourceRectangle.Extent);

				// This is the scale factor to bring the inner size to the draw size.
				var innerRatio = textureSize * spriteInstanceData.UnpaddedSizeReciprocal; // spriteInstance.InnerRatio;

				// Scale the... scale by the scale factor.
				adjustedScale *= innerRatio;

				adjustedOrigin *= instanceScale;
				adjustedOrigin /= innerRatio;
				adjustedOrigin += (Vector2F)spriteInstanceData.Padding.Offset;
			}
			else {
				adjustedOrigin *= instanceScale;
			}

			sourceRectangle.Invert = (source.Width < 0, source.Height < 0);

			if (debugModeEnabled && Debug.Mode.RegisterDrawForSelect(
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
				color = XColor.Red;
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
