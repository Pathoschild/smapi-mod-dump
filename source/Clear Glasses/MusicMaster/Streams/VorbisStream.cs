/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Audio;
using MusicMaster.Extensions;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace MusicMaster.Streams;

internal sealed class VorbisStream : AudioStream {
	private const bool UseSimd = true;
	private static readonly bool UseSsse3 = UseSimd && Extensions.Simd.Support.Enabled && Ssse3.IsSupported;

	private const int VectorsPerIteration = 4;

	private static readonly int VectorElementsSize =
		UseSsse3 ? 4 :
		1;

	private static readonly int VectorSize = VectorElementsSize * 32;

	private const float SampleRange = 0.99999994f;
	private const float SampleExpandedRange = SampleRange * 2.0f;

	private readonly MemoryStream _stream;
	protected override Stream DelegatingStream => _stream;

	internal override AudioChannels Channels { get; init; }
	internal override int Frequency { get; init; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static short Convert(float sample) {
		sample = Math.Clamp(sample, -SampleRange, SampleRange);
		sample += SampleRange;
		sample /= SampleExpandedRange;

		int sampleUnsigned32 = Math.Clamp((int)(sample * ushort.MaxValue), 0, ushort.MaxValue);
		short sampleSigned16 = unchecked((short)(sampleUnsigned32 - 32_768));

		return sampleSigned16;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ConvertBufferScalar(Span<short> destination, ReadOnlySpan<float> source, bool stereo, int stride) {
		int channels = stereo ? 2 : 1;
		if (channels == stride) {
			for (int index = 0; index < destination.Length; ++index) {
				destination[index] = Convert(source[index]);
			}
		}
		else {
			int sourceIndex = 0;
			for (int destIndex = 0; destIndex < destination.Length; destIndex += channels, sourceIndex += stride) {
				for (int i = 0; i < channels; ++i) {
					destination[destIndex + i] = Convert(source[sourceIndex + i]);
				}
			}
		}
	}

	/*
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void ConvertBufferSsse3(Span<short> destination, ReadOnlySpan<float> source, bool stereo, int stride) {
		int channels = stereo ? 2 : 1;
		uint sourceOffset = 0;

		fixed (float* sourcePtr = source) {
			fixed (short* destPtr = destination) {
				Vector128<float> minSampleVec = Vector128.Create(-SampleRange);
				Vector128<float> maxSampleVec = Vector128.Create(SampleRange);
				Vector128<float> expandedVec = Vector128.Create(SampleExpandedRange);
				Vector128<float> integerVec = Vector128.Create((float)ushort.MaxValue);
				Vector128<int> subtractVec = Vector128.Create(32_768);

				uint blocks = (uint)destination.Length / 4u;
				for (uint block = 0; block < blocks; ++block) {
					var in0 = Sse.LoadVector128(sourcePtr + sourceOffset + (sizeof(float) * 0));
					var in1 = Sse.LoadVector128(sourcePtr + sourceOffset + (sizeof(float) * 1));
					sourceOffset += sizeof(float) * 2u;
					var in2 = Sse.LoadVector128(sourcePtr + sourceOffset + (sizeof(float) * 0));
					var in3 = Sse.LoadVector128(sourcePtr + sourceOffset + (sizeof(float) * 1));
					sourceOffset += sizeof(float) * 2u;

					var vec0 = Sse.Max(in0, minSampleVec);
					var vec1 = Sse.Max(in1, minSampleVec);
					var vec2 = Sse.Max(in2, minSampleVec);
					var vec3 = Sse.Max(in3, minSampleVec);

					vec0 = Sse.Min(vec0, maxSampleVec);
					vec1 = Sse.Min(vec1, maxSampleVec);
					vec2 = Sse.Min(vec2, maxSampleVec);
					vec3 = Sse.Min(vec3, maxSampleVec);

					vec0 = Sse.Add(vec0, maxSampleVec);
					vec1 = Sse.Add(vec1, maxSampleVec);
					vec2 = Sse.Add(vec2, maxSampleVec);
					vec3 = Sse.Add(vec3, maxSampleVec);

					// TODO : Combine this divide and multiply

					vec0 = Sse.Divide(vec0, expandedVec);
					vec1 = Sse.Divide(vec1, expandedVec);
					vec2 = Sse.Divide(vec2, expandedVec);
					vec3 = Sse.Divide(vec3, expandedVec);

					vec0 = Sse.Multiply(vec0, integerVec);
					vec1 = Sse.Multiply(vec1, integerVec);
					vec2 = Sse.Multiply(vec2, integerVec);
					vec3 = Sse.Multiply(vec3, integerVec);

					var iVec0 = Sse2.ConvertToVector128Int32(vec0);
					var iVec1 = Sse2.ConvertToVector128Int32(vec1);
					var iVec2 = Sse2.ConvertToVector128Int32(vec2);
					var iVec3 = Sse2.ConvertToVector128Int32(vec3);

					iVec0 = Sse2.Subtract(iVec0, subtractVec);
					iVec1 = Sse2.Subtract(iVec1, subtractVec);
					iVec2 = Sse2.Subtract(iVec2, subtractVec);
					iVec3 = Sse2.Subtract(iVec3, subtractVec);

					Ssse3.Shuffle(iVec0.As<int, byte>(), )

					Vector128<int> sVec0;
				}

				int sourceIndex = 0;
				for (int destIndex = 0; destIndex < destination.Length; destIndex += channels, sourceIndex += stride) {
					for (int i = 0; i < channels; ++i) {
						destination[destIndex + i] = Convert(source[sourceIndex + i]);
					}
				}
			}
		}
	}
	*/

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ConvertBuffer(Span<short> destination, ReadOnlySpan<float> source, bool stereo, int stride) {
		/*
		if (source.Length <= VectorElementsSize * VectorsPerIteration) {
			ConvertBufferScalar(destination, source, stereo, stride);
		}
		else {
			if (UseSsse3) {
				ConvertBufferSsse3(destination, source, stereo, stride);
			}
			else {
				ConvertBufferScalar(destination, source, stereo, stride);
			}
		}
		*/

		ConvertBufferScalar(destination, source, stereo, stride);
	}

	internal VorbisStream(string path) : base(null) {
		using var reader = new NVorbis.VorbisReader(path) {
			ClipSamples = false
		};

		var buffer = GC.AllocateUninitializedArray<float>(checked((int)reader.TotalSamples));

		int readOffset = 0;
		int readLength = 0;
		int remainingLength = buffer.Length;
		while ((readLength = reader.ReadSamples(buffer, readOffset, remainingLength)) > 0) {
			readOffset += readLength;
			remainingLength -= readLength;
		}
		// TODO : check if readOffset is not TotalSamples, but zero was returned. What to do: no idea.

		// convert float to 16-bit PCM
		// Mono or Stereo Only
		int pcmChannels = Math.Min(reader.Channels, 2);
		int samplesPerChannel = checked((int)(buffer.Length / reader.Channels));
		var pcmBuffer = new byte[sizeof(short) * (samplesPerChannel * pcmChannels)];
		ConvertBuffer(pcmBuffer.AsSpan().Cast<short>(), buffer, reader.Channels >= 2, reader.Channels);

		_stream = new MemoryStream(pcmBuffer);

		Channels = pcmChannels >= 2 ? AudioChannels.Stereo : AudioChannels.Mono;
		Frequency = reader.SampleRate;
	}
}