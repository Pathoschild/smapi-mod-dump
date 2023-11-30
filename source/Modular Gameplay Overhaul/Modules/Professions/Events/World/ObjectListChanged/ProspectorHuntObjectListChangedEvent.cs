/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.World.ObjectListChanged;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Professions.TreasureHunts;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ProspectorHuntObjectListChangedEvent : ObjectListChangedEvent
{
    private ProspectorHunt? _hunt;

    /// <summary>Initializes a new instance of the <see cref="ProspectorHuntObjectListChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProspectorHuntObjectListChangedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        this._hunt ??= Game1.player.Get_ProspectorHunt();
    }

    /// <inheritdoc />
    protected override void OnObjectListChangedImpl(object? sender, ObjectListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation)
        {
            return;
        }

        if (!this._hunt!.TreasureTile.HasValue)
        {
            this.Disable();
            return;
        }

        if (!e.Location.Objects.ContainsKey(this._hunt.TreasureTile.Value))
        {
            this._hunt.Complete();
            this.Disable();
            return;
        }

        if (ProfessionsModule.Config.UseLegacyProspectorHunt)
        {
            return;
        }

        var removed = e.Removed.Where(r => r.Value.IsStone()).ToList();
        if (removed.Count != 1)
        {
            return;
        }

        var distanceToTreasure = (int)removed.Single().Value.DistanceTo(this._hunt!.TreasureTile.Value);
        var detectionDistance = (int)ProfessionsModule.Config.ProspectorDetectionDistance;
        if (detectionDistance > 0 && !distanceToTreasure.IsIn(1..detectionDistance))
        {
            return;
        }

        var pitch = (int)(2400f * (1f - ((float)distanceToTreasure / detectionDistance)));
        Game1.playSoundPitched("detector", pitch);
        Log.A($"Beeped at frequency {pitch} Hz");
    }
}
