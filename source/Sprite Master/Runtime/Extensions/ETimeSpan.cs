/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions {
	public static class ETimeSpan {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TimeSpan Multiply(this TimeSpan timespan, int multiplier) {
			return new TimeSpan(timespan.Ticks * multiplier);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TimeSpan Multiply (this TimeSpan timespan, float multiplier) {
			return new TimeSpan((long)Math.Round(timespan.Ticks * multiplier));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TimeSpan Multiply (this TimeSpan timespan, double multiplier) {
			return new TimeSpan((long)Math.Round(timespan.Ticks * multiplier));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TimeSpan Divide (this TimeSpan timespan, int divisor) {
			return new TimeSpan(timespan.Ticks / divisor);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TimeSpan Divide (this TimeSpan timespan, float divisor) {
			return new TimeSpan((long)Math.Round(timespan.Ticks / divisor));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TimeSpan Divide (this TimeSpan timespan, double divisor) {
			return new TimeSpan((long)Math.Round(timespan.Ticks / divisor));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TimeSpan Double (this TimeSpan timespan) {
			return new TimeSpan(timespan.Ticks << 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TimeSpan Halve (this TimeSpan timespan) {
			return new TimeSpan(timespan.Ticks >> 1);
		}
	}
}
