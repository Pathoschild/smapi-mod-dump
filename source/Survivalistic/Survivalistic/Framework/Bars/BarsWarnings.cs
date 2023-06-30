/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Survivalistic
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

namespace Survivalistic.Framework.Bars
{
    public class BarsWarnings
    {
        private static bool active_hunger_warning = false;
        private static bool active_thirst_warning = false;

        public static void VerifyStatus()
        {
            if (!Context.IsWorldReady) return;

            if (ModEntry.data.actual_hunger <= 15)
            {
                if (!active_hunger_warning)
                {
                    active_hunger_warning = true;
                    Game1.addHUDMessage(new HUDMessage(ModEntry.instance.Helper.Translation.Get("hunger-warning"), 2));
                }
            }
            else active_hunger_warning = false;

            if (ModEntry.data.actual_thirst <= 15)
            {
                if (!active_thirst_warning)
                {
                    active_thirst_warning = true;
                    Game1.addHUDMessage(new HUDMessage(ModEntry.instance.Helper.Translation.Get("thirsty-warning"), 2));
                }
            }
            else active_thirst_warning = false;
        }
    }
}
