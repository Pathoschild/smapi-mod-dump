/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.TreasureHunt;

#region using directives

using DaLion.Shared.Events;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="TreasureHunts.ITreasureHunt"/> starts.</summary>
internal sealed class TreasureHuntStartedEvent : ManagedEvent
{
    private readonly Action<object?, ITreasureHuntStartedEventArgs> _onStartedImpl;

    /// <summary>Initializes a new instance of the <see cref="TreasureHuntStartedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal TreasureHuntStartedEvent(
        Action<object?, ITreasureHuntStartedEventArgs> callback)
        : base(ModEntry.EventManager)
    {
        this._onStartedImpl = callback;
        TreasureHunts.TreasureHunt.Started += this.OnStarted;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        TreasureHunts.TreasureHunt.Started -= this.OnStarted;
    }

    /// <summary>Raised when a <see cref="TreasureHunts.ITreasureHunt"/> starts.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnStarted(object? sender, ITreasureHuntStartedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this._onStartedImpl(sender, e);
        }
    }
}
