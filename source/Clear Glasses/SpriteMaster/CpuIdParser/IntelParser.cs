/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace SpriteMaster.CpuIdParser;

// https://en.wikichip.org/wiki/intel/frequency_behavior
// https://en.wikichip.org/w/index.php?title=intel/frequency_behavior&oldid=97206

internal sealed class IntelParser {
	[MethodImpl(Runtime.MethodImpl.RunOnce)]
	[SuppressMessage("ReSharper", "StringLiteralTypo")]
	internal static SystemInfo.ArchitectureResult Parse(in SystemInfo.CpuId id) {
		SystemInfo.InstructionSets result = new() {
			Avx512 = false,
			Avx2 = Avx2.IsSupported
		};

		string microarchitecture;

		switch (id.Identifier) {
			// Big Cores (Client)

			case (0x0, 0x6, 0x0, 0xF, _): // Core
			case (0x0, 0x6, 0x1, 0x6, _): // Core
				microarchitecture = "Core";
				break;
			case (0x0, 0x6, 0x1, 0x7, _): // Penryn
				microarchitecture = "Penryn";
				break;
			case (0x0, 0x6, 0x1, 0xE, _): // Nehalem
			case (0x0, 0x6, 0x1, 0xF, _): // Nehalem
				microarchitecture = "Nehalem";
				break;
			case (0x0, 0x6, 0x2, 0x5, _): // Westmere
				microarchitecture = "Westmere";
				break;
			case (0x0, 0x6, 0x2, 0xA, _): // Sandy Bridge
				microarchitecture = "Sandy Bridge";
				break;
			case (0x0, 0x6, 0x3, 0xA, _): // Ivy Bridge
				microarchitecture = "Ivy Bridge";
				break;
			case (0x0, 0x6, 0x3, 0xC, _): // Haswell
			case (0x0, 0x6, 0x4, 0x5, _): // Haswell
			case (0x0, 0x6, 0x4, 0x6, _): // Haswell
				microarchitecture = "Haswell";
				break;
			case (0x0, 0x6, 0x3, 0xD, _): // Broadwell
			case (0x0, 0x6, 0x4, 0x7, _): // Broadwell
				microarchitecture = "Broadwell";
				break;
			case (0x0, 0x6, 0x4, 0xE, _): // Skylake
			case (0x0, 0x6, 0x5, 0xE, _): // Skylake
				microarchitecture = "Skylake";
				break;
			case (0x0, 0x6, 0x8, 0xE, 0xA): // Coffee Lake
			case (0x0, 0x6, 0x9, 0xE, 0xA): // Coffee Lake
			case (0x0, 0x6, 0x9, 0xE, 0xB): // Coffee Lake
			case (0x0, 0x6, 0x9, 0xE, 0xC): // Coffee Lake
			case (0x0, 0x6, 0x9, 0xE, 0xD): // Coffee Lake
				microarchitecture = "Coffee Lake";
				break;
			case (0x0, 0x6, 0x9, 0xE, 0x9): // Kaby Lake
				microarchitecture = "Kaby Lake";
				break;
			case (0x0, 0x6, 0x6, 0x6, _): // Cannon Lake
			case (0x0, 0x6, 0x9, 0xE, _): // Cannon Lake
				microarchitecture = "Cannon Lake";
				break;
			case (0x0, 0x6, 0x8, 0xE, 0xB): // Whiskey Lake
			case (0x0, 0x6, 0x8, 0xE, 0xC): // Whiskey Lake
				microarchitecture = "Whiskey Lake";
				break;
			case (0x0, 0x6, 0x8, 0xE, 0x9): // Amber Lake
				microarchitecture = "Amber Lake";
				break;
			case (0x0, 0x6, 0xA, 0x5, _): // Comet Lake
				microarchitecture = "Comet Lake";
				break;
			case (0x0, 0x6, 0x7, 0xE, _): // Ice Lake
			case (0x0, 0x6, 0x7, 0xD, _): // Ice Lake
				microarchitecture = "Ice Lake";
				break;
			case (0x0, 0x6, 0x8, 0xD, _): // Tiger Lake
			case (0x0, 0x6, 0x8, 0xC, _): // Tiger Lake
				microarchitecture = "Tiger Lake";
				break;
			case (0x0, 0x6, 0xA, 0x7, _): // Rocket Lake
				microarchitecture = "Rocket Lake";
				break;
			case (0x0, 0x6, 0x9, 0x7, _): // Alder Lake
			case (0x0, 0x6, 0x9, 0xA, _): // Alder Lake
				microarchitecture = "Alder Lake";
				break;
			case (0x0, 0x6, 0xB, 0x7, _): // Raptor Lake
				microarchitecture = "Raptor Lake";
				break;

			// Big Cores (Server)

			case (0x0, 0x6, 0x1, 0xD, _): // Penryn Server
				microarchitecture = "Penryn (Server)";
				break;
			case (0x0, 0x6, 0x2, 0xE, _): // Nehalem Server
			case (0x0, 0x6, 0x1, 0xA, _): // Nehalem Server
				microarchitecture = "Nehalem (Server)";
				break;
			case (0x0, 0x6, 0x2, 0xC, _): // Westmere Server
			case (0x0, 0x6, 0x2, 0xF, _): // Westmere Server
				microarchitecture = "Westmere (Server)";
				break;
			case (0x0, 0x6, 0x2, 0xD, _): // Sandy Bridge Server
				microarchitecture = "Sandy Bridge (Server)";
				break;
			case (0x0, 0x6, 0x3, 0xE, _): // Ivy Bridge Server
				microarchitecture = "Ivy Bridge (Server)";
				break;
			case (0x0, 0x6, 0x3, 0xF, _): // Haswell Server
				microarchitecture = "Haswell (Server)";
				break;
			case (0x0, 0x6, 0x4, 0xF, _): // Broadwell Server
			case (0x0, 0x6, 0x5, 0x6, _): // Broadwell Server
				microarchitecture = "Broadwell (Server)";
				break;
			case (0x0, 0x6, 0x5, 0x5, _): // Skylake Server
				microarchitecture = "Skylake (Server)";
				break;
			case (0x0, 0x6, 0x6, 0xC, _): // Ice Lake Server
			case (0x0, 0x6, 0x6, 0xE, _): // Ice Lake Server
				microarchitecture = "Ice Lake (Server)";
				break;
			case (0x0, 0x6, 0x8, 0xF, _): // Sapphire Rapids
				microarchitecture = "Sapphire Rapids";
				break;

			// Small Cores

			case (0x0, 0x6, 0x2, 0x6, _): // Bonnell
			case (0x0, 0x6, 0x1, 0xC, _): // Bonnell
				microarchitecture = "Bonnell";
				break;
			case (0x0, 0x6, 0x3, 0x6, _): // Saltwell
			case (0x0, 0x6, 0x3, 0x5, _): // Saltwell
			case (0x0, 0x6, 0x2, 0x7, _): // Saltwell
				microarchitecture = "Saltwell";
				break;
			case (0x0, 0x6, 0x5, 0xD, _): // Silvermont
			case (0x0, 0x6, 0x5, 0xA, _): // Silvermont
			case (0x0, 0x6, 0x4, 0xD, _): // Silvermont
			case (0x0, 0x6, 0x4, 0xA, _): // Silvermont
			case (0x0, 0x6, 0x3, 0x7, _): // Silvermont
				microarchitecture = "Silvermont";
				break;
			case (0x0, 0x6, 0x4, 0xC, _): // Airmont
				microarchitecture = "Airmont";
				break;
			case (0x0, 0x6, 0x5, 0xF, _): // Goldmont
			case (0x0, 0x6, 0x5, 0xC, _): // Goldmont
				microarchitecture = "Goldmont";
				break;
			case (0x0, 0x6, 0x7, 0xA, _): // Goldmont Plus
				microarchitecture = "Goldmont Plus";
				break;
			case (0x0, 0x6, 0x9, 0xC, _): // Tremont
			case (0x0, 0x6, 0x9, 0x6, _): // Tremont
			case (0x0, 0x6, 0x8, 0xA, _): // Tremont
				microarchitecture = "Tremont";
				break;

			case (0x0, 0x6, >= 0xB, _, _): // Unknown Newer
				microarchitecture = "Unknown (Newer)";
				break;

			default: // Unknown
				microarchitecture = "Unknown";
				break;
		}

		// Handle instruction sets / throttling
		switch (id.Identifier) {
			// On Haswell, if any core is executing AVX2, all cores are capped at AVX2 turbo frequency
			case (0x0, 0x6, 0x3, 0xC, _): // Haswell
			case (0x0, 0x6, 0x3, 0xF, _): // Haswell
			case (0x0, 0x6, 0x4, 0x5, _): // Haswell
			case (0x0, 0x6, 0x4, 0x6, _): // Haswell
			case (0x0, 0x6, 0x4, 0xC, _): // Airmont
			case (0x0, 0x6, 0x5, 0xC, _): // Goldmont
																		// On Broadwell and on, only grouped cores are capped
			case (0x0, 0x6, 0x3, 0xD, _): // Broadwell
			case (0x0, 0x6, 0x4, 0x7, _): // Broadwell
			case (0x0, 0x6, 0x4, 0xF, _): // Broadwell
			case (0x0, 0x6, 0x5, 0x6, _): // Broadwell
			case (0x0, 0x6, 0x4, 0xE, _): // Skylake
			case (0x0, 0x6, 0x5, 0x5, _): // Skylake
			case (0x0, 0x6, 0x5, 0xE, _): // Skylake
			case (0x0, 0x6, 0x6, 0x6, _): // Cannon Lake
			case (0x0, 0x6, 0x8, 0xE, _): // Cannon Lake / Kaby Lake
			case (0x0, 0x6, 0x9, 0xE, _): // Cannon Lake / Kaby Lake
				result = result with {
					Avx512 = false,
					Avx512Light = false,
					Avx2 = false
				};
				break;
			// Rocket Lake does not throttle
			case (0x0, 0x6, 0xA, 0x7, _): // Rocket Lake
			case (0x0, 0x6, >= 0xB, _, _): // Unknown Newer
				result = result with {
					Avx512 = true,
					Avx512Light = true,
					Avx2 = Avx2.IsSupported
				};
				break;
			default:
				result = result with {
					Avx512Light = true,
					Avx2 = Avx2.IsSupported
				};
				break;
		}

		return new(microarchitecture, result);
	}
}
