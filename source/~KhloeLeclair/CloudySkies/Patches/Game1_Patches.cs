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
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Leclair.Stardew.CloudySkies.Models;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.CloudySkies.Patches; 

public static class Game1_Patches {

	private static ModEntry? Mod;

	public static void Patch(ModEntry mod) {
		Mod = mod;

		try {

			// Drawing

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(Game1), nameof(Game1.drawWeather)),
				prefix: new HarmonyMethod(typeof(Game1_Patches), nameof(drawWeather__Prefix))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(Game1), nameof(Game1.DrawWorld)),
				transpiler: new HarmonyMethod(typeof(Game1_Patches), nameof(Game1_DrawWorld__Transpiler))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(Game1), nameof(Game1.DrawLighting)),
				transpiler: new HarmonyMethod(typeof(Game1_Patches), nameof(DrawLighting__Transpiler))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(Game1), nameof(Game1.DrawLightmapOnScreen)),
				transpiler: new HarmonyMethod(typeof(Game1_Patches), nameof(DrawLightmapOnScreen__Transpiler))
			);

			// Updating

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(Game1), nameof(Game1.performTenMinuteClockUpdate)),
				transpiler: new HarmonyMethod(typeof(Game1_Patches), nameof(performTenMinuteClockUpdate__Transpiler))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(Game1), nameof(Game1.UpdateGameClock)),
				transpiler: new HarmonyMethod(typeof(Game1_Patches), nameof(UpdateGameClock__Transpiler))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(Game1), nameof(Game1.updateWeather)),
				prefix: new HarmonyMethod(typeof(Game1_Patches), nameof(updateWeather__Prefix))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(Game1), nameof(Game1.updateRaindropPosition)),
				prefix: new HarmonyMethod(typeof(Game1_Patches), nameof(updateRaindropPosition__Prefix))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(Game1), nameof(Game1.updateRainDropPositionForPlayerMovement)),
				prefix: new HarmonyMethod(typeof(Game1_Patches), nameof(updateRaindropPositionForPlayerMovement__Prefix))
			);

		} catch(Exception ex) {
			mod.Log($"Error patching Game1. Weather will not work correctly.", StardewModdingAPI.LogLevel.Error, ex);
		}

	}

	#region Drawing

	private static IEnumerable<CodeInstruction> DrawLighting__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var GameLocation_isRainingHere = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsRainingHere));
		var hasAmbientColor = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.HasAmbientColor));

		foreach (var in0 in instructions) {
			if (in0.Calls(GameLocation_isRainingHere))
				// Old Code: Game1.currentLocation.IsRainingHere()
				// New Code: PatchHelper.HasAmbientColor(Game1.currentLocation)
				yield return new CodeInstruction(in0) {
					opcode = OpCodes.Call,
					operand = hasAmbientColor
				};

			else
				yield return in0;
		}

	}

	private static IEnumerable<CodeInstruction> DrawLightmapOnScreen__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var GameLocation_isRainingHere = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsRainingHere));
		var hasLightingTint = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.HasLightingTint));

		var getLightingTint = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.GetLightingTint));

		var Color_OrangeRed = AccessTools.PropertyGetter(typeof(Color), nameof(Color.OrangeRed));

		CodeInstruction[] instrs = instructions.ToArray();

		for(int i = 0; i < instrs.Length; i++) {
			var in0 = instrs[i];

			if (i + 2 < instrs.Length) {
				var in1 = instrs[i + 1];
				var in2 = instrs[i + 2];

				if (in0.Calls(Color_OrangeRed) && in1.AsDouble() == 0.45) {
					// Old Code: Color.OrangeRed * 0.45f
					// New Code: Game1_Patches.GetWeatherLightingTint()
					yield return new CodeInstruction(in0) {
						opcode = OpCodes.Ldnull,
						operand = null
					};
					yield return new CodeInstruction(OpCodes.Call, getLightingTint);

					// Skip the color multiplication.
					i += 2;
				}

			}

			if (in0.Calls(GameLocation_isRainingHere))
				// Old Code: Game1.currentLocation.IsRainingHere()
				// New Code: PatchHelper.HasLightingTint(Game1.currentLocation)
				yield return new CodeInstruction(in0) {
					opcode = OpCodes.Call,
					operand = hasLightingTint
				};

			else
				yield return in0;
		}
	}

	private static IEnumerable<CodeInstruction> Game1_DrawWorld__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var GameLocation_isRainingHere = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsRainingHere));
		var hasPostLightingTint = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.HasPostLightingTint));

		var getPostLightingTint = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.GetPostLightingTint));

		var Color_Blue = AccessTools.PropertyGetter(typeof(Color), nameof(Color.Blue));

		CodeInstruction[] instrs = instructions.ToArray();

		bool seen_raining = false;

		for(int i = 0; i < instrs.Length; i++) {
			var in0 = instrs[i];

			if (in0.Calls(GameLocation_isRainingHere)) {
				seen_raining = true;
				yield return new CodeInstruction(in0) {
					opcode = OpCodes.Call,
					operand = hasPostLightingTint
				};

				continue;
			}

			if (seen_raining && i + 5 < instrs.Length) {
				var in1 = instrs[i + 1];
				var in2 = instrs[i + 2];
				var in3 = instrs[i + 3];
				var in4 = instrs[i + 4];
				var in5	= instrs[i + 5];

				if (in0.Calls(Color_Blue) && in1.opcode == OpCodes.Ldc_R4) {
					// Old Code: Color.Blue * 0.2f
					// New Code: PatchHelper.GetPostLightingTint(null)
					yield return new CodeInstruction(in0) {
						opcode = OpCodes.Ldnull,
						operand = null
					};

					yield return new CodeInstruction(OpCodes.Call, getPostLightingTint);

					// Skip the color multiplication and stuff.
					i += 2;
					continue;
				}

				if (in0.LoadsConstant(0) &&
					in1.LoadsConstant(120) &&
					in2.LoadsConstant(150) &&
					in3.opcode == OpCodes.Newobj && in3.operand is ConstructorInfo cinfo && cinfo.DeclaringType == typeof(Color) &&
					in4.opcode == OpCodes.Ldc_R4
				) {
					// Old Code: new Color(0, 120, 150) * 0.22f
					// New Code: PatchHelper.GetPostLightingTint(null)
					yield return new CodeInstruction(in0) {
						opcode = OpCodes.Ldnull,
						operand = null
					};

					yield return new CodeInstruction(OpCodes.Call, getPostLightingTint);

					// Skip the color multiplication and stuff.
					i += 5;
					continue;
				}

			}

			yield return in0;
		}

	}

	private static bool drawWeather__Prefix(Game1 __instance, GameTime time, RenderTarget2D target_screen) {
		try {
			if (Mod is not null && Mod.DrawWeather(__instance, time, target_screen))
				return false;

		} catch (Exception ex) {
			Mod?.Log($"Error drawing weather: {ex}", StardewModdingAPI.LogLevel.Error, once: true);
		}

		return true;
	}

	#endregion

	#region Updates

	private static IEnumerable<CodeInstruction> performTenMinuteClockUpdate__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var GameLocation_IsRainingHere = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsRainingHere));
		var hasAmbientColor = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.HasAmbientColor));
		var hasMusic = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.HasMusic));

		var AmbientLight = AccessTools.Field(typeof(Game1), nameof(Game1.ambientLight));

		bool seen_ambient_light = false;

		foreach(var in0 in instructions) {

			if (in0.LoadsField(AmbientLight))
				seen_ambient_light = true;

			if (in0.Calls(GameLocation_IsRainingHere)) {
				if (seen_ambient_light)
					// If we have already seen the Game1.ambientLight access,
					// then we're moving on to checking the music rather
					// than the ambient light, so switch which method we call.
					yield return new CodeInstruction(in0) {
						opcode = OpCodes.Call,
						operand = hasMusic
					};

				else
					yield return new CodeInstruction(in0) {
						opcode = OpCodes.Call,
						operand = hasAmbientColor
					};

			} else
				yield return in0;
		}

	}

	private static IEnumerable<CodeInstruction> UpdateGameClock__Transpiler(IEnumerable<CodeInstruction> instructions) {

		var Game1_IsRainingHere = AccessTools.Method(typeof(Game1), nameof(Game1.IsRainingHere));
		var hasAmbientColor = AccessTools.Method(typeof(PatchHelper), nameof(PatchHelper.HasAmbientColor));

		foreach (var in0 in instructions) {

			if (in0.Calls(Game1_IsRainingHere))
				// Old Code: Game1.IsRainingHere(null)
				// New Code: PatchHelper.HasAmbientColor(null)
				yield return new CodeInstruction(in0) {
					opcode = OpCodes.Call,
					operand = hasAmbientColor
				};

			else
				yield return in0;
		}

	}


	private static bool updateRaindropPosition__Prefix() {
		try {
			if (Mod is not null && Mod.MoveWithViewport())
				return false;

		} catch(Exception ex) {
			Mod?.Log($"Error moving weather: {ex}", StardewModdingAPI.LogLevel.Error, once: true);
		}

		return true;
	}


	private static bool updateRaindropPositionForPlayerMovement__Prefix(int direction, float speed) {
		try {
			if (Mod is not null && Mod.MoveWithPlayer(direction, speed))
				return false;

		} catch (Exception ex) {
			Mod?.Log($"Error moving weather: {ex}", StardewModdingAPI.LogLevel.Error, once: true);
		}

		return true;
	}


	private static bool updateWeather__Prefix(GameTime time) {
		try {
			if (Mod is not null && Mod.UpdateWeather(time))
				return false;

		} catch (Exception ex) {
			Mod?.Log($"Error updating weather: {ex}", StardewModdingAPI.LogLevel.Error, once: true);
		}

		return true;
	}

	#endregion

}
