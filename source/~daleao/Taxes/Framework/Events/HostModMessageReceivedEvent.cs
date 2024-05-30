/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Taxes.Framework.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="HostModMessageReceivedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class HostModMessageReceivedEvent(EventManager? manager = null)
    : ModMessageReceivedEvent(manager ?? TaxesMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && Context.IsMainPlayer;

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != UniqueId)
        {
            return;
        }

        var key = e.Type;
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        var value = e.ReadAs<int>();
        switch (key)
        {
            case DataKeys.BusinessExpenses:
                if (value <= 0)
                {
                    return;
                }

                Log.I(
                    $"A farmhand has expended {value}g. This amount will be added to {Game1.player.farmName} farm's business expenses.");
                Data.Increment(Game1.player, key, value);
                break;
        }
    }
}
