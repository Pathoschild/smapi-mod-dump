using Harmony;
using NpcAdventure.Internal;
using NpcAdventure.StateMachine;
using StardewValley;
using static NpcAdventure.StateMachine.CompanionStateMachine;

namespace NpcAdventure.Patches
{
    internal class SpouseReturnHomePatch
    {
        private static readonly SetOnce<CompanionManager> manager = new SetOnce<CompanionManager>();
        private static CompanionManager Manager { get => manager.Value; set => manager.Value = value; }

        internal static bool Before_ReturnHomeFromFarmPosition(ref NPC __instance)
        {
            // Ignore home return when married spouse is recruited
            return !(Manager.PossibleCompanions.TryGetValue(__instance.Name, out CompanionStateMachine csm) && csm.CurrentStateFlag == StateFlag.RECRUITED);
        }

        internal static void Setup(HarmonyInstance harmony, CompanionManager manager)
        {
            Manager = manager;

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.returnHomeFromFarmPosition)),
                prefix: new HarmonyMethod(typeof(SpouseReturnHomePatch), nameof(SpouseReturnHomePatch.Before_ReturnHomeFromFarmPosition))
            );
        }
    }
}
