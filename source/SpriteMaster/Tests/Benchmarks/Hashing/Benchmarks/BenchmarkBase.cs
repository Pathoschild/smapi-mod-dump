/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using BenchmarkDotNet.Attributes;
using Benchmarks.BenchmarkBase.Benchmarks;
using Microsoft.Toolkit.HighPerformance;
using Murmur;
using SpriteMaster.Hashing.Algorithms;
using System.Data.HashFunction.CityHash;
using System.Data.HashFunction.xxHash;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Benchmarks.Hashing.Benchmarks;
public abstract class BenchmarkBaseHashing<TDataType, TBase> : BenchmarkBaseImpl<TDataType, TBase> where TDataType : IDataSet<TBase> {
	[GlobalSetup]
	public virtual void AlwaysRunBefore() {
		RuntimeHelpers.RunClassConstructor(typeof(XxHash3).TypeHandle);
	}

	private static class Impl {
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static ArgumentException ThrowArgumentException() =>
			new();

		internal interface IHashImpl<T> where T : unmanaged {
			T Hash(string value);
			T Hash(byte[] value);

			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal T Hash(string[] value) {
				if (typeof(T) == typeof(uint)) {
					uint result = default;
					foreach (var str in value) {
						result ^= (uint)(object)Hash(str);
					}

					return (T)(object)result;
				}
				else {
					ulong result = default;
					foreach (var str in value) {
						result ^= (ulong)(object)Hash(str);
					}

					return (T)(object)result;
				}
			}
		}

		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static T Hash<T, TDelegate>(TDataType data, TDelegate del = default!)
			where TDelegate : notnull, IHashImpl<T> where T : unmanaged {
			switch (data.Data) {
				case string str:
					return del.Hash(str);
				case byte[] bytes:
					return del.Hash(bytes);
				case string[] strings:
					return del.Hash(strings);
				default:
					throw ThrowArgumentException();
			}
		}

		internal struct XxHash3Impl : IHashImpl<ulong> {
			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ulong Hash(string value) {
				return SpriteMaster.Hashing.Algorithms.XxHash3.Hash64(value);
			}

			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ulong Hash(byte[] value) {
				return SpriteMaster.Hashing.Algorithms.XxHash3.Hash64(value);
			}
		}

		internal struct XxHash3CppClang : IHashImpl<ulong> {
			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public unsafe ulong Hash(string value) {
				var span = value.AsSpan().AsBytes();
				fixed (byte* data = span) {
					return Functions.XxHash3NativeClang(data, (ulong)span.Length);
				}
			}

			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public unsafe ulong Hash(byte[] value) {
				fixed (byte* data = value) {
					return Functions.XxHash3NativeClang(data, (ulong)value.Length);
				}
			}
		}

		internal struct XxHash32CppClang : IHashImpl<uint> {
			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public unsafe uint Hash(string value) {
				var span = value.AsSpan().AsBytes();
				fixed (byte* data = span) {
					return Functions.XxH32NativeClang(data, (ulong)span.Length, 0U);
				}
			}

			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public unsafe uint Hash(byte[] value) {
				fixed (byte* data = value) {
					return Functions.XxH32NativeClang(data, (ulong)value.Length, 0U);
				}
			}
		}

		internal struct XxHash64CppClang : IHashImpl<ulong> {
			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public unsafe ulong Hash(string value) {
				var span = value.AsSpan().AsBytes();
				fixed (byte* data = span) {
					return Functions.XxH64NativeClang(data, (ulong)span.Length, 0U);
				}
			}

			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public unsafe ulong Hash(byte[] value) {
				fixed (byte* data = value) {
					return Functions.XxH64NativeClang(data, (ulong)value.Length, 0U);
				}
			}
		}

		internal struct XxHash3CppMSVC : IHashImpl<ulong> {
			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public unsafe ulong Hash(string value) {
				var span = value.AsSpan().AsBytes();
				fixed (byte* data = span) {
					return Functions.XxHash3NativeVC(data, (ulong)span.Length);
				}
			}

			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public unsafe ulong Hash(byte[] value) {
				fixed (byte* data = value) {
					return Functions.XxHash3NativeVC(data, (ulong)value.Length);
				}
			}
		}

