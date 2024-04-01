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

namespace StardewDruid.Cast.Fates
{
    internal class Whisk : CastHandle
    {

        public Vector2 destination;

        public Whisk(Vector2 target,  Vector2 Destination)
            : base(target)
        {

            destination = Destination;

        }

        public override void CastEffect()
        {
            
            WhiskEvent whiskEvent = new(targetVector, destination);

            whiskEvent.EventTrigger();

            castFire = true;

        }

    }

}
