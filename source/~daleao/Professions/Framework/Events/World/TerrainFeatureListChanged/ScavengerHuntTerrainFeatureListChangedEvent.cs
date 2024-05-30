/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Events.World.TerrainFeatureListChanged;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ScavengerHuntTerrainFeatureListChangedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ScavengerHuntTerrainFeatureListChangedEvent(EventManager? manager = null)
    : TerrainFeatureListChangedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnTerrainFeatureListChangedImpl(object? sender, TerrainFeatureListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation)
        {
            return;
        }

        var hunt = State.ScavengerHunt!;
        if (!hunt.TreasureTile.HasValue)
        {
            this.Disable();
            return;
        }

        if (!e.Location.terrainFeatures.TryGetValue(hunt.TreasureTile.Value, out var feature) ||
            feature is not HoeDirt)
        {
            return;
        }

        hunt.Complete();
        this.Disable();
    }
}
