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
using System.Diagnostics.CodeAnalysis;
using DynamicDialogues.Models;
using StardewModdingAPI;
using StardewValley;

namespace DynamicDialogues.Framework;

[SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
internal static class Parser
{
    #region references
    private static ModConfig Cfg => ModEntry.Config;
    private static void Log(string msg, LogLevel lv = LogLevel.Trace) => ModEntry.Mon.Log(msg, lv);
    #endregion

    /// <summary>
    /// Checks if the conditions match to add a dialogue.
    /// </summary>
    /// <param name="which">The items the player must have.</param>
    /// <returns></returns>
    internal static bool ConditionsApply(PlayerConditions which)
    {
        var hat = string.IsNullOrWhiteSpace(which.Hat) || Game1.player.hat.Value.ItemId == which.Hat;
        var shirt = string.IsNullOrWhiteSpace(which.Shirt) || Game1.player.shirtItem.Value.ItemId == which.Shirt;
        var pants = string.IsNullOrWhiteSpace(which.Pants) || Game1.player.pantsItem.Value.ItemId == which.Pants;
        var rings = string.IsNullOrWhiteSpace(which.Rings) || HasItems(which.Rings, Game1.player.isWearingRing);
        var boots = string.IsNullOrWhiteSpace(which.Boots) || Game1.player.boots.Value.ItemId == which.Boots;
        var inv = string.IsNullOrWhiteSpace(which.Inventory) || HasItems(which.Inventory, Game1.player.Items.ContainsId);
        var gsq = GameStateQuery.CheckConditions(which.GameStateQuery);

        return hat && shirt && pants && rings && inv && gsq && boots;
    }

    /// <summary>
    /// Returns whether player is wearing a ring. Supports mods with more ring slots.
    /// </summary>
    /// <param name="which">Ring(s) to check for.</param>
    /// <param name="check">The method to use when checking for item.</param>
    /// <returns></returns>
    private static bool HasItems(string which, Func<string,bool> check)
    {
        /* Example:
         * which = "\"517 OR 519\" AND 520 OR 521 OR \"522 AND 523\""
         * will make the following checks for player:
         * 
         * 1. (glow ring OR magnet ring) AND slime ring
         * 2. slime ring OR warrior ring
         * 3. warrior ring OR (vampire and savage rings)
         *
         * if any of those checks are false, it'll return false.
         *
         */
        
        var spaceSplit = ArgUtility.SplitBySpaceQuoteAware(which);
        var count = 0;
        
        foreach (var condition in spaceSplit)
        {
            if (condition.Equals("and", StringComparison.OrdinalIgnoreCase) ||
                condition.Equals("or", StringComparison.OrdinalIgnoreCase))
                continue;

            bool flag;
            
            count++;
            
            //if string is and/or. (we add +1 to current one, so the check will always be "1 spot" ahead → check even numbers despite and/or being uneven)
            if(count % 2 == 0)
                continue;

            var typeOfComparison = spaceSplit[count];
            count++;

            var valid1 = IsThisItemValid(condition, check);
            var valid2 = IsThisItemValid(spaceSplit[count], check);

            if (typeOfComparison.Equals("and", StringComparison.OrdinalIgnoreCase) || typeOfComparison[0] == '&' || typeOfComparison[0] == '+')
                flag = valid1 && valid2;
            else
                flag = valid1 || valid2;

            if (!flag)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if player has item. If multiple, uses <see cref="ValidSubItems"/>.
    /// </summary>
    /// <param name="condition">The item(s).</param>
    /// <param name="check">The method to use.</param>
    /// <returns>Whether "check" is true for item(s).</returns>
    private static bool IsThisItemValid(string condition, Func<string,bool> check)
    {
        bool result;
        
        if (condition.Contains(' '))
        {
            var subSplit = condition.Split(' ');
            result = ValidSubItems(subSplit, check);
        }
        else
        {
            result = check(condition);
        }

        return result;
    }

    /// <summary>
    /// Checks for multiple items in inventory.
    /// </summary>
    /// <param name="subSplit">Multiple items to check.</param>
    /// <param name="check">The method to use.</param>
    /// <returns>If the items' conditions apply.</returns>
    private static bool ValidSubItems(IReadOnlyList<string> subSplit, Func<string,bool> check)
    {
        var subCount = 0;
        var result = false;
                
        //for each in substring (e.g "x and z or y")
        foreach (var s in subSplit)
        {
            //if this flag is false by the end of loop, it means a condition isn't valid
            bool flag;
                    
            subCount++;
            
            //if string is and/or (always uneven number)
            if (subCount % 2 == 0)
                continue;
                    
            var nextRing = subSplit[subCount + 1];
                    
            //if next check is "and", check for current AND following
            if (subSplit[subCount].Equals("and", StringComparison.OrdinalIgnoreCase))
            {
                flag = check(s) && check(nextRing);
            }
            //otherwise, check either is valid
            else
                flag = check(s) || check(nextRing);

            //if at any point conditions don't match, set such and break loop
            if (!flag)
            {
                result = false;
                break;
            }

            result = true;
        }

        return result;
    }
    
    internal static bool InRequiredLocation(NPC who, string place)
    {
        if (who.currentLocation.Name == place)
        {
            return true;
        }

        return place is null or "any";
    }

    /// <summary>
    /// For validating user additions. Passes the values to another bool, then returns that result.
    /// </summary>
    /// <param name="data">The raw dialogue data to check.</param>
    /// <param name="who">The NPC to check for.</param>
    /// <returns></returns>
    internal static bool IsValid(DialogueData data, string who) //rename to += dialogue
    {
        try
        {
            var time = data.Time;

            //check if text is bubble and if emotes are allowed. if so return false
            if (data.IsBubble && data.Emote is not -1) //removed: "data.MakeEmote == true && "
            {
                Log("Configs \"IsBubble\" and \"Emote\" are mutually exclusive (the two can't be applied at the same time). Patch will not be loaded.", LogLevel.Error);
                return false;
            }

            if (time > 0)
            {
                if (time is <= 600 or >= 2600)
                {
                    Log("Addition has a faulty hour!", LogLevel.Warn);
                    return false;
                }

                if (data.From is not 600 || data.To is not 2600)
                {
                    Log("'From/To' and 'Time' are mutually exclusive.", LogLevel.Warn);
                    return false;
                }
            }
            else if (data.From is 600 && data.To is 2600 && data.Location == "any") //if time isnt set, and from-to is empty + no location
            {
                Log("You must either set a specific Time, a From-To range, or a Location.");
                return false;
            }

            //if set to change facing, check value. if less than 0 and bigger than 3 return false
            if (!string.IsNullOrWhiteSpace(data.FaceDirection))
            {
                var dir = Getter.ReturnFacing(data.FaceDirection);
                if (dir is < 0 or > 3)
                {
                    Log("Addition has a faulty facedirection! Value must be between 0 and 3.", LogLevel.Warn);
                    return false;
                }
            }

            if (!ModEntry.Dialogues.TryGetValue(who, out var dialogue)) return true;
            foreach(var addition in dialogue)
            {
                if (addition.Time != data.Time || addition.Location != data.Location) continue;
                Log($"An entry with the values Time={data.Time} and Location={data.Location} already exists. Skipping.", LogLevel.Warn);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Log($"Error found in contentpack: {ex}", LogLevel.Error);
            return false;
        }
    }

    internal static bool InTimeRange(int newTime, int patchTime, int from, int to, NPC who)
    {
        var mapWithNpc = who.currentLocation.Name;

        if (patchTime > 0)
        {
            if(Cfg.Debug)
            {
                Log($"'Time' = {patchTime}. Returning whether it equals e.NewTime...");
            }

            return patchTime.Equals(newTime);
        }

        if (who.isMovingOnPathFindPath.Value) // to make sure its not patched when NPC is moving
        {
            Log($"Character {who.Name} is moving, patch won't be applied yet");
            return false;
        }

        if (Game1.player.currentLocation.Name != mapWithNpc)
        {
            if (Cfg.Debug)
            {
                Log("Player isnt in NPC location. Returning false.");
            }
            return false;
        }

        if (newTime < from || newTime > to) return 
            false;
        
        if (Cfg.Debug)
        {
            Log("NewTime is bigger than 'from' and lesser than 'to'. Returning true.");
        }
        return true;

    }

    /// <summary>
    /// Check if NPC exists. If null or not in friendship data, returns false.
    /// </summary>
    /// <param name="who"> The NPC to check.</param>
    /// <returns></returns>
    internal static bool Exists(string who) //rename to CharacterExists
    {
        foreach (var name in Game1.player.friendshipData.Keys)
        {
            if (name.Equals(who))
                return true;
        }

        return false;
    }
        
    internal static bool Exists(NPC who)
    {
        var monitor = ModEntry.Mon;
        var admitted = ModEntry.PatchableNPCs;

        if (who is null)
        {
            monitor.Log("NPC could not be found! See log for more details.", LogLevel.Error);
            monitor.Log("NPC returned null when calling  Game1.getCharacterFromName().");
            return false;
        }

        if (admitted.Contains(who.Name)) return true;
            
        monitor.Log($"NPC {who} is not in characters! Did you type their name correctly?", LogLevel.Warn);
        monitor.Log($"NPC {who} seems to exist, but wasn't found in the list of admitted NPCs. This may occur if you haven't met them yet, or if they haven't been unlocked.");
        return false;

    }
       
    /// <summary>
    /// Checks validity of greeting patch (ie. existing NPC and dialogue)
    /// </summary>
    /// <param name="chara"></param>
    /// <param name="dialogue"></param>
    /// <returns></returns>
    internal static bool IsValidGreeting(NPC chara, string dialogue)
    {
        if (chara is null)
        {
            Log("Character couldn't be found.");
            return false;
        }

        if (!string.IsNullOrWhiteSpace(dialogue)) 
            return true;
        
        Log("There's no dialogue!");
        return false;
    }
       
    /// <summary>
    /// Checks validity of notif patch.
    /// </summary>
    /// <param name="notif"> the notification data.</param>
    /// <returns></returns>
    internal static bool IsValidNotif(NotificationData notif)
    {
        if (string.IsNullOrWhiteSpace(notif.Message))
        {
            Log("No message found.");
            return false;
        }
        switch (notif.Time)
        {
            case <= 0 when string.IsNullOrWhiteSpace(notif.Message) || notif.Message is "any":
                Log("You must either set time and/or location!");
                return false;
            case > 0 and (<= 600 or >= 2600):
                Log("Time isn't valid!");
                return false;
        }

        if (string.IsNullOrWhiteSpace(notif.Sound)) 
            return true;
        
        ICue sb;
        try
        {
            sb = Game1.soundBank.GetCue(notif.Sound);
        }
        catch
        {
            sb = null;
        }

        if (sb is not null) 
            return true;
            
        Log("There's no sound with that name!");
        return false;

    }

    /// <summary>
    /// Checks question's validity.
    /// </summary>
    /// <param name="q"></param>
    /// <returns></returns>
    internal static bool IsValidQuestion(QuestionData q)
    {
        if(string.IsNullOrWhiteSpace(q.Question))
        {
            Log("Question must have text!",LogLevel.Error);
            return false;
        }

        if(string.IsNullOrWhiteSpace(q.Answer))
        {
            Log("Answer must have text!",LogLevel.Error);
            return false;
        }

        if(q.MaxTimesAsked < 0)
        {
            Log("Max times asked can't be less than 0!",LogLevel.Error);
            return false;
        }

        if(q.From is 600 || q.To is 2600)
        {
            Log("Time can't start at 600 or end at 2600.",LogLevel.Error);
            return false;
        }

        if (q.MaxTimesAsked != 1 && q.QuestToStart != "none")
        {
            Log("Quest dialogue can only be asked once. Change 'MaxTimesAsked' to 1 and try again.", LogLevel.Error);
        }
        return true;
    }
}