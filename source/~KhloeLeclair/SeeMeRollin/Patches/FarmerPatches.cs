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

using HarmonyLib;

using Microsoft.Xna.Framework;

using StardewModdingAPI;

using StardewValley;


namespace Leclair.Stardew.SeeMeRollin.Patches {
	public class FarmerPatches {

		public static IMonitor Monitor => ModEntry.Instance.Monitor;

		[HarmonyPatch(typeof(Farmer), "performBeginUsingTool")]
		public static class Buff_Begin {
			static bool Prefix(Farmer __instance) {
				ModEntry.Instance.FixAnimation(__instance);
				return true;
			}
		}

	}
}
