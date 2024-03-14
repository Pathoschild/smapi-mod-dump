/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace ForageFantasy
{
    using HarmonyLib;
    using StardewValley;
    using System;

    internal class Patcher
    {
        private static ForageFantasy mod;

        public static void PatchAll(ForageFantasy forageFantasy)
        {
            mod = forageFantasy;

            var harmony = new Harmony(mod.ModManifest.UniqueID);

            try
            {
                harmony.Patch(
                   original: AccessTools.Method(typeof(Crop), nameof(Crop.getRandomWildCropForSeason)),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(PatchSummerWildSeedResult)));

                MushroomTapperCalendar.ApplyPatches(forageFantasy.Config, harmony);

                QualityAndXPPatches.ApplyPatches(forageFantasy.Config, harmony);
            }
            catch (Exception e)
            {
                mod.ErrorLog("Error while trying to setup required patches:", e);
            }

            if (mod.Helper.ModRegistry.IsLoaded("Pathoschild.Automate"))
            {
                AutomateCompatibility.ApplyPatches(forageFantasy, harmony);
            }

            if (mod.Helper.ModRegistry.IsLoaded("BitwiseJonMods.OneClickShedReloader"))
            {
                OneClickShedReloaderCompatibility.ApplyPatches(forageFantasy, harmony);
            }
        }

        public static void PatchSummerWildSeedResult(ref string __result, ref Season season)
        {
            if (mod.Config.CommonFiddleheadFern && season == Season.Summer)
            {
                __result = FernAndBurgerLogic.GetWildSeedSummerForage();
            }
        }
    }
}