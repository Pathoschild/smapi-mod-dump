/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewRoguelike.HatQuests;
using StardewValley;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(SObject), nameof(SObject.sellToStorePrice))]
	internal class GoldOreSellPrice
	{
		public static bool Prefix(SObject __instance, ref int __result)
		{
			if (__instance.ParentSheetIndex == 384)
            {
                int basePrice = HatQuest.HasBuffFor(HatQuestType.TOPHAT) ? 20 : 15;
                __result = basePrice * Game1.getOnlineFarmers().Count;
				return false;
            }

			return true;
		}
	}
}
