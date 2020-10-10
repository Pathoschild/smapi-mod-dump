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
using System.Linq;

namespace Cropbeasts.Beasts
{
	// Much of this class is based on StardewValley.Monsters.Ghost.
	public class AncientBeast : Cropbeast
	{
		private readonly NetInt wasHitCounter = new NetInt (0);
		private readonly NetBool turningRight = new NetBool ();

		private int identifier = Game1.random.Next (-99999, 99999);

		private new int yOffset = 0;

		public AncientBeast ()
		{}

		public AncientBeast (CropTile cropTile, bool primary)
		: base (cropTile, primary, primary, !primary)
		{
			Slipperiness = 8;
			HideShadow = true;
		}

		protected override void initNetFields ()
		{
			base.initNetFields ();
			NetFields.AddFields (wasHitCounter, turningRight);
		}

		public override int takeDamage (int damage, int xTrajectory,
			int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
		{
			int result = takeDamage (damage, xTrajectory, yTrajectory, isBomb,
				addedPrecision, who, "shadowHit");
			if (result > -1)
			{
				addedSpeed = -1;
				setTrajectory (xTrajectory / 4, yTrajectory / 4);
				Utility.addSprinklesToLocation (currentLocation, getTileX (),
					getTileY (), 2, 2, 101, 50, secondaryColor.Value);
				Utility.removeLightSource (identifier);
			}
			return result;
		}

		public override void shedChunks (int _number, float _scale)
		{
			// The sprinkles above are used in lieu of this.
		}

		protected override void localDeathAnimation ()
		{
			base.localDeathAnimation ();

			currentLocation.localSound ("ghost");

			// Draw the harvest within the ghost being revealed.
			currentLocation.temporarySprites.Add (new TemporaryAnimatedSprite
				(cropTile.mapping.harvestTextureName, cropTile.mapping.harvestSourceRect,
				800f, 1, 0, Position + new Vector2 (4f, -8f), flicker: false,
				flipped: false, 0.91f, 0.01f, Color.White, 3f, 0.01f, 0f,
				(float) Math.PI / 64f));

			// Draw the evaporating ghost.
			currentLocation.temporarySprites.Add (new TemporaryAnimatedSprite
				(Sprite.textureName.Value, new Rectangle (0, 96, 16, 24), 200f,
				4, 0, Position, flicker: false, flipped: false, 0.9f, 0.001f,
				Color.White, 5f, 0.01f, 0f, (float) Math.PI / 64f));
		}

		public override void drawAboveAllLayers (SpriteBatch b)
		{
			// Draw the ghost.
			b.Draw (Sprite.Texture, getLocalPosition (Game1.viewport)
					+ new Vector2 (32f, 21 + yOffset), Sprite.SourceRect,
				Color.White, 0f, new Vector2 (8f, 16f), Math.Max (0.2f, Scale) * 5f,
				flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				Math.Max (0f, drawOnTop ? 0.991f
					: ((float) getStandingY () / 10000f)));

			// Draw its shadow. Not too dark, cuz why even ghost haz shadow?
			b.Draw (Game1.shadowTexture, getLocalPosition (Game1.viewport) +
				new Vector2 (32f, 64f), Game1.shadowTexture.Bounds,
				Color.White * 0.25f,
				0f, new Vector2 (Game1.shadowTexture.Bounds.Center.X,
					Game1.shadowTexture.Bounds.Center.Y),
				3f + (float) yOffset / 20f, SpriteEffects.None,
				(float) (getStandingY () - 1) / 10000f);
		}

		public override void drawAboveAlwaysFrontLayer (SpriteBatch b)
		{
			if (currentLocation != null)
				drawAboveAllLayers (b);
		}

		protected override void updateAnimation (GameTime time)
		{
			yOffset = (int) (Math.Sin ((time.TotalGameTime.Milliseconds / 1000f)
				* (Math.PI * 2.0)) * 20.0);

			if (currentLocation.Equals (Game1.currentLocation))
			{
				LightSource currentLightSource = Game1.currentLightSources
					.FirstOrDefault ((source) => source.Identifier == identifier);
				if (currentLightSource != null)
				{
					currentLightSource.position.Value = new Vector2
						(Position.X + 32f, Position.Y + 64f + (float) yOffset);
				}
				else
				{
					Game1.currentLightSources.Add (new LightSource (5,
						new Vector2 (Position.X + 8f, Position.Y + 64f), 1f,
						Color.LightBlue * 0.7f, identifier,
						LightSource.LightContext.None, 0L));
				}
			}

			float xDist = (Player.GetBoundingBox ().Center.X -
				GetBoundingBox ().Center.X) / -400f;
			float yDist = (Player.GetBoundingBox ().Center.Y -
				GetBoundingBox ().Center.Y) / 400f;

			if (wasHitCounter.Value <= 0)
			{
				float targetRotation = (float) Math.Atan2 (0f - yDist, xDist) -
					(float) Math.PI / 2f;

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

			float bonus = withinPlayerThreshold (moveTowardPlayerThreshold)
				? 1f : 3f;
			float xRot = bonus * (float) Math.Cos (rotation + Math.PI / 2.0);
			float yRot = -1f * bonus * (float) Math.Sin (rotation + Math.PI / 2.0);

			xVelocity += (0f - xRot) * 1.875f / 6f +
				(float) Game1.random.Next (-10, 10) / 100f;
			if (Math.Abs (xVelocity) > Math.Abs ((0f - xRot) * 5f))
				xVelocity -= (0f - xRot) * 1.875f / 6f;

			yVelocity += (0f - yRot) * 1.875f / 6f +
				(float) Game1.random.Next (-10, 10) / 100f;
			if (Math.Abs (yVelocity) > Math.Abs ((0f - yRot) * 5f))
				yVelocity -= (0f - yRot) * 1.875f / 6f;

			faceGeneralDirection (Player.getStandingPosition ());
			resetAnimationSpeed ();
		}

		public override void update (GameTime time, GameLocation location)
		{
			base.update (time, location);

			// Handle only empty locations here, and then with some delay.
			if (location.farmers.Count > 0 || Game1.random.NextDouble () < 0.99)
				return;

			// Follow the player out of the greenhouse.
			if (location.IsGreenhouse &&
				Game1.currentLocation.IsFarm && Game1.currentLocation.IsOutdoors)
			{
				Vector2 farmer = Game1.player.Position;
				double angle = Math.PI * (Game1.random.NextDouble () - 0.5);
				Vector2 position = farmer + new Vector2
					(4f * 64f * (float) Math.Sin (angle),
					4f * 64f * (float) Math.Cos (angle));
				poofTo (Game1.currentLocation, position);
			}
			// Follow the player into the greenhouse.
			else if (location.IsFarm && location.IsOutdoors &&
				Game1.currentLocation.IsGreenhouse)
			{
				var layer = Game1.currentLocation.map.Layers[0];
				Vector2 position = (Game1.random.NextDouble () < 0.5)
					? 64f * new Vector2 (layer.LayerWidth / 2 +
						((Game1.random.NextDouble () < 0.5) ? -1 : 1) *
							Game1.random.Next (4, Math.Max (5, layer.LayerWidth / 2)),
						layer.LayerHeight - 2)
					: 64f * new Vector2
						((Game1.random.NextDouble () < 0.5) ? 0 : layer.LayerWidth - 1,
						Game1.random.Next (layer.LayerHeight / 2, layer.LayerHeight - 2));
				poofTo (Game1.currentLocation, position);
			}
		}
	}
}
