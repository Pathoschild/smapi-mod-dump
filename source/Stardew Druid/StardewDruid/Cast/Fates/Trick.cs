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

namespace StardewDruid.Cast.Fates
{
    internal class Trick : CastHandle
    {

        public NPC riteWitness;

        public Trick(Vector2 target, NPC witness)
            : base(target)
        {

            riteWitness = witness;

        }

        public override void CastEffect()
        {
            
            TrickEvent trickEvent = new(targetVector, riteWitness);

            trickEvent.EventTrigger();

            castFire = true;

        }

    }

}
