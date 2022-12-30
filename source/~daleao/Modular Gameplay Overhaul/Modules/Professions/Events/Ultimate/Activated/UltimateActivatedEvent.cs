/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.Ultimate;

#region using directives

using DaLion.Shared.Events;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="Ultimates.IUltimate"/> is activated.</summary>
internal sealed class UltimateActivatedEvent : ManagedEvent
{
    private readonly Action<object?, IUltimateActivatedEventArgs> _onActivatedImpl;

    /// <summary>Initializes a new instance of the <see cref="UltimateActivatedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal UltimateActivatedEvent(Action<object?, IUltimateActivatedEventArgs> callback)
        : base(ModEntry.EventManager)
    {
        this._onActivatedImpl = callback;
        Ultimates.Ultimate.Activated += this.OnActivated;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        Ultimates.Ultimate.Activated -= this.OnActivated;
    }

    /// <summary>Raised when a player activates the combat <see cref="Ultimates.IUltimate"/>.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnActivated(object? sender, IUltimateActivatedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this._onActivatedImpl(sender, e);
        }
    }
}
