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

/// <summary>Base implementation of a console command for a mod.</summary>
internal abstract class ConsoleCommand : IConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="ConsoleCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    protected ConsoleCommand(CommandHandler handler)
    {
        this.Handler = handler;
    }

    /// <inheritdoc />
    public abstract string[] Triggers { get; }

    /// <inheritdoc />
    public abstract string Documentation { get; }

    /// <summary>Gets the <see cref="CommandHandler"/> instance that handles this command.</summary>
    protected CommandHandler Handler { get; }

    /// <inheritdoc />
    public abstract void Callback(string trigger, string[] args);
}
