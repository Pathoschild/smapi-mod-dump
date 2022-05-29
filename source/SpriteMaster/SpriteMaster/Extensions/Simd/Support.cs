/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.Runtime.Intrinsics.X86;

namespace SpriteMaster.Extensions.Simd;
internal static class Support {
	internal static bool UseAVX2 = true;



	static Support() {
		if (X86Base.IsSupported) {
			var (_, ebx, ecx, edx) = X86Base.CpuId(0, 0);
			var manufacturer = (ebx, ecx, edx);

			var (eax, _, _, _) = X86Base.CpuId(1, 0);
			int stepping = eax & 0b1111;
			int modelId = (eax >> 4) & 0b1111;
			int familyId = (eax >> 8) & 0b1111;
			int procType = (eax >> 12) & 0b11;
			int extendedModelId = (eax >> 16) & 0b1111;
			int extendedFamilyId = (eax >> 20) & 0b1111_1111;

			// AMD
			if (manufacturer == (0x68747541, 0x444d4163, 0x69746e65)) {
				// AMD can always use AVX2, if it is supported
				UseAVX2 = true;
			}
			// Intel
			else if (manufacturer == (0x756e6547, 0x6c65746e, 0x49656e69)) {
				switch (extendedFamilyId, familyId, extendedModelId, modelId) {
					case (0x0, 0x6, 0x3, 0xC): // Haswell
					case (0x0, 0x6, 0x3, 0xF): // Haswell
					case (0x0, 0x6, 0x4, 0x5): // Haswell
					case (0x0, 0x6, 0x4, 0x6): // Haswell
					case (0x0, 0x6, 0x4, 0xC): // Haswell
					case (0x0, 0x6, 0x5, 0xC): // Haswell
					case (0x0, 0x6, 0x3, 0xD): // Broadwell
					case (0x0, 0x6, 0x4, 0x7): // Broadwell
					case (0x0, 0x6, 0x4, 0xF): // Broadwell
					case (0x0, 0x6, 0x5, 0x6): // Broadwell
					case (0x0, 0x6, 0x4, 0xE): // Skylake
					case (0x0, 0x6, 0x5, 0x5): // Skylake
					case (0x0, 0x6, 0x5, 0xE): // Skylake
					case (0x0, 0x6, 0x6, 0x6): // Cannon Lake
					case (0x0, 0x6, 0x8, 0xE): // Cannon Lake / Kaby Lake
					case (0x0, 0x6, 0x9, 0xE): // Cannon Lake / Kaby Lake
						UseAVX2 = false;
						break;
					default:
						UseAVX2 = true;
						break;
				}
			}
			else {
				// For now, just don't allow AVX2
				UseAVX2 = false;
			}
		}
	}
}
