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
using Leclair.Stardew.Common.Integrations;

using StackSplitRedux;

using StardewValley;

using Leclair.Stardew.BetterCrafting.Menus;

namespace Leclair.Stardew.BetterCrafting.Integrations.StackSplitRedux {
	public class SSRIntegration : BaseAPIIntegration<IStackSplitAPI, ModEntry> {

		public SSRIntegration(ModEntry mod)
		: base(mod, "pepoluan.StackSplitRedux", "0.15.0") {
			if (!IsLoaded)
				return;

			API.RegisterBasicMenu(
				typeof(BetterCraftingPage),
				page => (page as BetterCraftingPage)?.inventory,
				page => {
					if (page is not BetterCraftingPage bcp)
						return null;
					return Self.Helper.Reflection.GetField<Item>(bcp, "hoverItem");
				},
				page => {
					if (page is not BetterCraftingPage bcp)
						return null;
					return Self.Helper.Reflection.GetField<Item>(bcp, "HeldItem");
				},
				null
				/*(page, point) => { 

					if (page is not BetterCraftingPage bcp)
						return null;

					if (bcp.Editing)
						return null;

					if (bcp.GetChildMenu() != null)
						return null;

					var recipe = bcp.GetRecipeUnderCursor(point.X, point.Y);
					if (recipe == null)
						return null;

					if (!bcp.CanPerformCraft(recipe))
						return null;

					return new Tuple<int, Action<bool, int>>(recipe.QuantityPerCraft, (success, amount) => {
						int times = (int) Math.Ceiling((double) amount / recipe.QuantityPerCraft);

						bcp.PerformCraft(
							recipe,
							times
						);
					});
				}*/
			);
		}
	}
}
