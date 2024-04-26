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
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;

using StardewValley;
using StardewValley.Objects;

namespace Leclair.Stardew.CloudySkies.Patches;

public static class GameLocation_Patches {

	private static ModEntry? Mod;

	public static void Patch(ModEntry mod) {
		Mod = mod;

		try {

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.addClouds)),
				transpiler: new HarmonyMethod(typeof(GameLocation_Patches), nameof(addClouds__Transpiler))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.addFrog)),
				transpiler: new HarmonyMethod(typeof(GameLocation_Patches), nameof(addFrog__Transpiler))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.ResetForEvent)),
				transpiler: new HarmonyMethod(typeof(GameLocation_Patches), nameof(ResetForEvent__Transpiler))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), "resetLocalState"),
				transpiler: new HarmonyMethod(typeof(GameLocation_Patches), nameof(resetLocalState__Transpiler))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.tryToAddCritters)),
				transpiler: new HarmonyMethod(typeof(GameLocation_Patches), nameof(tryToAddCritters__Transpiler))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), "_updateAmbientLighting"),
				transpiler: new HarmonyMethod(typeof(GameLocation_Patches), nameof(updateAmbientLighting__Transpiler))
			);

		} catch(Exception ex) {

			mod.Log($"Error patching GameLocation. Weather will not work correctly.", StardewModdingAPI.LogLevel.Error, ex);

		}

		try {

			foreach(var method in new MethodInfo[] {
				 AccessTools.Method(typeof(Furniture), nameof(Furniture.OnAdded)),
				 AccessTools.Method(typeof(Furniture), nameof(Furniture.timeToTurnOnLights)),
				 AccessTools.Method(typeof(Furniture), nameof(Furniture.DayUpdate)),
				 AccessTools.Method(typeof(Furniture), nameof(Furniture.removeLights))
			})
				mod.Harmony.Patch(
					original: method,
					transpiler: new HarmonyMethod(typeof(GameLocation_Patches), nameof(Furniture_Rain__Transpiler))
				);

		} catch(Exception ex) {
			mod.Log($"Error patching Furniture.", StardewModdingAPI.LogLevel.Error, ex);
		}

	}

	#region Helper Methods

	internal static bool UseNightTiles(GameLocation? location) {
		return PatchHelper.GetWeatherData(location)?.UseNightTiles ?? Game1.IsRainingHere(location);
	}

	internal static bool ShouldNotSpawnCritters(GameLocation? location) {
		var wd = PatchHelper.GetWeatherData(location);
		return wd is null ? Game1.IsRainingHere(location) : !wd.SpawnCritters;
	}

	internal static bool ShouldSpawnFrogs(GameLocation? location) {
		var wd = PatchHelper.GetWeatherData(location);
		return wd?.SpawnFrogs ?? Game1.IsRainingHere(location);
	}

	internal static bool ShouldNotSpawnClouds(GameLocation? location) {
		var wd = PatchHelper.GetWeatherData(location);
		if (wd is not null && wd.SpawnClouds.HasValue)
			return !wd.SpawnClouds.Value;

		return !(location ?? Game1.currentLocation).IsSummerHere() || Game1.IsRainingHere(location) || Game1.weatherIcon == 4;
	}

	#endregion

	private static IEnumerable<CodeInstruction> Furniture_Rain__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var GameLocation_IsRainingHere = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsRainingHere));
		var useNightTiles = AccessTools.Method(typeof(GameLocation_Patches), nameof(UseNightTiles));

		foreach (var in0 in instructions) {

			if (in0.Calls(GameLocation_IsRainingHere))
				// Old Code: loc.IsRainingHere()
				// New Code: UseNightTiles(loc)
				yield return new CodeInstruction(in0) {
					opcode = OpCodes.Call,
					operand = useNightTiles
				};
			else
				yield return in0;
		}

	}

	private static IEnumerable<CodeInstruction> addClouds__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var GameLocation_IsSummerHere = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsSummerHere));
		var GameLocation_IsRainingHere = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsRainingHere));
		var Game1_weatherIcon = AccessTools.Field(typeof(Game1), nameof(Game1.weatherIcon));

		var shouldNotSpawnClouds = AccessTools.Method(typeof(GameLocation_Patches), nameof(ShouldNotSpawnClouds));

		CodeInstruction[] instrs = instructions.ToArray();

		for (int i = 0; i < instrs.Length; i++) {
			var in0 = instrs[i];

			if (i + 2 < instrs.Length) { 
				var in1 = instrs[i + 1];
				var in2 = instrs[i + 2];

				if (in0.opcode == OpCodes.Ldarg_0 && in1.Calls(GameLocation_IsSummerHere) && in2.opcode == OpCodes.Brfalse) {
					// Old Code: !this.IsSummerHere()
					// New Code:
					// Skip this whole equality check. We don't care. Our other
					// check already handled it.
					i += 2;
					continue;
				}

				if (in0.LoadsField(Game1_weatherIcon) && in1.LoadsConstant(4) && in2.opcode == OpCodes.Beq) {
					// Old Code: Game1.weatherIcon == 4
					// New Code:
					// Skip this whole equality check. We don't care. Our other
					// check already handled it.
					i += 2;
					continue;
				}
			}

			if (in0.Calls(GameLocation_IsRainingHere))
				// Old Code: this.IsRainingHere()
				// New Code: ShouldSpawnClouds(this)
				yield return new CodeInstruction(in0) {
					opcode = OpCodes.Call,
					operand = shouldNotSpawnClouds
				};

			else
				yield return in0;
		}
	}

	private static IEnumerable<CodeInstruction> addFrog__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var GameLocation_IsRainingHere = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsRainingHere));

		var shouldSpawnFrogs = AccessTools.Method(typeof(GameLocation_Patches), nameof(ShouldSpawnFrogs));

		foreach(var in0 in instructions) {

			if (in0.Calls(GameLocation_IsRainingHere))
				yield return new CodeInstruction(in0) {
					opcode = OpCodes.Call,
					operand = shouldSpawnFrogs
				};

			else
				yield return in0;

		}

	}

	private static IEnumerable<CodeInstruction> ResetForEvent__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var GameLocation_IsRainingHere = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsRainingHere));

		var hasAmbientColor = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.HasAmbientColor));
		var getAmbientColor = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.GetAmbientColor));

		CodeInstruction[] instrs = instructions.ToArray();

		for (int i = 0; i < instrs.Length; i++) {
			var in0 = instrs[i];

			if (i + 3 < instrs.Length) {
				var in1 = instrs[i + 1];
				var in2 = instrs[i + 2];
				var in3 = instrs[i + 3];

				if (in0.LoadsConstant(255) &&
					in1.LoadsConstant(200) &&
					in2.LoadsConstant(80) &&
					in3.opcode == OpCodes.Newobj && in3.operand is ConstructorInfo cinfo && cinfo.DeclaringType == typeof(Color)
				) {
					// Old Code: new Color(255, 200, 80)
					// New Code: PatchHelper.GetAmbientColor(this)
					yield return new CodeInstruction(in0) {
						opcode = OpCodes.Ldarg_0,
						operand = null
					};

					yield return new CodeInstruction(OpCodes.Call, getAmbientColor);

					// Skip the whole new Color(...)
					i += 3;
					continue;
				}
			}

			if (in0.Calls(GameLocation_IsRainingHere))
				// Old Code: this.IsRainingHere()
				// New Code: PatchHelper.HasAmbientColor(this)
				yield return new CodeInstruction(in0) {
					opcode = OpCodes.Call,
					operand = hasAmbientColor
				};

			else
				yield return in0;
		}


	}

	private static IEnumerable<CodeInstruction> resetLocalState__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var Game1_isRaining = AccessTools.Field(typeof(Game1), nameof(Game1.isRaining));
		var GameLocation_IsRainingHere = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsRainingHere));

		var useNightTiles = AccessTools.Method(typeof(GameLocation_Patches), nameof(UseNightTiles));

		foreach(var in0 in instructions) {

			if (in0.LoadsField(Game1_isRaining)) {
				// Old Code: Game1.isRaining
				// New Code: UseNightTiles(this)
				yield return new CodeInstruction(in0) {
					opcode = OpCodes.Ldarg_0,
					operand = null
				};

				yield return new CodeInstruction(OpCodes.Call, useNightTiles);

			} else if (in0.Calls(GameLocation_IsRainingHere)) {
				// Old Code: this.IsRainingHere()
				// New Code: UseNightTiles(this)
				yield return new CodeInstruction(in0) {
					opcode = OpCodes.Call,
					operand = useNightTiles
				};

			} else
				yield return in0;
		}

	}

	private static IEnumerable<CodeInstruction> tryToAddCritters__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var GameLocation_IsRainingHere = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsRainingHere));
		var shouldNotSpawnCritters = AccessTools.Method(typeof(GameLocation_Patches), nameof(ShouldNotSpawnCritters));

		foreach (var in0 in instructions) {

			if (in0.Calls(GameLocation_IsRainingHere))
				// Old Code: this.IsRainingHere()
				// New Code: SkipSpawnCritters(this)
				yield return new CodeInstruction(in0) {
					opcode = OpCodes.Call,
					operand = shouldNotSpawnCritters
				};

			else
				yield return in0;
		}

	}

	private static IEnumerable<CodeInstruction> updateAmbientLighting__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var GameLocation_IsRainingHere = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsRainingHere));

		var hasAmbientColor = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.HasAmbientColor));
		var getAmbientColor = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.GetAmbientColor));

		CodeInstruction[] instrs = instructions.ToArray();

		for(int i = 0; i < instrs.Length; i++) {
			var in0 = instrs[i];

			if (i + 3 < instrs.Length) {
				var in1 = instrs[i + 1];
				var in2 = instrs[i + 2];
				var in3 = instrs[i + 3];

				if (in0.LoadsConstant(255) &&
					in1.LoadsConstant(200) &&
					in2.LoadsConstant(80) &&
					in3.opcode == OpCodes.Newobj && in3.operand is ConstructorInfo cinfo && cinfo.DeclaringType == typeof(Color)
				) {
					// Old Code: new Color(255, 200, 80)
					// New Code: PatchHelper.GetAmbientColor(this)
					yield return new CodeInstruction(in0) {
						opcode = OpCodes.Ldarg_0,
						operand = null
					};

					yield return new CodeInstruction(OpCodes.Call, getAmbientColor);

					// Skip the whole new Color(...)
					i += 3;
					continue;
				}
			}

			if (in0.Calls(GameLocation_IsRainingHere))
				// Old Code: this.IsRainingHere()
				// New Code: PatchHelper.HasAmbientColor(this)
				yield return new CodeInstruction(in0) {
					opcode = OpCodes.Call,
					operand = hasAmbientColor
				};

			else
				yield return in0;
		}

	}

}
