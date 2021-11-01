/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
using xTile.Dimensions;

namespace BattleRoyale.Patches
{
    class DrawExtraBackLayer : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(GameLocation), "drawBackground");

        public static bool Prefix(GameLocation __instance)
        {
            if (!(__instance is Mountain))
                return true;

            __instance.Map.GetLayer("BackBack").Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, wrapAround: false, 4);
            return true;
        }
    }
}