		internal struct XxHash32CppMSVC : IHashImpl<uint> {
			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public unsafe uint Hash(string value) {
				var span = value.AsSpan().AsBytes();
				fixed (byte* data = span) {
					return Functions.XxH32NativeVC(data, (ulong)span.Length, 0U);
				}
			}

			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public unsafe uint Hash(byte[] value) {
				fixed (byte* data = value) {
					return Functions.XxH32NativeVC(data, (ulong)value.Length, 0U);
				}
			}
		}

		internal struct XxHash64CppMSVC : IHashImpl<ulong> {
			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public unsafe ulong Hash(string value) {
				var span = value.AsSpan().AsBytes();
				fixed (byte* data = span) {
					return Functions.XxH64NativeVC(data, (ulong)span.Length, 0U);
				}
			}

			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public unsafe ulong Hash(byte[] value) {
				fixed (byte* data = value) {
					return Functions.XxH64NativeVC(data, (ulong)value.Length, 0U);
				}
			}
		}

		internal struct XxHash32K4Os : IHashImpl<uint> {
			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public uint Hash(string value) {
				var byteSpan = value.AsSpan().AsBytes();
				return K4os.Hash.xxHash.XXH32.DigestOf(byteSpan);
			}

			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public uint Hash(byte[] value) {
				return K4os.Hash.xxHash.XXH32.DigestOf(value);
			}
		}

		internal struct XxHash64K4Os : IHashImpl<ulong> {
			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ulong Hash(string value) {
				var byteSpan = value.AsSpan().AsBytes();
				return K4os.Hash.xxHash.XXH64.DigestOf(byteSpan);
			}

			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ulong Hash(byte[] value) {
				return K4os.Hash.xxHash.XXH64.DigestOf(value);
			}
		}

		internal struct XxHash3DotNet : IHashImpl<ulong> {
			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ulong Hash(string value) {
				var byteSpan = value.AsSpan().AsBytes();
				return XXHash3NET.XXHash3.Hash64(byteSpan);
			}

			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ulong Hash(byte[] value) {
				return XXHash3NET.XXHash3.Hash64(value);
			}
		}

		internal struct XxHashFactory : IHashImpl<ulong> {
			private static readonly IxxHash XXHashInterface =
				xxHashFactory.Instance.Create(new xxHashConfig {HashSizeInBits = 64});

			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ulong Hash(string value) {
				var byteSpan = value.AsSpan().AsBytes();
				return BitConverter.ToUInt64(XXHashInterface.ComputeHash(byteSpan.ToArray()).Hash);
			}

			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ulong Hash(byte[] value) {
				return BitConverter.ToUInt64(XXHashInterface.ComputeHash(value).Hash);
			}
		}

		internal struct CityHashImpl : IHashImpl<ulong> {
			private static readonly ICityHash CityHashInterface =
				CityHashFactory.Instance.Create(new CityHashConfig {HashSizeInBits = 64});

			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ulong Hash(string value) {
				var byteSpan = value.AsSpan().AsBytes();
				return BitConverter.ToUInt64(CityHashInterface.ComputeHash(byteSpan.ToArray()).Hash);
			}

			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ulong Hash(byte[] value) {
				return BitConverter.ToUInt64(CityHashInterface.ComputeHash(value).Hash);
			}
		}

		internal struct Murmur128Impl : IHashImpl<ulong> {
			private static readonly Murmur128 Murmur128Interface = MurmurHash.Create128(managed: true);

			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ulong Hash(string value) {
				var byteSpan = value.AsSpan().AsBytes();
				Span<byte> output = stackalloc byte[16];
				Murmur128Interface.TryComputeHash(byteSpan, output, out _);
				return BitConverter.ToUInt64(output);
			}

			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ulong Hash(byte[] value) {
				return BitConverter.ToUInt64(Murmur128Interface.ComputeHash(value));
			}
		}

		internal struct Marvin32Impl : IHashImpl<uint> {
			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public uint Hash(string value) {
				return (uint)value.GetHashCode();
			}

			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public uint Hash(byte[] value) {
				return (uint)Methods.Marvin32.ComputeHash32(value);
			}
		}

		internal struct Fnv1aImpl : IHashImpl<uint> {
			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public uint Hash(string value) {
				var byteSpan = value.AsSpan().AsBytes();
				return (uint)Functions.FNV1a(byteSpan);
			}

			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public uint Hash(byte[] value) {
				return (uint)Functions.FNV1a(value);
			}
		}
	}

