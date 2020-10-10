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
using StardewValley;
using System;

namespace Cropbeasts.Beasts
{
	// Much of this class is based on StardewValley.Monsters.RockCrab.
	public class Rootbeast : Cropbeast
	{
		private int startStopCooldown = 0;

		public Rootbeast ()
		{}

		public Rootbeast (CropTile cropTile, bool primary)
		: base (cropTile, primary, primary, !primary)
		{
			flip = cropTile.crop.flip.Value;
			addedSpeed = Game1.random.Next (3);
			HideShadow = true;
		}

		public override void spawn ()
		{
			base.spawn ();
			if (Config.RootbeastHiding)
				focusedOnFarmers = false;
		}

		protected override void initNetFields ()
		{
			base.initNetFields ();
			position.Field.AxisAlignedMovement = true;
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
				glowingColor = secondaryColor.Value;
				startStopCooldown = 500;
			}
			return result;
		}

		protected override void localDeathAnimation ()
		{
			base.localDeathAnimation ();

			stopGlowing ();

			currentLocation.localSound ("monsterdead");

			Utility.makeTemporarySpriteJuicier (new TemporaryAnimatedSprite (44,
				Position, primaryColor.Value, 10) { holdLastFrame = true,
				alphaFade = 0.01f }, currentLocation);
		}

		public bool isHiding => Sprite.currentFrame < 16 &&
			Sprite.currentFrame % 4 == 0;

		public override void draw (SpriteBatch b)
		{
			if (IsInvisible || !Utility.isOnScreen (Position, 128))
				return;

			// Don't show the hiding frame if not configured or while moving.
			if (Sprite.currentFrame % 4 == 0 &&
				(!Config.RootbeastHiding || isMoving ()))
			{
				++Sprite.currentFrame;
				Sprite.UpdateSourceRect ();
			}

			// Don't draw the shadow if hiding.
			HideShadow = isHiding;

			// Draw the base sprite with the crab legs.
			b.Draw (Sprite.Texture, getLocalPosition (Game1.viewport) +
				new Vector2 (Sprite.SpriteWidth * 2f,
					GetBoundingBox ().Height / 2f),
				Sprite.SourceRect, primaryColor.Value, rotation,
				new Vector2 (Sprite.SpriteWidth / 2f,
					Sprite.SpriteHeight * 0.75f),
				Math.Max (0.2f, Scale) * 4f,
				flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				Math.Max (0f, drawOnTop ? 0.991f : getStandingY() / 10000f));

			// Draw any glow, but only on the base sprite.
			if (isGlowing)
			{
				b.Draw (Sprite.Texture, getLocalPosition(Game1.viewport) +
					new Vector2 (Sprite.SpriteWidth * 2f,
						GetBoundingBox ().Height / 2f),
					Sprite.SourceRect, glowingColor * glowingTransparency,
					rotation, new Vector2 (Sprite.SpriteWidth / 2f,
						Sprite.SpriteHeight * 0.75f),
					Math.Max (0.2f, Scale) * 4f,
					flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
					Math.Max (0f, drawOnTop ? 0.99f
						: (getStandingY () / 10000f + 0.001f)));
			}

			// Draw the crop itself.
			b.Draw (cropTile.cropTexture, getLocalPosition (Game1.viewport) +
				new Vector2 (Sprite.SpriteWidth * 2f +
					0.5f * (Sprite.SpriteWidth - cropTile.cropSourceRect.Width),
					GetBoundingBox ().Height / 2 - (isHiding ? 0f : 28f)),
				cropTile.cropSourceRect, ContrastTint, rotation,
				new Vector2 (cropTile.cropSourceRect.Width * 0.5f,
					cropTile.cropSourceRect.Height * 0.75f),
				Math.Max (0.2f, Scale) * 4f,
				flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				Math.Max (0f, drawOnTop ? 0.992f : getStandingY() / 10000f + 0.002f));
		}

		public override void behaviorAtGameTick (GameTime time)
		{
			base.behaviorAtGameTick (time);

			startStopCooldown = Math.Max (0,
				startStopCooldown - time.ElapsedGameTime.Milliseconds);

			// If not near player, possibly stop and hide. Gets less likely as
			// health declines.
			if (Config.RootbeastHiding && startStopCooldown <= 0 &&
				!withinPlayerThreshold (moveTowardPlayerThreshold) &&
				Game1.random.NextDouble () < Math.Max (0.1, Math.Min (0.9,
					Health / (double) MaxHealth)))
			{
				Halt ();
				startStopCooldown = 500;
				return;
			}

			// Glow when invincible.
			updateGlow ();
			if (invincibleCountdown > 0)
			{
				glowingColor = secondaryColor.Value;
				invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
				if (invincibleCountdown <= 0)
					stopGlowing();
			}

			// When health is critical, flee from player.
			if (isFleeing || Health * 5 >= MaxHealth)
				return;

			// Speed up, but don't start moving again too quickly.
			if (!isFleeing && !isMoving ())
			{
				if (startStopCooldown > 0)
					return;
				startStopCooldown = 500;
				addedSpeed = Game1.random.Next (2, 4);
			}

			// Find the correct flight direction.
			fleePlayer (time);
		}

		protected override void updateMonsterSlaveAnimation (GameTime time)
		{
			if (isMoving())
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

			updateGlow ();
			if (invincibleCountdown > 0)
			{
				glowingColor = secondaryColor.Value;
				invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
				if (invincibleCountdown <= 0)
					stopGlowing ();
			}
		}
	}
}
