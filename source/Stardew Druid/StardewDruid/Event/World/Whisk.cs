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
using StardewDruid.Event.Challenge;
using StardewDruid.Map;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace StardewDruid.Event.World
{
    public class Whisk : EventHandle
    {

        //public int source;

        Vector2 destination;

        //public Whisk(Vector2 target, Rite rite, Vector2 Destination, int Source)
        public Whisk(Vector2 target, Rite rite, Vector2 Destination)
            : base(target, rite)
        {

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 2;

            //source = Source;

            destination = Destination * 64;

        }

        public override void EventTrigger()
        {

            Mod.instance.RegisterEvent(this, "whisk");

        }

        public override bool EventActive()
        {

            if (expireEarly)
            {

                return false;

            }

            if(targetPlayer.currentLocation.Name != targetLocation.Name)
            {

                return false;

            }

            if (expireTime < Game1.currentGameTime.TotalGameTime.TotalSeconds)
            {

                return false;

            }

            return true;

        }

        public override bool EventPerformAction()
        {

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

            ModUtility.AnimateQuickWarp(targetLocation, destination - new Vector2(0,32));

            expireEarly = true;

        }


    }

}
