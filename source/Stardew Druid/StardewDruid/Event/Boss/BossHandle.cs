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
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;

namespace StardewDruid.Event.Boss
{
    public class BossHandle : EventHandle
    {

        public BossHandle(Vector2 target,  Quest quest)
            : base(target)
        {

            questData = quest;

            eventId = quest.name;

        }

        public override void EventTrigger()
        {

            Mod.instance.RegisterEvent(this, "active");

            Mod.instance.CastMessage("Boss fight initiated");

        }

        public override bool EventActive()
        {

            int diffTime = (int)Math.Round(expireTime - Game1.currentGameTime.TotalGameTime.TotalSeconds);

            if (activeCounter != 0 && diffTime % 10 == 0 && diffTime != 0)
            {

                MinutesLeft(diffTime);

            }

            if (activeCounter % 30 == 0)
            {

                if (braziers.Count > 0)
                {

                    foreach (LightHandle brazier in braziers)
                    {

                        brazier.reset();

                    }

                }

            }

            if (Context.IsMultiplayer)
            {

                if (eventLock)
                {

                    return false;

                }

            }

            return base.EventActive();

        }

        public override void MinutesLeft(int minutes)
        {

            Mod.instance.CastMessage($"{minutes} minutes left to defeat the boss!",2);

        }

        public virtual void EventComplete()
        {

            Mod.instance.CompleteQuest(questData.name);

            EventQuery();

        }


        public override void EventAbort()
        {

            Mod.instance.CastMessage("Boss fight aborted, try again tomorrow!",3,true);

            base.EventAbort();

        }

    }

}
