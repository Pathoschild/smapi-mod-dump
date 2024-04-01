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

namespace StardewDruid.Cast.Weald
{
    internal class Trough : CastHandle
    {

        public Trough(Vector2 target)
            : base(target)
        {

        }

        public override void CastEffect()
        {
            if (targetLocation.objects.ContainsKey(targetVector))
            {
                return;
            }

            targetLocation.objects.Add(targetVector, new StardewValley.Object("178", 1));

            return;

        }

    }

}
