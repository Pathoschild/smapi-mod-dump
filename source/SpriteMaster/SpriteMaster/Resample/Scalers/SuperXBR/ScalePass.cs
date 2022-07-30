/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

#if !SHIPPING
using Microsoft.Toolkit.HighPerformance;
using SpriteMaster.Colors;
using SpriteMaster.Extensions;
using SpriteMaster.Resample.Scalers.SuperXBR.Cg;
using SpriteMaster.Types;
using SpriteMaster.Types.Fixed;
using System;

namespace SpriteMaster.Resample.Scalers.SuperXBR;

internal sealed partial class Scaler {
	// https://github.com/libretro/common-shaders/blob/master/xbr/super-xbr-6p-small-details.cgp
	// https://github.com/libretro/common-shaders/blob/master/xbr/shaders/super-xbr/super-xbr-small-details-pass0.cg
	// https://github.com/libretro/common-shaders/blob/master/xbr/shaders/super-xbr/super-xbr-small-details-pass1.cg
	// https://github.com/libretro/common-shaders/blob/master/xbr/shaders/super-xbr/super-xbr-small-details-pass2.cg
	private void Scale(ReadOnlySpan<Color16> source, Vector2I sourceSize, Span<Color16> target, Vector2I targetSize) {
		var source0 = Fixed16.ConvertToRealF(source.Elements()).Cast<float, Float4>();

		//var target0 = SpanExt.Make<Float4>(source.Length);
		//var pass0 = new Passes.Pass0(Configuration, sourceSize, sourceSize);
		//pass0.Pass(source0, target0);
		var target0 = source0;

		var target1 = SpanExt.Make<Float4>(target.Length);
		var pass1 = new Passes.Pass1(Configuration, sourceSize, targetSize);
		pass1.Pass(source0, target0, target1);

		//var target2 = SpanExt.Make<Float4>(target.Length);
		//var pass2 = new Passes.Pass2(Configuration, targetSize, targetSize);
		//pass2.Pass(target1, target2);
		var target2 = target1;

		for (int i = 0; i < target2.Length; ++i) {
			target[i].R = target2[i].R.ScalarToValue16();
			target[i].G = target2[i].G.ScalarToValue16();
			target[i].B = target2[i].B.ScalarToValue16();
			target[i].A = target2[i].A.ScalarToValue16();
		}
	}
}
#endif
