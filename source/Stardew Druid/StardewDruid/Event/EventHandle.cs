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
using Microsoft.Xna.Framework.Audio;
using StardewDruid.Cast;
using StardewDruid.Character;
using StardewDruid.Dialogue;
using StardewDruid.Map;
using StardewDruid.Monster;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Layers;

namespace StardewDruid.Event
{
    public class EventHandle
    {

        public string eventId;

        public double eventTime;

        public bool eventLock;

        public int activeCounter;

        public GameLocation targetLocation;

        public Vector2 targetVector;

        public Farmer targetPlayer;

        public double expireTime;

        public bool expireEarly;

        public bool eventAbort;

        public int eventLinger;

        public Random randomIndex;

        public MonsterHandle monsterHandle;

        public List<StardewDruid.Event.LightHandle> braziers;

        public List<StardewDruid.Character.Actor> actors;

        public List<TemporaryAnimatedSprite> animations;

        public bool soundTrack;

        public List<Vector2> ladders;

        public Dictionary<int, Dictionary<int, string>> cues;

        public Dictionary<int, Narrator> narrators;

        public Dictionary<int, StardewValley.NPC> voices;

        public Quest questData;

        public int sceneCounter;

        public EventHandle(Vector2 target)
        {

            eventTime = Game1.currentGameTime.TotalGameTime.TotalMilliseconds;

            targetVector = target;

            targetPlayer = Game1.player;

            targetLocation = Game1.player.currentLocation;

            randomIndex = Mod.instance.rite.randomIndex;

            eventLinger = -1;

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 60;

            braziers = new();

            actors = new();

            ladders = new();

            animations = new();

            cues = new();

        }

        public virtual void EventTrigger()
        {

            Mod.instance.RegisterEvent(this, "event");

        }

        public virtual void MinutesLeft(int minutes)
        {

            Mod.instance.CastMessage($"{minutes} more minutes left!",2);
        }

        public virtual bool EventActive()
        {

            if (eventAbort)
            {

                EventAbort();

                return false;

            }

            if (targetPlayer.currentLocation.Name != targetLocation.Name)
            {

                EventAbort();

                return false;

            }

            if (expireEarly)
            {

                return EventExpire();

            }

            double nowTime = Game1.currentGameTime.TotalGameTime.TotalSeconds;

            if (expireTime < nowTime)
            {

                return EventExpire();

            }

            return true;

        }

        public virtual bool EventPerformAction(SButton Button, string type = "Action")
        {

            return false;

        }

        public virtual void EventDecimal()
        {


        }

        public virtual bool EventExtend()
        {

            expireTime++;

            return true;

        }

        public virtual bool AttemptAbort()
        {

            return true;

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

        public virtual void RemoveBraziers()
        {

            if (braziers.Count > 0)
            {

                foreach (LightHandle brazier in braziers)
                {

                    brazier.shutdown();

                }

                braziers.Clear();

            }

        }

        public void ResetBraziers()
        {

            for (int i = braziers.Count - 1; i >= 0; i--)
            {

                LightHandle brazier = braziers.ElementAt(i);

                if (!brazier.reset())
                {

                    braziers.RemoveAt(i);

                }

            }

        }

        public virtual void RemoveActors()
        {

            if (actors.Count > 0)
            {

                foreach (StardewDruid.Character.Character actor in actors)
                {

                    actor.currentLocation.characters.Remove(actor);

                }

                actors.Clear();

            }

        }

        public virtual void RemoveAnimations()
        {
            
            if (animations.Count > 0)
            {
                
                foreach (TemporaryAnimatedSprite animation in animations)
                {
                    
                    targetLocation.temporarySprites.Remove(animation);
                
                }

                animations.Clear();
            
            }

        }

        public virtual void EventRemove()
        {

            RemoveMonsters();

            RemoveBraziers();

            RemoveActors();

            RemoveAnimations();

            if (soundTrack)
            {

                Game1.stopMusicTrack(MusicContext.Default);

            }

        }

        public virtual void EventInterval()
        {

            activeCounter++;

        }

        public virtual void DialogueCue(int cueIndex = -1)
        {

            DialogueCue(narrators, voices, cueIndex);

        }

        public virtual void DialogueCue(int voice,string text)
        {

            cues = new() { [0] = new() { [voice] = text, }, };

            DialogueCue(narrators, voices, 0);

        }

        public virtual void DialogueCue(Dictionary<int, Dialogue.Narrator> narrators, Dictionary<int,StardewValley.NPC> voices, int cueIndex = -1)
        {

            if (cues.ContainsKey(cueIndex))
            {

                foreach(KeyValuePair<int,string> cue in cues[cueIndex])
                {

                    if (voices.ContainsKey(cue.Key))
                    {

                        if(cue.Value == "...")
                        {
                            voices[cue.Key].doEmote(40);

                            continue;
                        }
                        else if(cue.Value == "!")
                        {

                            voices[cue.Key].doEmote(16);

                            continue;
                        }
                        else if (cue.Value == "?")
                        {

                            voices[cue.Key].doEmote(8);

                            continue;
                        }
                        else if (cue.Value == "x")
                        {

                            voices[cue.Key].doEmote(36);

                            continue;
                        }
                        voices[cue.Key].showTextAboveHead(cue.Value);

                    }

                    if (narrators.ContainsKey(cue.Key))
                    {

                        narrators[cue.Key].DisplayHUD(cue.Value);

                        if (Context.IsMultiplayer)
                        {
                            QueryData queryData = new()
                            {
                                name = questData.name,
                                value = cueIndex.ToString(),
                            
                            };

                            Mod.instance.EventQuery(queryData, "DialogueDisplay");

                        }

                    }



                }

            }

        }

        public void CastVoice(string message, int duration = 2000)
        {

            if (actors.Count <= 0)
            {

                this.AddActor(targetVector * 64);
            }

            actors.First().showTextAboveHead(message, duration: duration);


        }

        public void SetTrack(string track)
        {

            Game1.changeMusicTrack(track, false, MusicContext.Default);

            soundTrack = true;

        }

        public void AddActor(Vector2 position, bool slave = false)
        {
            
            Actor actor = CharacterData.DisembodiedVoice(this.targetLocation, position);

            actor.drawSlave = slave;

            targetLocation.characters.Add(actor);

            actors.Add(actor);

        }

        public void RemoveLadders()
        {
            Layer layer = targetLocation.map.GetLayer("Buildings");

            for (int index1 = 0; index1 < layer.LayerHeight; ++index1)
            {
                
                for (int index2 = 0; index2 < layer.LayerWidth; ++index2)
                {
                    
                    if (layer.Tiles[index2, index1] != null && layer.Tiles[index2, index1].TileIndex == 173)
                    {
                        
                        layer.Tiles[index2, index1] = null;
                        
                        Game1.player.TemporaryPassableTiles.Clear();
                        
                        if (ladders.Count == 0)
                        {
                            ladders.Add(new Vector2(index2, index1));
                        }

                    }
                
                }
            
            }
        
        }

        public virtual void EventScene(int index)
        {
            activeCounter = index;
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
                    location = Mod.instance.rite.castLocation.Name,
                };

                Mod.instance.EventQuery(queryData, eventQuery);

            }

        }

    }

}
