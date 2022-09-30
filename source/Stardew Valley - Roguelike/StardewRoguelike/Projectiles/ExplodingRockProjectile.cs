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
using System;

namespace StardewRoguelike.Projectiles
{
    public class ExplodingRockProjectile : BasicProjectile
    {
        private NetFloat TargetDistance = new();

        public ExplodingRockProjectile() : base()
        {
            InitNetFields();
        }

        public ExplodingRockProjectile(Vector2 tileToExplodeAt, float speedMultiplier, int damageToFarmer, Vector2 startingPosition, string firingSound, GameLocation location, Character firer)
            : base(damageToFarmer, 15, 0, 1, (float)Math.PI / 16f, 0f, 0f, startingPosition, "", firingSound, false, false, location, firer, false, null)
        {
            InitNetFields();

            Vector2 targetPosition = new((tileToExplodeAt.X * 64) + 32 - (getBoundingBox().Width / 2), (tileToExplodeAt.Y * 64) + 32 - (getBoundingBox().Height / 2));

            Vector2 velocity = targetPosition - startingPosition;
            velocity.Normalize();
            velocity *= speedMultiplier;

            TargetDistance.Value = (targetPosition - startingPosition).Length();

            xVelocity.Value = velocity.X;
            yVelocity.Value = velocity.Y;
        }

        private void InitNetFields()
        {
            NetFields.AddFields(TargetDistance);
        }

        public override bool update(GameTime time, GameLocation location)
        {
            if (travelDistance >= TargetDistance)
            {
                location.playSound("explosion");
                location.explode(new(position.X / 64, position.Y / 64), 1, Game1.player, true, damageToFarmer.Value);
                location.projectiles.Remove(this);
            }

            return base.update(time, location);
        }
    }
}
