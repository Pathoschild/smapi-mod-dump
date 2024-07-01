/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="FarmhandScavengerHuntUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class FarmhandScavengerHuntUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (!e.IsMultipleOf(15))
        {
            return;
        }

        var hunt = State.ScavengerHunt!;
        if (!hunt.TreasureTile.HasValue)
        {
            this.Disable();
            return;
        }

        if (!Game1.player.currentLocation.terrainFeatures.TryGetValue(hunt.TreasureTile.Value, out var feature) ||
            feature is not HoeDirt)
        {
            return;
        }

        hunt.Complete();
        this.Disable();
    }
}
