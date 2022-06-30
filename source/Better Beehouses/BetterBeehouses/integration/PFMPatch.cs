/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/BetterBeehouses
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using System.Collections.Generic;

namespace BetterBeehouses.integration
{
    class PFMPatch
    {
        private static bool isPatched = false;
        private static readonly string[] patchesToPatch = {"performDropDownAction", "DayUpdate", "checkForActionPrefix", "minutesElapsedPrefix"};
        internal static bool Setup()
        {
            if (!ModEntry.helper.ModRegistry.IsLoaded("Digus.ProducerFrameworkMod"))
                return false;

            ModEntry.monitor.Log("PFM Integration " + (isPatched ? "Disabling" : "Enabling"));
            var type = AccessTools.TypeByName("ProducerFrameworkMod.ObjectOverrides");

            if (!isPatched && ModEntry.config.PatchPFM)
            {
                for(int i = 0; i < patchesToPatch.Length; i++)
                    ModEntry.harmony.Patch(type.MethodNamed(patchesToPatch[i]), prefix: new(typeof(PFMPatch), nameof(Prefix)));
                isPatched = true;
            } else
            {
                for(int i = 0; i < patchesToPatch.Length; i++)
                    ModEntry.harmony.Unpatch(type.MethodNamed(patchesToPatch[i]), HarmonyPatchType.Prefix, ModEntry.ModID);
                isPatched = false;
            }

            return true;
        }
        private static bool Prefix(StardewValley.Object __0, ref bool __result)
            => !(__result = __0.Name.Normalize() == "beehouse");
    }
}
