/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using MonoGame.OpenGL;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace SpriteMaster.GL;

internal static partial class GraphicsDeviceExt {
	private static class Enabled {
		internal static bool DrawUserIndexedPrimitives = true;
	}

	internal static unsafe class SpriteBatcherValues {
		internal const int MaxBatchSize = SpriteBatcher.MaxBatchSize;
		internal static readonly short[] Indices16 = GC.AllocateUninitializedArray<short>(MaxBatchSize * 6);
		internal static readonly int[] Indices32 = GC.AllocateUninitializedArray<int>(MaxBatchSize * 6);

		internal static readonly Lazy<GLExt.ObjectId> IndexBuffer16 = new(() => GetIndexBuffer(Indices16), mode: LazyThreadSafetyMode.None);
		internal static readonly Lazy<GLExt.ObjectId> IndexBuffer32 = new(() => GetIndexBuffer(Indices32), mode: LazyThreadSafetyMode.None);

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

		static SpriteBatcherValues() {
			ref short ref16 = ref MemoryMarshal.GetArrayDataReference(Indices16);
			ref int ref32 = ref MemoryMarshal.GetArrayDataReference(Indices32);

			for (int i = 0, indexOffset = 0; i < MaxBatchSize; ++i, indexOffset += 6) {
				int index0 = i * 4;
				int index1 = index0 + 1;
				int index2 = index0 + 2;
				int index3 = index0 + 1;
				int index4 = index0 + 3;
				int index5 = index0 + 2;

				Unsafe.Add(ref ref16, indexOffset + 0) = (short)index0;
				Unsafe.Add(ref ref16, indexOffset + 1) = (short)index1;
				Unsafe.Add(ref ref16, indexOffset + 2) = (short)index2;
				Unsafe.Add(ref ref16, indexOffset + 3) = (short)index3;
				Unsafe.Add(ref ref16, indexOffset + 4) = (short)index4;
				Unsafe.Add(ref ref16, indexOffset + 5) = (short)index5;

				Unsafe.Add(ref ref32, indexOffset + 0) = index0;
				Unsafe.Add(ref ref32, indexOffset + 1) = index1;
				Unsafe.Add(ref ref32, indexOffset + 2) = index2;
				Unsafe.Add(ref ref32, indexOffset + 3) = index3;
				Unsafe.Add(ref ref32, indexOffset + 4) = index4;
				Unsafe.Add(ref ref32, indexOffset + 5) = index5;
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
	private static int GetElementCountArray(this PrimitiveType primitiveType, int primitiveCount) =>
		primitiveType switch {
			PrimitiveType.TriangleList => (int)((uint)primitiveCount * 3u),
			PrimitiveType.TriangleStrip => primitiveCount + 2,
			PrimitiveType.LineList => (int)((uint)primitiveCount * 2u),
			PrimitiveType.LineStrip => primitiveCount + 1,
			_ => ThrowHelper.ThrowNotSupportedException<int>(primitiveType.ToString())
		};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetTriangleListCount(int primitiveCount) => (int)((uint)primitiveCount * 3u);

	internal static unsafe void DrawUserIndexedPrimitivesFlushVertexArray<TVertex>(
		GraphicsDevice @this,
		PrimitiveType primitiveType,
		TVertex[] vertexData,
		int vertexOffset,
		int numVertices,
		short[] indexData,
		int indexOffset,
		int primitiveCount,
		VertexDeclaration vertexDeclaration
	) where TVertex : unmanaged {
		bool InnerCall() {
			if (!SMConfig.Extras.OpenGL.Enabled || !SMConfig.Extras.OpenGL.OptimizeDrawUserIndexedPrimitives || !Enabled.DrawUserIndexedPrimitives) {
				return false;
			}

			try {
				var indexBuffer = SpriteBatcherValues.IndexBuffer16.Value;
				if (indexBuffer == GLExt.ObjectId.None) {
					Debug.Error($"Disabling OpenGL Optimization due to index buffer not being created");
					Enabled.DrawUserIndexedPrimitives = false;
					return false;
				}

				@this.ApplyState(true);

				// Unbind current VBOs.
				MonoGame.OpenGL.GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
				GraphicsExtensions.CheckGLError();
				MonoGame.OpenGL.GL.BindBuffer(BufferTarget.ElementArrayBuffer, (int)indexBuffer);
				GraphicsExtensions.CheckGLError();
				@this._indexBufferDirty = true;

				// Pin the buffers.
				fixed (TVertex* vbPtr = vertexData) {
					nint vertexPointer = (nint)vbPtr;
					vertexPointer += vertexDeclaration.VertexStride * vertexOffset;

					// Setup the vertex declaration to point at the VB data.
					vertexDeclaration.GraphicsDevice = @this;
					vertexDeclaration.Apply(
						shader: @this._vertexShader,
						offset: vertexPointer,
						programHash: @this.ShaderProgramHash
					);

					Contract.Assert(primitiveType == PrimitiveType.TriangleList);

					//Draw
					GLExt.Checked(
						() => MonoGame.OpenGL.GL.DrawElements(
							GLPrimitiveType.Triangles,
							GetTriangleListCount(primitiveCount),
							DrawElementsType.UnsignedShort,
							(nint)indexOffset * sizeof(short)
						)
					);
				}
			}
			catch (Exception ex) when (ex is MemberAccessException or MonoGameGLException) {
				Debug.Error($"Disabling OpenGL Optimization due to {nameof(MemberAccessException)}", ex);
				Enabled.DrawUserIndexedPrimitives = false;
				return false;
			}

			return true;
		};

		if (!InnerCall()) {
			@this.DrawUserIndexedPrimitives(
				primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount,
				vertexDeclaration
			);
		}
	}

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
		if (!SMConfig.Extras.OpenGL.Enabled || !SMConfig.Extras.OpenGL.OptimizeDrawUserIndexedPrimitives || !Enabled.DrawUserIndexedPrimitives) {
			return false;
		}

		try {
			@this.ApplyState(true);

			// Unbind current VBOs.
			MonoGame.OpenGL.GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GraphicsExtensions.CheckGLError();
			MonoGame.OpenGL.GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
			GraphicsExtensions.CheckGLError();
			@this._indexBufferDirty = true;

			// Pin the buffers.
			fixed (TVertex* vbPtr = vertexData) {
				nint vertexPointer = (nint)vbPtr;
				vertexPointer += vertexDeclaration.VertexStride * vertexOffset;

				// Setup the vertex declaration to point at the VB data.
				vertexDeclaration.GraphicsDevice = @this;
				vertexDeclaration.Apply(
					shader: @this._vertexShader,
					offset: vertexPointer,
					programHash: @this.ShaderProgramHash
				);

				fixed (TIndex* ibPtr = indexData) {
					var offsetIndexPtr = (nint)(ibPtr + indexOffset);

					//Draw
					GLExt.Checked(() => MonoGame.OpenGL.GL.DrawElements(
						primitiveType.GetGl(),
						primitiveType.GetElementCountArray(primitiveCount),
						DrawElementsType.UnsignedShort,
						offsetIndexPtr
					));
				}
			}
		}
		catch (Exception ex) when (ex is MemberAccessException or MonoGameGLException) {
			Debug.Error($"Disabling OpenGL Optimization due to exception", ex);
			Enabled.DrawUserIndexedPrimitives = false;
			return false;
		}

		return true;
	}
}
