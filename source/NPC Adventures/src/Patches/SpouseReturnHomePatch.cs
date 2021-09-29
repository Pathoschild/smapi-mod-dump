/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using HarmonyLib;
using NpcAdventure.StateMachine;
using PurrplingCore.Patching;
using StardewValley;
using System;
using static NpcAdventure.StateMachine.CompanionStateMachine;

namespace NpcAdventure.Patches
{
    internal class SpouseReturnHomePatch : Patch<SpouseReturnHomePatch>
    {
        private CompanionManager Manager { get; set; }
        public override string Name => nameof(SpouseReturnHomePatch);

        public SpouseReturnHomePatch(CompanionManager manager)
        {
            this.Manager = manager;
            Instance = this;
        }

        private static bool Before_ReturnHomeFromFarmPosition(ref NPC __instance)
        {
            try
            {
                // Ignore home return when married spouse is recruited
                return !(Instance.Manager.PossibleCompanions.TryGetValue(__instance.Name, out CompanionStateMachine csm) && csm.CurrentStateFlag == StateFlag.RECRUITED);
            } catch (Exception ex)
            {
                Instance.LogFailure(ex, nameof(Before_ReturnHomeFromFarmPosition));
                return true;
            }
        }

        protected override void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.returnHomeFromFarmPosition)),
                prefix: new HarmonyMethod(typeof(SpouseReturnHomePatch), nameof(SpouseReturnHomePatch.Before_ReturnHomeFromFarmPosition))
            );
        }
    }
}
