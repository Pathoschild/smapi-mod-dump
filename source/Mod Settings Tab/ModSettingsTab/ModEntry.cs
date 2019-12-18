using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ModSettingsTab
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ModEntry : Mod
    {
        /// <summary>
        /// SMAPI helper
        /// </summary>
        public new static IModHelper Helper;

        /// <summary>
        /// SMAPI monitor
        /// see: https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Logging
        /// </summary>
        public static IMonitor Console;

        /// <summary>
        /// SMAPI internationalization helper
        /// </summary>
        public static ITranslationHelper I18N;

        /// <summary>
        /// mod entry point
        /// </summary>
        /// <param name="helper"></param>
        public override void Entry(IModHelper helper)
        {
            Helper = helper;
            Console = Monitor;
            I18N = helper.Translation;

            Helper.Events.Display.MenuChanged += MenuChanged;
            
            ModData.Api.GetMod("GilarF.ModSettingsTab").OptionsChanged += (o, eventArgs) =>
                ModData.Config = Helper.ReadConfig<TabConfig>();
            
            Helper.Events.GameLoop.GameLaunched += (sender, args) => ModData.Init();
            Helper.Events.GameLoop.GameLaunched += (sender, args) =>
                LocalizedContentManager.OnLanguageChange += code => ModData.Init();
            
        }

        public override object GetApi()
        {
            return ModData.Api;
        }

        /// <summary>
        /// handler of the change event of the active menu
        /// replaces the settings tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!(e.NewMenu is GameMenu menu)) return;
            menu.pages[GameMenu.optionsTab] = new Menu.OptionsPage(
                menu.xPositionOnScreen,
                menu.yPositionOnScreen,
                menu.width + ModData.Offset,
                menu.height);
        }
    }
}