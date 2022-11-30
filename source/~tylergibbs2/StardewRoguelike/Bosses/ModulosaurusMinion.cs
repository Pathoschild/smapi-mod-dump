/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using System;

namespace StardewRoguelike.Bosses
{
    public class ModulosaurusMinion : LavaLurk
    {
        public ModulosaurusMinion() : base() { }

        public ModulosaurusMinion(Vector2 position) : base(position)
        {
            setTileLocation(position);
        }

        public override bool TargetInRange()
        {
            return true;
        }

        public override void behaviorAtGameTick(GameTime time)
        {
			if (targettedFarmer is null || targettedFarmer.currentLocation != currentLocation)
			{
				targettedFarmer = null;
				targettedFarmer = findPlayer();
			}
			if (stateTimer > 0f)
			{
				stateTimer -= (float)time.ElapsedGameTime.TotalSeconds;
				if (stateTimer <= 0f)
				{
					stateTimer = 0f;
				}
			}
			if (currentState.Value == State.Submerged)
			{
				swimSpeed = 2;
				if (stateTimer == 0f)
				{
					currentState.Value = State.Lurking;
					stateTimer = 1f;
				}
			}
			else if (currentState.Value == State.Lurking)
			{
				this.swimSpeed = 1;
				if (this.stateTimer == 0f)
				{
					if (this.TargetInRange())
					{
						this.currentState.Value = State.Emerged;
						this.stateTimer = 1f;
						this.swimSpeed = 0;
					}
					else
					{
						this.currentState.Value = State.Diving;
						this.stateTimer = 1f;
					}
				}
			}
			else if (currentState.Value == State.Emerged)
			{
				if (stateTimer == 0f)
				{
					currentState.Value = State.Firing;
					stateTimer = 1f;
					fireTimer = 0.25f;
				}
			}
			else if (currentState.Value == State.Firing)
			{
				if (stateTimer == 0f)
				{
					currentState.Value = State.Diving;
					stateTimer = 1f;
				}
				if (fireTimer > 0f)
				{
					fireTimer -= (float)time.ElapsedGameTime.TotalSeconds;
					if (fireTimer <= 0f)
					{
						fireTimer = 0.25f;
						if (targettedFarmer is not null)
						{
							Vector2 shot_origin = Position + new Vector2(0f, -32f);
							Vector2 shot_velocity = targettedFarmer.Position - shot_origin;
							shot_velocity.Normalize();
							shot_velocity *= 7f;
							currentLocation.playSound("fireball");
							BasicProjectile projectile = new(25, 10, 0, 3, (float)Math.PI / 16f, shot_velocity.X, shot_velocity.Y, shot_origin, "", "", explode: false, damagesMonsters: false, currentLocation, this);
							projectile.IgnoreLocationCollision = true;
							projectile.ignoreTravelGracePeriod.Value = true;
							currentLocation.projectiles.Add(projectile);
						}
					}
				}
			}
			else if (currentState.Value == State.Diving && stateTimer == 0f)
			{
				currentLocation.characters.Remove(this);
			}
			if (targettedFarmer is not null && approachFarmer)
			{
				if (getTileX() > targettedFarmer.getTileX())
				{
					velocity.X = -1f;
				}
				else if (getTileX() < targettedFarmer.getTileX())
				{
					velocity.X = 1f;
				}
				if (getTileY() > targettedFarmer.getTileY())
				{
					velocity.Y = -1f;
				}
				else if (getTileY() < targettedFarmer.getTileY())
				{
					velocity.Y = 1f;
				}
			}
			if (velocity.X != 0f || velocity.Y != 0f)
			{
				Rectangle next_bounds = GetBoundingBox();
				Vector2 next_position = Position;
				next_bounds.Inflate(48, 48);
				next_bounds.X += (int)velocity.X * swimSpeed;
				next_position.X += (int)velocity.X * swimSpeed;
				if (!CheckInWater(next_bounds))
				{
					velocity.X *= -1f;
					next_bounds.X += (int)velocity.X * swimSpeed;
					next_position.X += (int)velocity.X * swimSpeed;
				}
				next_bounds.Y += (int)velocity.Y * swimSpeed;
				next_position.Y += (int)velocity.Y * swimSpeed;
				if (!CheckInWater(next_bounds))
				{
					velocity.Y *= -1f;
					next_bounds.Y += (int)velocity.Y * swimSpeed;
					next_position.Y += (int)velocity.Y * swimSpeed;
				}
				if (Position != next_position)
					Position = next_position;
			}
		}

		public new bool CheckInWater(Rectangle position)
        {
			return base.CheckInWater(position);
        }
    }
}
