/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/WhoLivesHere
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using WhoLivesHereCore;
using WhoLivesHereCore.i18n;

namespace WhoLivesHere.GMCM
{
    internal static class GMCM_Integration
    {
        private static IModHelper helper;
        private static IManifest manifest;
        private static WhoLivesHereConfig config;
        private static void ResetGuiVars()
        {
            //
            //  reset the exposed config items to their
            //  default values.
            //
            config.ToggleKey = new KeybindList(new Keybind(SButton.LeftControl, SButton.N), new Keybind(SButton.RightControl, SButton.N));
            config.AutoOffTime = 0;
            config.AutoOnTime = 0;
            config.PageDelay = 250;
            config.ShowMissingHay = true;
            config.HideEmptyTabs = false;
        }
        public static void Initialize(IModHelper ohelper, IManifest omanifest, WhoLivesHereConfig oconfig)
        {
            helper = ohelper;
            manifest = omanifest;
            config = oconfig;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }
        private static void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: manifest,
                reset: ResetGuiVars,
                save: () => helper.WriteConfig(config),
                 titleScreenOnly: false
            );
            //
            //  create config GUI
            //
            configMenu.AddSectionTitle(
               mod: manifest,
               text: () => "Who Lives Here",
               tooltip: () => ""
           );
            configMenu.AddKeybindList(
              mod: manifest,
              name: () => I18n.ToggleKey(),
              tooltip: () => I18n.ToggleKey_TT(),
               getValue: () => config.ToggleKey,
               setValue: value => config.ToggleKey = value
           );
            configMenu.AddNumberOption(
              mod: manifest,
              name: () => I18n.PageDelay(),
              tooltip: () => I18n.PageDelay_TT(),
              getValue: () => config.PageDelay,
              setValue: value => config.PageDelay = value
          );
            configMenu.AddBoolOption(
             mod: manifest,
             name: () => I18n.HideEmpty(),
             tooltip: () => I18n.HideEmpty_TT(),
             getValue: () => config.HideEmptyTabs,
             setValue: value => config.HideEmptyTabs = value
         );
           configMenu.AddBoolOption(
               mod: manifest,
               name: () => I18n.ShowMissing(),
               tooltip: () => I18n.ShowMissing_TT(),
               getValue: () => config.ShowMissingHay,
               setValue: value => config.ShowMissingHay = value
       );
            configMenu.AddNumberOption(
             mod: manifest,
             name: () => I18n.AutoOn(),
             tooltip: () => I18n.AutoOn_TT(),
             getValue: () => config.AutoOnTime,
             setValue: value => config.AutoOnTime = value
         );
            configMenu.AddNumberOption(
             mod: manifest,
             name: () => I18n.AutoOff(),
             tooltip: () => I18n.AutoOff_TT(),
             getValue: () => config.AutoOffTime,
             setValue: value => config.AutoOffTime = value
         );
        }
    }
}
