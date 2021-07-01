/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using Harmony;
using StardewValley;
using StardewValley.Buildings;

namespace BuildableLocationsFramework.Patches
{
    [HarmonyPatch( typeof( Building ), nameof( Building.updateInteriorWarps ) )]
    public static class BuildingUpdateInteriorWarpsPatch
    {
        public static void Postfix( Building __instance, GameLocation interior )
        {
            var targetName = Mod.findOutdoorsOf( __instance )?.Name;
            if ( targetName == null )
                return;

            if ( interior == null )
                interior = __instance.indoors.Value;
            if ( interior == null )
                return;
            foreach ( Warp warp in interior.warps )
            {
                warp.TargetName = targetName;
            }
        }
    }
}
