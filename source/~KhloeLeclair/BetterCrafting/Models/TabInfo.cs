/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Crafting;

using StardewValley.Menus;

namespace Leclair.Stardew.BetterCrafting.Models {
	public class TabInfo {
		public Category Category;
		public ClickableComponent Component;
		public List<IRecipe> Recipes;
		public SpriteInfo Sprite;
	}
}
