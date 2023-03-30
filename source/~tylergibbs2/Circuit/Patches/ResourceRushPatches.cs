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
using Netcode;
using StardewValley.Locations;

namespace Circuit.Patches
{
    internal class ResourceRushPatches
    {
        [HarmonyPatch(typeof(MineShaft), "adjustLevelChances")]
        public class MineShaftAdjustLevelChancesPatch
        {
            public static void Postfix(ref double gemStoneChance)
            {
                if (!ModEntry.ShouldPatch())
                    return;

                if (EventManager.EventIsActive(EventType.ResourceRush))
                    gemStoneChance *= 3;
            }
        }

        [HarmonyPatch(typeof(MineShaft), "populateLevel")]
        public class MineShaftPopulateLevelPatch
        {
            public static void Postfix(MineShaft __instance)
            {
                if (!ModEntry.ShouldPatch(EventType.ResourceRush))
                    return;

                // vanilla conditions
                bool dinoArea = (bool)__instance.GetType().GetProperty("isDinoArea", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(__instance)!;
                NetBool treasureRoom = (NetBool)__instance.GetType().GetField("netIsTreasureRoom", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(__instance)!;
                if ((!__instance.mustKillAllMonstersToAdvance() || dinoArea) && __instance.mineLevel % 5 != 0 && __instance.mineLevel > 2 && __instance.mineLevel != 220 && !treasureRoom.Value)
                    __instance.tryToAddOreClumps();
            }
        }
    }
}
