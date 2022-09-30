/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using System;

namespace StardewRoguelike.Bosses
{
    public class SkellyMinion : Skeleton
    {
        private int lineOfSightDistance = 11;

        public SkellyMinion() { }

        public SkellyMinion(Vector2 position, float difficulty, bool isMage = false) : base(position, isMage)
        {
            MaxHealth = (int)Math.Round(50 * difficulty);
            Health = MaxHealth;
            DamageToFarmer = 10;
            moveTowardPlayerThreshold.Value = 30;
            focusedOnFarmers = true;
        }

        public override void behaviorAtGameTick(GameTime time)
        {
            if (Roguelike.HardMode)
            {
                base.behaviorAtGameTick(time);
                return;
            }

            controller = null;

            bool spottedPlayer = (bool)typeof(Skeleton).GetField("spottedPlayer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(this);
            NetBool throwing = (NetBool)typeof(Skeleton).GetField("throwing", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(this);

            if (!spottedPlayer && Utility.doesPointHaveLineOfSightInMine(currentLocation, getTileLocation(), Player.getTileLocation(), lineOfSightDistance))
            {
                typeof(Skeleton).GetField("spottedPlayer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(this, true);
                Halt();
                facePlayer(Player);
            }
            else if (throwing.Value)
            {
                if (invincibleCountdown > 0)
                {
                    invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
                    if (invincibleCountdown <= 0)
                        stopGlowing();
                }
                Sprite.Animate(time, 20, 5, 150f);
                if (Sprite.currentFrame == 24)
                {
                    throwing.Value = false;
                    Sprite.currentFrame = 0;
                    faceDirection(2);
                    Vector2 v = Utility.getVelocityTowardPlayer(new Point((int)Position.X, (int)Position.Y), 8f, Player);
                    if (isMage.Value)
                    {
                        if (Game1.random.NextDouble() < 0.5)
                            currentLocation.projectiles.Add(new DebuffingProjectile(19, 14, 4, 4, (float)Math.PI / 16f, v.X, v.Y, new Vector2(Position.X, Position.Y), currentLocation, this));
                        else
                            currentLocation.projectiles.Add(new BasicProjectile(DamageToFarmer * 2, 9, 0, 4, 0f, v.X, v.Y, new Vector2(Position.X, Position.Y), "flameSpellHit", "flameSpell", explode: false, damagesMonsters: false, currentLocation, this));
                    }
                    else
                        currentLocation.projectiles.Add(new BasicProjectile(DamageToFarmer, 4, 0, 0, (float)Math.PI / 16f, v.X, v.Y, new Vector2(Position.X, Position.Y), "skeletonHit", "skeletonStep", explode: false, damagesMonsters: false, currentLocation, this));
                }
            }
            else if (spottedPlayer && Game1.random.NextDouble() < (isMage ? 0.008 : 0.002) && Utility.doesPointHaveLineOfSightInMine(currentLocation, getTileLocation(), Player.getTileLocation(), lineOfSightDistance))
            {
                throwing.Value = true;
                Halt();
                Sprite.currentFrame = 20;
                shake(750);
            }
        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {
            return base.takeDamage(damage, 0, 0, isBomb, addedPrecision, who);
        }
    }
}
