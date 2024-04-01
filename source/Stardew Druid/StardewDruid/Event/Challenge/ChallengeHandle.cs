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

namespace StardewDruid.Event.Challenge
{
    public class ChallengeHandle : EventHandle
    {

        public Vector2 challengeWithin;

        public Vector2 challengeRange;

        public List<Vector2> challengeTorches;

        public int challengeSeconds;

        public int challengeFrequency = 1;

        public int challengeAmplitude = 1;

        public List<int> challengeSpawn = new();

        public int challengeZone = 24;

        public int challengeWarning;

        public ChallengeHandle(Vector2 target,  Quest quest)
            : base(target)
        {

            questData = quest;

            eventId = quest.name;

        }

        public override void EventTrigger()
        {

            Mod.instance.RegisterEvent(this, "active");

            Mod.instance.CastMessage("Challenge Initiated");

            /*if (Context.IsMultiplayer)
            {

                activeCounter = -1;

                QueryData queryData = new()
                {
                    name = questData.name,
                    value = questData.name,
                    description = questData.questTitle,
                    time = Game1.currentGameTime.TotalGameTime.TotalMilliseconds,
                    location = Mod.instance.rite.castLocation.Name,
                    expire = (int)expireTime,
                };

                Mod.instance.EventQuery(queryData);

            }*/

        }

        public void SetupSpawn()
        {

            monsterHandle = new(targetVector, Mod.instance.rite.castLocation);

            monsterHandle.spawnIndex = challengeSpawn;

            monsterHandle.spawnFrequency = challengeFrequency;

            monsterHandle.spawnAmplitude = challengeAmplitude;

            monsterHandle.spawnWithin = challengeWithin;

            monsterHandle.spawnRange = challengeRange;

            if (questData.name.Contains("Two"))
            {

                monsterHandle.spawnCombat *= 2;

            }

            List<Vector2> spawnTorches = challengeTorches;

            foreach (Vector2 torchVector in spawnTorches)
            {

                LightHandle brazier = new(targetLocation, torchVector);

                braziers.Add(brazier);

            }

            int runTime = challengeSeconds == 0 ? 45 : challengeSeconds;

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + runTime;

        }

        public override bool EventActive()
        {

            if (Vector2.Distance(Game1.player.Tile, targetVector) > challengeZone)
            {

                challengeWarning++;

                Game1.addHUDMessage(new HUDMessage("Challenge will fail if you leave!", 2));

                if (challengeWarning > 5)
                {

                    eventAbort = true;

                }

            }

            int diffTime = (int)Math.Round(expireTime - Game1.currentGameTime.TotalGameTime.TotalSeconds);

            if (activeCounter != 0 && diffTime % 10 == 0 && diffTime != 0)
            {

                MinutesLeft(diffTime);

            }

            if (activeCounter % 30 == 0)
            {
                
                if(braziers.Count > 0)
                {
                    
                    foreach(LightHandle brazier in braziers) {

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

            Mod.instance.CastMessage($"Hold ground for another {minutes} minutes",2);

        }

        public override void EventAbort()
        {

            Mod.instance.CastMessage("Try the challenge again tomorrow");

        }

        public override bool EventExpire()
        {

            EventComplete();

            RemoveMonsters();

            return base.EventExpire();

        }

        public virtual void EventComplete()
        {

            Mod.instance.CompleteQuest(questData.name);

            EventQuery();
        
        }

        public override void EventInterval()
        {
            
            activeCounter++;

            monsterHandle.SpawnCheck();

            if (eventLinger != -1)
            {

                return;

            }

            monsterHandle.SpawnInterval();

            if (activeCounter % 30 == 0)
            {

                ResetBraziers();

            }

        }

    }

}
