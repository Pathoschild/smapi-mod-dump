/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace SpriteMaster.CpuIdParser;

internal sealed class AmdParser {
	[MethodImpl(Runtime.MethodImpl.RunOnce)]
	[SuppressMessage("ReSharper", "StringLiteralTypo")]
	internal static SystemInfo.ArchitectureResult Parse(in SystemInfo.CpuId id) {
		string microarchitecture;

		switch (id.Identifier) {
			// Zen 4
			case (0xA, 0xF, 0x7, _, _): // Zen 4 Phoenix
				microarchitecture = "Zen 4 'Phoenix'";
				break;
			case (0xA, 0xF, 0x6, _, _): // Zen 4 Raphael
				microarchitecture = "Zen 4 'Raphael'";
				break;
			case (0xA, 0xF, 0x1, 0x0, 0x0): // Zen 4 ES
				microarchitecture = "Zen 4 ES";
				break;
			// Zen 3
			case (0xA, 0xF, 0x5, 0x0, _): // Zen 3 Cezanne
				microarchitecture = "Zen 3 'Cezanne'";
				break;
			case (0xA, 0xF, 0x4, 0x4, _): // Zen 3
				microarchitecture = "Zen 3";
				break;
			case (0xA, 0xF, 0x4, 0x0, _): // Zen 3 Rembrandt
				microarchitecture = "Zen 3 'Rembrandt'";
				break;
			case (0xA, 0xF, 0x2, 0x1, _): // Zen 3 Vermeer
				microarchitecture = "Zen 3 'Vermeer'";
				break;
			case (0xA, 0xF, 0x0, 0x8, _): // Zen 3 Chagall
				microarchitecture = "Zen 3 'Chagall'";
				break;
			case (0xA, 0xF, 0x0, 0x1, 0x0): // Zen 3 ES
				microarchitecture = "Zen 3 ES";
				break;
			case (0xA, 0xF, 0x0, 0x1, 0x1): // Zen 3 Milan
				microarchitecture = "Zen 3 'Milan'";
				break;
			case (0xA, 0xF, 0x0, 0x1, 0x2): // Zen 3 ES
				microarchitecture = "Zen 3 ES";
				break;
			case (0xA, 0xF, 0x0, 0x0, 0x0): // Zen 3 ES
				microarchitecture = "Zen 3 ES";
				break;
			// Zen 2
			case (0x8, 0xF, 0x9, 0x0, _): // Zen 2 Van Gogh
				microarchitecture = "Zen 2 'Van Gogh'";
				break;
			case (0x8, 0xF, 0x7, 0x1, _): // Zen 2 Matisse
				microarchitecture = "Zen 2 'Matisse'";
				break;
			case (0x8, 0xF, 0x6, 0x8, _): // Zen 2 Lucienne
				microarchitecture = "Zen 2 'Lucienne'";
				break;
			case (0x8, 0xF, 0x6, 0x0, _): // Zen 2 Renoir / Grey Hawk
				microarchitecture = "Zen 2 'Renoir / Grey Hawk'";
				break;
			case (0x8, 0xF, 0x4, 0x7, _): // Zen 2 Xbox Series X
				microarchitecture = "Zen 2 'Xbox Series X'";
				break;
			case (0x8, 0xF, 0x3, 0x1, _): // Zen 2 Rome / Castle Peak
				microarchitecture = "Zen 2 'Rome / Castle Peak'";
				break;
			// Zen +
			case (0x8, 0xF, 0x1, 0x8, >= 0x1): // Zen+ Picasso
				microarchitecture = "Zen+ 'Picasso'";
				break;
			case (0x8, 0xF, 0x0, 0x8, _): // Zen+ Colfax / Pinnacle Ridge
				microarchitecture = "Zen+ 'Colfax / Pinnacle Ridge'";
				break;
			// Zen
			case (0x9, 0xF, 0x0, 0x0, _): // Zen Dhyana
				microarchitecture = "Zen 'Dhyana'";
				break;
			case (0x8, 0xF, 0x2, 0x0, _): // Zen Dali
				microarchitecture = "Zen 'Dali'";
				break;
			case (0x8, 0xF, 0x1, 0x8, _): // Zen Banded Kestral
				microarchitecture = "Zen 'Banded Kestrel'";
				break;
			case (0x8, 0xF, 0x0, 0x1, >= 0x2): // Zen Naples / Snowy Owl
				microarchitecture = "Zen 'Naples / Snowy Owl'";
				break;
			case (0x8, 0xF, 0x1, 0x1, _): // Zen Raven Ridge / Great Horned Owl
				microarchitecture = "Zen 'Raven Ridge / Great Horned Owl'";
				break;
			case (0x8, 0xF, 0x0, 0x1, _): // Zen Whitehaven / Summit Ridge
				microarchitecture = "Zen 'Whitehaven / Summit Ridge'";
				break;
			// Other
			case (0x7, 0xF, 0x3, 0x0, _): // Puma
				microarchitecture = "Puma";
				break;
			case (0x7, 0xF, 0x0, 0x0, _): // Jaguar
				microarchitecture = "Jaguar";
				break;
			case (0x6, 0xF, 0x7, 0x0, _): // Excavator 'Stoney Ridge'
				microarchitecture = "Excavator 'Stoney Ridge'";
				break;
			case (0x6, 0xF, 0x6, 0x5, _): // Excavator 'Bristol Ridge'
				microarchitecture = "Excavator 'Bristol Ridge'";
				break;
			case (0x6, 0xF, 0x6, 0x0, _): // Excavator 'Carrizo'
				microarchitecture = "Excavator 'Carrizo'";
				break;
			case (0x6, 0xF, 0x3, 0x8, _): // Steamroller 'Godaviri'
				microarchitecture = "Steamroller 'Godaviri'";
				break;
			case (0x6, 0xF, 0x3, 0x0, _): // Steamroller 'Kaveri'
				microarchitecture = "Steamroller 'Kaveri'";
				break;
			case (0x6, 0xF, 0x3, _, _): // Steamroller
				microarchitecture = "Steamroller";
				break;
			case (0x6, 0xF, 0x1, 0x3, _): // Piledriver 'Richland'
				microarchitecture = "Piledriver 'Richland'";
				break;
			case (0x6, 0xF, 0x1, 0x0, _): // Piledriver 'Trinity'
				microarchitecture = "Piledriver 'Trinity'";
				break;
			case (0x6, 0xF, 0x0, 0x2, _): // Piledriver
				microarchitecture = "Piledriver";
				break;
			case (0x6, 0xF, 0x0, 0x1, _): // Bulldozer
				microarchitecture = "Bulldozer";
				break;
			case (0x6, 0xF, _, _, _): // Excavator
				microarchitecture = "Excavator";
				break;
			case (0x5, 0xF, 0x0, _, _): // Bobcat
				microarchitecture = "Bobcat";
				break;
			case (0x3, 0xF, 0x0, 0x0, _): // Llano
				microarchitecture = "Llano";
				break;
			case (0x1, 0xF, _, _, _): // K10
				microarchitecture = "K10";
				break;
			case (0x0, 0xF, _, _, _): // K8
				microarchitecture = "K8";
				break;
			// Unknown but newer
			case ( >= 0xA, _, _, _, _): // Unknown but new
				microarchitecture = "Unknown (Newer)";
				break;
			default: // Unknown
				microarchitecture = "Unknown";
				break;
		}

		return new(
			microarchitecture,
			new() {
				Avx512 = false,
				Avx2 = Avx2.IsSupported
			}
		);
	}
}
