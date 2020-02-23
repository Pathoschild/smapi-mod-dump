using Harmony;
using NpcAdventure.Compatibility;
using NpcAdventure.Internal;
using StardewValley;
using System;
using static NpcAdventure.StateMachine.CompanionStateMachine;

namespace NpcAdventure.Patches
{
    internal class NpcCheckActionPatch
    {
        private static readonly SetOnce<CompanionManager> manager = new SetOnce<CompanionManager>();
        private static CompanionManager Manager { get => manager.Value; set => manager.Value = value; }

        internal static void Before_checkAction(NPC __instance, ref bool __state, Farmer who)
        {
            bool canKiss = (bool)TPMC.Instance?.CustomKissing.CanKissNpc(who, __instance) && (bool)TPMC.Instance?.CustomKissing.HasRequiredFriendshipToKiss(who, __instance);
            bool recruited = Manager.PossibleCompanions.TryGetValue(__instance.Name, out var csm) && csm.CurrentStateFlag == StateFlag.RECRUITED;

            // Save has been kissed flag to state for use in postfix (we want to know previous has kissed state before kiss)
            // Mark as kissed when we can't kiss them (cover angry emote when try to kiss - vanilla and custom kissing mod)
            __state = __instance.hasBeenKissedToday || !canKiss;
        }

        [HarmonyPriority(-1000)]
        [HarmonyAfter("Digus.CustomKissingMod")]
        internal static void After_checkAction(ref NPC __instance, ref bool __result, bool __state, Farmer who, GameLocation l)
        {
            if (__instance.CurrentDialogue.Count > 0)
                return;

            // Check our action when vanilla check action don't triggered or spouse was kissed today and farmer try to kiss again
            if (!__result || (__result && __state && who.FarmerSprite.CurrentFrame == 101))
            {
                __result = Manager.CheckAction(who, __instance, l);

                // Cancel kiss when spouse was kissed today and we did our action
                if (__result && who.FarmerSprite.CurrentFrame == 101)
                {
                    who.completelyStopAnimatingOrDoingAction();
                    __instance.IsEmoting = false;
                    __instance.Halt();
                    __instance.facePlayer(who);
                }
            }
        }

        internal static void Setup(HarmonyInstance harmony, CompanionManager manager)
        {
            Manager = manager;

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
                prefix: new HarmonyMethod(typeof(NpcCheckActionPatch), nameof(NpcCheckActionPatch.Before_checkAction)),
                postfix: new HarmonyMethod(typeof(NpcCheckActionPatch), nameof(NpcCheckActionPatch.After_checkAction))
            );
        }
    }
}
