/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

internal abstract class UpdateTickedEvent : BaseEvent
{
    /// <summary>Raised after the game state is updated.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        if (enabled.Value) OnUpdateTickedImpl(sender, e);
    }

    /// <inheritdoc cref="OnUpdateTicked" />
    protected abstract void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e);
}