/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

namespace SafeLightning.CommandParsing.Commands
{
    /// <summary>Prints the player's location.</summary>
    internal class PrintLocationCommand : BaseCommand
    {
        /*********
        ** Public methods
        *********/

        /// <summary>Construct an instance.</summary>
        /// <param name="monitor">The monitor used for command output.</param>
        public PrintLocationCommand(IMonitor monitor) : base(monitor, "print_location", "pl", "Prints the player's location.")
        {
        }

        /// <summary>Invoke the command.</summary>
        /// <param name="args">The command arguments</param>
        public override void Invoke(string[] args)
        {
            this.monitor.Log($"Okay, player is at {Game1.player.getTileLocation()}.", LogLevel.Info);
        }
    }
}
