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
using StardewDruid.Character;
using System;

namespace StardewDruid.Cast.Mists
{
    internal class Artifact : CastHandle
    {

        public Artifact(Vector2 target)
            : base(target)
        {

        }

        public override void CastEffect()
        {
            if (!targetLocation.objects.ContainsKey(targetVector))
            {
                return;

            }

            StardewValley.Object artifactSpot = targetLocation.objects[targetVector];

            if (artifactSpot == null)
            {
                return;

            }

            targetLocation.digUpArtifactSpot((int)targetVector.X, (int)targetVector.Y, targetPlayer);

            targetLocation.objects.Remove(targetVector);

            castFire = true;

            castCost = 8;

            ModUtility.AnimateBolt(targetLocation, targetVector * 64 + new Vector2(32));

            return;

        }

    }
}
