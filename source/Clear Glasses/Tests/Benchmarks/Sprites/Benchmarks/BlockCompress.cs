/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using BCnEncoder.Encoder;
using BenchmarkDotNet.Attributes;
using DirectXTexNet;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using StbDxtSharp;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TeximpNet.Compression;
using TeximpNet.DDS;
using CompressionQuality = TeximpNet.Compression.CompressionQuality;

namespace Benchmarks.Sprites.Benchmarks;

using ImpNetCompressionFormat = TeximpNet.Compression.CompressionFormat;

public class BlockCompress : Textures8 {
	#region TexImpNet

	private static unsafe byte[] TexImpNet(byte[] source, Vector2I dimensions, bool alpha, ImpNetCompressionFormat format, SurfaceFormat surfaceFormat, CompressionQuality quality) {
		using var compressor = new Compressor();
		compressor.Input.AlphaMode = alpha ? AlphaMode.Premultiplied : AlphaMode.None;
		compressor.Input.GenerateMipmaps = false;
		compressor.Compression.Format = format;
		compressor.Compression.Quality = quality;
		compressor.Compression.SetQuantization(true, true, binaryAlpha: format == ImpNetCompressionFormat.BC1a);

		compressor.Output.IsSRGBColorSpace = true;
		compressor.Output.OutputHeader = false;

		//public MipData (int width, int height, int rowPitch, IntPtr data, bool ownData = true)
		fixed (byte* p = source) {
			using var mipData = new MipData(dimensions.Width, dimensions.Height, dimensions.Width * sizeof(int), (IntPtr)p, false);
			compressor.Input.SetData(mipData, false);
			var memoryBuffer = GC.AllocateUninitializedArray<byte>(surfaceFormat.SizeBytes(dimensions), pinned: true);
			using var stream = memoryBuffer.Stream();
			if (compressor.Process(stream)) {
				return memoryBuffer;
			}
			else {
				throw new Exception("Compression Failed");
			}
		}
	} 

