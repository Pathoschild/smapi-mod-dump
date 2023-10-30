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

namespace StardewDruid.Event
{
    public class ChallengeHandle
    {

        public int activeCounter;

        public readonly Quest questData;

        public readonly Rite riteData;

        public GameLocation targetLocation;

        public Vector2 targetVector;

        public Farmer targetPlayer;

        public readonly Mod mod;

        public Vector2 challengeWithin;

        public Vector2 challengeRange;

        public List<Vector2> challengeTorches;

        public int challengeSeconds;

        public int challengeFrequency;

        public int challengeAmplitude;

        public List<int> challengeSpawn = new();

        public MonsterHandle monsterHandle;

        public List<Torch> torchList;

        public double expireTime;

        public bool expireEarly;

        public Random randomIndex;

        public NPC disembodiedVoice;

        public Vector2 voicePosition;

        public ChallengeHandle(Mod Mod, Vector2 target, Rite rite, Quest quest)
        {

            mod = Mod;

            riteData = rite;

            questData = quest;

            targetVector = target;

            targetPlayer = rite.caster;

            targetLocation = rite.castLocation;

            randomIndex = rite.randomIndex;

            voicePosition = target* 64;

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 60;

            torchList = new();

        }

        public virtual void EventTrigger()
        {

            mod.RegisterChallenge(this, "active");

            Game1.addHUDMessage(new HUDMessage($"Challenge Initiated", ""));

        }

        public void SetupSpawn()
        {

            monsterHandle = new(mod,targetVector,riteData);

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

        public virtual bool EventActive()
        {
            if (targetPlayer.currentLocation == targetLocation)
            {

                double nowTime = Game1.currentGameTime.TotalGameTime.TotalSeconds;

                if (expireTime >= nowTime && !expireEarly)
                {

                    int diffTime = (int)Math.Round(expireTime - nowTime);

                    if (activeCounter != 0 && diffTime % 10 == 0 && diffTime != 0)
                    {

                        Game1.addHUDMessage(new HUDMessage($"{diffTime} more minutes left!", "2"));

                    }

                    return true;

                }

                EventReward();

            }
            else
            {

                EventAbort();

            }

            return false;
        
        }

        public virtual void EventExtend()
        {

            expireTime++;

        }

        public virtual void EventReward()
        {


        }

        public virtual void EventAbort()
        {

            Game1.addHUDMessage(new HUDMessage($"Try the challenge again tomorrow", ""));

        }

        public virtual void EventRemove()
        {

            Game1.playSound("fireball");

            Game1.stopMusicTrack(Game1.MusicContext.Default);

            if (monsterHandle != null)
            {

                monsterHandle.ShutDown();

            }

            if(torchList.Count > 0)
            {
                
                foreach (Torch torch in torchList)
                {

                    targetLocation.objects.Remove(torch.TileLocation);

                    LightSource portalLight = torch.lightSource;

                    if (targetLocation.hasLightSource(portalLight.Identifier))
                    {

                        targetLocation.removeLightSource(portalLight.Identifier);

                    }

                    if (Game1.currentLightSources.Contains(portalLight))
                    {

                        Game1.currentLightSources.Remove(portalLight);
                    }

                }

                torchList.Clear();

            }

        }

        public virtual void EventInterval()
        {

            activeCounter++;

            if (monsterHandle != null)
            {

                monsterHandle.SpawnInterval();

            }

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

        public void CastVoice(string message, int duration = 2000)
        {

            if (disembodiedVoice == null)
            {

                disembodiedVoice = mod.RetrieveVoice(targetLocation, voicePosition);

            }

            disembodiedVoice.showTextAboveHead(message, duration: duration);

        }

    }

}
