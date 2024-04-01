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

namespace Benchmarks.Sprites.Methods.ReversePremultiply16.ByIndex;
internal static class ScalarUnrolled {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Reverse(Span<Color16> data, Vector2I size) {
		const int Unroll = 8;
		ushort lowPass = Common.PremultiplicationLowPass;

		int i = 0;

		for (; i + (Unroll - 1) < data.Length; i += Unroll) {
			{
				const int ii = 0;

				var item = data[i + ii];

				var alpha = item.A;

				switch (alpha.Value) {
					case ushort.MaxValue:
					case var _ when alpha.Value <= lowPass:
						continue;
					default:
						data[i + ii].SetRgb(
							item.R.ClampedDivide(alpha),
							item.G.ClampedDivide(alpha),
							item.B.ClampedDivide(alpha)
						);

						break;
				}
			}
			{
				const int ii = 1;

				var item = data[i + ii];

				var alpha = item.A;

				switch (alpha.Value) {
					case ushort.MaxValue:
					case var _ when alpha.Value <= lowPass:
						continue;
					default:
						data[i + ii].SetRgb(
							item.R.ClampedDivide(alpha),
							item.G.ClampedDivide(alpha),
							item.B.ClampedDivide(alpha)
						);

						break;
				}
			}
			{
				const int ii = 2;

				var item = data[i + ii];

				var alpha = item.A;

				switch (alpha.Value) {
					case ushort.MaxValue:
					case var _ when alpha.Value <= lowPass:
						continue;
					default:
						data[i + ii].SetRgb(
							item.R.ClampedDivide(alpha),
							item.G.ClampedDivide(alpha),
							item.B.ClampedDivide(alpha)
						);

						break;
				}
			}
			{
				const int ii = 3;

				var item = data[i + ii];

				var alpha = item.A;

				switch (alpha.Value) {
					case ushort.MaxValue:
					case var _ when alpha.Value <= lowPass:
						continue;
					default:
						data[i + ii].SetRgb(
							item.R.ClampedDivide(alpha),
							item.G.ClampedDivide(alpha),
							item.B.ClampedDivide(alpha)
						);

						break;
				}
			}
			{
				const int ii = 4;

				var item = data[i + ii];

				var alpha = item.A;

				switch (alpha.Value) {
					case ushort.MaxValue:
					case var _ when alpha.Value <= lowPass:
						continue;
					default:
						data[i + ii].SetRgb(
							item.R.ClampedDivide(alpha),
							item.G.ClampedDivide(alpha),
							item.B.ClampedDivide(alpha)
						);

						break;
				}
			}
			{
				const int ii = 5;

				var item = data[i + ii];

				var alpha = item.A;

				switch (alpha.Value) {
					case ushort.MaxValue:
					case var _ when alpha.Value <= lowPass:
						continue;
					default:
						data[i + ii].SetRgb(
							item.R.ClampedDivide(alpha),
							item.G.ClampedDivide(alpha),
							item.B.ClampedDivide(alpha)
						);

						break;
				}
			}
			{
				const int ii = 6;

				var item = data[i + ii];

				var alpha = item.A;

				switch (alpha.Value) {
					case ushort.MaxValue:
					case var _ when alpha.Value <= lowPass:
						continue;
					default:
						data[i + ii].SetRgb(
							item.R.ClampedDivide(alpha),
							item.G.ClampedDivide(alpha),
							item.B.ClampedDivide(alpha)
						);

						break;
				}
			}
			{
				const int ii = 7;

				var item = data[i + ii];

				var alpha = item.A;

				switch (alpha.Value) {
					case ushort.MaxValue:
					case var _ when alpha.Value <= lowPass:
						continue;
					default:
						data[i + ii].SetRgb(
							item.R.ClampedDivide(alpha),
							item.G.ClampedDivide(alpha),
							item.B.ClampedDivide(alpha)
						);

						break;
				}
			}
		}

		for (; i < data.Length; ++i) {
			var item = data[i];

			var alpha = item.A;

			switch (alpha.Value) {
				case ushort.MaxValue:
				case var _ when alpha.Value <= lowPass:
					continue;
				default:
					data[i].SetRgb(
						item.R.ClampedDivide(alpha),
						item.G.ClampedDivide(alpha),
						item.B.ClampedDivide(alpha)
					);

					break;
			}
		}
	}
}
