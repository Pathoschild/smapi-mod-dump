using System.ComponentModel;
using System.Runtime.CompilerServices;
using SpriteMaster.xBRZ.Common;

namespace SpriteMaster.xBRZ.Scalers {
	//access matrix area, top-left at position "out" for image with given width
	internal unsafe struct OutputMatrix {
		private readonly uint* _out;
		private readonly int _outWidth;
		private readonly int _n;
		private int _outi;
		private int _nr;

		[ImmutableObject(true)]
		private struct IntPair {
			public readonly int i;
			public readonly int j;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal IntPair(int i, int j) {
				this.i = i;
				this.j = j;
			}
		}

		private const int MaxScale = Config.MaxScale; // Highest possible scale
		private const int MaxScaleSquared = MaxScale * MaxScale;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public OutputMatrix (int scale, uint* outPtr, int outWidth) {
			_n = (scale - 2) * (Rotator.MaxRotations * MaxScaleSquared);
			_out = outPtr;
			_outWidth = outWidth;
			_outi = 0;
			_nr = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Move (RotationDegree rotDeg, int outi) {
			_nr = _n + (int)rotDeg * MaxScaleSquared;
			_outi = outi;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GetIndex (int i, int j) {
			var rot = MatrixRotation[_nr + i * MaxScale + j];
			return (_outi + rot.j + rot.i * _outWidth);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set (int i, int j, uint value) {
			_out[GetIndex(i, j)] = value;
		}

		// TODO : I _really_ don't like this but I don't want to fully refactor ScalerImplementations.cs right now.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref uint Ref (int i, int j) {
			return ref _out[GetIndex(i, j)];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint Get (int i, int j) {
			return _out[GetIndex(i, j)];
		}

		//calculate input matrix coordinates after rotation at program startup
		private static readonly IntPair[] MatrixRotation = new IntPair[(MaxScale - 1) * MaxScaleSquared * Rotator.MaxRotations];

		static OutputMatrix () {
			for (var n = 2; n < MaxScale + 1; n++) {
				for (var r = 0; r < Rotator.MaxRotations; r++) {
					var nr = (n - 2) * (Rotator.MaxRotations * MaxScaleSquared) + r * MaxScaleSquared;
					for (var i = 0; i < MaxScale; i++) {
						for (var j = 0; j < MaxScale; j++) {
							MatrixRotation[nr + i * MaxScale + j] = BuildMatrixRotation(r, i, j, n);
						}
					}
				}
			}
		}

		private static IntPair BuildMatrixRotation (int rotDeg, int i, int j, int n) {
			int iOld, jOld;

			if (rotDeg == 0) {
				iOld = i;
				jOld = j;
			}
			else {
				//old coordinates before rotation!
				var old = BuildMatrixRotation(rotDeg - 1, i, j, n);
				iOld = n - 1 - old.j;
				jOld = old.i;
			}

			return new IntPair(iOld, jOld);
		}
	}
}
