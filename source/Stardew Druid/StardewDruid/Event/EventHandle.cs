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
using Microsoft.Xna.Framework.Graphics;
using StardewDruid.Cast;
using StardewDruid.Character;
using StardewDruid.Data;
using StardewDruid.Dialogue;
using StardewDruid.Journal;
using StardewDruid.Monster;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using xTile.Layers;

namespace StardewDruid.Event
{
    public class EventHandle
    {

        public string eventId = "none";

        public GameLocation location;

        public Vector2 origin = Vector2.Zero;

        // -------------------------------------
        // trigger management

        public bool triggerEvent;

        public bool triggerActive;

        public int triggerCounter;

        // -------------------------------------
        // event management

        public bool eventActive;

        public bool eventComplete;

        public bool mainEvent = false;

        public int activeCounter;

        public int decimalCounter;

        public double expireTime;

        public bool expireEarly;

        public int expireIn = 60;

        public bool eventAbort;

        public int eventLinger = -1;

        // -------------------------------------
        // event entities

        public MonsterHandle monsterHandle;

        public List<StardewDruid.Event.LightHandle> braziers = new();

        public Dictionary<int,StardewDruid.Character.Actor> actors = new();

        public Dictionary<int,StardewDruid.Character.Character> companions = new();

        public List<TemporaryAnimatedSprite> animations = new();

        public Dictionary<int, Dictionary<int, string>> cues = new();

        public Dictionary<int, Narrator> narrators = new();

        public int dialogueCounter;

        public Dictionary<StardewDruid.Character.Character, int> dialogueLoader = new();

        // ------------------------------------

        public bool soundTrack;

        public List<Vector2> ladders = new();

        // ------------------------------------

        public EventHandle()
        {

        }

        public virtual void EventSetup(Vector2 target, string id, bool trigger = false)
        {

            origin = target;

            eventId = id;

            triggerEvent = trigger;

            Mod.instance.RegisterEvent(this, eventId);

        }

        // ------------------------------------

        public virtual bool TriggerActive()
        {
            
            if (origin == Vector2.Zero || eventId == "none")
            {

                return false;

            }
            
            if (Mod.instance.questHandle.quests[eventId].triggerLocation == null)
            {

                return false;

            }

            if(Game1.player.currentLocation is null)
            {

                return false;

            }

            if (Mod.instance.questHandle.quests[eventId].triggerLocation != Game1.player.currentLocation.Name)
            {

                if (Mod.instance.questHandle.quests[eventId].triggerLocation != Game1.player.currentLocation.GetType().ToString())
                {

                    return false;

                }

            }

            location = Game1.player.currentLocation;

            return true;

        }

        public virtual bool TriggerMarker()
        {

            if (origin == Vector2.Zero || eventId == "none")
            {

                return false;

            }

            triggerActive = true;

            return true;

        }

        public virtual bool TriggerCheck()
        {

            if (!triggerActive)
            {

                return false;

            }

            if (Vector2.Distance(Game1.player.Position, origin) > (6 * 64))
            {

                return false;

            }

            if (Mod.instance.questHandle.quests[eventId].triggerRite != Rite.rites.none)
            {

                if (Mod.instance.rite.castType != Mod.instance.questHandle.quests[eventId].triggerRite)
                {

                    return false;

                }

            }

            if (Mod.instance.questHandle.quests[eventId].triggerTime != 0)
            {

                if (Game1.timeOfDay < Mod.instance.questHandle.quests[eventId].triggerTime)
                {

                    Mod.instance.CastMessage("Return later today");

                    return false;

                }

            }

            EventActivate();

            return true;

        }

        public virtual void TriggerRemove()
        {
            
            RemoveBraziers();

            RemoveActors();

            RemoveAnimations();

            if (soundTrack)
            {

                Game1.stopMusicTrack(MusicContext.Default);

            }

            triggerActive = false;

            triggerCounter = 0;

        }

        public virtual void TriggerInterval()
        {

            triggerCounter++;

            float activeCycle = triggerCounter % 5;

            if (activeCycle != 1)
            {

                return;

            }

            Vector2 animationPosition = origin - new Vector2(32, 32);

            Vector2 animationMotion = new Vector2(0, -0.3f);

            Vector2 animationAcceleration = new Vector2(0f, 0.002f);

            if (animations.Count > 0)
            {

                if (location.temporarySprites.Contains(animations.First()))
                {

                    animations.First().Position = animationPosition;

                    animations.First().reset();

                    animations.First().motion = animationMotion;

                    return;

                }

                animations.Clear();

            }

            float animationSort = 999f;

            TemporaryAnimatedSprite targetAnimation = new(0, 6000f, 1, 1, animationPosition, false, false)
            {

                sourceRect = new(0, 0, 64, 64),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Target.png")),

                layerDepth = animationSort,

                scale = 2f,
                
                motion = animationMotion,

                acceleration = animationAcceleration,

                color = Mod.instance.iconData.schemeColour(Mod.instance.questHandle.quests[eventId].triggerRite),

            };

            location.temporarySprites.Add(targetAnimation);

            animations.Add(targetAnimation);

        }

        public void ResetToTrigger()
        {

            EventRemove();

            eventActive = false;

        }


        // ------------------------------------

        public virtual void EventActivate()
        {

            if (triggerActive)
            {

                TriggerRemove();

            }

            Mod.instance.RegisterEvent(this, eventId, mainEvent);

            eventActive = true;

            expireEarly = false;

            activeCounter = 0;

            decimalCounter = 0;

            eventAbort = false;

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + expireIn;

            location = Game1.player.currentLocation;

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

            if (Game1.player.currentLocation.Name != location.Name)
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

            if (Mod.instance.activeEvent.ContainsKey(eventId))
            {

                Mod.instance.activeEvent.Remove(eventId);

            }

        }

