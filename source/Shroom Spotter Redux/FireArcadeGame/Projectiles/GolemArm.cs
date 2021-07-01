/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using FireArcadeGame.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace FireArcadeGame.Projectiles
{
    public class GolemArm : BaseProjectile
    {
        public static Texture2D tex = Mod.instance.Helper.Content.Load< Texture2D >( "assets/golem_arm.png" );

        public Vector2 Speed;

        private static VertexBuffer buffer;

        public override bool HurtsPlayer => true;
        public override int Damage => 1;

        public GolemArm( World world )
        :   base( world )
        {
            if ( buffer == null )
            {
                float a = 0;
                float b = 1;
                var vertices = new List<VertexPositionColorTexture>();
                vertices.Add( new VertexPositionColorTexture( new Vector3( 0, 0, 0 ), Color.White, new Vector2( a, 0 ) ) );
                vertices.Add( new VertexPositionColorTexture( new Vector3( 0.5f, 0, 0 ), Color.White, new Vector2( b, 0 ) ) );
                vertices.Add( new VertexPositionColorTexture( new Vector3( 0.5f, 1, 0 ), Color.White, new Vector2( b, 1 ) ) );

                vertices.Add( new VertexPositionColorTexture( new Vector3( 0, 0, 0 ), Color.White, new Vector2( a, 0 ) ) );
                vertices.Add( new VertexPositionColorTexture( new Vector3( 0, 1, 0 ), Color.White, new Vector2( a, 1 ) ) );
                vertices.Add( new VertexPositionColorTexture( new Vector3( 0.5f, 1, 0 ), Color.White, new Vector2( b, 1 ) ) );

                buffer = new VertexBuffer( Game1.game1.GraphicsDevice, typeof( VertexPositionColorTexture ), vertices.Count(), BufferUsage.WriteOnly );
                buffer.SetData( vertices.ToArray() );
            }
        }

        public override void Trigger( BaseObject target )
        {
            if ( target is Player player )
            {
                player.Hurt( Damage );
                Dead = true;
            }
        }

        public override void Update()
        {
            base.Update();
            Position += new Vector3( Speed.X, 0, Speed.Y );

            if ( World.map.IsAirSolid( Position.X, Position.Z ) )
            {
                Dead = true;
            }
        }

        public override void Render( GraphicsDevice device, Matrix projection, Camera cam )
        {
            base.Render( device, projection, cam );
            var camForward = ( cam.pos - cam.target );
            camForward.Normalize();
            effect.World = Matrix.CreateConstrainedBillboard( Position, cam.pos, cam.up, null, null );
            effect.TextureEnabled = true;
            effect.Texture = tex;
            for ( int e = 0; e < effect.CurrentTechnique.Passes.Count; ++e )
            {
                var pass = effect.CurrentTechnique.Passes[ e ];
                pass.Apply();
                device.SetVertexBuffer( buffer );
                device.DrawPrimitives( PrimitiveType.TriangleList, 0, 2 );
            }
        }
    }
}
