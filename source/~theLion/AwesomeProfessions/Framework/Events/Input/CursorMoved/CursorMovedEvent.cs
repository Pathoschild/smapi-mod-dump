/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Input;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

internal abstract class CursorMovedEvent : BaseEvent
{
    /// <summary>Raised after the player moves the in-game cursor.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnCursorMoved(object sender, CursorMovedEventArgs e)
    {
        if (enabled.Value) OnCursorMovedImpl(sender, e);
    }

    /// <inheritdoc cref="OnCursorMoved" />
    protected abstract void OnCursorMovedImpl(object sender, CursorMovedEventArgs e);
}