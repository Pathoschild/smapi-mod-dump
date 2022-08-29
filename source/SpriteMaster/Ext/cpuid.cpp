#include <cstdio>
#include <cstdint>
#include <format>
#include <string>
#include <limits>
#include <array>
#include <cassert>

using namespace std;
using uint = std::uint32_t;

struct alignas(uint) registers final {
	const std::string reference;
	const std::string name;
	
	union {
		struct {
			const uint ebx;
			const uint edx;
			const uint ecx;
		};
		uint array[3];
	};
	
	template <size_t N>
	registers(const char (&str)[N], const char *name) :
		reference(str),
		name(name),
		ebx(*(uint*)&str[0]),
		edx(*(uint*)&str[4]),
		ecx(*(uint*)&str[8])
	{
		static_assert(N == 12 + 1, "brand string is not 12 characters");
	}
	
	constexpr std::string dump() const {
		// (0xb, 0xd, 0xc)
		return std::format("(0x%08X, 0x%08X, 0x%08X)", ebx, edx, ecx);
	}
} __attribute__((packed));

static const registers cpu_registers[] = {
	{ "AMDisbetter!", "AMD" },
	{ "AuthenticAMD", "AMD" },
	{ "GenuineIntel", "Intel" },
	{ "VIA VIA VIA ", "VIA" },
	{ "bhyve bhyve ", "bhyve" },
	{ " KVMKVMKVM  ", "KVM" },
	{ "TCGTCGTCGTCG", "QEMU" },
	{ "Microsoft Hv", "HyperV" },
	{ " lrpepyh  vr", "Parallels" },
	{ "prl  hyperv ", "Parallels" },
	{ "VMwareVMware", "VMware" },
	{ "XenVMMXenVMM", "Xen" },
	{ "ACRNACRNACRN", "ACRN" },
	{ " QNXQVMBSQG ", "QNX" }
};

int main() {
	for (const auto reg : cpu_registers) {
		std::printf("// '%s' (%s)\n", reg.reference.c_str(), reg.name.c_str());
		std::printf("case (0x%08X, 0x%08X, 0x%08X):\n", reg.ebx, reg.edx, reg.ecx);
	}
}