	#region xxHash3 .NET

	[Benchmark(Description = "xxHash3")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong XxHash3(TDataType dataSet) {
		return Impl.Hash<ulong, Impl.XxHash3Impl>(dataSet);
	}

	#endregion

	#region xxHash3 C++

	[Benchmark(Description = "xxHash3 C++ (Clang)", Baseline = true)]
	[ArgumentsSource(nameof(DataSets), Priority = 2)]
	public ulong XxHash3CPP_Clang(TDataType dataSet) {
		return Impl.Hash<ulong, Impl.XxHash3CppClang>(dataSet);
	}

	[Benchmark(Description = "xxHash32 C++ (Clang)")]
	[ArgumentsSource(nameof(DataSets), Priority = 2)]
	public uint XxHash32CPP_Clang(TDataType dataSet) {
		return Impl.Hash<uint, Impl.XxHash32CppClang>(dataSet);
	}

	[Benchmark(Description = "xxHash64 C++ (Clang)")]
	[ArgumentsSource(nameof(DataSets), Priority = 2)]
	public ulong XxHash64CPP_Clang(TDataType dataSet) {
		return Impl.Hash<ulong, Impl.XxHash64CppClang>(dataSet);
	}

	[Benchmark(Description = "xxHash3 C++ (MSVC)")]
	[ArgumentsSource(nameof(DataSets), Priority = 2)]
	public ulong XxHash3CPP_MSVC(TDataType dataSet) {
		return Impl.Hash<ulong, Impl.XxHash3CppMSVC>(dataSet);
	}

	[Benchmark(Description = "xxHash32 C++ (MSVC)")]
	[ArgumentsSource(nameof(DataSets), Priority = 2)]
	public uint XxHash32CPP_MSVC(TDataType dataSet) {
		return Impl.Hash<uint, Impl.XxHash32CppMSVC>(dataSet);
	}

	[Benchmark(Description = "xxHash64 C++ (MSVC)")]
	[ArgumentsSource(nameof(DataSets), Priority = 2)]
	public ulong XxHash64CPP_MSVC(TDataType dataSet) {
		return Impl.Hash<ulong, Impl.XxHash64CppMSVC>(dataSet);
	}

#if false
	[Benchmark(Description = "xxHash3 C++/CLI")]
	[ArgumentsSource(nameof(DataSets), Priority = 2)]
	public ulong xxHash3CPP_CLI(TDataType dataSet) {
		switch (dataSet.Data) {
			case string str:
				var byteSpan = str.AsSpan().AsBytes();
				return Functions.XxHash3CLI(byteSpan);
			case byte[] bytes:
				return Functions.XxHash3CLI(bytes);
			default:
				throw new ArgumentException();
		}
	}

	[Benchmark(Description = "xxHash3 C++/CLI (Experimental)")]
	[ArgumentsSource(nameof(DataSets), Priority = 2)]
	public ulong xxHash3CPP_CLI_Experimental(TDataType dataSet) {
		switch (dataSet.Data) {
			case string str:
				var byteSpan = str.AsSpan().AsBytes();
				return Functions.XxHash3CLITest(byteSpan);
			case byte[] bytes:
				return Functions.XxHash3CLITest(bytes);
			default:
				throw new ArgumentException();
		}
	}

	/*
	[Benchmark(Description = "xxHash3 C++/CLI (from C#)")]
	[ArgumentsSource(nameof(DataSets), Priority = 2)]
	public ulong xxHash3CPP_CLITest(TDataType dataSet) {
		return Functions.XxHash3CLITest(dataSet.Data);
	}
	*/
#endif

	#endregion

	#region Other

	[Benchmark(Description = "K4os XXHash32")]
	[ArgumentsSource(nameof(DataSets), Priority = 2)]
	public uint XxHash32K4os(TDataType dataSet) {
		return Impl.Hash<uint, Impl.XxHash32K4Os>(dataSet);
	}

	[Benchmark(Description = "K4os XXHash64")]
	[ArgumentsSource(nameof(DataSets), Priority = 2)]
	public ulong XxHash64K4os(TDataType dataSet) {
		return Impl.Hash<ulong, Impl.XxHash64K4Os>(dataSet);
	}

	[Benchmark(Description = "XXHash3NET")]
	[ArgumentsSource(nameof(DataSets), Priority = 2)]
	public ulong xXHash3Net(TDataType dataSet) {
		return Impl.Hash<ulong, Impl.XxHash3DotNet>(dataSet);
	}

	[Benchmark(Description = "xxHashFactory")]
	[ArgumentsSource(nameof(DataSets), Priority = 2)]
	public ulong xXHashFactory(TDataType dataSet) {
		return Impl.Hash<ulong, Impl.XxHashFactory>(dataSet);
	}

	[Benchmark(Description = "CityHash")]
	[ArgumentsSource(nameof(DataSets), Priority = 2)]
	public ulong CityHash(TDataType dataSet) {
		return Impl.Hash<ulong, Impl.CityHashImpl>(dataSet);
	}

	[Benchmark(Description = "Murmur128")]
	[ArgumentsSource(nameof(DataSets), Priority = 2)]
	public ulong Murmur128(TDataType dataSet) {
		return Impl.Hash<ulong, Impl.Murmur128Impl>(dataSet);
	}

	[Benchmark(Description = "Marvin32")]
	[ArgumentsSource(nameof(DataSets), Priority = 2)]
	public uint Marvin32(TDataType dataSet) {
		return Impl.Hash<uint, Impl.Marvin32Impl>(dataSet);
	}

	[Benchmark(Description = "FNV1a")]
	[ArgumentsSource(nameof(DataSets), Priority = 2)]
	public uint Fnv1a(TDataType dataSet) {
		return Impl.Hash<uint, Impl.Fnv1aImpl>(dataSet);
	}

	#endregion

#if false
	[Benchmark(Description = "xxHash3 C++ (Clang, pointer)")]
	[ArgumentsSource(nameof(DataSets), Priority = 2)]
	public unsafe ulong xxHash3CPPClangPtr(TDataType dataSet) {
		return Functions.XxHash3NativeClang(dataSet.DataPtr, (ulong)dataSet.Data.Length);
	}

	[Benchmark(Description = "xxHash3 C++ (MSVC)")]
	[ArgumentsSource(nameof(DataSets), Priority = 3)]
	public unsafe ulong xxHash3CPPVC(TDataType dataSet) {
		fixed (byte* data = dataSet.Data) {
			return Functions.XxHash3NativeVC(data, (ulong)dataSet.Data.Length);
		}
	}

	[Benchmark(Description = "xxHash3 C++ (MSVC, pointer)")]
	[ArgumentsSource(nameof(DataSets), Priority = 4)]
	public unsafe ulong xxHash3CPPVCPtr(TDataType dataSet) {
		return Functions.XxHash3NativeVC(dataSet.DataPtr, (ulong)dataSet.Data.Length);
	}
#endif

	/*
	[Benchmark(Description = "FNV1a")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong FNV1a(in TDataType dataSet) {
		return Functions.FNV1a(dataSet.Data);
	}

	[Benchmark(Description = "DGB2-32")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public int DJB232(in TDataType dataSet) {
		return dataSet.Data.GetDjb2HashCode();
	}

	[Benchmark(Description = "SHA1")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong SHA1(in TDataType dataSet) {
		return BitConverter.ToUInt64(System.Security.Cryptography.SHA1.HashData(dataSet.Data));
	}

	[Benchmark(Description = "MD5")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong MD5(in TDataType dataSet) {
		return BitConverter.ToUInt64(System.Security.Cryptography.MD5.HashData(dataSet.Data));
	}
	*/

	/*
	private static readonly SHA384 Sha384 = System.Security.Cryptography.SHA384.Create();

	[Benchmark(Description = "SHA384")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong SHA384(in TDataType dataSet) {
		return BitConverter.ToUInt64(Sha384.ComputeHash(dataSet.Data));
	}

	private static readonly SHA512 Sha512 = System.Security.Cryptography.SHA512.Create();

	[Benchmark(Description = "SHA512")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong SHA512(in TDataType dataSet) {
		return BitConverter.ToUInt64(Sha512.ComputeHash(dataSet.Data));
	}*/

	/*
	[Benchmark(Description = "CombHash")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong CombHash(in TDataType dataSet) {
		return Functions.CombHash(dataSet.Data);
	}
	*/
}
