using SpriteMaster.Extensions;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster {
	internal sealed class TexelTimer {
		private double DurationPerTexel = 0.0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Add(int texels, TimeSpan duration) {
			// This isn't a true running average - we'd lose too much precision over time when the sample count got too high, and I'm lazy.
			// Avoid a division by zero
			if (texels == 0) {
				return;
			}

			DurationPerTexel += (double)duration.Ticks / texels;
			DurationPerTexel *= 0.5;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Add (TextureAction action, TimeSpan duration) {
			Add(action.Texels, duration);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal TimeSpan Estimate (int texels) {
			return new TimeSpan((DurationPerTexel * texels).NextLong());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal TimeSpan Estimate(TextureAction action) {
			return Estimate(action.Texels);
		}
	}
}
