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

namespace BetterBeehouses.integration
{
    class PFMPatch
    {
        private static bool isPatched = false;
        internal static bool Setup()
        {
            if (!ModEntry.helper.ModRegistry.IsLoaded("Digus.ProducerFrameworkMod"))
                return false;

            var target = AccessTools.TypeByName("ProducerFrameworkMod.Controllers.ProducerController").MethodNamed("ValidateConfigProducerName");

            if (!isPatched && ModEntry.config.PatchPFM)
            {
                isPatched = false;
                ModEntry.harmony.Patch(target, postfix: new(typeof(PFMPatch), "Postfix"));
                isPatched = true;
            } else if(isPatched && !ModEntry.config.PatchPFM){
                ModEntry.harmony.Unpatch(target, HarmonyPatchType.Postfix, ModEntry.ModID);
                isPatched = false;
            }

            return true;
        }
        private static void Postfix(string producerName, ref bool __result)
        {
            if(producerName == "Bee House")
            {
                ModEntry.monitor.Log(ModEntry.helper.Translation.Get("general.removedPfmBeehouse"), LogLevel.Info);
                __result = false;
            }
        }
    }
}
