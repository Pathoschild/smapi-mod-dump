/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace FireArcadeGame.Objects
{
    public class Enemy : Character
    {
        public Enemy( World world )
        :   base( world )
        {
        }

        public override void Hurt( int amt )
        {
            base.Hurt( amt );
            if ( Health <= 0 )
            {
                Dead = true;
            }
        }

        public override void Update()
        {
            base.Update();

            foreach ( var proj in World.projectiles )
            {
                if ( proj.Dead )
                    continue;

                if ( ( proj.BoundingBox + new Vector2( proj.Position.X, proj.Position.Z ) ).Intersects( BoundingBox + new Vector2( Position.X, Position.Z ) ) && !proj.HurtsPlayer )
                {
                    proj.Trigger( this );
                }
            }

            if ( ( World.player.BoundingBox + new Vector2( World.player.Position.X, World.player.Position.Z ) ).Intersects( BoundingBox + new Vector2( Position.X, Position.Z ) ) )
            {
                World.player.Hurt( 1 );
            }
        }
    }
}
