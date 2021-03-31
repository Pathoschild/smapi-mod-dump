/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cantorsdust/StardewMods
**
*************************************************/

using StardewValley;

namespace TimeSpeed.Framework
{
    /// <summary>Displays messages to the user in-game.</summary>
    internal class Notifier
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Display a message for one second.</summary>
        /// <param name="message">The message to display.</param>
        public void QuickNotify(string message)
        {
            Game1.addHUDMessage(new HUDMessage(message, HUDMessage.newQuest_type) { timeLeft = 1000 });
        }

        /// <summary>Display a message for two seconds.</summary>
        /// <param name="message">The message to display.</param>
        public void ShortNotify(string message)
        {
            Game1.addHUDMessage(new HUDMessage(message, HUDMessage.newQuest_type) { timeLeft = 2000 });
        }
    }
}
