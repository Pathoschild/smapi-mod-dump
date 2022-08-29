/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/DynamicDialogues
**
*************************************************/

using StardewValley;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicDialogues
{
    internal class Parser
    {
        /// <summary>
        /// If the NPC is in the required location, return true. Defaults to true if location is any/null.
        /// </summary>
        /// <param name="who"> The NPC to check.</param>
        /// <param name="place">The place to use for comparison.</param>
        /// <returns></returns>
        internal static bool InRequiredLocation(NPC who, GameLocation place)
        {

            if (who.currentLocation == place)
            {
                return true;
            }
            else if (place is null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        internal static bool InRequiredLocation(NPC who, string place)
        {
            if (who.currentLocation.Name == place)
            {
                return true;
            }
            else if (place is null || place is "any")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static bool InRequiredLocation(string who, GameLocation place)
        {
            var npc = Game1.getCharacterFromName(who);
            
            if (npc.currentLocation == place)
            {
                return true;
            }
            else if (place is null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        /// <summary>
        /// For validating user additions. Passes the values to another bool, then returns that result.
        /// </summary>
        /// <param name="which">The raw dialogue data to check.</param>
        /// <returns></returns>
        internal static bool IsValid(RawDialogues data, string who) //rename to += dialogue
        {
            try
            {
                var time = data.Time;

                //check if text is bubble and if emotes are allowed. if so return false
                if (data.IsBubble == true && data.Emote is not -1) //removed: "data.MakeEmote == true && "
                {
                    ModEntry.Mon.Log("Configs \"IsBubble\" and \"Emote\" are mutually exclusive (the two can't be applied at the same time). Patch will not be loaded.", LogLevel.Error);
                    return false;
                }

                if (time > 0)
                {
                    if (time <= 600 || time >= 2600)
                    {
                        ModEntry.Mon.Log($"Addition has a faulty hour!", LogLevel.Warn);
                        return false;
                    }

                    if (data.From is not 600 || data.To is not 2600)
                    {
                        ModEntry.Mon.Log($"'From/To' and 'Time' are mutually exclusive.", LogLevel.Warn);
                        return false;
                    }
                }
                else if (data.From is 600 && data.To is 2600 && data.Location == "any") //if time isnt set, and from-to is empty + no location
                {
                    ModEntry.Mon.Log($"You must either set a specific Time, a From-To range, or a Location.");
                    return false;
                }

                //if set to change facing, check value. if less than 0 and bigger than 3 return false
                if (!String.IsNullOrWhiteSpace(data.FaceDirection))
                {
                    var dir = Getter.ReturnFacing(data.FaceDirection);
                    if (dir < 0 || dir > 3)
                    {
                        ModEntry.Mon.Log($"Addition has a faulty facedirection! Value must be between 0 and 3.", LogLevel.Warn);
                        return false;
                    }
                }

                if (ModEntry.Dialogues.ContainsKey(who))
                {
                    foreach(var addition in ModEntry.Dialogues[who])
                    {
                        if(addition.Time == data.Time && addition.Location.ToString() == data.Location)
                        {
                            ModEntry.Mon.Log($"An entry with the values Time={data.Time} and Location={data.Location} already exists. Skipping.", LogLevel.Warn);
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ModEntry.Mon.Log($"Error found in contentpack: {ex}", LogLevel.Error);
                return false;
            }
        }

        internal static bool InTimeRange(int newTime, int patchTime, int from, int to, NPC who)
        {
            var MapWithNPC = who.currentLocation.Name;

            if (patchTime > 0)
            {
                if(ModEntry.Debug)
                {
                    ModEntry.Mon.Log($"'Time' = {patchTime}. Returning whether it equals e.NewTime...");
                }

                return patchTime.Equals(newTime);
            }
            else
            {
                if (who.isMovingOnPathFindPath.Value) // to make sure its not patched when NPC is moving
                {
                    ModEntry.Mon.Log($"Character {who.Name} is moving, patch won't be applied yet");
                    return false;
                }

                if (!(Game1.player.currentLocation.Name == MapWithNPC))
                {
                    if (ModEntry.Debug)
                    {
                        ModEntry.Mon.Log("Player isnt in NPC location. Returning false.");
                    }
                    return false;
                }

                if(newTime >= from && newTime <= to)
                {
                    if (ModEntry.Debug)
                    {
                        ModEntry.Mon.Log("NewTime is bigger than 'from' and lesser than 'to'. Returning true.");
                    }
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if NPC exists. If null or not in friendship data, returns false.
        /// </summary>
        /// <param name="who"> The NPC to check.</param>
        /// <returns></returns>
        internal static bool Exists(string who) //rename to CharacterExists
        {
            var monitor = ModEntry.Mon;
            var admitted = ModEntry.NPCDispositions;
            var character = Game1.getCharacterFromName(who);

            if (character is null)
            {
                monitor.Log($"NPC {who} could not be found! See log for more details.", LogLevel.Error);
                monitor.Log($"NPC {who} returned null when calling  Game1.getCharacterFromName({who}).");
                return false;
            }

            if (!admitted.Contains(character.Name))
            {
                monitor.Log($"NPC {who} is not in characters! Did you type their name correctly?", LogLevel.Warn);
                monitor.Log($"NPC {who} seems to exist, but wasn't found in the list of admitted NPCs. This may occur if you haven't met them yet, or if they haven't been unlocked.");
                return false;
            }

            return true;
        }
        internal static bool Exists(NPC who)
        {
            var monitor = ModEntry.Mon;
            var admitted = ModEntry.NPCDispositions;

            if (who is null)
            {
                monitor.Log($"NPC {who} could not be found! See log for more details.", LogLevel.Error);
                monitor.Log($"NPC {who} returned null when calling  Game1.getCharacterFromName({who}).");
                return false;
            }

            if (!admitted.Contains(who.Name))
            {
                monitor.Log($"NPC {who} is not in characters! Did you type their name correctly?", LogLevel.Warn);
                monitor.Log($"NPC {who} seems to exist, but wasn't found in the list of admitted NPCs. This may occur if you haven't met them yet, or if they haven't been unlocked.");
                return false;
            }

            return true;
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
                ModEntry.Mon.Log("Character couldn't be found.");
                return false;
            }
            if (String.IsNullOrWhiteSpace(dialogue))
            {
                ModEntry.Mon.Log("There's no dialogue!");
                return false;
            }

            return true;
        }
       
        /// <summary>
        /// Checks validity of notif patch.
        /// </summary>
        /// <param name="notif"> the notification data.</param>
        /// <returns></returns>
        internal static bool IsValidNotif(RawNotifs notif)
        {
            if (String.IsNullOrWhiteSpace(notif.Message))
            {
                ModEntry.Mon.Log("No message found.");
                return false;
            }
            if (notif.Time <= 0 && (String.IsNullOrWhiteSpace(notif.Message) || notif.Message is "any"))
            {
                ModEntry.Mon.Log("You must either set time and/or location!");
                return false;
            }
            if(notif.Time > 0 && (notif.Time <= 600 || notif.Time >= 2600))
            {
                ModEntry.Mon.Log("Time isn't valid!");
                return false;
            }
            if(!String.IsNullOrWhiteSpace(notif.Sound))
            {
                ICue sb;
                try
                {
                    sb = Game1.soundBank.GetCue(notif.Sound);
                }
                catch
                {
                    sb = null;
                }
                if (sb is null)
                {
                    ModEntry.Mon.Log("There's no sound with that name!");
                    return false;
                }
            }

            return true;
        }

        internal static bool IsValidQuestion(RawQuestions q)
        {
            if(String.IsNullOrWhiteSpace(q.Question))
            {
                ModEntry.Mon.Log("Question must have text!",LogLevel.Error);
                return false;
            }

            if(String.IsNullOrWhiteSpace(q.Answer))
            {
                ModEntry.Mon.Log("Answer must have text!",LogLevel.Error);
                return false;
            }

            if(q.MaxTimesAsked < 0)
            {
                ModEntry.Mon.Log("Max times asked can't be less than 0!",LogLevel.Error);
                return false;
            }

            if(q.From is 600 || q.To is 2600)
            {
                ModEntry.Mon.Log("Time can't start at 600 or end at 2600.",LogLevel.Error);
                return false;
            }

            return true;
        }
    }
}