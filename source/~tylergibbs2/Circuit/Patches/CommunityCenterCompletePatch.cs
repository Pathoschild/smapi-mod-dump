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
using StardewValley.Locations;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(CommunityCenter), "doAreaCompleteReward")]
    internal class CommunityCenterCompletePatch
    {
        public static void Postfix(int whichArea)
        {
            if (!ModEntry.ShouldPatch())
                return;

            ModEntry.Instance.TaskManager?.OnCommunityCenterRoomComplete(whichArea);
        }
    }
}
