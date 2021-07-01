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
	public class Starbeast : Cropbeast
	{
		private int firingCooldown = 0;

		private new int yOffset = 0;

		private readonly NetEvent0 firedEvent = new ();

		private readonly NetEvent0 hurtAnimationEvent = new ();

		public Starbeast ()
		{ }

		public Starbeast (CropTile cropTile, bool primary)
		: base (cropTile, primary, primary, !primary)
		{
			IsWalkingTowardPlayer = false;
			HideShadow = true;

			Sprite.SpriteHeight = 16;
			Sprite.UpdateSourceRect ();
		}

		protected override void initNetFields ()
		{
			base.initNetFields ();
			NetFields.AddFields (firedEvent, hurtAnimationEvent);
			firedEvent.onEvent += delegate
			{
				if (!Game1.IsMasterGame)
					onFired ();
			};
			hurtAnimationEvent.onEvent += delegate
			{
				Sprite.currentFrame = Sprite.currentFrame -
					Sprite.currentFrame % 4 + 3;
			};
		}

		public override int takeDamage (int damage, int xTrajectory,
			int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
		{
			int result = takeDamage (damage, xTrajectory, yTrajectory, isBomb,
				addedPrecision, who, "slimeHit");
			if (result > -1)
			{
				setTrajectory (xTrajectory / 2, yTrajectory / 2);
				hurtAnimationEvent.Fire ();
			}
			return result;
		}

		protected override void localDeathAnimation ()
		{
			base.localDeathAnimation ();

			currentLocation.localSound ("slimedead");

			currentLocation.temporarySprites.Add (new TemporaryAnimatedSprite
				(Sprite.textureName.Value, new Rectangle (0, 64, 16, 16), 200f,
				4, 0, Position + new Vector2 (0f, -32f), flicker: false,
				flipped: false)
			{ scale = 4f });
			currentLocation.temporarySprites.Add (new TemporaryAnimatedSprite
				(362, 30f, 6, 1, Position + new Vector2 (-16 + Game1.random.Next (64),
					Game1.random.Next (64) - 32),
				flicker: false, Game1.random.NextDouble () < 0.5)
			{ delayBeforeAnimationStart = 100, color = primaryColor.Value });
			currentLocation.temporarySprites.Add (new TemporaryAnimatedSprite
				(362, 30f, 6, 1, Position + new Vector2 (-16 + Game1.random.Next (64),
					Game1.random.Next (64) - 32),
				flicker: false, Game1.random.NextDouble () < 0.5)
			{ delayBeforeAnimationStart = 200, color = primaryColor.Value });
			currentLocation.temporarySprites.Add (new TemporaryAnimatedSprite
				(362, 30f, 6, 1, Position + new Vector2 (-16 + Game1.random.Next (64),
					Game1.random.Next (64) - 32),
				flicker: false, Game1.random.NextDouble () < 0.5)
			{ delayBeforeAnimationStart = 300, color = primaryColor.Value });
			currentLocation.temporarySprites.Add (new TemporaryAnimatedSprite
				(362, 30f, 6, 1, Position + new Vector2 (-16 + Game1.random.Next (64),
					Game1.random.Next (64) - 32),
				flicker: false, Game1.random.NextDouble () < 0.5)
			{ delayBeforeAnimationStart = 400, color = primaryColor.Value });
		}

		public override void drawAboveAllLayers (SpriteBatch b)
		{
			b.Draw (Sprite.Texture, getLocalPosition (Game1.viewport) +
				new Vector2 (32f, 21 + yOffset), Sprite.SourceRect, Color.White,
				0f, new Vector2 (8f, 16f), Math.Max (0.2f, Scale) * 4f,
				flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				Math.Max (0f, drawOnTop ? 0.991f
					: ((float) getStandingY () / 10000f)));

			b.Draw (Game1.shadowTexture, getLocalPosition (Game1.viewport) +
				new Vector2 (32f, 64f), Game1.shadowTexture.Bounds, Color.White,
				0f, new Vector2 (Game1.shadowTexture.Bounds.Center.X,
					Game1.shadowTexture.Bounds.Center.Y),
				3f + (float) yOffset / 20f, SpriteEffects.None,
				(float) (getStandingY () - 1) / 10000f);
		}

		protected override void updateAnimation (GameTime time)
		{
			base.updateAnimation (time);
			yOffset = (int) (Math.Sin ((double) ((float)
				time.TotalGameTime.Milliseconds / 2000f) * (Math.PI * 2.0)) * 15.0);

			if (Sprite.currentFrame % 4 != 0 && Game1.random.NextDouble () < 0.1)
				Sprite.currentFrame -= Sprite.currentFrame % 4;

			if (Game1.random.NextDouble () < 0.01)
				Sprite.currentFrame++;

			resetAnimationSpeed ();
		}

		protected override void updateMonsterSlaveAnimation (GameTime time)
		{
			if (isMoving ())
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
			faceGeneralDirection (Player.Position);
		}

		private Vector2 onFired ()
		{
			switch (FacingDirection)
			{
			case 0:
				Sprite.currentFrame = 3;
				return Vector2.Zero;
			case 1:
				Sprite.currentFrame = 7;
				return new Vector2 (64f, 0f);
			case 2:
				Sprite.currentFrame = 11;
				return new Vector2 (0f, 32f);
			case 3:
				Sprite.currentFrame = 15;
				return new Vector2 (-32f, 0f);
			default:
				return Vector2.Zero;
			}
		}

		public override void update (GameTime time, GameLocation location)
		{
			base.update (time, location);
			firedEvent.Poll ();
		}

		public override void behaviorAtGameTick (GameTime time)
		{
			base.behaviorAtGameTick (time);

			faceGeneralDirection (Player.Position);

			firingCooldown = Math.Max (0,
				firingCooldown - time.ElapsedGameTime.Milliseconds);

			bool inRange = withinPlayerThreshold (moveTowardPlayerThreshold);

			if (inRange && firingCooldown <= 0 &&
				Game1.random.NextDouble () < 0.01)
			{
				IsWalkingTowardPlayer = false;
				Halt ();

				firedEvent.Fire ();
				onFired ();
				Sprite.UpdateSourceRect ();

				int damage = scaleByCombatSkill (10, 0.1f);
				Vector2 velocityTowardPlayer = Utility.getVelocityTowardPlayer
					(Utility.Vector2ToPoint (getStandingPosition ()), 8f, Player);
				var projectile = new Projectiles.Starburst (damage);
				projectile.fire (this, currentLocation, getStandingPosition (),
					velocityTowardPlayer);

				firingCooldown = Game1.random.Next (2500, 4000);
				lastProjectileHit = null;
			}
			else if (withinPlayerThreshold (2) && Game1.random.NextDouble () < 0.05)
			{
				Vector2 trajectory = Utility.getAwayFromPlayerTrajectory
					(GetBoundingBox (), Player) / 4f;
				setTrajectory ((int) trajectory.X, (int) trajectory.Y);
			}
			else if (Game1.random.NextDouble () < ((firingCooldown > 0) ? 0.05 : 0.01))
			{
				Halt ();
				if (!inRange || (!withinPlayerThreshold (4) &&
					(lastProjectileHit == false || Game1.random.NextDouble () < 0.1)))
				{
					Slipperiness = 8;
					float speed = ((firingCooldown > 0) ? 4f : 8f)
						* (1f + (float) Game1.random.NextDouble ());
					Vector2 velocity = Utility.getVelocityTowardPlayer
						(Utility.Vector2ToPoint (getStandingPosition ()),
						speed, Player);
					setTrajectory ((int) velocity.X, -1 * (int) velocity.Y);
				}
			}
		}
	}
}
