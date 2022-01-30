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

[ImmutableObject(true)]
unsafe ref struct Kernel3x3 {
	private fixed ulong Data[3 * 3];

	internal readonly Color16 this[int index] => (Color16)Data[index];

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Kernel3x3(Color16 _0, Color16 _1, Color16 _2, Color16 _3, Color16 _4, Color16 _5, Color16 _6, Color16 _7, Color16 _8) {
		Data[0] = _0.AsPacked;
		Data[1] = _1.AsPacked;
		Data[2] = _2.AsPacked;
		Data[3] = _3.AsPacked;
		Data[4] = _4.AsPacked;
		Data[5] = _5.AsPacked;
		Data[6] = _6.AsPacked;
		Data[7] = _7.AsPacked;
		Data[8] = _8.AsPacked;
	}
}
