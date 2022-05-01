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

using Leclair.Stardew.Common.Types;

namespace Leclair.Stardew.BetterCrafting.Models;

public class Category {

	public string? Id { get; set; }

	public string? Name { get; set; }
	public string? I18nKey { get; set; }

	public CategoryIcon? Icon { get; set; }

	public CaseInsensitiveHashSet? Recipes { get; set; }
	public string[]? UnwantedRecipes { get; set; }

}
