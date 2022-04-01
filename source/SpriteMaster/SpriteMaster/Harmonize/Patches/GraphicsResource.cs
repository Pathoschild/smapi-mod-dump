/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

namespace SpriteMaster.Harmonize.Patches;

static class GraphicsResource {
	/*
	[Harmonize("~GraphicsResource", Harmonize.Fixation.Prefix, PriorityLevel.First)]
	private static void Finalize(XNA.Graphics.GraphicsResource __instance) {
		switch (__instance) {
			case InternalTexture2D _:
				return;
			case Texture2D texture:
				if (!texture.IsDisposed) {
					//Debug.Trace($"Another module has leaked the following texture: {texture.GetType().FullName} '{texture.Name}' {texture} {(Bounds)texture.Bounds}");
				}
				break;
		}
	}
	*/
}
