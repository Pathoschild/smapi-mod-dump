/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.Common;

public class SpriteInfo {
	public Texture2D Texture;

	public Rectangle BaseSource;
	public Color? BaseColor;
	public float BaseScale;

	public Texture2D? OverlayTexture;
	public Rectangle? OverlaySource;
	public Color? OverlayColor;
	public float OverlayScale;

	public bool IsPrismatic;

	public int BaseFrames;
	public int OverlayFrames;
	public int FramesPerRow;
	public int FrameTime;
	public int FrameDelay;
	public int FramePadX;
	public int FramePadY;

	public SpriteInfo(
		Texture2D texture,
		Rectangle baseSource,
		Color? baseColor = null,
		float baseScale = 1f,
		Texture2D? overlayTexture = null,
		Rectangle? overlaySource = null,
		Color? overlayColor = null,
		float overlayScale = 1f,
		bool isPrismatic = false,
		int baseFrames = 1,
		int overlayFrames = 1,
		int framesPerRow = int.MaxValue,
		int frameTime = 100,
		int frameDelay = 0,
		int framePadX = 0,
		int framePadY = 0
	) {
		Texture = texture;
		BaseSource = baseSource;
		BaseColor = baseColor;
		BaseScale = baseScale;
		OverlayTexture = overlayTexture;
		OverlaySource = overlaySource;
		OverlayColor = overlayColor;
		OverlayScale = overlayScale;
		IsPrismatic = isPrismatic;
		BaseFrames = baseFrames;
		OverlayFrames = overlayFrames;
		FramesPerRow = framesPerRow;
		FrameTime = frameTime;
		FrameDelay = frameDelay;
		FramePadX = framePadX;
		FramePadY = framePadY;
	}

	public int Width {
		get {
			int result = BaseSource.Width;

			if (BaseFrames > 1) {
				int cols = FramesPerRow;
				if (cols > BaseFrames)
					cols = BaseFrames;

				result /= cols;
				if (FramePadX != 0)
					result -= (FramePadX * cols);
			}

			if (OverlaySource.HasValue) {
				int overlayWidth = OverlaySource.Value.Width;

				if (OverlayFrames > 1) {
					int cols = FramesPerRow;
					if (cols > OverlayFrames)
						cols = OverlayFrames;

					overlayWidth /= cols;

					if (FramePadX != 0)
						overlayWidth -= (FramePadX * cols);
				}

				if (overlayWidth > result)
					result = overlayWidth;
			}

			return result;
		}
	}

	public int Height {
		get {
			int result = BaseSource.Height;

			if (BaseFrames > 1) {
				int cols = FramesPerRow;
				if (cols > BaseFrames)
					cols = BaseFrames;

				int rows = (int) Math.Ceiling((double) BaseFrames / cols);

				result /= rows;
				if (FramePadY != 0)
					result -= (FramePadY * rows);
			}

			if (OverlaySource.HasValue) {
				int overlayHeight = OverlaySource.Value.Height;

				if (OverlayFrames > 1) {
					int cols = FramesPerRow;
					if (cols > OverlayFrames)
						cols = OverlayFrames;

					int rows = (int) Math.Ceiling((double) OverlayFrames / cols);

					overlayHeight /= rows;
					if (FramePadY != 0)
						overlayHeight -= (FramePadY * rows);
				}

				if (overlayHeight > result)
					result = overlayHeight;
			}

			return result;
		}
	}


	public static Rectangle GetFrame(Rectangle source, int frame, int frames, int cols, int frameTime = 100, int frameDelay = 0, int framePadX = 0, int framePadY = 0) {
		if (frames <= 1)
			return source;

		if (frame < 0)
			frame = (int) ((
				Game1.currentGameTime.TotalGameTime.TotalMilliseconds
				+ frameDelay
			) / frameTime) % frames;

		if (cols > frames)
			cols = frames;

		int rows = (int) Math.Ceiling((double) frames / cols);

		int width = source.Width;
		int height = source.Height;

		if (framePadX != 0)
			width -= framePadX * cols;
		if (framePadY != 0)
			height -= framePadY * rows;

		width /= cols;
		height /= rows;

		int row = frame / cols;
		int col = frame % cols;

		return new Rectangle(
			source.X + (col * width) + ((col) * framePadX),
			source.Y + (row * height) + ((row) * framePadY),
			width,
			height
		);
	}

