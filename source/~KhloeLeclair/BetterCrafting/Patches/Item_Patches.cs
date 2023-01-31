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
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Objects;

namespace Leclair.Stardew.BetterCrafting.Patches;

public static class Item_Patches {

	private static IMonitor? Monitor;

	internal static bool OverrideStackSize = false;

	public static void Patch(ModEntry mod) {
		Monitor = mod.Monitor;

		try {
			foreach(var type in AccessTools.AllTypes()) {
				if (type.IsAssignableTo(typeof(ISalable)) && AccessTools.DeclaredMethod(type, nameof(ISalable.maximumStackSize)) is MethodInfo method && !method.IsAbstract)
					mod.Harmony!.Patch(
						original: method,
						postfix: new HarmonyMethod(typeof(Item_Patches), nameof(maximumStackSize_Postfix))
					);
			}

		} catch (Exception ex) {
			mod.Log($"An error occurred while registering a harmony patch for items.", LogLevel.Warn, ex);
		}
	}

	static void maximumStackSize_Postfix(ISalable __instance, ref int __result) {
		if (OverrideStackSize)
			__result = 2;
	}

}
