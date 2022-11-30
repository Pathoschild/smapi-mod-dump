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
using System.Reflection;

using StardewModdingAPI;

using Leclair.Stardew.Common.Integrations;

namespace Leclair.Stardew.GiantCropTweaks.Integrations.GiantCropFertilizer;

internal class GCFIntegration : BaseIntegration<ModEntry> {

	private readonly Type? GCFEntry;
	private readonly IReflectedProperty<int>? GCF_GetId;

	internal GCFIntegration(ModEntry mod) : base(mod, "atravita.GiantCropFertilizer", "0.2.0") {

		if (!IsLoaded)
			return;

		try {
			GCFEntry = Type.GetType("GiantCropFertilizer.ModEntry, GiantCropFertilizer");
			if (GCFEntry == null)
				throw new ArgumentNullException("GCFEntry");

			GCF_GetId = mod.Helper.Reflection.GetProperty<int>(GCFEntry, "GiantCropFertilizerID");

		} catch(Exception) {
			Log($"Unable to find GiantCropFertilizer. Will not be able to process Giant Crop Fertilizer.", LogLevel.Warn);
		}
	}

	internal int GiantCropFertilizerID {
		get {
			if (!IsLoaded || GCF_GetId is null)
				return -1;
			return GCF_GetId.GetValue();
		}
	}

	internal double FertilizerChance {
		get {
			if (!IsLoaded)
				return 1.1d;

			try {
				object? config = GCFEntry?.GetProperty("Config", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null);
				if (config is null)
					return 1.1d;

				return Self.Helper.Reflection.GetProperty<double>(config, "GiantCropChance").GetValue();
			} catch(Exception ex) {
				Log($"Unable to get giant fertilizer chance.", LogLevel.Warn, ex);
				return 1.1d;
			}
		}
	}

}
