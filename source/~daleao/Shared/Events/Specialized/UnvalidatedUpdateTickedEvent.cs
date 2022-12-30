/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="ISpecializedEvents.UnvalidatedUpdateTicked"/> allowing dynamic enabling / disabling.</summary>
internal abstract class UnvalidatedUpdateTickedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="UnvalidatedUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected UnvalidatedUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
        //manager.ModEvents.Specialized.UnvalidatedUpdateTicked += this.OnUnvalidatedUpdateTicked;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        //this.Manager.ModEvents.Specialized.UnvalidatedUpdateTicked -= this.OnUnvalidatedUpdateTicked;
    }

    /// <inheritdoc cref="ISpecializedEvents.UnvalidatedUpdateTicked"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnUnvalidatedUpdateTicked(object? sender, UnvalidatedUpdateTickedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnUnvalidatedUpdateTickedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnUnvalidatedUpdateTicked"/>
    protected abstract void OnUnvalidatedUpdateTickedImpl(object? sender, UnvalidatedUpdateTickedEventArgs e);
}
