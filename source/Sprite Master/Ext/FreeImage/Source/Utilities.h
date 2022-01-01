// ==========================================================
// Utility functions
//
// Design and implementation by
// - Floris van den Berg (flvdberg@wxs.nl)
// - Hervé Drolon <drolon@infonie.fr>
// - Ryan Rubley (ryan@lostreality.org)
// - Mihail Naydenov (mnaydenov@users.sourceforge.net)
//
// This file is part of FreeImage 3
//
// COVERED CODE IS PROVIDED UNDER THIS LICENSE ON AN "AS IS" BASIS, WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, WITHOUT LIMITATION, WARRANTIES
// THAT THE COVERED CODE IS FREE OF DEFECTS, MERCHANTABLE, FIT FOR A PARTICULAR PURPOSE
// OR NON-INFRINGING. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE COVERED
// CODE IS WITH YOU. SHOULD ANY COVERED CODE PROVE DEFECTIVE IN ANY RESPECT, YOU (NOT
// THE INITIAL DEVELOPER OR ANY OTHER CONTRIBUTOR) ASSUME THE COST OF ANY NECESSARY
// SERVICING, REPAIR OR CORRECTION. THIS DISCLAIMER OF WARRANTY CONSTITUTES AN ESSENTIAL
// PART OF THIS LICENSE. NO USE OF ANY COVERED CODE IS AUTHORIZED HEREUNDER EXCEPT UNDER
// THIS DISCLAIMER.
//
// Use at your own risk!
// ==========================================================

#ifndef FREEIMAGE_UTILITIES_H
#define FREEIMAGE_UTILITIES_H

// ==========================================================
//   Standard includes used by the library
// ==========================================================

#include <cstdint>

#if _MSC_VER
#	define _noalias __declspec(noalias)
#	define _alloc_align(position) __declspec(restrict, allocator)
#	define _alloc_size(position) __declspec(restrict, allocator)
#	define _forceinline __forceinline
#	define _flatten
# define _assume(expr) __assume(expr)
#	define _unreachable __assume(0)
#	define _unpredictable(expr) (expr)
#	define _assume_aligned(ptr, alignment) (_assume_aligned_impl<alignment>(ptr))
#	define _hot
#	define _cold
#	define _const
#	define _pure
#	define _noalias __declspec(noalias)
#	define _leaf
#	define _nothrow __declspec(nothrow) noexcept
#elif __GNUC__
#	define _noalias
#	define _alloc_align(position) __attribute__((malloc, alloc_align(position)))
#	define _alloc_size(position) __attribute__((malloc, alloc_size(position)))
#	define _forceinline __attribute__((always_inline))
#	define _flatten __attribute__((flatten))
#	if __clang__
#		define _assume(expr) __builtin_assume(expr)
#		define _unpredictable(expr) (__builtin_unpredictable(expr))
#	else
#		define _assume(expr) do { if (!(expr)) __builtin_unreachable(); } while (0)
#		define _unpredictable(expr) (__builtin_expect_with_probability(!!(expr), 1, 0.5))
#	endif
#	define _unreachable __builtin_unreachable()
#	define _assume_aligned(ptr, alignment) (__builtin_assume_aligned(ptr, alignment))
#	define _hot __attribute__((hot))
#	define _cold __attribute__((cold))
#	define _const __attribute__((const))
#	define _pure __attribute__((pure))
#	define _noalias
#	define _leaf __attribute__((leaf))
#	define _nothrow __attribute__((nothrow)) noexcept
#else
#	define _noalias
#	define _alloc_align(position)
#	define _alloc_size(position)
#	define _forceinline
#	define _flatten
#	define _assume(expr)
#	define _unreachable
#	define _unpredictable(expr) (expr)
#	define _assume_aligned(ptr, alignment) (_assume_aligned_impl<alignment>(ptr))
#	define _hot
#	define _cold
#	define _const
#	define _pure
#	define _noalias
#	define _leaf
#	define _nothrow noexcept
#endif

template <typename T, size_t Align>
#if _MSC_VER
__declspec(noalias)
#endif
static inline T* _assume_aligned_impl(T* ptr) {
	_assume((uintptr_t(ptr) & (Align - 1)) == 0);
	return ptr;
}

#include <cmath>
#include <cstdlib> 
#include <memory.h>
#include <cstdio>
#include <cstring>
#include <cstdarg>
#include <cctype>
#include <cassert>
#include <cerrno>
#include <cfloat>
#include <climits>

