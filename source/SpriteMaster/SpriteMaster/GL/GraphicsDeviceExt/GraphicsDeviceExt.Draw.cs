/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

#define ENABLE_VBO
// #define ENABLE_VBO_MULTI

using Microsoft.Toolkit.HighPerformance;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.OpenGL;
using SpriteMaster.Extensions;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace SpriteMaster.GL;

internal static partial class GraphicsDeviceExt {
	static GraphicsDeviceExt() {
		SMConfig.ConfigChanged += OnConfigChanged;
		OnConfigChanged();
	}

	private static class Enabled {
		internal static bool DrawUserIndexedPrimitivesInternal = true;
		internal static bool DrawUserIndexedPrimitives = DrawUserIndexedPrimitivesInternal;
	}

	internal static unsafe class SpriteBatcherValues {
		internal const int MaxBatchSize = SpriteBatcher.MaxBatchSize;
		internal const int MaxIndicesCount = MaxBatchSize * 6;
		internal static readonly short[] Indices16 = GC.AllocateUninitializedArray<short>(MaxIndicesCount);

		internal static readonly Lazy<GLExt.ObjectId> IndexBuffer16 = new(() => GetIndexBuffer(Indices16), mode: LazyThreadSafetyMode.None);

		// Creates an OpenGL index buffer object given the provided data.
		private static GLExt.ObjectId GetIndexBuffer<T>(T[] data) where T : unmanaged {
			try {
				MonoGame.OpenGL.GL.GenBuffers(1, out int indexBuffer);
				GraphicsExtensions.CheckGLError();
				MonoGame.OpenGL.GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
				GraphicsExtensions.CheckGLError();
				fixed (T* ptr = data) {
					MonoGame.OpenGL.GL.BufferData(
						BufferTarget.ElementArrayBuffer,
						(nint)data.Length * sizeof(T),
						(nint)ptr,
						BufferUsageHint.StaticDraw
					);
				}

				GraphicsExtensions.CheckGLError();

				return (GLExt.ObjectId)indexBuffer;
			}
			catch (Exception) {
				return GLExt.ObjectId.None;
			}
		}

		// Returns the greatest array index possible given the number of elements.
		// We know that the 4th offset of each element (which is two triangles, 6 indices total) is the greatest,
		// so we can rapidly calculate a value based upon that assumption.
		internal static uint GetMaxArrayIndex(uint numElements, uint offset) {
			if (numElements == 0u) {
				return 0u;
			}

			if (offset != 0) {
				numElements += offset / 6;
				offset %= 6;
				if (offset > 4) {
					++numElements;
				}
			}

			return ((numElements - 1u) * 4u) + 3u;
		}

