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
using StardewRoguelike.Bosses;
using StardewValley;
using StardewValley.Projectiles;

namespace StardewRoguelike.Projectiles
{
    public class ReturningProjectile : BasicProjectile
    {
        private readonly NetVector2 StartingPosition = new();

        private readonly NetFloat ReturningVelocityMultiplier = new();

        public ReturningProjectile() : base()
        {
            InitNetFields();
        }

        public ReturningProjectile(float returningVelocityMultiplier, int damageToFarmer, int parentSheetIndex, int bouncesTillDestruct, int tailLength, float rotationVelocity, float velocityX, float velocityY, Vector2 startingPosition, string collisionSound, string firingSound, bool explode, bool damagesMonsters = false, GameLocation location = null, Character firer = null, bool spriteFromObjectSheet = false, onCollisionBehavior collisionBehavior = null)
            : base(damageToFarmer, parentSheetIndex, bouncesTillDestruct, tailLength, rotationVelocity, velocityX, velocityY, startingPosition, collisionSound, firingSound, explode, damagesMonsters, location, firer, spriteFromObjectSheet, collisionBehavior)
        {
            InitNetFields();

            ReturningVelocityMultiplier.Value = returningVelocityMultiplier;
            StartingPosition.Value = startingPosition;
        }

        private void InitNetFields()
        {
            NetFields.AddFields(ReturningVelocityMultiplier, StartingPosition);
        }

        public override void behaviorOnCollisionWithOther(GameLocation location)
        {
            damagesMonsters.Value = true;

            Vector2 newVelocity = StartingPosition.Value - position.Value;
            newVelocity.Normalize();
            newVelocity *= ReturningVelocityMultiplier.Value;

            rotation = BossManager.VectorToRadians(newVelocity) + BossManager.DegreesToRadians(90);

            xVelocity.Value = newVelocity.X;
            yVelocity.Value = newVelocity.Y;
        }
    }
}
