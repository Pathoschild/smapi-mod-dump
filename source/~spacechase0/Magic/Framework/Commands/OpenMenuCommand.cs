/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using Magic.Framework.Game.Interface;
using SpaceShared;
using SpaceShared.ConsoleCommands;
using StardewModdingAPI;
using StardewValley;

namespace Magic.Framework.Commands
{
    /// <summary>A command which causes the player to learn a spell.</summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = DiagnosticMessages.IsUsedViaReflection)]
    internal class OpenMenuCommand : ConsoleCommand
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public OpenMenuCommand()
            : base("magicmenu", "Opens the magic menu to choose or upgrade your spells.\n\nUsage:\n    magicmenu") { }

        /// <summary>Handle the command.</summary>
        /// <param name="monitor">Writes messages to the console and log file.</param>
        /// <param name="command">The command name.</param>
        /// <param name="args">The command arguments.</param>
        public override void Handle(IMonitor monitor, string command, string[] args)
        {
            Game1.activeClickableMenu = new MagicMenu();
        }
    }
}
