/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.TreasureHunt;

#region using directives

using DaLion.Shared.Events;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="TreasureHunts.ITreasureHunt"/> ends.</summary>
internal sealed class TreasureHuntEndedEvent : ManagedEvent
{
    private readonly Action<object?, ITreasureHuntEndedEventArgs> _onEndedImpl;

    /// <summary>Initializes a new instance of the <see cref="TreasureHuntEndedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal TreasureHuntEndedEvent(Action<object?, ITreasureHuntEndedEventArgs> callback)
        : base(ModEntry.EventManager)
    {
        this._onEndedImpl = callback;
        TreasureHunts.TreasureHunt.Ended += this.OnEnded;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        TreasureHunts.TreasureHunt.Ended -= this.OnEnded;
    }

    /// <summary>Raised when a <see cref="TreasureHunts.ITreasureHunt"/> ends.</summary>
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
