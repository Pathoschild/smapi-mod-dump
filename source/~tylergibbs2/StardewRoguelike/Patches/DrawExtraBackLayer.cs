/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewValley;
using xTile.Dimensions;

namespace StardewRoguelike.Patches
{
    class DrawExtraBackLayer : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(GameLocation), "drawBackground");

        public static bool Prefix(GameLocation __instance)
        {
            __instance.Map.GetLayer("BackBackBack")?.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, wrapAround: false, 4);
            __instance.Map.GetLayer("BackBack")?.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, wrapAround: false, 4);
            return true;
        }
    }
}