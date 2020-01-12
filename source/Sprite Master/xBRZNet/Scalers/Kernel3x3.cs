using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SpriteMaster.xBRZ.Scalers {
	// ReSharper disable once InconsistentNaming
	[ImmutableObject(true)]
	internal unsafe ref struct Kernel3x3 {
		private fixed uint Data[3 * 3];

		public uint this[int index] {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Data[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Kernel3x3(uint _0, uint _1, uint _2, uint _3, uint _4, uint _5, uint _6, uint _7, uint _8) {
			Data[0] = _0;
			Data[1] = _1;
			Data[2] = _2;
			Data[3] = _3;
			Data[4] = _4;
			Data[5] = _5;
			Data[6] = _6;
			Data[7] = _7;
			Data[8] = _8;
		}
	}
}
