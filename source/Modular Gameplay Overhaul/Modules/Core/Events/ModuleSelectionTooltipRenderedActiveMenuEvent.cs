/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Core.Events;

#region using directives

using DaLion.Overhaul.Modules.Core.ConfigMenu;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class ModuleSelectionTooltipRenderedActiveMenuEvent : RenderedActiveMenuEvent
{
    /// <summary>Initializes a new instance of the <see cref="ModuleSelectionTooltipRenderedActiveMenuEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ModuleSelectionTooltipRenderedActiveMenuEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => ModuleSelectionOption.Tooltip is not null;

    /// <inheritdoc />
    protected override void OnRenderedActiveMenuImpl(object? sender, RenderedActiveMenuEventArgs e)
    {
        IClickableMenu.drawHoverText(e.SpriteBatch, ModuleSelectionOption.Tooltip, Game1.smallFont);
    }
}
