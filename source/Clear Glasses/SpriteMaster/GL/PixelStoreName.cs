/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

namespace SpriteMaster.GL;

// https://registry.khronos.org/OpenGL-Refpages/gl4/html/glPixelStore.xhtml
internal enum PixelStoreName : uint {
	/// <summary>
	/// <seealso cref="bool" />, default <seealso langword="false" />
	/// </summary>
	PackSwapBytes = 0x0D00u,
	/// <summary>
	/// <seealso cref="bool" />, default <seealso langword="false" />
	/// </summary>
	PackLsbFirst = 0x0D01u,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="0" />
	/// <para>
	/// May be [<seealso langword="0" />, <seealso langword="int.MaxValue" />)
	/// </para>
	/// </summary>
	PackRowLength = 0x0D02u,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="0" />
	/// <para>
	/// May be [<seealso langword="0" />, <seealso langword="int.MaxValue" />)
	/// </para>
	/// </summary>
	PackImageHeight = 0x806Cu,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="0" />
	/// <para>
	/// May be [<seealso langword="0" />, <seealso langword="int.MaxValue" />)
	/// </para>
	/// </summary>
	PackSkipRows = 0x0D03u,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="0" />
	/// <para>
	/// May be [<seealso langword="0" />, <seealso langword="int.MaxValue" />)
	/// </para>
	/// </summary>
	PackSkipPixels = 0x0D04u,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="0" />
	/// <para>
	/// May be [<seealso langword="0" />, <seealso langword="int.MaxValue" />)
	/// </para>
	/// </summary>
	PackSkipImages = 0x806Bu,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="4" />
	/// <para>
	/// May be <seealso langword="1" />, <seealso langword="2" />, <seealso langword="4" />, or <seealso langword="8" />
	/// </para>
	/// </summary>
	PackAlignment = 0x0D05u,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="0" />
	/// <para>
	///	Requires <see href="https://registry.khronos.org/OpenGL/extensions/ARB/ARB_compressed_texture_pixel_storage.txt"><c>ARB_compressed_texture_pixel_storage</c></see>
	/// </para>
	/// </summary>
	PackCompressedBlockDepth = 0x912Du,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="0" />
	/// <para>
	///	Requires <see href="https://registry.khronos.org/OpenGL/extensions/ARB/ARB_compressed_texture_pixel_storage.txt"><c>ARB_compressed_texture_pixel_storage</c></see>
	/// </para>
	/// </summary>
	PackCompressedBlockHeight = 0x912Cu,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="0" />
	/// <para>
	///	Requires <see href="https://registry.khronos.org/OpenGL/extensions/ARB/ARB_compressed_texture_pixel_storage.txt"><c>ARB_compressed_texture_pixel_storage</c></see>
	/// </para>
	/// </summary>
	PackCompressedBlockSize = 0x912Eu,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="0" />
	/// <para>
	/// Requires <see href="https://registry.khronos.org/OpenGL/extensions/ARB/ARB_compressed_texture_pixel_storage.txt"><c>ARB_compressed_texture_pixel_storage</c></see>
	/// </para>
	/// </summary>
	PackCompressedBlockWidth = 0x912Bu,
	/// <summary>
	/// <seealso cref="bool" />, default <seealso langword="false" />
	/// <para>
	/// Requires <see href="https://registry.khronos.org/OpenGL/extensions/MESA/MESA_pack_invert.txt"><c>GL_MESA_pack_invert</c></see>
	/// </para>
	/// </summary>
	PackInvertMesa = 0x8758u,
	/// <summary>
	/// <seealso cref="bool" />, default <seealso langword="false" />
	/// <para>
	/// Requires <see href="https://registry.khronos.org/OpenGL/extensions/ANGLE/ANGLE_pack_reverse_row_order.txt"><c>GL_ANGLE_pack_reverse_row_order</c></see>
	/// </para>
	/// </summary>
	PacReverseRowOrderAngle = 0x93A4u,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="0" />
	/// <para>
	/// May be [<seealso langword="0" />, <seealso langword="int.MaxValue" />)
	/// </para>
	/// <para>
	///	Requires <see href="https://registry.khronos.org/OpenGL/extensions/APPLE/APPLE_row_bytes.txt"><c>GL_APPLE_row_bytes</c></see>
	/// </para>
	/// </summary>
	PackRowBytesApple = 0x8A15u,

