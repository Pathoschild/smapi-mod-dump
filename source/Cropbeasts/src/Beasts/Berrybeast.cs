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
	// Much of this class is based on StardewValley.Monsters.BigSlime.
	public class Berrybeast : Cropbeast
	{
		public enum FaceType
		{
			Blank,
			Eyes,
			Mouth,
			Both,
			Random
		}

		public readonly NetEnum<FaceType> faceType = new NetEnum<FaceType> ();

		private struct SpriteFrame
		{
			public readonly Vector2 offset;
			public readonly Vector2 scale;

			public SpriteFrame (float offsetX, float offsetY,
				float scaleX, float scaleY)
			{
				offset = new Vector2 (offsetX, offsetY);
				scale = new Vector2 (scaleX, scaleY);
			}
		}

		private static readonly SpriteFrame[] SpriteFrames =
			new SpriteFrame[]
		{
			new SpriteFrame (-8f, 12f, 1.333f, 0.833f), // (0,8) 32x20
			new SpriteFrame (-4f,  6f, 1.167f, 0.958f), // (2,5) 28x23
			new SpriteFrame (-2f,  4f, 1.083f, 0.958f), // (3,4) 26x23
			new SpriteFrame ( 0f,  0f, 1.000f, 1.000f), // (4,2) 24x24
			new SpriteFrame ( 4f, -2f, 0.833f, 0.625f), // (6,1) 20x15
			new SpriteFrame ( 0f,  0f, 1.000f, 1.000f), // (4,2) 24x24
			new SpriteFrame (-2f,  4f, 1.083f, 0.958f), // (3,4) 26x23
			new SpriteFrame (-4f,  6f, 1.167f, 0.958f), // (2,5) 28x23
		};

		private int rollDirection = 0;

		public Berrybeast ()
		{}

		public Berrybeast (CropTile cropTile, bool primary)
		: base (cropTile, primary, primary, !primary, "Berrybeast")
		{
			// Choose what facial features will appear on the beast.
			faceType.Value = (Config.BerrybeastFace == FaceType.Random)
				? (FaceType) Game1.random.Next (0, (int) FaceType.Random)
				: Config.BerrybeastFace;

			// Slightly randomize the scale.
			Scale = Game1.random.Next (95, 105) / 100f;

			// The spritesheet stores face types instead of animation
			// frames, so all of this does not apply.
			Sprite.ignoreSourceRectUpdates = true;
			Sprite.ignoreStopAnimation = true;
			ignoreMovementAnimations = true;

			// Set up the sprite class to track animation through the states
			// represented here in SpriteFrames.
			Sprite.framesPerAnimation = 8;
			Sprite.interval = 400f;
		}

		public override int takeDamage (int damage, int xTrajectory,
			int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
		{
			int result = takeDamage (damage, xTrajectory, yTrajectory, isBomb,
				addedPrecision, who, "hitEnemy");
			if (result > -1)
			{
				Slipperiness = 3;
				setTrajectory (xTrajectory, yTrajectory);
				IsWalkingTowardPlayer = true;
			}
			return result;
		}

		public override void shedChunks (int _number, float _scale)
		{}

		public override void draw (SpriteBatch b)
		{
			if (IsInvisible || !Utility.isOnScreen (Position, 128))
				return;

			// Calculate relevant metrics.
			var frame = SpriteFrames[Sprite.currentFrame % 8];
			Vector2 framePosition = getLocalPosition (Game1.viewport) +
				new Vector2 (32f, yJumpOffset) + frame.offset;
			Vector2 frameScale = Math.Max (0.2f, Scale) * 4f * frame.scale;

			// Draw the harvest itself, offset/scaled for the current frame.
			b.Draw (cropTile.mapping.harvestTexture, framePosition,
				cropTile.mapping.harvestSourceRect, ContrastTint, rotation,
				new Vector2 (8f, 8f), frameScale,
				flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				Math.Max (0f, drawOnTop ? 0.991f : getStandingY () / 10000f));

			// Draw any facial features with the same offset/scale.
			b.Draw (Sprite.Texture, framePosition, new Rectangle ((int) faceType.Value *
				Sprite.SpriteWidth, 0, Sprite.SpriteWidth, Sprite.SpriteHeight),
				Color.Lerp (PrecomposedTint, primaryColor.Value, 0.333f), rotation,
				new Vector2 (8f, 8f), frameScale,
				flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				Math.Max (0f, drawOnTop ? 0.992f : getStandingY () / 10000f + 0.001f));
		}

		protected override void localDeathAnimation ()
		{
			base.localDeathAnimation ();
			currentLocation.localSound ("slimedead");
			currentLocation.temporarySprites.Add (new TemporaryAnimatedSprite
				(44, Position, primaryColor.Value, 10, flipped: false, 70f));
			currentLocation.temporarySprites.Add (new TemporaryAnimatedSprite
				(44, Position + new Vector2 (-16f, 0f), primaryColor.Value, 10,
				flipped: false, 70f) { delayBeforeAnimationStart = 100 });
			currentLocation.temporarySprites.Add (new TemporaryAnimatedSprite
				(44, Position + new Vector2 (16f, 0f), primaryColor.Value, 10,
				flipped: false, 70f) { delayBeforeAnimationStart = 200 });
			currentLocation.temporarySprites.Add (new TemporaryAnimatedSprite
				(44, Position + new Vector2 (0f, -16f), primaryColor.Value, 10)
				{ delayBeforeAnimationStart = 300 });
		}

		protected override void updateAnimation (GameTime time)
		{
			int lastFrame = Sprite.currentFrame;
			Sprite.AnimateDown (time);
			Sprite.interval = isMoving () ? 100f : 200f;

			if (Utility.isOnScreen (Position, 128) &&
					Sprite.currentFrame == 0 && lastFrame == 7)
				currentLocation.localSound ("slimeHit");

			resetAnimationSpeed ();
		}

		private int flipCoin ()
		{
			return (new int[] { -1, 1 })[Game1.random.Next (2)];
		}

		public override void behaviorAtGameTick (GameTime time)
		{
			base.behaviorAtGameTick (time);

			// Occasionally change speed when hitting the ground.
			if (Sprite.currentFrame == 0 && rollDirection == 0 &&
				Game1.random.NextDouble () < 0.01)
			{
				// Choose the direction.
				int speedChange = (addedSpeed < 0) ? 1
					: (addedSpeed > 1) ? -1
					: flipCoin ();

				// Either slow down or speed up slightly.
				addedSpeed += speedChange;

				// Set an appropriate roll direction.
				if ((speedChange == 1 && moveLeft) ||
						(speedChange == -1 && moveRight))
					rollDirection = -1;
				else if ((speedChange == 1 && moveRight) ||
						(speedChange == -1 && moveLeft))
					rollDirection = 1;
				else
					rollDirection = (flipCoin () == -1) ? flipCoin () : 0;
			}

			// Start or continue a little cartwheel.
			if (rollDirection != 0)
			{
				double phase = Math.Abs (rotation) / (2.0 * Math.PI);
				double delta = 20.5 - Math.Pow ((phase - 0.5) * 9.0, 2.0);
				rotation += rollDirection * (float) (2.0 * Math.PI * delta / 360.0);
				if (Math.Abs (rotation) >= (float) (2.0 * Math.PI))
				{
					rotation = 0f;
					rollDirection = 0;
				}
			}
		}
	}
}
