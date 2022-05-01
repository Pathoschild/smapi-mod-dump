/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/9Rifleman/JustSleepIn
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
        public int SetWakeUpTime = 0;
        public int AlarmClockReminder = 0;
        public bool AlarmClockSet = false;
        public bool AlreadyAsked = false;
        public const string SunSettingDown = "The sun is slowly setting down on your farm.^^There is still light outside, but you can set your alarm clock now, if you don't intend to sleep in.^^Select the time to wake up:";
        public const string SunSetDown = "The sun has set down on your farm.^^There is still time be an early bird and get a good long sleep as well. But you can always just sleep in.^^Select the time to wake up:";
        public const string GettingVeryLate = "It's getting quite late. You should set your alarm clock if you want to be an early bird. Or just sleep in and get your beauty nap.^^Select the time to wake up:";
        public string AlarmClockMessage = "";

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.TimeChanged += this.DebugTimeSkipper;          // Just for testing
            helper.Events.GameLoop.DayStarted += this.DayStarted;
            helper.Events.GameLoop.TimeChanged += this.AlarmClockSelection;
        }

        public void DialogueSet(Farmer who, string dialogue_id)
        {

            switch (dialogue_id)
            {
                case "dialogue_id1":
                    SetWakeUpTime = 0600;
                    AlarmClockSet = true;
                    break;
                case "dialogue_id2":
                    SetWakeUpTime = 0700;
                    AlarmClockSet = true;
                    break;
                case "dialogue_id3":
                    SetWakeUpTime = 0800;
                    AlarmClockSet = true;
                    break;
                case "dialogue_id4":
                    SetWakeUpTime = 0900;
                    AlarmClockSet = true;
                    break;
                case "dialogue_id5":
                    SetWakeUpTime = 1000;
                    AlarmClockSet = true;
                    break;
                case "dialogue_id6":
                    SetWakeUpTime = 1100;
                    AlarmClockSet = true;
                    break;
                case "dialogue_id7":
                    SetWakeUpTime = 1200;
                    AlarmClockSet = true;
                    break;
                case "dialogue_id8":
                    AlreadyAsked = true;
                    break;
                case null:
                    break;
            }
        }


        private void AlarmClockSelection(object sender, TimeChangedEventArgs e)
        {
            AlarmClockReminder = Game1.getTrulyDarkTime();
            if (Game1.timeOfDay == AlarmClockReminder-200)
            {
                AlarmClockMessage = SunSettingDown;
            }
            else if (Game1.timeOfDay == AlarmClockReminder)
            {
                AlreadyAsked = false;
                AlarmClockMessage = SunSetDown;
            }
            else if (Game1.timeOfDay == 2200)
            {
                AlreadyAsked = false;
                AlarmClockMessage = GettingVeryLate;
            }

            if (Game1.timeOfDay >= (AlarmClockReminder-200) && AlarmClockSet == false && Game1.player.currentLocation == Game1.getLocationFromName("FarmHouse") && Game1.timeOfDay < 2200 && AlreadyAsked == false)
                AlarmClockDialogSetupEarly();                     

            else if (Game1.timeOfDay == 2200 && AlarmClockSet == false)
                AlarmClockDialogSetupLate();
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
            AlarmClockSet = false;
        }

        private void AlarmClockDialogSetupEarly()
        {
            List<Response> choices = new List<Response>()
                {
                new Response("dialogue_id1", "6:00AM"),
                new Response("dialogue_id2", "7:00AM"),
                new Response("dialogue_id3", "8:00AM"),
                new Response("dialogue_id4", "9:00AM"),
                new Response("dialogue_id5", "10:00AM"),
                new Response("dialogue_id6", "11:00AM"),
                new Response("dialogue_id7", "12:00PM"),
                new Response("dialogue_id8", "I'll select later.")
                };

            Game1.currentLocation.createQuestionDialogue(AlarmClockMessage, choices.ToArray(), new GameLocation.afterQuestionBehavior(DialogueSet));
        }

        private void AlarmClockDialogSetupLate()
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

            Game1.currentLocation.createQuestionDialogue(AlarmClockMessage, choices.ToArray(), new GameLocation.afterQuestionBehavior(DialogueSet));
        }

        private void DebugTimeSkipper(object sender, TimeChangedEventArgs e)
        {
            if (Game1.timeOfDay == 0610)
                Game1.timeOfDay = 1730;
        }
    }
}