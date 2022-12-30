/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop;

#region using directives

using System.Globalization;
using System.Linq;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ConservationismDayEndingEvent : DayEndingEvent
{
    /// <summary>Initializes a new instance of the <see cref="ConservationismDayEndingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ConservationismDayEndingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Conservationist);

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        var player = Game1.player;
        if (!TaxesModule.IsEnabled)
        {
            var taxBonus = player.Read<float>(DataFields.ConservationistActiveTaxBonusPct);
            if (taxBonus > 0f)
            {
                var amountSold = Game1.getFarm().getShippingBin(player).Sum(item =>
                    item is SObject obj ? obj.sellToStorePrice() * obj.Stack : item.salePrice() / 2);
                if (amountSold >= 0)
                {
                    player.Money += (int)(amountSold * taxBonus);
                }
            }
        }

        if (Game1.dayOfMonth != 28)
        {
            return;
        }

        var trashCollectedThisSeason = player.Read<uint>(DataFields.ConservationistTrashCollectedThisSeason);
        player.Write(DataFields.ConservationistTrashCollectedThisSeason, "0");
        if (trashCollectedThisSeason <= 0)
        {
            player.Write(DataFields.ConservationistActiveTaxBonusPct, "0");
            return;
        }

        var taxBonusForNextSeason =
            // ReSharper disable once PossibleLossOfFraction
            Math.Min(
                trashCollectedThisSeason / ProfessionsModule.Config.TrashNeededPerTaxBonusPct / 100f,
                ProfessionsModule.Config.ConservationistTaxBonusCeiling);
        player.Write(
            DataFields.ConservationistActiveTaxBonusPct,
            taxBonusForNextSeason.ToString(CultureInfo.InvariantCulture));
        if (taxBonusForNextSeason <= 0 || TaxesModule.IsEnabled)
        {
            return;
        }

        ModHelper.GameContent.InvalidateCacheAndLocalized("Data/mail");
        player.mailForTomorrow.Add($"{Manifest.UniqueID}/ConservationistTaxNotice");
    }
}
