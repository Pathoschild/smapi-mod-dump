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

internal abstract class ReturnedToTitleEvent : BaseEvent
{
    /// <summary>Raised after the game returns to the title screen.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    public void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
    {
        if (enabled.Value) OnReturnedToTitleImpl(sender, e);
    }

    /// <inheritdoc cref="OnReturnedToTitle" />
    protected abstract void OnReturnedToTitleImpl(object sender, ReturnedToTitleEventArgs e);
}