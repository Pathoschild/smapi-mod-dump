/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/cropbeasts
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using System;

namespace Cropbeasts.Beasts
{
	// Much of this class is based on StardewValley.Monsters.Fly.
	public class Leafbeast : Cropbeast
	{
		public const float maxSpeed = 7f;

		private readonly NetInt wasHitCounter = new NetInt (0);
		private readonly NetBool turningRight = new NetBool ();

		private ICue sound;

		public Leafbeast ()
		{
			sound = Game1.soundBank.GetCue ("batFlap");
		}

		public Leafbeast (CropTile cropTile, bool primary)
		: base (cropTile, primary, primary, !primary)
		{
			Slipperiness = 18 + Game1.random.Next (-8, 8);
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
				isBomb, addedPrecision, who, "harvest");
			if (result > -1)
				wasHitCounter.Value = 500;
			addedSpeed = Game1.random.Next (-1, 1);
			return result;
		}

		protected override void localDeathAnimation ()
		{
			base.localDeathAnimation ();

			stopGlowing ();

			Utilities.TryStopCue (sound);
			currentLocation.localSound ("monsterdead");

			Utility.makeTemporarySpriteJuicier (new TemporaryAnimatedSprite (50,
				Position, primaryColor.Value, 8) { holdLastFrame = true,
				interval = 70f, alpha = 0.8f, alphaFade = 0.01f },
				currentLocation, 2, 32, 32);
		}

		public override void drawAboveAllLayers (SpriteBatch b)
		{
			if (!Utility.isOnScreen (Position, 128))
				return;

			// Draw the base sprite with the leaf-wings.
			b.Draw (Sprite.Texture, getLocalPosition (Game1.viewport) +
				new Vector2 (32f, GetBoundingBox ().Height / 2f - 32f),
				Sprite.SourceRect, secondaryColor.Value, rotation,
				new Vector2 (8f, 16f), Math.Max (0.2f, Scale) * 4f,
				flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				0.9999f);

			// Draw the harvest itself.
			b.Draw (cropTile.mapping.harvestTexture, getLocalPosition (Game1.viewport) +
				new Vector2 (32f, GetBoundingBox ().Height / 2f - 32f),
				cropTile.mapping.harvestSourceRect, ContrastTint, rotation,
				new Vector2 (8f, 8f), Math.Max (0.2f, Scale) * 3.3f,
				flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);

			// Draw the shadow.
			b.Draw (Game1.shadowTexture, getLocalPosition (Game1.viewport) +
				new Vector2 (32f, GetBoundingBox ().Height / 2f),
				Game1.shadowTexture.Bounds, Color.White, 0f,
				new Vector2 (Game1.shadowTexture.Bounds.Center.X,
					Game1.shadowTexture.Bounds.Center.Y),
				4f, SpriteEffects.None, (getStandingY () - 1) / 10000f);
		}

		public override void drawAboveAlwaysFrontLayer (SpriteBatch b)
		{
			if (currentLocation != null && currentLocation.treatAsOutdoors.Value)
				drawAboveAllLayers (b);
		}

		protected override void updateAnimation (GameTime time)
		{
			if (!sound.IsPlaying && currentLocation.Equals (Game1.currentLocation))
			{
				sound.SetVariable ("Volume", 0f);
				Utilities.TryPlayCue (sound);
			}
			if (Game1.fadeToBlackAlpha > 0.8f && Game1.fadeIn)
			{
				Utilities.TryStopCue (sound);
			}
			else
			{
				sound.SetVariable ("Volume",
					Math.Max (0f, sound.GetVariable ("Volume") - 1f));
				float max = Math.Max (0f, 400f - Vector2.Distance (Position,
					Player.Position) / 64f / 16f * 150f);
				if (max > sound.GetVariable("Volume"))
					sound.SetVariable ("Volume", max);
			}

			if (wasHitCounter.Value >= 0)
				wasHitCounter.Value -= time.ElapsedGameTime.Milliseconds;

			Sprite.Animate (time, (FacingDirection == 0) ? 8
				: ((FacingDirection != 2) ? (FacingDirection * 4) : 0), 4, 50f);

			if ((withinPlayerThreshold () || Utility.isOnScreen (position, 256))
				&& invincibleCountdown <= 0)
			{
				faceDirection (0);

				float xDistance = -(Player.GetBoundingBox ().Center.X -
					GetBoundingBox ().Center.X);
				float yDistance = Player.GetBoundingBox ().Center.Y -
					GetBoundingBox ().Center.Y;
				float xyDistance = Math.Max (1f, Math.Abs (xDistance) +
					Math.Abs (yDistance));

				if (xyDistance < 64f)
				{
					xVelocity = Math.Max (-maxSpeed, Math.Min (maxSpeed,
						xVelocity * 1.1f));
					yVelocity = Math.Max (-maxSpeed, Math.Min (maxSpeed,
						yVelocity * 1.1f));
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

					wasHitCounter.Value = 5 + Game1.random.Next (-1, 2);
				}

				float factor = Math.Min (maxSpeed, Math.Max (2f, maxSpeed -
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
			resetAnimationSpeed ();
		}

		public override void behaviorAtGameTick(GameTime time)
		{
			base.behaviorAtGameTick (time);

			if (withinPlayerThreshold (moveTowardPlayerThreshold))
			{
				if (!isGlowing)
					startGlowing (primaryColor.Value, border: true, 0.25f);
			}
			else
			{
				if (isGlowing)
					stopGlowing ();
			}

			if (double.IsNaN (xVelocity) || double.IsNaN (yVelocity))
				Health = -500;
		}

		public override void Removed ()
		{
			base.Removed ();
			Utilities.TryStopCue (sound);
		}
	}
}
