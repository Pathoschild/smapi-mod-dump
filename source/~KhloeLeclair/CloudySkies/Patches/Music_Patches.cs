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
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;

using Leclair.Stardew.CloudySkies.Models;

using StardewValley;
using StardewValley.Network;

namespace Leclair.Stardew.CloudySkies.Patches;

/// <summary>
/// While many of my patches are grouped by what file they're patching, this
/// particular patch set is based on what it's changing.
///
/// In this case: it's changing the behavior for weather-based music overrides.
/// </summary>

public static class Music_Patches {

	private static ModEntry? Mod;

	public static void Patch(ModEntry mod) {
		Mod = mod;

		try {

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(Game1), nameof(Game1.changeMusicTrack)),
				transpiler: new HarmonyMethod(typeof(Music_Patches), nameof(Game1_changeMusicTrack__Transpiler))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(Game1), nameof(Game1.GetDefaultSongPriority)),
				transpiler: new HarmonyMethod(typeof(Music_Patches), nameof(Game1_GetDefaultSongPriority__Transpiler))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(Game1), "onFadeToBlackComplete"),
				transpiler: new HarmonyMethod(typeof(Music_Patches), nameof(Game1_onFadeToBlackComplete__Transpiler))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(Game1), nameof(Game1.playMorningSong)),
				transpiler: new HarmonyMethod(typeof(Music_Patches), nameof(Game1_PlayMorningSong__Transpiler))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(Game1), nameof(Game1.updateMusic)),
				transpiler: new HarmonyMethod(typeof(Music_Patches), nameof(Game1_UpdateMusic__Transpiler))
			);

		} catch(Exception ex) {
			mod.Log($"Error patching Game1. Weather music will not work correctly.", StardewModdingAPI.LogLevel.Error, ex);
		}

		try {

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.checkForMusic)),
				transpiler: new HarmonyMethod(typeof(Music_Patches), nameof(GameLocation_checkForMusic__Transpiler))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.GetLocationSpecificMusic)),
				transpiler: new HarmonyMethod(typeof(Music_Patches), nameof(GameLocation_GetLocationSpecificMusic__Transpiler))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.GetMorningSong)),
				transpiler: new HarmonyMethod(typeof(Music_Patches), nameof(GameLocation_GetMorningSong__Transpiler))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.HandleMusicChange)),
				transpiler: new HarmonyMethod(typeof(Music_Patches), nameof(GameLocation_HandleMusicChange__Transpiler))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsMiniJukeboxPlaying)),
				transpiler: new HarmonyMethod(typeof(Music_Patches), nameof(GameLocation_IsMiniJukeboxPlaying__Transpiler))
			);

		} catch(Exception ex) {
			mod.Log($"Error patching GameLocation. Weather music will not work correctly.", StardewModdingAPI.LogLevel.Error, ex);
		}


	}

	#region Helper Methods

	public static void PlayCustomWeatherMusic() {
		Game1.changeMusicTrack(PatchHelper.GetMusicOrDefault(null), track_interruptable: true);
	}

	#endregion

	#region Game1 Patches

	private static IEnumerable<CodeInstruction> Game1_changeMusicTrack__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var Game1_isRainingHere = AccessTools.Method(typeof(Game1), nameof(Game1.IsRainingHere));
		var hasMusic = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.HasMusic));

		var getMusicOrDefault = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.GetMusicOrDefault));

		foreach (var in0 in instructions) {

			if (in0.Calls(Game1_isRainingHere))
				// Old Code: Game1.IsRainingHere(null)
				// New Code: PatchHelper.HasMusic(null)
				yield return new CodeInstruction(in0) {
					opcode = OpCodes.Call,
					operand = hasMusic
				};

			else if (in0.LoadsConstant("rain")) {
				// Old Code: "rain"
				// New Code: PatchHelper.GetMusicOrDefault(null)
				yield return new CodeInstruction(in0) {
					opcode = OpCodes.Ldnull,
					operand = null
				};

				yield return new CodeInstruction(OpCodes.Call, getMusicOrDefault);

			} else
				yield return in0;
		}

	}

	private static IEnumerable<CodeInstruction> Game1_onFadeToBlackComplete__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var GameLocation_isRainingHere = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsRainingHere));
		var hasMusic = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.HasMusic));

		var getMusicOrDefault = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.GetMusicOrDefault));

		var getOutsideFrequency = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.GetOutsideFrequency));
		var getInsideFrequency = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.GetInsideFrequency));

		CodeInstruction[] instrs = instructions.ToArray();

		for (int i = 0; i < instrs.Length; i++) {
			var in0 = instrs[i];

			if (i + 1 < instrs.Length && in0.LoadsConstant("Frequency")) {
				yield return in0;

				var in1 = instrs[i + 1];
				if (in1.opcode == OpCodes.Ldc_R4 && in1.AsDouble() == 100.0) {
					// Old Code: 100f
					// New Code: PatchHelper.GetOutsideFrequency()
					i++;
					yield return new CodeInstruction(in1) {
						opcode = OpCodes.Call,
						operand = getOutsideFrequency
					};

				} else if (in1.opcode == OpCodes.Ldc_R4 && in1.AsDouble() == 15.0) {
					// Old Code: 15f
					// New Code: PatchHelper.GetInsideFrequency()
					i++;
					yield return new CodeInstruction(in1) {
						opcode = OpCodes.Call,
						operand = getInsideFrequency
					};
				}

				continue;
			}

			if (in0.Calls(GameLocation_isRainingHere))
				// Old Code: Game1.currentLocation.IsRainingHere()
				// New Code PatchHelper.HasMusic(Game1.currentLocation)
				yield return new CodeInstruction(in0) {
					opcode = OpCodes.Call,
					operand = hasMusic
				};

			else if (in0.LoadsConstant("rain")) {
				// Old Code: "rain"
				// New Code: PatchHelper.GetMusicOrDefault(null)
				yield return new CodeInstruction(in0) {
					opcode = OpCodes.Ldnull,
					operand = null
				};

				yield return new CodeInstruction(OpCodes.Call, getMusicOrDefault);

			} else
				yield return in0;

		}
	}

	private static IEnumerable<CodeInstruction> Game1_PlayMorningSong__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var isRainingHere = AccessTools.Method(typeof(Game1), nameof(Game1.IsRainingHere));
		var hasMusic = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.HasMusic));

		// This is... fragile. But what do we do about it?
		var playMorningSong = AccessTools.Method(typeof(Game1), "<playMorningSong>g__PlayAction|654_0");
		var playCustomWeatherMusic = AccessTools.Method(typeof(Music_Patches), nameof(PlayCustomWeatherMusic));

		foreach (var instr in instructions) {
			if (instr.Calls(isRainingHere))
				// Old Code: Game1.IsRainingHere(null)
				// New Code: PatchHelper.HasMusic(null)
				yield return new CodeInstruction(instr) {
					opcode = OpCodes.Call,
					operand = hasMusic
				};
			else if (instr.Calls(playMorningSong))
				// Old Code: PlayAction();
				// New Code: Music_Patches.PlayCustomWeatherMusic();
				yield return new CodeInstruction(instr) {
					opcode = OpCodes.Call,
					operand = playCustomWeatherMusic
				};
			else if (instr.opcode == OpCodes.Ldftn && instr.operand is MethodInfo minfo && minfo == playMorningSong)
				// Old Code: PlayAction
				// New Code: Music_Patches.PlayCustomWeatherMusic
				yield return new CodeInstruction(instr) {
					opcode = OpCodes.Ldftn,
					operand = playCustomWeatherMusic
				};
			else
				yield return instr;
		}
	}

	private static IEnumerable<CodeInstruction> Game1_GetDefaultSongPriority__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var getMusicOrDefault = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.GetMusicOrDefault));
		var Game1_instanceGameLocation = AccessTools.Field(typeof(Game1), nameof(Game1.instanceGameLocation));

		foreach (var instr in instructions) {
			if (instr.LoadsConstant("rain")) {
				// Old Code: "rain"
				// New Code: PatchHelper.GetMusicOrDefault(instance.instanceGameLocation)
				yield return new CodeInstruction(instr) {
					opcode = OpCodes.Ldarg_2,
					operand = null
				};

				yield return new CodeInstruction(OpCodes.Ldfld, Game1_instanceGameLocation);

				yield return new CodeInstruction(instr) {
					opcode = OpCodes.Call,
					operand = getMusicOrDefault
				};

			} else
				yield return instr;
		}

	}

	private static IEnumerable<CodeInstruction> Game1_UpdateMusic__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var isRainingHere = AccessTools.Method(typeof(Game1), nameof(Game1.IsRainingHere));
		var hasMusic = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.HasMusic));

		var getMusicOrDefault = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.GetMusicOrDefault));

		var getOutsideFrequency = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.GetOutsideFrequency));
		var getInsideFrequency = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.GetInsideFrequency));

		CodeInstruction[] instrs = instructions.ToArray();

		for (int i = 0; i < instrs.Length; i++) {
			var in0 = instrs[i];

			if (i + 1 < instrs.Length && in0.LoadsConstant("Frequency")) {
				yield return in0;

				var in1 = instrs[i + 1];
				if (in1.opcode == OpCodes.Ldc_R4 && in1.AsDouble() == 100.0) {
					// Old Code: Game1.currentSong.SetVariable("Frequency", 100f);
					// New Code: Game1.currentSong.SetVariable("Frequency", PatchHelper.GetOutsideFrequency());
					i++;
					yield return new CodeInstruction(in1) {
						opcode = OpCodes.Call,
						operand = getOutsideFrequency
					};

				} else if (in1.opcode == OpCodes.Ldc_R4 && in1.AsDouble() == 15.0) {
					// Old Code: Game1.currentSong.SetVariable("Frequency", 15f);
					// New Code: Game1.currentSong.SetVariable("Frequency", PatchHelper.GetInsideFrequency());
					i++;
					yield return new CodeInstruction(in1) {
						opcode = OpCodes.Call,
						operand = getInsideFrequency
					};
				}

				continue;
			}

			if (in0.Calls(isRainingHere))
				// Old Code: Game1.IsRainingHere(null)
				// New Code: PatchHelper.HasMusic(null)
				yield return new CodeInstruction(in0) {
					opcode = OpCodes.Call,
					operand = hasMusic
				};

			else if (in0.LoadsConstant("rain")) {
				// Old Code: "rain"
				// New Code: PatchHelper.GetMusicOrDefault(null)
				yield return new CodeInstruction(in0) {
					opcode = OpCodes.Ldnull,
					operand = null
				};
				yield return new CodeInstruction(OpCodes.Call, getMusicOrDefault);

			} else
				yield return in0;
		}

	}

	#endregion

	#region GameLocation Patches

	private static IEnumerable<CodeInstruction> GameLocation_checkForMusic__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var isRainingHere = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsRainingHere));
		var hasMusic = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.HasMusic));

		var getMusicOrDefault = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.GetMusicOrDefault));

		foreach (var instr in instructions) {

			if (instr.Calls(isRainingHere))
				// Old Code: this.IsRainingHere()
				// New Code: PatchHelper.HasMusic(this)
				yield return new CodeInstruction(instr) {
					opcode = OpCodes.Call,
					operand = hasMusic
				};

			else if (instr.LoadsConstant("rain")) {
				// Old Code: "rain"
				// New Code: PatchHelper.GetMusicOrDefault(this)
				yield return new CodeInstruction(instr) {
					opcode = OpCodes.Ldarg_0,
					operand = null
				};

				yield return new CodeInstruction(OpCodes.Call, getMusicOrDefault);

			} else
				yield return instr;
		}

	}

	private static IEnumerable<CodeInstruction> GameLocation_IsMiniJukeboxPlaying__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var isRainingHere = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsRainingHere));
		var our_isRainingHere = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.HasMusic));

		foreach (var instr in instructions) {
			if (instr.Calls(isRainingHere))
				// Old Code: this.IsRainingHere()
				// New Code: PatchHelper.HasMusic(this)
				yield return new CodeInstruction(instr) {
					opcode = OpCodes.Call,
					operand = our_isRainingHere
				};

			else
				yield return instr;
		}

	}

	private static IEnumerable<CodeInstruction> GameLocation_GetLocationSpecificMusic__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var isRainingHere = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsRainingHere));
		var our_isRainingHere = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.HasMusic));

		foreach (var instr in instructions) {
			if (instr.Calls(isRainingHere))
				// Old Code: this.IsRainingHere()
				// New Code: PatchHelper.HasMusic(this)
				yield return new CodeInstruction(instr) {
					opcode = OpCodes.Call,
					operand = our_isRainingHere
				};

			else
				yield return instr;
		}

	}

	private static IEnumerable<CodeInstruction> GameLocation_GetMorningSong__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var LocationWeather_get_IsRaining = AccessTools.PropertyGetter(typeof(LocationWeather), nameof(LocationWeather.IsRaining));

		var hasMusic = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.HasMusic));

		var getMusicOrDefault = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.GetMusicOrDefault));

		CodeInstruction[] instrs = instructions.ToArray();

		for(int i = 0; i < instrs.Length; i++) {
			var in0 = instrs[i];

			if (in0.LoadsConstant("rain")) {
				// Old Code: "rain"
				// New Code: PatchHelper.GetMusicOrDefault(this)
				yield return new CodeInstruction(in0) {
					opcode = OpCodes.Ldarg_0,
					operand = null
				};

				yield return new CodeInstruction(OpCodes.Call, getMusicOrDefault);
				continue;
			}

			if (i + 1 < instrs.Length && in0.opcode == OpCodes.Ldloc_0) {
				var in1 = instrs[i + 1];
				if (in1.Calls(LocationWeather_get_IsRaining)) {
					// Old Code: locationWeather.IsRaining
					// New Code: PatchHelper.HasMusic(this)
					yield return new CodeInstruction(in0) {
						opcode = OpCodes.Ldarg_0,
						operand = null
					};

					yield return new CodeInstruction(OpCodes.Call, hasMusic);

					i++;
					continue;
				}
			}

			yield return in0;
		}
	}

	private static IEnumerable<CodeInstruction> GameLocation_HandleMusicChange__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var Game1_isRainingHere = AccessTools.Method(typeof(Game1), nameof(Game1.IsRainingHere));
		var hasMusic = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.HasMusic));

		var getMusicOrDefault = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.GetMusicOrDefault));

		bool seen_first_rain = false;
		bool seen_first_raining_here = false;

		foreach (var instr in instructions) {
			if (instr.Calls(Game1_isRainingHere)) {
				if (seen_first_raining_here) {
					// Old Code: Game1.IsRainingHere(null)
					// New Code: PatchHelper.HasMusic(null)
					yield return new CodeInstruction(instr) {
						opcode = OpCodes.Call,
						operand = hasMusic
					};

				} else {
					// The first IsRainingHere check, we want to do a more involved check
					// to see if the location's weather music is not the same as the
					// currently playing track.

					// Old Code: if (!Game1.IsRainingHere(newLocation))
					// New Code: if (!(GetWeatherMusicOrDefault(newLocation) == currentTrack))

					yield return new CodeInstruction(instr) {
						opcode = OpCodes.Call,
						operand = getMusicOrDefault
					};

					yield return new CodeInstruction(OpCodes.Ldloc_0);

					yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Equals), [typeof(string), typeof(string)]));

					seen_first_raining_here = true;

				}

			} else if (instr.LoadsConstant("rain")) {
				if (seen_first_rain) {
					// Old Code: "rain"
					// New Code: PatchHelper.GetMusicOrDefault(newLocation)
					yield return new CodeInstruction(instr) {
						opcode = OpCodes.Ldarg_1,
						operand = null
					};

				} else {
					// The first rain check, we want to check the old location's music name.
					// Old Code: "rain"
					// New Code: PatchHelper.GetMusicOrDefault(oldLocation)
					yield return new CodeInstruction(instr) {
						opcode = OpCodes.Ldarg_0,
						operand = null
					};

					seen_first_rain = true;
				}

				yield return new CodeInstruction(OpCodes.Call, getMusicOrDefault);

			} else
				yield return instr;
		}

	}

	#endregion

}
