/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.Locations;

namespace StardewRoguelike.Patches
{
    internal class MineShaftUpdateWhenCurrentLocation : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(MineShaft), "UpdateWhenCurrentLocation");

        public static void Postfix(MineShaft __instance, GameTime time)
        {
            ChallengeFloor.DoUpdate(__instance, time);
        }
    }
}
