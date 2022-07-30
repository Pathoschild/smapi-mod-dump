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
using SpriteMaster.Types.Fixed;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample.Scalers.xBRZ.Structures;

internal abstract class AbstractScaler {
	internal readonly int Scale;
	internal readonly Config Configuration;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	protected AbstractScaler(int scale, Config configuration) {
		Scale = scale;
		Configuration = configuration;
	}

	internal abstract void BlendLineSteep(Color16 color, ref OutputMatrix matrix);
	internal abstract void BlendLineSteepAndShallow(Color16 color, ref OutputMatrix matrix);
	internal abstract void BlendLineShallow(Color16 color, ref OutputMatrix matrix);
	internal abstract void BlendLineDiagonal(Color16 color, ref OutputMatrix matrix);
	internal abstract void BlendCorner(Color16 color, ref OutputMatrix matrix);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	protected void AlphaBlend(int n, int m, ref Color16 dstRef, Color16 color) {
		//assert n < 256 : "possible overflow of (color & redMask) * N";
		//assert m < 256 : "possible overflow of (color & redMask) * N + (dst & redMask) * (M - N)";
		//assert 0 < n && n < m : "0 < N && N < M";

		//this works because 8 upper bits are free
		var dst = dstRef;
		int mn = m - n;
		var r = BlendComponent(n, m, mn, dst.R, color.R);
		var g = BlendComponent(n, m, mn, dst.G, color.G);
		var b = BlendComponent(n, m, mn, dst.B, color.B);
		var a = BlendComponent(n, m, mn, dst.A, color.A);
		dstRef = new(r, g, b, a);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static Fixed16 BlendComponent(int n, int m, int mn, Fixed16 inComponent, Fixed16 setComponent) {
		var blend = (long)setComponent.Value * n + (long)inComponent.Value * mn;
		var component = (Fixed16)(blend / m);
		return component;
	}
}

internal sealed class Scaler2X : AbstractScaler {
	internal new const int Scale = 2;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Scaler2X(in Config config) : base(Scale, config) { }

	internal override void BlendLineShallow(Color16 color, ref OutputMatrix matrix) {
		AlphaBlend(1, 4, ref matrix[Scale - 1, 0], color);
		AlphaBlend(3, 4, ref matrix[Scale - 1, 1], color);
	}

	internal override void BlendLineSteep(Color16 color, ref OutputMatrix matrix) {
		AlphaBlend(1, 4, ref matrix[0, Scale - 1], color);
		AlphaBlend(3, 4, ref matrix[1, Scale - 1], color);
	}

	internal override void BlendLineSteepAndShallow(Color16 color, ref OutputMatrix matrix) {
		AlphaBlend(1, 4, ref matrix[1, 0], color);
		AlphaBlend(1, 4, ref matrix[0, 1], color);
		AlphaBlend(5, 6, ref matrix[1, 1], color); //[!] fixes 7/8 used in xBR
	}

	internal override void BlendLineDiagonal(Color16 color, ref OutputMatrix matrix) {
		AlphaBlend(1, 2, ref matrix[1, 1], color);
	}

	internal override void BlendCorner(Color16 color, ref OutputMatrix matrix) {
		//model a round corner
		AlphaBlend(43, 200, ref matrix[1, 1], color); //exact: 1 - pi/4 = 0.2146018366
	}
}

internal sealed class Scaler3X : AbstractScaler {
	internal new const int Scale = 3;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Scaler3X(in Config config) : base(Scale, config) { }

	internal override void BlendLineShallow(Color16 color, ref OutputMatrix matrix) {
		AlphaBlend(1, 4, ref matrix[Scale - 1, 0], color);
		AlphaBlend(1, 4, ref matrix[Scale - 2, 2], color);
		AlphaBlend(3, 4, ref matrix[Scale - 1, 1], color);
		matrix[Scale - 1, 2] = color;
	}

