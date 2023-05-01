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
        ModConfig Config = new();

        public int SetWakeUpTime = 0;
        public int PermSetWakeUpTime = 0;
        public int AlarmClockReminder = 0;
        public bool AlarmClockSet = false;
        public bool PermAlarmClockSet = false;
        public bool AlreadyAsked = false;
        public bool ClockTurnedOff = false;
        public bool LetterObtained = false;

        public const string SunSettingDown = "The sun is slowly setting down on your farm.^^There is still light outside, but you can set your alarm clock now, if you don't intend to sleep in.^^Select the time to wake up:";
        public const string SunSetDown = "The sun has set down on your farm.^^There is still time be an early bird and get a good long sleep as well. But you can always just sleep in.^^Select the time to wake up:";
        public const string GettingVeryLate = "It's getting quite late. You should set your alarm clock if you want to be an early bird. Or just sleep in and get your beauty nap.^^Select the time to wake up:";
        public const string ManualSetup = "When would you like to wake up tomorrow?.^^Select the time to wake up:";
        public string AlarmClockMessage = "";

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            //helper.Events.Input.ButtonPressed += this.DebugButtons;          // Just for debug

            helper.Events.GameLoop.DayStarted += this.DayStarted;
            helper.Events.GameLoop.DayEnding += this.DayEnding;
            helper.Events.GameLoop.TimeChanged += this.AlarmClockSelection;
            helper.Events.Input.ButtonPressed += this.ManualDialog;
            helper.Events.Input.ButtonPressed += this.TurnOffSwitch;
            helper.Events.Content.AssetRequested += this.TutorialMail;
            helper.Events.Input.ButtonsChanged += this.StaticTimeSetter;
            helper.Events.GameLoop.OneSecondUpdateTicked += this.PermTimeSetter;
        }

        private void StaticTimeSetter(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (this.Config.StaticTimeToggle.JustPressed())
            {
                if (PermAlarmClockSet == false)
                {
                    Game1.addHUDMessage(new HUDMessage("Alarm clock set permanently to: " + PermSetWakeUpTime / 100 + ":00. Pop-ups disabled.", 2));
                    ClockTurnedOff = true;
                    AlarmClockSet = true;
                    PermAlarmClockSet = true;
                }
                else
                {
                    Game1.addHUDMessage(new HUDMessage("Alarm clock reset to 6:00. Pop-ups enabled.", 2));
                    ClockTurnedOff = false;
                    AlarmClockSet = false;
                    PermAlarmClockSet = false;
                    SetWakeUpTime = 0600;
                }
            }
        }

        private void PermTimeSetter(object sender, OneSecondUpdateTickedEventArgs e)
        {
            PermSetWakeUpTime = SetWakeUpTime;
        }

        private void TutorialMail(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/mail"))
                e.Edit(this.EditImpl);
        }

        public void EditImpl(IAssetData asset)
        {
            var data = asset.AsDictionary<string, string>().Data;
            data["JSIWizardMail2"] = "Hello, @!^^To set the alarm clock manually at any time press V (or Xbox button).^^To disable or enable the RP prompts press ~ (or Left Stick).^^If you have selected the wake up time and want to keep it going forward, press LCtrl and ~. In this mode the other options are disabled.^^Here's a little something to get you going after that well-deserved long nap.^^Have fun! %item object 201 1 %%[#]Just Sleep In";
        }

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            GameLocation location = new();
            if (SetWakeUpTime == 0)
            {
                Game1.timeOfDay = 0600;
            }
            else
            {
                Game1.timeOfDay = SetWakeUpTime;
                if (PermAlarmClockSet == true)
                {
                    SetWakeUpTime = PermSetWakeUpTime;
                }
                else
                    SetWakeUpTime = 0;
            }
            if (ClockTurnedOff == true)
            {
                AlarmClockSet = true;
            }
            else
            {
                AlarmClockSet = false;
            }
            location.playSound("rooster");
        }

        private void ManualDialog(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (this.Helper.Input.IsDown(SButton.V) || this.Helper.Input.IsDown(SButton.BigButton))
            {
                if (PermAlarmClockSet == true)
                {
                    Game1.addHUDMessage(new HUDMessage("Fixed wake-up time mode enabled. Please disable it before making changes.", 3));
                    return;
                }
                AlarmClockSelectionManual();
            }
        }

        private void TurnOffSwitch(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (this.Helper.Input.IsDown(SButton.OemTilde) || this.Helper.Input.IsDown(SButton.LeftStick))
            {
                if (this.Helper.Input.IsDown(SButton.LeftControl))
                    return;
                if (PermAlarmClockSet == true)
                {
                    Game1.addHUDMessage(new HUDMessage("Fixed wake-up time mode enabled. Please disable it before making changes.", 3));
                    return;
                }
                if (ClockTurnedOff == false)
                {
                    Game1.addHUDMessage(new HUDMessage("Just Sleep In pop-ups disabled.", 3));
                    ClockTurnedOff = true;
                    AlarmClockSet = true;
                }
                else
                {
                    Game1.addHUDMessage(new HUDMessage("Just Sleep In pop-ups enabled.", 4));
                    ClockTurnedOff = false;
                    AlarmClockSet = false;
                }
            }
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
            if (Game1.timeOfDay == AlarmClockReminder - 200)
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
            /*else
            {
                AlarmClockMessage = ManualSetup;
            }*/

            if (Game1.timeOfDay >= (AlarmClockReminder - 200) && AlarmClockSet == false && Game1.player.currentLocation == Game1.getLocationFromName("FarmHouse") && Game1.timeOfDay < 2200 && AlreadyAsked == false)
                AlarmClockDialogSetupEarly();

            else if (Game1.timeOfDay == 2200 && AlarmClockSet == false)
                AlarmClockDialogSetupLate();
        }

        private void AlarmClockSelectionManual()
        {
            AlarmClockMessage = ManualSetup;
            AlarmClockDialogSetupEarly();
        }


        private void AlarmClockDialogSetupEarly()
        {
            List<Response> choices = new()
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
            List<Response> choices = new()
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

        private void DayEnding(object sender, DayEndingEventArgs e)
        {
            Game1.player.mailReceived.Remove("JSIWizardMail1");
            Game1.addMailForTomorrow("JSIWizardMail2");
            LetterObtained = true;
        }

        private void DebugButtons(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (this.Helper.Input.IsDown(SButton.B))
            {
                Game1.player.mailReceived.Remove("JSIWizardMail1");

            }
            if (this.Helper.Input.IsDown(SButton.H))
            {
                Game1.timeOfDay += 10;
            }
        }
    }
}