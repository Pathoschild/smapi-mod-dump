using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace Cropbeasts.Projectiles
{
	internal class FakeDamager : Monster
	{}

	public abstract class BeastProjectile : Projectile
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;

		public readonly NetInt damageToFarmer = new NetInt (0);

		// parryCatchIndex defaults to currentTileSheetIndex if spriteFromObjectSheet, else none
		protected readonly NetInt parryCatchIndex = new NetInt (-1);
		protected readonly NetDouble parryCatchChance = new NetDouble (0.0);

		protected readonly NetString firingSound = new NetString ();
		protected readonly NetString collisionSound = new NetString ();
		protected readonly NetBool shouldExplode = new NetBool (false);

		protected BeastProjectile ()
		{
			NetFields.AddFields (damageToFarmer, parryCatchIndex, parryCatchChance,
				firingSound, collisionSound, shouldExplode);
		}

		public virtual void fire (Cropbeast firer, GameLocation location,
			Vector2 startingPosition, Vector2 velocity,
			float rotationVelocity = 0f)
		{
			theOneWhoFiredMe.Set (location, firer);
			position.Value = startingPosition;
			this.xVelocity.Value = velocity.X;
			this.yVelocity.Value = velocity.Y;
			this.rotationVelocity.Value = rotationVelocity;

			if ((firingSound.Value ?? "") != "")
				location.playSound (firingSound.Value);

			location.projectiles.Add (this);
		}

		public override void behaviorOnCollisionWithPlayer
			(GameLocation location, Farmer player)
		{
			int oldHealth = player.health;
			player.takeDamage (damageToFarmer.Value, overrideParry: false,
				new FakeDamager ());

			// If parried, maybe give the catch.
			if (player.health == oldHealth)
			{
				if (parryCatchIndex.Value == -1 && spriteFromObjectSheet.Value)
					parryCatchIndex.Value = currentTileSheetIndex.Value;

				if (Game1.random.NextDouble () < parryCatchChance.Value &&
					parryCatchIndex.Value != -1)
				{

					DelayedAction.playSoundAfterDelay ("dwoop", 100, location);
					player.addItemByMenuIfNecessary
						(new SObject (parryCatchIndex.Value, 1));
				}
			}
			// Otherwise, notify the firer and explode.
			else
			{
				Cropbeast beast = theOneWhoFiredMe.Get (location) as Cropbeast;
				beast?.onDealProjectileDamage (player, oldHealth - player.health);

				explode (location);
			}
		}

		public override void behaviorOnCollisionWithTerrainFeature
			(TerrainFeature _t, Vector2 _tileLocation, GameLocation location)
		{
			Cropbeast beast = theOneWhoFiredMe.Get (location) as Cropbeast;
			beast?.onProjectileSpoiled ();
			explode (location);
		}

		public override void behaviorOnCollisionWithMineWall (int _tileX,
			int _tileY)
		{}

		public override void behaviorOnCollisionWithOther (GameLocation location)
		{
			Cropbeast beast = theOneWhoFiredMe.Get (location) as Cropbeast;
			beast?.onProjectileSpoiled ();
			explode (location);
		}

		public override void behaviorOnCollisionWithMonster (NPC _npc,
			GameLocation _location)
		{}

		public override void updatePosition (GameTime time)
		{
			position.X += xVelocity;
			position.Y += yVelocity;
		}

		protected virtual void explode (GameLocation location)
		{
			if (spriteFromObjectSheet.Value)
			{
				Game1.createRadialDebris (location, 12,
					(int) (position.X + 32f) / 64, (int) (position.Y + 32f) / 64,
					6, resource: false);
			}
			else
			{
				Rectangle sourceRect = Game1.getSourceRectForStandardTileSheet
					(Projectile.projectileSheet, currentTileSheetIndex.Value);
				sourceRect.X += 28;
				sourceRect.Y += 28;
				sourceRect.Width = 8;
				sourceRect.Height = 8;
				Game1.createRadialDebris (location, "TileSheets\\Projectiles",
					sourceRect, 4, (int) position.X + 32, (int) position.Y + 32,
					12, (int) (position.Y / 64f) + 1);
			}

			if ((collisionSound.Value ?? "") != "")
				location.playSound (collisionSound);

			if (shouldExplode.Value)
			{
				Utilities.MP.broadcastSprites (location, new TemporaryAnimatedSprite
					(362, Game1.random.Next (30, 90), 6, 1, position,
					flicker: false, Game1.random.NextDouble () < 0.5));
			}

			destroyMe = true;
		}
	}
}