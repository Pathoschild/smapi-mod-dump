/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/NeatAdditions
**
*************************************************/

using Harmony;
using StardewValley;
using StardewValley.Menus;

namespace NeatAdditions.CornerFlashGlitch
{
	class DialogueBox_draw : IPatch
	{
		public string GetPatchName() => "top left corner flash glitch fix";
		
		public static bool Prefix(Dialogue __instance, bool ___transitioning, int ___transitionWidth, int ___transitionHeight)
			=> !___transitioning || (___transitionWidth > 0 && ___transitionHeight > 0);
		
		public void Patch(HarmonyInstance harmony)
		{
			var original = (new DialogueBox(1,1,1,1)).GetType().GetMethod("draw");
			var prefix = GetType().GetMethod("Prefix");
			harmony.Patch(original, new HarmonyMethod(prefix), null);
		}
	}
}
