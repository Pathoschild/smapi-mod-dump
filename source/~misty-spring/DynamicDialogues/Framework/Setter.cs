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
using System.Collections.Generic;
using System.Linq;
using DynamicDialogues.Models;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Triggers;
using static DynamicDialogues.Framework.Parser;
using static DynamicDialogues.Framework.Getter;
// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable LoopCanBeConvertedToQuery

namespace DynamicDialogues.Framework;

internal static class Setter
{
    #region references
    //we reference them here because i don't want modentry's references to be so clogged
    private static List<(string, string, string)> Patched => ModEntry.AlreadyPatched;
    private static Dictionary<string, List<QuestionData>> Question => ModEntry.Questions;
    private static Dictionary<string, List<DialogueData>> Dialog => ModEntry.Dialogues;
    private static Dictionary<string, List<string>> RandomDialogues => ModEntry.RandomPool;
    private static List<NotificationData> Notifications => ModEntry.Notifs;
    private static IMonitor Monitor => ModEntry.Mon;
    private static IModHelper Helper => ModEntry.Help;
    private static ModConfig Cfg => ModEntry.Config;
    private static void Log(string msg, LogLevel lv = LogLevel.Trace) => ModEntry.Mon.Log(msg, lv);
    #endregion

    internal static void OnTimeChange(object sender, TimeChangedEventArgs e)
    {
        //random dialogue
        if (e.NewTime % 30 == 0)
        {
            // ReSharper disable once PossibleNullReferenceException
            foreach (var name in RandomDialogues?.Keys)
            {
                var who = Game1.getCharacterFromName(name);
                //if there's already dialogue
                if (who?.CurrentDialogue?.Count != 0) continue;

                //if not in player location, we don't add it to avoid clutter
                if (!who.currentLocation.Equals(Game1.player.currentLocation)) 
                    continue;

                if (RandomDialogues == null || RandomDialogues.Any() == false) 
                    continue;
                
                var random = RandomDialogue(RandomDialogues[name], name);
                SetNewDialogue(who, random, true, true);
            }
        }
        
        //regular dialogue
        foreach (var patch in Dialog)
        {
            foreach (var d in patch.Value)
            {
                //AlreadyPatched contains already added patched. if current patch is not there (and conditions apply), it's used -then- added to the list.

                var timesum = $"At{d.Time}From{d.From}To{d.To}";
                var conditional = (patch.Key, timesum, d.Location);
                if ((bool)Patched?.Contains(conditional))
                {
                    Monitor.LogOnce($"Dialogue {conditional} has already been used today. Skipping...");
                    continue;
                }

                if (Cfg.Verbose)
                {
                    Log($"Checking dialogue with key {conditional}...");
                }

                var chara = Game1.getCharacterFromName(patch.Key);

                /*if patch must only be added when npc *isnt* moving
                 * taken out because it could just confuse more
                if (d.ApplyWhenMoving == false)
                {
                    if (chara.isMovingOnPathFindPath.Value)
                    {
                        Log($"NPC {chara.Name} is moving on pathfind path. Patch won't be applied yet.");
                        continue;
                    }
                }*/

                var inLocation = InRequiredLocation(chara, d.Location);
                var timeMatch = InTimeRange(e.NewTime, d.Time, d.From, d.To, chara);
                var conditionsMatch = true;
                
                //if conditions isn't empty
                if (d.Conditions != new PlayerConditions())
                {
                    conditionsMatch = ConditionsApply(d.Conditions);
                }

                if (Cfg.Debug)
                {
                    Log($" inLocation = {inLocation}; timeMatch = {timeMatch}");
                }

                if (!timeMatch || !inLocation || !conditionsMatch) continue;

                if (Cfg.Verbose)
                {
                    Log("Conditions match. Applying...");
                }

                //get facing direction if any
                var facing = ReturnFacing(d.FaceDirection);

                /* Extra options:
                 * if any emote, do it.
                 * if shake is greater than 0, shake.
                 * if jump is true, make npc jump
                 * if facedirection isn't -1, set facedirection
                 * if trigger action is set, call
                 */
                
                if (d.Emote >= 0)
                {
                    Log($"Doing emote for {patch.Key}. Index: {d.Emote}");
                    chara.doEmote(d.Emote);
                }
                
                if (d.Shake > 0)
                {
                    Log($"Shaking {patch.Key} for {d.Shake} milliseconds.");
                    chara.shake(d.Shake);
                }
                
                if (d.Jump)
                {
                    Log($"{patch.Key} will jump..");
                    chara.jump();
                }
                
                if (facing != -1)
                {
                    Log($"Changing {patch.Key} facing direction to {d.FaceDirection}.");
                    chara.faceDirection(facing);
                }
                
                if (!string.IsNullOrWhiteSpace(d.TriggerAction))
                {
                    TriggerActionManager.TryRunAction(d.TriggerAction, out var error, out var exception);
                    if(!string.IsNullOrWhiteSpace(error))
                        Log($"Error: {error}. {exception}");
                }
                
                /*if set to animate AND the npc isnt moving (to avoid bugs with walking sprite). if animation is null / doesnt exist, it will consider the bool as false
                 * NPC.isMovingOnPathFindPath.Value gets only if on path. NPC.isMoving() also considers animations, apparently.*/

                if ((bool)d.Animation?.Enabled && chara.isMoving() == false)
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
                    Log("Adding text as bubble.");
                    chara.showTextAboveHead(FormatBubble(d.Dialogue));
                }
                else
                {
                    //if the user wants to override current dialogue, this will do it.
                    if (d.Override)
                    {
                        Log($"Clearing {patch.Key} dialogue.");
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
                        SetNewDialogue(chara, d.Dialogue, true, d.ClearOnMove);
                    }
                }
                Log($"Adding dialogue for {patch.Key} at {e.NewTime}, in {chara.currentLocation.Name}");

                /* List is checked daily, but removing causes errors in the foreach loop.
                 * So, there'll be a list with today's already added values (tuple of NPC name, time, location)
                 */
                Patched.Add(conditional);
            }
        }
        foreach (var notif in Notifications)
        {
            var pos = Notifications.IndexOf(notif);
            // we use notif+index since those aren't tied to a npc. 
            // time turned to string due to change in how conditionals are saved
            var conditional = ($"notification-{pos}", notif.Time.ToString(), notif.Location);

            if ((bool)Patched?.Contains(conditional))
            {
                Monitor.LogOnce($"Key {conditional} has already been used today. Skipping...");
                continue;
            }

            Log($"Checking notif with key {conditional}...");
            var currentLocation = Game1.player.currentLocation;
            var inLocation = notif.Location == currentLocation.Name;
            var timeMatch = notif.Time.Equals(e.NewTime);

            if (Cfg.Debug)
            {
                Monitor.LogOnce($"Player name: {Game1.player.Name}");
                Log($"currentLocation.Name = {currentLocation.Name} ; inLocation = {inLocation}; timeMatch = {timeMatch}");
            }

            if ((!timeMatch || !inLocation) && (notif.Time != -1 || !inLocation) &&
                (!timeMatch || notif.Location is not "any")) continue;

            if (!ConditionsApply(notif.Conditions))
            {
                Log($"Conditions don't match for {conditional}. Skipping...", LogLevel.Debug);
            }
            
            if (!string.IsNullOrWhiteSpace(notif.TriggerAction))
            {
                TriggerActionManager.TryRunAction(notif.TriggerAction, out var error, out var exception);
                if(!string.IsNullOrWhiteSpace(error))
                    Log($"Error: {error}. {exception}");
            }

            Log($"Adding notif for player at {e.NewTime}, in {currentLocation.Name}");
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

            Patched.Add(conditional);
        }
        
