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
using DynamicDialogues.Framework;
using DynamicDialogues.Models;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Triggers;
// ReSharper disable LoopCanBeConvertedToQuery

namespace DynamicDialogues;

/// <summary>
/// Custom trigger actions.
/// </summary>
/// <see cref="StardewValley.Triggers.TriggerActionManager.DefaultActions"/>
/// <see cref="StardewValley.Event"/>
internal static class TriggerActions
{
    private static IMonitor Monitor => ModEntry.Mon;
    private static IModHelper Helper => ModEntry.Help;

    private static void TestInput(IEnumerable<string> args)
    {
        var fullString = "";
        foreach (var s in args)
            fullString += ' ' + s;

        Monitor.Log($"Args: {fullString}", LogLevel.Debug);
    }

    internal static bool DoEvent(string[] args, TriggerActionContext context, out string error)
    {
        //command <eventID> <location> [checkprec] [checkseen] [reset_if_not_played]

        TestInput(args);
        
        /*
        if(ModEntry.EventLock)
        {
            error = "An event is already ongoing.";
            return false;
        }
        */

        if (context.Data is null)
        {
            error = "This action can only be used via Data/TriggerActions.";
            return false;
        }

        if (context.Data.Trigger != "DayStarted")
        {
            error = "This action can only be used on day start.";
            return false;
        }

        if (!ArgUtility.TryGet(args, 1, out var @event, out error))
        {
            return false;
        }

        if (!ArgUtility.TryGet(args, 2, out var location, out error))
        {
            return false;
        }

        var checkPrecondition = args.Length < 4 || bool.Parse(args[3]);
        var checkSeen = args.Length < 5 || bool.Parse(args[4]);
        var resetTriggerIfUnseen = args.Length < 6 || bool.Parse(args[5]);

        if (!ModEntry.EventLock)
        {
            //actions to play event
            if (Game1.PlayEvent(@event, Utility.fuzzyLocationSearch(location), out var validEvent, checkPrecondition, checkSeen))
            {
                ModEntry.EventLock = true;
            }

            if (validEvent) 
                return true;
            
            error = "Event isn't valid.";
            return false;
        }

        ModEntry.EventQueue.Add(new EventData(@event, location, checkPrecondition, checkSeen, resetTriggerIfUnseen, context.Data.Id));

        return true;
    }

    internal static bool SendNotification(string[] args, TriggerActionContext context, out string error)
    {
        TestInput(args);

        //command <notif id> <check if already sent>
        
        if (context.Data is null)
        {
            error = "This action can only be used via Data/TriggerActions.";
            return false;
        }

        if (!ArgUtility.TryGet(args, 1, out var key, out error))
        {
            return false;
        }

        var allData = Helper.GameContent.Load<Dictionary<string, NotificationData>>("mistyspring.dynamicdialogues/Notifs");
        var notif = allData[key];

        if(!Parser.IsValidNotif(notif))
        {
            error = "The notification isn't valid. Check log for more details.";
            return false;
        }

        var checkSeen = args.Length < 3 || bool.Parse(args[2]);
        if(checkSeen)
        {
            var pos = ModEntry.Notifs.IndexOf(notif);
            var conditional = ($"notification-{pos}", notif.Time.ToString(), notif.Location);
            if ((bool)ModEntry.AlreadyPatched?.Contains(conditional))
            {
                Monitor.LogOnce($"Key {conditional} has already been used today. Skipping...");
                return true;
            }
        }
        
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

        return true;
    }

    internal static bool Speak(string[] args, TriggerActionContext context, out string error)
    {
        TestInput(args);
        //command <who> <key> [shouldOverride]

        if (context.Data is null)
        {
            error = "This action can only be used via Data/TriggerActions.";
            return false;
        }

        if (!ArgUtility.TryGet(args, 1, out var who, out error))
        {
            return false;
        }

        if (!ArgUtility.TryGet(args, 2, out var msgKey, out error))
        {
            return false;
        }

        var shouldOverride = args.Length < 4 || bool.Parse(args[3]);

        if (Game1.dialogueUp && !shouldOverride)
        {
            error = "A dialogue is already occurring.";
            return false;
        }

        var npc = Game1.getCharacterFromName(who);
        if (npc == null)
        {
            error = "No NPC found with name '" + who + "'";
            return false;
        }

        string key;
        if (!msgKey.Contains('\\') && !msgKey.Contains('/'))
            key = $"Characters/Dialogue/{npc.Name}:{msgKey}";
        else
            key = msgKey;

        if (!Game1.content.IsValidTranslationKey(key))
        {
            error = "";
            return false;
        }

        var item = new Dialogue(npc, key);
        npc.CurrentDialogue.Push(item);
        Game1.drawDialogue(npc);

        return true;
    }

    /// <summary>
    /// Adds experience to the given skill.
    /// </summary>
    /// <param name="args">Arguments.</param>
    /// <param name="context">Trigger context.</param>
    /// <param name="error">Error, if any.</param>
    /// <returns>Whether the action was run.</returns>
    /// <see cref="TriggerActionManager.DefaultActions.AddCookingRecipe"/>
    public static bool AddExp(string[] args, TriggerActionContext context, out string error)
    {
        TestInput(args);
        
        //command <who> <skill> <amt>
        
        var p = !ArgUtility.TryGet(args, 1, out var playerKey, out error);
        var t = !ArgUtility.TryGet(args, 2, out var skillRaw, out error);
        var i = !ArgUtility.TryGetOptionalInt(args, 3, out var amount, out error, defaultValue: 50);
        var invalid = p || t || i;

        if (invalid || string.IsNullOrWhiteSpace(skillRaw))
            return false;

        int skill;
        if (int.TryParse(skillRaw, out var parsedSkill))
            skill = parsedSkill;
        else
        {
            skill = skillRaw.ToLower() switch {
                "farming" => 0,
                "fishing" => 1,
                "foraging" => 2,
                "mining" => 3,
                "combat" => 4,
                "luck" => 5,
                _ => 0
            };
        }

#if DEBUG
        Monitor.Log($"Values:\namount = {amount}\nwho = {playerKey}, \nskill = {skill}({skillRaw})", LogLevel.Debug);
#endif
        var success = GameStateQuery.Helpers.WithPlayer(Game1.player, playerKey, target =>
        {
            try
            {
                target.gainExperience(skill, amount);
            }
            catch (Exception e)
            {
                Monitor.Log("Error:" + e);
                return false;
            }
            return true;
        });
        
        return success;
    }
}
