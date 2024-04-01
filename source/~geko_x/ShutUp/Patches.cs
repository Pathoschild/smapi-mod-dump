/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/geko_x/stardew-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShutUp {
	class Patches {

		// Harmony patch

		//public static bool playSound_prePatch_GekoX_ShutUp(string cueName) {

		//	try {
		//		if (ModEntry.config.sounds.Contains(cueName)) {
		//			if (ModEntry.config.showDebugSpam)
		//				ModEntry.monitor.Log($"Shutting {cueName} up", LogLevel.Info);

		//			return false;
		//		}

		//		if (Game1.soundBank != null) {
		//			try {
		//				Game1.soundBank.PlayCue(cueName);
		//			} catch (Exception ex) {
		//				Game1.debugOutput = Game1.parseText(ex.Message);
		//				Console.WriteLine(ex);
		//			}
		//		}

		//		return false;
		//	} catch (Exception e) {
		//		ModEntry.monitor.Log($"Failed in {nameof(playSound_prePatch_GekoX_ShutUp)}:\n{e}", LogLevel.Error);
		//		return true;
		//	}
		//}

		public static bool playLocal_PrePatch_GekoX_ShutUp(string cueName, GameLocation location, Vector2? position, int? pitch, SoundContext context, out ICue cue) {

			doDebugSpam($"Attempting to play sound {cueName}");

			try {

				cue = Game1.soundBank.GetCue(cueName);

				if (ModEntry.config.sounds.Contains(cueName)) {
					doDebugSpam($"Shutting {cueName} up");

					return false;
				}

				doDebugSpam($"Passing sound {cueName} through to vanilla");
				return true;

			}
			catch (Exception e) {
				cue = null;
				ModEntry.monitor.Log($"Failed in {nameof(playLocal_PrePatch_GekoX_ShutUp)} and falling back to vanilla game code:\n{e}", LogLevel.Error);
				
				return true;
			}
		}

		private static void doDebugSpam(string msg) {
			if (ModEntry.config.showDebugSpam)
				ModEntry.monitor.Log(msg, LogLevel.Debug);
		}

	}

}
