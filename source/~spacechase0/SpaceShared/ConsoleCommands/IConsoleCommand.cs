/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace SpaceShared.ConsoleCommands
{
    /// <summary>A console command to register.</summary>
    internal interface IConsoleCommand
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The command name the user must type.</summary>
        string Name { get; }

        /// <summary>The command description.</summary>
        string Description { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Handle the command.</summary>
        /// <param name="monitor">Writes messages to the console and log file.</param>
        /// <param name="command">The command name.</param>
        /// <param name="args">The command arguments.</param>
        void Handle(IMonitor monitor, string command, string[] args);
    }
}
