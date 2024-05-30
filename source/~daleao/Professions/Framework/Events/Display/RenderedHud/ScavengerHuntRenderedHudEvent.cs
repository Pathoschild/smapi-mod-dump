/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Events.Display.RenderedHud;

#region using directives

using DaLion.Professions.Framework.TreasureHunts;
using DaLion.Professions.Framework.UI;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ScavengerHuntRenderedHudEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ScavengerHuntRenderedHudEvent(EventManager? manager = null)
    : RenderedHudEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        State.ScavengerHunt ??= new ScavengerHunt();
        if (!State.ScavengerHunt.TreasureTile.HasValue)
        {
            return;
        }

        var treasureTile = State.ScavengerHunt.TreasureTile.Value;

        // track target
        HudPointer.Instance.DrawAsTrackingPointer(treasureTile, Color.Violet);

        // reveal if close enough
        if (Game1.player.SquaredTileDistance(treasureTile) <=
            Config.ScavengerDetectionDistance * Config.ScavengerDetectionDistance)
        {
            HudPointer.Instance.DrawOverTile(treasureTile, Color.Violet);
        }
    }
}
