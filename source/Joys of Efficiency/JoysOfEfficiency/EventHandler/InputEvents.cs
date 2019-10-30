using JoysOfEfficiency.Automation;
using JoysOfEfficiency.Core;
using JoysOfEfficiency.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace JoysOfEfficiency.EventHandler
{
    internal class InputEvents
    {
        private static Config Conf => InstanceHolder.Config;
        public void OnButtonPressed(object sender, ButtonPressedEventArgs args)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }
            if (!Context.IsPlayerFree || Game1.activeClickableMenu != null)
            {
                return;
            }
            if (args.Button == Conf.ButtonShowMenu)
            {
                //Open Up Menu
                JoeMenu.OpenMenu();
            }
            else if (args.Button == Conf.ButtonToggleBlackList)
            {
                HarvestAutomation.ToggleBlacklistUnderCursor();
            }
            else if (args.Button == Conf.ButtonToggleFlowerColorUnification)
            {
                FlowerColorUnifier.ToggleFlowerColorUnification();
            }
        }
    }
}
