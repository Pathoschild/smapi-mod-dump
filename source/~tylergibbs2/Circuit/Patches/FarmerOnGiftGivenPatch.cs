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
using StardewValley;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(Farmer), nameof(Farmer.onGiftGiven))]
    internal class FarmerOnGiftGivenPatch
    {
        public static void Postfix(NPC npc, SObject item)
        {
            if (!ModEntry.ShouldPatch() || item.bigCraftable.Value)
                return;

            if (Game1.player.hasItemBeenGifted(npc, item.ParentSheetIndex))
                ModEntry.Instance.TaskManager?.OnItemGifted(npc, item);
        }
    }
}
