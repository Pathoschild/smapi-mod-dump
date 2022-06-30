/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace SpriteMaster.Caching;

internal static partial class TextureFileCache {
	private const bool UseAVX512 = false;
	internal static readonly bool UseAVX2 = true && Extensions.Simd.Support.Avx2 && Avx2.IsSupported;
	internal static readonly bool UseSSE2 = true && Sse2.IsSupported;
	internal static readonly bool UseNeon = true && AdvSimd.IsSupported;

	private static readonly int VectorSize =
		UseAVX512 ? 512 :
		UseAVX2 ? 256 :
		(UseSSE2 || UseNeon) ? 128 :
		64;
	private const bool UsePrefetch = true;
	private const uint CacheLine = 0x40u;
	private const uint PrefetchDistance = CacheLine;
}
