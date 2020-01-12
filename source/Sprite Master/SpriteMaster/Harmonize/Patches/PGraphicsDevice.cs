using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SpriteMaster.Harmonize.Patches {
	[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Harmony")]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony")]
	internal static class PGraphicsDevice {
		[Harmonize("Present")]
		internal static bool Present (GraphicsDevice __instance) {
			DrawState.OnPresent();
			return true;
		}

		[Harmonize("Present", platform: HarmonizeAttribute.Platform.Windows)]
		internal static bool Present (GraphicsDevice __instance, Rectangle? sourceRectangle, Rectangle? destinationRectangle, IntPtr overrideWindowHandle) {
			DrawState.OnPresent();
			return true;
		}
	}
}
