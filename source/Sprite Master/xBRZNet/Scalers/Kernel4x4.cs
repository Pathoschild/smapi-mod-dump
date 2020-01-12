using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SpriteMaster.xBRZ.Scalers {
	/*
			input kernel area naming convention:
			-----------------
			| A | B | C | D |
			----|---|---|---|
			| E | F | G | H | //evalute the four corners between F, G, J, K
			----|---|---|---| //input pixel is at position F
			| I | J | K | L |
			----|---|---|---|
			| M | N | O | P |
			-----------------
	*/
	// ReSharper disable once InconsistentNaming
	[ImmutableObject(true)]
	internal unsafe ref struct Kernel4x4 {
		private fixed uint Data[4 * 4];

		public uint this[int index] {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Data[index];
		}

		public uint A { get => Data[0]; }
		public uint B { get => Data[1]; }
		public uint C { get => Data[2]; }
		public uint D { get => Data[3]; }
		public uint E { get => Data[4]; }
		public uint F { get => Data[5]; }
		public uint G { get => Data[6]; }
		public uint H { get => Data[7]; }
		public uint I { get => Data[8]; }
		public uint J { get => Data[9]; }
		public uint K { get => Data[10]; }
		public uint L { get => Data[11]; }
		public uint M { get => Data[12]; }
		public uint N { get => Data[13]; }
		public uint O { get => Data[14]; }
		public uint P { get => Data[15]; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Kernel4x4 (
			uint _0,
			uint _1,
			uint _2,
			uint _3,
			uint _4,
			uint _5,
			uint _6,
			uint _7,
			uint _8,
			uint _9,
			uint _10,
			uint _11,
			uint _12,
			uint _13,
			uint _14,
			uint _15
		) {
			Data[0] = _0;
			Data[1] = _1;
			Data[2] = _2;
			Data[3] = _3;
			Data[4] = _4;
			Data[5] = _5;
			Data[6] = _6;
			Data[7] = _7;
			Data[8] = _8;
			Data[9] = _9;
			Data[10] = _10;
			Data[11] = _11;
			Data[12] = _12;
			Data[13] = _13;
			Data[14] = _14;
			Data[15] = _15;
		}
	}
}