#include <string>
#include <list>
#include <map>
#include <set>
#include <vector>
#include <stack>
#include <sstream>
#include <algorithm>
#include <limits>
#include <memory>

// ==========================================================
//   Bitmap palette and pixels alignment
// ==========================================================

#define FIBITMAP_ALIGNMENT	16	// We will use a 16 bytes alignment boundary

// Memory allocation on a specified alignment boundary
// defined in BitmapAccess.cpp

void* FreeImage_Aligned_Malloc(size_t amount, size_t alignment);
void FreeImage_Aligned_Free(void* mem);

#if defined(__cplusplus)
extern "C" {
#endif

/**
Allocate a FIBITMAP with possibly no pixel data 
(i.e. only header data and some or all metadata)
@param header_only If TRUE, allocate a 'header only' FIBITMAP, otherwise allocate a full FIBITMAP
@param type Image type
@param width Image width
@param height Image height
@param bpp Number of bits per pixel
@param red_mask Image red mask 
@param green_mask Image green mask
@param blue_mask Image blue mask
@return Returns the allocated FIBITMAP
@see FreeImage_AllocateT
*/
DLL_API FIBITMAP * DLL_CALLCONV FreeImage_AllocateHeaderT(BOOL header_only, FREE_IMAGE_TYPE type, int width, int height, int bpp FI_DEFAULT(8), unsigned red_mask FI_DEFAULT(0), unsigned green_mask FI_DEFAULT(0), unsigned blue_mask FI_DEFAULT(0));

/**
Allocate a FIBITMAP of type FIT_BITMAP, with possibly no pixel data 
(i.e. only header data and some or all metadata)
@param header_only If TRUE, allocate a 'header only' FIBITMAP, otherwise allocate a full FIBITMAP
@param width Image width
@param height Image height
@param bpp Number of bits per pixel
@param red_mask Image red mask 
@param green_mask Image green mask
@param blue_mask Image blue mask
@return Returns the allocated FIBITMAP
@see FreeImage_Allocate
*/
DLL_API FIBITMAP * DLL_CALLCONV FreeImage_AllocateHeader(BOOL header_only, int width, int height, int bpp, unsigned red_mask FI_DEFAULT(0), unsigned green_mask FI_DEFAULT(0), unsigned blue_mask FI_DEFAULT(0));

/**
Allocate a FIBITMAP with no pixel data and wrap a user provided pixel buffer
@param ext_bits Pointer to external user's pixel buffer
@param ext_pitch Pointer to external user's pixel buffer pitch
@param type Image type
@param width Image width
@param height Image height
@param bpp Number of bits per pixel
@param red_mask Image red mask 
@param green_mask Image green mask
@param blue_mask Image blue mask
@return Returns the allocated FIBITMAP
@see FreeImage_ConvertFromRawBitsEx
*/
DLL_API FIBITMAP * DLL_CALLCONV FreeImage_AllocateHeaderForBits(BYTE *ext_bits, unsigned ext_pitch, FREE_IMAGE_TYPE type, int width, int height, int bpp, unsigned red_mask, unsigned green_mask, unsigned blue_mask);

/**
Helper for 16-bit FIT_BITMAP
@see FreeImage_GetRGBMasks
*/
DLL_API BOOL DLL_CALLCONV FreeImage_HasRGBMasks(FIBITMAP *dib);

#if defined(__cplusplus)
}
#endif


// ==========================================================
//   File I/O structs
// ==========================================================

// these structs are for file I/O and should not be confused with similar
// structs in FreeImage.h which are for in-memory bitmap handling

#ifdef _WIN32
#pragma pack(push, 1)
#else
#pragma pack(1)
#endif // _WIN32

typedef struct tagFILE_RGBA {
  unsigned char r,g,b,a;
} FILE_RGBA;

typedef struct tagFILE_BGRA {
  unsigned char b,g,r,a;
} FILE_BGRA;

typedef struct tagFILE_RGB {
  unsigned char r,g,b;
} FILE_RGB;

typedef struct tagFILE_BGR {
  unsigned char b,g,r;
} FILE_BGR;

#ifdef _WIN32
#pragma pack(pop)
#else
#pragma pack()
#endif // _WIN32

// ==========================================================
//   Template utility functions
// ==========================================================

/// Max function
template <class T> static constexpr inline _noalias _pure const T& MAX(const T &a, const T &b) {
	return (a >= b) ? a: b;
}

