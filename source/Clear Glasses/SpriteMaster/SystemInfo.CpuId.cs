/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using System;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace SpriteMaster;

internal static partial class SystemInfo {
	internal enum ProcessorType : byte {
		[UsedImplicitly]
		OriginalOem = 0b00,
		[UsedImplicitly]
		IntelOverDrive = 0b01,
		[UsedImplicitly]
		Dual = 0b10,
		[UsedImplicitly]
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

	internal readonly record struct ArchitectureResult(string Name, InstructionSets Sets);

	internal static readonly string Microarchitecture = "Unknown";

	static SystemInfo() {
		if (!X86Base.IsSupported) {
			return;
		}

		CpuIdentifier = CpuId.Create();

		(Microarchitecture, Instructions) = CpuIdentifier.Brand switch {
			Brand.AMD => CpuIdParser.AmdParser.Parse(CpuIdentifier),
			Brand.Intel => CpuIdParser.IntelParser.Parse(CpuIdentifier),
			_ => CpuIdParser.GenericParser.Parse(CpuIdentifier),
		};
	}
}
