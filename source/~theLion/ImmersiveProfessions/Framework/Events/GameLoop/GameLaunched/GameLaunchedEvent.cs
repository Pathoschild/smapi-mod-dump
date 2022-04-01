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

internal abstract class GameLaunchedEvent : BaseEvent
{
    /// <summary>Raised after the game is launched, right before the first update tick.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    public void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
        if (enabled.Value || GetType().Name.StartsWith("Static")) OnGameLaunchedImpl(sender, e);
    }

    /// <inheritdoc cref="OnGameLaunched" />
    protected abstract void OnGameLaunchedImpl(object sender, GameLaunchedEventArgs e);
}