/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Colors;
using SpriteMaster.Types;
using System.Runtime.CompilerServices;
using static SpriteMaster.Colors.ColorHelpers;

namespace SpriteMaster.Resample.Scalers.xBRZ.Color;

class ColorDist {
	protected readonly Config Configuration;
	private readonly YccConfig YccConfiguration;

	// TODO : Only sRGB presently has the linearizer/delinearizer implemented.
	private static readonly ColorSpace CurrentColorSpace = ColorSpace.sRGB_Precise;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal ColorDist(Config cfg) {
		Configuration = cfg;
		YccConfiguration = new() {
			LuminanceWeight = Configuration.LuminanceWeight,
			ChrominanceWeight = Configuration.ChrominanceWeight
		};
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal uint ColorDistance(in Color16 pix1, in Color16 pix2) {
		return Resample.Scalers.Common.ColorDistance(
			useRedmean: Configuration.UseRedmean,
			gammaCorrected: Configuration.GammaCorrected,
			hasAlpha: Configuration.HasAlpha,
			pix1: pix1,
			pix2: pix2,
			yccConfig: YccConfiguration
		);
	}
}
