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

internal abstract class RenderedActiveMenuEvent : BaseEvent
{
    /// <summary>
    ///     When a menu is open, raised after that menu is drawn to the sprite batch but before it's rendered to the
    ///     screen.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
    {
        if (enabled.Value) OnRenderedActiveMenuImpl(sender, e);
    }

    /// <inheritdoc cref="OnRenderedActiveMenu" />
    protected abstract void OnRenderedActiveMenuImpl(object sender, RenderedActiveMenuEventArgs e);
}