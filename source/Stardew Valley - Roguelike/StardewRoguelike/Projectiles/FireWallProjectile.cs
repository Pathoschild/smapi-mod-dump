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
using StardewValley.Projectiles;

namespace StardewRoguelike.Projectiles
{
    public class FireWallProjectile : BasicProjectile
    {
        private NetFloat DistanceBeforeStopping = new();

        private NetFloat DisappearAfter = new();

        public FireWallProjectile() : base()
        {
            InitNetFields();
        }

        public FireWallProjectile(float distanceBeforeStopping, float disappearAfter, int damageToFarmer, int parentSheetIndex, int bouncesTillDestruct, int tailLength, float rotationVelocity, float velocityX, float velocityY, Vector2 startingPosition, string collisionSound, string firingSound, bool explode, bool damagesMonsters = false, GameLocation location = null, Character firer = null, bool spriteFromObjectSheet = false, onCollisionBehavior collisionBehavior = null)
            : base(damageToFarmer, parentSheetIndex, bouncesTillDestruct, tailLength, rotationVelocity, velocityX, velocityY, startingPosition, collisionSound, firingSound, explode, damagesMonsters, location, firer, spriteFromObjectSheet, collisionBehavior)
        {
            InitNetFields();

            DisappearAfter.Value = disappearAfter;
            DistanceBeforeStopping.Value = distanceBeforeStopping;
        }

        private void InitNetFields()
        {
            NetFields.AddFields(DisappearAfter, DistanceBeforeStopping);
        }

        public override void behaviorOnCollisionWithPlayer(GameLocation location, Farmer player)
        {
            if (damagesMonsters.Value)
                return;

            if (debuff.Value != -1 && player.CanBeDamaged() && Game1.random.Next(11) >= player.immunity && !player.hasBuff(28))
            {
                if (Game1.player == player)
                    Game1.buffsDisplay.addOtherBuff(new Buff(debuff));

                location.playSound(debuffSound.Value);
            }

            player.takeDamage(damageToFarmer.Value, overrideParry: true, null);
        }

        public override void updatePosition(GameTime time)
        {
            if (travelDistance >= DistanceBeforeStopping.Value)
                return;

            position.X += xVelocity;
            position.Y += yVelocity;
        }

        public override bool update(GameTime time, GameLocation location)
        {
            if (Game1.IsMasterGame)
            {
                DisappearAfter.Value -= (float)time.ElapsedGameTime.TotalSeconds;
                if (DisappearAfter <= 0f)
                    location.projectiles.Remove(this);
            }

            if (base.update(time, location))
            {
                bool collidesWithPlayer = false;

                foreach (Farmer player in location.farmers)
                {
                    if (player.GetBoundingBox().Intersects(getBoundingBox()))
                    {
                        collidesWithPlayer = true;
                        break;
                    }
                }

                if (!collidesWithPlayer)
                    return true;
            }

            return false;
        }
    }
}
