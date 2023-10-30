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
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using FlipBuildings.Utilities;

namespace FlipBuildings.Patches
{
	internal class BuildingPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(Building), nameof(Building.draw), new Type[] { typeof(SpriteBatch) }),
				transpiler: new HarmonyMethod(typeof(BuildingPatch), nameof(DrawTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(Building), nameof(Building.drawShadow), new Type[] { typeof(SpriteBatch), typeof(Int32), typeof(Int32) }),
				transpiler: new HarmonyMethod(typeof(BuildingPatch), nameof(DrawShadowTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(Building), nameof(Building.load)),
				postfix: new HarmonyMethod(typeof(BuildingPatch), nameof(LoadPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(Building), nameof(Building.getUpgradeSignLocation)),
				postfix: new HarmonyMethod(typeof(BuildingPatch), nameof(GetUpgradeSignLocationPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(Building), nameof(Building.getPorchStandingSpot)),
				postfix: new HarmonyMethod(typeof(BuildingPatch), nameof(GetPorchStandingSpotPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(Building), nameof(Building.getMailboxPosition)),
				postfix: new HarmonyMethod(typeof(BuildingPatch), nameof(GetMailboxPositionPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(Building), nameof(Building.intersects)),
				postfix: new HarmonyMethod(typeof(BuildingPatch), nameof(IntersectsPostfix))
			);
		}

		private static IEnumerable<CodeInstruction> DrawTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
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
					offset: 57,
					targetInstruction: new(OpCodes.Ldc_I4_S, (sbyte)92),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_S, (sbyte)100)
					},
					goNext: false
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 20,
					targetInstruction: new(OpCodes.Conv_R4),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Conv_R4),
						new(OpCodes.Ldc_R4, -1f),
						new(OpCodes.Mul)
					},
					goNext: false
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 15,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 61,
					targetInstruction: new(OpCodes.Ldc_I4_S, (sbyte)92),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_S, (sbyte)100)
					},
					goNext: false
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 20,
					targetInstruction: new(OpCodes.Conv_R4),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Conv_R4),
						new(OpCodes.Ldc_R4, -1f),
						new(OpCodes.Mul)
					},
					goNext: false
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 15,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 40,
					targetInstruction: new(OpCodes.Ldc_I4_S, (sbyte)92),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_S, (sbyte)100)
					},
					goNext: false
				),
				// null
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 15,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				)
			};
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, typeof(Building), nameof(Building.draw));
		}

		private static IEnumerable<CodeInstruction> DrawShadowTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 17,
					targetInstruction: new(OpCodes.Ldsfld, typeof(Building).GetField(nameof(Building.leftShadow), BindingFlags.Public | BindingFlags.Static)),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldsfld, typeof(Building).GetField(nameof(Building.rightShadow), BindingFlags.Public | BindingFlags.Static))
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
					offset: 2,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 17,
					targetInstruction: new(OpCodes.Ldsfld, typeof(Building).GetField(nameof(Building.rightShadow), BindingFlags.Public | BindingFlags.Static)),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldsfld, typeof(Building).GetField(nameof(Building.leftShadow), BindingFlags.Public | BindingFlags.Static))
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
				)
			};
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, typeof(Building), nameof(Building.drawShadow));
		}

		private static void LoadPostfix(Building __instance)
		{
			if (!__instance.modData.ContainsKey(ModDataKeys.FLIPPED))
				return;
			BuildingHelper.Update(__instance);
		}

		private static void GetUpgradeSignLocationPostfix(Building __instance, ref Vector2 __result)
		{
			if (!__instance.modData.ContainsKey(ModDataKeys.FLIPPED))
				return;
			if (__instance.indoors.Value != null && __instance.indoors.Value is Shed)
				__result = new Vector2((int)__instance.tileX.Value + 1, (int)__instance.tileY.Value + 1) * 64f + new Vector2(12f, -16f);
			else
				__result = new Vector2(((int)__instance.tileX.Value + __instance.tilesWide.Value) * 64 - 32, (int)__instance.tileY.Value * 64 - 32);
		}

		private static void GetPorchStandingSpotPostfix(Building __instance, ref Point __result)
		{
			if (!__instance.modData.ContainsKey(ModDataKeys.FLIPPED))
				return;
			if (__instance.isCabin)
				__result = new Point((int)__instance.tileX.Value + (int)__instance.tilesWide.Value - 2, (int)__instance.tileY.Value + (int)__instance.tilesHigh.Value - 1);
		}

		private static void GetMailboxPositionPostfix(Building __instance, ref Point __result)
		{
			if (!__instance.modData.ContainsKey(ModDataKeys.FLIPPED))
				return;
			if (__instance.isCabin)
				__result = new Point((int)__instance.tileX.Value, (int)__instance.tileY.Value + (int)__instance.tilesHigh.Value - 1);
		}

		private static void IntersectsPostfix(Building __instance, ref bool __result, Rectangle boundingBox)
		{
			if (!__instance.modData.ContainsKey(ModDataKeys.FLIPPED))
				return;
			if (__instance.isCabin && (int)__instance.daysOfConstructionLeft.Value <= 0)
				__result = new Rectangle(((int)__instance.tileX.Value) * 64, ((int)__instance.tileY.Value + (int)__instance.tilesHigh.Value - 1) * 64, 64, 64).Intersects(boundingBox) || new Rectangle((int)__instance.tileX.Value * 64, (int)__instance.tileY.Value * 64, (int)__instance.tilesWide.Value * 64, ((int)__instance.tilesHigh.Value - 1) * 64).Intersects(boundingBox);
		}
	}
}
