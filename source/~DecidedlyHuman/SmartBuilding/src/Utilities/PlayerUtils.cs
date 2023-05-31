/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using DecidedlyShared.Logging;
using StardewModdingAPI;
using StardewValley;

namespace SmartBuilding.Utilities
{
    public class PlayerUtils
    {
        private readonly Logger logger;

        public PlayerUtils(Logger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// </summary>
        /// <param name="item">The item to be refunded to the player's inventory.</param>
        /// <param name="reason">The reason for the refund. This could be an error, or simply the player cancelling the build.</param>
        /// <param name="logLevel">The <see cref="StardewModdingAPI.LogLevel" /> to log with.</param>
        /// <param name="shouldLog">
        ///     Whether or not to log. This is forced true by <see cref="StardewModdingAPI.LogLevel.Alert" />,
        ///     <see cref="StardewModdingAPI.LogLevel.Error" />, and <see cref="StardewModdingAPI.LogLevel.Warn" />.
        /// </param>
        public void RefundItem(Item item, string reason = "Something went wrong", LogLevel logLevel = LogLevel.Trace,
            bool shouldLog = false)
        {
            Game1.player.addItemByMenuIfNecessary(item.getOne());

            if (shouldLog || logLevel == LogLevel.Debug || logLevel >= LogLevel.Error)
                this.logger.Log(
                    $"{reason} {I18n.SmartBuilding_Error_Refunding_RefundingItemToPlayerInventory()} {item.Name}",
                    logLevel);
        }
    }
}
