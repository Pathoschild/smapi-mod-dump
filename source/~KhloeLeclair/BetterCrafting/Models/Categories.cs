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

namespace Leclair.Stardew.BetterCrafting.Models;

public class Categories {

	public Category[] Cooking { get; set; } = Array.Empty<Category>();

	public Category[] Crafting { get; set; } = Array.Empty<Category>();

	public AppliedDefaults? Applied { get; set; }

}
