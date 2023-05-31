/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System;
using StardewModdingAPI.Events;
using StardewValley;
using static DynamicDialogues.Framework.Parser;
using static DynamicDialogues.Framework.Getter;
using System.Collections.Generic;
using System.Linq;

namespace DynamicDialogues.Framework
{
    internal static class Setter
    {
        internal static void OnTimeChange(object sender, TimeChangedEventArgs e)
        {
            if (e.NewTime % 30 == 0)
            {
                // ReSharper disable once PossibleNullReferenceException
                foreach (var name in ModEntry.RandomPool?.Keys)
                {
                    var who = Game1.getCharacterFromName(name);
                    if (who?.CurrentDialogue?.Count != 0) continue;
                    
                    var random = RandomDialogue(ModEntry.RandomPool[name], name);
                    if (random == null)
                        continue;

                    who.setNewDialogue(random, true, true);
                    //who.Dialogue.Add(new QuestionData(who,null,random));
                }
            }
            foreach (var patch in ModEntry.Dialogues)
            {
                foreach (var d in patch.Value)
                {
                    //AlreadyPatched contains already added patched. if current patch is not there (and conditions apply), it's used -then- added to the list.

                    var timesum = $"At{d.Time}From{d.From}To{d.To}";
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
                    var playerConditionsMatch = HasItems(d.PlayerItems);
                    
                    if (ModEntry.Config.Debug)
                    {
                        ModEntry.Mon.Log($" inLocation = {inLocation}; timeMatch = {timeMatch}");
                    }

                    if (!timeMatch || !inLocation || !playerConditionsMatch) continue;
                    
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
                        var listOfFrames = FramesForAnimation(d.Animation.Frames);

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
                                Game1.drawDialogue(chara);
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
            foreach (var notif in ModEntry.Notifs)
            {
                var pos = ModEntry.Notifs.IndexOf(notif);
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

                if ((!timeMatch || !inLocation) && (notif.Time != -1 || !inLocation) &&
                    (!timeMatch || notif.Location is not "any")) continue;
                
                ModEntry.Mon.Log($"Adding notif for player at {e.NewTime}, in {cLoc.Name}");
                if (notif.IsBox)
                {
                    Game1.drawObjectDialogue(notif.Message);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(notif.Sound))
                    {
                        Game1.soundBank.PlayCue(notif.Sound);
                    }

                    Game1.showGlobalMessage(notif.Message);
                }

                ModEntry.AlreadyPatched.Add(conditional);
            }
            foreach (var nameAndQuestions in ModEntry.Questions)
            {
                var chara = Game1.getCharacterFromName(nameAndQuestions.Key);
                if (chara.CurrentDialogue.Any()) continue;
                var qna = QuestionDialogue(nameAndQuestions.Value, chara);
                if (qna is "$qna#")//old: "$y '...'")
                {
                    continue;
                }

                //use a method in "getter" that returns the proper string by giving it NaQ.Value - 
                //old: chara.setNewDialogue(qna, true, true);
                chara.setNewDialogue(new QuestionData(chara, qna));
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
            foreach (var name in ModEntry.PatchableNPCs) //NPCsToPatch
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

                /*//mission/quests - deprecated (can be used from Questions)
                if (e.NameWithoutLocale.IsEquivalentTo($"mistyspring.dynamicdialogues/Quests/{name}"))
                {
                    e.LoadFrom(
                    () => new Dictionary<string, RawMission>(),
                    AssetLoadPriority.Low
                    );
                }*/
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

            //event data - playerFind
            if (e.NameWithoutLocale.IsEquivalentTo("mistyspring.dynamicdialogues/Commands/playerFind", true))
            {
                e.LoadFrom(
                    () => new Dictionary<string, string>(),
                    AssetLoadPriority.Low);
            }
        }


        public static void ReloadAssets(object sender, AssetsInvalidatedEventArgs e)
        {
            //get dialogue for NPCs
            foreach (var name in ModEntry.PatchableNPCs)
            {
                if (e.NamesWithoutLocale.Any(a => a.Name.Equals($"mistyspring.dynamicdialogues/Dialogues/{name}")))
                {
                    var compatRaw = ModEntry.Help.GameContent.Load<Dictionary<string, RawDialogues>>(
                        $"mistyspring.dynamicdialogues/Dialogues/{name}");
                    ModEntry.GetNPCDialogues(compatRaw, name);
                }

                if (e.NamesWithoutLocale.Any(a => a.Name.Equals($"mistyspring.dynamicdialogues/Questions/{name}")))
                {
                    var questionsRaw = ModEntry.Help.GameContent.Load<Dictionary<string, RawQuestions>>(
                        $"mistyspring.dynamicdialogues/Questions/{name}");
                    ModEntry.GetQuestions(questionsRaw, name);
                }

                if (e.NamesWithoutLocale.Any(a => a.Name.Equals($"Characters/Dialogue/{name}")))
                {
                    try
                    {
                        GetDialogues(name);
                    }
                    catch (Exception ex)
                    {
                        ModEntry.Mon.Log($"Error while reloading \"Characters/Dialogue/{name}\": {ex}");
                    }
                }
            }

            ModEntry.RemoveAnyEmpty();

            if(e.NamesWithoutLocale.Any(a => a.Name.Equals($"mistyspring.dynamicdialogues/Greetings")))
            {
                //get greetings
                var greetRaw =
                    ModEntry.Help.GameContent.Load<Dictionary<string, Dictionary<string, string>>>(
                        "mistyspring.dynamicdialogues/Greetings");
                ModEntry.GetGreetings(greetRaw);
            }

            if (e.NamesWithoutLocale.Any(a => a.Name.Equals($"mistyspring.dynamicdialogues/Greetings")))
            {
                //get notifs
                var notifRaw =
                    ModEntry.Help.GameContent.Load<Dictionary<string, RawNotifs>>(
                        "mistyspring.dynamicdialogues/Notifs");
                ModEntry.GetNotifs(notifRaw);
            }
        }

        private static void GetDialogues(string name)
        {
            ModEntry.RandomPool.Remove(name);
            
            var dialogues = ModEntry.Help.GameContent.Load<Dictionary<string, string>>($"Characters/Dialogue/{name}");

            if (dialogues == null || dialogues.Count == 0)
                return;

            List<string> texts = new();
            foreach (var pair in dialogues)
            {
                if(pair.Key.StartsWith("Random"))
                {
                    texts.Add(pair.Value);
                }
            }

            //dont add npcs with no dialogue
            if (texts.Count != 0)
            {
                ModEntry.RandomPool.Add(name, texts);
            }

            var temp = new List<string>();
            
            foreach (var pair in dialogues)
            {
                if (pair.Value.StartsWith("Gift."))
                {
                    temp.Add(pair.Value);
                }
            }
            
            if (temp.Count != 0)
            {
                ModEntry.HasCustomGifting.Add(name,temp.ToArray());
            }
        }
    }
}