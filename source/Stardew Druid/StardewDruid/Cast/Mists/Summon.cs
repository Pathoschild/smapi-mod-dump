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

namespace StardewDruid.Cast.Mists
{
    internal class Summon : CastHandle
    {

        public Summon(Vector2 target)
            : base(target)
        {

            castCost = 0;

        }

        public override void CastEffect()
        {

            Event.Challenge.Summon portalEvent = new(targetVector);

            portalEvent.EventTrigger();

            castLimit = true;

            castFire = true;

        }

    }

}
