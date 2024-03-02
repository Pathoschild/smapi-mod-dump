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
using System.Linq;
using DynamicDialogues.Models;
using StardewModdingAPI;
using StardewValley;

namespace DynamicDialogues;

[SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
internal static class Debug
{
    #region references
    //we reference them here because i don't want modentry's references to be so clogged
    private static Dictionary<string, List<QuestionData>> Question => ModEntry.Questions;
    private static Dictionary<string, List<DialogueData>> Dialog => ModEntry.Dialogues;
    private static Dictionary<string, List<string>> RandomDialogue => ModEntry.RandomPool;
    private static List<NotificationData> Notifications => ModEntry.Notifs;
    private static void Log(string msg, LogLevel logLevel = LogLevel.Trace) => ModEntry.Mon.Log(msg, logLevel);
    #endregion

    internal static void Print(string arg1, string[] arg2)
    {
        if (!arg2.Any() || arg2.Length == 0)
        {
            Log("Please specify a type. (Possible values: Dialogues, Random, Questions, Notifs", LogLevel.Warn);
            return;
        }

        var det = arg2.Contains("det");

        if (arg2.Contains("Dialogues"))
        {
            var dials = "Dialogues: ";
            if (Dialog == null || Dialog?.Count == 0)
            {
                dials += "None";
                Log(dials, LogLevel.Info);
            }
            else
            {
                // ReSharper disable once PossibleNullReferenceException
                foreach (var pair in Dialog)
                {
                    dials += pair.Key + $" ({pair.Value?.Count}), ";
                }
                Log(dials, LogLevel.Info);
            }
        }
        if (arg2.Contains("Notifs"))
        {
            var noti = "Notifications: ";

            if (Notifications?.Count == 0 || Notifications == null)
            {
                noti += "none";
                Log(noti, LogLevel.Info);
            }
            else
            {
                foreach (var data in Notifications)
                {
                    noti += "\n";

                    noti += $"Message: {data.Message}, Time: {data.Time}, Location: {data.Location}, IsBox: {data.IsBox} Sound: {data.Sound},";
                }
                Log(noti, LogLevel.Info);
            }

        }
        if (arg2.Contains("Random"))
        {
            var ran = "Random dialogues: ";

            if (RandomDialogue?.Count == 0 || RandomDialogue == null)
            {
                ran += "none";
                Log(ran, LogLevel.Info);
            }
            else
            {
                foreach (var pair in RandomDialogue)
                {
                    if (det)
                        ran += "\n";

                    ran += pair.Key + $" {(det ? Listify(pair.Value) : "(" + pair.Value?.Count + "), ")}";
                }
                Log(ran, LogLevel.Info);
            }
        }

        if (!arg2.Contains("Questions")) return;

        var qs = "Questions: ";

        if (Question == null || Question?.Count == 0)
        {
            qs += "None";
            Log(qs, LogLevel.Info);
        }
        else
        {
            // ReSharper disable once PossibleNullReferenceException
            foreach (var pair in Dialog)
            {
                qs += pair.Key + $" ({pair.Value?.Count}), ";
            }
            Log(qs, LogLevel.Info);
        }
    }

    private static string Listify(List<string> value)
    {
        var result = "\n";
        foreach (var text in value)
        {
            result += $"\"{text}\"\n";
        }
        return result;
    }

    public static void SayHiTo(string arg1, string[] arg2)
    {
        try
        {
            if (arg2.Length != 2)
            {
                Log("Format: sayHiTo <npc talking> <npc to greet>.\nExample: `sayHiTo Alex Evelyn`", LogLevel.Warn);
                return;
            }

            var chara = arg2[0];
            var who = Game1.getCharacterFromName(chara);
            var pos = Game1.player.Position;
            pos.X++;

            Game1.warpCharacter(who, Game1.player.currentLocation, pos);

            var chara2 = arg2[1];
            var greeted = Game1.getCharacterFromName(chara2);

            who.sayHiTo(greeted);
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
            throw;
        }
    }

    public static void GetQuestionsFor(string arg1, string[] arg2)
    {
        if (arg2 == null || !arg2.Any())
            return;

        try
        {
            var who = arg2[0];
            if (string.IsNullOrWhiteSpace(who))
                return;

            if (!Question.ContainsKey(who))
                return;

            var data = Question[who];

            var position = 0;
            string result = null;
            foreach (var rq in data)
            {
                position++;
                var addition = $"\n[{position}]\nQuestion:{rq.Question}\nAnswer:{rq.Answer}\n";
                result += addition;
            }

            Log(result ?? "No data was found.", LogLevel.Info);
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
            throw;
        }
    }
}