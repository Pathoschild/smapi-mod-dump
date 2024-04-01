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
using System.Numerics;

namespace Benchmarks.Arrays.Benchmarks;

public partial class CloneBuffers {
	private const int RandSeed = 0x13377113;
	private static readonly long MinSize = Program.CurrentOptions.Min;
	private static readonly long MaxSize = Program.CurrentOptions.Max;

	static CloneBuffers() {
		var random = new Random(RandSeed);

		DataSetArrayUnfixed<byte> MakeDataSet(long length) =>
			new(random, length);

		long start = MinSize;

		bool makeEmpty = start == 0;

		int requiredSize = (makeEmpty ? 1 : 0) + (BitOperations.Log2((uint)MaxSize) - BitOperations.Log2((uint)MinSize)) + 1;

		var dataSets = new List<DataSetArrayUnfixed<byte>>(requiredSize);

		if (makeEmpty) {
			var dataSet = MakeDataSet(start);

			//dataSets.Add(dataSet);

			start = 1;
		}

		if (MaxSize == MinSize) {
			if (MaxSize != 0) {
				var dataSet = MakeDataSet(MaxSize);
				dataSets.Add(dataSet);
			}
		}
		else {
			for (long i = start; i <= MaxSize; i *= 2) {
				//var halfIndex = (i >> 1) + (i >> 2);
				//if (halfIndex != 0 && halfIndex != i) {
				//	dataSets.Add(MakeDataSet(halfIndex));
				//}

				dataSets.Add(MakeDataSet(i));
			}
		}

		DefaultDataSetsStatic.AddRange(dataSets);
	}
}
