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
        internal static bool setup()
        {
            if (!ModEntry.helper.ModRegistry.IsLoaded("Digus.ProducerFrameworkMod"))
                return false;

            ModEntry.monitor.Log(ModEntry.helper.Translation.Get("general.pfmWarning"), LogLevel.Warn);
            var target = AccessTools.TypeByName("ProducerFrameworkMod.Controllers.ProducerController").MethodNamed("ValidateConfigProducerName");
            ModEntry.harmony.Patch(target, postfix: new(typeof(PFMPatch),"Postfix"));

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
