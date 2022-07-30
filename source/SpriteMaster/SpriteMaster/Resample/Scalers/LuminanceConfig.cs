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
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample.Scalers;

internal abstract class LuminanceConfig : Config {
	internal readonly double LuminanceWeight;
	internal readonly double ChrominanceWeight;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	protected LuminanceConfig(
		Vector2B wrapped,
		bool hasAlpha,
		bool gammaCorrected,
		double luminanceWeight
	) : base(
		wrapped,
		hasAlpha,
		gammaCorrected
	) {
		LuminanceWeight = luminanceWeight;

		var adjustedLuminanceWeight = luminanceWeight / (luminanceWeight + 1.0);
		LuminanceWeight = adjustedLuminanceWeight * 2.0;
		ChrominanceWeight = (1.0 - adjustedLuminanceWeight) * 2.0;
	}
}
