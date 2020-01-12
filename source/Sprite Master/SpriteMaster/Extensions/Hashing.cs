using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Types;
using System;
using System.Data.HashFunction.xxHash;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions {
	using XRectangle = Microsoft.Xna.Framework.Rectangle;
	public static class Hashing {
		private const bool UseXorHash = false;

		private static ulong AccumulateHash(ulong hash, ulong hashend) {
			if (UseXorHash) {
				return hash ^ hashend;
			}
			// Stolen from C++ Boost.
			return hash ^ (hashend + 0x9e3779b9ul + (hash << 6) + (hash >> 2));
		}

		private static ulong AccumulateHash (ulong hash, int hashend) {
			return AccumulateHash(hash, unchecked((ulong)hashend));
		}

		public static ulong CombineHash(params ulong[] hashes) {
			unchecked {
				ulong hash = 0;
				foreach (var subHash in hashes) {
					hash = AccumulateHash(hash, subHash);
				}
				return hash;
			}
		}

		public static ulong CombineHash (params object[] hashes) {
			unchecked {
				ulong hash = 0;

				foreach (var subHash in hashes) {
					switch (subHash) {
						case char i: hash = AccumulateHash(hash, i.GetHashCode()); break;
						case sbyte i: hash = AccumulateHash(hash, i.GetHashCode()); break;
						case short i: hash = AccumulateHash(hash, i.GetHashCode()); break;
						case int i: hash = AccumulateHash(hash, i.GetHashCode()); break;
						case long i: hash = AccumulateHash(hash, (ulong)i); break;
						case byte i: hash = AccumulateHash(hash, i.GetHashCode()); break;
						case ushort i: hash = AccumulateHash(hash, i.GetHashCode()); break;
						case uint i: hash = AccumulateHash(hash, i.GetHashCode()); break;
						case ulong i: hash = AccumulateHash(hash, i); break;
						default: hash = AccumulateHash(hash, subHash.GetHashCode()); break;
					}
				}
				return hash;
			}
		}

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
		public static ulong HashFNV1 (this byte[] data, int start, int length) {
			return new Span<byte>(data, start, length).HashFNV1();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong HashFNV1<T> (this T[] data) where T : unmanaged {
			return data.CastAs<T, byte>().HashFNV1();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe ulong HashFNV1<T> (this in Span<T> data) where T : unmanaged {
			return data.As<byte>().HashFNV1();
		}

		private static xxHashConfig GetHashConfig () {
			var config = new xxHashConfig();
			config.HashSizeInBits = 64;
			return config;
		}
		private static readonly IxxHash HasherXX = xxHashFactory.Instance.Create(GetHashConfig());

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
				using (var stream = new UnmanagedMemoryStream(p, data.Length)) {
					return HasherXX.ComputeHash(stream).Hash.HashXXCompute();
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong HashXX (this byte[] data, int start, int length) {
			using (var stream = new MemoryStream(data, start, length)) {
				return HasherXX.ComputeHash(stream).Hash.HashXXCompute();
			}
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
