/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IMultiplayerEvents.ModMessageReceived"/> allowing dynamic enabling / disabling.</summary>
public abstract class ModMessageReceivedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ModMessageReceivedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected ModMessageReceivedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
    }

    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && base.IsEnabled;

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Multiplayer.ModMessageReceived -= this.OnModMessageReceived;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="IMultiplayerEvents.ModMessageReceived"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnModMessageReceivedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnModMessageReceived"/>
    protected abstract void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e);
}
