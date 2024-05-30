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

namespace MyMod
{
    /// <summary>
    /// 
    /// !!!IMPORTANT!!!
    /// BEFORE USING THIS TEMPLATE:
    /// Change "MyMod" to your mod's name/namespace in the following locations:
    ///     (1) ModEntry.cs namespace
    ///     (2) IGenericModConfigMenuApi.cs namespace
    ///     (3) ModConfig.cs namespace
    ///     (4) Manifest.json
    ///         - Name
    ///         - Description (optional)
    ///         - UniqueID
    ///         - EntryDll
    ///         - UpdateKeys (semi-optional. update this before uploading to Nexus)
    ///         - Repository (optional)
    ///         - ModFolder
    ///     (5) README.md
    ///         - Line 1 ("MyMod by chiccen")
    ///         - Line 4 -- insert a brief description of the mod
    ///         - Line 5 -- add Github folder to end of link (optional)
    ///         - Line 6 -- replace "{{MyMod}}" in Nexus link to your mod code
    ///         - Line 8 -- "Translating MyMod"
    ///         - Line 15
    ///         - Line 24 -- right side header of translation chart
    ///         
    /// It is also recommended you update "Description" and Nexus update key in manifest.json
    /// You can delete this entire comment summary once these tasks are completed.
    /// 
    /// </summary>


    public partial class ModEntry : Mod
    {
        public static IMonitor SMonitor;
        public static IModHelper SHelper;
        public static ModConfig Config;
        public static ModEntry context;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            I18n.Init(helper.Translation);
            context = this;
            SMonitor = Monitor;
            SHelper = helper;

            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;

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

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Log("Launching with Debug mode enabled.", debugOnly: true);

            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.TestBool(),
                tooltip: () => I18n.TestBool_1(),
                getValue: () => Config.ExampleBool,
                setValue: value => Config.ExampleBool = value
                );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.TestInt(),
                tooltip: () => I18n.TestInt_1(),
                getValue: () => Config.ExampleInt,
                setValue: value => Config.ExampleInt = value
                );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Debug(),
                tooltip: () => I18n.Debug_1(),
                getValue: () => Config.Debug,
                setValue: value => Config.Debug = value
            );
        }

    }
}
