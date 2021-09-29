/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using HarmonyLib;

using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>
        /// Applies any Harmony patches used by this mod.
        /// </summary>
        private void ApplyHarmonyPatches()
        {
            var harmony = new Harmony(this.ModManifest.UniqueID); //create this mod's Harmony instance

            //apply all patches
            HarmonyPatch_AddSpawnedMineralsToCollections.ApplyPatch(harmony);
            HarmonyPatch_UpdateCursorOverPlacedItem.ApplyPatch(harmony);
            HarmonyPatch_OptimizeMonsterCode.ApplyPatch(harmony);
        }
    }
}
