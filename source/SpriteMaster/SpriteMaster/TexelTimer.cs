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

namespace SpriteMaster;

internal sealed class TexelTimer {
	private long TotalDuration = 0;
	private long TotalTexels = 0;

	private double DurationPerTexel => (TotalTexels == 0) ? 0.0 : (double)TotalDuration / TotalTexels;

	internal void Reset() {
		TotalDuration = 0;
		TotalTexels = 0;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal void Add(int texels, TimeSpan duration) {
		// Avoid a division by zero
		if (texels == 0) {
			return;
		}

		TotalDuration += duration.Ticks;
		TotalTexels += texels;
		//var texelDuration = (double)duration.Ticks / texels;
		//DurationPerTexel -= DurationPerTexel / MaxDurationCounts;
		//DurationPerTexel += texelDuration / MaxDurationCounts;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal void Add(TextureAction action, TimeSpan duration) => Add(action.Size, duration);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal TimeSpan Estimate(int texels) => TimeSpan.FromTicks((DurationPerTexel * texels).NextLong());

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal TimeSpan Estimate(TextureAction action) => Estimate(action.Size);
}
