/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/EventLimiter
**
*************************************************/

using System;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewModdingAPI.Events;
using StardewValley;
using HarmonyLib;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace EventLimiter
{
    /// <summary>The API which lets other mods add a config UI through Generic Mod Config Menu.</summary>
    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);

        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
    }

    // Data model for CP integration
    class InternalExceptionModel
    {
        public List<int> EventLimiterExceptions;
    }

    public class ModEntry
        : Mod
    {
        private ModConfig config;
        public List<int> InternalExceptions = new List<int>();

        // Counters for event tracking
        public static readonly PerScreen<int> EventCounterDay = new PerScreen<int>();
        public static readonly PerScreen<int> EventCounterRow = new PerScreen<int>();

        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);

            // Try and read config, use default values if unable
            try
            {
                this.config = helper.ReadConfig<ModConfig>();
            }
            catch (Exception ex)
            {
                this.config = new ModConfig();
                this.Monitor.Log("Error reading config, using default values...", LogLevel.Warn);
                this.Monitor.Log($"An error occured reading the config. Details:\n{ex}");
            }

            // Add harmony patches
            Patches.Hook(harmony, this.Monitor, this.config, this.InternalExceptions);

            // Allow EventLimiterapi access to needed values
            EventLimiterApi.Hook(this.config, this.InternalExceptions);           

            foreach (IModInfo mod in this.Helper.ModRegistry.GetAll())
            {
                // Check if it's a Content Patcher pack
                if (mod.IsContentPack == false || mod.Manifest.ContentPackFor?.UniqueID.Trim().Equals("Pathoschild.ContentPatcher", StringComparison.InvariantCultureIgnoreCase) == false)
                {
                    continue;
                }                    

                // Use reflection on IModInfo to get non-public property
                string directoryPath = (string)mod.GetType().GetProperty("DirectoryPath")?.GetValue(mod);

                if (directoryPath == null)
                {
                    throw new InvalidOperationException($"Couldn't get DirectoryPath property from the mod info for {mod.Manifest.Name}.");
                }


                // read JSON file into data model
                IContentPack contentPack = this.Helper.ContentPacks.CreateFake(directoryPath);
                InternalExceptionModel model = contentPack.ReadJsonFile<InternalExceptionModel>("content.json");

                // Get event IDs from model and add to internal exceptions
                if (model?.EventLimiterExceptions != null)
                {
                    foreach (int eventid in model.EventLimiterExceptions)
                    {
                        this.InternalExceptions.Add(eventid);
                        this.Monitor.Log($"Content pack {mod.Manifest.Name} added event {eventid} as event limit exception");
                    }
                }               
            }

            // Add event handlers
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.Events.GameLoop.DayStarted += this.DayStarted;
            helper.Events.Input.ButtonPressed += this.ButtonPressed;
        }

        // Get EventLimiterApi
        public override object GetApi()
        {
            return new EventLimiterApi();
        }

        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // Reset events in a row counter if needed, button press used as event because warped event won't consider some events for resetting counters
            if (EventCounterRow.Value > 0 && Game1.CurrentEvent == null && Context.CanPlayerMove == true)
            {
                EventCounterRow.Value = 0;
                this.Monitor.Log("Resetting events in a row counter");
            }
        }

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            // Reset counters each day
            EventCounterDay.Value = 0;
            EventCounterRow.Value = 0;
            this.Monitor.Log("Resetting event counters");
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                return;
            }

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.config)
            );

            // Add EventsPerDay option
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                getValue: () => this.config.EventsPerDay,
                setValue: value => this.config.EventsPerDay = value,
                min: 0,
                tooltip: () => "The maximum number of events shown in a day",
                name: () => "Events per day");

            // Add EventsInARow option
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                getValue: () => this.config.EventsInARow,
                setValue: value => this.config.EventsInARow = value,
                min: 0,
                tooltip: () => "The maximum number of events shown when entering a new location",
                name: () => "Events in a row");

            // Add ExemptEventsCountTowardsLimit option
            configMenu.AddBoolOption(
               mod: this.ModManifest,
               name: () => "Exceptions count towards limit",
               tooltip: () => "Event exceptions will count towards event limits",
               getValue: () => this.config.ExemptEventsCountTowardsLimit,
               setValue: value => this.config.ExemptEventsCountTowardsLimit = value);

            // Add Exceptions option
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Exceptions",
                tooltip: () => "Event ids which will never be skipped. Enter only numbers separated by commas",
                getValue: () => string.Join(", ", this.config.Exceptions), 
                setValue: value => this.config.Exceptions = GetExceptionsFromString(value));

        }

        private int[] GetExceptionsFromString(string value)
        {
            var formattedstring = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();

            var ints = from field in formattedstring.Where((x) => { int y; return Int32.TryParse(x, out y); })
                       select Int32.Parse(field);

            return ints.ToArray();
        }
    }
}