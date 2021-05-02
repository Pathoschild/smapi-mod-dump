/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Revitalize.Framework.Utilities
{
    public class RotationUtilities
    {
        public static float get90Degrees()
        {
            float angle = (float)Math.PI * .5f;
            return angle;
        }

        public static float get180Degrees()
        {
            float angle = (float)Math.PI;
            return angle;
        }

        public static float get270Degrees()
        {
            float angle = (float)Math.PI * (1.5f);
            return angle;
        }

        public static float get360Degrees()
        {
            return 0;
        }

        /// <summary>
        /// Gets a rotation from the degrees passed in.
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static float getRotationFromDegrees(int degrees)
        {
            float amount = degrees / 180;
            float angle = (float)Math.PI * (amount);
            return angle;
        }

        /// <summary>
        /// Gets an angle from a passed in vector.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static float getAngleFromVector(Vector2 vec)
        {
            Vector2 zero = new Vector2(1,0);
            vec = vec.UnitVector();
            float dot=Vector2.Dot(zero, vec);
            float len1 = vec.Length();
            float len2 = zero.Length();
            float lenTotal = len1 * len2;
            float cosAngle = dot / lenTotal;

            float angle = (float)((Math.Acos(cosAngle)*180)/Math.PI);
            return angle;

        }

        public static float getRadiansFromVector(Vector2 vec)
        {
            Vector2 zero = new Vector2(1, 0);
            vec = vec.UnitVector();
            float dot = Vector2.Dot(zero, vec);
            float len1 = vec.Length();
            float len2 = zero.Length();
            float lenTotal = len1 * len2;
            float cosAngle = dot / lenTotal;

            float angle = (float)((Math.Acos(cosAngle)));
            return angle;
        }

        /// <summary>
        /// Gets a rotation amount for xna based off the unit circle.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static float getRotationFromVector(Vector2 vec)
        {
            return getRadiansFromVector(vec);
        }

    }
}
