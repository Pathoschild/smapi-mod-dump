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

/// <summary>Base implementation of a console command for a mod.</summary>
internal abstract class ConsoleCommand : IConsoleCommand
{
    protected readonly string _entry;

    /// <summary>Construct an instance.</summary>
    /// <param name="entry">The entry command for this mod.</param>
    protected ConsoleCommand(string entry)
    {
        _entry = entry;
    }

    /// <inheritdoc />
    public abstract string Trigger { get; }

    /// <inheritdoc />
    public abstract string Documentation { get; }

    /// <inheritdoc />
    public abstract void Callback(string[] args);
}