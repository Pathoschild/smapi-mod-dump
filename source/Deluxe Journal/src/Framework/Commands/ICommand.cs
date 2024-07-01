/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewModdingAPI;

namespace DeluxeJournal.Framework.Commands
{
    internal interface ICommand
    {
        /// <summary>Command name.</summary>
        string Name { get; }

        /// <summary>The human-readable documentation shown when the player runs the built-in 'help' command.</summary>
        string Documentation { get; }

        /// <summary>Try to handle the command.</summary>
        /// <param name="monitor">Writes messages to the console.</param>
        /// <param name="command">Command name.</param>
        /// <param name="args">Arguments submitted by the user.</param>
        void TryHandle(IMonitor monitor, string command, string[] args);
    }
}
