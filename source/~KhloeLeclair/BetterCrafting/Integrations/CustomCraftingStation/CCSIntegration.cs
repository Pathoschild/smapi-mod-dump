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

using System;
using System.Linq;
using System.Collections.Generic;

using HarmonyLib;

using StardewModdingAPI;

using Leclair.Stardew.Common.Integrations;

namespace Leclair.Stardew.BetterCrafting.Integrations.CustomCraftingStation;

public class CCSIntegration : BaseIntegration<ModEntry> {

	private readonly IReflectionHelper Helper;

	// ModEntry
	public readonly Type? EntryType;
	private static object? Entry;

	public CCSIntegration(ModEntry mod)
	: base(mod, "Cherry.CustomCraftingStations", "1.1.3") {
		Helper = mod.Helper.Reflection;

		if (!IsLoaded)
			return;

		try {
			EntryType = Type.GetType("CustomCraftingStation.ModEntry, CustomCraftingStation");
			if (EntryType == null)
				throw new ArgumentNullException("cannot get ModEntry");

		} catch(Exception ex) {
			Log($"Unable to find classes. Disabling integration.", LogLevel.Info, ex, LogLevel.Debug);
			IsLoaded = false;
			return;
		}

		mod.Harmony!.Patch(
			AccessTools.Method(EntryType, "GameLoop_SaveLoaded"),
			new HarmonyMethod(typeof(CCSIntegration), nameof(OnSaveLoaded_Prefix))
		);
	}

	public List<string>? GetRemovedRecipes(bool cooking) {
		if (!IsLoaded || Entry == null)
			return null;

		return Helper.GetField<List<string>>(Entry, cooking ? "_cookingRecipesToRemove" : "_craftingRecipesToRemove", false)?.GetValue();
	}

	public static bool OnSaveLoaded_Prefix(object __instance) {
		Entry = __instance;
		return true;
	}
}
