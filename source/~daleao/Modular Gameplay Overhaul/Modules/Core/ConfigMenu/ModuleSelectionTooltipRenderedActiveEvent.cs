/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class ModuleSelectionTooltipRenderedActiveEvent : RenderedActiveMenuEvent
{
    /// <summary>Initializes a new instance of the <see cref="ModuleSelectionTooltipRenderedActiveEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ModuleSelectionTooltipRenderedActiveEvent(EventManager manager)
        : base(manager)
    {
    }

    public override bool IsEnabled => ModuleSelectionOption.Tooltip is not null;

    /// <inheritdoc />
    protected override void OnRenderedActiveMenuImpl(object? sender, RenderedActiveMenuEventArgs e)
    {
        IClickableMenu.drawHoverText(e.SpriteBatch, ModuleSelectionOption.Tooltip, Game1.smallFont);
    }
}
