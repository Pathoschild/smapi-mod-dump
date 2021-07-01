/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace PyromancersJourney.Framework.Objects
{
    internal class Enemy : Character
    {
        public Enemy(World world)
            : base(world) { }

        public override void Hurt(int amt)
        {
            base.Hurt(amt);
            if (this.Health <= 0)
            {
                this.Dead = true;
            }
        }

        public override void Update()
        {
            base.Update();

            foreach (var proj in this.World.Projectiles)
            {
                if (proj.Dead)
                    continue;

                if ((proj.BoundingBox + new Vector2(proj.Position.X, proj.Position.Z)).Intersects(this.BoundingBox + new Vector2(this.Position.X, this.Position.Z)) && !proj.HurtsPlayer)
                {
                    proj.Trigger(this);
                }
            }

            if ((this.World.Player.BoundingBox + new Vector2(this.World.Player.Position.X, this.World.Player.Position.Z)).Intersects(this.BoundingBox + new Vector2(this.Position.X, this.Position.Z)))
            {
                this.World.Player.Hurt(1);
            }
        }
    }
}
