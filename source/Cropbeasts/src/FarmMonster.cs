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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;

namespace Cropbeasts
{
	public class FarmMonster : Monster
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;

		protected GameLocation intendedLocation { get; private set; }
		protected TimeSpan spawnTime { get; private set; }

		protected TimeSpan dieTime { get; private set; }
		protected readonly NetInt hitsTaken = new NetInt (0);
		protected readonly NetInt damageDealt = new NetInt (0);

		protected long updateTimer { get; private set; }
		private bool registered;

		protected bool? lastProjectileHit;

		protected bool isFleeing { get; private set; }
		private Vector2 lastFleePosition;
		private int fleeFailCount;
		private int fleeDirectionOffset;

		protected FarmMonster ()
		{}

		protected FarmMonster (string name, GameLocation location,
			Vector2 tileLocation)
		: base (name, tileLocation, 2)
		{
			wildernessFarmMonster = true;
			intendedLocation = location;
		}

		protected override void initNetFields ()
		{
			base.initNetFields ();
			NetFields.AddFields (hitsTaken, damageDealt);
		}

		public virtual void spawn ()
		{
			intendedLocation.characters.Add (this);
			focusedOnFarmers = true;
			spawnTime = Game1.currentGameTime.TotalGameTime;

			// Since the facing direction wasn't precalculated, do it here.
			Farmer farmer = Utilities.FindNearestFarmer (intendedLocation,
				getTileLocationPoint (), out _);
			if (farmer != null)
				faceGeneralDirection (farmer.getTileLocation ());
		}

		public virtual double rateCombatPerformance ()
		{
			Monitor.Log ($"Rating combat performance against {Name}...");
			double rating = (rateHitsToKill () * 1.5 + rateTimeToKill () +
				rateDamageDealt () * 2.0) / 4.5;
			Monitor.Log ($"...weighted score of {rating.ToString ("P0")}.");
			return rating;
		}

		protected virtual double rateHitsToKill ()
		{
			// Top score for one-hit kill, bottom score for five or more hits.
			double rating = Math.Max (0.0, Math.Min (1.0, 1.0 - (hitsTaken.Value - 1.0) / 4.0));
			Monitor.Log ($"- {hitsTaken.Value} hits to kill; score: {rating.ToString ("P0")}");
			return rating;
		}

		protected virtual double rateTimeToKill ()
		{
			// Top score for 10 seconds or less, bottom score for 20+ seconds.
			double secondsToKill = (dieTime - spawnTime).Ticks / TimeSpan.TicksPerSecond;
			double rating = Math.Max (0.0, Math.Min (1.0, 1.0 - (secondsToKill - 10.0) / 10.0));
			Monitor.Log ($"- {secondsToKill} seconds to kill; score: {rating.ToString ("P0")}");
			return rating;
		}

		protected virtual double rateDamageDealt ()
		{
			// Top score for no damage, bottom score for 33% or more of HP.
			// This sums damage to all players but divides by the local player's HP.
			// That is intentional since multiplayer combat is at an advantage.
			double ceiling = 0.33 * Game1.player.maxHealth;
			double rating = Math.Max (0.0, Math.Min (1.0, 1.0 - damageDealt.Value / ceiling));
			Monitor.Log ($"- {damageDealt.Value} damage to player(s); score: {rating.ToString ("P0")}");
			return rating;
		}

