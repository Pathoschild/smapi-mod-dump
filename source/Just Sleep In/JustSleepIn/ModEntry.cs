/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/9Rifleman/Just-Sleep-In
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.GameData;

namespace JustSleepIn
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private int SetWakeUpTime = 0;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.TimeChanged += this.AlarmClockSelection;
            helper.Events.GameLoop.DayStarted += this.DayStarted;
        }

        public void DialogueSet(Farmer who, string dialogue_id)
        {

            switch (dialogue_id)
            {
                case "dialogue_id1":
                    SetWakeUpTime = 0600;
                    break;
                case "dialogue_id2":
                    SetWakeUpTime = 0700;
                    break;
                case "dialogue_id3":
                    SetWakeUpTime = 0800;
                    break;
                case "dialogue_id4":
                    SetWakeUpTime = 0900;
                    break;
                case "dialogue_id5":
                    SetWakeUpTime = 1000;
                    break;
                case "dialogue_id6":
                    SetWakeUpTime = 1100;
                    break;
                case "dialogue_id7":
                    SetWakeUpTime = 1200;
                    break;
                case null:
                    break;
            }
        }


        private void AlarmClockSelection(object sender, TimeChangedEventArgs e)
        {
            if (Game1.timeOfDay == 0610)
            {
                List<Response> choices = new List<Response>()
                {
                new Response("dialogue_id1", "6:00AM"),
                new Response("dialogue_id2", "7:00AM"),
                new Response("dialogue_id3", "8:00AM"),
                new Response("dialogue_id4", "9:00AM"),
                new Response("dialogue_id5", "10:00AM"),
                new Response("dialogue_id6", "11:00AM"),
                new Response("dialogue_id7", "12:00PM")
                };

                // And here we case it to pop up on the farmer's screen. When the farmer has picked a choice, it sends that information to the method below (DialogueSet
                Game1.currentLocation.createQuestionDialogue($"The sun is slowly setting down on your farm...^^If you're staying up late, would you like to sleep in?^^Select the time to wake up:", choices.ToArray(), new GameLocation.afterQuestionBehavior(DialogueSet));
            }
        }

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            GameLocation location = new();
            location.playSound("rooster");
            if (SetWakeUpTime == 0)
            {
                Game1.timeOfDay = 0600;
            }
            else
            {
                Game1.timeOfDay = SetWakeUpTime;
                SetWakeUpTime = 0;
            }

        }
    }
}