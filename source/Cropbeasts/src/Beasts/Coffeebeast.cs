using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using System;

namespace Cropbeasts.Beasts
{
	// Much of this class is based on StardewValley.Monsters.DustSpirit.
	public class Coffeebeast : Cropbeast
	{
		private bool runningAwayFromFarmer = false;
		private bool chargingFarmer = false;

		public NetByte voice = new NetByte ();
		private ICue meep;

		public Coffeebeast ()
		{
			meep = Game1.soundBank.GetCue ("dustMeep");
		}

		public Coffeebeast (CropTile cropTile, bool primary)
		: base (cropTile, primary, primary, !primary)
		{
			IsWalkingTowardPlayer = false;
			chargingFarmer = Game1.random.NextDouble () < 0.8;
			Sprite.interval = 100f;
			HideShadow = true;

			// Randomly vary the size and meeping pitch.
			Scale = Game1.random.Next (75, 101) / 100f;
			meep = Game1.soundBank.GetCue ("dustMeep");
			voice.Value = (byte) Game1.random.Next (1, 24);
		}

		public override int takeDamage (int damage, int xTrajectory,
			int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
		{
			int result = takeDamage (damage, xTrajectory, yTrajectory, isBomb,
				addedPrecision, who, "tinyWhip");
			if (result > -1)
				runAway ();
			return result;
		}

		public override void draw (SpriteBatch b)
		{
			if (IsInvisible || !Utility.isOnScreen (Position, 128))
				return;

			b.Draw (Sprite.Texture, getLocalPosition (Game1.viewport) +
				new Vector2 (32f, 64 + yJumpOffset), Sprite.SourceRect,
				PrecomposedTint, rotation, new Vector2 (8f, 16f), new Vector2
					(Scale + (float) Math.Max (-0.1, (yJumpOffset + 32) / 128.0),
					Scale - Math.Max (-0.1f, yJumpOffset / 256f)) * 4f,
				SpriteEffects.None, Math.Max (0f, drawOnTop ? 0.991f
					: getStandingY() / 10000f));

			if (isGlowing)
			{
				b.Draw (Sprite.Texture, getLocalPosition(Game1.viewport) +
					new Vector2 (32f, 64 + yJumpOffset), Sprite.SourceRect,
					glowingColor * glowingTransparency, rotation,
					new Vector2 (8f, 16f), Math.Max (0.2f, Scale) * 4f,
					SpriteEffects.None, Math.Max (0f, drawOnTop ? 0.99f
						: (getStandingY () / 10000f + 0.001f)));
			}

			b.Draw (Game1.shadowTexture, getLocalPosition (Game1.viewport) +
				new Vector2 (32f, 80f), Game1.shadowTexture.Bounds, Color.White,
				0f, new Vector2 (Game1.shadowTexture.Bounds.Center.X,
					Game1.shadowTexture.Bounds.Center.Y),
				4f + yJumpOffset / 64f, SpriteEffects.None,
				(getStandingY () - 1) / 10000f);
		}

		protected override void localDeathAnimation ()
		{
			base.localDeathAnimation ();

			currentLocation.localSound ("dustMeep");

			currentLocation.temporarySprites.Add (new TemporaryAnimatedSprite
				(44, Position, primaryColor.Value, 10));
			currentLocation.temporarySprites.Add (new TemporaryAnimatedSprite
				(44, Position + new Vector2 (Game1.random.Next (-32, 32),
					Game1.random.Next (-32, 32)), secondaryColor.Value, 10)
				{
					delayBeforeAnimationStart = 150,
					scale = 0.5f,
				});
			currentLocation.temporarySprites.Add (new TemporaryAnimatedSprite
				(44, Position + new Vector2 (Game1.random.Next (-32, 32),
					Game1.random.Next (-32, 32)), primaryColor.Value, 10)
				{
					delayBeforeAnimationStart = 300,
					scale = 0.5f,
				});
			currentLocation.temporarySprites.Add (new TemporaryAnimatedSprite
				(44, Position + new Vector2 (Game1.random.Next (-32, 32),
					Game1.random.Next (-32, 32)), secondaryColor.Value, 10)
				{
					delayBeforeAnimationStart = 450,
					scale = 0.5f,
				});
		}

		protected override void updateAnimation (GameTime time)
		{
			Sprite.AnimateDown (time);
			if (yJumpOffset == 0)
			{
				jumpWithoutSound ();
				yJumpVelocity = Game1.random.Next (50, 70) / 10f;
				if (Game1.random.NextDouble () < 0.1 &&
					!meep.IsPlaying &&
					Utility.isOnScreen (Position, 64) &&
					currentLocation.Equals (Game1.currentLocation))
				{
					meep.SetVariable ("Pitch", voice.Value * 100 +
						Game1.random.Next (-100, 100));
					Utilities.TryPlayCue (meep);
				}
			}
			resetAnimationSpeed ();
		}

		public override void behaviorAtGameTick (GameTime time)
		{
			base.behaviorAtGameTick (time);
			if (yJumpOffset == 0)
			{
				if (Game1.random.NextDouble () < 0.05)
				{
					Utilities.MP.broadcastSprites (currentLocation,
						new TemporaryAnimatedSprite ("TileSheets\\animations",
							new Rectangle (0, 128, 64, 64), 40f, 4, 0,
							getStandingPosition (), false, false)
							{ color = primaryColor.Value });
					yJumpVelocity *= 2f;
				}
				if (!chargingFarmer)
					xVelocity = Game1.random.Next (-20, 21) / 5f;
			}
			if (chargingFarmer)
			{
				Slipperiness = 10;
				Vector2 trajectory = Utility.getAwayFromPlayerTrajectory
					(GetBoundingBox(), Player);

				xVelocity += (0f - trajectory.X) / 150f +
					((Game1.random.NextDouble () < 0.01)
						? Game1.random.Next (-50, 50) / 10f : 0f);
				if (Math.Abs (xVelocity) > 5f)
					xVelocity = Math.Sign(xVelocity) * 5;

				yVelocity += (0f - trajectory.Y) / 150f +
					((Game1.random.NextDouble () < 0.01)
						? Game1.random.Next (-50, 50) / 10f : 0f);
				if (Math.Abs(yVelocity) > 5f)
					yVelocity = Math.Sign(yVelocity) * 5;

				if (Game1.random.NextDouble () < 0.0001)
				{
					controller = new PathFindController (this, currentLocation,
						new Point ((int) Player.getTileLocation ().X,
							(int) Player.getTileLocation ().Y),
						Game1.random.Next (4), null, 300);
					chargingFarmer = false;
				}
			}
			else if (controller == null && !runningAwayFromFarmer)
			{
				runAway ();
			}
			else if (controller == null && runningAwayFromFarmer)
			{
				chargingFarmer = true;
			}
		}

		private void runAway ()
		{
			addedSpeed = 2;
			controller = new PathFindController (this, currentLocation,
				Utility.isOffScreenEndFunction, -1,
				eraseOldPathController: false, delegate {}, 350,
				Point.Zero);
			chargingFarmer = false;
			runningAwayFromFarmer = true;
		}
	}
}
