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

/// <summary>A dynamic event raised when a <see cref="ILimitBreak"/> reaches the maximum charge value.</summary>
internal sealed class LimitFullyChargedEvent : ManagedEvent
{
    private readonly Action<object?, ILimitFullyChargedEventArgs> _onFullyChargedImpl;

    /// <summary>Initializes a new instance of the <see cref="LimitFullyChargedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal LimitFullyChargedEvent(
        Action<object?, ILimitFullyChargedEventArgs> callback,
        EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
        this._onFullyChargedImpl = callback;
        LimitBreak.FullyCharged += this.OnFullyCharged;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        LimitBreak.FullyCharged -= this.OnFullyCharged;
    }

    /// <summary>Raised when the local player's <see cref="ILimitBreak"/> charge value reaches max value.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnFullyCharged(object? sender, ILimitFullyChargedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this._onFullyChargedImpl(sender, e);
        }
    }
}
