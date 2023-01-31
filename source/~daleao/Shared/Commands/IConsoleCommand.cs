/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Commands;

/// <summary>Interface for a console command for a mod.</summary>
internal interface IConsoleCommand
{
    /// <summary>Gets the statement that triggers this command.</summary>
    string[] Triggers { get; }

    /// <summary>Gets the human-readable documentation shown when the player runs the 'help' command.</summary>
    string Documentation { get; }

    /// <summary>The action that will be executed.</summary>
    /// <param name="trigger">The trigger word.</param>
    /// <param name="args">The command arguments.</param>
    void Callback(string trigger, string[] args);
}
