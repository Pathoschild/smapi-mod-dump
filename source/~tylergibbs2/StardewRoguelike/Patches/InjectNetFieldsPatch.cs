/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewRoguelike.VirtualProperties;
using StardewValley;
using StardewValley.Locations;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(MineShaft), "initNetFields")]
    internal class InjectMineShaftNetFieldsPatch
    {
        public static void Postfix(MineShaft __instance)
        {
            __instance.NetFields.AddFields(
                __instance.get_MineShaftLevel(),
                __instance.get_MineShaftChestFloor(),
                __instance.get_MineShaftForgeFloor(),
                __instance.get_MineShaftCustomDestination(),
                __instance.get_MineShaftEntryTime(),
                __instance.get_MineShaftChallengeFloor(),
                __instance.get_MineShaftIsChallengeFloor(),
                __instance.get_MineShaftDwarfGates(),
                __instance.get_MineShaftNetChests(),
                __instance.get_MineShaftLoadedMap()
            );
        }
    }

    [HarmonyPatch(typeof(Farmer), "farmerInit")]
    internal class InjectFarmerNetFieldsPatch
    {
        public static void Postfix(Farmer __instance)
        {
            __instance.NetFields.AddFields(
                __instance.get_FarmerIsSpectating(),
                __instance.get_FarmerWasDamagedOnThisLevel(),
                __instance.get_FarmerCurrentLevel(),
                __instance.get_FarmerCurses()
            );
        }
    }

    [HarmonyPatch(typeof(DwarfGate), "InitNetFields")]
    internal class InjectDwarfGateNetFieldsPatch
    {
        public static void Postfix(DwarfGate __instance)
        {
            __instance.NetFields.AddFields(
                __instance.get_DwarfGateDisabled()
            );
        }
    }

    [HarmonyPatch(typeof(GameLocation), "initNetFields")]
    internal class InjectGameLocationNetFields
    {
        public static void Postfix(GameLocation __instance)
        {
            __instance.NetFields.AddFields(
                __instance.get_DebuffPlayerEvent()
            );
        }
    }

    [HarmonyPatch(typeof(FarmerTeam), MethodType.Constructor)]
    internal class InjectFarmerTeamNetFields
    {
        public static void Postfix(FarmerTeam __instance)
        {
            __instance.NetFields.AddFields(
                __instance.get_FarmerTeamHardMode()
            );
        }
    }
}
