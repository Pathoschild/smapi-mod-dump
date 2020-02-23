using SpriteMaster.Types;
using System;
using System.Data.HashFunction.xxHash;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions {
	using XRectangle = Microsoft.Xna.Framework.Rectangle;

	internal static class _HashValues {
		public const ulong Default = 0ul;
	}

	public static class Hash {
		public enum CombineType {
			Xor,
			Boost
		}
		public const CombineType DefaultCombineType = CombineType.Boost;

		public const ulong Default = _HashValues.Default;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong Accumulate (ulong hash, ulong hashend) {
			switch (DefaultCombineType) {
				case CombineType.Xor: return hash ^ hashend;
				// Stolen from C++ Boost.
				case CombineType.Boost: return hash ^ (hashend + 0x9e3779b9ul + (hash << 6) + (hash >> 2));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong Accumulate (ulong hash, int hashend) {
			return Accumulate(hash, unchecked((ulong)hashend));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Combine (params ulong[] hashes) {
			unchecked {
				ulong hash = 0;
				foreach (var subHash in hashes) {
					hash = Accumulate(hash, subHash);
				}
				return hash;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Combine (params object[] hashes) {
			unchecked {
				ulong hash = 0;

				foreach (var subHash in hashes) {
					hash = subHash switch {
						long i => Accumulate(hash, (ulong)i),
						ulong i => Accumulate(hash, i),
						_ => Accumulate(hash, subHash.GetHashCode()),
					};
				}
				return hash;
			}
		}
	}

	public static class Hashing {
		public const ulong Default = _HashValues.Default;

		// FNV-1a hash.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong HashFNV1 (this byte[] data) {
			return new Span<byte>(data).HashFNV1();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong HashFNV1 (this in Span<byte> data) {
			const ulong prime = 0x100000001b3;
			ulong hash = 0xcbf29ce484222325;
			foreach (byte octet in data) {
				hash ^= octet;
				hash *= prime;
			}

			return hash;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong HashFNV1 (this byte[] data,/* int start,*/ int length) {
			return new Span<byte>(data, /*start, */length).HashFNV1();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong HashFNV1<T> (this T[] data) where T : unmanaged {
			return data.CastAs<T, byte>().HashFNV1();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe ulong HashFNV1<T> (this in Span<T> data) where T : unmanaged {
			return data.As<byte>().HashFNV1();
		}

		private static readonly IxxHash HasherXX = xxHashFactory.Instance.Create(new xxHashConfig() { HashSizeInBits = 64 });

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong HashXXCompute (this byte[] hashData) {
			return BitConverter.ToUInt64(hashData, 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong HashXX (this byte[] data) {
			return HasherXX.ComputeHash(data).Hash.HashXXCompute();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe ulong HashXX (this in Span<byte> data) {
			fixed (byte* p = &data.GetPinnableReference()) {
				using var stream = new UnmanagedMemoryStream(p, data.Length);
				return HasherXX.ComputeHash(stream).Hash.HashXXCompute();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong HashXX (this byte[] data, int start, int length) {
			using var stream = new MemoryStream(data, start, length);
			return HasherXX.ComputeHash(stream).Hash.HashXXCompute();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong HashXX<T> (this T[] data) where T : unmanaged {
			return data.CastAs<T, byte>().HashXX();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong HashXX<T> (this in Span<T> data) where T : unmanaged {
			return data.As<byte>().HashXX();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Hash (this byte[] data) {
			return data.HashXX();
			//return data.HashFNV1();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Hash (this in Span<byte> data) {
			return data.HashXX();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Hash (this byte[] data, int start, int length) {
			return data.HashXX(start, length);
			//return data.HashFNV1();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Hash<T> (this T[] data) where T : unmanaged {
			return data.HashXX();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Hash<T> (this in Span<T> data) where T : unmanaged {
			return data.HashXX();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Hash (this in Rectangle rectangle) {
			return
				((ulong)rectangle.X & 0xFFFF) |
				(((ulong)rectangle.Y & 0xFFFF) << 16) |
				(((ulong)rectangle.Width & 0xFFFF) << 32) |
				(((ulong)rectangle.Height & 0xFFFF) << 48);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Hash (this in XRectangle rectangle) {
			return
				((ulong)rectangle.X & 0xFFFF) |
				(((ulong)rectangle.Y & 0xFFFF) << 16) |
				(((ulong)rectangle.Width & 0xFFFF) << 32) |
				(((ulong)rectangle.Height & 0xFFFF) << 48);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Hash (this in Bounds rectangle) {
			return
				((ulong)rectangle.X & 0xFFFF) |
				(((ulong)rectangle.Y & 0xFFFF) << 16) |
				(((ulong)rectangle.Width & 0xFFFF) << 32) |
				(((ulong)rectangle.Height & 0xFFFF) << 48);
		}
	}
}
