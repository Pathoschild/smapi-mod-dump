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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace DynamicDialogues.Framework
{
    [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
    internal static class Getter
    {
        /// <summary>
        /// Formats the bubble set by user. "@" is replaced by player name.
        /// </summary>
        /// <param name="which">The dialogue to check.</param>
        /// <returns></returns>
        internal static string FormatBubble(string which)
        {
            var result = which;

            var rawspan = which.AsSpan();
            if (rawspan.Contains('@'))
            {
                result = which.Replace("@", Game1.player?.Name);
            }

            return result;
        }

        /// <summary>
        /// Returns the FaceDirection string to int, since it also accepts up/down/etc values.
        /// </summary>
        /// <param name="which">The string to check.</param>
        /// <returns></returns>
        internal static int ReturnFacing(string which)
        {
            if (String.IsNullOrWhiteSpace(which))
            {
                return -1;
            }
            var word = which.ToLower();

            switch (word)
            {
                case "up":
                    return 0;
                case "right":
                    return 1;
                case "down":
                    return 2;
                case "left":
                    return 3;
                default:
                    try
                    {
                        var toInt = int.Parse(which);
                        if (toInt is >= 0 and <= 3)
                        {
                            return toInt;
                        }
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch(Exception)
                    { }

                    return -1;
            }
        }

        /// <summary>
        /// Returns a string with all questions/answers a character can give.
        /// </summary>
        /// <param name="questionAndAnswer">List of q data.</param>
        /// <param name="who">The NPC.</param>
        /// <returns></returns>
        internal static string QuestionDialogue(List<RawQuestions> questionAndAnswer, NPC who)
        {
            //$y seems to be infinite version of question. so it's used for this
            var result = "$qna#";
            var questions = "";
            var answers = "";
            var missions = "";

            foreach(var extra in questionAndAnswer)
            {
                if(extra.From > Game1.timeOfDay || extra.To < Game1.timeOfDay)
                {
                    continue;
                }
                if(extra.Location is not "any" && extra.Location.Equals(who.currentLocation.Name) == false)
                {
                    continue;
                }

                if(extra.MaxTimesAsked > 0)
                {
                    var count = 0;
                    if (ModEntry.QuestionCounter.TryGetValue(extra.Question, out var value))
                    {
                        count = value;
                    }
                    else
                    {
                        ModEntry.QuestionCounter.Add(extra.Question, 0);
                    }

                    if (count >= extra.MaxTimesAsked) continue;
                    questions += $"{extra.Question}_";
                    answers += $"{extra.Answer}_";
                    missions += $"{extra.QuestToStart ?? "none"}_";
                    ModEntry.QuestionCounter[extra.Question]++;
                }
                else
                {
                    questions += $"{extra.Question}_";
                    answers += $"{extra.Answer}_";
                    missions += $"{extra.QuestToStart ?? "none"}_";
                }
            }
            
            //if the last char is '_', remove it from string.
            if(questions.Last().Equals('_'))
            {
                var q = new StringBuilder(questions);
                var a = new StringBuilder(answers);
                var m = new StringBuilder(missions);
                q.Remove(q.Length - 1, 0);
                a.Remove(a.Length - 1, 0);
                m.Remove(m.Length - 1, 0);

                questions = q.ToString();
                answers = a.ToString();
                missions = m.ToString();
            }

            result += $"{questions}#{answers}#{missions}"; //changed ¬ to #

            return result;
        }

        /// <summary>
        /// Return a random dialogue from a list.
        /// </summary>
        /// <param name="data">The list to be used.</param>
        /// <param name="who">The NPC.</param>
        /// <returns></returns>
        internal static string RandomDialogue(List<string> data, string who)
        {
            if (data == null || data.Count == 0)
            {
                ModEntry.Mon.Log("data file is empty.");
                return null;
            }

            var index = Game1.random.Next(data.Count);

            //if its already been used, check for another
            if (ModEntry.AlreadyPatched.Contains((who, $"{index}", "random")))
            {
                for(var i = 0; i < data.Count; i++)
                {
                    index = Game1.random.Next(data.Count);

                    if (ModEntry.AlreadyPatched.Contains((who, $"{index}", "random")) == false)
                    {
                        break;
                    }

                    //if all have been added, set index to -1
                    if(i == (data.Count -1))
                    {
                        index = -1;
                    }
                }
            }

            //if index is -1, return null
            if (index == -1)
            {
                return null;
            }

            ModEntry.AlreadyPatched?.Add((who, $"{index}", "random"));

            return data[index];
        }
        
        /*
         * deprecated in uhh check later
        /// <summary>
        /// Return a formatted question, which uses a random RawMission from a list.
        /// </summary>
        /// <param name="missions"></param>
        /// <param name="who"></param>
        /// <returns></returns>
        internal static RawMission RandomMission(List<RawMission> data)
        {
            var index = Game1.random.Next(data.Count);
            var mission = data[index];

            mission = ValidateMission(mission, data);

            return mission;
        }
        
        /// <summary>
        /// Will check that mission conditions match. If not, attempts two retries (returns null if both fail).
        /// </summary>
        /// <param name="which">The quest conditions to use.</param>
        /// <param name="data">The list, in case of requiring a retry.</param>
        /// <returns></returns>
        internal static RawMission ValidateMission(RawMission which, List<RawMission> data)
        {
            var result = which;

            if(which.From < Game1.timeOfDay || Game1.timeOfDay > which.To || (Game1.getLocationFromName(which.Location) == null && which.Location is not "any"))
            {
                if (data.Count is 1)
                {
                    return null;
                }

                var index = Game1.random.Next(data.Count);
                var retry = data[index];

                if (retry.From < Game1.timeOfDay || Game1.timeOfDay > retry.To || (Game1.getLocationFromName(retry.Location) == null && retry.Location is not "any"))
                {
                    return null;
                }
            }

            return result;
        }
        
        /// <summary>
        /// Get responses to a specific mission request.
        /// </summary>
        /// <param name="mission">The data to use for dialogues.</param>
        /// <returns></returns>
        internal static Response[] GetResponses(RawMission mission)
        {
            var result = new Response[2];
            result[0] = new Response($"DD_accept_{mission.ID}", mission.AcceptQuest);
            result[1] = new Response($"DD_rejectedQuest", mission.RejectQuest);

            return result;
        }
        
        /// <summary>
        /// Adds mission to player if the specific choice was made.
        /// </summary>
        /// <param name="who">The farmer to add it to.</param>
        /// <param name="which">The string to check.</param>
        internal static void AddMission(Farmer who, string which)
        {
            if(which.Equals("DD_rejectedQuest"))
            {
                return;
            }
            else
            {
                int numOnly = 0;
                try
                {
                    numOnly = int.Parse(which.Remove(0, 10));
                }
                catch (Exception ex)
                {
                    ModEntry.Mon.Log($"An error has occurred: {ex}", StardewModdingAPI.LogLevel.Error);
                }

                //logging and error-avoiding
                if(ModEntry.Config.Debug)
                {
                    ModEntry.Mon.Log($"string which = {which}; int numOnly = {numOnly};", StardewModdingAPI.LogLevel.Debug);
                }
                if(numOnly is 0)
                {
                    return;
                }
                
                //if parsed correctly, add quest to player
                who.addQuest(numOnly);
            }
        }*/
        
        /// <summary>
        /// Return the index in a dictionary.
        /// </summary>
        /// <param name="dict">dictionary to check</param>
        /// <param name="name">the key whose index to return</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">if it's not in the dictionary</exception>
        internal static int GetIndex(Dictionary<string, RawQuestions> dict, string name)
        {
            var position = 0;
            foreach(var key in dict.Keys)
            {
                if(key.Equals(name))
                {
                    return position;
                }

                position++;
            }

            throw new ArgumentOutOfRangeException(); //not in dict
        }

        /// <summary>
        /// Array with frames for an animation, taken from a contentpack's patch.
        /// </summary>
        /// <param name="data">the string to split</param>
        /// <returns></returns>
        internal static int[] FramesForAnimation(string data)
        {
            var asList = data.Split(' ');
            var result = new List<int>();

            foreach(var item in asList)
            {
                var parsedItem = int.Parse(item);
                result.Add(parsedItem);
            }

            return result.ToArray();
        }
    }
}
