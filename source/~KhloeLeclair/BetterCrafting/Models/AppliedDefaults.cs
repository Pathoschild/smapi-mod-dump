/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Leclair.Stardew.Common.Types;

namespace Leclair.Stardew.BetterCrafting.Models;

public class AppliedDefaults {

	public AppliedStuff Cooking { get; set; } = new();

	public AppliedStuff Crafting { get; set; } = new();

}

public class AppliedStuff {
	public List<string> AddedCategories { get; set; } = new();
	public Dictionary<string, CaseInsensitiveHashSet> AddedRecipes { get; set; } = new();
	public Dictionary<string, CaseInsensitiveHashSet> RemovedRecipes { get; set; } = new();
}

public class AddedAPICategories {
	public Dictionary<string, Category> AddedCategories { get; set; } = new();
	public Dictionary<string, CaseInsensitiveHashSet> AddedRecipes { get; set; } = new();
	public Dictionary<string, CaseInsensitiveHashSet> RemovedRecipes { get; set; } = new();
}
