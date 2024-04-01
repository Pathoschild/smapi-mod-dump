/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
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
internal ref struct Kernel4X4 {
	internal readonly Color16 A, B, C, D;
	internal readonly Color16 E, F, G, H;
	internal readonly Color16 I, J, K, L;
	internal readonly Color16 M, N, O, P;

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
		A = _0;
		B = _1;
		C = _2;
		D = _3;
		E = _4;
		F = _5;
		G = _6;
		H = _7;
		I = _8;
		J = _9;
		K = _10;
		L = _11;
		M = _12;
		N = _13;
		O = _14;
		P = _15;
	}
}
