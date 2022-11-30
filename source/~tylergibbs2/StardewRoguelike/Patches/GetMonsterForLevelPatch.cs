/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewValley.Locations;
using StardewValley.Monsters;

namespace StardewRoguelike.Patches
{
    internal class GetMonsterForLevelPatch : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(MineShaft), "getMonsterForThisLevel");

        public static void Postfix(ref Monster __result, MineShaft __instance)
        {
            int mineLevel = Roguelike.GetLevelFromMineshaft(__instance);
            bool isHardSkullCaverns = Roguelike.ScalingOrder[^2] <= mineLevel % 48 && mineLevel % 48 <= Roguelike.ScalingOrder[^1];
            bool isLooped = mineLevel > Roguelike.ScalingOrder[^1];

            if (__result is Bat bat && isHardSkullCaverns && isLooped)
                __result = new Bat(bat.Position, 1000);
		}
    }
}
