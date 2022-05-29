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
internal unsafe ref struct Kernel4X4 {
#pragma warning disable CS0649
	private fixed ulong Data[4 * 4];
#pragma warning restore CS0649

	internal readonly Color16 A => (Color16)Data[0x0];
	internal readonly Color16 B => (Color16)Data[0x1];
	internal readonly Color16 C => (Color16)Data[0x2];
	internal readonly Color16 D => (Color16)Data[0x3];
	internal readonly Color16 E => (Color16)Data[0x4];
	internal readonly Color16 F => (Color16)Data[0x5];
	internal readonly Color16 G => (Color16)Data[0x6];
	internal readonly Color16 H => (Color16)Data[0x7];
	internal readonly Color16 I => (Color16)Data[0x8];
	internal readonly Color16 J => (Color16)Data[0x9];
	internal readonly Color16 K => (Color16)Data[0xA];
	internal readonly Color16 L => (Color16)Data[0xB];
	internal readonly Color16 M => (Color16)Data[0xC];
	internal readonly Color16 N => (Color16)Data[0xD];
	internal readonly Color16 O => (Color16)Data[0xE];
	internal readonly Color16 P => (Color16)Data[0xF];

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
	) {
		Data[0] = _0.AsPacked;
		Data[1] = _1.AsPacked;
		Data[2] = _2.AsPacked;
		Data[3] = _3.AsPacked;
		Data[4] = _4.AsPacked;
		Data[5] = _5.AsPacked;
		Data[6] = _6.AsPacked;
		Data[7] = _7.AsPacked;
		Data[8] = _8.AsPacked;
		Data[9] = _9.AsPacked;
		Data[10] = _10.AsPacked;
		Data[11] = _11.AsPacked;
		Data[12] = _12.AsPacked;
		Data[13] = _13.AsPacked;
		Data[14] = _14.AsPacked;
		Data[15] = _15.AsPacked;
	}
}
