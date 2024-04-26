/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.CloudySkies.Models;

public class CustomDebris {

	public Vector2 Position;

	public int GutterWidth;

	public float deltaX;
	public float deltaY;

	public bool Blowing;

	public bool CanBlow;

	public int Width;
	public int Height;

	public bool ShouldAnimate;
	public int AnimationTimer;
	public int TimePerFrame;
	public int CurrentFrame;
	public sbyte AnimationDirection;

	public int SourceIndex = -1;

	public CustomDebris(Vector2 position, int width, int height, bool canBlow, int gutterWidth, int timePerFrame, bool shouldAnimate) {
		Position = position;
		Width = width;
		Height = height;
		CanBlow = canBlow;
		GutterWidth = gutterWidth;
		TimePerFrame = timePerFrame;
		ShouldAnimate = shouldAnimate;

		deltaX = Game1.random.Next(-10, 0) / 50f;
		deltaY = Game1.random.Next(-15, 10) / 50f;
	}

	public void ClampToViewport() {
		bool wrapped = false;

		if (Position.X < (-Width - GutterWidth)) {
			Position.X = Game1.viewport.Width;
			Position.Y = Game1.random.Next(Game1.viewport.Height - 64);
			wrapped = true;

		} else if (Position.X > (Game1.viewport.Width + GutterWidth)) {
			Position.X = -Width;
			Position.Y = Game1.random.Next(Game1.viewport.Height - 64);
			wrapped = true;
		}

		if (Position.Y < (-Height - GutterWidth)) {
			Position.X = Game1.random.Next(Game1.viewport.Width - 64);
			Position.Y = Game1.viewport.Height;
			wrapped = true;

		} else if (Position.Y > (Game1.viewport.Height + GutterWidth)) {
			Position.X = Game1.random.Next(Game1.viewport.Width - 64);
			Position.Y = -Width;
			wrapped = true;
		}

		if (wrapped) {
			SourceIndex = -1;
			deltaX = Game1.random.Next(-10, 0) / 50f;
			deltaY = Game1.random.Next(-15, 10) / 50f;
		}
	}


	public void Update(GameTime time) {
		Position.X += deltaX + WeatherDebris.globalWind;
		Position.Y += deltaY - -0.5f;

		if (deltaY < 0f && !Blowing)
			deltaY += 0.01f;

		if (!Game1.fadeToBlack && Game1.fadeToBlackAlpha <= 0f)
			ClampToViewport();

		if (Blowing) {
			deltaY -= 0.01f;
			if (Game1.random.NextDouble() < 0.006 || deltaY < -2f)
				Blowing = false;

		} else if (Game1.random.NextDouble() < 0.001 && CanBlow)
			Blowing = true;

		AnimationTimer += time.ElapsedGameTime.Milliseconds;
		if (AnimationTimer < TimePerFrame)
			return;

		AnimationTimer = 0;
		CurrentFrame += AnimationDirection;

		if (AnimationDirection == 0) {
			if (CurrentFrame >= 9)
				AnimationDirection = -1;
			else
				AnimationDirection = 1;
		}

		if (CurrentFrame > 10) {
			if (Game1.random.NextDouble() < 0.82) {
				CurrentFrame--;
				AnimationDirection = 0;
				deltaX += 0.1f;
				deltaY -= 0.2f;
			} else
				CurrentFrame = 0;

		} else if (CurrentFrame == 4 && AnimationDirection == -1) {
			CurrentFrame++;
			AnimationDirection = 0;
			deltaX -= 0.1f;
			deltaY -= 0.1f;
		}

		if (CurrentFrame == 7 && AnimationDirection == -1)
			deltaY -= 0.2f;
	}

	public void Draw(SpriteBatch batch, Texture2D texture, Rectangle source, Color color, float scale, SpriteEffects effects) {
		batch.Draw(
			texture,
			Position,
			ShouldAnimate
				? new Rectangle(
					source.X + CurrentFrame * source.Width,
					source.Y,
					source.Width, source.Height
				)
				: source,
			color,
			0f,
			Vector2.Zero,
			scale,
			effects,
			1E-06f
		);
	}

}
