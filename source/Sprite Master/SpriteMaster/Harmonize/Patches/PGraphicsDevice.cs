using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;
using static SpriteMaster.Harmonize.Harmonize;

namespace SpriteMaster.Harmonize.Patches {
	[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Harmony")]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony")]
	internal static class PGraphicsDevice {
		[Harmonize("Present", fixation: HarmonizeAttribute.Fixation.Prefix, priority: PriorityLevel.Last)]
		internal static bool Present (GraphicsDevice __instance) {
			DrawState.OnPresent();
			return true;
		}

		[Harmonize("Present", fixation: HarmonizeAttribute.Fixation.Prefix, priority: PriorityLevel.Last, platform: HarmonizeAttribute.Platform.Windows)]
		internal static bool Present (GraphicsDevice __instance, Rectangle? sourceRectangle, Rectangle? destinationRectangle, IntPtr overrideWindowHandle) {
			DrawState.OnPresent();
			return true;
		}

		[Harmonize("Present", fixation: HarmonizeAttribute.Fixation.Postfix, priority: PriorityLevel.First)]
		internal static void PresentPost (GraphicsDevice __instance) {
			DrawState.OnPresentPost();
		}

		[Harmonize("Present", fixation: HarmonizeAttribute.Fixation.Postfix, priority: PriorityLevel.First, platform: HarmonizeAttribute.Platform.Windows)]
		internal static void PresentPost (GraphicsDevice __instance, Rectangle? sourceRectangle, Rectangle? destinationRectangle, IntPtr overrideWindowHandle) {
			DrawState.OnPresentPost();
		}
	}
}
