/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.ToolbarIcons.Framework.Services.Integrations.Vanilla;

using StardewMods.ToolbarIcons.Framework.Interfaces;
using StardewValley.Menus;

/// <inheritdoc />
internal sealed class SpecialOrders : IVanillaIntegration
{
    /// <inheritdoc />
    public int Index => 8;

    /// <inheritdoc />
    public string HoverText => I18n.Button_SpecialOrders();

    /// <inheritdoc />
    public void DoAction() => Game1.activeClickableMenu = new SpecialOrdersBoard();
}