	/// <summary>
	/// <seealso cref="bool" />, default <seealso langword="false" />
	/// </summary>
	UnpackSwapBytes = 0x0CF0u,
	/// <summary>
	/// <seealso cref="bool" />, default <seealso langword="false" />
	/// </summary>
	UnpackLsbFirst = 0x0CF1u,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="0" />
	/// <para>
	/// May be [<seealso langword="0" />, <seealso langword="int.MaxValue" />)
	/// </para>
	/// </summary>
	UnpackRowLength = 0x0CF2u,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="0" />
	/// <para>
	/// May be [<seealso langword="0" />, <seealso langword="int.MaxValue" />)
	/// </para>
	/// </summary>
	UnpackImageHeight = 0x806Eu,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="0" />
	/// <para>
	/// May be [<seealso langword="0" />, <seealso langword="int.MaxValue" />)
	/// </para>
	/// </summary>
	UnpackSkipRows = 0x0CF3u,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="0" />
	/// <para>
	/// May be [<seealso langword="0" />, <seealso langword="int.MaxValue" />)
	/// </para>
	/// </summary>
	UnpackSkipPixels = 0x0CF4u,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="0" />
	/// <para>
	/// May be [<seealso langword="0" />, <seealso langword="int.MaxValue" />)
	/// </para>
	/// </summary>
	UnpackSkipImages = 0x806Du,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="0" />
	/// <para>
	/// May be <seealso langword="1" />, <seealso langword="2" />, <seealso langword="4" />, or <seealso langword="8" />
	/// </para>
	/// </summary>
	UnpackAlignment = 0x0CF5u,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="0" />
	/// <para>
	///	Requires <see href="https://registry.khronos.org/OpenGL/extensions/ARB/ARB_compressed_texture_pixel_storage.txt"><c>ARB_compressed_texture_pixel_storage</c></see>
	/// </para>
	/// </summary>
	UnpackCompressedBlockDepth = 0x9129u,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="0" />
	/// <para>
	///	Requires <see href="https://registry.khronos.org/OpenGL/extensions/ARB/ARB_compressed_texture_pixel_storage.txt"><c>ARB_compressed_texture_pixel_storage</c></see>
	/// </para>
	/// </summary>
	UnpackCompressedBlockHeight = 0x9128u,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="0" />
	/// <para>
	///	Requires <see href="https://registry.khronos.org/OpenGL/extensions/ARB/ARB_compressed_texture_pixel_storage.txt"><c>ARB_compressed_texture_pixel_storage</c></see>
	/// </para>
	/// </summary>
	UnpackCompressedBlockSize = 0x912Au,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="0" />
	/// <para>
	/// Requires <see href="https://registry.khronos.org/OpenGL/extensions/ARB/ARB_compressed_texture_pixel_storage.txt"><c>ARB_compressed_texture_pixel_storage</c></see>
	/// </para>
	/// </summary>
	UnpackCompressedBlockWidth = 0x9127u,
	/// <summary>
	/// <seealso cref="int" />, default <seealso langword="0" />
	/// <para>
	/// May be [<seealso langword="0" />, <seealso langword="int.MaxValue" />)
	/// </para>
	/// <para>
	///	Requires <see href="https://registry.khronos.org/OpenGL/extensions/APPLE/APPLE_row_bytes.txt"><c>GL_APPLE_row_bytes</c></see>
	/// </para>
	/// </summary>
	UnpackRowBytesApple = 0x8A16u,

	// gl_UNPACK_PREMULTIPLY_ALPHA_WEBGL
}