	internal override void BlendLineSteep(Color16 color, ref OutputMatrix matrix) {
		AlphaBlend(1, 4, ref matrix[0, Scale - 1], color);
		AlphaBlend(1, 4, ref matrix[2, Scale - 2], color);
		AlphaBlend(3, 4, ref matrix[1, Scale - 1], color);
		matrix[2, Scale - 1] = color;
	}

	internal override void BlendLineSteepAndShallow(Color16 color, ref OutputMatrix matrix) {
		AlphaBlend(1, 4, ref matrix[2, 0], color);
		AlphaBlend(1, 4, ref matrix[0, 2], color);
		AlphaBlend(3, 4, ref matrix[2, 1], color);
		AlphaBlend(3, 4, ref matrix[1, 2], color);
		matrix[2, 2] = color;
	}

	internal override void BlendLineDiagonal(Color16 color, ref OutputMatrix matrix) {
		AlphaBlend(1, 8, ref matrix[1, 2], color);
		AlphaBlend(1, 8, ref matrix[2, 1], color);
		AlphaBlend(7, 8, ref matrix[2, 2], color);
	}

	internal override void BlendCorner(Color16 color, ref OutputMatrix matrix) {
		//model a round corner
		AlphaBlend(91, 200, ref matrix[2, 2], color); //exact: 0.4545939598
#if true
		AlphaBlend(3, 200, ref matrix[2, 1], color); //0.01413008627 -> negligible + avoid conflicts with other rotations for this odd scale
		AlphaBlend(3, 200, ref matrix[1, 2], color); //0.01413008627
#endif
	}
}

internal sealed class Scaler4X : AbstractScaler {
	internal new const int Scale = 4;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Scaler4X(in Config config) : base(Scale, config) { }

	internal override void BlendLineShallow(Color16 color, ref OutputMatrix matrix) {
		AlphaBlend(1, 4, ref matrix[Scale - 1, 0], color);
		AlphaBlend(1, 4, ref matrix[Scale - 2, 2], color);
		AlphaBlend(3, 4, ref matrix[Scale - 1, 1], color);
		AlphaBlend(3, 4, ref matrix[Scale - 2, 3], color);
		matrix[Scale - 1, 2] = color;
		matrix[Scale - 1, 3] = color;
	}

	internal override void BlendLineSteep(Color16 color, ref OutputMatrix matrix) {
		AlphaBlend(1, 4, ref matrix[0, Scale - 1], color);
		AlphaBlend(1, 4, ref matrix[2, Scale - 2], color);
		AlphaBlend(3, 4, ref matrix[1, Scale - 1], color);
		AlphaBlend(3, 4, ref matrix[3, Scale - 2], color);
		matrix[2, Scale - 1] = color;
		matrix[3, Scale - 1] = color;
	}

	internal override void BlendLineSteepAndShallow(Color16 color, ref OutputMatrix matrix) {
		AlphaBlend(3, 4, ref matrix[3, 1], color);
		AlphaBlend(3, 4, ref matrix[1, 3], color);
		AlphaBlend(1, 4, ref matrix[3, 0], color);
		AlphaBlend(1, 4, ref matrix[0, 3], color);
		AlphaBlend(1, 3, ref matrix[2, 2], color); //[!] fixes 1/4 used in xBR
		matrix[3, 3] = color;
		matrix[3, 2] = color;
		matrix[2, 3] = color;
	}

	internal override void BlendLineDiagonal(Color16 color, ref OutputMatrix matrix) {
		AlphaBlend(1, 2, ref matrix[Scale - 1, Scale / 2], color);
		AlphaBlend(1, 2, ref matrix[Scale - 2, Scale / 2 + 1], color);
		matrix[Scale - 1, Scale - 1] = color;
	}

	internal override void BlendCorner(Color16 color, ref OutputMatrix matrix) {
		//model a round corner
		AlphaBlend(137, 200, ref matrix[3, 3], color); //exact: 0.6848532563
		AlphaBlend(17, 200, ref matrix[3, 2], color); //0.08677704501
		AlphaBlend(17, 200, ref matrix[2, 3], color); //0.08677704501
	}
}

internal sealed class Scaler5X : AbstractScaler {
	internal new const int Scale = 5;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Scaler5X(in Config config) : base(Scale, config) { }

