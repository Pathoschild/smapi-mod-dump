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
using SpriteMaster.Resample.Scalers.SuperXBR.Cg;
using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;

using static SpriteMaster.Resample.Scalers.SuperXBR.Cg.CgMath;

namespace SpriteMaster.Resample.Scalers.SuperXBR.Passes;

internal abstract class Pass {
	protected readonly Config Configuration;
	protected readonly Vector2I SourceSize;
	protected readonly Vector2I TargetSize;
	protected Pass(Config config, Vector2I sourceSize, Vector2I targetSize) {
		Configuration = config;
		SourceSize = sourceSize;
		TargetSize = targetSize;
	}

	protected abstract float[] PixelWeights { get; }

	protected float WeightedDifferenceDiagonal(DiffTexel b0, DiffTexel b1, DiffTexel c0, DiffTexel c1, DiffTexel c2, DiffTexel d0, DiffTexel d1, DiffTexel d2, DiffTexel d3, DiffTexel e1, DiffTexel e2, DiffTexel e3, DiffTexel f2, DiffTexel f3) {
		return (
			PixelWeights[0] * (Difference(c1, c2) + Difference(c1, c0) + Difference(e2, e1) + Difference(e2, e3)) +
			PixelWeights[1] * (Difference(d2, d3) + Difference(d0, d1)) +
			PixelWeights[2] * (Difference(d1, d3) + Difference(d0, d2)) +
			PixelWeights[3] * Difference(d1, d2) +
			PixelWeights[4] * (Difference(c0, c2) + Difference(e1, e3)) +
			PixelWeights[5] * (Difference(b0, b1) + Difference(f2, f3))
		);
	}

	protected float WeightedDifferenceHorizontalVertical(DiffTexel i1, DiffTexel i2, DiffTexel i3, DiffTexel i4, DiffTexel e1, DiffTexel e2, DiffTexel e3, DiffTexel e4) {
		return (
			PixelWeights[3] * (Difference(i1, i2) + Difference(i3, i4)) +
			PixelWeights[0] * (Difference(i1, e1) + Difference(i2, e2) + Difference(i3, e3) + Difference(i4, e4))
		);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	protected int GetX(int x, Vector2I size) {
		if (Configuration.Wrapped.X) {
			x = (x + size.Width) % size.Width;
		}
		else {
			x = Math.Clamp(x, 0, size.Width - 1);
		}
		return x;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	protected int GetY(int y, Vector2I size) {
		if (Configuration.Wrapped.Y) {
			y = (y + size.Height) % size.Height;
		}
		else {
			y = Math.Clamp(y, 0, size.Height - 1);
		}
		return y;
	}

	protected readonly ref struct Texture {
		private readonly Pass Pass;
		private readonly ReadOnlySpan<Float4> Data;
		private readonly Vector2I Size;

		internal Texture(Pass pass, ReadOnlySpan<Float4> data, Vector2I size) {
			Pass = pass;
			Data = data;
			Size = size;
		}
		private int GetOffset(int x, int y) {
			if (Pass.Configuration.Wrapped.X) {
				x = (x + Size.Width) % Size.Width;
			}
			else {
				x = Math.Clamp(x, 0, Size.Width - 1);
			}
			if (Pass.Configuration.Wrapped.Y) {
				y = (y + Size.Height) % Size.Height;
			}
			else {
				y = Math.Clamp(y, 0, Size.Height - 1);
			}

			var offset = (y * Size.Width) + x;
			if (offset < 0 || offset >= Data.Length) {
				throw new IndexOutOfRangeException($"{offset} <\\> {Data.Length}");
			}
			return offset;
		}

		internal readonly Float4 Sample(int x, int y) {
			return Data[GetOffset(x, y)];
		}

		internal readonly Float4 Sample(Float2 xy) => Sample((int)xy.X, (int)xy.Y);

		internal readonly Float4 Sample(Vector2I xy) => Sample(xy.X, xy.Y);
	}
}
#endif
