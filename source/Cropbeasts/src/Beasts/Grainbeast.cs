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
	// Much of this class is based on StardewValley.Monsters.Skeleton.
	public class Grainbeast : Cropbeast
	{
		private readonly NetBool throwing = new (false);

		private readonly NetFloat legRotation = new (0f);

		private long stepSoundCooldown = 0L;

		public Grainbeast ()
		{ }

		public Grainbeast (CropTile cropTile, bool primary)
		: base (cropTile, primary, primary, cropTile.spawnSecondaries ? !primary : primary)
		{
			Sprite.SpriteWidth = 18;
			Sprite.SpriteHeight = 48;
			Sprite.UpdateSourceRect ();
		}

		protected override void initNetFields ()
		{
			base.initNetFields ();
			NetFields.AddFields (throwing, legRotation);
			position.Field.AxisAlignedMovement = true;
		}

		public override Rectangle GetBoundingBox ()
		{
			return new Rectangle ((int) Position.X + 8, (int) Position.Y - 16,
				Sprite.SpriteWidth * 3, 64);
		}

		public override int takeDamage (int damage, int xTrajectory,
			int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
		{
			int result = takeDamage (damage, xTrajectory, yTrajectory, isBomb,
				addedPrecision, who, "cut");
			shake (750);
			Slipperiness = 3;
			if (throwing.Value)
			{
				throwing.Value = false;
				Halt ();
			}
			return result;
		}

		protected override void sharedDeathAnimation ()
		{
			base.sharedDeathAnimation ();
			currentLocation.playSound ("dirtyHit");
			shedChunks (10, 0.75f);
		}

		public override void draw (SpriteBatch b)
		{
			if (IsInvisible || !Utility.isOnScreen (Position, 128))
				return;

			Rectangle csr = cropTile.cropSourceRect;
			Color tint = Color.Lerp (ContrastTint, Color.Goldenrod,
				wateringCharges.Value * 1f / MaxWateringCharges);

			int shakeMag = Math.Max ((shakeTimer > 0) ? 1 : 0, shakeTimer / 187);
			Vector2 shakeOffset = new (Game1.random.Next (-2 * shakeMag, 2 * shakeMag + 1),
				Game1.random.Next (-shakeMag, shakeMag + 1));

			// Draw the "head".
			Rectangle head = new (csr.X, csr.Y, csr.Width, csr.Height / 4);
			b.Draw (cropTile.cropTexture, getLocalPosition (Game1.viewport) +
				new Vector2 (Sprite.SpriteWidth * 2f, GetBoundingBox ().Height / 2) +
				shakeOffset, head, tint, 0f, new Vector2 (csr.Width * 0.5f,
				csr.Height * 1.25f), Math.Max (0.2f, Scale) * 4f * new Vector2 (1.1f, 1f),
				(FacingDirection < 2) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				Math.Max (0f, drawOnTop ? 0.992f : getStandingY () / 10000f + 0.002f));

			// Draw the "body".
			Rectangle body = new (csr.X, csr.Y + csr.Height / 4,
				csr.Width, csr.Height / 2);
			b.Draw (cropTile.cropTexture, getLocalPosition (Game1.viewport) +
				new Vector2 (Sprite.SpriteWidth * 2f, GetBoundingBox ().Height / 2) +
				shakeOffset, body, tint, 0f, new Vector2 (csr.Width * 0.5f,
				csr.Height / 2f), Math.Max (0.2f, Scale) * 4f * new Vector2 (1.1f, 2f),
				(FacingDirection < 2) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				Math.Max (0f, drawOnTop ? 0.992f : getStandingY () / 10000f + 0.002f));

			// Draw the "legs".
			Rectangle legs = new (csr.X, csr.Y + csr.Height * 5 / 8,
				csr.Width, csr.Height * 3 / 8);
			b.Draw (cropTile.cropTexture, getLocalPosition (Game1.viewport) +
				new Vector2 (Sprite.SpriteWidth * 2f, GetBoundingBox ().Height / 2) +
				shakeOffset, legs, tint, legRotation, new Vector2 (csr.Width * 0.5f,
				csr.Height / 8f), Math.Max (0.2f, Scale) * 4f * new Vector2 (1.1f, 1f),
				(FacingDirection < 2) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				Math.Max (0f, drawOnTop ? 0.992f : getStandingY () / 10000f + 0.002f));
		}

		public override void MovePosition (GameTime time,
			xTile.Dimensions.Rectangle viewport, GameLocation currentLocation)
		{
			base.MovePosition (time, viewport, currentLocation);

			if (!Position.Equals (lastPosition) &&
				(stepSoundCooldown == 0L || updateTimer >= stepSoundCooldown))
			{
				currentLocation.playSound ("grassyStep");
				stepSoundCooldown = updateTimer + 500L;
			}
		}

		public override void update (GameTime time, GameLocation location)
		{
			if (!throwing.Value)
			{
				base.update (time, location);
				return;
			}

			if (Game1.IsMasterGame)
				behaviorAtGameTick (time);
			updateAnimation (time);
		}

		protected override void updateAnimation (GameTime time)
		{
			base.updateAnimation (time);
			if (isMoving ())
			{
				legRotation.Value = (float) Math.Sin (Math.PI *
					time.TotalGameTime.Milliseconds / 250.0) * 10f / 360f *
					2f * (float) Math.PI;
			}
		}

		protected override void updateMonsterSlaveAnimation (GameTime time)
		{
			if (throwing.Value)
			{
				if (invincibleCountdown > 0)
				{
					invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
					if (invincibleCountdown <= 0)
						stopGlowing ();
				}
				Sprite.Animate (time, 20, 5, 50f);
				if (Sprite.currentFrame == 24)
					Sprite.currentFrame = 23;
			}
			else if (isMoving ())
			{
				switch (FacingDirection)
				{
				case 0:
					Sprite.AnimateUp (time);
					break;
				case 3:
					Sprite.AnimateLeft (time);
					break;
				case 1:
					Sprite.AnimateRight (time);
					break;
				case 2:
					Sprite.AnimateDown (time);
					break;
				}
			}
			else
			{
				Sprite.StopAnimation ();
			}
		}

		public override void behaviorAtGameTick (GameTime time)
		{
			base.behaviorAtGameTick (time);

			// Advance an in-progress throwing animation.
			if (throwing.Value)
			{
				if (invincibleCountdown > 0)
				{
					invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
					if (invincibleCountdown <= 0)
						stopGlowing ();
				}
				Sprite.Animate (time, 20, 5, 50f);
				// If the animation is complete, fire the projectile.
				if (Sprite.currentFrame == 24)
				{
					throwing.Value = false;
					Sprite.currentFrame = 0;
					faceDirection (2);

					int damage = scaleByCombatSkill (4, 0.4f);
					Vector2 velocityTowardPlayer = Utility.getVelocityTowardPlayer
						(Utility.Vector2ToPoint (getStandingPosition ()), 12f, Player);
					var projectile = new Projectiles.SeedShot (damage, cropTile.seedIndex);
					projectile.fire (this, currentLocation, getStandingPosition (),
						velocityTowardPlayer);
				}
			}
			// Don't get too close to the player.
			else if (isFleeing || withinPlayerThreshold (3))
			{
				fleePlayer (time);
			}
			// Maybe start a throwing animation if there is line of sight.
			else if (Game1.random.NextDouble () < 0.01 &&
				Utility.doesPointHaveLineOfSightInMine (currentLocation,
					getTileLocation (), Player.getTileLocation (),
					2 * moveTowardPlayerThreshold))
			{
				throwing.Value = true;
				Halt ();
				Sprite.currentFrame = 20;
				shake (125);
				DelayedAction.playSoundAfterDelay ("coin", 125, currentLocation);
			}
			// Approach the player if too far away.
			else
			{
				IsWalkingTowardPlayer =
					!withinPlayerThreshold (moveTowardPlayerThreshold);
			}
		}
	}
}
