/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using Harmony;
using Pathoschild.Stardew.Automate;
using PFMAutomate.Automate;
using ProducerFrameworkMod.Controllers;
using Object = StardewValley.Object;

namespace PFMAutomate
{
    public class CCRMAutomateOverrides
    {
        [HarmonyPriority(800)]
        public static void GetFor(ref object __result, Object obj)
        {
            string machineFullName = __result?.GetType().FullName;
            if (machineFullName == "CCRMAutomate.Automate.ClonerMachine" && (ProducerController.HasProducerRule(obj.Name) || ProducerController.GetProducerConfig(obj.Name) != null))
            {
                __result = new CustomClonerMachine((IMachine)__result);
            }
        }
    }
}
