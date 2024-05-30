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

namespace FruitTreeTweaks
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {

        public static IMonitor SMonitor;
        public static IModHelper SHelper;
        public static ModConfig Config;
        public static ModEntry context;

        private static int attempts = 0;


        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            I18n.Init(helper.Translation);

            context = this;

            SMonitor = Monitor;
            SHelper = helper;

            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;

            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll();

        }

        /// <summary>
		///     Small method that handles Debug mode to make SMAPI logs a bit easier to read.
		/// </summary>
        /// <remarks>
        ///     Allows basic Log functions to upgrade Logs to <see cref="LogLevel.Debug"/>, excluding <see cref="LogLevel.Error"/>, when debugging for ease of reading.<br/>
        ///     For <b>Debug Only</b> Logs -- use <c>debugOnly: true</c> and omit <see cref="LogLevel"/> -- <code>Log(message, debugOnly: true);</code><br/>
        ///     For Debug Logs that <b>always</b> show -- use <see cref="LogLevel"/> and omit <c>debugOnly</c> -- <code>Log(message, <see cref="LogLevel"/>)</code>.
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
        ///     For <b>Debug Only</b> Logs -- use <c>debugOnly: true</c> and omit <see cref="LogLevel"/> -- <code>LogOnce(message, debugOnly: true);</code><br/>
        ///     For Debug Logs that <b>always</b> show -- use <see cref="LogLevel"/> and omit <c>debugOnly</c> -- <code>LogOnce(message, <see cref="LogLevel"/>)</code>.
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

            Log("Fruit Tree Tweaks launching with Debug enabled.", debugOnly: true);

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.EnableMod(),
                getValue: () => Config.EnableMod,
                setValue: value => Config.EnableMod = value
            );
            Log($"Mod enabled: {Config.EnableMod}", debugOnly: true);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.CropsBlock(),
                tooltip: () => I18n.CropsBlock_1(),
                getValue: () => Config.CropsBlock,
                setValue: value => Config.CropsBlock = value
            );
            Log($"Crops block: {Config.CropsBlock}", debugOnly: true);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.TreesBlock(),
                tooltip: () => I18n.TreesBlock_1(),
                getValue: () => Config.TreesBlock,
                setValue: value => Config.TreesBlock = value
            );
            Log($"Trees block: {Config.TreesBlock}", debugOnly: true);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.ObjectsBlock(),
                tooltip: () => I18n.ObjectsBlock_1(),
                getValue: () => Config.ObjectsBlock,
                setValue: value => Config.ObjectsBlock = value
            );
            Log($"Objects block: {Config.ObjectsBlock}", debugOnly: true);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.PlantAnywhere(),
                tooltip: () => I18n.PlantAnywhere_1(),
                getValue: () => Config.PlantAnywhere,
                setValue: value => Config.PlantAnywhere = value
            );
            Log($"Plant Anywhere: {Config.PlantAnywhere}", debugOnly: true);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.FruitAllSeasons(),
                tooltip: () => I18n.FruitAllSeasons_1(),
                getValue: () => Config.FruitAllSeasons,
                setValue: value => Config.FruitAllSeasons = value
            );
            Log($"Fruit All Seasons: {Config.FruitAllSeasons}", debugOnly: true);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.FruitInWinter(),
                tooltip: () => I18n.FruitInWinter_1(),
                getValue: () => Config.FruitInWinter,
                setValue: value => Config.FruitInWinter = value
            );
            Log($"Fruit In Winter: {Config.FruitInWinter}", debugOnly: true);

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.MaxFruitTree(),
                getValue: () => Config.MaxFruitPerTree,
                setValue: value => Config.MaxFruitPerTree = value
            );
            Log($"Max Fruit / Tree: {Config.MaxFruitPerTree}", debugOnly: true);

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.DaysUntilMature(),
                getValue: () => Config.DaysUntilMature,
                setValue: value => Config.DaysUntilMature = value
            );
            Log($"Days to Mature: {Config.DaysUntilMature}", debugOnly: true);
            /* future feature? maybe?
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.SaplingMaturity(),
                tooltip: () => I18n.SaplingMaturity_1(),
                getValue: () => Config.UseBaseSaplingMaturity,
                setValue: value => Config.UseBaseSaplingMaturity = value
            );
            Log($"Use Base Sapling Maturity: {Config.UseBaseSaplingMaturity}", debugOnly: true);*/

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.MinFruitDay(),
                tooltip: () => I18n.MinFruitDay_1(),
                getValue: () => Config.MinFruitPerDay,
                setValue: value => Config.MinFruitPerDay = value
            );
            Log($"Min Fruit / Day: {Config.MinFruitPerDay}", debugOnly: true);

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.MaxFruitDay(),
                tooltip: () => I18n.MaxFruitDay_1(),
                getValue: () => Config.MaxFruitPerDay,
                setValue: value => Config.MaxFruitPerDay = value
            );
            Log($"Max Fruit / Day: {Config.MaxFruitPerDay}", debugOnly: true);

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.DaysUntilSilver(),
                tooltip: () => I18n.DaysUntilTip(),
                getValue: () => Config.DaysUntilSilverFruit,
                setValue: value => Config.DaysUntilSilverFruit = value
            );
            Log($"Days until Silver: {Config.DaysUntilSilverFruit}", debugOnly: true);

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.DaysUntilGold(),
                tooltip: () => I18n.DaysUntilTip(),
                getValue: () => Config.DaysUntilGoldFruit,
                setValue: value => Config.DaysUntilGoldFruit = value
            );
            Log($"Days until Gold: {Config.DaysUntilGoldFruit}", debugOnly: true);

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.DaysUntilIridium(),
                tooltip: () => I18n.DaysUntilTip(),
                getValue: () => Config.DaysUntilIridiumFruit,
                setValue: value => Config.DaysUntilIridiumFruit = value
            );
            Log($"Days until Iridium: {Config.DaysUntilIridiumFruit}", debugOnly: true);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Debug(),
                tooltip: () => I18n.Debug_1(),
                getValue: () => Config.Debug,
                setValue: value => Config.Debug = value
            );
            Log($"Debug: Well you're reading this, aren't you?", debugOnly: true); // xaxaxa
            /* future feature
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.GodMode(),
                tooltip: () => I18n.GodMode_1(),
                getValue: () => Config.GodMode,
                setValue: value => Config.GodMode = value
            );
            Log($"God Mode: {Config.GodMode}", debugOnly: true);
            */

            configMenu.SetTitleScreenOnlyForNextOptions(
                mod: ModManifest,
                titleScreenOnly: true
                );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => I18n.Section_Title(),
                tooltip: () => I18n.Section_Tooltip()
                );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.ColorVar(),
                tooltip: () => I18n.ColorVar_1(),
                getValue: () => Config.ColorVariation,
                setValue: value => Config.ColorVariation = value
            );
            Log($"Color Variation: {Config.ColorVariation}", debugOnly: true);

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.SizeVar(),
                tooltip: () => I18n.SizeVar_1(),
                getValue: () => Config.SizeVariation,
                setValue: value => Config.SizeVariation = value,
                min: 0,
                max: 99
            );
            Log($"Size Variation: {Config.SizeVariation}", debugOnly: true);

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.BufferX(),
                tooltip: () => I18n.BufferX_1(),
                getValue: () => Config.FruitSpawnBufferX,
                setValue: value => Config.FruitSpawnBufferX = value
            );
            Log($"Fruit Buffer X: {Config.FruitSpawnBufferX}", debugOnly: true);

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.BufferY(),
                tooltip: () => I18n.BufferY_1(),
                getValue: () => Config.FruitSpawnBufferY,
                setValue: value => Config.FruitSpawnBufferY = value
            );
            Log($"Fruit Buffer Y: {Config.FruitSpawnBufferY}", debugOnly: true);
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            fruitToday = GetFruitPerDay(); // this breaks if it is anywhere else so dont move it
        }
    }
}