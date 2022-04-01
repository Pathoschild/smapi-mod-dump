/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/nymvaline/StardewValley-AnkiStudyBreak
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace AnkiStudyBreak
{
    /// <summary>The mod entry point.</summary>
    public class AnkiStudyBreak : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        private ModConfig Config; 

        private int numCardsStudiedToday = 0;
        private int targetCardsStudiedToday = 0;
        private int targetNumberDayStart;
        private int targetNumberInterval;
        private int timeIntervalForReviews;
        private bool dayStartReviews;
        private bool intervalReviews;
        private int lastReviewSessionTime = 600; // 600 = day start time, may need to configure?
        private string ankiConnectUrl;
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            targetNumberDayStart = Config.NumAnkiCardsForDayStart;
            targetNumberInterval = Config.NumAnkiCardsForIntervals;
            timeIntervalForReviews = Config.NumHoursForIntervals*100;
            intervalReviews = Config.AnkiBreakAtIntervals;
            dayStartReviews = Config.AnkiBreakOnDayStart;
            ankiConnectUrl = Config.AnkiConnectURL;

            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            helper.Events.GameLoop.TimeChanged += GameLoop_TimeChanged;
        }

        private void GameLoop_TimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (intervalReviews)
            {
                if (e.NewTime >= lastReviewSessionTime + timeIntervalForReviews)
                {
                    // also TODO: allow non-hour-long intervals. also TODO: handle timeskips.
                    lastReviewSessionTime = e.NewTime;
                    Task mytask = new Task(() => InternalSendHttpRequest(true, targetNumberInterval)); // TODO: what if there are no cards left?
                    this.Monitor.Log($"Anki Study Break: Card check! Sending first HTTP request here at time {e.NewTime}. First requests are sent at intervals of {timeIntervalForReviews}", LogLevel.Debug);
                    mytask.Start();
                }
            }
        }

        bool ContinueCardCheck = false;
        bool StartExitstop = false;
        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if(Context.CanPlayerMove && ContinueCardCheck)
            {
                CreateCardCheckDialogBox(targetCardsStudiedToday - numCardsStudiedToday);
                ContinueCardCheck = false;
            }
            if(StartExitstop)
            {
                CreateExitToMenuDialogBox();
                StartExitstop = false;
            }
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            lastReviewSessionTime = 600; // TODO need to set
            if (dayStartReviews)
            {
                Task mytask = new Task(() => InternalSendHttpRequest(true, targetNumberDayStart));
                this.Monitor.Log("Anki Study Break: Card check! Sending first HTTP request here.", LogLevel.Debug);
                mytask.Start();
            }
        }

        /*********
        ** Private methods
        *********/
        private void CreateCardCheckDialogBox(int remaining)
        {
            List<Response> choices = new List<Response>()
            {
                new Response("main_menu","Exit To Menu" ),
                new Response("check_cards", "Card Check") // TODO how to localize?
            };
            Game1.currentLocation.createQuestionDialogue($"Anki cards!? {remaining} remaining", choices.ToArray(), new GameLocation.afterQuestionBehavior(CardCheckDialogueSet));
        }
        // This is the callback from the question dialogue.
        public void CardCheckDialogueSet(Farmer who, string dialogue_id)
        {
            // Here you get which option was picked as dialogue_id.
            //Game1.addHUDMessage(new HUDMessage($"Farmer {who} chose option {dialogue_id}"));
            if (dialogue_id.Equals("main_menu"))
            {
                //CreateExitToMenuDialogBox();
                StartExitstop = true;
            }
            else
            {
                    Task mytask2 = new Task(() => InternalSendHttpRequest(false, 0));
                    mytask2.Start();
                    this.Monitor.Log("Anki Study Break: Card check! Sending HTTP request here.", LogLevel.Debug);
            }
        }
        private void CreateExitToMenuDialogBox()
        {
            List<Response> choices = new List<Response>()
            {
                new Response("confirm_main_menu", "I'm sure, exit to menu" ),
                new Response("return", "No, keep playing") // TODO how to localize?
            };
            Game1.currentLocation.createQuestionDialogue($"Are you sure you want to exit?", choices.ToArray(), new GameLocation.afterQuestionBehavior(ExitToMenuDialogueSet));
        }
        public void ExitToMenuDialogueSet(Farmer who, string dialogue_id)
        {
            // Here you get which option was picked as dialogue_id.
            //Game1.addHUDMessage(new HUDMessage($"Second: Farmer {who} chose option {dialogue_id}"));
            if (dialogue_id.Equals("confirm_main_menu"))
            {
                Game1.ExitToTitle();
            }
            else
            {
                ContinueCardCheck = true;
            }
        }
        private HttpClient client = new();
        public async Task InternalSendHttpRequest(bool isFirst, int targetNumber) // target number only is relevant if isFirst
        {
            var request = @"{
              ""action"": ""getNumCardsReviewedToday"",
              ""version"": 6
            }";

            var content = new StringContent(request, System.Text.Encoding.UTF8, "application/json");

            //Uri uri = new Uri("http://127.0.0.1:8765");
            Uri uri = new Uri(ankiConnectUrl);

            this.Monitor.Log($"Anki Study Break: sending json: {request} to {ankiConnectUrl}", LogLevel.Debug);
            var response = await client.PostAsync(uri, content);

            var responseString = await response.Content.ReadAsStringAsync();
            
            this.Monitor.Log($"Anki Study Break: received from AnkiConnect json: {responseString}.", LogLevel.Debug);

            AnkiConnectResponse returnValue = JsonSerializer.Deserialize<AnkiConnectResponse>(responseString);

            numCardsStudiedToday = returnValue.result;
            if(isFirst)
            {
                targetCardsStudiedToday = numCardsStudiedToday + targetNumber;
            }
            if (numCardsStudiedToday < targetCardsStudiedToday)
            {
                ContinueCardCheck=true;
            }
            this.Monitor.Log($"Anki Study Break: {returnValue.result} cards studied today. Current target: {targetCardsStudiedToday}", LogLevel.Debug);
        }
    }
}