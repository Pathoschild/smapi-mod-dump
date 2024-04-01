/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using Microsoft.Toolkit.HighPerformance;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Benchmarks.Hashing;

internal static class Functions {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong FNV1a(ReadOnlySpan<byte> data) {
		ulong hash = 0xcbf29ce484222325UL;
		foreach (var octet in data) {
			hash ^= octet;
			hash *= 0x00000100000001B3UL;
		}
		return hash;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static uint SDBM32(ReadOnlySpan<byte> data) {
		uint hash = 0U;
		foreach (var octet in data) {
			hash = octet + (hash << 6) + (hash << 16) - hash;
		}
		return hash;
	}

	internal static unsafe uint SDBM32c(ReadOnlySpan<byte> data) {
		uint hash = 0U;
		foreach (var octet in data) {
			hash *= 0x1003F;
			hash += octet;
		}
		return hash;
	}

	// var uData = data.Cast<byte, uint>();

	// https://godbolt.org/z/z31oG9rzz
	internal static unsafe uint SDBM32cUnroll(ReadOnlySpan<byte> data) {
		uint hash = 0U;
		uint len = (uint)data.Length;
		for (; len >= 4; len -= 4) {
			var octet0 = data[0];
			var octet1 = data[1];
			hash *= 0x1003F;
			hash += octet0;
			var octet2 = data[2];
			hash *= 0x1003F;
			hash += octet1;
			var octet3 = data[3];
			hash *= 0x1003F;
			hash += octet2;
			hash *= 0x1003F;
			hash += octet3;
			data = data[4..];
		}
		foreach (var octet in data) {
			hash *= 0x1003F;
			hash += octet;
		}
		return hash;
	}

	internal static unsafe uint SDBM32cUnroll2(ReadOnlySpan<byte> data) {
		uint hash = 0U;
		var uData = data.Cast<byte, uint>();
		foreach (var octetFour in uData) {
			var octet0 = (byte)octetFour;
			var octet1 = (byte)(octetFour >> 8);
			hash *= 0x1003F;
			hash += octet0;
			var octet2 = (byte)(octetFour >> 16);
			hash *= 0x1003F;
			hash += octet1;
			var octet3 = (byte)(octetFour >> 24);
			hash *= 0x1003F;
			hash += octet2;
			hash *= 0x1003F;
			hash += octet3;
		}

		data = data[(uData.Length * sizeof(uint))..];
		foreach (var octet in data) {
			hash *= 0x1003F;
			hash += octet;
		}
		return hash;
	}

	internal static unsafe uint SDBM32cUnroll3(ReadOnlySpan<byte> data) {
		uint hash = 0U;
		uint i = 0;

		uint len = (uint)data.Length;
		for (; i + 3 <= len; i += 4) {
			var octetFour = data.Cast<byte, uint>()[(int)(i / 4u)];
			var octet0 = (byte)octetFour;
			var octet1 = (byte)(octetFour >> 8);
			hash *= 0x1003F;
			hash += octet0;
			var octet2 = (byte)(octetFour >> 16);
			hash *= 0x1003F;
			hash += octet1;
			var octet3 = (byte)(octetFour >> 24);
			hash *= 0x1003F;
			hash += octet2;
			hash *= 0x1003F;
			hash += octet3;
		}
		for (; i < len; ++i) {
			var octet = data[(int)i];
			hash *= 0x1003F;
			hash += octet;
		}
		return hash;
	}

#if false
    internal static unsafe uint SDBM32c(byte* data, uint len) {
		uint hash = 0U;
        for (; len != 0; --len) {
		    var octet = *data++;
            hash *= 0x1003F;
            hash += octet;
		}
		return hash;
	}
    
    // https://godbolt.org/z/z31oG9rzz
    internal static unsafe uint SDBM32cUnroll(byte* data, uint len) {
		uint hash = 0U;
        for (; len >= 4; len -= 4) {
            var octet0 = data[0];
            var octet1 = data[1];
            hash *= 0x1003F;
            hash += octet0;
            var octet2 = data[2];
            hash *= 0x1003F;
            hash += octet1;
            var octet3 = data[3];
            hash *= 0x1003F;
            hash += octet2;
            hash *= 0x1003F;
            hash += octet3; 
            data += 4;
        }
        for (; len != 0; --len) {
		    var octet = *data++;
            hash *= 0x1003F;
            hash += octet;
		}
		return hash;
	}
    
