/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Toolkit.HighPerformance;
using SpriteMaster.Colors;
using SpriteMaster.Extensions;
using SpriteMaster.Resample.Scalers.SuperXBR.Cg;
using SpriteMaster.Types;
using SpriteMaster.Types.Fixed;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample.Scalers.SuperXBR;
sealed partial class Scaler {
	// https://github.com/libretro/common-shaders/blob/master/xbr/super-xbr-6p-small-details.cgp
	// https://github.com/libretro/common-shaders/blob/master/xbr/shaders/super-xbr/super-xbr-small-details-pass0.cg
	// https://github.com/libretro/common-shaders/blob/master/xbr/shaders/super-xbr/super-xbr-small-details-pass1.cg
	// https://github.com/libretro/common-shaders/blob/master/xbr/shaders/super-xbr/super-xbr-small-details-pass2.cg

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private void Scale(ReadOnlySpan<Color16> source, Vector2I sourceSize, Span<Color16> target, Vector2I targetSize) {
		var source0 = Fixed16.ConvertToReal(source.Elements()).Cast<float, Float4>();

		var target0 = SpanExt.MakeUninitialized<Float4>(source.Length);
		var pass0 = new Passes.Pass0(Configuration, sourceSize, sourceSize);
		pass0.Pass(source0, target0);

		var target1 = SpanExt.MakeUninitialized<Float4>(target.Length);
		var pass1 = new Passes.Pass1(Configuration, sourceSize, targetSize);
		pass1.Pass(source0, target0, target1);

		var target2 = SpanExt.MakeUninitialized<Float4>(target.Length);
		var pass2 = new Passes.Pass2(Configuration, targetSize, targetSize);
		pass2.Pass(target1, target2);

		for (int i = 0; i < target2.Length; ++i) {
			target[i].R = ColorHelpers.ScalarToValue16(target2[i].R);
			target[i].G = ColorHelpers.ScalarToValue16(target2[i].G);
			target[i].B = ColorHelpers.ScalarToValue16(target2[i].B);
			target[i].A = ColorHelpers.ScalarToValue16(target2[i].A);
		}
	}
}
