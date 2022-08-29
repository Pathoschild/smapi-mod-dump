/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using Common.Events;
using Common.Extensions;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class GaldoraHudThemeWarpedEvent : WarpedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal GaldoraHudThemeWarpedEvent(ProfessionEventManager manager)
        : base(manager)
    {
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("FlashShifter.StardewValleyExpandedCP"))
            AlwaysEnabled = true;
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation.GetType() == e.OldLocation.GetType()) return;

        if (e.NewLocation.NameOrUniqueName.IsIn("Custom_CastleVillageOutpost", "Custom_CrimsonBadlands",
                "Custom_IridiumQuarry", "Custom_TreasureCave"))
            ModEntry.ModHelper.GameContent.InvalidateCache($"{ModEntry.Manifest.UniqueID}/UltimateMeter");
    }
}