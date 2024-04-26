/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace Swim
{
    internal class SwimDialog
    {
        private static IMonitor SMonitor;
        private static ModConfig Config;
        private static IModHelper SHelper;
        private static List<int> marinerQuestions;

        public static void Initialize(IMonitor monitor, IModHelper helper, ModConfig config)
        {
            SMonitor = monitor;
            Config = config;
            SHelper = helper;

            marinerQuestions = new List<int>();

        }

        internal static void OldMarinerDialogue(string whichAnswer)
        {
            string playerTerm = Game1.content.LoadString("Strings\\Locations:Beach_Mariner_Player_" + (Game1.player.IsMale ? "Male" : "Female"));
            SMonitor.Log("answer " + whichAnswer);
            if (whichAnswer == "SwimMod_Mariner_Questions_Yes")
            {
                CreateMarinerQuestions();
                string preface = SHelper.Translation.Get(Game1.player.mailReceived.Contains("SwimMod_Mariner_Already") ? "SwimMod_Mariner_Questions_Yes_Old" : "SwimMod_Mariner_Questions_Yes");
                Game1.player.mailReceived.Add("SwimMod_Mariner_Already");
                ShowNextQuestion(preface, 0);
            }
            else if (whichAnswer == "SwimMod_Mariner_Questions_No")
            {
                string preface = SHelper.Translation.Get(Game1.player.mailReceived.Contains("SwimMod_Mariner_Already") ? "SwimMod_Mariner_Questions_No_Old" : "SwimMod_Mariner_Questions_No");
                Game1.player.mailReceived.Add("SwimMod_Mariner_Already");
                Game1.drawObjectDialogue(preface);
            }
            else if (whichAnswer.StartsWith("SwimMod_Mariner_Question_"))
            {
                SMonitor.Log($"answered question {whichAnswer}");
                string[] keys = whichAnswer.Split('_');
                string preface = "";
                switch(keys[keys.Length-1])
                {
                    case "Y":
                        preface = string.Format(SHelper.Translation.Get($"SwimMod_Mariner_Answer_Y_{keys[keys.Length - 3]}"), playerTerm);
                        break;
                    case "N":
                        preface = string.Format(SHelper.Translation.Get("SwimMod_Mariner_Answer_N"), playerTerm);
                        Game1.drawObjectDialogue(preface);
                        ModEntry.marinerQuestionsWrongToday.Value = true;
                        return;
                    case "S":
                        preface = string.Format(SHelper.Translation.Get("SwimMod_Mariner_Answer_S"), playerTerm);
                        Game1.drawObjectDialogue(preface);
                        ModEntry.marinerQuestionsWrongToday.Value = true;
                        return;
                }
                int next = int.Parse(keys[keys.Length - 2]) + 1;
                if(marinerQuestions.Count > next)
                {
                    ShowNextQuestion(preface, next);
                }
                else
                {
                    CompleteEvent();
                }
            }
        }

        private static void CreateMarinerQuestions()
        {
            if (marinerQuestions.Count == 0)
            {
                int i = 1;
                while (true)
                {
                    Translation r = SHelper.Translation.Get($"SwimMod_Mariner_Question_{i}");
                    if (!r.HasValue())
                        break;
                    marinerQuestions.Add(i++);
                }
            }
            int n = marinerQuestions.Count;
            while (n > 1)
            {
                n--;
                int k = ModEntry.myRand.Value.Next(n + 1);
                var value = marinerQuestions[k];
                marinerQuestions[k] = marinerQuestions[n];
                marinerQuestions[n] = value;
            }
        }

        private static void ShowNextQuestion(string preface, int index)
        {
            int qi = marinerQuestions[index];
            Translation s2 = SHelper.Translation.Get($"SwimMod_Mariner_Question_{qi}");
            if (!s2.HasValue())
            {
                SMonitor.Log("no dialogue: " + s2.ToString(), LogLevel.Error);
                return;
            }
            //Monitor.Value.Log("has dialogue: " + s2.ToString());
            List<Response> responses = new List<Response>();
            int i = 1;
            while (true)
            {
                Translation r = SHelper.Translation.Get($"SwimMod_Mariner_Question_{qi}_{i}");
                if (!r.HasValue())
                    break;
                string str = r.ToString().Split('#')[0];
                SMonitor.Log(str);

                responses.Add(new Response($"SwimMod_Mariner_Question_{qi}_{index}_{r.ToString().Split('#')[1]}", str));
                i++;
            }
            //Monitor.Value.Log($"next question: {preface}{s2}");
            Game1.player.currentLocation.createQuestionDialogue($"{preface}{s2}", responses.ToArray(), $"SwimMod_Mariner_Question_{qi}");
        }
        private static void CompleteEvent()
        {
            string playerTerm = Game1.content.LoadString("Strings\\Locations:Beach_Mariner_Player_" + (Game1.player.IsMale ? "Male" : "Female"));
            string preface = SHelper.Translation.Get("SwimMod_Mariner_Completed");
            Game1.drawObjectDialogue(string.Format(preface, playerTerm));
            Game1.stopMusicTrack(StardewValley.GameData.MusicContext.Default);
            Game1.playSound("Cowboy_Secret");
            Game1.player.mailReceived.Add("SwimMod_Mariner_Completed");
            Game1.player.currentLocation.resetForPlayerEntry();
            SwimMaps.AddScubaChest(Game1.player.currentLocation, new Vector2(10,6), "ScubaTank");
        }
    }
}