		public override int takeDamage (int damage, int xTrajectory,
			int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
		{
			// Overrides must also call the overload below instead of the base.
			return takeDamage (damage, xTrajectory, yTrajectory, isBomb,
				addedPrecision, who, "hitEnemy");
		}

		protected int takeDamage (int damage, int xTrajectory,
			int yTrajectory, bool isBomb, double addedPrecision, Farmer who,
			string hitSound)
		{
			if (currentLocation == null) return -1;
			int result = takeDamage (damage, xTrajectory, yTrajectory, isBomb,
				addedPrecision, hitSound);
			if (result > -1 && Health > 0)
				++hitsTaken.Value;
			return result;
		}

		public override void onDealContactDamage (Farmer who)
		{
			// This skips the random factors, but gives an adequate impression.
			if (who.IsLocalPlayer)
				damageDealt.Value += Math.Max (1, DamageToFarmer - who.resilience);
		}

		public virtual void onDealProjectileDamage (Farmer who, int amount)
		{
			if (who.IsLocalPlayer)
				damageDealt.Value += amount;
			lastProjectileHit = true;
		}

		public virtual void onProjectileSpoiled ()
		{
			lastProjectileHit = false;
		}

		// NOTE: Due to a base game bug, Monster.localDeathAnimation does not
		// propagate to players other than the player who slays a monster. To
		// work around the bug while minimizing future refactoring when it is
		// fixed, localDeathAnimation is redefined here and called from
		// processDeath, which must in turn be called from cleanUpMonsters.

		protected new virtual void localDeathAnimation ()
		{}

		public override void update (GameTime time, GameLocation location)
		{
			base.update (time, location);

			updateTimer += time.ElapsedGameTime.Milliseconds;

			if (!registered)
			{
				registered = true;
				ModEntry.Instance.registerMonster (this);
			}

			if (Health <= 0)
				processDeath ();
		}

		public void processDeath ()
		{
			if (dieTime.Ticks == 0L)
			{
				++hitsTaken.Value;
				dieTime = Game1.currentGameTime.TotalGameTime;
				localDeathAnimation ();
			}
		}

		protected virtual void fleePlayer (GameTime time)
		{
			// Check whether done fleeing.
			if (isFleeing && !withinPlayerThreshold (moveTowardPlayerThreshold))
			{
				isFleeing = false;
				lastFleePosition = new Vector2 ();
				fleeFailCount = 0;
				fleeDirectionOffset = 0;
			}
			else
			{
				isFleeing = true;
			}

			// Detect getting stuck or unstruck.
			if ((lastFleePosition - Position).Length () < 1f)
				++fleeFailCount;
			else
				fleeFailCount = 0;

			// Switch to a different direction if needed.
			if (fleeFailCount > 10)
			{
				fleeDirectionOffset = Game1.random.Next (1, 4);
				fleeFailCount = 0;
			}

			lastFleePosition = Position;

			int newDirection;
			if (Math.Abs (Player.GetBoundingBox ().Center.Y -
				GetBoundingBox ().Center.Y) > 192)
			{
				if (Player.GetBoundingBox ().Center.X -
						GetBoundingBox ().Center.X > 0)
					newDirection = 3;
				else
					newDirection = 1;
			}
			else if (Player.GetBoundingBox ().Center.Y -
				GetBoundingBox ().Center.Y > 0)
			{
				newDirection = 0;
			}
			else
			{
				newDirection = 2;
			}

			FacingDirection = (newDirection + fleeDirectionOffset) % 4;
			setMovingInFacingDirection ();
			MovePosition (time, Game1.viewport, currentLocation);
		}

		public void drawTrackingArrow (SpriteBatch b)
		{
			if (Utility.isOnScreen (Position, 64))
				return;

			Rectangle bounds = Game1.graphics.GraphicsDevice.Viewport.Bounds;
			Vector2 renderPos = new Vector2 ();
			float rotation = 0f;

			if (Position.X > Game1.viewport.MaxCorner.X - 64f)
			{
				renderPos.X = bounds.Right - 8f;
				rotation = (float) Math.PI / 2f;
			}
			else if (Position.X < Game1.viewport.X)
			{
				renderPos.X = 8f;
				rotation = -(float) Math.PI / 2f;
			}
			else
			{
				renderPos.X = Position.X - Game1.viewport.X;
			}

			if (Position.Y > Game1.viewport.MaxCorner.Y - 64f)
			{
				renderPos.Y = bounds.Bottom - 8f;
				rotation = (float) Math.PI;
			}
			else if (Position.Y < Game1.viewport.Y)
			{
				renderPos.Y = 8f;
			}
			else
			{
				renderPos.Y = Position.Y - Game1.viewport.Y;
			}

			if (renderPos.X == 8f && renderPos.Y == 8f)
				rotation += (float) Math.PI / 4f;
			if (renderPos.X == 8f && renderPos.Y == bounds.Bottom - 8f)
				rotation += (float) Math.PI / 4f;
			if (renderPos.X == bounds.Right - 8f && renderPos.Y == 8f)
				rotation -= (float) Math.PI / 4f;
			if (renderPos.X == bounds.Right - 8f && renderPos.Y == bounds.Bottom - 8f)
				rotation -= (float) Math.PI / 4f;

			Rectangle sourceRect = new Rectangle (421, 459, 12, 12);
			float scale = 2f;
			Vector2 position = Utility.makeSafe (renderPos, new Vector2
				(sourceRect.Width * scale, sourceRect.Height * scale));
			b.Draw (Game1.mouseCursors, position, sourceRect, Color.White,
				rotation, new Vector2 (6f, 6f), scale, SpriteEffects.None, 1f);
		}
	}
}
