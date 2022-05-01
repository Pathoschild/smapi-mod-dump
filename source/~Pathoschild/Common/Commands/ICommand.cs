/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.Common.Commands
{
    /// <summary>A console command implemented by Content Patcher.</summary>
    internal interface ICommand
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The command's sub-name (e.g. the 'export' in 'patch export').</summary>
        string Name { get; }

        /// <summary>The command description for the 'patch help' command.</summary>
        string Description { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Handle a command invocation.</summary>
        /// <param name="args">The command arguments, excluding the name and sub-command.</param>
        void Handle(string[] args);
    }
}