    internal static unsafe uint SDBM32cUnroll2(byte* data, uint len) {
		uint hash = 0U;
        for (; len >= 4; len -= 4) {
            var octetFour = *(uint*)data;
            var octet0 = (byte)octetFour;
            var octet1 = (byte)(octetFour >> 8);
            hash *= 0x1003F;
            hash += octet0;
            var octet2 = (byte)(octetFour >> 16);
            hash *= 0x1003F;
            hash += octet1;
            var octet3 = (byte)(octetFour >> 24);
            hash *= 0x1003F;
            hash += octet2;
            hash *= 0x1003F;
            hash += octet3; 
            data += 4;
        }
        for (; len != 0; --len) {
		    var octet = *data++;
            hash *= 0x1003F;
            hash += octet;
		}
		return hash;
	}
    
    internal static unsafe uint SDBM32cUnroll3(byte* data, uint len) {
		uint hash = 0U;
        uint i = 0;
        
        for (; i + 3 <= len; i += 4) {
            var octetFour = *(uint*)(data + i);
            var octet0 = (byte)octetFour;
            var octet1 = (byte)(octetFour >> 8);
            hash *= 0x1003F;
            hash += octet0;
            var octet2 = (byte)(octetFour >> 16);
            hash *= 0x1003F;
            hash += octet1;
            var octet3 = (byte)(octetFour >> 24);
            hash *= 0x1003F;
            hash += octet2;
            hash *= 0x1003F;
            hash += octet3; 
        }
        for (; i < len; ++i) {
		    var octet = data[i];
            hash *= 0x1003F;
            hash += octet;
		}
		return hash;
	}
#endif

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong CombHash(ReadOnlySpan<byte> data) {
		return data.Length <= 8 ?
			FNV1a(data) :
			SpriteMaster.Hashing.HashUtility.HashXx3(data);
	}

	[DllImport("libxxhash.clang.dll", EntryPoint = "XXH3_64bits", ExactSpelling = true, BestFitMapping = false, CallingConvention = CallingConvention.Cdecl)]
	internal static extern unsafe ulong XxHash3NativeClang(void* input, ulong length);

	[DllImport("libxxhash.clang.dll", EntryPoint = "XXH32", ExactSpelling = true, BestFitMapping = false, CallingConvention = CallingConvention.Cdecl)]
	internal static extern unsafe uint XxH32NativeClang(void* input, ulong length, uint seed);

	[DllImport("libxxhash.clang.dll", EntryPoint = "XXH64", ExactSpelling = true, BestFitMapping = false, CallingConvention = CallingConvention.Cdecl)]
	internal static extern unsafe ulong XxH64NativeClang(void* input, ulong length, uint seed);

	[DllImport("libxxhash.msvc.dll", EntryPoint = "XXH3_64bits", ExactSpelling = true, BestFitMapping = false, CallingConvention = CallingConvention.Cdecl)]
	internal static extern unsafe ulong XxHash3NativeVC(void* input, ulong length);

	[DllImport("libxxhash.msvc.dll", EntryPoint = "XXH32", ExactSpelling = true, BestFitMapping = false, CallingConvention = CallingConvention.Cdecl)]
	internal static extern unsafe uint XxH32NativeVC(void* input, ulong length, uint seed);

	[DllImport("libxxhash.msvc.dll", EntryPoint = "XXH64", ExactSpelling = true, BestFitMapping = false, CallingConvention = CallingConvention.Cdecl)]
	internal static extern unsafe ulong XxH64NativeVC(void* input, ulong length, uint seed);

#if false
	internal static ulong XxHash3CLI(byte[] input) {
		return XxHashCLI.XxHash3.Hash64(input);
	}

	internal static ulong XxHash3CLI(ReadOnlySpan<byte> input) {
		return XxHashCLI.XxHash3.Hash64(input);
	}

	internal static ulong XxHash3CLITest(byte[] input) {
		return XxHash3Experimental.XxHash3Test.Hash64(input);
	}

	internal static ulong XxHash3CLITest(ReadOnlySpan<byte> input) {
		return XxHash3Experimental.XxHash3Test.Hash64(input);
	}
#endif
}
