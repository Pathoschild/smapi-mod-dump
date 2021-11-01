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
		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static TimeSpan Multiply (this TimeSpan timespan, int multiplier) => new TimeSpan(timespan.Ticks * multiplier);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static TimeSpan Multiply (this TimeSpan timespan, float multiplier) => new TimeSpan((long)Math.Round(timespan.Ticks * multiplier));

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static TimeSpan Multiply (this TimeSpan timespan, double multiplier) => new TimeSpan((long)Math.Round(timespan.Ticks * multiplier));

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static TimeSpan Divide (this TimeSpan timespan, int divisor) => new TimeSpan(timespan.Ticks / divisor);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static TimeSpan Divide (this TimeSpan timespan, float divisor) => new TimeSpan((long)Math.Round(timespan.Ticks / divisor));

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static TimeSpan Divide (this TimeSpan timespan, double divisor) => new TimeSpan((long)Math.Round(timespan.Ticks / divisor));

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static TimeSpan Double (this TimeSpan timespan) => new TimeSpan(timespan.Ticks << 1);

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public static TimeSpan Halve (this TimeSpan timespan) => new TimeSpan(timespan.Ticks >> 1);
	}
}
