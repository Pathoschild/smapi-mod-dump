/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Core.Framework.Events;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="OutOfCombatWarpedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class OutOfCombatWarpedEvent(EventManager? manager = null)
    : WarpedEvent(manager ?? CoreMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation.IsEnemyArea() || State.AreEnemiesNearby)
        {
            this.Manager.Enable<OutOfCombatOneSecondUpdateTickedEvent>();
        }
        else
        {
            this.Manager.Disable<OutOfCombatOneSecondUpdateTickedEvent>();
        }
    }
}
