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
internal static class ScalarFloat {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe void Reverse(Span<Color16> data, Vector2I size) {
		ushort lowPass = Common.PremultiplicationLowPass;

		fixed (Color16* pData = data) {
			for (int i = 0; i < data.Length; ++i) {
				var item = pData[i];

				var alpha = item.A;
				var alphaFloat = 1.0f / (alpha.Value / 255.0f);

				switch (alpha.Value) {
					case ushort.MaxValue:
					case var _ when alpha.Value <= lowPass:
						continue;
					default:
						pData[i].SetRgb(
							(ushort)((item.R.Value * alphaFloat) + 0.5f),
							(ushort)((item.G.Value * alphaFloat) + 0.5f),
							(ushort)((item.B.Value * alphaFloat) + 0.5f)
						);

						break;
				}
			}
		}
	}
}