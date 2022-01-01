/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SpriteMaster.xBRZ.Scalers;

/*
		input kernel area naming convention:
		┌───┬───┬───┬───┐
		│ A │ B │ C │ D │
		├───┼───┼───┼───┤
		│ E │ F │ G │ H │ //evalute the four corners between F, G, J, K
		├───┼───┼───┼───┤ //input pixel is at position F
		│ I │ J │ K │ L │
		├───┼───┼───┼───┤
		│ M │ N │ O │ P │
		└───┴───┴───┴───┘
*/
[ImmutableObject(true)]
unsafe ref struct Kernel4x4 {
	private fixed uint Data[4 * 4];

	internal readonly uint this[int index] => Data[index];

	internal readonly uint A => Data[0];
	internal readonly uint B => Data[1];
	internal readonly uint C => Data[2];
	internal readonly uint D => Data[3];
	internal readonly uint E => Data[4];
	internal readonly uint F => Data[5];
	internal readonly uint G => Data[6];
	internal readonly uint H => Data[7];
	internal readonly uint I => Data[8];
	internal readonly uint J => Data[9];
	internal readonly uint K => Data[10];
	internal readonly uint L => Data[11];
	internal readonly uint M => Data[12];
	internal readonly uint N => Data[13];
	internal readonly uint O => Data[14];
	internal readonly uint P => Data[15];

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Kernel4x4(
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
