/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.Player;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[RequiresMod("FlashShifter.StardewValleyExpandedCP")]
[AlwaysEnabledEvent]
internal sealed class GaldoraHudThemeWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="GaldoraHudThemeWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal GaldoraHudThemeWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation.GetType() == e.OldLocation.GetType())
        {
            return;
        }

        if (e.NewLocation.NameOrUniqueName.IsIn(
                "Custom_CastleVillageOutpost",
                "Custom_CrimsonBadlands",
                "Custom_IridiumQuarry",
                "Custom_TreasureCave"))
        {
            ModHelper.GameContent.InvalidateCache($"{Manifest.UniqueID}/UltimateMeter");
        }
    }
}
