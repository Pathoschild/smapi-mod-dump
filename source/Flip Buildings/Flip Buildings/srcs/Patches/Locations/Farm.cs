/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/FlipBuildings
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using FlipBuildings.Utilities;

namespace FlipBuildings.Patches
{
	internal class FarmPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(Farm), nameof(Farm.draw), new Type[] { typeof(SpriteBatch) }),
				transpiler: new HarmonyMethod(typeof(FarmPatch), nameof(DrawTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(Farm), nameof(Farm.UpdateWhenCurrentLocation), new Type[] { typeof(GameTime) }),
				transpiler: new HarmonyMethod(typeof(FarmPatch), nameof(UpdateWhenCurrentLocationTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(Farm), nameof(Farm.GetMainFarmHouseEntry)),
				postfix: new HarmonyMethod(typeof(FarmPatch), nameof(GetMainFarmHouseEntryPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(Farm), nameof(Farm.GetHouseRect)),
				postfix: new HarmonyMethod(typeof(FarmPatch), nameof(GetHouseRectPostfix))
			);
		}

		private static IEnumerable<CodeInstruction> DrawTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 20,
					targetInstruction: new(OpCodes.Ldc_I4_5),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_3)
					},
					goNext: false
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 2,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 22,
					targetInstruction: new(OpCodes.Ldc_I4_5),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_3)
					},
					goNext: false
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 2,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 20,
					targetInstruction: new(OpCodes.Ldc_I4_3),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_5)
					},
					goNext: false
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 2,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 27,
					targetInstruction: new(OpCodes.Ldc_R4, 384f),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_R4, 192f)
					},
					goNext: false
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 7,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				)
			};
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, typeof(Farm), nameof(Farm.draw));
		}

		private static IEnumerable<CodeInstruction> UpdateWhenCurrentLocationTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(List<TemporaryAnimatedSprite>).GetMethod("Add")),
					offset: 63,
					targetInstruction: new(OpCodes.Ldfld, typeof(Point).GetField("X")),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldfld, typeof(Point).GetField("X")),
						new(OpCodes.Ldc_I4_1),
						new(OpCodes.Add)
					},
					goNext: false
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(List<TemporaryAnimatedSprite>).GetMethod("Add")),
					offset: 51,
					targetInstruction: new(OpCodes.Mul),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Mul),
						new(OpCodes.Ldc_I4_S, (sbyte)-1),
						new(OpCodes.Mul),
						new(OpCodes.Ldc_I4_S, (sbyte)20),
						new(OpCodes.Sub)
					}
				),
				new(
					instanceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Farm).GetField("buildings")),
						new(OpCodes.Ldloc_3),
						new(OpCodes.Callvirt, typeof(Netcode.NetCollection<Building>).GetMethod("get_Item", new[] { typeof(Int32) }))
					},
					instanceType: typeof(Building),
					referenceInstruction: new(OpCodes.Callvirt, typeof(List<TemporaryAnimatedSprite>).GetMethod("Add")),
					offset: 61,
					targetInstruction: new(OpCodes.Ldc_I4_4),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					},
					goNext: false
				),
				new(
					instanceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Farm).GetField("buildings")),
						new(OpCodes.Ldloc_3),
						new(OpCodes.Callvirt, typeof(Netcode.NetCollection<Building>).GetMethod("get_Item", new[] { typeof(Int32) }))
					},
					instanceType: typeof(Building),
					referenceInstruction: new(OpCodes.Callvirt, typeof(List<TemporaryAnimatedSprite>).GetMethod("Add")),
					offset: 57,
					targetInstruction: new(OpCodes.Ldc_I4_S, (sbyte)-20),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_S, (sbyte)20),
						new(OpCodes.Ldc_I4_S, (sbyte)20),
						new(OpCodes.Sub)
					}
				)
			};
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, typeof(Farm), nameof(Farm.UpdateWhenCurrentLocation));
		}

		private static void GetMainFarmHouseEntryPostfix(Farm __instance, ref Point __result)
		{
			if (!__instance.modData.ContainsKey(ModDataKeys.FLIPPED))
				return;
			__result = new Point(__instance.mainFarmhouseEntry.Value.X - 2, __instance.mainFarmhouseEntry.Value.Y);
		}

		private static void GetHouseRectPostfix(Farm __instance, ref Rectangle __result)
		{
			if (!__instance.modData.ContainsKey(ModDataKeys.FLIPPED))
				return;
			Point mainFarmHouseEntry = __instance.GetMainFarmHouseEntry();
			__result = new Rectangle(mainFarmHouseEntry.X - 3, mainFarmHouseEntry.Y - 4, 9, 6);
		}
	}
}
