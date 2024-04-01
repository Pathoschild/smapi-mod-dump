/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using Benchmarks.BenchmarkBase.Benchmarks;
using System.Runtime.CompilerServices;
using System.Text;

namespace Benchmarks.Hashing.Benchmarks;

public partial class Strings {
	private const int RandSeed = 0x13377113;
	private static readonly long MinSize = Program.CurrentOptions.Min;
	private static readonly long MaxSize = Program.CurrentOptions.Max;

	private static readonly char[] Chars = {
		'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o',
		'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D',
		'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S',
		'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
		'1', '2', '3', '4', '5', '6', '7', '8', '9', '0'
	};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static char RandomChar(Random rand) {
		return Chars[rand.Next(0, Chars.Length)];
	}

	static Strings() {
		var random = new Random(RandSeed);

		void AddSet(in DataSet<string> dataSet) {
			DefaultDataSetsStatic.Add(dataSet);
		}

		long min = MinSize;

		if (min == 0 && min != MaxSize) {
			AddSet(new(new StringBuilder().ToString()));

			min = 1;
		}

		for (long i = min; i <= MaxSize; i *= 2) {
			var sb = new StringBuilder();
			for (int j = 0; j < i; ++j) {
				sb.Append(RandomChar(random));
			}

			AddSet(new(sb.ToString()));
		}

		if (DefaultDataSetsStatic.Last().Data.Length != MaxSize) {
			var sb = new StringBuilder();
			for (int j = 0; j < MaxSize; ++j) {
				sb.Append(RandomChar(random));
			}

			AddSet(new(sb.ToString()));
		}
	}
}
