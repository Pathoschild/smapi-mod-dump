/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

using StardewValley;
using StardewValley.Menus;
using System;

namespace DaLion.Stardew.Arsenal.Framework;

internal static class SpaceCoreUtils
{
    internal static Func<IClickableMenu, ClickableTextureComponent>? GetNewForgeMenuLeftIngredientSpot { get; set; }
    internal static Func<IClickableMenu, int, int>? GetNewForgeMenuForgeCostAtLevel { get; set; }
    internal static Func<IClickableMenu, Item, Item, int>? GetNewForgeMenuForgeCost { get; set; }
    internal static Action<IClickableMenu, Item>? SetNewForgeMenuHeldItem { get; set; }
}