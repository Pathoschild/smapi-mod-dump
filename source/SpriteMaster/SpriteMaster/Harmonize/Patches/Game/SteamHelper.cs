/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

#if false
using StardewValley.SDKs;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace SpriteMaster.Harmonize.Patches.Game;

internal static class SteamHelper {
	private static readonly bool IsSteamBuild =
		((int?)typeof(StardewValley.Program).GetField("buildType", BindingFlags.Public | BindingFlags.Static)
			?.GetValue(null) ?? -1) ==
				StardewValley.Program.build_steam;

	private static readonly Type? SteamHelperType =
		typeof(StardewValley.Program).Assembly.GetType("StardewValley.SDKs.SteamHelper");

	private static Task<SDKHelper?>? SDKHelperTask = null;

	internal static void Init() {
		if (!IsSteamBuild || SteamHelperType is null) {
			return;
		}

		/*
		SDKHelperTask = Task.Run(
			() => (SDKHelper?)Activator.CreateInstance(SteamHelperType)
		);
		*/
	}

	[Harmonize(
		typeof(StardewValley.Program),
		"sdk.get",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static bool SDK_GetPre(ref SDKHelper __result, ref SDKHelper ____sdk) {
		if (!IsSteamBuild || ____sdk is not null) {
			return true;
		}

		if (SDKHelperTask is not null) {
			var result = SDKHelperTask.Result;
			if (result is not null) {
				____sdk = result;
			}

			SDKHelperTask = null;
		}

		return true;
	}
}
#endif
