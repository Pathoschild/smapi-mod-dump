/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewRoguelike.ChallengeFloors;
using StardewRoguelike.VirtualProperties;
using StardewValley.Locations;

namespace StardewRoguelike.Patches
{
    internal class MineShaftShouldCreateLadderPatch : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(MineShaft), "shouldCreateLadderOnThisLevel");

        public static bool Prefix(MineShaft __instance, ref bool __result)
        {
            if (ChallengeFloor.IsChallengeFloor(__instance))
            {
                ChallengeBase challenge = __instance.get_MineShaftChallengeFloor();
                __result = challenge.ShouldSpawnLadder(__instance);
                return false;
            }

            return true;
        }
    }
}
