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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace WaterYourCrops
{
    
    public partial class ModEntry : Mod
    {
        public static IMonitor SMonitor;
        public static IModHelper SHelper;
        public static ModConfig Config;
        public static ModConfig extConfig;
        public static ModEntry context;
        public static Texture2D waterTexture;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            extConfig = Helper.ReadConfig<ModConfig>();
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

        private void SaveConfig()
        {
            Helper.WriteConfig(Config);
            Log("GMCM Config saved.");
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Log("Launching with Debug mode enabled.", debugOnly: true);

            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            var configMenuExt = Helper.ModRegistry.GetApi<IGMCMOptionsAPI>("jltaylor-us.GMCMOptions");
            if (configMenu is null || configMenuExt is null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => {
                    Config = new ModConfig();
                    extConfig = new ModConfig();
                },
                save: SaveConfig
            );

            configMenu.OnFieldChanged(
                mod: ModManifest,
                onChange: (string str, object obj) =>
                {
                    switch (str)
                    {
                        case "color":
                            extConfig.IndicatorColor = (Color) obj;
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

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.OnlyWaterCan(),
                tooltip: () => I18n.OnlyWaterCanTip(),
                getValue: () => Config.Debug,
                setValue: value => Config.Debug = value
            );
            if (configMenuExt is not null)
            {
                configMenuExt.AddColorOption(
                    mod: ModManifest,
                    name: () => I18n.IndicatorColor(),
                    tooltip: () => I18n.IndicatorColorTip(),
                    getValue: () => Config.IndicatorColor,
                    setValue: value => Config.IndicatorColor = value,
                    showAlpha: false,
                    colorPickerStyle: (uint)(IGMCMOptionsAPI.ColorPickerStyle.AllStyles | IGMCMOptionsAPI.ColorPickerStyle.RadioChooser),
                    fieldId: "color"
                );
            }
            else
            {
                Config.IndicatorColor = Color.LightPink;
                Log("GMCM Options was not loaded. Defaulting color to pink. Please install or verify that GMCM Options is installed to change the indicator color.", LogLevel.Alert);
            }

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.IndicatorOpacity,
                setValue: value => Config.IndicatorOpacity = value,
                name: () => I18n.IndicatorOpacity(),
                tooltip: () => I18n.IndicatorOpacityTip(),
                min: 0f,
                max: 1f,
                interval: 0.1f
                );

            configMenuExt.AddHorizontalSeparator(
                mod: ModManifest,
                getColor: () => Utility.GetPrismaticColor(speedMultiplier: 2)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Debug(),
                tooltip: () => I18n.DebugTip(),
                getValue: () => Config.Debug,
                setValue: value => Config.Debug = value
            );

            waterTexture = Helper.ModContent.Load<Texture2D>("assets/waterTexture.png");
            if (waterTexture != null) Log("Successfully loaded texture.", debugOnly: true);
            else Log("Couldn't load indicator texture.", LogLevel.Error);
        }

    }
}
