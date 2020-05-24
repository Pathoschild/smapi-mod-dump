using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using System;

namespace Cropbeasts.Beasts
{
	// Much of this class is based on StardewValley.Monsters.DinoMonster.
	public class Cactusbeast : Cropbeast
	{
		public readonly NetBool attacking = new NetBool (false);
		public int timeUntilNextAttack = 0;

		public readonly NetBool firing = new NetBool (false);
		public int nextFireTime = 0;
		public int totalFireTime = 0;

		public readonly NetBool wandering = new NetBool (false);
		public int nextWanderTime = 0;
		public int nextChangeDirectionTime = 0;

		public Cactusbeast ()
		{}

		public Cactusbeast (CropTile cropTile, bool primary)
		: base (cropTile, primary, primary, !primary)
		{
			Sprite.SpriteWidth = 32;
			Sprite.SpriteHeight = 32;
			Sprite.UpdateSourceRect ();

			timeUntilNextAttack = 2000;
			nextChangeDirectionTime = Game1.random.Next (1000, 3000);
			nextWanderTime = Game1.random.Next (1000, 2000);
		}

		protected override void initNetFields ()
		{
			base.initNetFields ();
			NetFields.AddFields (attacking, firing, wandering);
		}

		public override int takeDamage (int damage, int xTrajectory,
			int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
		{
			int result = takeDamage (damage, xTrajectory, yTrajectory, isBomb,
				addedPrecision, who, "hitEnemy");
			if (result > -1)
			{
				// Damage is returned nearly in kind, depending on distance.
				float distance = Vector2.Distance (Position, who.Position);
				int scaledDamage = (int) (result * Math.Max (0.1f, Math.Min (0.8f,
					1f - Math.Pow (distance / 10f, 2f))));
				if (scaledDamage > 0)
					who.takeDamage (scaledDamage, true, this);
			}
			return result;
		}

		protected override void localDeathAnimation ()
		{
			base.localDeathAnimation ();

			currentLocation.localSound ("monsterdead");
			currentLocation.localSound ("grunt");

			Utility.makeTemporarySpriteJuicier (new TemporaryAnimatedSprite (44,
				Position, primaryColor.Value, 10) { holdLastFrame = true,
				alphaFade = 0.01f, interval = 70f }, currentLocation, 8, 96);
		}

		public override void draw (SpriteBatch b)
		{
			if (IsInvisible || !Utility.isOnScreen (Position, 128))
				return;

			b.Draw (Sprite.Texture, getLocalPosition (Game1.viewport) +
				new Vector2 (56f, 16 + yJumpOffset), Sprite.SourceRect,
				Color.White, rotation, new Vector2 (16f, 16f),
				Math.Max (0.2f, scale) * 4f,
				flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				Math.Max (0f, drawOnTop ? 0.991f : getStandingY () / 10000f));

			if (isGlowing)
			{
				b.Draw (Sprite.Texture, getLocalPosition (Game1.viewport) +
					new Vector2 (56f, 16 + yJumpOffset), Sprite.SourceRect,
					glowingColor * glowingTransparency, 0f, new Vector2 (16f, 16f),
					Math.Max (0.2f, scale) * 4f,
					flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
					Math.Max (0f, drawOnTop ? 0.991f : getStandingY () / 10000f +
						0.001f));
			}
		}

		public override Rectangle GetBoundingBox ()
		{
			return new Rectangle ((int) Position.X + 8, (int) Position.Y,
				Sprite.SpriteWidth * 4 * 3 / 4, 64);
		}

		public override void behaviorAtGameTick (GameTime time)
		{
			if (attacking.Value)
			{
				IsWalkingTowardPlayer = false;
				Halt ();
			}
			else if (withinPlayerThreshold ())
			{
				IsWalkingTowardPlayer = true;
			}
			else
			{
				IsWalkingTowardPlayer = false;
				nextChangeDirectionTime -= time.ElapsedGameTime.Milliseconds;
				nextWanderTime -= time.ElapsedGameTime.Milliseconds;
				if (nextChangeDirectionTime < 0)
				{
					nextChangeDirectionTime = Game1.random.Next (500, 1000);
					facingDirection.Value = (facingDirection.Value +
						(Game1.random.Next(0, 3) - 1) + 4) % 4;
				}
				if (nextWanderTime < 0)
				{
					nextWanderTime = Game1.random.Next (1000,
						wandering.Value ? 2000 : 3000);
					wandering.Value = !wandering.Value;
				}
				if (wandering.Value)
				{
					moveLeft = moveUp = moveRight = moveDown = false;
					tryToMoveInDirection (facingDirection.Value, isFarmer: false,
						DamageToFarmer, isGlider);
				}
			}

			timeUntilNextAttack -= time.ElapsedGameTime.Milliseconds;

			if (!attacking.Value && withinPlayerThreshold (4))
			{
				firing.Value = false;
				if (timeUntilNextAttack < 0)
				{
					timeUntilNextAttack = 0;
					attacking.Value = true;
					nextFireTime = 500;
					totalFireTime = 2000;
					currentLocation.playSound ("croak");
				}
				return;
			}

			if (totalFireTime <= 0)
				return;

			if (!firing && Player != null)
				faceGeneralDirection (Player.Position);

			totalFireTime -= time.ElapsedGameTime.Milliseconds;
			if (nextFireTime > 0)
			{
				nextFireTime -= time.ElapsedGameTime.Milliseconds;
				if (nextFireTime <= 0)
				{
					if (!firing.Value)
						firing.Value = true;

					float angle = 0f;
					Vector2 startingPosition = Utility.PointToVector2
						(GetBoundingBox ().Center) - new Vector2 (32f, 32f);
					switch (facingDirection.Value)
					{
					case 0:
						yVelocity = -1f;
						startingPosition.Y -= 64f;
						angle = 90f;
						break;
					case 1:
						xVelocity = -1f;
						startingPosition.X += 64f;
						angle = 0f;
						break;
					case 3:
						xVelocity = 1f;
						startingPosition.X -= 64f;
						angle = 180f;
						break;
					case 2:
						yVelocity = 1f;
						angle = 270f;
						break;
					}
					angle += (float) Math.Sin (totalFireTime / 1000.0 * 180.0 *
						Math.PI / 180.0) * 25f;
					Vector2 vector = new Vector2 ((float) Math.Cos (angle * Math.PI / 180.0),
						0f - (float) Math.Sin (angle * Math.PI / 180.0)) * 10f;

					int damage = scaleByCombatSkill (1, 0.1f);
					var projectile = new Projectiles.Sandblast (damage);
					projectile.fire (this, currentLocation, startingPosition,
						vector, (float) Math.PI / 16f);

					nextFireTime = 50;
				}
			}

			if (totalFireTime <= 0)
			{
				totalFireTime = 0;
				nextFireTime = 0;
				attacking.Value = false;
				timeUntilNextAttack = Game1.random.Next (750, 1500);
			}
		}

		protected override void updateAnimation (GameTime time)
		{
			if (attacking.Value)
			{
				Sprite.CurrentFrame = 16 + (firing.Value ? 0 : 1) +
					FacingDirection switch
					{
						2 => 0,
						1 => 4,
						0 => 8,
						3 => 12,
						_ => 0,
					};
			}
			else
			{
				switch (FacingDirection)
				{
				case 0:
					Sprite.AnimateUp (time);
					break;
				case 1:
					Sprite.AnimateRight (time);
					break;
				case 2:
					Sprite.AnimateDown (time);
					break;
				case 3:
					Sprite.AnimateLeft (time);
					break;
				}
				if (!isMoving () && !wandering.Value)
					Sprite.StopAnimation ();
			}
		}
	}
}
