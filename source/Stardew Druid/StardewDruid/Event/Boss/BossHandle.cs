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

        public readonly Quest questData;

        public BossHandle(Vector2 target, Rite rite, Quest quest)
            : base(target, rite)
        {

            questData = quest;

            eventId = quest.name;

        }

        public override void EventTrigger()
        {

            Mod.instance.RegisterEvent(this, "active");

            Game1.addHUDMessage(new HUDMessage($"Boss fight initiated", ""));

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

                    foreach (Brazier brazier in braziers)
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
            Game1.addHUDMessage(new HUDMessage($"{minutes} minutes left to defeat the boss!", "2"));
        }

        public virtual void EventComplete()
        {

            Mod.instance.CompleteQuest(questData.name);

            EventQuery();

        }

        public virtual void EventQuery(string eventQuery = "EventComplete")
        {

            if (Context.IsMultiplayer)
            {
                QueryData queryData = new()
                {
                    name = questData.name,
                    value = questData.name,
                    description = questData.questTitle,
                    time = Game1.currentGameTime.TotalGameTime.TotalMilliseconds,
                    location = riteData.castLocation.Name,
                    expire = (int)expireTime,
                    targetX = (int)targetVector.X,
                    targetY = (int)targetVector.Y,
                };

                Mod.instance.EventQuery(queryData, eventQuery);

            }

        }

        public override void EventAbort()
        {

            Game1.addHUDMessage(new HUDMessage($"Boss fight aborted", ""));

            base.EventAbort();

        }


    }

}
