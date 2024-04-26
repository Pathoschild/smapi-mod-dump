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

using Leclair.Stardew.Common.Integrations;

using StardewModdingAPI;

namespace Leclair.Stardew.BetterCrafting.Integrations.ConvenientChests;

public class CCIntegration : BaseIntegration<ModEntry> {

	private readonly IReflectionHelper Helper;

	public readonly Type? EntryType;

	public CCIntegration(ModEntry mod)
	: base(mod, "aEnigma.ConvenientChests", "1.6") {
		Helper = mod.Helper.Reflection;

		if (!IsLoaded)
			return;

		try {
			EntryType = Type.GetType("ConvenientChests.ModEntry, ConvenientChests");
			if (EntryType == null)
				throw new ArgumentNullException("cannot get ModEntry");

		} catch(Exception ex) {
			Log($"Unable to find classes. Disabling integration.", LogLevel.Info, ex, LogLevel.Debug);
			IsLoaded = false;
			return;
		}

	}

	public int GetCraftRadius() {
		if (!IsLoaded || EntryType is null)
			return 0;

		object? config = Helper.GetProperty<object>(EntryType, "Config", false)?.GetValue();
		if (config == null)
			return 0;

		if (!(Helper.GetProperty<bool>(config, "CraftFromChests", false)?.GetValue() ?? false))
			return 0;

		return Helper.GetProperty<int>(config, "CraftRadius", false)?.GetValue() ?? 0;
	}

}
