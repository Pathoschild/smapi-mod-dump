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

namespace SpriteMaster.xBRZ.Color {
	internal sealed class ColorEq : ColorDist {
		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public ColorEq (in Config configuration) : base(in configuration) { }

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public bool IsColorEqual (uint color1, uint color2) {
			var eqColorThres = Configuration.EqualColorTolerancePow2;
			return DistYCbCr(color1, color2) < eqColorThres;
		}
	}
}
