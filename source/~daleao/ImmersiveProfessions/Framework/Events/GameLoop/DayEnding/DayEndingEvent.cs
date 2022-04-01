/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

internal abstract class DayEndingEvent : BaseEvent
{
    /// <summary>Raised before the game ends the current day.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnDayEnding(object sender, DayEndingEventArgs e)
    {
        if (enabled.Value) OnDayEndingImpl(sender, e);
    }

    /// <inheritdoc cref="OnDayEnding" />
    protected abstract void OnDayEndingImpl(object sender, DayEndingEventArgs e);
}