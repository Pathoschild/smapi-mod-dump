/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Events.Player.Warped;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="GaldoraHudThemeWarpedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[ModRequirement("FlashShifter.StardewValleyExpandedCP", "Stardew Valley Expanded")]
[AlwaysEnabledEvent]
internal sealed class GaldoraHudThemeWarpedEvent(EventManager? manager = null)
    : WarpedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation.GetType() == e.OldLocation.GetType())
        {
            return;
        }

        if (e.NewLocation.NameOrUniqueName.IsAnyOf(
                "Custom_CastleVillageOutpost",
                "Custom_CrimsonBadlands",
                "Custom_IridiumQuarry",
                "Custom_TreasureCave"))
        {
            ModHelper.GameContent.InvalidateCache($"{UniqueId}/LimitGauge");
        }
    }
}
