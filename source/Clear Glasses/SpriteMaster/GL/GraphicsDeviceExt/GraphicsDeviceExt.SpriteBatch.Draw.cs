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

internal static partial class GraphicsDeviceExt {

#if false

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetTriangleListCount(int primitiveCount) => (int)((uint)primitiveCount * 3u);

	private static class IndexBuffer16Cache {
		internal static readonly GLExt.ObjectId Value;

		static IndexBuffer16Cache() {
			var indexBuffer = SpriteBatcherValues.IndexBuffer16.Value;
			if (indexBuffer == GLExt.ObjectId.None) {
				Debug.Error($"Disabling OpenGL Optimization due to index buffer not being created");
				Enabled.DrawUserIndexedPrimitivesInternal = false;
			}

			Value = indexBuffer;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe bool DrawUserIndexedPrimitivesFlushVertexArrayInner<TVertex>(
		GraphicsDevice @this,
		PrimitiveType primitiveType,
		TVertex[] vertexData,
		int vertexOffset,
		int numVertices,
		int indexOffset,
		int primitiveCount,
		VertexDeclaration vertexDeclaration
	) where TVertex : unmanaged {
		/*
		if (!SMConfig.Extras.OpenGL.Enabled || !SMConfig.Extras.OpenGL.OptimizeDrawUserIndexedPrimitives || !Enabled.DrawUserIndexedPrimitives) {
			return false;
		}

		const int indexDataLength = SpriteBatcherValues.MaxBatchSize * 6;
		*/

		/*
		if (vertexData == null || vertexData.Length == 0)
			throw new ArgumentNullException(nameof(vertexData));

		if (vertexOffset < 0 || vertexOffset >= vertexData.Length)
			throw new ArgumentOutOfRangeException(nameof(vertexOffset));

		if (numVertices <= 0 || numVertices > vertexData.Length)
			throw new ArgumentOutOfRangeException(nameof(numVertices));

		if (vertexOffset + numVertices > vertexData.Length)
			throw new ArgumentOutOfRangeException(nameof(numVertices));

		if (indexOffset < 0 || indexOffset >= indexDataLength)
			throw new ArgumentOutOfRangeException(nameof(indexOffset));

		if (primitiveCount <= 0)
			throw new ArgumentOutOfRangeException(nameof(primitiveCount));

		if (indexOffset + GetElementCountArray(primitiveType, primitiveCount) > indexDataLength)
			throw new ArgumentOutOfRangeException(nameof(primitiveCount));

		if (vertexDeclaration == null)
			throw new ArgumentNullException(nameof(vertexDeclaration));

		if (vertexDeclaration.VertexStride < sizeof(TVertex))
			throw new ArgumentOutOfRangeException(nameof(vertexDeclaration), "Vertex stride of vertexDeclaration should be at least as big as the stride of the actual vertices.");
		*/
		
		//try {
			var indexBuffer = IndexBuffer16Cache.Value;

			@this.ApplyState(true);

			// Unbind current VBOs.
			MonoGame.OpenGL.GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GraphicsExtensions.CheckGLError();
			MonoGame.OpenGL.GL.BindBuffer(BufferTarget.ElementArrayBuffer, (int)indexBuffer);
			GraphicsExtensions.CheckGLError();
			@this._indexBufferDirty = true;

			// Pin the buffers.
			fixed (TVertex* vbPtr = vertexData) {
				var vertexPointer = (nint)vbPtr;
				vertexPointer += vertexDeclaration.VertexStride * vertexOffset;

				// Setup the vertex declaration to point at the VB data.
				vertexDeclaration.GraphicsDevice = @this;
				vertexDeclaration.Apply(
					shader: @this._vertexShader,
					offset: vertexPointer,
					programHash: @this.ShaderProgramHash
				);

				Contract.Assert(primitiveType == PrimitiveType.TriangleList);
				
				uint elementCount = (uint)GetTriangleListCount(primitiveCount);

				//Draw
				GLExt.DrawRangeElements(
					GLPrimitiveType.Triangles,
					0,
					SpriteBatcherValues.GetMaxArrayIndex(elementCount, (uint)indexOffset),
					elementCount,
					GLExt.ValueType.UnsignedShort,
					(nint)indexOffset * sizeof(short)
				);
				GraphicsExtensions.CheckGLError();
			}

			unchecked {
				@this._graphicsMetrics._drawCount++;
				@this._graphicsMetrics._primitiveCount += primitiveCount;
			}
			/*
		}
		catch (Exception ex) when (ex is MemberAccessException or MonoGameGLException) {
			Debug.Error($"Disabling OpenGL Optimization due to {nameof(MemberAccessException)}", ex);
			Enabled.DrawUserIndexedPrimitives = false;
			return false;
		}
			*/

		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		if (!DrawUserIndexedPrimitivesFlushVertexArrayInner<TVertex>(@this, primitiveType, vertexData, vertexOffset, numVertices, indexOffset, primitiveCount, vertexDeclaration)) {
			@this.DrawUserIndexedPrimitives(
				primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount,
				vertexDeclaration
			);
		}
	}
#endif

}
