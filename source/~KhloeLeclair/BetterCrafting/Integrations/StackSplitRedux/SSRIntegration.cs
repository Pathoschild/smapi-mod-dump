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

using Leclair.Stardew.Common.Integrations;

using StackSplitRedux;

using StardewValley;

using Leclair.Stardew.BetterCrafting.Menus;

namespace Leclair.Stardew.BetterCrafting.Integrations.StackSplitRedux {
	public class SSRIntegration : BaseAPIIntegration<IStackSplitAPI, ModEntry> {

		public SSRIntegration(ModEntry mod)
		: base(mod, "pepoluan.StackSplitRedux", "0.14.0") {
			if (!IsLoaded)
				return;

			API.RegisterBasicMenu<BetterCraftingPage>(
				page => page.inventory,
				page => {
					return Self.Helper.Reflection.GetField<Item>(page, "hoverItem");
				},
				page => {
					return Self.Helper.Reflection.GetField<Item>(page, "HeldItem");
				},
				null
			);
		}
	}
}
