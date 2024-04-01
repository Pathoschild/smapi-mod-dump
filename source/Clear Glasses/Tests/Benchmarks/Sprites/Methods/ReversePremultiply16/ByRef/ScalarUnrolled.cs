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
using System.Runtime.InteropServices;

namespace Benchmarks.Sprites.Methods.ReversePremultiply16.ByRef;
internal static class ScalarUnrolled {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Reverse(Span<Color16> data, Vector2I size) {
		const int Unroll = 8;
		ushort lowPass = Common.PremultiplicationLowPass;

		int i = 0;

		for (; i + (Unroll - 1) < data.Length; i += Unroll) {
			{
				const int ii = 0;

				ref var refItem = ref Unsafe.Add(ref MemoryMarshal.GetReference(data), i + ii);
				var item = refItem;

				var alpha = item.A;

				switch (alpha.Value) {
					case ushort.MaxValue:
					case var _ when alpha.Value <= lowPass:
						continue;
					default:
						refItem.SetRgb(
							item.R.ClampedDivide(alpha),
							item.G.ClampedDivide(alpha),
							item.B.ClampedDivide(alpha)
						);

						break;
				}
			}
			{
				const int ii = 1;

				ref var refItem = ref Unsafe.Add(ref MemoryMarshal.GetReference(data), i + ii);
				var item = refItem;

				var alpha = item.A;

				switch (alpha.Value) {
					case ushort.MaxValue:
					case var _ when alpha.Value <= lowPass:
						continue;
					default:
						refItem.SetRgb(
							item.R.ClampedDivide(alpha),
							item.G.ClampedDivide(alpha),
							item.B.ClampedDivide(alpha)
						);

						break;
				}
			}
			{
				const int ii = 2;

				ref var refItem = ref Unsafe.Add(ref MemoryMarshal.GetReference(data), i + ii);
				var item = refItem;

				var alpha = item.A;

				switch (alpha.Value) {
					case ushort.MaxValue:
					case var _ when alpha.Value <= lowPass:
						continue;
					default:
						refItem.SetRgb(
							item.R.ClampedDivide(alpha),
							item.G.ClampedDivide(alpha),
							item.B.ClampedDivide(alpha)
						);

						break;
				}
			}
			{
				const int ii = 3;

				ref var refItem = ref Unsafe.Add(ref MemoryMarshal.GetReference(data), i + ii);
				var item = refItem;

				var alpha = item.A;

				switch (alpha.Value) {
					case ushort.MaxValue:
					case var _ when alpha.Value <= lowPass:
						continue;
					default:
						refItem.SetRgb(
							item.R.ClampedDivide(alpha),
							item.G.ClampedDivide(alpha),
							item.B.ClampedDivide(alpha)
						);

						break;
				}
			}
			{
				const int ii = 4;

				ref var refItem = ref Unsafe.Add(ref MemoryMarshal.GetReference(data), i + ii);
				var item = refItem;

				var alpha = item.A;

				switch (alpha.Value) {
					case ushort.MaxValue:
					case var _ when alpha.Value <= lowPass:
						continue;
					default:
						refItem.SetRgb(
							item.R.ClampedDivide(alpha),
							item.G.ClampedDivide(alpha),
							item.B.ClampedDivide(alpha)
						);

						break;
				}
			}
			{
				const int ii = 5;

				ref var refItem = ref Unsafe.Add(ref MemoryMarshal.GetReference(data), i + ii);
				var item = refItem;

				var alpha = item.A;

				switch (alpha.Value) {
					case ushort.MaxValue:
					case var _ when alpha.Value <= lowPass:
						continue;
					default:
						refItem.SetRgb(
							item.R.ClampedDivide(alpha),
							item.G.ClampedDivide(alpha),
							item.B.ClampedDivide(alpha)
						);

						break;
				}
			}
			{
				const int ii = 6;

				ref var refItem = ref Unsafe.Add(ref MemoryMarshal.GetReference(data), i + ii);
				var item = refItem;

				var alpha = item.A;

				switch (alpha.Value) {
					case ushort.MaxValue:
					case var _ when alpha.Value <= lowPass:
						continue;
					default:
						refItem.SetRgb(
							item.R.ClampedDivide(alpha),
							item.G.ClampedDivide(alpha),
							item.B.ClampedDivide(alpha)
						);

						break;
				}
			}
			{
				const int ii = 7;

				ref var refItem = ref Unsafe.Add(ref MemoryMarshal.GetReference(data), i + ii);
				var item = refItem;

				var alpha = item.A;

				switch (alpha.Value) {
					case ushort.MaxValue:
					case var _ when alpha.Value <= lowPass:
						continue;
					default:
						refItem.SetRgb(
							item.R.ClampedDivide(alpha),
							item.G.ClampedDivide(alpha),
							item.B.ClampedDivide(alpha)
						);

						break;
				}
			}
		}

		for (; i < data.Length; ++i) {
			ref var refItem = ref Unsafe.Add(ref MemoryMarshal.GetReference(data), i);
			var item = refItem;

			var alpha = item.A;

			switch (alpha.Value) {
				case ushort.MaxValue:
				case var _ when alpha.Value <= lowPass:
					continue;
				default:
					refItem.SetRgb(
						item.R.ClampedDivide(alpha),
						item.G.ClampedDivide(alpha),
						item.B.ClampedDivide(alpha)
					);

					break;
			}
		}
	}
}
