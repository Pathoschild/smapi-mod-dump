/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using StardewValley;
using StardewModdingAPI;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DynamicDialogues.Framework
{
    [SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
    internal static class Parser
    {
        /// <summary>
        /// Checks if the conditions match to add a dialogue.
        /// </summary>
        /// <param name="which">The items the player must have.</param>
        /// <returns></returns>
        internal static bool HasItems(PlayerConditions which)
        {
            var hat = true;
            var shirt = true;
            var pants = true;
            var rings = true;
            var inv = true;
            
            if (!string.IsNullOrWhiteSpace(which.Hat))
            {
                hat = Game1.player.hat.Value.ItemId == which.Hat;
            }
            if (!string.IsNullOrWhiteSpace(which.Shirt))
            {
                shirt = Game1.player.shirtItem.Value.ItemId == which.Shirt;
            }
            if (!string.IsNullOrWhiteSpace(which.Pants))
            {
                pants = Game1.player.pantsItem.Value.ItemId == which.Pants;
            }
            if (!string.IsNullOrWhiteSpace(which.Rings))
            {
                rings = HasRings(which.Rings);
            }
            if (!string.IsNullOrWhiteSpace(which.Inventory))
            {
                inv = InInventory(which.Inventory);
            }

            return hat && shirt && pants && rings && inv;
        }

        /// <summary>
        /// Returns if item(s) are in user's inventory.
        /// </summary>
        /// <param name="which">The item(s) to check.</param>
        /// <returns></returns>
        private static bool InInventory(string which)
        {
            //Game1.player.Items.Any(i => i.ItemId == which.Inventory)
            //this assumes a format of "id AND id"
            if (which.Contains("AND", StringComparison.Ordinal))
            {
                var items = which.Split(' ');
                foreach (var id in items)
                {
                    if (id == "AND" || string.IsNullOrWhiteSpace(id))
                        continue;

                    var has = Game1.player.Items.Any(i => i.ItemId == id);
                    
                    if (!has)
                        return false;
                }

                return true;
            }
            //assumes format of "id OR id (...)"
            else if(which.Contains("OR",StringComparison.Ordinal))
            {
                var items = which.Split(' ');
                foreach (var id in items)
                {
                    if (id == "OR" || string.IsNullOrWhiteSpace(id))
                        continue;

                    var has = Game1.player.Items.Any(i => i.ItemId == id);
                    
                    if (has)
                        return true;
                }

                return false;
            }
            //assumes single ID
            else
            {
                return Game1.player.Items.Any(i => i.ItemId == which);
            }
        }
        
        /// <summary>
        /// Returns whether player is wearing a ring.
        /// </summary>
        /// <param name="which">Ring(s) to check for.</param>
        /// <returns></returns>
        private static bool HasRings(string which)
        {
            //this assumes a format of "id AND id"
            if (which.Contains("AND", StringComparison.Ordinal))
            {
                var rings = which.Split(' ');
                var ring1 = Game1.player.isWearingRing(rings[0]);
                var ring2 = Game1.player.isWearingRing(rings[2]);

                return ring1 && ring2;
            }
            //assumes format of "id OR id (...)"
            else if(which.Contains("OR",StringComparison.Ordinal))
            {
                var rings = which.Split(' ');
                foreach (var id in rings)
                {
                    if (id == "OR" || string.IsNullOrWhiteSpace(id))
                        continue;

                    var isWearing = Game1.player.isWearingRing(id);
                    
                    if (isWearing)
                        return true;
                }

                return false;
            }
            //assumes single ID
            else
            {
                return Game1.player.isWearingRing(which);
            }
        }
        
        /// <summary>
        /// If the NPC is in the required location, return true. Defaults to true if location is any/null.
        /// </summary>
        /// <param name="who"> The NPC to check.</param>
        /// <param name="place">The place to use for comparison.</param>
        /// <returns></returns>
        internal static bool InRequiredLocation(NPC who, GameLocation place) => InRequiredLocation(who, place.Name);

        internal static bool InRequiredLocation(NPC who, string place)
        {
            if (who.currentLocation.Name == place)
            {
                return true;
            }
            else if (place is null or "any")
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
        /// <param name="data">The raw dialogue data to check.</param>
        /// <param name="who">The NPC to check for.</param>
        /// <returns></returns>
        internal static bool IsValid(RawDialogues data, string who) //rename to += dialogue
        {
            try
            {
                var time = data.Time;

                //check if text is bubble and if emotes are allowed. if so return false
                if (data.IsBubble && data.Emote is not -1) //removed: "data.MakeEmote == true && "
                {
                    ModEntry.Mon.Log("Configs \"IsBubble\" and \"Emote\" are mutually exclusive (the two can't be applied at the same time). Patch will not be loaded.", LogLevel.Error);
                    return false;
                }

                if (time > 0)
                {
                    if (time is <= 600 or >= 2600)
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
                if (!string.IsNullOrWhiteSpace(data.FaceDirection))
                {
                    var dir = Getter.ReturnFacing(data.FaceDirection);
                    if (dir is < 0 or > 3)
                    {
                        ModEntry.Mon.Log($"Addition has a faulty facedirection! Value must be between 0 and 3.", LogLevel.Warn);
                        return false;
                    }
                }

                if (!ModEntry.Dialogues.TryGetValue(who, out var dialogue)) return true;
                foreach(var addition in dialogue)
                {
                    if (addition.Time != data.Time || addition.Location != data.Location) continue;
                    ModEntry.Mon.Log($"An entry with the values Time={data.Time} and Location={data.Location} already exists. Skipping.", LogLevel.Warn);
                    return false;
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
            var mapWithNpc = who.currentLocation.Name;

            if (patchTime > 0)
            {
                if(ModEntry.Config.Debug)
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

                if (Game1.player.currentLocation.Name != mapWithNpc)
                {
                    if (ModEntry.Config.Debug)
                    {
                        ModEntry.Mon.Log("Player isnt in NPC location. Returning false.");
                    }
                    return false;
                }

                if(newTime >= from && newTime <= to)
                {
                    if (ModEntry.Config.Debug)
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
                monitor.Log($"NPC could not be found! See log for more details.", LogLevel.Error);
                monitor.Log($"NPC returned null when calling  Game1.getCharacterFromName().");
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

        /// <summary>
        /// Checks question's validity.
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        internal static bool IsValidQuestion(RawQuestions q)
        {
            if(string.IsNullOrWhiteSpace(q.Question))
            {
                ModEntry.Mon.Log("Question must have text!",LogLevel.Error);
                return false;
            }

            if(string.IsNullOrWhiteSpace(q.Answer))
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

            if (q.MaxTimesAsked != 1 && q.QuestToStart != "none")
            {
                ModEntry.Mon.Log("Quest dialogue can only be asked once. Change 'MaxTimesAsked' to 1 and try again.", LogLevel.Error);
            }
            return true;
        }
    }
}