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
using StardewDruid.Cast;
using StardewDruid.Event;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;

namespace StardewDruid.Cast.Fates
{
    public class WhiskEvent : EventHandle
    {

        public Vector2 origin;

        public Vector2 destination;

        public int whiskCounter;

        public SpellHandle whiskSpell;

        public WhiskEvent(Vector2 target,  Vector2 Destination)
            : base(target)
        {

            origin = targetVector * 64f;

            destination = Destination * 64;

        }

        public override void EventTrigger()
        {

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 2;

            SpellHandle whiskSpell = new(targetLocation, destination, origin,3);

            whiskSpell.scheme = SpellHandle.schemes.fates;

            whiskSpell.indicator = SpellHandle.indicators.fates;

            whiskSpell.TargetCircle();

            whiskSpell.LaunchMissile();

            animations = whiskSpell.animations;

            Mod.instance.RegisterEvent(this, "whisk");

            Mod.instance.clickRegister[1] = "whisk";

        }

        public override bool EventPerformAction(SButton Button, string Type)
        {

            if (Type != "Action")
            {

                return false;

            }

            if (!EventActive())
            {

                return false;

            }

            if (!Mod.instance.rite.castTask.ContainsKey("masterWhisk"))
            {

                Mod.instance.UpdateTask("lessonWhisk", 1);

            }

            PerformWarp();

            return true;

        }

        public override bool EventActive()
        {

            if (whiskCounter > 12)
            {

                return false;

            }

            return base.EventActive();

        }

        public override void EventDecimal()
        {
            whiskCounter++;
        }

        public override void EventInterval()
        {

            if (Mod.instance.rite.caster.isRidingHorse())
            {

                PerformWarp();

            }

        }

        public void PerformWarp()
        {

            Game1.flashAlpha = 1;

            Vector2 distance = (destination - origin) / 13 * (whiskCounter + 1);

            Vector2 arrive = origin + distance;

            Mod.instance.rite.caster.Position = arrive;

            ModUtility.AnimateQuickWarp(targetLocation, arrive);

            RemoveAnimations();

            expireEarly = true;

        }


    }

}
