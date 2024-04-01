/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;


namespace StardewDruid.Cast.Mists
{
    internal class Rod : CastHandle
    {

        public Rod(Vector2 target)
            : base(target)
        {
            castCost = 0;
        }

        public override void CastEffect()
        {

            StardewValley.Object targetObject = targetLocation.objects[targetVector];

            targetObject.heldObject.Value = new StardewValley.Object("787", 1);

            targetObject.MinutesUntilReady = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay);

            targetObject.shakeTimer = 1000;

            ModUtility.AnimateBolt(targetLocation, targetVector * 64 + new Vector2(32));

            return;

        }

    }

}
