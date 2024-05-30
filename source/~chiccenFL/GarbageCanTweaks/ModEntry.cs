/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chiccenFL/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Reflection.Metadata.Ecma335;

namespace GarbageCanTweaks
{

    public partial class ModEntry : Mod
    {
        public static IMonitor SMonitor;
        public static IModHelper SHelper;
        public static ModConfig Config;
        public static ModEntry context;
        public static string dataFile;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            I18n.Init(helper.Translation);
            context = this;
            SMonitor = Monitor;
            SHelper = helper;

            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;

            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll();
        }

        /// <summary>
		///     Small method that handles Debug mode to make SMAPI logs a bit easier to read.
		/// </summary>
        /// <remarks>
        ///     Allows basic Log functions to upgrade Logs to <see cref="LogLevel.Debug"/>, excluding <see cref="LogLevel.Error"/>, when debugging for ease of reading.<br/>
        ///     For <b>Debug Only</b> Logs -- use <c>debugOnly: true</c> and omit <see cref="LogLevel"/> <code>Log(message, debugOnly: true);</code><br/>
        ///     For Debug Logs that <b>always</b> show -- use <see cref="LogLevel"/> and omit <c>debugOnly</c> <code>Log(message, <see cref="LogLevel"/>);</code>.
        /// </remarks>
		/// <param name="message"></param>
		/// <param name="level"></param>

        public static void Log(string message, LogLevel level = LogLevel.Trace, bool debugOnly = false)
        {
            level = Config.Debug && level != LogLevel.Error ? LogLevel.Debug : level;
            if (!debugOnly) SMonitor.Log(message, level);
            else if (debugOnly && Config.Debug) SMonitor.Log(message, level);
            else return;
        }

        /// <summary>
		///     Small method that handles Debug mode to make SMAPI logs a bit easier to read.
		/// </summary>
        /// <remarks>
        ///     Allows basic Log functions to upgrade Logs to <see cref="LogLevel.Debug"/>, excluding <see cref="LogLevel.Error"/>, when debugging for ease of reading.<br/>
        ///     For <b>Debug Only</b> Logs -- use <c>debugOnly: true</c> and omit <see cref="LogLevel"/> <code>LogOnce(message, debugOnly: true);</code><br/>
        ///     For Debug Logs that <b>always</b> show -- use <see cref="LogLevel"/> and omit <c>debugOnly</c> <code>LogOnce(message, <see cref="LogLevel"/>);</code>
        /// </remarks>
		/// <param name="message"></param>
		/// <param name="level"></param>
		public static void LogOnce(string message, LogLevel level = LogLevel.Trace, bool debugOnly = false)
        {
            level = Config.Debug && level != LogLevel.Error ? LogLevel.Debug : level;
            if (!debugOnly) SMonitor.LogOnce(message, level);
            if (debugOnly && Config.Debug) SMonitor.LogOnce(message, level);
            else return;
        }

        private static string getCan(string name)
        {
            
            if (noGift.Contains(name) || Game1.getCharacterFromName(name) is null)
            {
                Log($"check for {name} failed, or was a non-gifting NPC", debugOnly: true);
                return "";
            }
            Log($"Finding can for {name}", debugOnly: true);
            
            if (name.Equals("Jodi") || name.Equals("Kent") || name.Equals("Sam") || name.Equals("Vincent"))
                return "JodiAndKent";
            if (name.Equals("Evelyn") || name.Equals("George") || name.Equals("Alex"))
                return "Evelyn";
            if (name.Equals("Haley") || name.Equals("Emily"))
                return "EmilyAndHaley";
            if (name.Equals("Gus"))
                return "Saloon";
            if (name.Equals("Clint"))
                return "Blacksmith";
            else return "Mayor";
            
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Log("Launching with Debug mode enabled.", debugOnly: true);

            Load();

            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.OnFieldChanged(
                mod: ModManifest,
                onChange: (str, obj) =>
                {
                    switch (str)
                    {
                        case "table":
                            dataFile = (string)obj;
                            dataFile = $"assets/{dataFile}.json";
                            Log($"Garbage data change to {dataFile}", debugOnly: true);
                            break;
                        default: break;
                    }
                }
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.EnableMod(),
                getValue: () => Config.EnableMod,
                setValue: value => Config.EnableMod = value
            );

            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => I18n.Table(),
                tooltip: () => I18n.Table_1(),
                getValue: () => Config.LootTable,
                setValue: value => Config.LootTable = value,
                allowedValues: packs.ToArray(),
                fieldId: "table"
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.EnableBirthday(),
                tooltip: () => I18n.EnableBirthday_1(),
                getValue: () => Config.EnableBirthday,
                setValue: value => Config.EnableBirthday = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.BirthdayChance(),
                tooltip: () => I18n.BirthdayChance_1(),
                getValue: () => Config.BirthdayChance,
                setValue: value => Config.BirthdayChance = value,
                min: 0f,
                max: 1f,
                interval: 0.05f
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.LootChance(),
                tooltip: () => I18n.LootChance_1(),
                getValue: () => Config.LootChance,
                setValue: value => Config.LootChance = value,
                min: 0f
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Debug(),
                tooltip: () => I18n.Debug_1(),
                getValue: () => Config.Debug,
                setValue: value => Config.Debug = value
            );

            dataFile = $"assets/{Config.LootTable}.json";

            Log("loaded GMCM options. mod is ready!");
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            Reload();
            birthday = Utility.getTodaysBirthdayNPC();
            bCan = (birthday is not null) ? getCan(birthday.Name) : "";
            string bday = (birthday is not null) ? $"Today is {birthday.Name}'s birthday!" : "No birthday today.";
            Log(bday, debugOnly: true);
            Log("loaded NPC birthday cans");
        }

    }
}
