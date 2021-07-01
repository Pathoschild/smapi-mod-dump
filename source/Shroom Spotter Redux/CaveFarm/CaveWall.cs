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
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace CaveFarm
{
    public class CaveWall : TerrainFeature
    {
        public readonly NetInt health = new NetInt();

        public CaveWall()
        :   base( false )
        {
            this.health.Value = 3;
            NetFields.AddField( health );
        }

        public override Rectangle getBoundingBox( Vector2 tileLocation )
        {
            return new Rectangle( ( int ) tileLocation.X * 64, ( int ) tileLocation.Y * 64, 64, 64 );
        }

        public override bool performToolAction( Tool t, int damage, Vector2 tileLocation, GameLocation location )
        {
            if ( t is Pickaxe pickaxe )
            {
                location.playSound( "hammer" );
                health.Value -= ( (pickaxe.UpgradeLevel + 1) / 2 ) + 1;
            }
            else if ( damage > 0 )
            {
                health.Value -= damage;
            }

            if ( health > 0 )
                return true;
            return false;
        }

        public override void draw( SpriteBatch b, Vector2 tileLocation )
        {
            int x = (int) tileLocation.X;
            int y = (int) tileLocation.Y;
            b.Draw( Game1.staminaRect, Game1.GlobalToLocal( Game1.viewport, new Rectangle( x * Game1.tileSize, y * Game1.tileSize - Game1.tileSize * 2, Game1.tileSize, Game1.tileSize * 3 ) ), null, Color.Black, 0, Vector2.Zero, SpriteEffects.None, (y + 1) * 64 / 10000f - x * 64 / 1000000f );
        }
    }
}
