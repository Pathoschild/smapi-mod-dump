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
using StardewValley;
using lv = StardewModdingAPI.LogLevel;

namespace DynamicDialogues
{
    [SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
    internal static class Debug
    {
        internal static void Print(string arg1, string[] arg2)
        {
            if (!arg2.Any() || arg2.Length == 0)
            {
                ModEntry.Mon.Log("Please specify a type. (Possible values: Dialogues, Random, Questions, Notifs", lv.Warn);
                return;
            }

            var det = arg2.Contains("det");

            if (arg2.Contains("Dialogues"))
            {
                var dials = "Dialogues: ";
                if(ModEntry.Dialogues == null || ModEntry.Dialogues?.Count == 0)
                {
                    dials += "None";
                    ModEntry.Mon.Log(dials, lv.Info);
                }
                else
                {
                    // ReSharper disable once PossibleNullReferenceException
                    foreach (var pair in ModEntry.Dialogues)
                    {
                        dials += pair.Key + $" ({pair.Value?.Count}), ";
                    }
                    ModEntry.Mon.Log(dials, lv.Info);
                }
            }
            if (arg2.Contains("Notifs"))
            {
                var noti = "Notifications: ";

                if (ModEntry.Notifs?.Count == 0 || ModEntry.Notifs == null)
                {
                    noti += "none";
                    ModEntry.Mon.Log(noti, lv.Info);
                }
                else
                {
                    foreach (var data in ModEntry.Notifs)
                    {
                        noti += "\n";

                        noti += $"Message: {data.Message}, Time: {data.Time}, Location: {data.Location}, IsBox: {data.IsBox} Sound: {data.Sound},";
                    }
                    ModEntry.Mon.Log(noti, lv.Info);
                }

            }
            if (arg2.Contains("Random"))
            {
                var ran = "Random dialogues: ";

                if(ModEntry.RandomPool?.Count == 0 || ModEntry.RandomPool == null)
                {
                    ran += "none";
                    ModEntry.Mon.Log(ran, lv.Info);
                }
                else
                {
                    foreach(var pair in ModEntry.RandomPool)
                    {
                        if (det)
                            ran += "\n";

                        ran += pair.Key + $" {(det ? Listify(pair.Value) : "("+pair.Value?.Count+"), ")}";
                    }
                    ModEntry.Mon.Log(ran, lv.Info);
                }
            }

            if (!arg2.Contains("Questions")) return;
            
            var qs = "Questions: ";

            if (ModEntry.Questions == null || ModEntry.Questions?.Count == 0)
            {
                qs += "None";
                ModEntry.Mon.Log(qs, lv.Info);
            }
            else
            {
                // ReSharper disable once PossibleNullReferenceException
                foreach (var pair in ModEntry.Dialogues)
                {
                    qs += pair.Key + $" ({pair.Value?.Count}), ";
                }
                ModEntry.Mon.Log(qs, lv.Info);
            }
        }

        private static string Listify(List<string> value)
        {
            var result = "\n";
            foreach(var text in value)
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
                    ModEntry.Mon.Log("Format: sayHiTo <npc talking> <npc to greet>.\nExample: `sayHiTo Alex Evelyn`",lv.Warn);
                    return;
                }

                var chara = arg2[0];
                var who = Game1.getCharacterFromName(chara);
                var pos = Game1.player.Position;
                pos.X++;
                
                Game1.warpCharacter(who,Game1.player.currentLocation,pos);

                var chara2 = arg2[1];
                var greeted = Game1.getCharacterFromName(chara2);

                who.sayHiTo(greeted);
            }
            catch (Exception e)
            {
                ModEntry.Mon.Log($"Error: {e}", lv.Error);
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

                if (!ModEntry.Questions.ContainsKey(who))
                    return;

                var data = ModEntry.Questions[who];

                var position = 0;
                string result = null;
                foreach (var rq in data)
                {
                    position++;
                    var addition = $"\n[{position}]\nQuestion:{rq.Question}\nAnswer:{rq.Answer}\n";
                    result += addition;
                }
                
                ModEntry.Mon.Log(result ?? "No data was found.",lv.Info);
            }
            catch (Exception e)
            {
                ModEntry.Mon.Log($"Error: {e}", lv.Error);
                throw;
            }
        }
    }
}
