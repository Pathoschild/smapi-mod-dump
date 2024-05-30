/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Events.GameLoop.TimeChanged;

#region using directives

using DaLion.Professions.Framework.TreasureHunts;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ScavengerHuntTimeChangedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ScavengerHuntTimeChangedEvent(EventManager? manager = null)
    : TimeChangedEvent(manager ?? ProfessionsMod.EventManager)
{
    private static uint _previousHuntStepsTaken;

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        if (_previousHuntStepsTaken == 0)
        {
            _previousHuntStepsTaken = Game1.player.stats.StepsTaken;
        }
    }

    /// <inheritdoc />
    protected override void OnTimeChangedImpl(object? sender, TimeChangedEventArgs e)
    {
        if (!Game1.currentLocation.IsSuitableScavengerHuntLocation())
        {
            return;
        }

        var delta = Game1.player.stats.StepsTaken - _previousHuntStepsTaken;
        State.ScavengerHunt ??= new ScavengerHunt();
        if (State.ScavengerHunt.TryStart(Math.Pow(1.0016, delta) - 1d))
        {
            _previousHuntStepsTaken += delta;
        }
    }
}
