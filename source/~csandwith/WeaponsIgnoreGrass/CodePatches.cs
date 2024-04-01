/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace WeaponsIgnoreGrass
{
    public partial class ModEntry 
    { 
        [HarmonyPatch(typeof(Grass), nameof(Grass.performToolAction))]
        public class Grass_performToolAction_Patch
        {
            public static bool Prefix(Tool t, int explosion)
            {
                return (!Config.EnableMod || explosion > 0 || t is not MeleeWeapon || (t as MeleeWeapon).isScythe(t.ParentSheetIndex));
            }
        }
    }
}