        foreach (var nameAndQuestions in Question)
        {
            var chara = Game1.getCharacterFromName(nameAndQuestions.Key);
            if (chara.CurrentDialogue.Any()) 
                continue;
            
            var qna = QuestionDialogue(nameAndQuestions.Value, chara);
            if (string.IsNullOrWhiteSpace(qna))
                continue;

            chara.setNewDialogue(new Question(chara, qna));
        }
    }

    private static void SetNewDialogue(NPC who, string text, bool add, bool clearOnMove) =>
        who.setNewDialogue(new Dialogue(who, null, text), add, clearOnMove);

    internal static void OnAssetRequest(object sender, AssetRequestedEventArgs e)
    {
        //each NPC file
        foreach (var name in ModEntry.PatchableNPCs) //NPCsToPatch
        {
            //dialogue
            if (e.NameWithoutLocale.IsEquivalentTo($"mistyspring.dynamicdialogues/Dialogues/{name}", true))
            {
                e.LoadFrom(
                    () => new Dictionary<string, DialogueData>(),
                    AssetLoadPriority.Low
                );
            }

            //questions
            if (e.NameWithoutLocale.IsEquivalentTo($"mistyspring.dynamicdialogues/Questions/{name}", true))
            {
                e.LoadFrom(
                    () => new Dictionary<string, QuestionData>(),
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
                () => new Dictionary<string, NotificationData>(),
                AssetLoadPriority.Low
            );
        }

        //event data - objectHunt
        if (e.NameWithoutLocale.IsEquivalentTo("mistyspring.dynamicdialogues/Commands/objectHunt", true))
        {
            e.LoadFrom(
                () => new Dictionary<string, HuntContext>(),
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
                var compatRaw = Helper.GameContent.Load<Dictionary<string, DialogueData>>(
                    $"mistyspring.dynamicdialogues/Dialogues/{name}");
                ModEntry.GetNPCDialogues(compatRaw, name);
            }

            if (e.NamesWithoutLocale.Any(a => a.Name.Equals($"mistyspring.dynamicdialogues/Questions/{name}")))
            {
                var questionsRaw = Helper.GameContent.Load<Dictionary<string, QuestionData>>(
                    $"mistyspring.dynamicdialogues/Questions/{name}");
                ModEntry.GetQuestions(questionsRaw, name);
            }

            if (!e.NamesWithoutLocale.Any(a => a.Name.Equals($"Characters/Dialogue/{name}"))) 
                continue;
            try
            {
                GetDialogues(name);
            }
            catch (Exception ex)
            {
                Log($"Error while reloading \"Characters/Dialogue/{name}\": {ex}");
            }
        }

        ModEntry.RemoveAnyEmpty();

        if (e.NamesWithoutLocale.Any(a => a.Name.Equals("mistyspring.dynamicdialogues/Greetings")))
        {
            //get greetings
            var greetRaw =
                Helper.GameContent.Load<Dictionary<string, Dictionary<string, string>>>(
                    "mistyspring.dynamicdialogues/Greetings");
            ModEntry.GetGreetings(greetRaw);
        }

        if (e.NamesWithoutLocale.Any(a => a.Name.Equals("mistyspring.dynamicdialogues/Notifs")))
        {
            //get notifs
            var notifRaw =
                Helper.GameContent.Load<Dictionary<string, NotificationData>>(
                    "mistyspring.dynamicdialogues/Notifs");
            ModEntry.GetNotifs(notifRaw);
        }
    }

    private static void GetDialogues(string name)
    {
        RandomDialogues.Remove(name);

        var dialogues = Helper.GameContent.Load<Dictionary<string, string>>($"Characters/Dialogue/{name}");

        if (dialogues == null || dialogues.Count == 0)
            return;

        List<string> texts = new();
        foreach (var pair in dialogues)
        {
            if (pair.Key.StartsWith("Random"))
            {
                texts.Add(pair.Value);
            }
        }

        //dont add npcs with no dialogue
        if (texts.Count != 0)
        {
            RandomDialogues.Add(name, texts);
        }
    }

    internal static void OnDayEnd(object sender, DayEndingEventArgs e) => ModEntry.EventLock = false;
}