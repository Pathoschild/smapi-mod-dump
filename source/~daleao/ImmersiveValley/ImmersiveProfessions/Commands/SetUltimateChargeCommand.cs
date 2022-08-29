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
using Framework.VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class SetUltimateChargeCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SetUltimateChargeCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "ready_ult", "rdy" };

    /// <inheritdoc />
    public override string Documentation => "Max-out the player's Special Ability charge, or set it to the specified percentage.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var ultimate = Game1.player.get_Ultimate();
        if (ultimate is null)
        {
            Log.W("Not registered to an Ultimate.");
            return;
        }

        switch (args.Length)
        {
            case <= 0:
                ultimate.ChargeValue = ultimate.MaxValue;
                return;
            case > 1:
                Log.W("Too many arguments. Specify a single value between 0 and 100.");
                return;
        }

        if (!int.TryParse(args[0], out var value) || value is < 0 or > 100)
        {
            Log.W("Bad arguments. Specify an integer value between 0 and 100.");
            return;
        }

        ultimate.ChargeValue = (double)value * ultimate.MaxValue / 100d;
    }
}