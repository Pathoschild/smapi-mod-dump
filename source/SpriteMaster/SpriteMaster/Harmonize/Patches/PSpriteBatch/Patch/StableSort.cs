/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Harmonize.Patches.PSpriteBatch.Patch;
static class StableSort {
	private static readonly Type? SpriteBatchItemType = typeof(XNA.Graphics.SpriteBatch).Assembly.GetType("Microsoft.Xna.Framework.Graphics.SpriteBatchItem");

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static void Swap<T>(ref T a, ref T b) => (a, b) = (b, a);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static int Partition<T>(Span<T> data, Span<int> indices, int low, int high) where T : IComparable<T> {
		var pivot = data[high];
		var pivotIndex = indices[high];

		var i = low - 1;

		for (int j = low; j <= high - 1; ++j) {
			var jIndex = indices[j];
			var compareResult = data[j].CompareTo(pivot);
			if (compareResult == 0) {
				compareResult = jIndex - pivotIndex;
			}
			if (compareResult < 0) {
				++i;
				Swap(ref data[i], ref data[j]);
				Swap(ref indices[i], ref indices[j]);
			}
		}
		Swap(ref data[i + 1], ref data[high]);
		Swap(ref indices[i + 1], ref indices[high]);
		return i + 1;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static void QuickSort<T>(Span<T> data, Span<int> indices, int low, int high) where T : IComparable<T> {
		if (data.Length <= 1) {
			return;
		}

		if (low < high) {
			var pIndex = Partition(data, indices, low, high);
			QuickSort(data, indices, low, pIndex - 1);
			QuickSort(data, indices, pIndex + 1, high);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static void ArrayStableSort<T>(T[] array, int index, int length) where T : IComparable<T> {
		if (!Config.Enabled || !Config.Extras.StableSort) {
			Array.Sort(array, index, length);
			return;
		}

		// Not _optimal_, really need a proper stable sort. Optimize later.
		Span<int> indices = stackalloc int[length];
		for (int i = 0; i < length; ++i) {
			indices[i] = i;
		}
		var span = new Span<T>(array, index, length);
		QuickSort(span, indices, 0, length - 1);
	}

	[Harmonize(
		typeof(XNA.Graphics.SpriteBatch),
		"Microsoft.Xna.Framework.Graphics.SpriteBatcher",
		"DrawBatch",
		fixation: Harmonize.Fixation.Transpile
	)]
	internal static IEnumerable<CodeInstruction> SpriteBatcherTranspiler(IEnumerable<CodeInstruction> instructions) {
		if (SpriteBatchItemType is null) {
			Debug.Error($"Could not apply SpriteBatcher stable sorting patch: {nameof(SpriteBatchItemType)} was null");
			return instructions;
		}


		var newMethod = typeof(StableSort).GetMethod("ArrayStableSort", BindingFlags.Static | BindingFlags.NonPublic)?.MakeGenericMethod(new Type[] { SpriteBatchItemType });

		if (newMethod is null) {
			Debug.Error($"Could not apply SpriteBatcher stable sorting patch: could not find MethodInfo for ArrayStableSort");
			return instructions;
		}

		IEnumerable<CodeInstruction> ApplyPatch() {
			foreach (var instruction in instructions) {
				if (
					instruction.opcode.Value != OpCodes.Call.Value ||
					instruction.operand is not MethodInfo callee ||
					!callee.IsGenericMethod ||
					callee.GetGenericArguments().FirstOrDefault() != SpriteBatchItemType ||
					callee.DeclaringType != typeof(Array) ||
					callee.Name != "Sort" ||
					callee.GetParameters().Length != 3
				) {
					yield return instruction;
					continue;
				}

				yield return new CodeInstruction(OpCodes.Call, newMethod);
			}
		}

		var result = ApplyPatch();

		if (result.SequenceEqual(instructions)) {
			Debug.Error("Could not apply SpriteBatcher stable sorting patch: Sort call could not be found in IL");
		}

		return result;
	}
}
