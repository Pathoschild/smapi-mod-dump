/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Circuit.VirtualProperties;
using HarmonyLib;
using StardewValley;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(FarmerTeam), MethodType.Constructor)]
    internal class InjectFarmerTeamNetFields
    {
        public static void Postfix(FarmerTeam __instance)
        {
            __instance.NetFields.AddFields(
                __instance.get_FarmerTeamCurrentEvent()
            );
        }
    }
}
