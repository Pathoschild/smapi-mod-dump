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
		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static unsafe double StandardDeviation (this in FixedSpan<int> data, int startIndex = 0, int count = 0) {
			return StandardDeviation((int*)data.Pointer, data.Length, startIndex, count);
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static unsafe double StandardDeviation (int* data, int length, int startIndex = 0, int count = 0) {
			//return StandardDeviation(new FixedSpan<int>(data, length), startIndex: startIndex, count: count);

			Contract.AssertPositiveOrZero(startIndex);
			Contract.AssertLess(startIndex, length);
			int endIndex = startIndex + count;
			Contract.AssertLess(startIndex, endIndex);
			Contract.AssertLess(endIndex, length);

			double sum = 0.0;
			foreach (int i in startIndex.RangeTo(endIndex)) {
				sum += data[i];
			}
			sum /= count;

			double meanDifference = 0.0;
			foreach (int i in startIndex.RangeTo(endIndex)) {
				var difference = Math.Abs(data[i] - sum);
				meanDifference = difference * difference;
			}
			meanDifference /= count;
			meanDifference = Math.Sqrt(meanDifference);

			return meanDifference;
		}
	}
}
