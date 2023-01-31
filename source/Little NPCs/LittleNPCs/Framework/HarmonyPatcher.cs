/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/LittleNPCs
**
*************************************************/

using HarmonyLib;

using StardewValley;
using StardewValley.Characters;


namespace LittleNPCs.Framework {
    internal static class HarmonyPatcher {
        public static void Create(ModEntry modEntry) {
            // Create Harmony instance.
            Harmony harmony = new Harmony(modEntry.ModManifest.UniqueID);
            // NPC.arriveAtFarmHouse patch (postfix).
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.arriveAtFarmHouse)),
                postfix:  new HarmonyMethod(typeof(Patches.NPCArriveAtFarmHousePatch), nameof(Patches.NPCArriveAtFarmHousePatch.Postfix))
            );
            // NPC.checkSchedule patch (prefix).
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.checkSchedule)),
                prefix:   new HarmonyMethod(typeof(Patches.NPCCheckSchedulePatch), nameof(Patches.NPCCheckSchedulePatch.Prefix))
            );
            // NPC.parseMasterSchedule patch (prefix, postfix, finalizer).
            harmony.Patch(
                original:  AccessTools.Method(typeof(NPC), nameof(NPC.parseMasterSchedule)),
                prefix:    new HarmonyMethod(typeof(Patches.NPCParseMasterSchedulePatch), nameof(Patches.NPCParseMasterSchedulePatch.Prefix)),
                postfix:   new HarmonyMethod(typeof(Patches.NPCParseMasterSchedulePatch), nameof(Patches.NPCParseMasterSchedulePatch.Postfix)),
                finalizer: new HarmonyMethod(typeof(Patches.NPCParseMasterSchedulePatch), nameof(Patches.NPCParseMasterSchedulePatch.Finalizer))
            );
            // NPC.prepareToDisembarkOnNewSchedulePath patch (postfix).
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), "prepareToDisembarkOnNewSchedulePath"),
                postfix:  new HarmonyMethod(typeof(Patches.NPCPrepareToDisembarkOnNewSchedulePathPatch), nameof(Patches.NPCPrepareToDisembarkOnNewSchedulePathPatch.Postfix))
            );
            // PathFindController.handleWarps patch (prefix).
            harmony.Patch(
                original: AccessTools.Method(typeof(PathFindController), nameof(PathFindController.handleWarps)),
                prefix:   new HarmonyMethod(typeof(Patches.PFCHandleWarpsPatch), nameof(Patches.PFCHandleWarpsPatch.Prefix))
            );
            // Dialogue.checkForSpecialCharacters patch (prefix).
            harmony.Patch(
                original: AccessTools.Method(typeof(Dialogue), nameof(Dialogue.checkForSpecialCharacters)),
                prefix:   new HarmonyMethod(typeof(Patches.DialogueCheckForSpecialCharactersPatch), nameof(Patches.DialogueCheckForSpecialCharactersPatch.Prefix))
            );
            // Child.GetChildIndex patch (prefix).
            harmony.Patch(
                original: AccessTools.Method(typeof(Child), nameof(Child.GetChildIndex)),
                prefix:   new HarmonyMethod(typeof(Patches.ChildGetChildIndexPatch), nameof(Patches.ChildGetChildIndexPatch.Prefix))
            );
        }
    }
}