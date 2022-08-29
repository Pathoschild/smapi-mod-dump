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
using JetBrains.Annotations;
using LinqFasterer;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Configuration;
using SpriteMaster.Extensions.Reflection;
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

	private readonly record struct KeyType<TKey>(TKey Key, int Index) where TKey : IComparable<TKey>;

	private readonly struct KeyTypeComparer<TKey> : IComparer<KeyType<TKey>> where TKey : IComparable<TKey> {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Compare(KeyType<TKey> x, KeyType<TKey> y) {
			int result = x.Key.CompareTo(y.Key);
			if (result != 0) {
				return result;
			}
			return x.Index.CompareTo(y.Index);
		}
	}

	private interface ISortKeyGetter<TObject, TKey> where TKey : IComparable<TKey> {
		TKey Invoke(TObject obj);
	}

	private static class TypedImpl<TKey> where TKey : IComparable<TKey> {
		private static KeyType<TKey>[] KeyList = Array.Empty<KeyType<TKey>>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static KeyType<TKey>[] Get(int length) {
			var keyList = KeyList;
			if (keyList.Length < length) {
				KeyList = keyList = GC.AllocateUninitializedArray<KeyType<TKey>>(length);
			}

			return keyList;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static void StableSortImpl<TObject, TKey, TSortKeyGetter>(TObject[] array, int index, int length, TSortKeyGetter sortKeyGetter)
		where TKey : IComparable<TKey>
		where TSortKeyGetter : ISortKeyGetter<TObject, TKey> {
		int requiredLength = length + index;
		var keyList = TypedImpl<TKey>.Get(requiredLength);

		for (int i = index; i < requiredLength; ++i) {
			keyList[i] = new(Key: sortKeyGetter.Invoke(array[i]), Index: i);
		}

		Array.Sort(keyList, array, index, length, default(KeyTypeComparer<TKey>));
	}

	internal static class BySortKey {
		internal readonly struct SortKeyGetter<T> : ISortKeyGetter<T, float> {
			[MethodImpl(Runtime.MethodImpl.Inline)]
			public readonly float Invoke(T? obj) => obj is null ? float.MinValue : GetSortKeyImpl!(obj);
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal static void StableSort<T>(T[] array, int index, int length) where T : IComparable<T> {
			StableSortImpl<T, float, SortKeyGetter<T>>(array, index, length, default);
		}
	}

	internal static class ByInterface<T> where T : IComparable<T> {
		internal readonly struct SortKeyGetter : ISortKeyGetter<T, T> {
			[MethodImpl(Runtime.MethodImpl.Inline)]
			public readonly T Invoke(T obj) => obj;
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal static void StableSort(T[] array, int index, int length) {
			StableSortImpl<T, T, SortKeyGetter>(array, index, length, default);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	public static void ArrayStableSort<T>(T[] array, int index, int length, SpriteSortMode sortMode) where T : IComparable<T> {
		if (DrawState.CurrentBlendState == BlendState.Additive && sortMode != SpriteSortMode.Texture) {
			// There is basically no reason to sort when the blend state is additive.
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
	public static IEnumerable<CodeInstruction> DrawBatchTranspiler(IEnumerable<CodeInstruction> instructions) {
		if (SpriteBatchItemType is null) {
			Debug.Error($"Could not apply SpriteBatcher stable sorting patch: {nameof(SpriteBatchItemType)} was null");
			return instructions;
		}

		if (GetSortKeyImpl is null) {
			Debug.Warning("Could not get accessor for SpriteBatchItem 'SortKey' - slower path being used");
		}

		var newMethod = typeof(StableSort).GetStaticMethod("ArrayStableSort")?.MakeGenericMethod(SpriteBatchItemType);

		if (newMethod is null) {
			Debug.Error("Could not apply SpriteBatcher stable sorting patch: could not find MethodInfo for ArrayStableSort");
			return instructions;
		}

		var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();

		bool applied = false;

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

				yield return new(OpCodes.Ldarg_1);
				yield return new(OpCodes.Call, newMethod);
				applied = true;
			}
		}

		var result = ApplyPatch().ToArray();

		if (!applied) {
			Debug.Error("Could not apply SpriteBatcher stable sorting patch: Sort call could not be found in IL");
		}

		return result;
	}
}
