using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;

namespace ModSettingsTab
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            Framework.Helper.Init(helper,Monitor);

            Helper.Events.Display.MenuChanged += MenuChanged;
            
            ModData.Api.GetMod("GilarF.ModSettingsTab").OptionsChanged += (o, eventArgs) =>
                ModData.Config = Helper.ReadConfig<TabConfig>();
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
            menu.pages[GameMenu.optionsTab] = new Menu.GameOptionsPage(
                menu.xPositionOnScreen,
                menu.yPositionOnScreen,
                menu.width + ModData.Offset,
                menu.height);
        }
    }
}