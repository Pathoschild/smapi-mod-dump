/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;

namespace DecidedlyShared.Logging
{
    public class Logger
    {
        private IMonitor monitor;
        private ITranslationHelper translationHelper;
        //private HashSet<> messageQueue;
        
        public Logger(IMonitor monitor, ITranslationHelper translationHelper)
        {
            this.monitor = monitor;
            this.translationHelper = translationHelper;
        }

        public void Log(string logMessage, LogLevel logLevel = LogLevel.Info, bool shouldAlwaysDisplayInHud = false)
        {
            monitor.Log(logMessage, logLevel);

            // If it's a high priority LogLevel or it's marked as should be displayed, we display it on the screen.
            if (logLevel >= LogLevel.Warn || shouldAlwaysDisplayInHud)
            {
                HUDMessage message = new HUDMessage(logMessage, 2);

                if (!Game1.doesHUDMessageExist(logMessage))
                    Game1.addHUDMessage(message);
            }
        }

        public void Exception(Exception e)
        {
            monitor.Log($"Exception: {e.Message}", LogLevel.Error);
            monitor.Log($"Full exception data: \n{e.Data}", LogLevel.Error);
        }
    }
}