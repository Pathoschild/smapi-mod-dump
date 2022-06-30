/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Common.Commands;

/// <summary>Interface for a console command for a mod.</summary>
internal interface IConsoleCommand
{
    /// <summary>The statement that triggers this command.</summary>
    string Trigger { get; }

    /// <summary>The human-readable documentation shown when the player runs the 'help' command.</summary>
    string Documentation { get; }

    /// <summary>The action that will be executed.</summary>
    void Callback(string[] args);
}