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
    public class ShadowKingMinion : Shooter
    {
        private bool isGood;

        private float difficulty;

        public ShadowKingMinion() { }

        public ShadowKingMinion(Vector2 position, float difficulty, bool isGood = false) : base(position)
        {
            setTileLocation(position);
            aimTime = 2f;
            this.isGood = isGood;
            this.difficulty = difficulty;
        }

        public override void updateMovement(GameLocation location, GameTime time)
        {
        }

        public override void behaviorAtGameTick(GameTime time)
        {
            if (!shooting.Value)
            {
                if (Player is not null)
                {
                    Halt();
                    faceGeneralDirection(Player.getStandingPosition());
                    shooting.Value = true;
                    nextShot = aimTime;
                    shotsLeft = isGood ? 3 : 1;
                }
            }
            else
            {
                xVelocity = 0f;
                yVelocity = 0f;
                if (shotsLeft > 0)
                {
                    if (nextShot > 0f)
                    {
                        nextShot -= (float)time.ElapsedGameTime.TotalSeconds;
                        if (nextShot <= 0f)
                        {
                            Vector2 shot_velocity = Vector2.Zero;
                            float starting_rotation = 0f;
                            if (isGood && Player is not null)
                            {
                                shot_velocity = Player.Position - Position;
                                shot_velocity.Normalize();
                                starting_rotation = RoguelikeUtility.VectorToRadians(shot_velocity) + RoguelikeUtility.DegreesToRadians(90);
                            }
                            else
                            {
                                switch (facingDirection.Value)
                                {
                                    case 0:
                                        shot_velocity = new Vector2(0f, -1f);
                                        starting_rotation = 0f;
                                        break;
                                    case 1:
                                        shot_velocity = new Vector2(1f, 0f);
                                        starting_rotation = (float)Math.PI / 2f;
                                        break;
                                    case 2:
                                        shot_velocity = new Vector2(0f, 1f);
                                        starting_rotation = (float)Math.PI;
                                        break;
                                    case 3:
                                        shot_velocity = new Vector2(-1f, 0f);
                                        starting_rotation = -(float)Math.PI / 2f;
                                        break;
                                }
                            }

                            shot_velocity *= projectileSpeed;
                            fireEvent.Fire();
                            currentLocation.playSound(fireSound);
                            BasicProjectile projectile = new((int)(DamageToFarmer * difficulty), firedProjectile, 0, 0, 0f, shot_velocity.X, shot_velocity.Y, Position, "", "", explode: false, damagesMonsters: false, base.currentLocation, this);
                            projectile.startingRotation.Value = starting_rotation;
                            projectile.height.Value = 24f;
                            projectile.debuff.Value = projectileDebuff;
                            projectile.ignoreTravelGracePeriod.Value = true;
                            projectile.IgnoreLocationCollision = true;
                            currentLocation.projectiles.Add(projectile);
                            shotsLeft--;
                            if (shotsLeft == 0)
                                nextShot = aimEndTime;
                            else
                                nextShot = burstTime;
                        }
                    }
                }
                else if (nextShot > 0f)
                {
                    nextShot -= (float)time.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    shooting.Value = false;
                    currentLocation.characters.Remove(this);
                }
            }
        }
    }
}
