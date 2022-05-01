/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System.Collections.Generic;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Crafting;

using StardewValley.Menus;

namespace Leclair.Stardew.BetterCrafting.Models;

public record TabInfo {

	public Category Category { get; set; } = default!;
	public ClickableComponent Component { get; set; } = default!;
	public List<IRecipe> Recipes { get; set; } = default!;
	public List<IRecipe> FilteredRecipes { get; set; } = default!;
	public SpriteInfo? Sprite { get; set; } = default!;

}
