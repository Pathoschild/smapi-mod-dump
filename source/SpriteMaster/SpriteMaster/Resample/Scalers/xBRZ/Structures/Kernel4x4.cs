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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Resample.Scalers.xBRZ.Structures;

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
[StructLayout(LayoutKind.Sequential, Size = (4 * 4 * sizeof(ulong)))]
internal unsafe ref struct Kernel4X4 {
	private readonly ulong Offset;
	private readonly Color16* Data => (Color16*)Unsafe.AsPointer(ref Unsafe.AsRef(Offset));

	internal readonly Color16 A => Data[0x0];
	internal readonly Color16 B => Data[0x1];
	internal readonly Color16 C => Data[0x2];
	internal readonly Color16 D => Data[0x3];
	internal readonly Color16 E => Data[0x4];
	internal readonly Color16 F => Data[0x5];
	internal readonly Color16 G => Data[0x6];
	internal readonly Color16 H => Data[0x7];
	internal readonly Color16 I => Data[0x8];
	internal readonly Color16 J => Data[0x9];
	internal readonly Color16 K => Data[0xA];
	internal readonly Color16 L => Data[0xB];
	internal readonly Color16 M => Data[0xC];
	internal readonly Color16 N => Data[0xD];
	internal readonly Color16 O => Data[0xE];
	internal readonly Color16 P => Data[0xF];

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Kernel4X4(
		Color16 _0,
		Color16 _1,
		Color16 _2,
		Color16 _3,
		Color16 _4,
		Color16 _5,
		Color16 _6,
		Color16 _7,
		Color16 _8,
		Color16 _9,
		Color16 _10,
		Color16 _11,
		Color16 _12,
		Color16 _13,
		Color16 _14,
		Color16 _15
	) : this() {
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
