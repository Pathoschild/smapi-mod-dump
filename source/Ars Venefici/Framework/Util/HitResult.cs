/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Util
{
    public abstract class HitResult
    {
        protected Vector2 location;
        private float direction;

        protected HitResult(Vector2 location, float direction) 
        {
            this.location = location;
            this.direction = direction;
        }

        public float DistanceTo​(Character character)
        {
            //return Vector2.Distance(location, character.getStandingPosition());

            double d0 = location.X - character.getStandingPosition().X;
            double d1 = location.Y - character.getStandingPosition().Y;
            return (float)(d0 * d0 + d1 * d1);
        }

        public Vector2 GetLocation()
        {
            return location;
        }

        public float GetDirection()
        {
            return direction;
        }

        public virtual HitResultType GetHitResultType() 
        {
            return HitResultType.MISS;
        }

        public enum HitResultType
        {
            TERRAIN_FEATURE,
            CHARACTER,
            MISS
        }
    }
}
