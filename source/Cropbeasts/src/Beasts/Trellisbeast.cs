using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using System;

namespace Cropbeasts.Beasts
{
	// Much of this class is based on StardewValley.Monsters.Bat.
	public class Trellisbeast : Cropbeast
	{
		public const float rotationIncrement = (float) Math.PI / 64f;
		public const float maxSpeed = 5f;

		private readonly NetInt wasHitCounter = new NetInt (0);
		private readonly NetBool turningRight = new NetBool ();

		private ICue sound;

		public Trellisbeast ()
		{
			sound = Game1.soundBank.GetCue ("batFlap");
		}

		public Trellisbeast (CropTile cropTile, bool primary)
		: base (cropTile, primary, primary, !primary)
		{
			Slipperiness = 24 + Game1.random.Next (-10, 11);
			Halt ();
			IsWalkingTowardPlayer = false;
			HideShadow = true;

			sound = Game1.soundBank.GetCue ("batFlap");
		}

		protected override void initNetFields ()
		{
			base.initNetFields ();
			NetFields.AddFields (wasHitCounter, turningRight);
		}

		public override int takeDamage (int damage, int xTrajectory,
			int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
		{
			int result = takeDamage (damage, xTrajectory, yTrajectory,
				isBomb, addedPrecision, who, "hitEnemy");
			if (result > -1)
				wasHitCounter.Value = 500;
			addedSpeed = Game1.random.Next (-1, 1);
			return result;
		}

		protected override void localDeathAnimation ()
		{
			base.localDeathAnimation ();
			currentLocation.localSound ("batScreech");
			currentLocation.temporarySprites.Add (new TemporaryAnimatedSprite
				(44, Position, secondaryColor.Value, 10));
		}

		public override void drawAboveAllLayers (SpriteBatch b)
		{
			if (!Utility.isOnScreen (Position, 128))
				return;

			// Draw the base sprite with the bat wings.
			b.Draw (Sprite.Texture, getLocalPosition (Game1.viewport) +
				new Vector2 (32f, 32f), Sprite.SourceRect, secondaryColor.Value,
				0f, new Vector2 (8f, 16f), Math.Max (0.2f, Scale) * 4f,
				flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0.92f);

			// Draw the harvest itself.
			float yOffset = (Sprite.currentFrame % 4) switch
			{
				1 =>  -8f,
				2 => -10f,
				3 =>  -8f,
				_ =>  -4f,
			};
			b.Draw (cropTile.mapping.harvestTexture, getLocalPosition (Game1.viewport) +
				new Vector2 (32f, 32f + yOffset), cropTile.mapping.harvestSourceRect,
				ContrastTint, 0f, new Vector2 (8f, 8f), Math.Max (0.2f, Scale) *
				((cropTile.harvestIndex == 188) ? 3.5f : 2.5f), // Green Bean bigger
				flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);

			// Draw the shadow.
			b.Draw (Game1.shadowTexture, getLocalPosition (Game1.viewport) +
				new Vector2 (32f, 64f), Game1.shadowTexture.Bounds, Color.White,
				0f, new Vector2 (Game1.shadowTexture.Bounds.Center.X,
				Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None,
				0.0001f);

			if (isGlowing)
			{
				b.Draw (Sprite.Texture, getLocalPosition (Game1.viewport) +
					new Vector2 (32f, 32f), Sprite.SourceRect,
					glowingColor * glowingTransparency, 0f,
					new Vector2 (8f, 16f), Math.Max (0.2f, Scale) * 4f,
					flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
					Math.Max (0f, drawOnTop ? 0.99f : getStandingY () / 10000f + 0.001f));
			}
		}

		public override void behaviorAtGameTick (GameTime time)
		{
			base.behaviorAtGameTick (time);

			if (wasHitCounter.Value >= 0)
				wasHitCounter.Value -= time.ElapsedGameTime.Milliseconds;

			if (double.IsNaN (xVelocity) || double.IsNaN (yVelocity) ||
					Position.X < -2000f || Position.Y < -2000f)
				Health = -500;

			var layer = currentLocation.Map.Layers[0];
			if (Position.X <= -640f || Position.Y <= -640f ||
					Position.X >= (float) (layer.LayerWidth * 64 + 640) ||
					Position.Y >= (float) (layer.LayerHeight * 64 + 640))
				Health = -500;

			if (invincibleCountdown > 0)
				return;

			float xDistance = -(Player.GetBoundingBox ().Center.X -
				GetBoundingBox ().Center.X);
			float yDistance = Player.GetBoundingBox ().Center.Y -
				GetBoundingBox ().Center.Y;
			float xyDistance = Math.Max (1f, Math.Abs (xDistance) +
				Math.Abs (yDistance));

			if (xyDistance < 64f)
			{
				xVelocity = Math.Max (-maxSpeed, Math.Min (maxSpeed,
					xVelocity * 1.05f));
				yVelocity = Math.Max (-maxSpeed, Math.Min (maxSpeed,
					yVelocity * 1.05f));
			}

			xDistance /= xyDistance;
			yDistance /= xyDistance;

			if (wasHitCounter.Value <= 0)
			{
				float targetRotation = (float) Math.Atan2 (0f - yDistance,
					xDistance) - (float) Math.PI / 2f;

				if ((double) (Math.Abs (targetRotation) - Math.Abs (rotation))
						> Math.PI * 7.0 / 8.0 && Game1.random.NextDouble () < 0.5)
					turningRight.Value = true;
				else if ((double) (Math.Abs (targetRotation) - Math.Abs (rotation))
						< Math.PI / 8.0)
					turningRight.Value = false;

				float rotationChange = (float) Math.Sign (targetRotation -
					rotation) * ((float) Math.PI / 64f);
				if (turningRight.Value)
					rotation -= rotationChange;
				else
					rotation += rotationChange;
				rotation %= (float) Math.PI * 2f;

				wasHitCounter.Value = 0;
			}

			float factor = Math.Min (maxSpeed, Math.Max (1f, maxSpeed -
				xyDistance / 64f / 2f));
			xDistance = (float) Math.Cos ((double) rotation + Math.PI / 2.0);
			yDistance = 0f - (float) Math.Sin ((double) rotation + Math.PI / 2.0);

			xVelocity += (0f - xDistance) * factor / 6f +
				(float) Game1.random.Next (-10, 10) / 100f;
			if (Math.Abs (xVelocity) > Math.Abs ((0f - xDistance) * maxSpeed))
				xVelocity -= (0f - xDistance) * factor / 6f;

			yVelocity += (0f - yDistance) * factor / 6f +
				(float) Game1.random.Next (-10, 10) / 100f;
			if (Math.Abs (yVelocity) > Math.Abs ((0f - yDistance) * maxSpeed))
				yVelocity -= (0f - yDistance) * factor / 6f;
		}

		protected override void updateAnimation (GameTime time)
		{
			Sprite.Animate (time, 0, 4, 80f);

			if (Sprite.currentFrame % 3 == 0 &&
					Utility.isOnScreen (Position, 512) &&
					!sound.IsPlaying &&
					currentLocation.Equals (Game1.currentLocation))
				Utilities.TryPlayCue (sound);

			resetAnimationSpeed ();
		}
	}
}
