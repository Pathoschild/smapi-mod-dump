/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/FireArcadeGame
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
