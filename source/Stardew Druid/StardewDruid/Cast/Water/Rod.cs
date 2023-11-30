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
using StardewModdingAPI;
using StardewValley;
using System;


namespace StardewDruid.Cast.Water
{
    internal class Rod : CastHandle
    {

        public Rod(Vector2 target, Rite rite)
            : base(target, rite)
        {
            castCost = 0;
        }

        public override void CastEffect()
        {

            StardewValley.Object targetObject = targetLocation.objects[targetVector];

            targetObject.heldObject.Value = new StardewValley.Object(787, 1);

            targetObject.MinutesUntilReady = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay);

            targetObject.shakeTimer = 1000;

            ModUtility.AnimateBolt(targetLocation, targetVector);

            return;

        }

    }

}