/// Min function
template <class T> static constexpr inline _noalias _pure const T& MIN(const T &a, const T &b) {
	return (a <= b) ? a: b;
}

#include <algorithm>
/// INPLACESWAP adopted from codeguru.com 
template <class T> static constexpr inline _noalias const void INPLACESWAP(T& a, T& b) {
	std::swap(a, b);
}

/// Clamp function
template <class T> static constexpr inline _noalias _pure const T& CLAMP(const T &value, const T &min_value, const T &max_value) {
	return std::clamp(value, min_value, max_value);
}

/** This procedure computes minimum min and maximum max
 of n numbers using only (3n/2) - 2 comparisons.
 min = L[i1] and max = L[i2].
 ref: Aho A.V., Hopcroft J.E., Ullman J.D., 
 The design and analysis of computer algorithms, 
 Addison-Wesley, Reading, 1974.
*/
template <class T> static constexpr inline _noalias void
MAXMIN(const T * __restrict L, long n, T& __restrict max, T& __restrict min) {
	long i1, i2, i, j;
	T x1, x2;
	long k1, k2;

	i1 = 0; i2 = 0; min = L[0]; max = L[0]; j = 0;
	if((n % 2) != 0)  j = 1;
	for(i = j; i < n; i+= 2) {
		k1 = i; k2 = i+1;
		x1 = L[k1]; x2 = L[k2];
		if(x1 > x2)	{
			k1 = k2;  k2 = i;
			x1 = x2;  x2 = L[k2];
		}
		if(x1 < min) {
			min = x1;  i1 = k1;
		}
		if(x2 > max) {
			max = x2;  i2 = k2;
		}
	}
}

// ==========================================================
//   Utility functions
// ==========================================================

#ifndef _WIN32
inline char*
i2a(unsigned i, char *a, unsigned r) {
	if (i/r > 0) a = i2a(i/r,a,r);
	*a = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"[i%r];
	return a+1;
}

/** 
 Transforms integer i into an ascii string and stores the result in a; 
 string is encoded in the base indicated by r.
 @param i Number to be converted
 @param a String result
 @param r Base of value; must be in the range 2 - 36
 @return Returns a
*/
inline char *
_itoa(int i, char *a, int r) {
	r = ((r < 2) || (r > 36)) ? 10 : r;
	if(i < 0) {
		*a = '-';
		*i2a(-i, a+1, r) = 0;
	}
	else *i2a(i, a, r) = 0;
	return a;
}

#endif // !_WIN32

static constexpr inline _noalias _const unsigned char
HINIBBLE (unsigned char byte) {
	return byte & 0xF0;
}

static constexpr inline _noalias _const unsigned char
LOWNIBBLE (unsigned char byte) {
	return byte & 0x0F;
}

static constexpr inline _noalias _const int
CalculateUsedBits(int nbits) {
	if (!std::is_constant_evaluated()) {
#if _MSC_VER
		return __popcnt((unsigned)nbits);
#elif __GNUC__
		return __builtin_popcount((unsigned)nbits);
#endif
	}
	constexpr const unsigned NumBits = sizeof(nbits) * 8;
	int bit_count = 0;
	unsigned bits = unsigned(nbits);

	for (unsigned i = 0; i < NumBits; ++i) {
		bit_count += (bits & 1);
		bits >>= 1;
	}

	return bit_count;
}

static constexpr inline _noalias _const unsigned
CalculateLine(const unsigned width, const unsigned bitdepth) {
	return (unsigned)( ((unsigned long long)width * bitdepth + 7) / 8 );
}

static constexpr inline _noalias _const unsigned
CalculatePitch(const unsigned line) {
	return (line + 3) & ~3;
}

static constexpr inline _noalias _const unsigned
CalculateUsedPaletteEntries(const unsigned bit_count) {
	if ((bit_count >= 1) && (bit_count <= 8)) {
		return 1 << bit_count;
	}

	return 0;
}

static constexpr inline _noalias _const BYTE*
CalculateScanLine(BYTE *bits, const unsigned pitch, const int scanline) {
	if (!bits) [[unlikely]] {
		return nullptr;
	}

	return bits + ((size_t)pitch * scanline);
}

// ----------------------------------------------------------

template <size_t N>
struct arbitrary_buffer final {
	uint8_t _[N];
};

