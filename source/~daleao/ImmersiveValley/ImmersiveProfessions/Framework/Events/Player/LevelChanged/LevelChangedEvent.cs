/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IPlayerEvents.LevelChanged"/> allowing dynamic enabling / disabling.</summary>
internal abstract class LevelChangedEvent : BaseEvent
{
    /// <summary>Raised after a player's skill level changes.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnLevelChanged(object sender, LevelChangedEventArgs e)
    {
        if (enabled.Value) OnLevelChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnLevelChanged" />
    protected abstract void OnLevelChangedImpl(object sender, LevelChangedEventArgs e);
}