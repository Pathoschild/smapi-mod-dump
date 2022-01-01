/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/geko_x/stardew-mods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShutUp {
	class Patches {

		// Harmony patch

		public static bool playSound_prePatch_GekoX_ShutUp(string cueName) {

			try {
				if (ModEntry.config.sounds.Contains(cueName)) {
					if (ModEntry.config.showDebugSpam)
						ModEntry.monitor.Log($"Shutting {cueName} up", LogLevel.Info);

					return false;
				}

				if (Game1.soundBank != null) {
					try {
						Game1.soundBank.PlayCue(cueName);
					} catch (Exception ex) {
						Game1.debugOutput = Game1.parseText(ex.Message);
						Console.WriteLine(ex);
					}
				}

				return false;
			}

			catch (Exception e) {
				ModEntry.monitor.Log($"Failed in {nameof(playSound_prePatch_GekoX_ShutUp)}:\n{e}", LogLevel.Error);
				return true;
			}	
		}

		public static bool playSoundPitched_prePatch_GekoX_ShutUp(string cueName, int pitch) {

			try {
				if (ModEntry.config.sounds.Contains(cueName)) {
					if(ModEntry.config.showDebugSpam)
						ModEntry.monitor.Log($"Shutting {cueName} up", LogLevel.Info);

					return false;
				}

				if (Game1.soundBank != null) {
					try {
						ICue cue = Game1.soundBank.GetCue(cueName);
						cue.SetVariable("Pitch", pitch);
						cue.Play();
					} catch (Exception ex) {
						Game1.debugOutput = Game1.parseText(ex.Message);
						Console.WriteLine(ex);
					}
				}

				return false;

			}
			
			catch (Exception e) {
				ModEntry.monitor.Log($"Failed in {nameof(playSoundPitched_prePatch_GekoX_ShutUp)}:\n{e}", LogLevel.Error);
				return true;
			}

		}
	}
}
