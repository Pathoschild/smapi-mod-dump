/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using System.Reflection;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace SeasonalSaveSlots
{
    public class ModEntry : Mod
    {
        public static ModConfig Config { get; private set; } = null!;

        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);

            // Activate the Harmony patch
            Harmony harmonyInstance = new(helper.ModRegistry.ModID);
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

            Config = helper.ReadConfig<ModConfig>();

            // Initialize the mod for GMCM
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
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
                name: () => I18n.Config_ShowSeasonIcon_Title(),
                tooltip: () => I18n.Config_ShowSeasonIcon_Tooltip(),
                getValue: () => Config.ShowSeasonIcon,
                setValue: value => Config.ShowSeasonIcon = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config_ShowSeasonColoredLine_Title(),
                tooltip: () => I18n.Config_ShowSeasonColoredLine_Tooltip(),
                getValue: () => Config.ShowSeasonColoredLine,
                setValue: value => Config.ShowSeasonColoredLine = value
            );
        }
    }
}
