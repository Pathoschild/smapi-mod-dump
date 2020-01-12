using Harmony;
using Microsoft.Xna.Framework.Graphics;
using NpcAdventure.Events;
using NpcAdventure.StateMachine;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Patches
{
    internal class CompanionSayHiPatch
    {
        private static CompanionManager manager;

        internal static bool Prefix(ref NPC __instance, Character c)
        {
            if (manager != null && manager.PossibleCompanions.TryGetValue(__instance.Name, out CompanionStateMachine csm))
            {
                // Avoid say hi to monsters while companion is recruited
                return !(csm.CurrentStateFlag == CompanionStateMachine.StateFlag.RECRUITED && c is Monster);
            }

            return true;
        }

        internal static void Setup(HarmonyInstance harmony, CompanionManager manager)
        {
            CompanionSayHiPatch.manager = manager;

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.sayHiTo)),
                prefix: new HarmonyMethod(typeof(CompanionSayHiPatch), nameof(CompanionSayHiPatch.Prefix))
            );
        }
    }
}
