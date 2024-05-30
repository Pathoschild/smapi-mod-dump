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
using StardewValley.Delegates;

namespace WildTreeTweaks
{

    /// <summary>
    /// Ideas:
    /// DONE? More liberal placement
    /// Configurable tap rates/days
    /// Configurable nightly grow chance
    /// Luckier loot drop tables
    /// </summary>

    public partial class ModEntry : Mod
    {
        public static IMonitor SMonitor;
        public static IModHelper SHelper;
        public static ModConfig Config;
        public static ModConfig extConfig;
        public static ModEntry context;
        public static bool updateTrees;
        public static List<GameLocation> updateLocations = new();

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            I18n.Init(helper.Translation);
            context = this;
            SMonitor = Monitor;
            SHelper = helper;

            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.GameLoop.ReturnedToTitle += GameLoop_ReturnedToTile;
            helper.Events.Player.Warped += Player_Warped;

            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll();

            helper.ConsoleCommands.Add("fix_stumps", "DEBUG ONLY. Reset all trees from stumps to not stumps. Does not accept any arguments.", this.FixStumps);
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

        private void SaveConfig()
        {
            Helper.WriteConfig(Config);
            Log("Saved GMCM config.");
            updateLocations.Clear();
            updateTrees = true;
        }

        private void GameLoop_ReturnedToTile(object sender, ReturnedToTitleEventArgs e)
        {
            leaves.Clear();
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Log("Launching with Debug mode enabled.", debugOnly: true);

            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () =>
                {
                    Config = new ModConfig();
                },
                save: SaveConfig
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.EnableMod(),
                getValue: () => Config.EnableMod,
                setValue: value => Config.EnableMod = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.OnlyOnFarm(),
                tooltip: () => I18n.OnlyOnFarm_1(),
                getValue: () => Config.OnlyOnFarm,
                setValue: value => Config.OnlyOnFarm = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.GrowInWinter(),
                tooltip: () => I18n.GrowInWinter_1(),
                getValue: () => Config.GrowInWinter,
                setValue: value => Config.GrowInWinter = value,
                fieldId: "GrowInWinter"
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.GrowNearTrees(),
                tooltip: () => I18n.GrowNearTrees_1(),
                getValue: () => Config.GrowNearTrees,
                setValue: value => Config.GrowNearTrees = value,
                fieldId: "GrowNearTrees"
            );
            /* not working atm
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.MossFromMature(),
                tooltip: () => I18n.MossfromMature_1(),
                getValue: () => Config.MossFromMature,
                setValue: value => Config.MossFromMature = value,
                fieldId: "MossFromMature"
            );*/

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.Health(),
                tooltip: () => I18n.Health_1(),
                getValue: () => Config.Health,
                setValue: value => Config.Health = value,
                min: 1f,
                fieldId: "Health"
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.WoodMulti(),
                tooltip: () => I18n.WoodMulti_1(),
                getValue: () => Config.WoodMultiplier,
                setValue: value => Config.WoodMultiplier = value,
                min: 0f
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.BookChanceBool(),
                tooltip: () => I18n.BookChanceBool_1(),
                getValue: () => Config.BookChanceBool,
                setValue: value => Config.BookChanceBool = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.BookChance(),
                tooltip: () => I18n.BookChance_1(),
                getValue: () => Config.BookChance,
                setValue: value => Config.BookChance = value,
                min: 0f,
                max: 1f,
                interval: 0.0005f
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.MysterChance(),
                tooltip: () => I18n.MysteryChance_1(),
                getValue: () => Config.MysteryBoxChance,
                setValue: value => Config.MysteryBoxChance = value,
                min: 0f,
                max: 1f,
                interval: 0.005f
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.GrowthChance(),
                tooltip: () => I18n.GrowthChance_1(),
                getValue: () => Config.GrowthChance,
                setValue: value => Config.GrowthChance = value,
                min: 0f,
                max: 1f,
                interval: 0.05f,
                fieldId: "GrowthChance"
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.SeedChance(),
                tooltip: () => I18n.SeedChance_1(),
                getValue: () => Config.SeedChance,
                setValue: value => Config.SeedChance = value,
                min: 0f,
                max: 1f,
                interval: 0.01f,
                fieldId: "SeedChance"
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.SeedSpread(),
                tooltip: () => I18n.SeedSpread_1(),
                getValue: () => Config.SeedSpreadChance,
                setValue: value => Config.SeedSpreadChance = value,
                min: 0f,
                max: 1f,
                interval: 0.05f,
                fieldId: "SeedSpreadChance"
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Debug(),
                tooltip: () => I18n.Debug_1(),
                getValue: () => Config.Debug,
                setValue: value => Config.Debug = value
            );

            Log("Loaded Wild Tree Tweaks config.", debugOnly: true);
        }

    }
}