/**
Fast generic assign (faster than for loop)
@param dst Destination pixel
@param src Source pixel
@param bytesperpixel # of bytes per pixel
*/
static constexpr inline void
AssignPixel(BYTE* dst, const BYTE* src, unsigned bytesperpixel) {
	switch (bytesperpixel) {
	case 1:
		*dst = *src;
		break;
	case 2:
		*(arbitrary_buffer<2> *)dst = *(const arbitrary_buffer<2> *)src;
		break;
	case 3:
		*(arbitrary_buffer<3> *)dst = *(const arbitrary_buffer<3> *)src;
		break;
	case 4:
		*(arbitrary_buffer<4> *)dst = *(const arbitrary_buffer<4> *)src;
		break;
	case 6:
		*(arbitrary_buffer<6> *)dst = *(const arbitrary_buffer<6> *)src;
		break;
	case 8:
		*(arbitrary_buffer<8> *)dst = *(const arbitrary_buffer<8> *)src;
		break;
	case 12:
		*(arbitrary_buffer<12> *)dst = *(const arbitrary_buffer<12> *)src;
		break;
	case 16:
		*(arbitrary_buffer<16> *)dst = *(const arbitrary_buffer<16> *)src;
		break;
	default:
		__assume(0);
		assert(0);
	}
}

/**
Swap red and blue channels in a 24- or 32-bit dib. 
@return Returns TRUE if successful, returns FALSE otherwise
@see See definition in Conversion.cpp
*/
BOOL SwapRedBlue32(FIBITMAP* dib);

/**
Inplace convert CMYK to RGBA.(8- and 16-bit). 
Alpha is filled with the first extra channel if any or white otherwise.
@return Returns TRUE if successful, returns FALSE otherwise
@see See definition in Conversion.cpp
*/
BOOL ConvertCMYKtoRGBA(FIBITMAP* dib);

/**
Inplace convert CIELab to RGBA (8- and 16-bit).
@return Returns TRUE if successful, returns FALSE otherwise
@see See definition in Conversion.cpp
*/
BOOL ConvertLABtoRGB(FIBITMAP* dib);

/**
RGBA to RGB conversion
@see See definition in Conversion.cpp
*/
FIBITMAP* RemoveAlphaChannel(FIBITMAP* dib);

/**
Rotate a dib according to Exif info
@param dib Input / Output dib to rotate
@see Exif.cpp, PluginJPEG.cpp
*/
void RotateExif(FIBITMAP **dib);


// ==========================================================
//   Big Endian / Little Endian utility functions
// ==========================================================

inline WORD 
__SwapUInt16(WORD arg) { 
#if defined(_MSC_VER) && _MSC_VER >= 1310 
	return _byteswap_ushort(arg); 
#elif defined(__GNUC__)
	return __builtin_bswap16(arg);
#else 
	// swap bytes 
	WORD result;
	result = ((arg << 8) & 0xFF00) | ((arg >> 8) & 0x00FF); 
	return result; 
#endif 
} 
 
inline DWORD 
__SwapUInt32(DWORD arg) { 
#if defined(_MSC_VER) && _MSC_VER >= 1310 
	return _byteswap_ulong(arg); 
#elif defined(__GNUC__) 
	return __builtin_bswap32(arg);
#else 
	// swap words then bytes
	DWORD result; 
	result = ((arg & 0x000000FF) << 24) | ((arg & 0x0000FF00) << 8) | ((arg >> 8) & 0x0000FF00) | ((arg >> 24) & 0x000000FF); 
	return result; 
#endif 
} 

inline void
SwapShort(WORD *sp) {
	*sp = __SwapUInt16(*sp);
}

inline void
SwapLong(DWORD *lp) {
	*lp = __SwapUInt32(*lp);
}
 
inline void
SwapInt64(UINT64 *arg) {
#if defined(_MSC_VER) && _MSC_VER >= 1310
	*arg = _byteswap_uint64(*arg);
#elif defined(__GNUC__)
	*arg = __builtin_bswap64(*arg);
#else
	union Swap {
		UINT64 sv;
		DWORD ul[2];
	} tmp, result;
	tmp.sv = *arg;
	SwapLong(&tmp.ul[0]);
	SwapLong(&tmp.ul[1]);
	result.ul[0] = tmp.ul[1];
	result.ul[1] = tmp.ul[0];
	*arg = result.sv;
#endif
}

// ==========================================================
//   Greyscale and color conversion
// ==========================================================

