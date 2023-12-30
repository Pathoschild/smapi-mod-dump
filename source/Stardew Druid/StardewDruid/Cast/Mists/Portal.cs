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
    internal class Portal : CastHandle
    {

        public Portal(Vector2 target, Rite rite)
            : base(target, rite)
        {

            castCost = 0;

        }

        public override void CastEffect()
        {

            Event.World.Portal portalEvent = new(targetVector, riteData);

            portalEvent.EventTrigger();

            castLimit = true;

            castFire = true;

        }

    }

}
