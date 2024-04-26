/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods
**
*************************************************/

using StardewValley.Menus;

namespace BrighterBuildingPaint; 

internal partial class Mod {
    
    public class BuildingPaintMenu_BuildingColorSlider_Patch {
        public static void Postfix(BuildingPaintMenu.BuildingColorSlider __instance) {
            if (!Config.Enabled) return;

            if (__instance.handle.myID == 107) {
                __instance.max = Config.MaxSaturation;
            }
            
            if (__instance.handle.myID == 108) {
                __instance.max = Config.MaxBrightness;
                __instance.min = Config.MinBrightness;
            }
        }
    }
}