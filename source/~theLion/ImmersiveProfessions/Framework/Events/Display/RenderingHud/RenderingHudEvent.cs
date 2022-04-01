/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

internal abstract class RenderingHudEvent : BaseEvent
{
    /// <summary>Raised before the game draws anything to the screen in a draw tick, as soon as the sprite batch is opened.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnRenderingHud(object sender, RenderingHudEventArgs e)
    {
        if (enabled.Value) OnRenderingHudImpl(sender, e);
    }

    /// <inheritdoc cref="OnRenderingHud" />
    protected abstract void OnRenderingHudImpl(object sender, RenderingHudEventArgs e);
}