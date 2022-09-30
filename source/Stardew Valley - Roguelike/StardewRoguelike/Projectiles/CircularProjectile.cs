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
using System;

namespace StardewRoguelike.Projectiles
{
    public class CircularProjectile : BasicProjectile
    {
        private readonly NetFloat angle = new();

        private readonly NetFloat AngularVelocity = new();

        private readonly NetFloat Distance = new();

        private readonly NetVector2 PointOfRotation = new();

        public CircularProjectile() : base()
        {
            InitNetFields();
        }

        public CircularProjectile(int damageToFarmer, int parentSheetIndex, int bouncesTillDestruct, int tailLength, float rotationVelocity, float angularVelocity, Vector2 pointOfRotation, Vector2 startingPosition, string collisionSound, string firingSound, bool explode, bool damagesMonsters = false, GameLocation location = null, Character firer = null, bool spriteFromObjectSheet = false, onCollisionBehavior collisionBehavior = null)
            : base(damageToFarmer, parentSheetIndex, bouncesTillDestruct, tailLength, rotationVelocity, 0f, 0f, startingPosition, collisionSound, firingSound, explode, damagesMonsters, location, firer, spriteFromObjectSheet, collisionBehavior)
        {
            InitNetFields();

            AngularVelocity.Value = angularVelocity;
            Distance.Value = Vector2.Distance(pointOfRotation, startingPosition);
            PointOfRotation.Value = pointOfRotation;

            angle.Value = BossManager.VectorToRadians(pointOfRotation - startingPosition);
        }

        private void InitNetFields()
        {
            NetFields.AddFields(angle, AngularVelocity, Distance, PointOfRotation);
        }

        public override void updatePosition(GameTime time)
        {
            angle.Value += (float)time.ElapsedGameTime.TotalSeconds * AngularVelocity.Value;

            Vector2 displacement = new(Distance.Value * (float)Math.Sin(angle.Value), Distance.Value * (float)Math.Cos(angle.Value));
            position.X = PointOfRotation.Value.X + displacement.X;
            position.Y = PointOfRotation.Value.Y + displacement.Y;
        }
    }
}