/**
Extract the luminance channel L from a RGBF image. 
Luminance is calculated from the sRGB model using a D65 white point, using the Rec.709 formula : 
L = ( 0.2126 * r ) + ( 0.7152 * g ) + ( 0.0722 * b )
Reference : 
A Standard Default Color Space for the Internet - sRGB. 
[online] http://www.w3.org/Graphics/Color/sRGB
*/
template <typename T>
constexpr static inline float LUMA_REC709(const T& r, const T& g, const T& b) {
	return (0.2126F * r + 0.7152F * g + 0.0722F * b);
}

template <typename T>
constexpr static inline BYTE GREY(const T& r, const T& g, const T& b) {
	return (BYTE)(LUMA_REC709(r, g, b) + 0.5F);
}
/*
#define GREY(r, g, b) (BYTE)(((WORD)r * 77 + (WORD)g * 150 + (WORD)b * 29) >> 8)	// .299R + .587G + .114B
*/
/*
#define GREY(r, g, b) (BYTE)(((WORD)r * 169 + (WORD)g * 256 + (WORD)b * 87) >> 9)	// .33R + 0.5G + .17B
*/

/**
Convert a RGB 24-bit value to a 16-bit 565 value
*/
template <typename T>
constexpr static inline uint16_t RGB565(const T& b, const T& g, const T& r) {
	return ((((b) >> 3) << FI16_565_BLUE_SHIFT) | (((g) >> 2) << FI16_565_GREEN_SHIFT) | (((r) >> 3) << FI16_565_RED_SHIFT));
}

/**
Convert a RGB 24-bit value to a 16-bit 555 value
*/
template <typename T>
constexpr static inline uint16_t RGB555(const T& b, const T& g, const T& r) {
	return ((((b) >> 3) << FI16_555_BLUE_SHIFT) | (((g) >> 3) << FI16_555_GREEN_SHIFT) | (((r) >> 3) << FI16_555_RED_SHIFT));
}

/**
Returns TRUE if the format of a dib is RGB565
*/
template <typename T>
constexpr static inline bool IS_FORMAT_RGB565(const T& dib) {
	return (FreeImage_GetRedMask(dib) == FI16_565_RED_MASK) && (FreeImage_GetGreenMask(dib) == FI16_565_GREEN_MASK) && (FreeImage_GetBlueMask(dib) == FI16_565_BLUE_MASK);
}

/**
Convert a RGB565 or RGB555 RGBQUAD pixel to a WORD
*/
template <typename T, typename U>
constexpr static inline uint16_t RGBQUAD_TO_WORD(const T& dib, const U& color) {
	return IS_FORMAT_RGB565(dib) ? RGB565(color->rgbBlue, color->rgbGreen, color->rgbRed) : RGB555(color->rgbBlue, color->rgbGreen, color->rgbRed);
}

/**
Create a greyscale palette
*/
template <typename T>
constexpr static inline void CREATE_GREYSCALE_PALETTE(T& __restrict palette, const uint32_t entries) {
	const auto vIncrement = 0x00FFFFFF / (entries - 1);
	for (
		uint32_t i = 0, v = 0;
		i < entries;
		i++, v += vIncrement
	) {
		((uint32_t* __restrict)palette)[i] = v;
	}
}

/**
Create a reverse greyscale palette
*/
template <typename T>
constexpr static inline void CREATE_GREYSCALE_PALETTE_REVERSE(T& __restrict palette, const uint32_t entries) {
	const auto vDecrement = 0x00FFFFFF / (entries - 1);
	for (
		uint32_t i = 0, v = 0x00FFFFFF;
		i < entries;
		i++, v -= vDecrement) {
		((uint32_t* __restrict)palette)[i] = v;
	}
}


// ==========================================================
//   Generic error messages
// ==========================================================

static constexpr const char FI_MSG_ERROR_MEMORY[] = "Memory allocation failed";
static constexpr const char FI_MSG_ERROR_DIB_MEMORY[] = "DIB allocation failed, maybe caused by an invalid image size or by a lack of memory";
static constexpr const char FI_MSG_ERROR_PARSING[] = "Parsing error";
static constexpr const char FI_MSG_ERROR_MAGIC_NUMBER[] = "Invalid magic number";
static constexpr const char FI_MSG_ERROR_UNSUPPORTED_FORMAT[] = "Unsupported format";
static constexpr const char FI_MSG_ERROR_UNSUPPORTED_COMPRESSION[] = "Unsupported compression type";
static constexpr const char FI_MSG_WARNING_INVALID_THUMBNAIL[] = "Warning: attached thumbnail cannot be written to output file (invalid format) - Thumbnail saving aborted";

#endif // FREEIMAGE_UTILITIES_H
