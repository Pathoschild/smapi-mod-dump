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
using StardewValley.Buildings;
using FlipBuildings.Utilities;

namespace FlipBuildings.Patches
{
	internal class MillPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(Mill), nameof(Mill.draw), new Type[] { typeof(SpriteBatch) }),
				transpiler: new HarmonyMethod(typeof(MillPatch), nameof(DrawTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(Mill), nameof(Mill.doAction), new Type[] { typeof(Vector2), typeof(StardewValley.Farmer) }),
				transpiler: new HarmonyMethod(typeof(MillPatch), nameof(DoActionTranspiler))
			);
		}

		private static IEnumerable<CodeInstruction> DrawTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchHelper.CodeReplacement[] codeReplacements = new PatchHelper.CodeReplacement[]
			{
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
					offset: 85,
					targetInstruction: new(OpCodes.Ldc_I4_S, (sbyte)32),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_S, (sbyte)96)
					}
				),
				// null
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 68,
					targetInstruction: new(OpCodes.Ldc_I4_S, (sbyte)52),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_S, (sbyte)116)
					}
				),
				// null
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 47,
					targetInstruction: new(OpCodes.Ldc_I4, 192),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4, -16)
					},
					goNext: false
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 20,
					targetInstruction: new(OpCodes.Ldc_I4_0),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_1)
					}
				),
				new(
					referenceInstruction: new(OpCodes.Callvirt, typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) })),
					offset: 62,
					targetInstruction: new(OpCodes.Ldc_I4, 192),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4, -4)
					}
				),
				// null
			};
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, typeof(Mill), nameof(Mill.draw));
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
						new(OpCodes.Call, typeof(Netcode.NetFieldBase<int, Netcode.NetInt>).GetMethod("op_Implicit"))
					},
					offset: 1,
					isNegativeOffset: false,
					targetInstruction: new(OpCodes.Ldc_I4_1),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_2)
					}
				),
				new(
					referenceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldarg_0),
						new(OpCodes.Ldfld, typeof(Building).GetField("tileX")),
						new(OpCodes.Call, typeof(Netcode.NetFieldBase<int, Netcode.NetInt>).GetMethod("op_Implicit"))
					},
					offset: 1,
					isNegativeOffset: false,
					targetInstruction: new(OpCodes.Ldc_I4_3),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_0)
					}
				)
			};
			return PatchHelper.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, typeof(Mill), nameof(Mill.doAction));
		}
	}
}
