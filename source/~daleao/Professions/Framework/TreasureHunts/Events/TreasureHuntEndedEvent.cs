/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.TreasureHunts.Events;

#region using directives

using DaLion.Shared.Events;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="ITreasureHunt"/> ends.</summary>
internal sealed class TreasureHuntEndedEvent : ManagedEvent
{
    private readonly Action<object?, ITreasureHuntEndedEventArgs> _onEndedImpl;

    /// <summary>Initializes a new instance of the <see cref="TreasureHuntEndedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal TreasureHuntEndedEvent(
        Action<object?, ITreasureHuntEndedEventArgs> callback,
        EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
        this._onEndedImpl = callback;
        TreasureHunt.Ended += this.OnEnded;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        TreasureHunt.Ended -= this.OnEnded;
    }

    /// <summary>Raised when a <see cref="ITreasureHunt"/> ends.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnEnded(object? sender, ITreasureHuntEndedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this._onEndedImpl(sender, e);
        }
    }
}