        public virtual void EventLocation()
        {

            if (location != Game1.player.currentLocation)
            {

                EventRemove();

                location = Game1.player.currentLocation;

            }

        }

        public virtual bool EventPerformAction(SButton Button, string type = "Action")
        {

            return false;

        }

        public virtual void EventDecimal()
        {


        }

        public virtual void EventInterval()
        {

            activeCounter++;

        }

        // ------------------------------------
        // entities
        
        public void CastVoice(string message, int actor = 0, int duration = 2000)
        {

            if (actors.Count <= 0)
            {

                this.AddActor(actor,origin);

            }

            if (!actors.ContainsKey(actor))
            {

                return;

            }

            actors[actor].showTextAboveHead(message, duration: duration);


        }

        public void SetTrack(string track)
        {

            Game1.changeMusicTrack(track, false, MusicContext.Default);

            soundTrack = true;

        }

        public void AddActor(int id, Vector2 position, bool slave = false)
        {

            if (actors.ContainsKey(id))
            {

                actors[id].Position = position;

                return;

            }

            Actor actor = new Actor(CharacterData.characters.disembodied);

            actor.SwitchToMode(Character.Character.mode.scene,Game1.player);

            actor.collidesWithOtherCharacters.Value = true;
            
            actor.farmerPassesThrough = true;

            actor.drawSlave = slave;

            actor.Position = position;

            actor.currentLocation = location;

            location.characters.Add(actor);

            actors.Add(id,actor);

        }

        public void RemoveLadders()
        {
            Layer layer = location.map.GetLayer("Buildings");

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
                    name = eventId,
                    value = eventId,
                    description = Mod.instance.questHandle.quests[eventId].title,
                    time = Game1.currentGameTime.TotalGameTime.TotalMilliseconds,
                    location = Mod.instance.rite.castLocation.Name,
                };

                Mod.instance.EventQuery(queryData, eventQuery);

            }

        }

        // ------------------------------------
        // dialogue

        public virtual void DialogueCue(int cueIndex = -1)
        {

            DialogueCue(narrators, cueIndex);

        }

        public virtual void DialogueCue(int voice, string text)
        {

            cues = new() { [0] = new() { [voice] = text, }, };

            DialogueCue(narrators, 0);

        }

        public virtual void DialogueCue(Dictionary<int, Dialogue.Narrator> narrators, int cueIndex = -1)
        {

            if (cues.ContainsKey(cueIndex))
            {

                foreach (KeyValuePair<int, string> cue in cues[cueIndex])
                {

                    if (companions.ContainsKey(cue.Key))
                    {

                        if (cue.Value == "...")
                        {
                            companions[cue.Key].doEmote(40);

                            continue;
                        }
                        else if (cue.Value == "!")
                        {

                            companions[cue.Key].doEmote(16);

                            continue;
                        }
                        else if (cue.Value == "?")
                        {

                            companions[cue.Key].doEmote(8);

                            continue;
                        }
                        else if (cue.Value == "x")
                        {

                            companions[cue.Key].doEmote(36);

                            continue;
                        }

                        companions[cue.Key].showTextAboveHead(cue.Value);

                    }

                    if (actors.ContainsKey(cueIndex))
                    {

                        actors[cue.Key].showTextAboveHead(cue.Value);

                    }

                    if (narrators.ContainsKey(cue.Key))
                    {

                        narrators[cue.Key].DisplayHUD(cue.Value);

                        if (Context.IsMultiplayer)
                        {
                            QueryData queryData = new()
                            {
                                name = eventId,
                                value = cueIndex.ToString(),

                            };

                            Mod.instance.EventQuery(queryData, "DialogueDisplay");

                        }

                    }

                }

            }

        }

        public virtual void DialogueLoad(int companionId, int dialogueId)
        {

            if (!companions.ContainsKey(companionId))
            {

                return;

            }

            dialogueLoader[companions[companionId]] = dialogueId;

            return;

        }

        public virtual bool DialogueNext(StardewDruid.Character.Character npc)
        {

            if (dialogueLoader.ContainsKey(npc))
            {

                DialogueSetups(npc, dialogueLoader[npc]);

                dialogueLoader.Remove(npc);

                return true;

            }

            return false;

        }

        public virtual void DialogueSetups(StardewDruid.Character.Character npc, int dialogueId)
        {



        }

        public virtual void DialogueResponses(Farmer visitor, string answer)
        {


        }

        public virtual void DialogueAfter(Data.CharacterData.characters character, int dialogueId)
        {


        }

        // ----------------------------------
        // clean up

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

                foreach (KeyValuePair<int, StardewDruid.Character.Actor> actor in actors)
                {

                    actor.Value.currentLocation.characters.Remove(actor.Value);

                }

                actors.Clear();

            }

            if (companions.Count > 0)
            {

                foreach (KeyValuePair<int,StardewDruid.Character.Character> companion in companions)
                {

                    companion.Value.SwitchToMode(Character.Character.mode.random, Game1.player);

                }

                companions.Clear();

            }

        }

        public virtual void RemoveAnimations()
        {

            if (animations.Count > 0)
            {

                foreach (TemporaryAnimatedSprite animation in animations)
                {

                    location.temporarySprites.Remove(animation);

                }

                animations.Clear();

            }

        }

    }

}
