/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HappyHomeDesigner
**
*************************************************/

using HappyHomeDesigner.Framework;
using HappyHomeDesigner.Menus;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace HappyHomeDesigner.Patches
{
	internal class ReplaceShop
	{
		internal static void Apply(Harmony harmony)
		{
			var patch = new HarmonyMethod(typeof(ReplaceShop), nameof(PatchOpenShop));
			var targets = typeof(Utility).GetMethods(
				BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static
			).Where(m => m.Name is nameof(Utility.TryOpenShopMenu));

			foreach (var method in targets)
				harmony.TryPatch(method, transpiler: patch);
		}

		public static IEnumerable<CodeInstruction> PatchOpenShop(IEnumerable<CodeInstruction> src, ILGenerator gen)
		{
			var il = new CodeMatcher(src, gen);

			il.MatchStartForward(
				new CodeMatch(OpCodes.Call, typeof(Game1).GetProperty(nameof(Game1.activeClickableMenu)).SetMethod)
			);

			if (il.IsInvalid)
			{
				ModEntry.monitor.Log($"Failed to patch shop open! Could not find injection point.", LogLevel.Error);
				return null;
			}

			il.Insert(
				new CodeMatch(OpCodes.Call, typeof(ReplaceShop).GetMethod(nameof(CheckAndReplace)))
			);

			return il.InstructionEnumeration();
		}

		public static ShopMenu CheckAndReplace(ShopMenu menu)
		{
			if (Catalog.TryShowCatalog(menu))
				return null;
			return menu;
		}
	}
}
