using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarDustCore
{
    namespace Messages
    {
        public static class HudWithIcon
        {

            public static void showStarMessage(string message)
            {
                Game1.addHUDMessage(new HUDMessage(message, 1));
                if (!message.Contains("Inventory"))
                {
                    Game1.playSound("cancel");
                    return;
                }
                if (!Game1.player.mailReceived.Contains("BackpackTip"))
                {
                    Game1.player.mailReceived.Add("BackpackTip");
                    Game1.addMailForTomorrow("pierreBackpack", false, false);
                }
            }
            /*
            public static void showMessage2(string message)
            {
                Game1.addHUDMessage(new HUDMessage(message, 2));
                if (!message.Contains("Inventory"))
                {
                    Game1.playSound("cancel");
                    return;
                }
                if (!Game1.player.mailReceived.Contains("BackpackTip"))
                {
                    Game1.player.mailReceived.Add("BackpackTip");
                    Game1.addMailForTomorrow("pierreBackpack", false, false);
                }
            }
            */

            public static void showRedMessage(string message)
            {
                Game1.addHUDMessage(new HUDMessage(message, 3));
                if (!message.Contains("Inventory"))
                {
                    Game1.playSound("cancel");
                    return;
                }
                if (!Game1.player.mailReceived.Contains("BackpackTip"))
                {
                    Game1.player.mailReceived.Add("BackpackTip");
                    Game1.addMailForTomorrow("pierreBackpack", false, false);
                }
            }
        }
    }
}
