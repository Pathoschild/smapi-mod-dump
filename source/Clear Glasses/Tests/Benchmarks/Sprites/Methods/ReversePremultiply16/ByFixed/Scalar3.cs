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
using System.Runtime.CompilerServices;

namespace Benchmarks.Sprites.Methods.ReversePremultiply16.ByFixed;
internal static class Scalar3 {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe void Reverse(Span<Color16> data, Vector2I size) {
		ushort lowPass = Common.PremultiplicationLowPass;

		fixed (Color16* pDataRef = data) {
			Color16* pData = pDataRef;

			for (int i = 0; i < data.Length; ++i) {
				var item = *pData;

				var alpha = item.A;

				switch (alpha.Value) {
					case ushort.MaxValue:
					case var _ when alpha.Value <= lowPass:
						continue;
					default:
						pData->SetRgb(
							item.R.ClampedDivide(alpha),
							item.G.ClampedDivide(alpha),
							item.B.ClampedDivide(alpha)
						);

						break;
				}

				++pData;
			}
		}
	}
}