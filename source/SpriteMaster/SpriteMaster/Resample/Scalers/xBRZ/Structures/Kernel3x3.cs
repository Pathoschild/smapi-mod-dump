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

[ImmutableObject(true)]
[StructLayout(LayoutKind.Sequential, Size = (3 * 3 * sizeof(ulong)))]
internal unsafe ref struct Kernel3X3 {
	private readonly ulong Offset;
	private readonly ulong* Data => (ulong*)Unsafe.AsPointer(ref Unsafe.AsRef(Offset));

	internal readonly Color16 this[int index] => (Color16)Data[index];

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Kernel3X3(Color16 _0, Color16 _1, Color16 _2, Color16 _3, Color16 _4, Color16 _5, Color16 _6, Color16 _7, Color16 _8) : this() {
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
