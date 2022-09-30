/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using StardewValley;

namespace StardewRoguelike.Patches
{
	internal class GoldOreSellPrice : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new(typeof(Object), "sellToStorePrice");

		public static bool Prefix(Object __instance, ref int __result)
		{
			if (__instance.ParentSheetIndex == 384)
            {
				__result = 15 * Game1.getOnlineFarmers().Count;
				return false;
            }

			return true;
		}
	}
}
