/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/UpgradeablePan
**
*************************************************/

using StardewValley;
using StardewValley.Tools;

namespace UpgradablePan
{
	public class ItemPatches
	{
		public static bool canBeTrashed_Prefix(ref Item __instance, ref bool __result)
		{
			// Make it so you can't throw pans away.
			if (__instance is Pan)
			{
				__result = false;
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}