	public virtual void Draw(SpriteBatch batch, Vector2 location, float scale, int frame = -1, float size = 16, Color? baseColor = null, Color? overlayColor = null, float alpha = 1) {
		Rectangle source = GetFrame(BaseSource, frame, BaseFrames, FramesPerRow, FrameTime, FrameDelay, FramePadX, FramePadY);
		Rectangle? overlay = OverlaySource.HasValue ?
			GetFrame(OverlaySource.Value, frame, OverlayFrames, FramesPerRow, FrameTime, FrameDelay, FramePadX, FramePadY)
			: null;

		float width = source.Width * BaseScale;
		float height = source.Height * BaseScale;

		if (overlay.HasValue) {
			width = Math.Max(width, overlay.Value.Width * OverlayScale);
			height = Math.Max(height, overlay.Value.Height * OverlayScale);
		}

		float max = Math.Max(width, height);

		float targetSize = scale * size;
		float s = Math.Min(scale, targetSize / max);

		// Draw the base.
		float bs = s * BaseScale;
		float offsetX = Math.Max((targetSize - (source.Width * bs)) / 2, 0);
		float offsetY = Math.Max((targetSize - (source.Height * bs)) / 2, 0);

		Color color = baseColor ?? BaseColor ?? Color.White;
		if (!baseColor.HasValue && IsPrismatic && ! overlay.HasValue)
			color = Utility.GetPrismaticColor();

		batch.Draw(
			Texture,
			new Vector2(
				(float) Math.Floor(location.X + offsetX),
				(float) Math.Floor(location.Y + offsetY)
			),
			source,
			color * alpha,
			0f,
			Vector2.Zero,
			bs,
			SpriteEffects.None,
			1f
		);

		if (overlay != null) {
			float os = s * OverlayScale;
			offsetX = Math.Max((targetSize - (overlay.Value.Width * os)) / 2, 0);
			offsetY = Math.Max((targetSize - (overlay.Value.Height * os)) / 2, 0);

			color = overlayColor ?? OverlayColor ?? Color.White;
			if (! overlayColor.HasValue && IsPrismatic)
				color = Utility.GetPrismaticColor();

			batch.Draw(
				OverlayTexture ?? Texture,
				new Vector2(
					(float) Math.Floor(location.X + offsetX),
					(float) Math.Floor(location.Y + offsetY)
				),
				overlay.Value,
				color * alpha,
				0f,
				Vector2.Zero,
				os,
				SpriteEffects.None,
				1f
			);
		}
	}

	public virtual void Draw(SpriteBatch batch, Vector2 location, float scale, Vector2 size, int frame = -1, Color? baseColor = null, Color? overlayColor = null, float alpha = 1) {
		Rectangle source = GetFrame(BaseSource, frame, BaseFrames, FramesPerRow, FrameTime, FrameDelay, FramePadX, FramePadY);
		Rectangle? overlay = OverlaySource.HasValue ?
			GetFrame(OverlaySource.Value, frame, OverlayFrames, FramesPerRow, FrameTime, FrameDelay, FramePadX, FramePadY)
			: null;

		float width = source.Width * BaseScale;
		float height = source.Height * BaseScale;

		if (overlay.HasValue) {
			width = Math.Max(width, overlay.Value.Width * OverlayScale);
			height = Math.Max(height, overlay.Value.Height * OverlayScale);
		}

		
		float targetWidth = scale * size.X;
		float targetHeight = scale * size.Y;

		float s = Math.Min(scale, Math.Min(targetWidth / width, targetHeight / height));

		// Draw the base.
		float bs = s * BaseScale;
		float offsetX = Math.Max((targetWidth - (source.Width * bs)) / 2, 0);
		float offsetY = Math.Max((targetHeight - (source.Height * bs)) / 2, 0);

		Color color = baseColor ?? BaseColor ?? Color.White;
		if (!baseColor.HasValue && IsPrismatic && !overlay.HasValue)
			color = Utility.GetPrismaticColor();

		batch.Draw(
			Texture,
			new Vector2(
				(float) Math.Floor(location.X + offsetX),
				(float) Math.Floor(location.Y + offsetY)
			),
			source,
			color * alpha,
			0f,
			Vector2.Zero,
			bs,
			SpriteEffects.None,
			1f
		);

		if (overlay != null) {
			float os = s * OverlayScale;
			offsetX = Math.Max((targetWidth - (overlay.Value.Width * os)) / 2, 0);
			offsetY = Math.Max((targetHeight - (overlay.Value.Height * os)) / 2, 0);

			color = overlayColor ?? OverlayColor ?? Color.White;
			if (!overlayColor.HasValue && IsPrismatic)
				color = Utility.GetPrismaticColor();

			batch.Draw(
				OverlayTexture ?? Texture,
				new Vector2(
					(float) Math.Floor(location.X + offsetX),
					(float) Math.Floor(location.Y + offsetY)
				),
				overlay.Value,
				color * alpha,
				0f,
				Vector2.Zero,
				os,
				SpriteEffects.None,
				1f
			);
		}
	}
}
