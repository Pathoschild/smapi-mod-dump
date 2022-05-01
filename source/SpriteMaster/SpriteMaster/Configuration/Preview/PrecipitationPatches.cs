/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMHarmonize = SpriteMaster.Harmonize;

namespace SpriteMaster.Configuration.Preview;

static class PrecipitationPatches {
	private static PrecipitationType Precipitation => PrecipitationOverride ?? Scene.Current?.Precipitation ?? PrecipitationType.None;
	internal static PrecipitationType? PrecipitationOverride = null;

	[SMHarmonize.Harmonize(
		typeof(Game1),
		"IsSnowingHere",
		SMHarmonize.Harmonize.Fixation.Prefix,
		SMHarmonize.Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static bool IsSnowingHere(ref bool __result, GameLocation location) {
		if (Precipitation != PrecipitationType.Snow) {
			if (Scene.Current is null) {
				return true;
			}
			else {
				__result = false;
				return false;
			}
		}

		if (location == Scene.SceneLocation.Value || location is null) {
			__result = true;
			return false;
		}

		return true;
	}

	[SMHarmonize.Harmonize(
		typeof(Game1),
		"IsRainingHere",
		SMHarmonize.Harmonize.Fixation.Prefix,
		SMHarmonize.Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static bool IsRainingHere(ref bool __result, GameLocation location) {
		if (Precipitation != PrecipitationType.Rain) {
			if (Scene.Current is null) {
				return true;
			}
			else {
				__result = false;
				return false;
			}
		}

		if (location == Scene.SceneLocation.Value || location is null) {
			__result = true;
			return false;
		}

		return true;
	}
}
