/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/andyruwruw/stardew-valley-water-bot
**
*************************************************/

using StardewModdingAPI;

namespace BotFramework
{
    /// <summary>
	/// Static method of printing to <see cref="IMonitor"/>.
	/// </summary>
    public class Logger
    {
        /// <summary>
		/// Static reference to <see cref="IMonitor"/>.
		/// </summary>
		private static IMonitor Monitor = null;

        /// <summary>
        /// Sets static reference to <see cref="IMonitor"/>.
        /// </summary>
        /// <param name="monitor"><see cref="IMonitor"/> reference.</param>
        public static void SetMonitor(IMonitor monitor)
        {
            if (Logger.Monitor == null)
            {
                Logger.Monitor = monitor;
            }
        }

        /// <summary>
        /// Prints to a given <see cref="LogLevel"/> of <see cref="IMonitor"/>.
        /// </summary>
        /// <param name="message">Message to be printed.</param>
        /// <param name="level"><see cref="LogLevel"/> to print to.</param>
        public static void Log(
            String message,
            LogLevel level = LogLevel.Debug
        )
        {
            Logger.Monitor.Log(
                message,
                level
            );
        }

        /// <summary>
        /// Prints to <see cref="LogLevel.Debug"/> of <see cref="IMonitor"/>.
        /// </summary>
        /// <param name="message">Message to be printed.</param>
        public static void Debug(String message)
        {
            Logger.Log(
                message,
                LogLevel.Debug
            );
        }

        /// <summary>
        /// Prints to <see cref="LogLevel.Info"/> of <see cref="IMonitor"/>.
        /// </summary>
        /// <param name="message">Message to be printed.</param>
        public static void Info(String message)
        {
            Logger.Log(
                message,
                LogLevel.Info
            );
        }

        /// <summary>
        /// Prints to <see cref="LogLevel.Trace"/> of <see cref="IMonitor"/>.
        /// </summary>
        /// <param name="message">Message to be printed.</param>
        public static void Trace(String message)
        {
            Logger.Log(
                message,
                LogLevel.Trace
            );
        }
    }
}
