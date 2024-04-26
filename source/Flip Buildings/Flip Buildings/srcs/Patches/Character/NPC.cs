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
using StardewValley;
using StardewValley.Buildings;
using FlipBuildings.Utilities;

namespace FlipBuildings.Patches
{
	internal class NPCPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(NPC), "updateConstructionAnimation"),
				transpiler: new HarmonyMethod(typeof(NPCPatch), nameof(UpdateConstructionAnimationTranspiler))
			);
		}

		private static IEnumerable<CodeInstruction> UpdateConstructionAnimationTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			PatchUtility.CodeReplacement[] codeReplacements = new PatchUtility.CodeReplacement[]
			{
				new(
					instanceType: typeof(Building),
					instanceInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldloc_1),
						new(OpCodes.Call, typeof(Farm).GetMethod(nameof(Farm.GetMainFarmHouse)))
					},
					referenceInstruction: new(OpCodes.Call, typeof(Game1).GetMethod(nameof(Game1.warpCharacter), new Type[] { typeof(NPC), typeof(string), typeof(Vector2) })),
					offset: 10,
					targetInstruction: new(OpCodes.Ldc_I4_4),
					replacementInstructions: new CodeInstruction[]
					{
						new(OpCodes.Ldc_I4_6)
					}
				)
			};
			return PatchUtility.ReplaceInstructionsByOffsets(instructions, iLGenerator, codeReplacements, typeof(NPC), "updateConstructionAnimation");
		}
	}
}
