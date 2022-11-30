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

namespace BattleRoyale.Patches
{
    class SewerFix : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(Sewer), "MakeMapModifications");

        public static bool Prefix(Sewer __instance)
        {
            __instance.setMapTileIndex(31, 17, -1, "Buildings");
            __instance.setMapTileIndex(31, 16, -1, "Front");
            return false;
        }
    }
}
