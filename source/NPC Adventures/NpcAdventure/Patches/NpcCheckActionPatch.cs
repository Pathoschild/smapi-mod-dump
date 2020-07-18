using Harmony;
using NpcAdventure.Compatibility;
using NpcAdventure.Model;
using StardewModdingAPI;
using StardewValley;
using System;

namespace NpcAdventure.Patches
{
    internal class NpcCheckActionPatch : Patch<NpcCheckActionPatch>
    {
        private CompanionManager Manager { get; set; }
        private IInputHelper Input { get; }
        private Config Config { get; }

        public override string Name => nameof(NpcCheckActionPatch);

        public NpcCheckActionPatch(CompanionManager manager, IInputHelper input, Config config)
        {
            this.Manager = manager;
            this.Input = input;
            this.Config = config;
            Instance = this;
        }

        private static bool Before_checkAction(NPC __instance, ref bool __result, ref bool __state, Farmer who, GameLocation l)
        {
            try
            {
                if (Instance.Config.RequestsWithShift)
                {
                    // If requests (ask to follow and etc) enabled, process CSM check action only in this way
                    // Shift button must be hold for ask to follow, show companion menu dialog and etc
                    if (Instance.Input.IsDown(Instance.Config.RequestsShiftButton) && Instance.Manager.CheckAction(who, __instance, l))
                    {
                        __result = true;
                        return false;
                    }

                    return true;
                }

                bool canKiss = (bool)TPMC.Instance?.CustomKissing.CanKissNpc(who, __instance) && (bool)TPMC.Instance?.CustomKissing.HasRequiredFriendshipToKiss(who, __instance);

                // Save has been kissed flag to state for use in postfix (we want to know previous has kissed state before kiss)
                // Mark as kissed when we can't kiss them (cover angry emote when try to kiss - vanilla and custom kissing mod)
                __state = __instance.hasBeenKissedToday.Value || !canKiss;

                return true;
            } catch (Exception ex)
            {
                Instance.LogFailure(ex, nameof(Before_checkAction));
                __state = false; // Fallback: Mark NPC as NOT kissed today for after patch

                return true;
            }
        }

        [HarmonyPriority(-1000)]
        [HarmonyAfter("Digus.CustomKissingMod")]
        private static void After_checkAction(ref NPC __instance, ref bool __result, bool __state, Farmer who, GameLocation l)
        {
            try
            {
                if (__instance.CurrentDialogue.Count > 0 || Instance.Config.RequestsWithShift)
                    return;

                // Check our action when vanilla check action don't triggered or spouse was kissed today and farmer try to kiss again
                if (!__result || (__result && __state && who.FarmerSprite.CurrentFrame == 101))
                {
                    __result = Instance.Manager.CheckAction(who, __instance, l);

                    // Cancel kiss when spouse was kissed today and we did our action
                    if (__result && who.FarmerSprite.CurrentFrame == 101)
                    {
                        who.completelyStopAnimatingOrDoingAction();
                        __instance.IsEmoting = false;
                        __instance.Halt();
                        __instance.facePlayer(who);
                    }
                }
            } catch (Exception ex)
            {
                Instance.LogFailure(ex, nameof(After_checkAction));
            }
        }

        protected override void Apply(HarmonyInstance harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
                prefix: new HarmonyMethod(typeof(NpcCheckActionPatch), nameof(NpcCheckActionPatch.Before_checkAction)),
                postfix: new HarmonyMethod(typeof(NpcCheckActionPatch), nameof(NpcCheckActionPatch.After_checkAction))
            );
        }
    }
}
