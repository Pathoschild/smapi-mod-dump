/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tesla1889tv/ControlValleyMod
**
*************************************************/

using StardewValley;

namespace ControlValley
{
    public class UI
    {
        public static void ShowError(string msg)
        {
            Game1.addHUDMessage(new HUDMessage(msg, HUDMessage.error_type));
        }

        public static void ShowInfo(string msg)
        {
            Game1.addHUDMessage(new HUDMessage(msg, HUDMessage.newQuest_type));
        }
    }
}
