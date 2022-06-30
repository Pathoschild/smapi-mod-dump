/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace SpriteMaster;

internal static partial class SystemInfo {
	internal enum ProcessorType : byte {
		OriginalOem = 0b00,
		IntelOverDrive = 0b01,
		Dual = 0b10,
		IntelReserved = 0b11,
	}

	// ReSharper disable InconsistentNaming
	internal enum Brand : uint {
		Unknown = 0,
		Intel,
		AMD,
		VIA,

		// Virtual Machines
		Bhyve,
		KVM,
		QEMU,
		HyperV,
		Parallels,
		VMWare,
		Xen,
		ACRN,
		QNX,
		//Rosetta2 // cannot be distinguished from Intel
	}
	// ReSharper restore InconsistentNaming

	[StructLayout(LayoutKind.Sequential, Pack = sizeof(uint), Size = 3 * sizeof(uint))]
	private readonly record struct BrandRegisters(uint Ebx, uint Edx, uint Ecx) {
		internal BrandRegisters((int eax, int ebx, int ecx, int edx) registers) :
			this((uint)registers.ebx, (uint)registers.edx, (uint)registers.ecx) { }
	}

	private static Brand ParseBrand(BrandRegisters registers) =>
		registers switch {
			// 'AMDisbetter!' (AMD)
			(0x69444D41, 0x74656273, 0x21726574) => Brand.AMD,
			// 'AuthenticAMD' (AMD)
			(0x68747541, 0x69746E65, 0x444D4163) => Brand.AMD,
			// 'GenuineIntel' (Intel)
			(0x756E6547, 0x49656E69, 0x6C65746E) => Brand.Intel,
			// 'VIA VIA VIA ' (VIA)
			(0x20414956, 0x20414956, 0x20414956) => Brand.VIA,
			// 'bhyve bhyve ' (bhyve)
			(0x76796862, 0x68622065, 0x20657679) => Brand.Bhyve,
			// ' KVMKVMKVM  ' (KVM)
			(0x4D564B20, 0x4B4D564B, 0x20204D56) => Brand.KVM,
			// 'TCGTCGTCGTCG' (QEMU)
			(0x54474354, 0x43544743, 0x47435447) => Brand.QEMU,
			// 'Microsoft Hv' (HyperV)
			(0x7263694D, 0x666F736F, 0x76482074) => Brand.HyperV,
			// ' lrpepyh  vr' (Parallels)
			(0x70726C20, 0x68797065, 0x72762020) => Brand.Parallels,
			// 'prl  hyperv ' (Parallels)
			(0x206C7270, 0x70796820, 0x20767265) => Brand.Parallels,
			// 'VMwareVMware' (VMware)
			(0x61774D56, 0x4D566572, 0x65726177) => Brand.VMWare,
			// 'XenVMMXenVMM' (Xen)
			(0x566E6558, 0x65584D4D, 0x4D4D566E) => Brand.Xen,
			// 'ACRNACRNACRN' (ACRN)
			(0x4E524341, 0x4E524341, 0x4E524341) => Brand.ACRN,
			// ' QNXQVMBSQG ' (QNX)
			(0x584E5120, 0x424D5651, 0x20475153) => Brand.QNX,
			_ => Brand.Unknown
		};

	[StructLayout(LayoutKind.Auto, Size = 4)]
	internal readonly record struct Identifier(byte ExtendedFamilyId, byte FamilyId, byte ExtendedModelId, byte ModelId, byte Stepping);

	[StructLayout(LayoutKind.Auto)]
	internal readonly record struct CpuId(
		string BrandString,
		Brand Brand,
		byte Stepping,
		byte ModelId,
		byte FamilyId,
		ProcessorType Type,
		byte ExtendedModelId,
		byte ExtendedFamilyId
	) {
		internal readonly Identifier Identifier => new(ExtendedFamilyId, FamilyId, ExtendedModelId, ModelId, Stepping);

		internal readonly byte Family => (byte)(ExtendedFamilyId << 4 | FamilyId);
		internal readonly byte Model => (byte)(ExtendedModelId << 4 | ModelId);

		private static byte Extract(int value, int shift, byte mask) =>
			(byte)((((uint)value) >> shift) & mask);

		internal static CpuId Create() {
			BrandRegisters brandRegisters = new(X86Base.CpuId(0, 0));

			string brandString;
			unsafe {
				var brandBytes = new ReadOnlySpan<byte>((byte*)&brandRegisters, sizeof(BrandRegisters));
				brandString = System.Text.Encoding.ASCII.GetString(brandBytes);
			}

			var (eax, _, _, _) = X86Base.CpuId(1, 0);

			return new() {
				Brand = ParseBrand(brandRegisters),
				BrandString = brandString,
				Stepping = Extract(eax, 0, 0b1111),
				ModelId = Extract(eax, 4, 0b1111),
				FamilyId = Extract(eax, 8, 0b1111),
				Type = (ProcessorType)Extract(eax, 12, 0b11),
				ExtendedModelId = Extract(eax, 16, 0b1111),
				ExtendedFamilyId = Extract(eax, 20, 0b1111_1111)
			};
		}
	}

