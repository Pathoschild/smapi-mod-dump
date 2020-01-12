using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using static SpriteMaster.Harmonize.Harmonize;

namespace SpriteMaster.Harmonize.Patches {
	[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Harmony")]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony")]

	internal static class PRenderTarget2D {
		/*
		[HarmonyPatch("CreateRenderTarget", HarmonyPatch.Fixation.Prefix, PriorityLevel.Last)]
		private static bool CreateRenderTarget (RenderTarget2D __instance, GraphicsDevice graphicsDevice, ref int width, ref int height, [MarshalAs(UnmanagedType.U1)] ref bool mipMap, SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat, ref int preferredMultiSampleCount, ref RenderTargetUsage usage) {
			const int Scale = 4;
			const int MSAA = 2;
			
			if (width >= graphicsDevice.Viewport.Width && height >= graphicsDevice.Viewport.Height) {
				width = Math.Min(Config.ClampDimension, width * Scale);
				height = Math.Min(Config.ClampDimension, height * Scale);
			}
			else {
				width = Math.Min(Config.ClampDimension, Math.Max(graphicsDevice.Viewport.Width * Scale, width * Scale));
				height = Math.Min(Config.ClampDimension, Math.Max(graphicsDevice.Viewport.Height * Scale, height * Scale));
			}
			preferredMultiSampleCount = Config.DrawState.EnableMSAA ? Math.Max(MSAA, preferredMultiSampleCount) : preferredMultiSampleCount;
			// This is required to prevent aliasing effects.
			mipMap = true;
			//usage = RenderTargetUsage.DiscardContents;

			return true;
		}
		*/
	}
}
