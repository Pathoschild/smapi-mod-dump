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
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace Cropbeasts.Beasts
{
	// Parts of this class are based on ShadowBrute and/or DinoMonster.
	public class GiantCropbeast : Cropbeast
	{
		private long thudCooldown = 0L;
		private long jumpCooldown = 0L;

		public GiantCropbeast ()
		{ }

		public GiantCropbeast (CropTile cropTile, bool primary)
		: base (cropTile, primary, primary, primary)
		{
			HideShadow = true;

			Sprite.SpriteWidth = 48;
			Sprite.SpriteHeight = 64;
			defaultAnimationInterval.Value = 600;
			Sprite.UpdateSourceRect ();
		}

		public override int takeDamage (int damage, int xTrajectory,
			int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
		{
			int result = takeDamage (damage, xTrajectory, yTrajectory, isBomb,
				addedPrecision, who, "axchop");

			// Screech and jump away if damaged more than 20% this time or to
			// less than 20% of health, as long as still alive.
			if (Health > 0 && (jumpCooldown == 0L || updateTimer >= jumpCooldown) &&
				(result * 5 > MaxHealth || Health * 5 < MaxHealth))
			{
				jumpRelativeToPlayer (toward: false);
			}

			return result;
		}

		protected override void localDeathAnimation ()
		{
			// Spawn a random portion of the secondary harvest into individual
			// Berrybeasts instead of immediately yielding the full harvest. The
			// primary and any tertiary harvest are still yielded normally, as
			// are the experience points.
			if (Context.IsMainPlayer && !reverted)
			{
				calculateHarvest ();
				while ((secondaryHarvest?.Stack ?? 0) > 1 &&
					Game1.random.NextDouble () < 0.05 * secondaryHarvest.Stack)
				{
					--secondaryHarvest.Stack;
					Berrybeast child = spawnChild<Berrybeast> (64);
					child.Health = child.MaxHealth = Math.Max (3, child.Health / 10);
					child.DamageToFarmer /= 5;
				}
			}

			base.localDeathAnimation ();

			currentLocation.localSound ("stumpCrack");

			Utility.makeTemporarySpriteJuicier (new TemporaryAnimatedSprite (44,
				Position, primaryColor.Value, 10)
			{
				holdLastFrame = true,
				interval = 70f,
				alpha = 0.8f,
				alphaFade = 0.008f
			},
				currentLocation, 8, 96, 96);
		}

		internal static readonly Dictionary<int, int> StockWhich = new ()
		{
			{ 190, GiantCrop.cauliflower },
			{ 254, GiantCrop.melon },
			{ 276, GiantCrop.pumpkin },
		};

		public override void draw (SpriteBatch b)
		{
			float layerDepth = (getTileLocation ().Y + 2f) * 64f / 10000f;

			Rectangle csr = cropTile.cropSourceRect;

			// Draw the base sprite with torso, arms and legs.
			b.Draw (Sprite.Texture, getLocalPosition (Game1.viewport) +
					new Vector2 (csr.Width, yOffset),
				Sprite.SourceRect, secondaryColor.Value, rotation,
				new Vector2 (24f, 32f), Math.Max (0.2f, Scale) * 4f,
				SpriteEffects.None, layerDepth);

			// Draw the crop itself.
			b.Draw (cropTile.cropTexture, getLocalPosition (Game1.viewport) +
					new Vector2 (csr.Width, yOffset - 0.175f * csr.Height),
				csr, ContrastTint, rotation,
				new Vector2 (csr.Width / 2f, csr.Height / 2f),
				Math.Max (0.2f, Scale) * 3.3f, (FacingDirection < 2)
					? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				layerDepth + 0.0001f);
		}

		public override Rectangle GetBoundingBox ()
		{
			return new Rectangle ((int) Position.X - 8, (int) Position.Y - 16,
				Sprite.SpriteWidth * 5 / 2, Sprite.SpriteHeight * 2);
		}

		public override void MovePosition (GameTime time,
			xTile.Dimensions.Rectangle viewport, GameLocation currentLocation)
		{
			base.MovePosition (time, viewport, currentLocation);

			if (thudCooldown == 0L || updateTimer >= thudCooldown &&
				yJumpVelocity <= 0f)
			{
				currentLocation.playSound ("treethud");
				thudCooldown = updateTimer + 600L;
			}
		}

		public override void update (GameTime time, GameLocation location)
		{
			base.update (time, location);

			// Occasionally jump toward the player.
			if ((jumpCooldown == 0L || updateTimer >= jumpCooldown) &&
				withinPlayerThreshold (moveTowardPlayerThreshold) &&
				Game1.random.NextDouble () < 0.003f)
			{
				jumpRelativeToPlayer (toward: true);
			}

			if (isMoving () && yJumpVelocity <= 0f)
			{
				float phase = updateTimer % 600L / 600f;
				yOffset = phase * -8f;
				rotation = 2f * (float) Math.PI / 40f * (phase - 0.5f +
					((phase < 0.5f) ? 40f : 0f));
			}
			else
			{
				yOffset = 0f;
				rotation = 0f;
			}
		}

		protected override void updateMonsterSlaveAnimation (GameTime time)
		{
			if (isMoving () && yJumpVelocity <= 0f)
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
			}
		}

		protected virtual void jumpRelativeToPlayer (bool toward)
		{
			jumpCooldown = updateTimer + 3000L;
			currentLocation.playSoundPitched ("batScreech", -200);
			Vector2 awayFromPlayerTrajectory =
				Utility.getAwayFromPlayerTrajectory (GetBoundingBox (), Player);
			setTrajectory (awayFromPlayerTrajectory * (toward ? -2f : 2f));
			jumpWithoutSound (2f);
			DelayedAction.playSoundAfterDelay ("stumpCrack", 100,
				currentLocation);
		}
	}
}
