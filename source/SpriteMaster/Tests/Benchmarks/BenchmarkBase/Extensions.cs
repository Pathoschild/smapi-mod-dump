/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;

namespace Benchmarks.BenchmarkBase;

[PublicAPI]
public static class Extensions {
	// https://stackoverflow.com/a/5807238/5055153
	public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random? rng) {
		return source.ShuffleIterator(rng ?? new Random());
	}

	private static IEnumerable<T> ShuffleIterator<T>(
		this IEnumerable<T> source, Random rng) {
		var buffer = source.ToList();
		for (int i = 0; i < buffer.Count; i++) {
			int j = rng.Next(i, buffer.Count);
			yield return buffer[j];

			buffer[j] = buffer[i];
		}
	}

	public static T[] ShuffleF<T>(this T[] source, Random? rng) {
		return source.ShuffleIteratorF(rng ?? new Random());
	}

	private static T[] ShuffleIteratorF<T>(
		this T[] source, Random rng) {
		var buffer = source.ToArray();
		var dest = GC.AllocateUninitializedArray<T>(source.Length);
		for (int i = 0; i < buffer.Length; i++) {
			int j = rng.Next(i, buffer.Length);
			dest[i] = buffer[j];

			buffer[j] = buffer[i];
		}

		return dest;
	}

	public static T[] ShuffleF<T>(this IList<T> source, Random? rng) {
		if (source is T[] array) {
			return array.ShuffleF(rng);
		}

		return source.ShuffleIteratorF(rng ?? new Random());
	}

	private static T[] ShuffleIteratorF<T>(
		this IList<T> source, Random rng) {
		var buffer = source.ToArray();
		var dest = GC.AllocateUninitializedArray<T>(source.Count);
		for (int i = 0; i < buffer.Length; i++) {
			int j = rng.Next(i, buffer.Length);
			dest[i] = buffer[j];

			buffer[j] = buffer[i];
		}

		return dest;
	}
}
