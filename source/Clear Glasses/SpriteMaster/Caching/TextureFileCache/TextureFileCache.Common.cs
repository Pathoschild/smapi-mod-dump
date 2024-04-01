/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace SpriteMaster.Caching;

internal static partial class TextureFileCache {
	private const bool UseAvx512 = Extensions.Simd.Support.Avx512;
	internal static readonly bool UseAvx2 = true && Extensions.Simd.Support.Avx2;
	internal static readonly bool UseSse2 = Extensions.Simd.Support.Enabled && Sse2.IsSupported;
	internal static readonly bool UseNeon = Extensions.Simd.Support.Enabled && AdvSimd.IsSupported;

	private static readonly int VectorSize =
		UseAvx512 ? 512 :
		UseAvx2 ? 256 :
		(UseSse2 || UseNeon) ? 128 :
		64;
	private const bool UsePrefetch = true;
	private const uint CacheLine = 0x40u;
	private const uint PrefetchDistance = CacheLine;
}
