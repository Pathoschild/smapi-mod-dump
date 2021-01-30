/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/


using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions {
	public static class Statistics {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe double StandardDeviation (this in Types.Span<int> data, int startIndex = 0, int count = 0) {
			return StandardDeviation((int*)data.Pointer, data.Length, startIndex, count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe double StandardDeviation (int* data, int length, int startIndex = 0, int count = 0) {
			//return StandardDeviation(new Span<int>(data, length), startIndex: startIndex, count: count);

			Contract.AssertPositiveOrZero(startIndex);
			Contract.AssertLess(startIndex, length);
			int endIndex = startIndex + count;
			Contract.AssertLess(startIndex, endIndex);
			Contract.AssertLess(endIndex, length);

			double sum = 0.0;
			foreach (int i in startIndex..endIndex) {
				sum += data[i];
			}
			sum /= count;

			double meanDifference = 0.0;
			foreach (int i in startIndex..endIndex) {
				var difference = Math.Abs(data[i] - sum);
				meanDifference = difference * difference;
			}
			meanDifference /= count;
			meanDifference = Math.Sqrt(meanDifference);

			return meanDifference;
		}
	}
}
