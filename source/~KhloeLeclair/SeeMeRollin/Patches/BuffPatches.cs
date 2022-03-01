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
using StardewValley.Menus;

namespace Leclair.Stardew.SeeMeRollin.Patches {
	public class BuffPatches {

		public static IMonitor Monitor => ModEntry.instance.Monitor;

		[HarmonyPatch(typeof(Buff), nameof(Buff.update))]
		public static class Buff_Update {
			static bool Prefix(Buff __instance, GameTime time, ref bool __result) {
				if (__instance is RollinBuff) {
					__result = false;
					return false;
				}

				return true;
			}
		}

		[HarmonyPatch(typeof(Buff), nameof(Buff.getTimeLeft))]
		public static class Buff_TimeLeft {
			static bool Prefix(Buff __instance, ref string __result) {
				if (__instance is RollinBuff) {
					__result = "";
					return false;
				}

				return true;
			}
		}

		[HarmonyPatch(typeof(Buff), nameof(Buff.getClickableComponents))]
		public static class Buff_Components {
			static bool Prefix(Buff __instance, ref List<ClickableTextureComponent> __result) {
				if (__instance is RollinBuff && !ModEntry.instance.Config.ShowBuff) {
					__result = new();
					return false;
				}

				return true;
			}
		}

	}
}
