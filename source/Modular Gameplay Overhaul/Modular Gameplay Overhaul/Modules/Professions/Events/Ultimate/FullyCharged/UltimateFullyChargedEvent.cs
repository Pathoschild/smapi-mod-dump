/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.Ultimate;

#region using directives

using DaLion.Shared.Events;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="Ultimates.IUltimate"/> reaches the maximum charge value.</summary>
internal sealed class UltimateFullyChargedEvent : ManagedEvent
{
    private readonly Action<object?, IUltimateFullyChargedEventArgs> _onFullyChargedImpl;

    /// <summary>Initializes a new instance of the <see cref="UltimateFullyChargedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal UltimateFullyChargedEvent(
        Action<object?, IUltimateFullyChargedEventArgs> callback)
        : base(ModEntry.EventManager)
    {
        this._onFullyChargedImpl = callback;
        Ultimates.Ultimate.FullyCharged += this.OnFullyCharged;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        Ultimates.Ultimate.FullyCharged -= this.OnFullyCharged;
    }

    /// <summary>Raised when the local player's <see cref="Ultimates.IUltimate"/> charge value reaches max value.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnFullyCharged(object? sender, IUltimateFullyChargedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this._onFullyChargedImpl(sender, e);
        }
    }
}
