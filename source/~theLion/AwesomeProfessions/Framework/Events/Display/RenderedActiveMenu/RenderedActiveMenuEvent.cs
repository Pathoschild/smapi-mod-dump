/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events;

internal abstract class RenderedActiveMenuEvent : BaseEvent
{
    /// <inheritdoc />
    public override void Hook()
    {
        ModEntry.ModHelper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
    }

    /// <inheritdoc />
    public override void Unhook()
    {
        ModEntry.ModHelper.Events.Display.RenderedActiveMenu -= OnRenderedActiveMenu;
    }

    /// <summary>
    ///     When a menu is open, raised after that menu is drawn to the sprite batch but before it's rendered to the
    ///     screen.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public abstract void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e);
}