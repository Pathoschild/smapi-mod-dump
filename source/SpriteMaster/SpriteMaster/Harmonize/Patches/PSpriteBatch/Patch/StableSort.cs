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
using LinqFasterer;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Harmonize.Patches.PSpriteBatch.Patch;

internal static class StableSort {
	private static readonly Type? SpriteBatchItemType = typeof(XSpriteBatch).Assembly.GetType("Microsoft.Xna.Framework.Graphics.SpriteBatchItem");
	private static readonly Func<object?, float>? GetSortKeyImpl = SpriteBatchItemType?.GetFieldGetter<object?, float>("SortKey");

	internal static class BySortKey {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		private static float GetSortKey(object? obj) => obj is null ? float.MinValue : GetSortKeyImpl!(obj);

		private readonly record struct KeyType(float Key, int Index);

		private sealed class KeyTypeComparerClass : IComparer<KeyType> {
			public int Compare(KeyType x, KeyType y) {
				int result = x.Key.CompareTo(y.Key);
				if (result != 0) {
					return result;
				}
				return x.Index.CompareTo(y.Index);
			}
		}
		private static readonly KeyTypeComparerClass KeyTypeComparer = new();

		private static KeyType[] KeyList = Array.Empty<KeyType>();

		internal static void StableSort<T>(T[] array, int index, int length) where T : IComparable<T> {
			int requiredLength = length + index;
			if (requiredLength > KeyList.Length) {
				KeyList = GC.AllocateUninitializedArray<KeyType>(requiredLength);
			}
			for (int i = index; i < requiredLength; ++i) {
				KeyList[i] = new(Key: GetSortKey(array[i]), Index: i);
			}

			Array.Sort(KeyList, array, index, length, KeyTypeComparer);
		}
	}

	internal static class ByInterface<T> where T : IComparable<T> {
		private readonly record struct KeyType(T Key, int Index);

		private sealed class KeyTypeComparerClass : IComparer<KeyType> {
			public int Compare(KeyType x, KeyType y) {
				int result = x.Key.CompareTo(y.Key);
				if (result != 0) {
					return result;
				}
				return x.Index.CompareTo(y.Index);
			}
		}
		private static readonly KeyTypeComparerClass KeyTypeComparer = new();

		private static KeyType[] KeyList = Array.Empty<KeyType>();

		internal static void StableSort(T[] array, int index, int length) {
			int requiredLength = length + index;
			if (requiredLength > KeyList.Length) {
				KeyList = GC.AllocateUninitializedArray<KeyType>(requiredLength);
			}
			for (int i = index; i < requiredLength; ++i) {
				KeyList[i] = new(Key: array[i], Index: i);
			}

			Array.Sort(KeyList, array, index, length, KeyTypeComparer);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static void ArrayStableSort<T>(T[] array, int index, int length, SpriteSortMode sortMode) where T : IComparable<T> {
		if (DrawState.CurrentBlendState == BlendState.Additive) {
			// There is basically no reason to sort when the blend state is additive.
			return;
		}

		if (sortMode != SpriteSortMode.FrontToBack) {
			return;
		}

		if (!Config.IsEnabled || !Config.Extras.StableSort) {
			Array.Sort(array, index, length);
			return;
		}

		if (GetSortKeyImpl is not null) {
			BySortKey.StableSort(array, index, length);
		}
		else {
			ByInterface<T>.StableSort(array, index, length);
		}
	}

	[HarmonizeTranspile(
		typeof(XSpriteBatch),
		"Microsoft.Xna.Framework.Graphics.SpriteBatcher",
		"DrawBatch",
		argumentTypes: new [] { typeof(SpriteSortMode), typeof(Effect) }
	)]
	public static IEnumerable<CodeInstruction> SpriteBatcherTranspiler(IEnumerable<CodeInstruction> instructions) {
		if (SpriteBatchItemType is null) {
			Debug.Error($"Could not apply SpriteBatcher stable sorting patch: {nameof(SpriteBatchItemType)} was null");
			return instructions;
		}

		if (GetSortKeyImpl is null) {
			Debug.Warning("Could not get accessor for SpriteBatchItem 'SortKey' - slower path being used");
		}


		var newMethod = typeof(StableSort).GetMethod("ArrayStableSort", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)?.MakeGenericMethod(SpriteBatchItemType);
		//var newMethod = typeof(StableSort).GetMethod("ArrayStableSort", BindingFlags.Static | BindingFlags.NonPublic)?.MakeGenericMethod(new Type[] { SpriteBatchItemType });

		if (newMethod is null) {
			Debug.Error("Could not apply SpriteBatcher stable sorting patch: could not find MethodInfo for ArrayStableSort");
			return instructions;
		}

		var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();

		IEnumerable<CodeInstruction> ApplyPatch() {
			foreach (var instruction in codeInstructions) {
				if (
					instruction.opcode.Value != OpCodes.Call.Value ||
					instruction.operand is not MethodInfo callee ||
					!callee.IsGenericMethod ||
					callee.GetGenericArguments().FirstOrDefaultF() != SpriteBatchItemType ||
					callee.DeclaringType != typeof(Array) ||
					callee.Name != "Sort" ||
					callee.GetParameters().Length != 3
				) {
					yield return instruction;
					continue;
				}

				yield return new CodeInstruction(OpCodes.Ldarg_1);
				yield return new CodeInstruction(OpCodes.Call, newMethod);
			}
		}

		var result = ApplyPatch();

		if (result.SequenceEqual(codeInstructions)) {
			Debug.Error("Could not apply SpriteBatcher stable sorting patch: Sort call could not be found in IL");
		}

		return result;
	}
}
