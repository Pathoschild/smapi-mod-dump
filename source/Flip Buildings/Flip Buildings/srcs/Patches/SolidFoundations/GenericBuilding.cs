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
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Netcode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using FlipBuildings.Utilities;

namespace FlipBuildings.Patches.SF
{
	internal class GenericBuildingPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(CompatibilityHelper.GenericBuildingType, "draw", new Type[] { typeof(SpriteBatch) }),
				transpiler: new HarmonyMethod(typeof(GenericBuildingPatch), nameof(DrawTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityHelper.GenericBuildingType, "drawShadow", new Type[] { typeof(SpriteBatch), typeof(Int32), typeof(Int32) }),
				transpiler: new HarmonyMethod(typeof(GenericBuildingPatch), nameof(DrawShadowTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityHelper.GenericBuildingType, "drawBackground", new Type[] { typeof(SpriteBatch) }),
				transpiler: new HarmonyMethod(typeof(GenericBuildingPatch), nameof(DrawBackgroundTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityHelper.GenericBuildingType, "LoadFromBuildingData", new Type[] { typeof(Boolean) }),
				transpiler: new HarmonyMethod(typeof(GenericBuildingPatch), nameof(LoadFromBuildingDataTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityHelper.GenericBuildingType, "doAction", new Type[] { typeof(Vector2), typeof(Farmer) }),
				transpiler: new HarmonyMethod(typeof(GenericBuildingPatch), nameof(DoActionTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityHelper.GenericBuildingType, "doesTileHaveProperty", new Type[] { typeof(Int32), typeof(Int32), typeof(string), typeof(string), typeof(string).MakeByRefType() }),
				transpiler: new HarmonyMethod(typeof(GenericBuildingPatch), nameof(DoesTileHavePropertyTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityHelper.GenericBuildingType, "isActionableTile", new Type[] { typeof(Int32), typeof(Int32), typeof(Farmer) }),
				transpiler: new HarmonyMethod(typeof(GenericBuildingPatch), nameof(IsActionableTileTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityHelper.GenericBuildingType, "IsAuxiliaryTile", new Type[] { typeof(Vector2) }),
				transpiler: new HarmonyMethod(typeof(GenericBuildingPatch), nameof(IsAuxiliaryTileTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityHelper.GenericBuildingType, "getRectForAnimalDoor"),
				transpiler: new HarmonyMethod(typeof(GenericBuildingPatch), nameof(GetRectForAnimalDoorTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityHelper.GenericBuildingType, "updateInteriorWarps", new Type[] { typeof(GameLocation) }),
				transpiler: new HarmonyMethod(typeof(GenericBuildingPatch), nameof(UpdateInteriorWarpsTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityHelper.GenericBuildingType, "Update", new Type[] { typeof(GameTime) }),
				transpiler: new HarmonyMethod(typeof(GenericBuildingPatch), nameof(UpdateTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityHelper.GenericBuildingType, "UpdateBackport", new Type[] { typeof(GameTime) }),
				transpiler: new HarmonyMethod(typeof(GenericBuildingPatch), nameof(UpdateBackportTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityHelper.GenericBuildingType, "isTilePassable", new Type[] { typeof(Vector2) }),
				transpiler: new HarmonyMethod(typeof(GenericBuildingPatch), nameof(IsTilePassableTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityHelper.GenericBuildingType, "IsInTilePropertyRadius", new Type[] { typeof(Vector2) }),
				transpiler: new HarmonyMethod(typeof(GenericBuildingPatch), nameof(IsInTilePropertyRadiusTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityHelper.GenericBuildingType, "ResetLights"),
				transpiler: new HarmonyMethod(typeof(GenericBuildingPatch), nameof(ResetLightsTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityHelper.GenericBuildingType, "getUpgradeSignLocation"),
				transpiler: new HarmonyMethod(typeof(GenericBuildingPatch), nameof(GetUpgradeSignLocationTranspiler))
			);
		}

		private static IEnumerable<CodeInstruction> DrawTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_0),
						new(OpCodes.Call, CompatibilityHelper.GenericBuildingType.GetProperty("Model").GetGetMethod()),
						new(OpCodes.Ldfld, CompatibilityHelper.BuildingDataType.GetField("DrawOffset")),
						new(OpCodes.Ldc_R4, 4f)
					},
					offset: 0,
					targetInstruction: new(OpCodes.Ldc_R4, 4f),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldfld, typeof(Vector2).GetField("X")),
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Call, CompatibilityHelper.GenericBuildingType.GetProperty("Model").GetGetMethod()),
						new(OpCodes.Ldfld, CompatibilityHelper.ExtendedBuildingModelType.GetField("DrawOffset")),
						new(OpCodes.Ldfld, typeof(Vector2).GetField("Y")),
						new(OpCodes.Newobj, typeof(Vector2).GetConstructor(new Type[] { typeof(Single), typeof(Single) })),
						new(OpCodes.Ldc_R4, 4f)
					}
				),
				new(
					referenceInstruction: new(OpCodes.Newobj, typeof(Vector2).GetConstructor(new Type[] { typeof(Single), typeof(Single) })),
					offset: 13,
					targetInstruction: new(OpCodes.Conv_R4),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Conv_R4)
					}
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 21,
					targetInstruction: new(OpCodes.Call, typeof(Vector2).GetMethod("op_Addition", new Type[] { typeof(Vector2), typeof(Vector2) })),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldfld, typeof(Vector2).GetField("X")),
						new(OpCodes.Ldc_R4, -1f),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Conv_R4),
						new(OpCodes.Ldc_R4, 16f),
						new(OpCodes.Mul),
						new(OpCodes.Add),
						new(OpCodes.Ldloc_S, (sbyte)13),
						new(OpCodes.Ldfld, typeof(Rectangle).GetField("Width")),
						new(OpCodes.Sub),
						new(OpCodes.Ldloc_S, (sbyte)12),
						new(OpCodes.Ldflda, CompatibilityHelper.ExtendedBuildingDrawLayerType.GetField("DrawPosition")),
						new(OpCodes.Ldfld, typeof(Vector2).GetField("Y")),
						new(OpCodes.Newobj, typeof(Vector2).GetConstructor(new Type[] { typeof(Single), typeof(Single) })),
						new(OpCodes.Call, typeof(Vector2).GetMethod("op_Addition", new Type[] { typeof(Vector2), typeof(Vector2) }))
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
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 43,
					targetInstruction: new(OpCodes.Ldfld, typeof(Vector2).GetField("X")),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldfld, typeof(Vector2).GetField("X")),
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
						new(OpCodes.Ldc_I4_1),
						new(OpCodes.Sub)
					},
					goNext: false
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 22,
					targetInstruction: new(OpCodes.Ldloc_S, (sbyte)27),
					checkOperand: false,
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldloc_S, (sbyte)27),
						new(OpCodes.Ldc_R4, 16f),
						new(OpCodes.Sub),
					},
					goNext: false
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 4,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 33,
					targetInstruction: new(OpCodes.Ldc_R4, 4f),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_R4, 0f),
					}
				),
				// null
				new(
					referenceInstruction: new(OpCodes.Newobj, typeof(Vector2).GetConstructor(new Type[] { typeof(Single), typeof(Single) })),
					offset: 13,
					targetInstruction: new(OpCodes.Conv_R4),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Conv_R4)
					}
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 21,
					targetInstruction: new(OpCodes.Call, typeof(Vector2).GetMethod("op_Addition", new Type[] { typeof(Vector2), typeof(Vector2) })),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldfld, typeof(Vector2).GetField("X")),
						new(OpCodes.Ldc_R4, -1f),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Conv_R4),
						new(OpCodes.Ldc_R4, 16f),
						new(OpCodes.Mul),
						new(OpCodes.Add),
						new(OpCodes.Ldloc_S, (sbyte)32),
						new(OpCodes.Ldfld, typeof(Rectangle).GetField("Width")),
						new(OpCodes.Sub),
						new(OpCodes.Ldloc_S, (sbyte)31),
						new(OpCodes.Ldflda, CompatibilityHelper.ExtendedBuildingDrawLayerType.GetField("DrawPosition")),
						new(OpCodes.Ldfld, typeof(Vector2).GetField("Y")),
						new(OpCodes.Newobj, typeof(Vector2).GetConstructor(new Type[] { typeof(Single), typeof(Single) })),
						new(OpCodes.Call, typeof(Vector2).GetMethod("op_Addition", new Type[] { typeof(Vector2), typeof(Vector2) }))
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
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, CompatibilityHelper.GenericBuildingType, "draw");
		}

		private static IEnumerable<CodeInstruction> DrawShadowTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 18,
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
					offset: 18,
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
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, CompatibilityHelper.GenericBuildingType, "drawShadow");
		}

		private static IEnumerable<CodeInstruction> DrawBackgroundTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldflda, CompatibilityHelper.BuildingDrawLayerType.GetField("AnimalDoorOffset")),
						new(OpCodes.Ldfld, typeof(Point).GetField("X")),
						new(OpCodes.Conv_R4)
					},
					offset: 0,
					targetInstruction: new(OpCodes.Conv_R4),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Conv_R4)
					}
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 21,
					targetInstruction: new(OpCodes.Call, typeof(Vector2).GetMethod("op_Addition", new Type[] { typeof(Vector2), typeof(Vector2) })),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldfld, typeof(Vector2).GetField("X")),
						new(OpCodes.Ldc_R4, -1f),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Conv_R4),
						new(OpCodes.Ldc_R4, 16f),
						new(OpCodes.Mul),
						new(OpCodes.Add),
						new(OpCodes.Ldloc_S, (sbyte)5),
						new(OpCodes.Ldfld, typeof(Rectangle).GetField("Width")),
						new(OpCodes.Sub),
						new(OpCodes.Ldloc_S, (sbyte)4),
						new(OpCodes.Ldflda, CompatibilityHelper.ExtendedBuildingDrawLayerType.GetField("DrawPosition")),
						new(OpCodes.Ldfld, typeof(Vector2).GetField("Y")),
						new(OpCodes.Newobj, typeof(Vector2).GetConstructor(new Type[] { typeof(Single), typeof(Single) })),
						new(OpCodes.Call, typeof(Vector2).GetMethod("op_Addition", new Type[] { typeof(Vector2), typeof(Vector2) }))
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
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, CompatibilityHelper.GenericBuildingType, "drawBackground");
		}

		private static IEnumerable<CodeInstruction> LoadFromBuildingDataTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_0),
						new(OpCodes.Call, CompatibilityHelper.GenericBuildingType.GetProperty("Model").GetGetMethod()),
						new(OpCodes.Callvirt, CompatibilityHelper.BuildingDataType.GetMethod("GetAnimalDoorRect"))
					},
					offset: 4,
					isNegativeOffset: false,
					targetInstruction: new(OpCodes.Callvirt,typeof(NetFieldBase<Point, NetPoint>).GetProperty("Value").GetSetMethod()),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldfld, typeof(Rectangle).GetField("X")),
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
						new(OpCodes.Ldc_I4_1),
						new(OpCodes.Sub),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Call, CompatibilityHelper.GenericBuildingType.GetProperty("Model").GetGetMethod()),
						new(OpCodes.Callvirt, CompatibilityHelper.BuildingDataType.GetMethod("GetAnimalDoorRect")),
						new(OpCodes.Ldfld, typeof(Rectangle).GetField("Y")),
						new(OpCodes.Newobj, typeof(Point).GetConstructor(new Type[] { typeof(Int32), typeof(Int32) })),
						new(OpCodes.Callvirt,typeof(NetFieldBase<Point, NetPoint>).GetProperty("Value").GetSetMethod()),
					}
				)
			};
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, CompatibilityHelper.GenericBuildingType, "LoadFromBuildingData");
		}

		private static IEnumerable<CodeInstruction> DoActionTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tileX")),
						new(OpCodes.Call, typeof(NetFieldBase<int, NetInt>).GetMethod("op_Implicit")),
						new(OpCodes.Add)
					},
					offset: 4,
					targetInstruction: new(OpCodes.Callvirt, typeof(NetPoint).GetProperty("X").GetGetMethod()),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Callvirt, typeof(NetPoint).GetProperty("X").GetGetMethod()),
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
						new(OpCodes.Ldc_I4_1),
						new(OpCodes.Sub)
					}
				),
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tileX")),
						new(OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Sub)
					},
					offset: 0,
					targetInstruction: new(OpCodes.Sub),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Sub),
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
						new(OpCodes.Ldc_I4_1),
						new(OpCodes.Sub)
					}
				),
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldarg_1),
						new(OpCodes.Ldarg_2),
						new(OpCodes.Call, typeof(Building).GetMethod(nameof(Building.doAction)))
					},
					offset: 2,
					targetInstruction: new(OpCodes.Ldarg_1),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_1),
						new(OpCodes.Ldfld, typeof(Vector2).GetField("X")),
						new(OpCodes.Conv_I4),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tileX")),
						new(OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Sub),
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
						new(OpCodes.Ldc_I4_1),
						new(OpCodes.Sub),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tileX")),
						new(OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
						new(OpCodes.Ldarg_1),
						new(OpCodes.Ldfld, typeof(Vector2).GetField("Y")),
						new(OpCodes.Conv_I4),
						new(OpCodes.Newobj, typeof(Vector2).GetConstructor(new Type[] { typeof(Single), typeof(Single) }))
					}
				)
			};
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, CompatibilityHelper.GenericBuildingType, "doAction");
		}

		private static IEnumerable<CodeInstruction> DoesTileHavePropertyTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tileX")),
						new(OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Sub),
					},
					offset: 0,
					targetInstruction: new(OpCodes.Sub),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Sub),
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
						new(OpCodes.Ldc_I4_1),
						new(OpCodes.Sub)
					}
				)
			};
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, CompatibilityHelper.GenericBuildingType, "doesTileHaveProperty");
		}

		private static IEnumerable<CodeInstruction> IsActionableTileTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldarg_1),
						new(OpCodes.Ldarg_2),
						new(OpCodes.Ldarg_3),
						new(OpCodes.Call, typeof(Building).GetMethod(nameof(Building.isActionableTile)))
					},
					offset: 3,
					targetInstruction: new(OpCodes.Ldarg_1),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_1),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tileX")),
						new(OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Sub),
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
						new(OpCodes.Ldc_I4_1),
						new(OpCodes.Sub),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tileX")),
						new(OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
					}
				)
			};
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, CompatibilityHelper.GenericBuildingType, "isActionableTile");
		}

		private static IEnumerable<CodeInstruction> IsAuxiliaryTileTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tileX")),
						new(OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
					},
					offset: 4,
					targetInstruction: new(OpCodes.Ldfld, typeof(Point).GetField("X")),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldfld, typeof(Point).GetField("X")),
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
						new(OpCodes.Ldc_I4_1),
						new(OpCodes.Sub)
					}
				)
			};
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, CompatibilityHelper.GenericBuildingType, "IsAuxiliaryTile");
		}

		private static IEnumerable<CodeInstruction> GetRectForAnimalDoorTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tileX")),
						new(OpCodes.Call, typeof(NetFieldBase<int, NetInt>).GetMethod("op_Implicit")),
						new(OpCodes.Add)
					},
					offset: 4,
					targetInstruction: new(OpCodes.Ldfld, typeof(Rectangle).GetField("X")),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldfld, typeof(Rectangle).GetField("X")),
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
						new(OpCodes.Ldloc_1),
						new(OpCodes.Ldfld, typeof(Rectangle).GetField("Width")),
						new(OpCodes.Sub)
					}
				),
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tileX")),
						new(OpCodes.Call, typeof(NetFieldBase<int, NetInt>).GetMethod("op_Implicit")),
						new(OpCodes.Add)
					},
					offset: 4,
					targetInstruction: new(OpCodes.Callvirt, typeof(NetPoint).GetProperty("X").GetGetMethod()),
					replacementInstructions: new CodeInstruction[]
					{
						 new(OpCodes.Callvirt, typeof(NetPoint).GetProperty("X").GetGetMethod()),
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
						new(OpCodes.Ldloc_1),
						new(OpCodes.Ldfld, typeof(Rectangle).GetField("Width")),
						new(OpCodes.Sub)
					}
				)
			};
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, CompatibilityHelper.GenericBuildingType, "getRectForAnimalDoor");
		}

		private static IEnumerable<CodeInstruction> UpdateInteriorWarpsTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_0),
						new(OpCodes.Call, CompatibilityHelper.GenericBuildingType.GetProperty("Model").GetGetMethod()),
						new(OpCodes.Ldflda, CompatibilityHelper.BuildingDataType.GetField("HumanDoor")),
						new(OpCodes.Ldfld, typeof(Point).GetField("X"))
					},
					offset: 0,
					targetInstruction: new(OpCodes.Ldfld, typeof(Point).GetField("X")),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldfld, typeof(Point).GetField("X")),
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
						new(OpCodes.Ldc_I4_1),
						new(OpCodes.Sub)
					}
				),
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_0),
						new(OpCodes.Call, CompatibilityHelper.GenericBuildingType.GetProperty("Model").GetGetMethod()),
						new(OpCodes.Ldfld, CompatibilityHelper.ExtendedBuildingModelType.GetField("TunnelDoors"))
					},
					offset: 4,
					isNegativeOffset: false,
					targetInstruction: new(OpCodes.Ldfld, typeof(Point).GetField("X")),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldfld, typeof(Point).GetField("X")),
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
						new(OpCodes.Ldc_I4_1),
						new(OpCodes.Sub)
					}
				)
			};
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, CompatibilityHelper.GenericBuildingType, "updateInteriorWarps");
		}

		private static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldfld, typeof(Vector2).GetField("X")),
						new(OpCodes.Conv_I4),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tileX")),
						new(OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Sub)
					},
					offset: 0,
					targetInstruction: new(OpCodes.Sub),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Sub),
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
						new(OpCodes.Ldc_I4_1),
						new(OpCodes.Sub)
					}
				),
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldfld, typeof(Vector2).GetField("X")),
						new(OpCodes.Conv_I4),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tileX")),
						new(OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Sub)
					},
					offset: 0,
					targetInstruction: new(OpCodes.Sub),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Sub),
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
						new(OpCodes.Ldc_I4_1),
						new(OpCodes.Sub)
					}
				)
			};
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, CompatibilityHelper.GenericBuildingType, "Update");
		}

		private static IEnumerable<CodeInstruction> UpdateBackportTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_0),
						new(OpCodes.Call, CompatibilityHelper.GenericBuildingType.GetProperty("Model").GetGetMethod()),
						new(OpCodes.Ldfld, CompatibilityHelper.BuildingDataType.GetField("DrawOffset")),
						new(OpCodes.Ldc_R4, 4f)
					},
					offset: 0,
					targetInstruction: new(OpCodes.Ldc_R4, 4f),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldfld, typeof(Vector2).GetField("X")),
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Call, CompatibilityHelper.GenericBuildingType.GetProperty("Model").GetGetMethod()),
						new(OpCodes.Ldfld, CompatibilityHelper.ExtendedBuildingModelType.GetField("DrawOffset")),
						new(OpCodes.Ldfld, typeof(Vector2).GetField("Y")),
						new(OpCodes.Newobj, typeof(Vector2).GetConstructor(new Type[] { typeof(Single), typeof(Single) })),
						new(OpCodes.Ldc_R4, 4f)
					}
				)
			};
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, CompatibilityHelper.GenericBuildingType, "UpdateBackport");
		}

		private static IEnumerable<CodeInstruction> IsTilePassableTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
				new(
					referenceInstruction: new(OpCodes.Callvirt, CompatibilityHelper.BuildingDataType.GetMethod("IsTilePassable", new Type[] { typeof(Int32), typeof(Int32) })),
					offset: 8,
					targetInstruction: new(OpCodes.Sub),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Sub),
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
						new(OpCodes.Ldc_I4_1),
						new(OpCodes.Sub)
					}
				)
			};
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, CompatibilityHelper.GenericBuildingType, "isTilePassable");
		}

		private static IEnumerable<CodeInstruction> IsInTilePropertyRadiusTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_1),
						new(OpCodes.Ldfld, typeof(Vector2).GetField("X"))
					},
					offset: 0,
					targetInstruction: new(OpCodes.Ldfld, typeof(Vector2).GetField("X")),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldfld, typeof(Vector2).GetField("X")),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tileX")),
						new(OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Sub),
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
						new(OpCodes.Ldc_I4_1),
						new(OpCodes.Sub),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tileX")),
						new(OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add)
					}
				),
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_1),
						new(OpCodes.Ldfld, typeof(Vector2).GetField("X"))
					},
					offset: 0,
					targetInstruction: new(OpCodes.Ldfld, typeof(Vector2).GetField("X")),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldfld, typeof(Vector2).GetField("X")),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tileX")),
						new(OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Sub),
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
						new(OpCodes.Ldc_I4_1),
						new(OpCodes.Sub),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tileX")),
						new(OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add)
					}
				)
			};
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, CompatibilityHelper.GenericBuildingType, "IsInTilePropertyRadius");
		}

		private static IEnumerable<CodeInstruction> ResetLightsTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
				new(
					referenceInstruction: new(OpCodes.Call, typeof(Vector2).GetConstructor(new Type[] { typeof(Single), typeof(Single) })),
					offset: 18,
					targetInstruction: new(OpCodes.Conv_R4),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Conv_R4),
						new(OpCodes.Ldloc_1),
						new(OpCodes.Ldfld, typeof(Point).GetField("X")),
						new(OpCodes.Sub),
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
						new(OpCodes.Ldloc_1),
						new(OpCodes.Ldfld, typeof(Point).GetField("X")),
						new(OpCodes.Add)
					},
					goNext: false
				),
				new(
					referenceInstruction: new(OpCodes.Call, typeof(Vector2).GetConstructor(new Type[] { typeof(Single), typeof(Single) })),
					offset: 11,
					targetInstruction: new(OpCodes.Add),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Sub)
					}
				)
			};
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, CompatibilityHelper.GenericBuildingType, "ResetLights");
		}

		private static IEnumerable<CodeInstruction> GetUpgradeSignLocationTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tileX")),
						new(OpCodes.Call, typeof(NetFieldBase<int, NetInt>).GetMethod("op_Implicit")),
						new(OpCodes.Conv_R4),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Call, CompatibilityHelper.GenericBuildingType.GetProperty("Model").GetGetMethod()),
						new(OpCodes.Ldflda, CompatibilityHelper.BuildingDataType.GetField("UpgradeSignTile")),
						new(OpCodes.Ldfld, typeof(Vector2).GetField("X"))
					},
					offset: 0,
					targetInstruction: new(OpCodes.Ldfld, typeof(Vector2).GetField("X")),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldfld, typeof(Vector2).GetField("X")),
						new(OpCodes.Ldc_I4_M1),
						new(OpCodes.Mul),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
						new(OpCodes.Ldc_I4_1),
						new(OpCodes.Sub)
					}
				),
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tileX")),
						new(OpCodes.Call, typeof(NetFieldBase<int, NetInt>).GetMethod("op_Implicit")),
						new(OpCodes.Ldc_I4_5),
						new(OpCodes.Add)
					},
					offset: 1,
					targetInstruction: new(OpCodes.Ldc_I4_5),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tileX")),
						new(OpCodes.Call, typeof(NetFieldBase<int, NetInt>).GetMethod("op_Implicit")),
						new(OpCodes.Ldc_I4_S, (sbyte)64),
						new(OpCodes.Mul),
						new(OpCodes.Ldc_I4_S, (sbyte)32),
						new(OpCodes.Add)
					},
					offset: 0,
					targetInstruction: new(OpCodes.Add),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Sub),
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tilesWide")),
						new(OpCodes.Callvirt, typeof(NetInt).GetProperty("Value").GetGetMethod()),
						new(OpCodes.Add),
						new(OpCodes.Ldc_I4_S, (sbyte)64),
						new(OpCodes.Mul)
					}
				)
			};
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, CompatibilityHelper.GenericBuildingType, "getUpgradeSignLocation");
		}
	}
}
