/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley;
using static DynamicDialogues.Parser;
using static DynamicDialogues.Getter;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DynamicDialogues
{
    internal class Setter
    {
        internal static void OnTimeChange(object sender, TimeChangedEventArgs e)
        {
            if (e.NewTime % 30 == 0)
            {
                foreach (var group in ModEntry.MissionData)
                {
                    var who = Game1.getCharacterFromName(group.Key);
                    if (!(who.currentLocation.Name.Equals(Game1.player.currentLocation.Name)))
                    {
                        continue;
                    }

                    if (who.CurrentDialogue.Count == 0 && Game1.random.Next(100) <= ModEntry.Config.MissionChance && ModEntry.CurrentQuests.Contains(who.Name) == false)
                    {
                        var mission = RandomMission(group.Value);

                        if (mission is null)
                        {
                            continue;
                        }

                        Game1.player.currentLocation.createQuestionDialogue(mission.Dialogue, GetResponses(mission), new GameLocation.afterQuestionBehavior(AddMission), who);
                    }
                }
                foreach (var name in ModEntry.RandomPool?.Keys)
                {
                    var who = Game1.getCharacterFromName(name);
                    if (who?.CurrentDialogue?.Count == 0)
                    {
                        var random = RandomDialogue(ModEntry.RandomPool[name], name);
                        if (random == null)
                            continue;

                        who.setNewDialogue(random, true, true);
                    }
                }
            }
            foreach (var patch in ModEntry.Dialogues)
            {
                foreach (var d in patch.Value)
                {
                    //AlreadyPatched contains already added patched. if current patch is not there (and conditions apply), it's used -then- added to the list.

                    string timesum = $"At{d.Time}From{d.From}To{d.To}";
                    var conditional = (patch.Key, timesum, d.Location);
                    if ((bool)(ModEntry.AlreadyPatched?.Contains(conditional)))
                    {
                        ModEntry.Mon.LogOnce($"Dialogue {conditional} has already been used today. Skipping...");
                        continue;
                    }

                    if (ModEntry.Config.Verbose)
                    {
                        ModEntry.Mon.Log($"Checking dialogue with key {conditional}...");
                    }

                    var chara = Game1.getCharacterFromName(patch.Key);

                    /*if patch must only be added when npc *isnt* moving
                     * taken out because it could just confuse more
                    if (d.ApplyWhenMoving == false)
                    {
                        if (chara.isMovingOnPathFindPath.Value)
                        {
                            ModEntry.Mon.Log($"NPC {chara.Name} is moving on pathfind path. Patch won't be applied yet.");
                            continue;
                        }
                    }*/

                    var inLocation = InRequiredLocation(chara, d.Location);
                    var timeMatch = InTimeRange(e.NewTime, d.Time, d.From, d.To, chara);

                    if (ModEntry.Config.Debug)
                    {
                        ModEntry.Mon.Log($" inLocation = {inLocation}; timeMatch = {timeMatch}");
                    }

                    if (timeMatch && inLocation)
                    {
                        if (ModEntry.Config.Verbose)
                        {
                            ModEntry.Mon.Log("Conditions match. Applying...");
                        }

                        //get facing direction if any
                        var facing = ReturnFacing(d.FaceDirection);

                        /* Extra options: 
                         * if any emote, do it. 
                         * if shake is greater than 0, shake. 
                         * if jump is true, make npc jump
                         * if facedirection isn't -1, set facedirection
                         */
                        if (d.Emote >= 0)
                        {
                            ModEntry.Mon.Log($"Doing emote for {patch.Key}. Index: {d.Emote}");
                            chara.doEmote(d.Emote);
                        }
                        if (d.Shake > 0)
                        {
                            ModEntry.Mon.Log($"Shaking {patch.Key} for {d.Shake} milliseconds.");
                            chara.shake(d.Shake);
                        }
                        if (d.Jump)
                        {
                            ModEntry.Mon.Log($"{patch.Key} will jump..");
                            chara.jump();
                        }
                        if (facing != -1)
                        {
                            ModEntry.Mon.Log($"Changing {patch.Key} facing direction to {d.FaceDirection}.");
                            chara.faceDirection(facing);
                        }
                        /*if set to animate AND the npc isnt moving (to avoid bugs with walking sprite). if animation is null / doesnt exist, it will consider the bool as false
                         * NPC.isMovingOnPathFindPath.Value gets only if on path. NPC.isMoving() also considers animations, apparently.*/

                        if ((bool)(d.Animation?.Enabled) && chara.isMoving() == false)
                        {
                            /* makes new list with anim. frames, gets frames from string, then adds each w/ interval- THEN sets the animation */

                            List<FarmerSprite.AnimationFrame> list = new();
                            int[] listOfFrames = FramesForAnimation(d.Animation.Frames);

                            foreach (var frame in listOfFrames)
                            {
                                list.Add(new FarmerSprite.AnimationFrame(frame, d.Animation.Interval));
                            }
                            chara.Sprite.setCurrentAnimation(list);
                        }

                        /* If its supposed to be a bubble, put the dialogue there. If not, proceed as usual. */
                        if (d.IsBubble)
                        {
                            ModEntry.Mon.Log($"Adding text as bubble.");
                            chara.showTextAboveHead(FormatBubble(d.Dialogue));
                        }
                        else
                        {
                            //if the user wants to override current dialogue, this will do it.
                            if (d.Override)
                            {
                                ModEntry.Mon.Log($"Clearing {patch.Key} dialogue.");
                                chara.CurrentDialogue.Clear();
                                chara.endOfRouteMessage.Value = null;
                            }

                            //if should be immediate. ie not wait for npc to pass by
                            if (d.Immediate)
                            {
                                //if npc in location OR force true
                                if (Game1.player.currentLocation.Name == d.Location || d.Force)
                                {
                                    Game1.drawDialogue(chara, d.Dialogue);
                                }
                            }
                            else
                            {
                                //set new dialogue, log to trace
                                chara.setNewDialogue(d.Dialogue, true, d.ClearOnMove);
                            }
                        }
                        ModEntry.Mon.Log($"Adding dialogue for {patch.Key} at {e.NewTime}, in {chara.currentLocation.Name}");

                        /* List is checked daily, but removing causes errors in the foreach loop.
                         * So, there'll be a list with today's already added values (tuple of NPC name, time, location)
                        */
                        ModEntry.AlreadyPatched.Add(conditional);
                    }
                }
            }
            foreach (var notif in ModEntry.Notifs)
            {
                int pos = ModEntry.Notifs.IndexOf(notif);
                // we use notif+index since those aren't tied to a npc. 
                // time turned to string due to change in how conditionals are saved
                var conditional = ($"notification-{pos}", notif.Time.ToString(), notif.Location);

                if ((bool)(ModEntry.AlreadyPatched?.Contains(conditional)))
                {
                    ModEntry.Mon.LogOnce($"Key {conditional} has already been used today. Skipping...");
                    continue;
                }

                ModEntry.Mon.Log($"Checking notif with key {conditional}...");
                var cLoc = Game1.player.currentLocation;
                var inLocation = notif.Location == cLoc.Name;
                var timeMatch = notif.Time.Equals(e.NewTime);

                if (ModEntry.Config.Debug)
                {
                    ModEntry.Mon.LogOnce($"Player name: {Game1.player.Name}");
                    ModEntry.Mon.Log($"cLoc.Name = {cLoc.Name} ; inLocation = {inLocation}; timeMatch = {timeMatch}");
                }

                if ((timeMatch && inLocation) || (notif.Time == -1 && inLocation) || (timeMatch && notif.Location is "any"))
                {
                    ModEntry.Mon.Log($"Adding notif for player at {e.NewTime}, in {cLoc.Name}");
                    if (notif.IsBox)
                    {
                        Game1.drawObjectDialogue(notif.Message);
                    }
                    else
                    {
                        if (!String.IsNullOrWhiteSpace(notif.Sound))
                        {
                            Game1.soundBank.PlayCue(notif.Sound);
                        }

                        Game1.showGlobalMessage(notif.Message);
                    }

                    ModEntry.AlreadyPatched.Add(conditional);
                }
            }
            foreach (var NaQ in ModEntry.Questions)
            {
                NPC chara = Game1.getCharacterFromName(NaQ.Key);
                if (!chara.CurrentDialogue.Any())
                {
                    var qna = QuestionDialogue(NaQ.Value, chara);
                    if (qna is "$y '...'")
                    {
                        continue;
                    }

                    //use a method in "getter" that returns the proper string by giving it NaQ.Value - 
                    chara.setNewDialogue(qna, true, true);
                }
            }
        }

        internal static void OnAssetRequest(object sender, AssetRequestedEventArgs e)
        {
            /*
            //list of admitted NPCs - deprecated but still here jic
            if (e.NameWithoutLocale.IsEquivalentTo("mistyspring.dynamicdialogues/NPCs", true))
            {
                e.LoadFrom(
                () => new List<string>(),
                AssetLoadPriority.Low
            );
            }*/

            //each NPC file
            foreach (var name in ModEntry.NPCDispositions) //NPCsToPatch
            {
                //dialogue
                if (e.NameWithoutLocale.IsEquivalentTo($"mistyspring.dynamicdialogues/Dialogues/{name}", true))
                {
                    e.LoadFrom(
                    () => new Dictionary<string, RawDialogues>(),
                    AssetLoadPriority.Low
                    );
                }

                //questions
                if (e.NameWithoutLocale.IsEquivalentTo($"mistyspring.dynamicdialogues/Questions/{name}", true))
                {
                    e.LoadFrom(
                    () => new Dictionary<string, RawQuestions>(),
                    AssetLoadPriority.Low
                );
                }

                //mission/quests
                if (e.NameWithoutLocale.IsEquivalentTo($"mistyspring.dynamicdialogues/Quests/{name}"))
                {
                    e.LoadFrom(
                    () => new Dictionary<string, RawMission>(),
                    AssetLoadPriority.Low
                    );
                }
            }

            //greetings
            if (e.NameWithoutLocale.IsEquivalentTo("mistyspring.dynamicdialogues/Greetings", true))
            {
                e.LoadFrom(
                () => new Dictionary<string, Dictionary<string, string>>(),
                AssetLoadPriority.Low
            );
            }

            //notifs
            if (e.NameWithoutLocale.IsEquivalentTo("mistyspring.dynamicdialogues/Notifs", true))
            {
                e.LoadFrom(
                () => new Dictionary<string, RawNotifs>(),
                AssetLoadPriority.Low
                );
            }
        }

        //on day end, we remove repeatable events from 'seen' list
        internal static void OnDayEnded(object sender, DayEndingEventArgs e)
        {
            foreach(var data in ModEntry.Questions)
            {
                foreach(var raw in data.Value)
                {
                    if(raw.CanRepeatEvent)
                    {
                        Game1.player.eventsSeen.Remove(int.Parse(raw.EventToStart));
                    }
                }
            }
        }
    }
}