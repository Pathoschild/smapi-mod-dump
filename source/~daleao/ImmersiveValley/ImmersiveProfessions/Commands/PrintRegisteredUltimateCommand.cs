/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Commands;

#region using directives

using Common;
using Common.Commands;
using Common.Extensions;
using JetBrains.Annotations;

#endregion using directives

[UsedImplicitly]
internal sealed class PrintRegisteredUltimateCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal PrintRegisteredUltimateCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "which_ult";

    /// <inheritdoc />
    public override string Documentation => "Print the player's currently registered Special Ability, if any.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (ModEntry.PlayerState.RegisteredUltimate is null)
        {
            Log.I("Not registered to an Ultimate.");
            return;
        }

        var key = ModEntry.PlayerState.RegisteredUltimate.Index.ToString().SplitCamelCase()[0].ToLowerInvariant();
        var professionDisplayName = ModEntry.i18n.Get(key + ".name.male");
        var ultiName = ModEntry.i18n.Get(key + ".ulti");
        Log.I($"Registered to {professionDisplayName}'s {ultiName}.");
    }
}