/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.Runtime.CompilerServices;

namespace SpriteMaster.xBRZ.Color;

sealed class ColorEq : ColorDist {
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal ColorEq (Config configuration) : base(configuration) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal bool IsColorEqual (uint color1, uint color2) {
		var equalColorThreshold = Configuration.EqualColorToleranceSq;
		return DistYCbCr(color1, color2) < equalColorThreshold;
	}
}
