/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Benchmarks.Sprites.Methods.ReversePremultiply16.ByRef;
internal static class ScalarBranchless {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Reverse(Span<Color16> data, Vector2I size) {
		ushort lowPass = Common.PremultiplicationLowPass;

		for (int i = 0; i < data.Length; ++i) {
			ref var refItem = ref Unsafe.Add(ref MemoryMarshal.GetReference(data), i);
			var item = refItem;

			var alpha = item.A;

			ushort alphaMask = (ushort)((alpha.Value > lowPass).As<ushort>() - 1);
			alpha = (ushort)(alpha.Value | alphaMask);

			refItem.SetRgb(
				item.R.ClampedDivide(alpha),
				item.G.ClampedDivide(alpha),
				item.B.ClampedDivide(alpha)
			);
		}
	}
}