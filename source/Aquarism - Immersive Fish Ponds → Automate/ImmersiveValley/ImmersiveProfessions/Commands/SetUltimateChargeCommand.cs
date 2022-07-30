/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Commands;

#region using directives

using Common;
using Common.Commands;
using JetBrains.Annotations;

#endregion using directives

[UsedImplicitly]
internal sealed class SetUltimateChargeCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SetUltimateChargeCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "ready_ult";

    /// <inheritdoc />
    public override string Documentation => "Max-out the player's Special Ability charge, or set it to the specified percentage.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (ModEntry.PlayerState.RegisteredUltimate is null)
        {
            Log.W("Not registered to an Ultimate.");
            return;
        }

        if (args.Length <= 0)
        {
            ModEntry.PlayerState.RegisteredUltimate.ChargeValue = ModEntry.PlayerState.RegisteredUltimate.MaxValue;
            return;
        }

        if (args.Length > 1)
        {
            Log.W("Too many arguments. Specify a single value between 0 and 100.");
            return;
        }

        if (!int.TryParse(args[0], out var value) || value is < 0 or > 100)
        {
            Log.W("Bad arguments. Specify an integer value between 0 and 100.");
            return;
        }

        ModEntry.PlayerState.RegisteredUltimate.ChargeValue =
            (double)value * ModEntry.PlayerState.RegisteredUltimate.MaxValue / 100.0;
    }
}