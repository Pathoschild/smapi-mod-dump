/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GlimmerDev/StardewValleyMod_SleepWarning
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using GenericModConfigMenu;
using System;
using static System.Collections.Specialized.BitVector32;

namespace SleepWarning
{
    /// <summary>The mod entry point.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        private SleepWarningConfig Config;

        /// <summary> Pretty sound names used by Generic Mod Config Menu. </summary>
        private readonly Dictionary<string, string> SoundNames = new()
            {
                { "crystal", "Ding (Default)" },
                { "cameraNoise", "Camera Click" },
                { "cat", "Cat" },
                { "cluck", "Chicken" },
                { "cow", "Cow" },
                { "dog_bark", "Dog" },
                { "goat", "Goat" },
                { "parrot", "Parrot" },
                { "pig", "Pig" },
                { "select", "Select (UI)" },
                { "whistle", "Whistle" }
            };

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<SleepWarningConfig>();
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
        }


        /*********
        ** Private methods
        *********/
        /// <summary> Plays a sound after the specified delay.</summary>
        /// <param name="sound">The sound to play.</param>
        /// <param name="delay">The delay in milliseconds.</param>
        private static void PlayDelayedSound(string sound, int delay = 0, int pitch = -1)
        {
            DelayedAction.playSoundAfterDelay(sound, delay, null, null, pitch, true);
        }

        /// <summary> Plays the sleep warning sound and repeats it the specified amount of times. </summary>
        /// <param name="repeat">The number of times to repeat.</param>
        private void PlayWarningSound(int repeat)
        {
            // play ding at low pitch, other sounds at normal pitch
            int pitch = (this.Config.WarningSound == "crystal") ? 1: -1;
            for (int i = 0; i < repeat; i++)
                PlayDelayedSound(this.Config.WarningSound, 500 * i, pitch);
        }

        /// <inheritdoc cref="IGameLoopEvents.TimeChanged"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimeChanged(object? sender, TimeChangedEventArgs e) 
        {
            if (Game1.timeOfDay < this.Config.FirstWarnTime)
                return;

            if (this.Config.ThirdWarnTime > -1 && Game1.timeOfDay == this.Config.ThirdWarnTime)
            {
                PlayWarningSound(3);
            }
            else if (this.Config.SecondWarnTime > -1 && Game1.timeOfDay == this.Config.SecondWarnTime)
            {
                PlayWarningSound(2);
            }
            else if (this.Config.FirstWarnTime > -1 && Game1.timeOfDay == this.Config.FirstWarnTime)
            {
                PlayWarningSound(1);
            }
        }

        /*********
        ** GENERIC MOD CONFIG MENU INTEGRATION
        *********/

        /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // Add support for Generic Mod Config Menu, if installed
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new SleepWarningConfig(),
                save: () => this.Helper.WriteConfig(this.Config) 
                );
            configMenu.AddSectionTitle(
                mod: this.ModManifest, 
                text:() => "Time Options"
                );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "First Warn Time",
                getValue: () => ReverseConvertTime(this.Config.FirstWarnTime),
                setValue: value => this.Config.FirstWarnTime = ConvertTimeValue(value),
                min: -1,
                max: 40,
                formatValue: FormatTimeValue 
                );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Second Warn Time",
                getValue: () => ReverseConvertTime(this.Config.SecondWarnTime),
                setValue: value => this.Config.SecondWarnTime = ConvertTimeValue(value),
                min: -1,
                max: 40,
                formatValue: FormatTimeValue 
                );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Third Warn Time",
                getValue: () => ReverseConvertTime(this.Config.ThirdWarnTime),
                setValue: value => this.Config.ThirdWarnTime = ConvertTimeValue(value),
                min: -1,
                max: 40,
                formatValue: FormatTimeValue 
                );
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Audio Options"
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Warning Sound",
                getValue: () => this.Config.WarningSound,
                setValue: value => this.Config.WarningSound = value,
                allowedValues: new string[] { "cameraNoise", "cat", "cluck", "cow", "crystal", "Duck", "dog_bark", "goat",
                                                "parrot", "pig", "select", "whistle" },
                formatAllowedValue: FormatSoundValue,
                fieldId: "glimmerDev.SleepWarning.WarnSound"
                );
        }
        
        /// <summary> Formats the config menu's interval value into a readable time string.</summary>
        /// <param name="time">The input time value from the config menu.</param>
        /// <returns>A formatted string representing the time.</returns>
        private string FormatTimeValue(int time)
        {
            if (time == -1)
                return "Disable";
            string min_str = (time % 2 == 0) ? "00" : "30";
            int hours = (time / 2) + 6;
            string am_pm = (hours > 11 && hours < 24) ? " PM" : " AM";
            string hour_str = (hours % 12 == 0) ? "12" : (hours % 12).ToString();
            string result = hour_str + ":" + min_str + am_pm;
            return result;
        }

        /// <summary>Formats a Stardew Valley sound name into a more user friendly name.</summary>
        /// <param name="sound">The internal sound name.</param>
        /// <returns>A formatted string representing the friendly name.</returns>
        private string FormatSoundValue(string sound)
        {
            if (SoundNames.ContainsKey(sound))
                return SoundNames[sound];

            return sound;
        }
            
        /// <summary>Converts the config menu time value into the Stardew Valley format.</summary>
        /// <param name="time">The input time value from the config menu.</param>
        /// <returns>An integer time value in the Stardew Valley format.</returns>
        private static int ConvertTimeValue(int time)
        {
            if (time == -1)
                return -1;
            int minutes = (time % 2 == 0) ? 0 : 30;
            int hours = time / 2 * 100 + 600;
            return hours + minutes;
        }

        /// <summary>Converts a Stardew Valely time value to a config menu time value.</summary>
        /// <param name="time">The Stardew Valley time value.</param>
        /// <returns>The equivalent config menu time value.</returns>
        private static int ReverseConvertTime(int time)
        {
            if (time == -1)
                return -1;
            int hours = Math.Clamp(time / 100, 6, 26) - 6;
            int minutes = Math.Min(time/10 % 10, 5);

            return hours * 2 + (minutes >= 3 ? 1 : 0);
        }

    }
}
