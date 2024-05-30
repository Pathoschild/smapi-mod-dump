/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Events.World.ObjectListChanged;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ProspectorHuntObjectListChangedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ProspectorHuntObjectListChangedEvent(EventManager? manager = null)
    : ObjectListChangedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnObjectListChangedImpl(object? sender, ObjectListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation)
        {
            return;
        }

        var hunt = State.ProspectorHunt!;
        if (!hunt.TreasureTile.HasValue)
        {
            this.Disable();
            return;
        }

        if (!e.Location.Objects.ContainsKey(hunt.TreasureTile.Value))
        {
            hunt.Complete();
            this.Disable();
            return;
        }

        if (Config.UseLegacyProspectorHunt)
        {
            return;
        }

        var removed = e.Removed.Where(r => r.Value.IsStone()).ToList();
        if (removed.Count != 1)
        {
            return;
        }

        var distanceToTreasure = (int)removed.Single().Value.SquaredTileDistance(hunt.TreasureTile.Value);
        var detectionDistance = (int)(Config.ProspectorDetectionDistance * Config.ProspectorDetectionDistance);
        if (detectionDistance > 0 && !distanceToTreasure.IsIn(1..detectionDistance))
        {
            return;
        }

        var pitch = (int)(2400f * (1f - ((float)distanceToTreasure / detectionDistance)));
        Game1.playSound("detector", pitch);
        Log.A($"Beeped at frequency {pitch} Hz");
    }
}
