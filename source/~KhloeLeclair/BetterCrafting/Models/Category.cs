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

using Leclair.Stardew.BetterCrafting.DynamicRules;
using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.Types;

using Newtonsoft.Json;

namespace Leclair.Stardew.BetterCrafting.Models;

public class Category {

	public string? Id { get; set; }

	public string? Name { get; set; }
	public string? I18nKey { get; set; }

	public bool UseRules { get; set; }
	public List<DynamicRuleData>? DynamicRules { get; set; }

	public CategoryIcon? Icon { get; set; }

	public CaseInsensitiveHashSet? Recipes { get; set; }
	public string[]? UnwantedRecipes { get; set; }

	public bool IncludeInMisc { get; set; }

	[JsonIgnore]
	public (IDynamicRuleHandler, object?, DynamicRuleData)[]? CachedRules { get; set; }

	[JsonIgnore]
	public List<IRecipe>? CachedRecipes { get; set; }

}