	internal override void BlendLineShallow(Color16 color, ref OutputMatrix matrix) {
		AlphaBlend(1, 4, ref matrix[Scale - 1, 0], color);
		AlphaBlend(1, 4, ref matrix[Scale - 2, 2], color);
		AlphaBlend(1, 4, ref matrix[Scale - 3, 4], color);
		AlphaBlend(3, 4, ref matrix[Scale - 1, 1], color);
		AlphaBlend(3, 4, ref matrix[Scale - 2, 3], color);
		matrix[Scale - 1, 2] = color;
		matrix[Scale - 1, 3] = color;
		matrix[Scale - 1, 4] = color;
		matrix[Scale - 2, 4] = color;
	}

	internal override void BlendLineSteep(Color16 color, ref OutputMatrix matrix) {
		AlphaBlend(1, 4, ref matrix[0, Scale - 1], color);
		AlphaBlend(1, 4, ref matrix[2, Scale - 2], color);
		AlphaBlend(1, 4, ref matrix[4, Scale - 3], color);
		AlphaBlend(3, 4, ref matrix[1, Scale - 1], color);
		AlphaBlend(3, 4, ref matrix[3, Scale - 2], color);
		matrix[2, Scale - 1] = color;
		matrix[3, Scale - 1] = color;
		matrix[4, Scale - 1] = color;
		matrix[4, Scale - 2] = color;
	}

	internal override void BlendLineSteepAndShallow(Color16 color, ref OutputMatrix matrix) {
		AlphaBlend(1, 4, ref matrix[0, Scale - 1], color);
		AlphaBlend(1, 4, ref matrix[2, Scale - 2], color);
		AlphaBlend(3, 4, ref matrix[1, Scale - 1], color);
		AlphaBlend(1, 4, ref matrix[Scale - 1, 0], color);
		AlphaBlend(1, 4, ref matrix[Scale - 2, 2], color);
		AlphaBlend(3, 4, ref matrix[Scale - 1, 1], color);
		AlphaBlend(2, 3, ref matrix[3, 3], color);
		matrix[2, Scale - 1] = color;
		matrix[3, Scale - 1] = color;
		matrix[4, Scale - 1] = color;
		matrix[Scale - 1, 2] = color;
		matrix[Scale - 1, 3] = color;
	}

	internal override void BlendLineDiagonal(Color16 color, ref OutputMatrix matrix) {
		AlphaBlend(1, 8, ref matrix[Scale - 1, Scale / 2], color); //conflict with other rotations for this odd scale
		AlphaBlend(1, 8, ref matrix[Scale - 2, Scale / 2 + 1], color);
		AlphaBlend(1, 8, ref matrix[Scale - 3, Scale / 2 + 2], color);
		AlphaBlend(7, 8, ref matrix[4, 3], color);
		AlphaBlend(7, 8, ref matrix[3, 4], color);
		matrix[4, 4] = color;
	}

	internal override void BlendCorner(Color16 color, ref OutputMatrix matrix) {
		//model a round corner
		AlphaBlend(173, 200, ref matrix[4, 4], color); //exact: 0.8631434088
		AlphaBlend(46, 200, ref matrix[4, 3], color); //0.2306749731
		AlphaBlend(46, 200, ref matrix[3, 4], color); //0.2306749731
#if true
		AlphaBlend(3, 200, ref matrix[4, 2], color); //0.01676812367 -> negligible + avoid conflicts with other rotations for this odd scale
		AlphaBlend(3, 200, ref matrix[2, 4], color); //0.01676812367
#endif
	}
}

internal sealed class Scaler6X : AbstractScaler {
	internal new const int Scale = 6;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Scaler6X(in Config config) : base(Scale, config) { }

