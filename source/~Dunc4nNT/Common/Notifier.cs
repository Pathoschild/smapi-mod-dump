/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dunc4nNT/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

namespace NeverToxic.StardewMods.Common
{
    public class Notifier
    {
        public static void DisplayHudNotification(string message, float duration = 2500, LogLevel logLevel = LogLevel.Info)
        {
            if (logLevel is LogLevel.Warn || logLevel is LogLevel.Alert)
                Game1.addHUDMessage(new HUDMessage(message, HUDMessage.newQuest_type) { timeLeft = duration });
            else if (logLevel is LogLevel.Error)
                Game1.addHUDMessage(new HUDMessage(message, HUDMessage.error_type) { timeLeft = duration });
            else
                Game1.addHUDMessage(new HUDMessage(message, HUDMessage.newQuest_type) { timeLeft = duration, noIcon = true });
        }
    }
}
