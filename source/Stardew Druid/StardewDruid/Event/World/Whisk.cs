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
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;

namespace StardewDruid.Event.World
{
    public class Whisk : EventHandle
    {

        public Vector2 origin;

        public Vector2 destination;


        public Whisk(Vector2 target, Rite rite, Vector2 Destination)
            : base(target, rite)
        {

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 2;

            origin = targetVector * 64f;

            destination = Destination * 64;

        }

        public override void EventTrigger()
        {

            animations.Add(ModUtility.AnimateFateTarget(targetLocation, origin, destination));

            Mod.instance.RegisterEvent(this, "whisk");
        
            Mod.instance.clickRegister[1] = "whisk";
        
        }

        public override bool EventPerformAction(SButton Button, string Type)
        {

            if(Type != "Action")
            {

                return false;

            }

            if (!EventActive())
            {

                return false;

            }

            if (!riteData.castTask.ContainsKey("masterWhisk"))
            {

                Mod.instance.UpdateTask("lessonWhisk", 1);

            }

            PerformWarp();

            return true;

        }

        public override void EventInterval()
        {

            if (riteData.caster.isRidingHorse())
            {

                PerformWarp();

            }

        }

        public void PerformWarp()
        {

            Game1.flashAlpha = 1;

            riteData.caster.Position = destination;

            ModUtility.AnimateQuickWarp(targetLocation, destination - new Vector2(0, 32), "Solar");

            RemoveAnimations();

            expireEarly = true;

        }


    }

}