	internal override void BlendLineShallow(Color16 color, ref OutputMatrix matrix) {
		AlphaBlend(1, 4, ref matrix[Scale - 1, 0], color);
		AlphaBlend(1, 4, ref matrix[Scale - 2, 2], color);
		AlphaBlend(1, 4, ref matrix[Scale - 3, 4], color);
		AlphaBlend(3, 4, ref matrix[Scale - 1, 1], color);
		AlphaBlend(3, 4, ref matrix[Scale - 2, 3], color);
		AlphaBlend(3, 4, ref matrix[Scale - 3, 5], color);

		matrix[Scale - 1, 2] = color;
		matrix[Scale - 1, 3] = color;
		matrix[Scale - 1, 4] = color;
		matrix[Scale - 1, 5] = color;
		matrix[Scale - 2, 4] = color;
		matrix[Scale - 2, 5] = color;
	}

	internal override void BlendLineSteep(Color16 color, ref OutputMatrix matrix) {
		AlphaBlend(1, 4, ref matrix[0, Scale - 1], color);
		AlphaBlend(1, 4, ref matrix[2, Scale - 2], color);
		AlphaBlend(1, 4, ref matrix[4, Scale - 3], color);
		AlphaBlend(3, 4, ref matrix[1, Scale - 1], color);
		AlphaBlend(3, 4, ref matrix[3, Scale - 2], color);
		AlphaBlend(3, 4, ref matrix[5, Scale - 3], color);

		matrix[2, Scale - 1] = color;
		matrix[3, Scale - 1] = color;
		matrix[4, Scale - 1] = color;
		matrix[5, Scale - 1] = color;
		matrix[4, Scale - 2] = color;
		matrix[5, Scale - 2] = color;
	}

	internal override void BlendLineSteepAndShallow(Color16 color, ref OutputMatrix matrix) {
		AlphaBlend(1, 4, ref matrix[0, Scale - 1], color);
		AlphaBlend(1, 4, ref matrix[2, Scale - 2], color);
		AlphaBlend(3, 4, ref matrix[1, Scale - 1], color);
		AlphaBlend(3, 4, ref matrix[3, Scale - 2], color);
		AlphaBlend(1, 4, ref matrix[Scale - 1, 0], color);
		AlphaBlend(1, 4, ref matrix[Scale - 2, 2], color);
		AlphaBlend(3, 4, ref matrix[Scale - 1, 1], color);
		AlphaBlend(3, 4, ref matrix[Scale - 2, 3], color);

		matrix[2, Scale - 1] = color;
		matrix[3, Scale - 1] = color;
		matrix[4, Scale - 1] = color;
		matrix[5, Scale - 1] = color;
		matrix[4, Scale - 2] = color;
		matrix[5, Scale - 2] = color;
		matrix[Scale - 1, 2] = color;
		matrix[Scale - 1, 3] = color;
	}

	internal override void BlendLineDiagonal(Color16 color, ref OutputMatrix matrix) {
		AlphaBlend(1, 2, ref matrix[Scale - 1, Scale / 2], color);
		AlphaBlend(1, 2, ref matrix[Scale - 2, Scale / 2 + 1], color);
		AlphaBlend(1, 2, ref matrix[Scale - 3, Scale / 2 + 2], color);

		matrix[Scale - 2, Scale - 1] = color;
		matrix[Scale - 1, Scale - 1] = color;
		matrix[Scale - 1, Scale - 2] = color;
	}

	internal override void BlendCorner(Color16 color, ref OutputMatrix matrix) {
		//model a round corner
		AlphaBlend(194, 200, ref matrix[5, 5], color); //exact: 0.9711013910
		AlphaBlend(85, 200, ref matrix[4, 5], color); //0.4236372243
		AlphaBlend(85, 200, ref matrix[5, 4], color); //0.4236372243
		AlphaBlend(11, 200, ref matrix[5, 3], color); //0.05652034508
		AlphaBlend(11, 200, ref matrix[3, 5], color); //0.05652034508
	}
}
