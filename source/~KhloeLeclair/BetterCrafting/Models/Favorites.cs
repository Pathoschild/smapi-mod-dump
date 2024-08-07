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

using Leclair.Stardew.Common.Types;

namespace Leclair.Stardew.BetterCrafting.Models;

public class Favorites {

	public Dictionary<long, CaseInsensitiveHashSet> Cooking { get; set; } = new();
	public Dictionary<long, CaseInsensitiveHashSet> Crafting { get; set; } = new();

}
