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

namespace FireArcadeGame
{
    public class Camera
    {
        public Vector3 pos = new Vector3( 10, 0, 10 );
        public Vector3 up = Vector3.Up;
        public Vector3 target = Vector3.Zero;

        public Matrix CreateViewMatrix()
        {
            return Matrix.CreateLookAt( pos, target, up );
        }
    }
}
