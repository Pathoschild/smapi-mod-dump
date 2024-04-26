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

using System;
using System.Collections.Generic;

namespace Leclair.Stardew.BetterCrafting.Models;

public class Categories {

	public Category[]? Cooking { get; set; }

	public Category[]? Crafting { get; set; }

	public AppliedDefaults? Applied { get; set; }

}

public class CPCategories {

	public Dictionary<string, Category> Cooking { get; set; } = null!;

	public Dictionary<string, Category> Crafting { get; set; } = null!;

}
