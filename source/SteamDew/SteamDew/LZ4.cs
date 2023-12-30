/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/myuusubi/SteamDew
**
*************************************************/

using System;
using System.Runtime.InteropServices;

namespace SteamDew {

public class LZ4 {

[DllImport("liblwjgl_lz4", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Java_org_lwjgl_util_lz4_LZ4_LZ4_1compressBound")]
private static extern int lwjgl_compressBound(IntPtr env, IntPtr clazz, int inputSize);

[DllImport("liblwjgl_lz4", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Java_org_lwjgl_util_lz4_LZ4_nLZ4_1compress_1default")]
private static extern int lwjgl_compress_default(IntPtr env, IntPtr clazz, IntPtr src, IntPtr dest, int srcSize, int dstCapacity);

[DllImport("liblwjgl_lz4", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Java_org_lwjgl_util_lz4_LZ4_nLZ4_1decompress_1safe")]
private static extern int lwjgl_decompress_safe(IntPtr env, IntPtr clazz, IntPtr src, IntPtr dest, int compressedSize, int dstCapacity);

public static int compressBound(int inputSize) {
	return LZ4.lwjgl_compressBound(IntPtr.Zero, IntPtr.Zero, inputSize);
}

public static int compress_default(IntPtr src, IntPtr dest, int srcSize, int dstCapacity) {
	return LZ4.lwjgl_compress_default(IntPtr.Zero, IntPtr.Zero, src, dest, srcSize, dstCapacity);
}

public static int decompress_safe(IntPtr src, IntPtr dest, int compressedSize, int dstCapacity) {
	return LZ4.lwjgl_decompress_safe(IntPtr.Zero, IntPtr.Zero, src, dest, compressedSize, dstCapacity);
}

} /* class LZ4 */

} /* namespace SteamDew */