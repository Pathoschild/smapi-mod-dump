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
using System;

namespace StardewDruid.Cast.Fates
{
    public class Gravity : CastHandle
    {

        public int type;

        public Gravity(Vector2 target, Rite rite, int Type = 0)
            : base(target, rite)
        {

            int castCombat = rite.caster.CombatLevel / 2;

            castCost = Math.Max(6, 12 - castCombat);

            type = Type;

        }

        public override void CastEffect()
        {

            if (!riteData.castTask.ContainsKey("masterGravity"))
            {

                Mod.instance.UpdateTask("lessonGravity", 1);

            }

            Event.World.Gravity gravityEvent = new(targetVector, riteData, type,Mod.instance.DamageLevel());

            gravityEvent.EventTrigger();

            castFire = true;

        }

    }

}