		static SpriteBatcherValues() {
			fixed (short* ptr16 = Indices16) {
				for (uint i = 0, indexOffset = 0; i < MaxBatchSize; ++i, indexOffset += 6u) {
					uint index0 = i * 4u;
					uint index1 = index0 + 1u;
					uint index2 = index0 + 2u;
					uint index3 = index0 + 1u;
					uint index4 = index0 + 3u;
					uint index5 = index0 + 2u;

					ptr16[indexOffset + 0u] = (short)(ushort)index0;
					ptr16[indexOffset + 1u] = (short)(ushort)index1;
					ptr16[indexOffset + 2u] = (short)(ushort)index2;
					ptr16[indexOffset + 3u] = (short)(ushort)index3;
					ptr16[indexOffset + 4u] = (short)(ushort)index4;
					ptr16[indexOffset + 5u] = (short)(ushort)index5;
				}
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static GLPrimitiveType GetGl(this PrimitiveType primitiveType) =>
		primitiveType switch {
			PrimitiveType.TriangleList => GLPrimitiveType.Triangles,
			PrimitiveType.TriangleStrip => GLPrimitiveType.TriangleStrip,
			PrimitiveType.LineList => GLPrimitiveType.Lines,
			PrimitiveType.LineStrip => GLPrimitiveType.LineStrip,
			_ => ThrowHelper.ThrowArgumentException<GLPrimitiveType>(nameof(primitiveType))
		};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint GetElementCountArray(this PrimitiveType primitiveType, int primitiveCount) =>
		primitiveType switch {
			PrimitiveType.TriangleList => (uint)primitiveCount * 3u,
			PrimitiveType.TriangleStrip => (uint)primitiveCount + 2u,
			PrimitiveType.LineList => (uint)primitiveCount * 2u,
			PrimitiveType.LineStrip => (uint)primitiveCount + 1,
			_ => ThrowHelper.ThrowNotSupportedException<uint>(primitiveType.ToString())
		};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static GLExt.ValueType GetExpensiveIndexType(GLExt.ValueType value) {
#if SHIPPING
		Debug.WarningOnce($"Expensive Index Type queried: {value}");
#endif
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe GLExt.ValueType GetIndexType<TIndex>() where TIndex : unmanaged =>
		sizeof(TIndex) switch {
			1 => GetExpensiveIndexType(GLExt.ValueType.UnsignedByte),
			2 => GLExt.ValueType.UnsignedShort,
			4 => GLExt.ValueType.UnsignedInt,
			_ => ThrowHelper.ThrowArgumentException<GLExt.ValueType>(nameof(TIndex))
		};

	private static uint EnabledAttributeBitmask = GetEnabledAttributeBitmask();

	// Gets a bitmask from the current MonoGame enabledVertexAttributes list.
	private static uint GetEnabledAttributeBitmask() {
		uint bitmask = 0u;
		foreach (var attribute in GraphicsDevice._enabledVertexAttributes) {
			bitmask |= 1u << attribute;
		}

		return bitmask;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool SetVertexAttributeArray(GraphicsDevice @this, bool[] attributes) {
		
		// Iterate over each attribute, setting the bitmask's bits appropriately for each offset in the array.
		uint attributeBit = 1u;
		uint bitmask = 0U;
		foreach (var flagValue in attributes) {
			int flag = flagValue.ReinterpretAs<byte>();

			bitmask |= (uint)-flag & attributeBit;

			attributeBit <<= 1;
		}

		// If the bitmask is different, then attribute enablement has changed - take the slow path.
		// This is incredibly rare, surprisingly.
		if (bitmask != EnabledAttributeBitmask) {
			return SetVertexAttributeArraySlowPath(attributes);
		}

		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool SetVertexAttributeBitmask(GraphicsDevice @this, uint bitmask, bool[] attributes) {
		// If the bitmask is different, then attribute enablement has changed - take the slow path.
		// This is incredibly rare, surprisingly.
		if (bitmask != EnabledAttributeBitmask) {
			return SetVertexAttributeArraySlowPath(attributes);
		}

		return true;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static bool SetVertexAttributeArraySlowPath(bool[] attributes) {
		uint attributeBit = 1u;
		uint bitmask = EnabledAttributeBitmask;

		// Iterate over each attribute, compare against the current bitmask, and enable/disable the Vertex Attribute Array as appropriate.
		for (int attribute = 0; attribute < attributes.Length; ++attribute, attributeBit <<= 1) {
			bool flagValue = attributes[attribute];
			int flag = flagValue.ReinterpretAs<byte>();

			uint currentValue = bitmask & attributeBit;
			if (flagValue) {
				if (currentValue == 0u) {
					bitmask |= attributeBit;
					MonoGame.OpenGL.GL.EnableVertexAttribArray(attribute);
				}
			}
			else {
				if (currentValue != 0u) {
					bitmask &= ~attributeBit;
					MonoGame.OpenGL.GL.DisableVertexAttribArray(attribute);
				}
			}

			bitmask = (bitmask & ~attributeBit) | ((uint)-flag & attributeBit);
		}

		EnabledAttributeBitmask = bitmask;

		// Update the MonoGame enabledVertexAttributes list just in-case something needs it.
		var list = GraphicsDevice._enabledVertexAttributes;
		list.Clear();

		// Iterates over the bitmask and adds each offset/index into the list.
		int index = 0;
		while (BitOperations.TrailingZeroCount(bitmask) is var shift && shift < 32) {
			index += shift;
			bitmask >>= shift + 1;
			list.Add(index);
			++index;
		}

		return true;
	}

	private static int LastProgramHash = int.MinValue;
	private static VertexDeclaration.VertexDeclarationAttributeInfo? LastAttributeInfo = null;

	// Their implementation of this already caches using a `Dictionary`, but we can avoid even the overhead of the dictionary lookup
	// in 99% of cases by just using a simple inline cache like this. This hits almost every time.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static VertexDeclaration.VertexDeclarationAttributeInfo GetAttributeInfo(VertexDeclaration @this, Shader shader, int programHash) {
		if (LastProgramHash != programHash || LastAttributeInfo is not {} attributeInfo) {
			LastAttributeInfo = attributeInfo = @this.GetAttributeInfo(shader, programHash);
			LastProgramHash = programHash;
		}

		return attributeInfo;
	}

	private static readonly bool SupportsInstancing = DrawState.Device.GraphicsCapabilities.SupportsInstancing;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void VertexDeclarationApply(VertexDeclaration @this, Shader shader, nint offset, int programHash) {
		VertexDeclaration.VertexDeclarationAttributeInfo attributeInfo = GetAttributeInfo(@this, shader, programHash);

		uint vertexStride = (uint)@this.VertexStride;
		uint bitmask = 0u;

		// It's faster to iterate over a list-span than a list itself. Significantly so (about 5x).
		foreach (var element in attributeInfo.Elements.AsSpan()) {
			// Call our function pointer version - this is called a lot, so we want reduced overhead.
			VertexAttribPointerInternal(
				(uint)element.AttributeLocation,
				element.NumberOfElements,
				(GLExt.ValueType)element.VertexAttribPointerType,
				element.Normalized,
				vertexStride,
				offset + element.Offset
			);

			bitmask |= 1u << element.AttributeLocation;

			// This allows us to avoid a more complex boolean check every iteration of the loop
			if (SupportsInstancing) {
				MonoGame.OpenGL.GL.VertexAttribDivisor(element.AttributeLocation, 0);
			}
		}
		SetVertexAttributeBitmask(@this.GraphicsDevice, bitmask, attributeInfo.EnabledAttributes);
		GraphicsDevice._attribsDirty = true;
	}

	#region BindBufferOverride

	private struct DirtyState {
		internal uint VertexAttribPointer = uint.MaxValue;
		internal bool VertexAttribDivisor = true;

		public DirtyState() {}
	}

	private static readonly Dictionary<BufferTarget, GLExt.ObjectId> OtherBufferBindings = new();
	private static readonly GLExt.ObjectId[] PrimaryBufferBindingsArray = GC.AllocateArray<GLExt.ObjectId>(2, pinned: true);
	private static readonly unsafe GLExt.ObjectId* PrimaryBufferBindings = PrimaryBufferBindingsArray.GetPointerFromPinned();
	private static readonly DirtyState[] BufferBindingsDirtyArray = GC.AllocateArray<DirtyState>(2, pinned: true);
	private static readonly unsafe DirtyState* BufferBindingsDirty = BufferBindingsDirtyArray.GetPointerFromPinned();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void BindBufferOverride(BufferTarget target, int obj) =>
		BindBufferInternal(target, (GLExt.ObjectId)obj);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void BindBufferInternal(BufferTarget target, GLExt.ObjectId obj) {
		int offset = (int)target - (int)BufferTarget.ArrayBuffer;

		if (offset <= 1) {
			if (PrimaryBufferBindings[offset] != obj) {
				PrimaryBufferBindings[offset] = obj;
				BufferBindingsDirty[offset] = new();
				GLExt.BindBuffer(target, obj);
			}
		}
		else {
			BindBufferInternalSlowPath(target, obj);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static unsafe void BindBufferInternalSlowPath(BufferTarget target, GLExt.ObjectId obj) {
		if (!OtherBufferBindings.TryGetValue(target, out var boundObj) || boundObj != obj) {
			OtherBufferBindings[target] = obj;
			GLExt.BindBuffer(target, obj);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe bool BindBufferInternalResult(BufferTarget target, GLExt.ObjectId obj) {
		int offset = (int)target - (int)BufferTarget.ArrayBuffer;

		if (PrimaryBufferBindings[offset] != obj) {
			PrimaryBufferBindings[offset] = obj;
			BufferBindingsDirty[offset] = new();
			GLExt.BindBuffer(target, obj);
			return true;
		}

		return false;
	}

	#endregion

	#region VertexAttributeDivisor

	private static readonly Lazy<int> MaxVertexAttributes = new(
		() => {
			MonoGame.OpenGL.GL.GetInteger((int)MonoGame.OpenGL.GetPName.MaxVertexAttribs, out int value);
			return value;
		}
	);

	private static readonly uint[] VertexAttribDivisorsArray = GC.AllocateArray<uint>(MaxVertexAttributes.Value, pinned: true);
	private static readonly unsafe uint* VertexAttribDivisors = VertexAttribDivisorsArray.GetPointerFromPinned();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void VertexAttribDivisorOverride(int index, int divisor) {
		uint uDivisor = (uint)divisor;
		
		if (BufferBindingsDirty[0].VertexAttribDivisor || VertexAttribDivisors[index] != uDivisor) {
			BufferBindingsDirty[0].VertexAttribDivisor = false;
			VertexAttribDivisors[index] = uDivisor;

			GLExt.VertexAttribDivisor((uint)index, uDivisor);
		}
	}

	#endregion

	#region VertexAttributePointer

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void VertexAttribPointerOverride(
		int location,
		int elementCount,
		VertexAttribPointerType type,
		bool normalize,
		int stride,
		IntPtr data
	) {
		VertexAttribPointerInternal(
			(uint)location,
			elementCount,
			(GLExt.ValueType)type,
			normalize,
			(uint)stride,
			data
		);
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private readonly record struct AttributePointer(
		nint Data,							// 8
		int ElementCount,				// 12
		GLExt.ValueType Type,		// 16
		uint Stride,						// 20
		bool Normalize					// 21 (24)
	) {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe bool FastEquals(in AttributePointer other) {
			ulong* thisPtr = (ulong*)Unsafe.AsPointer(ref Unsafe.AsRef(this));
			ulong* otherPtr = (ulong*)Unsafe.AsPointer(ref Unsafe.AsRef(other));

			return
				thisPtr[0] == otherPtr[0] &&
				thisPtr[1] == otherPtr[1] &&
				thisPtr[2] == otherPtr[2];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe bool FastEqualsBr(in AttributePointer other) {
			ulong* thisPtr = (ulong*)Unsafe.AsPointer(ref Unsafe.AsRef(this));
			ulong* otherPtr = (ulong*)Unsafe.AsPointer(ref Unsafe.AsRef(other));

			return
				(thisPtr[0] == otherPtr[0]) &
				(thisPtr[1] == otherPtr[1]) &
				(thisPtr[2] == otherPtr[2]);
		}
	};

	private static readonly AttributePointer[] VertexAttributePointersArray = GC.AllocateArray<AttributePointer>(MaxVertexAttributes.Value, pinned: true);
	private static readonly unsafe AttributePointer* VertexAttributePointers = VertexAttributePointersArray.GetPointerFromPinned();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void VertexAttribPointerInternal(
		uint location,
		int elementCount,
		GLExt.ValueType type,
		bool normalize,
		uint stride,
		nint data
	) {
		var currentAttributePointer = new AttributePointer(data, elementCount, type, stride, normalize);
		var attributePointer = VertexAttributePointers + location;
		if (!BufferBindingsDirty[0].VertexAttribPointer.GetBit((int)location) && currentAttributePointer.FastEquals(*attributePointer)) {
			return;
		}

		BufferBindingsDirty[0].VertexAttribPointer.ClearBit((int)location);

		*attributePointer = currentAttributePointer;

		GLExt.VertexAttribPointer(
			location,
			elementCount,
			type,
			normalize,
			stride,
			data
		);
	}

	#endregion

	private const uint MaxSpriteBatchCount = SpriteBatcher.MaxBatchSize * 4u;
	private const uint MaxSequentialBufferCount = MaxSpriteBatchCount * 64u;

	private static GLExt.ObjectId MakeTransientVertexBuffer() {
		try {
			MonoGame.OpenGL.GL.GenBuffers(1, out int vertexBuffer);
			GraphicsExtensions.CheckGLError();
			MonoGame.OpenGL.GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
			GraphicsExtensions.CheckGLError();

			unsafe {
				switch (VBOType) {
					case VBType.BufferData:
#if ENABLE_VBO_MULTI
					case VBType.BufferDataMulti:
#endif
						break; // no need to initialize
					case VBType.BufferSubDataFull:
					case VBType.BufferMapDataFull:
						GLExt.BufferData(
							BufferTarget.ArrayBuffer,
							(nint)MaxSpriteBatchCount * (nint)Marshal.SizeOf<VertexPositionColorTexture>(),
							(nint)0,
							(BufferUsageHint)35048
						);
						break;
					case VBType.BufferMapDataSequential:
					case VBType.BufferSubDataSequential:
						GLExt.BufferData(
							BufferTarget.ArrayBuffer,
							(nint)MaxSequentialBufferCount * (nint)Marshal.SizeOf<VertexPositionColorTexture>(),
							(nint)0,
							(BufferUsageHint)35048
						);
						break;
				}
			}

			GraphicsExtensions.CheckGLError();

			return (GLExt.ObjectId)vertexBuffer;
		}
		catch (Exception) {
			return GLExt.ObjectId.None;
		}
	}

	private static readonly Lazy<GLExt.ObjectId> TransientVertexBuffer = new(MakeTransientVertexBuffer);
#if ENABLE_VBO_MULTI
	private static uint CurrentVertexBufferMultiIndex = 0;
	private const uint MultiBuffers = 128;
	private static readonly Lazy<GLExt.ObjectId[]> TransientVertexBufferMulti = new(() => {
			var array = new GLExt.ObjectId[MultiBuffers];
			for (int i = 0; i < array.Length; ++i) {
				array[i] = MakeTransientVertexBuffer();
			}
			return array;
		}
	);

	private static GLExt.ObjectId CurrentTransientVertexBufferMulti => TransientVertexBufferMulti.Value[CurrentVertexBufferMultiIndex++ % MultiBuffers];
#endif

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ApplyVertexDeclaration(
		GraphicsDevice @this,
		VertexDeclaration declaration,
		nint vertexPointer
	) {
		// Setup the vertex declaration to point at the VB data.
		declaration.GraphicsDevice = @this;
		// Use our optimized version.
		VertexDeclarationApply(
			declaration,
			shader: @this._vertexShader,
			offset: vertexPointer,
			programHash: @this.ShaderProgramHash
		);
	}

	private enum VBType {
		BufferData,
		BufferDataMulti,
		BufferSubDataFull,
		BufferSubDataSequential,
		BufferMapDataFull,
		BufferMapDataSequential,
	}

	private const VBType VBOType = VBType.BufferData;
	private static nint CurrentBufferOffset = 0;

	private const BufferAccess WriteOnly = (BufferAccess)35001;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe nint HandleVertexBuffer<TVertex>(
		TVertex[] vertexData,
		int vertexOffset,
		int numVertices,
		VertexDeclaration vertexDeclaration
	) where TVertex : unmanaged {
		nint offset = vertexOffset * vertexDeclaration.VertexStride;
		nint size = (nint)numVertices * vertexDeclaration.VertexStride;

		switch (VBOType) {
			case VBType.BufferData:
			case VBType.BufferDataMulti: {
				fixed (TVertex* vbPtr = vertexData) {
					GLExt.BufferData(
						BufferTarget.ArrayBuffer,
						size,
						(nint)vbPtr + offset,
						BufferUsageHint.StreamDraw
					);
				}

				GraphicsExtensions.CheckGLError();
				return 0;
			}
			case VBType.BufferSubDataFull: {
				fixed (TVertex* vbPtr = vertexData) {
					MonoGame.OpenGL.GL.BufferSubData(
						BufferTarget.ArrayBuffer,
						(nint)0,
						size,
						(nint)vbPtr + offset
					);
				}

				GraphicsExtensions.CheckGLError();
				return 0;
			}
			case VBType.BufferMapDataFull: {
				nint address = MonoGame.OpenGL.GL.MapBuffer(BufferTarget.ArrayBuffer, WriteOnly);
				fixed (TVertex* vbPtr = vertexData) {
					Buffer.MemoryCopy(
						source: (void*)((nint)vbPtr + offset),
						destination: (void*)address,
						destinationSizeInBytes: size,
						sourceBytesToCopy: size
					);
				}
				MonoGame.OpenGL.GL.UnmapBuffer(BufferTarget.ArrayBuffer);

				GraphicsExtensions.CheckGLError();
				return 0;
			}
			case VBType.BufferSubDataSequential: {
				nint bufferSize = (nint)MaxSequentialBufferCount * (nint)Marshal.SizeOf<VertexPositionColorTexture>();

				nint bufferOffset = CurrentBufferOffset;
				if (bufferOffset + size > bufferSize) {
					CurrentBufferOffset = bufferOffset = 0;
				}
				else {
					CurrentBufferOffset += size;
				}

				fixed (TVertex* vbPtr = vertexData) {
					MonoGame.OpenGL.GL.BufferSubData(
						BufferTarget.ArrayBuffer,
						bufferOffset,
						size,
						(nint)vbPtr + offset
					);
				}

				GraphicsExtensions.CheckGLError();
				return bufferOffset;
			}
			case VBType.BufferMapDataSequential: {
				nint bufferSize = (nint)MaxSequentialBufferCount * (nint)Marshal.SizeOf<VertexPositionColorTexture>();

				nint bufferOffset = CurrentBufferOffset;
				if (bufferOffset + size > bufferSize) {
					CurrentBufferOffset = bufferOffset = 0;
				}
				else {
					CurrentBufferOffset += size;
				}

				nint address = MonoGame.OpenGL.GL.MapBuffer(BufferTarget.ArrayBuffer, WriteOnly);
				fixed (TVertex* vbPtr = vertexData) {
					Buffer.MemoryCopy(
						source: (void*)((nint)vbPtr + offset),
						destination: (void*)(address + bufferOffset),
						destinationSizeInBytes: size,
						sourceBytesToCopy: size
					);
				}
				MonoGame.OpenGL.GL.UnmapBuffer(BufferTarget.ArrayBuffer);

				GraphicsExtensions.CheckGLError();
				return bufferOffset;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static GLExt.ObjectId GetTransientVertexBuffer() {
#if ENABLE_VBO_MULTI
		if (VBOType is VBType.BufferDataMulti) {
			return CurrentTransientVertexBufferMulti;
		}
		else
#endif
		{
			return TransientVertexBuffer.Value;
		}
	}

	private const bool UseVertexBufferObject = true;
	internal static unsafe bool DrawUserIndexedPrimitives<TVertex, TIndex>(
		GraphicsDevice @this,
		PrimitiveType primitiveType,
		TVertex[] vertexData,
		int vertexOffset,
		int numVertices,
		TIndex[] indexData,
		int indexOffset,
		int primitiveCount,
		VertexDeclaration vertexDeclaration
	) where TVertex : unmanaged where TIndex : unmanaged {
		++DrawState.Statistics.DrawCalls;

		if (!Enabled.DrawUserIndexedPrimitives) {
			return false;
		}

		try {
			bool isSpriteBatcher = sizeof(TIndex) == 2 && ReferenceEquals(indexData, SpriteBatcherValues.Indices16);
			bool useVertexBufferObject = UseVertexBufferObject && isSpriteBatcher;

			// TODO : This can be optimized as well - lots of interdependant booleans.
			@this.ApplyState(true);

			(GLExt.ObjectId vbo, GLExt.ObjectId ibo) = useVertexBufferObject ?
				(GetTransientVertexBuffer(), SpriteBatcherValues.IndexBuffer16.Value) :
				(GLExt.ObjectId.None, GLExt.ObjectId.None);

			// Unbind current VBOs.
			BindBufferInternal(BufferTarget.ArrayBuffer, vbo);
			GraphicsExtensions.CheckGLError();
			if (BindBufferInternalResult(BufferTarget.ElementArrayBuffer, ibo)) {
				@this._indexBufferDirty = true;
			}
			GraphicsExtensions.CheckGLError();

			var count = primitiveType.GetElementCountArray(primitiveCount);

			// Perform as much work outside of 'fixed' as possible so as to limit the time the GC might have to stall if it is triggered.
			nint vertexPointerOffset = vertexDeclaration.VertexStride * vertexOffset;

			if (useVertexBufferObject) {
				nint offset = HandleVertexBuffer<TVertex>(vertexData, vertexOffset, numVertices, vertexDeclaration);

				// Setup the vertex declaration to point at the VB data.
				ApplyVertexDeclaration(@this, vertexDeclaration, offset);

				GLExt.DrawRangeElements(
					GLPrimitiveType.Triangles,
					0,
					SpriteBatcherValues.GetMaxArrayIndex((uint)primitiveCount, (uint)indexOffset),
					count,
					GLExt.ValueType.UnsignedShort,
					indexOffset
				);
				GraphicsExtensions.CheckGLError();
			}
			else {
				var type = primitiveType.GetGl();

				// Pin the buffers.
				fixed (TVertex* vbPtr = vertexData) {
					nint vertexPointer = (nint)vbPtr + vertexPointerOffset;

					// Setup the vertex declaration to point at the VB data.
					ApplyVertexDeclaration(@this, vertexDeclaration, vertexPointer);

					fixed (TIndex* ibPtr = indexData) {
						var offsetIndexPtr = (nint)(ibPtr + indexOffset);

						//Draw
						// If we are drawing from the pre-cached spritebatcher indices, we can use `glDrawRangeElements` instead.
						if (isSpriteBatcher) {
							GLExt.DrawRangeElements(
								GLPrimitiveType.Triangles,
								0,
								SpriteBatcherValues.GetMaxArrayIndex((uint)primitiveCount, (uint)indexOffset),
								count,
								GLExt.ValueType.UnsignedShort,
								offsetIndexPtr
							);
						}
						else {
							GLExt.DrawElements(
								type,
								count,
								GetIndexType<TIndex>(),
								offsetIndexPtr
							);
						}

						GraphicsExtensions.CheckGLError();
					}
				}
			}
		}
		catch (Exception ex) when (ex is MemberAccessException or MonoGameGLException) {
			Debug.Error($"Disabling OpenGL Optimization due to exception", ex);
			Enabled.DrawUserIndexedPrimitivesInternal = false;
			OnConfigChanged();
			return false;
		}

		return true;
	}

	private static readonly MonoGame.OpenGL.GL.BindBufferDelegate OriginalBindBuffer =
		MonoGame.OpenGL.GL.BindBuffer;

	private static readonly MonoGame.OpenGL.GL.VertexAttribDivisorDelegate OriginalVertexAttribDivisor =
		MonoGame.OpenGL.GL.VertexAttribDivisor;

	private static readonly MonoGame.OpenGL.GL.VertexAttribPointerDelegate OriginalVertexAttribPointer =
		MonoGame.OpenGL.GL.VertexAttribPointer;

	// When the config changes, update the enablement booleans.
	private static void OnConfigChanged() {
		Enabled.DrawUserIndexedPrimitives =
			Enabled.DrawUserIndexedPrimitivesInternal &&
			SMConfig.Extras.OpenGL.Enabled &&
			SMConfig.Extras.OpenGL.OptimizeDrawUserIndexedPrimitives;

		MonoGame.OpenGL.GL.BindBuffer = BindBufferOverride;
		MonoGame.OpenGL.GL.VertexAttribDivisor = VertexAttribDivisorOverride;
		MonoGame.OpenGL.GL.VertexAttribPointer = VertexAttribPointerOverride;
	}
}
