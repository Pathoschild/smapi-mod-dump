/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Benchmarks.BenchmarkBase.Benchmarks;
using System.Runtime.CompilerServices;
using System.Text;

namespace Benchmarks.Strings.Benchmarks.Sources;

public abstract class StringSource : BenchmarkBase<DataSet<StringSource.StringData[]>, StringSource.StringData[]> {
	public readonly struct StringData {
		internal readonly string String;
		internal readonly StringBuilder Builder;
		internal readonly object Reference;

		internal StringData(Random rand, string str) {
			String = str;
			Builder = new StringBuilder(str);
			Reference = rand.Next(2) == 0 ? String : Builder;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string DeIntern(string str) {
		if (string.IsInterned(str) is not null) {
			return new StringBuilder().Append(str.ToArray()).ToString();
		}

		return str;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void AddSet(Random rand, IList<string> set) {
		var newStringData = GC.AllocateUninitializedArray<StringData>(set.Count);
		for (var i = 0; i < set.Count; ++i) {
			newStringData[i] = new(rand, DeIntern(set[i]));
		}
		DataSets.Add(new(newStringData));
	}
}