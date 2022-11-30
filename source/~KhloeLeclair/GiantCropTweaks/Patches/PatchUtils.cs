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
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;

namespace Leclair.Stardew.GiantCropTweaks.Patches;

internal static class PatchUtils {

	internal static IEnumerable<CodeInstruction> ReplaceCalls(
		IEnumerable<CodeInstruction> instructions,
		IEnumerable<KeyValuePair<MethodInfo, MethodInfo>>? callReplacements = null,
		IEnumerable<KeyValuePair<FieldInfo, MethodInfo>>? fieldReplacements = null
	) {
		var callDict = callReplacements is null ? null : callReplacements is IDictionary<MethodInfo, MethodInfo> cdict ? cdict : callReplacements.ToDictionary(x => x.Key, x => x.Value);
		var fieldDict = fieldReplacements is null ? null : fieldReplacements is IDictionary<FieldInfo, MethodInfo> fdict ? fdict : fieldReplacements.ToDictionary(x => x.Key, x => x.Value);

		foreach (var in0 in instructions) {
			if (in0.opcode == OpCodes.Call && callDict is not null && in0.operand is MethodInfo method && callDict.TryGetValue(method, out var replacement)) {
				var ret = new CodeInstruction(in0) {
					operand = replacement
				};

				yield return ret;
				continue;
			}

			if (in0.opcode == OpCodes.Ldsfld && fieldDict is not null && in0.operand is FieldInfo field && fieldDict.TryGetValue(field, out var repl)) {
				var ret = new CodeInstruction(in0) {
					opcode = OpCodes.Call,
					operand = repl
				};

				yield return ret;
				continue;
			}

			yield return in0;
		}
	}

	internal static bool IsCall(this CodeInstruction instr, MethodInfo method) {
		return instr.opcode == OpCodes.Call && instr.operand is MethodInfo m && m == method;
	}

	internal static bool IsCallVirt(this CodeInstruction instr, MethodInfo method) {
		return instr.opcode == OpCodes.Callvirt && instr.operand is MethodInfo m && m == method;
	}

	internal static int? AsInt(this CodeInstruction instr) {
		if (instr.opcode == OpCodes.Ldc_I4_0)
			return 0;
		if (instr.opcode == OpCodes.Ldc_I4_1)
			return 1;
		if (instr.opcode == OpCodes.Ldc_I4_2)
			return 2;
		if (instr.opcode == OpCodes.Ldc_I4_3)
			return 3;
		if (instr.opcode == OpCodes.Ldc_I4_4)
			return 4;
		if (instr.opcode == OpCodes.Ldc_I4_5)
			return 5;
		if (instr.opcode == OpCodes.Ldc_I4_6)
			return 6;
		if (instr.opcode == OpCodes.Ldc_I4_7)
			return 7;
		if (instr.opcode == OpCodes.Ldc_I4_8)
			return 8;
		if (instr.opcode == OpCodes.Ldc_I4_M1)
			return -1;
		if (instr.opcode != OpCodes.Ldc_I4 && instr.opcode != OpCodes.Ldc_I4_S)
			return null;

		if (instr.operand is int val)
			return val;
		if (instr.operand is byte bval)
			return bval;
		if (instr.operand is sbyte sbval)
			return sbval;
		if (instr.operand is short shval)
			return shval;
		if (instr.operand is ushort ushval)
			return ushval;

		return null;
	}

}
