/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics.CodeAnalysis;
using static SpriteMaster.Harmonize.Harmonize;

namespace SpriteMaster.Harmonize.Patches;

[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Harmony")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony")]
internal static class PGraphicsDevice {
	#region Present

	//[Harmonize("Present", fixation: Harmonize.Fixation.Postfix, priority: PriorityLevel.Last, critical: false)]
	//internal static void PresentPost(GraphicsDevice __instance, XRectangle? sourceRectangle, XRectangle? destinationRectangle, IntPtr overrideWindowHandle) => DrawState.OnPresentPost();

	[Harmonize("Present", fixation: Fixation.Prefix, priority: PriorityLevel.Last)]
	public static void PresentPre(GraphicsDevice __instance) {
		DrawState.OnPresent();
	}

	[Harmonize("Present", fixation: Fixation.Postfix, priority: PriorityLevel.Last)]
	public static void PresentPost(GraphicsDevice __instance) {
		DrawState.OnPresentPost();
	}

	#endregion

	[Harmonize(typeof(Microsoft.Xna.Framework.Game), "BeginDraw", fixation: Fixation.Prefix, priority: PriorityLevel.First)]
	public static void OnBeginDraw(Microsoft.Xna.Framework.Game __instance) {
		DrawState.OnBeginDraw();
	}

	#region Reset

	[Harmonize("Reset", fixation: Fixation.Postfix, priority: PriorityLevel.Last)]
	public static void OnResetPost(GraphicsDevice __instance) {
		DrawState.OnPresentPost();
	}

	#endregion

	[Harmonize("SetVertexAttributeArray", fixation: Fixation.Prefix, priority: PriorityLevel.Last)]
	public static bool OnSetVertexAttributeArray(GraphicsDevice __instance, bool[] attrs) {
		return !GL.GraphicsDeviceExt.SetVertexAttributeArray(
			__instance,
			attrs
		);
	}

	#region OnPlatformDrawUserIndexedPrimitives

	[Harmonize(
		"PlatformDrawUserIndexedPrimitives",
		Fixation.Prefix,
		PriorityLevel.Last,
		generic: Generic.GfxVertex
	)]
	public static unsafe bool OnDrawUserIndexedPrimitives<T>(
		GraphicsDevice __instance,
		PrimitiveType primitiveType,
		T[] vertexData,
		int vertexOffset,
		int numVertices,
		short[] indexData,
		int indexOffset,
		int primitiveCount,
		VertexDeclaration vertexDeclaration
	) where T : unmanaged {
		return !GL.GraphicsDeviceExt.DrawUserIndexedPrimitives(
			__instance,
			primitiveType,
			vertexData,
			vertexOffset,
			numVertices,
			indexData,
			indexOffset,
			primitiveCount,
			vertexDeclaration
		);
	}

	[Harmonize(
		"PlatformDrawUserIndexedPrimitives",
		Fixation.Prefix,
		PriorityLevel.Last,
		generic: Generic.GfxVertex
	)]
	public static unsafe bool OnDrawUserIndexedPrimitives<T>(
		GraphicsDevice __instance,
		PrimitiveType primitiveType,
		T[] vertexData,
		int vertexOffset,
		int numVertices,
		int[] indexData,
		int indexOffset,
		int primitiveCount,
		VertexDeclaration vertexDeclaration
	) where T : unmanaged {
		return !GL.GraphicsDeviceExt.DrawUserIndexedPrimitives(
			__instance,
			primitiveType,
			vertexData,
			vertexOffset,
			numVertices,
			indexData,
			indexOffset,
			primitiveCount,
			vertexDeclaration
		);
	}

	#endregion
}
