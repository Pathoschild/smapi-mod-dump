/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace BetterSlingshots.Framework
{
    internal class Utils
    {
        public static Vector2 GetCorrectVelocity(Vector2 aimPos, Farmer who, bool reverse)
        {
            Vector2 velocity = Utility.getVelocityTowardPoint(Utils.GetCorrectFarmerPosition(who), aimPos, (float)(15 + Game1.random.Next(4, 6)) * (1f + who.weaponSpeedModifier));

            return reverse ? velocity * -1 : velocity;
        }

        public static Vector2 GetCorrectFarmerPosition(Farmer who)
        {
            return new Vector2(who.getStandingX() - 16 + 32, who.getStandingY() - 64 - 8 + 32);
        }
    }
}