	[Benchmark(Description = "TexImpNet Dxt1 (Highest)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void TexImpNetDxt1(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			data.TempReference = TexImpNet(
				data.Reference,
				data.Size,
				false,
				ImpNetCompressionFormat.BC1,
				SurfaceFormat.Dxt1,
				CompressionQuality.Highest
			);
		}
	}

	[Benchmark(Description = "TexImpNet Dxt1 (Normal)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void TexImpNetNormalDxt1(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			data.TempReference = TexImpNet(
				data.Reference,
				data.Size,
				false,
				ImpNetCompressionFormat.BC1,
				SurfaceFormat.Dxt1,
				CompressionQuality.Normal
			);
		}
	}

	[Benchmark(Description = "TexImpNet Dxt1 (Low)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void TexImpNetLowDxt1(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			data.TempReference = TexImpNet(
				data.Reference,
				data.Size,
				false,
				ImpNetCompressionFormat.BC1,
				SurfaceFormat.Dxt1,
				CompressionQuality.Fastest
			);
		}
	}

	[Benchmark(Description = "TexImpNet Dxt1a (Highest)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void TexImpNetDxt1a(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			data.TempReference = TexImpNet(
				data.Reference,
				data.Size,
				true,
				ImpNetCompressionFormat.BC1a,
				SurfaceFormat.Dxt1a,
				CompressionQuality.Highest
			);
		}
	}

	[Benchmark(Description = "TexImpNet Dxt1a (Normal)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void TexImpNetNormalDxt1a(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			data.TempReference = TexImpNet(
				data.Reference,
				data.Size,
				true,
				ImpNetCompressionFormat.BC1a,
				SurfaceFormat.Dxt1a,
				CompressionQuality.Normal
			);
		}
	}

	[Benchmark(Description = "TexImpNet Dxt1a (Low)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void TexImpNetLowDxt1a(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			data.TempReference = TexImpNet(
				data.Reference,
				data.Size,
				true,
				ImpNetCompressionFormat.BC1a,
				SurfaceFormat.Dxt1a,
				CompressionQuality.Fastest
			);
		}
	}

	[Benchmark(Description = "TexImpNet Dxt5 (Highest)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void TexImpNetDxt5(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			data.TempReference = TexImpNet(
				data.Reference,
				data.Size,
				true,
				ImpNetCompressionFormat.BC3,
				SurfaceFormat.Dxt5,
				CompressionQuality.Highest
			);
		}
	}

	[Benchmark(Description = "TexImpNet Dxt5 (Normal)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void TexImpNetNormalDxt5(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			data.TempReference = TexImpNet(
				data.Reference,
				data.Size,
				true,
				ImpNetCompressionFormat.BC3,
				SurfaceFormat.Dxt5,
				CompressionQuality.Normal
			);
		}
	}

	[Benchmark(Description = "TexImpNet Dxt5 (Low)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void TexImpNetLowDxt5(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			data.TempReference = TexImpNet(
				data.Reference,
				data.Size,
				true,
				ImpNetCompressionFormat.BC3,
				SurfaceFormat.Dxt5,
				CompressionQuality.Fastest
			);
		}
	}

	#endregion

	#region BCnEncoder

	[Benchmark(Description = "BCnEncoder Dxt1 (Highest)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void BCnEncoderDxt1(in SpriteDataSet dataSet) {
		var encoder = new BCnEncoder.Encoder.BcEncoder(BCnEncoder.Shared.CompressionFormat.Bc1) {
			Options = {
				IsParallel = false
			},
			OutputOptions = {
				GenerateMipMaps = false,
				Quality = BCnEncoder.Encoder.CompressionQuality.BestQuality
			}
		};

		foreach (var data in dataSet.Data) {
			data.TempReference = encoder.EncodeToRawBytes(data.Span, data.Size.Width, data.Size.Height, PixelFormat.Argb32)[0];
		}
	}

	[Benchmark(Description = "BCnEncoder Dxt1 (Normal)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void BCnEncoderNormalDxt1(in SpriteDataSet dataSet) {
		var encoder = new BCnEncoder.Encoder.BcEncoder(BCnEncoder.Shared.CompressionFormat.Bc1) {
			Options = {
				IsParallel = false
			},
			OutputOptions = {
				GenerateMipMaps = false,
				Quality = BCnEncoder.Encoder.CompressionQuality.Balanced
			}
		};

		foreach (var data in dataSet.Data) {
			data.TempReference = encoder.EncodeToRawBytes(data.Span, data.Size.Width, data.Size.Height, PixelFormat.Argb32)[0];
		}
	}

	[Benchmark(Description = "BCnEncoder Dxt1 (Low)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void BCnEncoderLowDxt1(in SpriteDataSet dataSet) {
		var encoder = new BCnEncoder.Encoder.BcEncoder(BCnEncoder.Shared.CompressionFormat.Bc1) {
			Options = {
				IsParallel = false
			},
			OutputOptions = {
				GenerateMipMaps = false,
				Quality = BCnEncoder.Encoder.CompressionQuality.Fast
			}
		};

		foreach (var data in dataSet.Data) {
			data.TempReference = encoder.EncodeToRawBytes(data.Span, data.Size.Width, data.Size.Height, PixelFormat.Argb32)[0];
		}
	}

	[Benchmark(Description = "BCnEncoder Dxt1 (Parallel)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void BCnEncoderParallelDxt1(in SpriteDataSet dataSet) {
		var encoder = new BCnEncoder.Encoder.BcEncoder(BCnEncoder.Shared.CompressionFormat.Bc1) {
			Options = {
				IsParallel = true
			},
			OutputOptions = {
				GenerateMipMaps = false,
				Quality = BCnEncoder.Encoder.CompressionQuality.BestQuality
			}
		};

		foreach (var data in dataSet.Data) {
			data.TempReference = encoder.EncodeToRawBytes(data.Span, data.Size.Width, data.Size.Height, PixelFormat.Argb32)[0];
		}
	}

	[Benchmark(Description = "BCnEncoder Dxt1a (Highest)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void BCnEncoderDxt1a(in SpriteDataSet dataSet) {
		var encoder = new BCnEncoder.Encoder.BcEncoder(BCnEncoder.Shared.CompressionFormat.Bc1WithAlpha) {
			Options = {
				IsParallel = false
			},
			OutputOptions = {
				GenerateMipMaps = false,
				Quality = BCnEncoder.Encoder.CompressionQuality.BestQuality
			}
		};

		foreach (var data in dataSet.Data) {
			data.TempReference = encoder.EncodeToRawBytes(data.Span, data.Size.Width, data.Size.Height, PixelFormat.Argb32)[0];
		}
	}

	[Benchmark(Description = "BCnEncoder Dxt1a (Normal)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void BCnEncoderNormalDxt1a(in SpriteDataSet dataSet) {
		var encoder = new BCnEncoder.Encoder.BcEncoder(BCnEncoder.Shared.CompressionFormat.Bc1WithAlpha) {
			Options = {
				IsParallel = false
			},
			OutputOptions = {
				GenerateMipMaps = false,
				Quality = BCnEncoder.Encoder.CompressionQuality.Balanced
			}
		};

		foreach (var data in dataSet.Data) {
			data.TempReference = encoder.EncodeToRawBytes(data.Span, data.Size.Width, data.Size.Height, PixelFormat.Argb32)[0];
		}
	}

	[Benchmark(Description = "BCnEncoder Dxt1a (Low)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void BCnEncoderLowDxt1a(in SpriteDataSet dataSet) {
		var encoder = new BCnEncoder.Encoder.BcEncoder(BCnEncoder.Shared.CompressionFormat.Bc1WithAlpha) {
			Options = {
				IsParallel = false
			},
			OutputOptions = {
				GenerateMipMaps = false,
				Quality = BCnEncoder.Encoder.CompressionQuality.Fast
			}
		};

		foreach (var data in dataSet.Data) {
			data.TempReference = encoder.EncodeToRawBytes(data.Span, data.Size.Width, data.Size.Height, PixelFormat.Argb32)[0];
		}
	}

	[Benchmark(Description = "BCnEncoder Dxt1a (Parallel)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void BCnEncoderParallelDxt1a(in SpriteDataSet dataSet) {
		var encoder = new BCnEncoder.Encoder.BcEncoder(BCnEncoder.Shared.CompressionFormat.Bc1WithAlpha) {
			Options = {
				IsParallel = true
			},
			OutputOptions = {
				GenerateMipMaps = false,
				Quality = BCnEncoder.Encoder.CompressionQuality.BestQuality
			}
		};

		foreach (var data in dataSet.Data) {
			data.TempReference = encoder.EncodeToRawBytes(data.Span, data.Size.Width, data.Size.Height, PixelFormat.Argb32)[0];
		}
	}

	[Benchmark(Description = "BCnEncoder Dxt5 (Highest)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void BCnEncoderDxt5(in SpriteDataSet dataSet) {
		var encoder = new BCnEncoder.Encoder.BcEncoder(BCnEncoder.Shared.CompressionFormat.Bc3) {
			Options = {
				IsParallel = false
			},
			OutputOptions = {
				GenerateMipMaps = false,
				Quality = BCnEncoder.Encoder.CompressionQuality.BestQuality
			}
		};

		foreach (var data in dataSet.Data) {
			data.TempReference = encoder.EncodeToRawBytes(data.Span, data.Size.Width, data.Size.Height, PixelFormat.Argb32)[0];
		}
	}

	[Benchmark(Description = "BCnEncoder Dxt5 (Normal)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void BCnEncoderNormalDxt5(in SpriteDataSet dataSet) {
		var encoder = new BCnEncoder.Encoder.BcEncoder(BCnEncoder.Shared.CompressionFormat.Bc3) {
			Options = {
				IsParallel = false
			},
			OutputOptions = {
				GenerateMipMaps = false,
				Quality = BCnEncoder.Encoder.CompressionQuality.Balanced
			}
		};

		foreach (var data in dataSet.Data) {
			data.TempReference = encoder.EncodeToRawBytes(data.Span, data.Size.Width, data.Size.Height, PixelFormat.Argb32)[0];
		}
	}

	[Benchmark(Description = "BCnEncoder Dxt5 (Low)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void BCnEncoderLowDxt5(in SpriteDataSet dataSet) {
		var encoder = new BCnEncoder.Encoder.BcEncoder(BCnEncoder.Shared.CompressionFormat.Bc3) {
			Options = {
				IsParallel = false
			},
			OutputOptions = {
				GenerateMipMaps = false,
				Quality = BCnEncoder.Encoder.CompressionQuality.Fast
			}
		};

		foreach (var data in dataSet.Data) {
			data.TempReference = encoder.EncodeToRawBytes(data.Span, data.Size.Width, data.Size.Height, PixelFormat.Argb32)[0];
		}
	}

	[Benchmark(Description = "BCnEncoder Dxt5 (Parallel)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void BCnEncoderParallelDxt5(in SpriteDataSet dataSet) {
		var encoder = new BCnEncoder.Encoder.BcEncoder(BCnEncoder.Shared.CompressionFormat.Bc3) {
			Options = {
				IsParallel = true
			},
			OutputOptions = {
				GenerateMipMaps = false,
				Quality = BCnEncoder.Encoder.CompressionQuality.BestQuality
			}
		};

		foreach (var data in dataSet.Data) {
			data.TempReference = encoder.EncodeToRawBytes(data.Span, data.Size.Width, data.Size.Height, PixelFormat.Argb32)[0];
		}
	}

	#endregion

	#region Stb

	[Benchmark(Description = "Stb Dxt1 (Highest)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void StbDxt1(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			data.TempReference = StbDxtSharp.StbDxt.CompressDxt1(data.Size.Width, data.Size.Height, data.Reference, CompressionMode.HighQuality);
		}
	}

	[Benchmark(Description = "Stb Dxt5 (Highest)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void StbDxt5(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			data.TempReference = StbDxtSharp.StbDxt.CompressDxt5(data.Size.Width, data.Size.Height, data.Reference, CompressionMode.HighQuality);
		}
	}

	#endregion

	#region DirectXTex

	private static unsafe byte[] DirectXTex(byte[] source, Vector2I size, DXGI_FORMAT format) {
		using var scratch = DirectXTexNet.TexHelper.Instance.Initialize2D(
			DXGI_FORMAT.B8G8R8A8_UNORM_SRGB, size.Width, size.Height, 1, 1, CP_FLAGS.NONE
		);
		var image = scratch.GetImage(0);
		byte* dstPixels = (byte*)image.Pixels;
		long offset = 0;
		long sourceOffset = 0;
		for (int y = 0; y < size.Height; y++) {
			Unsafe.CopyBlockUnaligned(
				ref Unsafe.AsRef(*(dstPixels + offset)),
				ref Unsafe.AddByteOffset(ref MemoryMarshal.GetArrayDataReference(source), (IntPtr)(ulong)sourceOffset),
				(uint)image.Width * 4
			);
			offset += image.RowPitch;
			sourceOffset += image.Width * 4;
		}

		using var compressed = scratch.Compress(DXGI_FORMAT.BC1_UNORM_SRGB, TEX_COMPRESS_FLAGS.SRGB, 0.5f);
		var compressedImage = compressed.GetImage(0);
		byte* outData = (byte*)compressedImage.Pixels;
		SurfaceFormat surfaceFormat = format switch {
			DXGI_FORMAT.BC1_UNORM_SRGB => SurfaceFormat.Dxt1SRgb,
			DXGI_FORMAT.BC1_UNORM => SurfaceFormat.Dxt1,
			DXGI_FORMAT.BC1_TYPELESS => SurfaceFormat.Dxt1,
			DXGI_FORMAT.BC2_UNORM_SRGB => SurfaceFormat.Dxt3SRgb,
			DXGI_FORMAT.BC2_UNORM => SurfaceFormat.Dxt3,
			DXGI_FORMAT.BC2_TYPELESS => SurfaceFormat.Dxt3,
			DXGI_FORMAT.BC3_UNORM_SRGB => SurfaceFormat.Dxt5SRgb,
			DXGI_FORMAT.BC3_UNORM => SurfaceFormat.Dxt5,
			DXGI_FORMAT.BC3_TYPELESS => SurfaceFormat.Dxt5,
			_ => ThrowHelper.ThrowInvalidOperationException<SurfaceFormat>($"Unknown Format: {format}")
		};
		int byteSize = surfaceFormat.SizeBytes(size);
		Vector2I blockSize = surfaceFormat.BlockEdge();
		int rowSize = surfaceFormat.SizeBytes(new Vector2I(size.Width, blockSize.Y));
		byte[] result = GC.AllocateUninitializedArray<byte>(byteSize);
		sourceOffset = 0;
		long destOffset = 0;
		for (int y = 0; y < size.Height; y += blockSize.Y) {
			Unsafe.CopyBlockUnaligned(
				ref Unsafe.AddByteOffset(ref MemoryMarshal.GetArrayDataReference(result), (IntPtr)(ulong)destOffset),
				ref Unsafe.AsRef(*(outData + sourceOffset)),
				(uint)compressedImage.RowPitch
			);
			offset += rowSize;
			sourceOffset += compressedImage.RowPitch;
		}

		return result;
	}

	[Benchmark(Description = "DirectXTex Dxt1")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void DirectXTexDxt1(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			data.TempReference = DirectXTex(data.Reference, data.Size, DXGI_FORMAT.BC1_UNORM_SRGB);
		}
	}

	[Benchmark(Description = "DirectXTex Dxt5")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void DirectXTexDxt5(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			data.TempReference = DirectXTex(data.Reference, data.Size, DXGI_FORMAT.BC3_UNORM_SRGB);
		}
	}

	#endregion

#region SmStb

#if false
	[Benchmark(Description = "SmStb Dxt1 (Highest)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void SmStbDxt1(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			int size = SpriteMaster.Resample.Encoder.StbBlockEncoder.RequiredSize(data.Size, false);
			var buffer = GC.AllocateUninitializedArray<byte>(size);
			SpriteMaster.Resample.Encoder.StbBlockEncoder.CompressDxt(buffer, data.Span, data.Size, false, SpriteMaster.Resample.Encoder.StbBlockEncoder.CompressionMode.HighQuality);
			data.TempReference = buffer;
		}
	}

	[Benchmark(Description = "SmStb Dxt1 (Experimental, Highest)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void SmStbExpDxt1(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			int size = SpriteMaster.Resample.Encoder.StbBlockEncoderExp.RequiredSize(data.Size, false);
			var buffer = GC.AllocateUninitializedArray<byte>(size);
			SpriteMaster.Resample.Encoder.StbBlockEncoderExp.CompressDxt(buffer, data.Span, data.Size, false, SpriteMaster.Resample.Encoder.StbBlockEncoderExp.CompressionMode.HighQuality);
			data.TempReference = buffer;
		}
	}

	[Benchmark(Description = "SmStb Dxt5 (Highest)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void SmStbDxt5(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			int size = SpriteMaster.Resample.Encoder.StbBlockEncoder.RequiredSize(data.Size, true);
			var buffer = GC.AllocateUninitializedArray<byte>(size);
			SpriteMaster.Resample.Encoder.StbBlockEncoder.CompressDxt(buffer, data.Span, data.Size, true, SpriteMaster.Resample.Encoder.StbBlockEncoder.CompressionMode.HighQuality);
			data.TempReference = buffer;
		}
	}

	[Benchmark(Description = "SmStb Dxt5 (Experimental, Highest)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void SmStbExpDxt5(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			int size = SpriteMaster.Resample.Encoder.StbBlockEncoderExp.RequiredSize(data.Size, true);
			var buffer = GC.AllocateUninitializedArray<byte>(size);
			SpriteMaster.Resample.Encoder.StbBlockEncoderExp.CompressDxt(buffer, data.Span, data.Size, true, SpriteMaster.Resample.Encoder.StbBlockEncoderExp.CompressionMode.HighQuality);
			data.TempReference = buffer;
		}
	}
#endif

#endregion
}
