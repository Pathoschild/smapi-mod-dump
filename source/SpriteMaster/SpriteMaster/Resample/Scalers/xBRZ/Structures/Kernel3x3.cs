/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using SpriteMaster.Types;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Resample.Scalers.xBRZ.Structures;

[ImmutableObject(true)]
[StructLayout(LayoutKind.Sequential, Size = (3 * 3 * sizeof(ulong)))]
internal ref struct Kernel3X3 {
	[UsedImplicitly]
	private readonly ulong A, B, C;
	[UsedImplicitly]
	private readonly ulong E, F, G;
	[UsedImplicitly]
	private readonly ulong I, J, K;

	internal readonly Color16 this[int index] => (Color16)Unsafe.Add(ref Unsafe.AsRef(A), index);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal Kernel3X3(Color16 _0, Color16 _1, Color16 _2, Color16 _3, Color16 _4, Color16 _5, Color16 _6, Color16 _7, Color16 _8) : this() {
		A = _0.AsPacked;
		B = _1.AsPacked;
		C = _2.AsPacked;
		E = _3.AsPacked;
		F = _4.AsPacked;
		G = _5.AsPacked;
		I = _6.AsPacked;
		J = _7.AsPacked;
		K = _8.AsPacked;
	}
}