	internal readonly record struct InstructionSets(
		bool Avx512,
		bool Avx512Light,
		bool Avx2
	);

	internal static readonly CpuId CpuIdentifier;

	internal static readonly InstructionSets Instructions = new() {
		Avx512 = false,
		Avx512Light = true, // bit-scanning and simple (non-multiply) operations
		Avx2 = true
	};

	private static (InstructionSets, string) HandleAmd(CpuId id) {
		string microarchitecture = "Unknown";

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
			// K10
			case (0x1, 0xF, _, _, _): // K10
				microarchitecture = "K10";
				break;
			// K8
			case (0x0, 0xF, _, _, _): // K8
				microarchitecture = "K8";
				break;
		}

		return (new() {
			Avx512 = false,
			Avx2 = Avx2.IsSupported
		}, microarchitecture);
	}

	internal static readonly string Microarchitecture = "Unknown";

	// https://en.wikichip.org/wiki/intel/frequency_behavior
	// https://en.wikichip.org/w/index.php?title=intel/frequency_behavior&oldid=97206
	private static (InstructionSets, string) HandleIntel(CpuId id) {
		InstructionSets result = new() {
			Avx512 = false,
			Avx2 = Avx2.IsSupported
		};

		string microarchitecture = "Unknown";

		switch (id.Identifier) {
			case (0x0, 0x6, 0x3, 0xC, _): // Haswell
			case (0x0, 0x6, 0x3, 0xF, _): // Haswell
			case (0x0, 0x6, 0x4, 0x5, _): // Haswell
			case (0x0, 0x6, 0x4, 0x6, _): // Haswell
			case (0x0, 0x6, 0x4, 0xC, _): // Haswell
			case (0x0, 0x6, 0x5, 0xC, _): // Haswell
				microarchitecture = "Haswell";
				break;
			case (0x0, 0x6, 0x3, 0xD, _): // Broadwell
			case (0x0, 0x6, 0x4, 0x7, _): // Broadwell
			case (0x0, 0x6, 0x4, 0xF, _): // Broadwell
			case (0x0, 0x6, 0x5, 0x6, _): // Broadwell
				microarchitecture = "Broadwell";
				break;
			case (0x0, 0x6, 0x4, 0xE, _): // Skylake
			case (0x0, 0x6, 0x5, 0x5, _): // Skylake
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
				microarchitecture = "Cannon Lake";
				break;
			case (0x0, 0x6, 0x9, 0xE, _): // Cannon Lake
				microarchitecture = "Cannon Lake";
				break;
			case (0x0, 0x6, 0xA, 0x7, _): // Rocket Lake
				microarchitecture = "Rocket Lake";
				break;
			case (0x0, 0x6, 0x9, 0x7, _): // Alder Lake
			case (0x0, 0x6, 0x9, 0xA, _): // Alder Lake
				microarchitecture = "Alder Lake";
				break;
			case (0x0, 0x6, 0x8, 0xD, _): // Tiger Lake
			case (0x0, 0x6, 0x8, 0xC, _): // Tiger Lake
				microarchitecture = "Tiger Lake";
				break;
			case (0x0, 0x6, 0x7, 0xE, _): // Ice Lake
			case (0x0, 0x6, 0x7, 0xD, _): // Ice Lake
				microarchitecture = "Ice Lake";
				break;
			case (0x0, 0x6, 0xA, 0x5, _): // Comet Lake
				microarchitecture = "Comet Lake";
				break;
			case (0x0, 0x6, 0x8, 0xE, 0x9): // Amber Lake
				microarchitecture = "Amber Lake";
				break;
			case (0x0, 0x6, 0x8, 0xE, 0xB): // Whiskey Lake
			case (0x0, 0x6, 0x8, 0xE, 0xC): // Whiskey Lake
				microarchitecture = "Whiskey Lake";
				break;
		}

		// Handle instruction sets / throttling
		switch (id.Identifier) {
			// On Haswell, if any core is executing AVX2, all cores are capped at AVX2 turbo frequency
			case (0x0, 0x6, 0x3, 0xC, _): // Haswell
			case (0x0, 0x6, 0x3, 0xF, _): // Haswell
			case (0x0, 0x6, 0x4, 0x5, _): // Haswell
			case (0x0, 0x6, 0x4, 0x6, _): // Haswell
			case (0x0, 0x6, 0x4, 0xC, _): // Haswell
			case (0x0, 0x6, 0x5, 0xC, _): // Haswell
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

		return (result, microarchitecture);
	}

	private static (InstructionSets, string) HandleOther(CpuId id) {
		// I don't know what to do in this situation
		return (new() { Avx2 = false, Avx512 = false }, "Unknown");
	}

	static SystemInfo() {
		if (!X86Base.IsSupported) {
			return;
		}

		CpuIdentifier = CpuId.Create();

		(Instructions, Microarchitecture) = CpuIdentifier.Brand switch {
			Brand.AMD => HandleAmd(CpuIdentifier),
			Brand.Intel => HandleIntel(CpuIdentifier),
			_ => HandleOther(CpuIdentifier)
		};
	}
}
