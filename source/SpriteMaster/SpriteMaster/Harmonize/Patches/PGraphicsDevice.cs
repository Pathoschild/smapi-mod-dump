/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using static SpriteMaster.Harmonize.Harmonize;

namespace SpriteMaster.Harmonize.Patches;

[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Harmony")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony")]
static class PGraphicsDevice {
	#region Present

	//[Harmonize("Present", fixation: Harmonize.Fixation.Postfix, priority: PriorityLevel.Last, critical: false)]
	//internal static void PresentPost(GraphicsDevice __instance, in Rectangle? sourceRectangle, in Rectangle? destinationRectangle, IntPtr overrideWindowHandle) => DrawState.OnPresentPost();

	[Harmonize("Present", fixation: Harmonize.Fixation.Prefix, priority: PriorityLevel.Last)]
	internal static bool Present(GraphicsDevice __instance) {
		DrawState.OnPresent();
		return true;
	}

	[Harmonize("Present", fixation: Harmonize.Fixation.Postfix, priority: PriorityLevel.Last)]
	internal static void OnPresent(GraphicsDevice __instance) {
		DrawState.OnPresentPost();
	}

	#endregion

	#region Reset

	[Harmonize("Reset", fixation: Harmonize.Fixation.Postfix, priority: PriorityLevel.Last)]
	internal static void OnResetPost(GraphicsDevice __instance) {
		DrawState.OnPresentPost();
	}

	#endregion
}
