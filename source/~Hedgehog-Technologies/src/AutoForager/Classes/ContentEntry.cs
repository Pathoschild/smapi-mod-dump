/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hedgehog-Technologies/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace AutoForager.Classes
{
	internal class ContentEntry
	{
		public string Category { get; set; }
		public List<string>? Forageables { get; set; }
		public List<string>? FruitTrees { get; set; }
		public List<string>? WildTrees { get; set; }
		public List<string>? IgnoredItems { get; set; }

		public ContentEntry()
		{
			Category = string.Empty;
			Forageables = new();
			FruitTrees = new();
			WildTrees = new();
			IgnoredItems = new();
		}
	}
}
