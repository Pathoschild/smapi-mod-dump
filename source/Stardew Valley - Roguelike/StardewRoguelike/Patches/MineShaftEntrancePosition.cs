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
using StardewRoguelike.VirtualProperties;

namespace StardewRoguelike.Patches
{
    internal class MineShaftEntrancePosition : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(MineShaft), "mineEntrancePosition");

        public static bool Prefix(MineShaft __instance, ref Vector2 __result)
        {
            Vector2 customDest = __instance.get_MineShaftCustomDestination().Value;
            if (customDest != Vector2.Zero)
            {
                __result = customDest;
                return false;
            }

            return true;
        }
    }
}
