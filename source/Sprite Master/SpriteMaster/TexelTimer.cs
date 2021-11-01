/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster {
	internal sealed class TexelTimer {
		private double DurationPerTexel = 0.0;
		private const int MaxDurationCounts = 50;

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal void Add(int texels, TimeSpan duration) {
			// Avoid a division by zero
			if (texels == 0) {
				return;
			}

			var texelDuration = (double)duration.Ticks / texels;
			DurationPerTexel -= DurationPerTexel / MaxDurationCounts;
			DurationPerTexel += texelDuration / MaxDurationCounts;
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal void Add (TextureAction action, TimeSpan duration) {
			Add(action.Texels, duration);
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal TimeSpan Estimate (int texels) {
			return TimeSpan.FromTicks((DurationPerTexel * texels).NextLong());
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal TimeSpan Estimate(TextureAction action) {
			return Estimate(action.Texels);
		}
	}
}
