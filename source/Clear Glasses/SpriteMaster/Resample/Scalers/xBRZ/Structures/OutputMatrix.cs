/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using SpriteMaster.Resample.Scalers.xBRZ.Common;
using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Resample.Scalers.xBRZ.Structures;

//access matrix area, top-left at position "out" for image with given width
[StructLayout(LayoutKind.Auto)]
internal ref struct OutputMatrix {
	private readonly Span<Color16> OutSpan;
	private readonly int Width;
	private readonly int N;
	private int Index = 0;
	private int NRow = 0;

	private readonly record struct IntPair(int I, int J);

	private const int MaxScale = Config.MaxScale; // Highest possible scale
	private const int MaxScaleSquared = MaxScale * MaxScale;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal OutputMatrix(int scale, Span<Color16> outSpan, int outWidth) {
		N = (scale - 2) * Rotator.MaxRotations * MaxScaleSquared;
		OutSpan = outSpan;
		Width = outWidth;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal void Move(RotationDegree rotDeg, int outIndex) {
		NRow = N + (int)rotDeg * MaxScaleSquared;
		Index = outIndex;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private readonly int GetIndex(int i, int j) {
		var rot = MatrixRotation[NRow + i * MaxScale + j];
		return Index + rot.J + rot.I * Width;
	}

	internal readonly ref Color16 this[int i, int j] => ref OutSpan[GetIndex(i, j)];

	//calculate input matrix coordinates after rotation at program startup
	private static readonly FixedArray<IntPair> MatrixRotation = new((MaxScale - 1) * MaxScaleSquared * Rotator.MaxRotations);

	static OutputMatrix() {
		for (var n = 2; n < MaxScale + 1; n++) {
			for (var r = 0; r < Rotator.MaxRotations; r++) {
				var nr = (n - 2) * Rotator.MaxRotations * MaxScaleSquared + r * MaxScaleSquared;
				for (var i = 0; i < MaxScale; i++) {
					for (var j = 0; j < MaxScale; j++) {
						MatrixRotation[nr + i * MaxScale + j] = BuildMatrixRotation(r, i, j, n);
					}
				}
			}
		}
	}

	private static IntPair BuildMatrixRotation(int rotDeg, int i, int j, int n) {
		int iOld, jOld;

		if (rotDeg == 0) {
			iOld = i;
			jOld = j;
		}
		else {
			//old coordinates before rotation!
			var old = BuildMatrixRotation(rotDeg - 1, i, j, n);
			iOld = n - 1 - old.J;
			jOld = old.I;
		}

		return new(iOld, jOld);
	}
}
