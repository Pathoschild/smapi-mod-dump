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
using StardewDruid.Character;
using StardewDruid.Map;
using StardewDruid.Monster;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace StardewDruid.Event
{
    public class EventHandle
    {

        public int activeCounter;

        public readonly Rite riteData;

        public GameLocation targetLocation;

        public Vector2 targetVector;

        public Farmer targetPlayer;

        public double expireTime;

        public bool expireEarly;

        public bool eventAbort;

        public int eventLinger;

        public Random randomIndex;

        public MonsterHandle monsterHandle;

        public List<Torch> torchList;

        public List<Actor> actors;

        public Vector2 voicePosition;

        public bool soundTrack;

        public bool eventSync;

        public EventHandle(Vector2 target, Rite rite)
        {

            riteData = rite;

            targetVector = target;

            targetPlayer = rite.caster;

            targetLocation = rite.castLocation;

            randomIndex = rite.randomIndex;

            eventLinger = -1;

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 60;

            voicePosition = target * 64;

            torchList = new();

            actors = new();

        }

        public virtual void EventTrigger()
        {

            Mod.instance.RegisterEvent(this, "event");

        }

        public virtual void MinutesLeft(int minutes)
        {

            Game1.addHUDMessage(new HUDMessage($"{minutes} more minutes left!", "2"));

        }

        public virtual bool EventActive()
        {

            if (targetPlayer.currentLocation == targetLocation && !eventAbort)
            {

                double nowTime = Game1.currentGameTime.TotalGameTime.TotalSeconds;

                if (expireTime >= nowTime && !expireEarly)
                {

                    int diffTime = (int)Math.Round(expireTime - nowTime);

                    if (activeCounter != 0 && diffTime % 10 == 0 && diffTime != 0)
                    {

                        MinutesLeft(diffTime);

                    }

                    return true;

                }

                return EventExpire();

            }
            else
            {

                EventAbort();

            }

            return false;

        }

        public virtual bool EventPerformAction(SButton Button)
        {

            return false;

        }

        public virtual void EventDecimal()
        {


        }

        public virtual void EventExtend()
        {

            expireTime++;

        }

        public virtual void EventAbort()
        {

            eventLinger = 0;

        }

        public virtual bool EventExpire()
        {

            if (eventLinger > 0)
            {

                eventLinger--;

                return true;

            }

            return false;

        }

        public virtual void RemoveMonsters()
        {

            if (monsterHandle != null)
            {

                monsterHandle.ShutDown();

            }

        }

        public virtual void EventRemove()
        {

            RemoveMonsters();

            if (torchList.Count > 0)
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

                Game1.playSound("fireball");

            }

            if (actors.Count > 0)
            {

                foreach (StardewDruid.Character.Character actor in actors)
                {

                    actor.currentLocation.characters.Remove(actor);

                }

                actors.Clear();


            }

            if (soundTrack)
            {

                Game1.stopMusicTrack(Game1.MusicContext.Default);

            }

        }

        public virtual void EventInterval()
        {

            activeCounter++;

            if (eventLinger != -1)
            {

                return;

            }

            if (monsterHandle != null)
            {

                monsterHandle.SpawnInterval();

            }

        }

        public void CastVoice(string message, int duration = 2000)
        {

            if (actors.Count <= 0)
            {

                this.AddActor(this.voicePosition);

            }

            actors[0].showTextAboveHead(message, duration: duration);

        }

        public void SetTrack(string track)
        {

            Game1.changeMusicTrack(track, false, Game1.MusicContext.Default);

            soundTrack = true;

        }

        public void AddActor(Vector2 position, bool slave = false)
        {
            Actor actor = CharacterData.DisembodiedVoice(this.targetLocation, position);
            actor.drawSlave = slave;
            targetLocation.characters.Add(actor);
            actors.Add(actor);
        }

    }

}
