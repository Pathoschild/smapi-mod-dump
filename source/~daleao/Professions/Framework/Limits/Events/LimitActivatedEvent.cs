/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Limits.Events;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="ILimitBreak"/> is activated.</summary>
internal sealed class LimitActivatedEvent : ManagedEvent
{
    private readonly Action<object?, ILimitActivatedEventArgs> _onActivatedImpl;

    /// <summary>Initializes a new instance of the <see cref="LimitActivatedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal LimitActivatedEvent(
        Action<object?, ILimitActivatedEventArgs> callback,
        EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
        this._onActivatedImpl = callback;
        LimitBreak.Activated += this.OnActivated;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        LimitBreak.Activated -= this.OnActivated;
    }

    /// <summary>Raised when a player activates the combat <see cref="ILimitBreak"/>.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnActivated(object? sender, ILimitActivatedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this._onActivatedImpl(sender, e);
        }
    }
}
