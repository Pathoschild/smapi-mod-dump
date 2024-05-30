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

/// <summary>Initializes a new instance of the <see cref="TaxDayStartedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class TaxDayStartedEvent(EventManager? manager = null)
    : DayStartedEvent(manager ?? TaxesMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        var player = Game1.player;
        var toDebit = Data.ReadAs<int>(player, DataKeys.LatestAmountWithheld);
        if (toDebit <= 0)
        {
            return;
        }

        player.Money -= toDebit;
        Game1.addHUDMessage(
            new HUDMessage(
                I18n.Hud_Debt_Debit(toDebit.ToString()),
                HUDMessage.newQuest_type) { timeLeft = HUDMessage.defaultTime * 2 });
        Data.Write(player, DataKeys.LatestAmountWithheld, string.Empty);
        this.Disable();
    }
}
