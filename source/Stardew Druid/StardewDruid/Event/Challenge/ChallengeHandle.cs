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
using StardewDruid.Monster;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;

namespace StardewDruid.Event.Challenge
{
    public class ChallengeHandle : EventHandle
    {

        public readonly Quest questData;

        public Vector2 challengeWithin;

        public Vector2 challengeRange;

        public List<Vector2> challengeTorches;

        public int challengeSeconds;

        public int challengeFrequency = 1;

        public int challengeAmplitude = 1;

        public List<int> challengeSpawn = new();

        public int challengeZone = 24;

        public int challengeWarning;

        public ChallengeHandle(Vector2 target, Rite rite, Quest quest)
            : base(target, rite)
        {

            questData = quest;

        }

        public override void EventTrigger()
        {

            Mod.instance.RegisterEvent(this, "active");

            Game1.addHUDMessage(new HUDMessage($"Challenge Initiated", ""));

            //Mod.instance.lockoutRegister.Add("earth", true);

        }

        public void SetupSpawn()
        {

            monsterHandle = new(targetVector, riteData);

            monsterHandle.spawnIndex = challengeSpawn;

            monsterHandle.spawnFrequency = challengeFrequency;

            monsterHandle.spawnAmplitude = challengeAmplitude;

            monsterHandle.spawnWithin = challengeWithin;

            monsterHandle.spawnRange = challengeRange;

            List<Vector2> spawnTorches = challengeTorches;

            foreach (Vector2 torchVector in spawnTorches)
            {

                Torch torch = ModUtility.StoneBrazier(riteData.castLocation, torchVector);

                torchList.Add(torch);

            }

            int runTime = challengeSeconds == 0 ? 45 : challengeSeconds;

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + runTime;

        }

        public override bool EventActive()
        {

            if (Vector2.Distance(Game1.player.getTileLocation(), targetVector) > challengeZone)
            {

                challengeWarning++;

                Game1.addHUDMessage(new HUDMessage("Challenge will fail if you leave!", 2));

                if(challengeWarning > 5)
                {

                    eventAbort = true;

                }

            }

            return base.EventActive();

        }

        public override void EventAbort()
        {

            Game1.addHUDMessage(new HUDMessage($"Try the challenge again tomorrow", ""));

        }


        public void UpdateFriendship(List<string> NPCIndex)
        {

            foreach (string NPCName in NPCIndex)
            {

                NPC characterFromName = Game1.getCharacterFromName(NPCName);

                characterFromName ??= Game1.getCharacterFromName<Child>(NPCName, mustBeVillager: false);

                if (characterFromName != null)
                {

                    targetPlayer.changeFriendship(375, characterFromName);

                }
                else
                {

                    //mod.Monitor.Log($"Unable to raise Friendship for {NPCName}", LogLevel.Debug);

                }

            }

        }


    }

}
