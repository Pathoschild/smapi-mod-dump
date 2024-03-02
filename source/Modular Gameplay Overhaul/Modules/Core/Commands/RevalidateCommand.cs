/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Core.Commands;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Collections;

#endregion using directives

[UsedImplicitly]
internal sealed class RevalidateCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="RevalidateCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal RevalidateCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "revalidate" };

    /// <inheritdoc />
    public override string Documentation => "Force a full revalidation of all modules.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        EnumerateModules().ForEach(module => module.Revalidate());
        Log.I("Revalidation completed.");
    }
}
