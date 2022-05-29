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
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Microsoft.Xna.Framework;

using StardewValley;

namespace Leclair.Stardew.ThemeManager.Patches;

internal static class PatchUtils {

	internal static IEnumerable<KeyValuePair<FieldInfo, T>> HydrateFieldKeys<T>(this IEnumerable<KeyValuePair<string, T>> source, Type type) {
		return source.Select(x => new KeyValuePair<FieldInfo, T>(
			AccessTools.Field(type, x.Key),
			x.Value
		));
	}

	internal static IEnumerable<KeyValuePair<MethodInfo, T>> HydrateMethodKeys<T>(this IEnumerable<KeyValuePair<string, T>> source, Type type) {
		return source.Select(x => new KeyValuePair<MethodInfo, T>(
			AccessTools.Method(type, x.Key),
			x.Value
		));
	}

	internal static IEnumerable<KeyValuePair<MethodInfo, T>> HydratePropertyKeys<T>(this IEnumerable<KeyValuePair<string, T>> source, Type type) {
		return source.Select(x => new KeyValuePair<MethodInfo, T>(
			AccessTools.PropertyGetter(type, x.Key),
			x.Value
		));
	}

	internal static IEnumerable<KeyValuePair<T, FieldInfo>> HydrateFieldValues<T>(this IEnumerable<KeyValuePair<T, string>> source, Type type) {
		return source.Select(x => new KeyValuePair<T, FieldInfo>(
			x.Key,
			AccessTools.Field(type, x.Value)
		));
	}

	internal static IEnumerable<KeyValuePair<T, MethodInfo>> HydrateMethodValues<T>(this IEnumerable<KeyValuePair<T, string>> source, Type type) {
		return source.Select(x => new KeyValuePair<T, MethodInfo>(
			x.Key,
			AccessTools.Method(type, x.Value)
		));
	}

	internal static IEnumerable<KeyValuePair<T, MethodInfo>> HydratePropertyValues<T>(this IEnumerable<KeyValuePair<T, string>> source, Type type) {
		return source.Select(x => new KeyValuePair<T, MethodInfo>(
			x.Key,
			AccessTools.PropertyGetter(type, x.Value)
		));
	}

	internal static IEnumerable<CodeInstruction> ReplaceColors(
		IEnumerable<CodeInstruction> instructions,
		Type type,
		IEnumerable<KeyValuePair<string, string>> replacements,
		IEnumerable<KeyValuePair<FieldInfo, MethodInfo>>? fieldReplacements = null
	) {
		return ReplaceCalls(
			instructions,
			replacements
				.HydratePropertyKeys(typeof(Color))
				.HydrateMethodValues(type),
			fieldReplacements
		);
	}

	internal static IEnumerable<CodeInstruction> ReplaceColors(
		IEnumerable<CodeInstruction> instructions,
		IEnumerable<KeyValuePair<string, MethodInfo>> replacements,
		IEnumerable<KeyValuePair<FieldInfo, MethodInfo>>? fieldReplacements = null
	) {
		return ReplaceCalls(
			instructions,
			replacements.HydratePropertyKeys(typeof(Color)),
			fieldReplacements
		);
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

	internal static IEnumerable<CodeInstruction> ReplaceColors(
		IEnumerable<CodeInstruction> instructions,
		IEnumerable<KeyValuePair<Color, MethodInfo>> replacements
	) {
		var replDict = replacements is null ? null : replacements is IDictionary<Color, MethodInfo> dict ? dict : replacements.ToDictionary(x => x.Key, x => x.Value);

		var instrs = instructions.ToArray();

		for(int i = 0; i < instrs.Length; i++) {
			CodeInstruction in0 = instrs[i];
			if (i + 3 < instrs.Length && replDict is not null) {
				CodeInstruction in1 = instrs[i + 1];
				CodeInstruction in2 = instrs[i + 2];
				CodeInstruction in3 = instrs[i + 3];

				if (in3.opcode == OpCodes.Newobj && in3.operand is ConstructorInfo ctor && ctor.DeclaringType == typeof(Color)) {
					int? val0 = in0.AsInt();
					int? val1 = in1.AsInt();
					int? val2 = in2.AsInt();

					if (val0.HasValue && val1.HasValue && val2.HasValue) {
						Color c = new(val0.Value, val1.Value, val2.Value);
						if (replDict.TryGetValue(c, out var repl)) {

							yield return new CodeInstruction(
								opcode: OpCodes.Call,
								operand: repl
							);

							i += 3;
							continue;
						}
					}
				}
			}

			yield return in0;
		}
	}

	internal static IEnumerable<CodeInstruction> ReplaceCalls(
		IEnumerable<CodeInstruction> instructions,
		IEnumerable<KeyValuePair<MethodInfo, MethodInfo>>? callReplacements = null,
		IEnumerable<KeyValuePair<FieldInfo, MethodInfo>>? fieldReplacements = null
	) {
		var callDict = callReplacements is null ? null : callReplacements is IDictionary<MethodInfo, MethodInfo> cdict ? cdict : callReplacements.ToDictionary(x => x.Key, x => x.Value);
		var fieldDict = fieldReplacements is null ? null : fieldReplacements is IDictionary<FieldInfo, MethodInfo> fdict ? fdict : fieldReplacements.ToDictionary(x => x.Key, x => x.Value);

		foreach(var in0 in instructions) {
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

}
