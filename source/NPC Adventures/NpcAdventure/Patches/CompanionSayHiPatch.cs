using Harmony;
using Microsoft.Xna.Framework.Graphics;
using NpcAdventure.Events;
using NpcAdventure.Internal;
using NpcAdventure.StateMachine;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NpcAdventure.StateMachine.CompanionStateMachine;

namespace NpcAdventure.Patches
{
    internal class CompanionSayHiPatch
    {
        private static readonly SetOnce<CompanionManager> manager = new SetOnce<CompanionManager>();
        private static CompanionManager Manager { get => manager.Value; set => manager.Value = value; }

        internal static bool Before_sayHiTo(ref NPC __instance, Character c)
        {
            if (Manager != null && Manager.PossibleCompanions.TryGetValue(__instance.Name, out CompanionStateMachine csm))
            {
                // Avoid say hi to monsters while companion is recruited
                return !(csm.CurrentStateFlag == StateFlag.RECRUITED && c is Monster);
            }

            return true;
        }

        internal static void Setup(HarmonyInstance harmony, CompanionManager manager)
        {
            Manager = manager;

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.sayHiTo)),
                prefix: new HarmonyMethod(typeof(CompanionSayHiPatch), nameof(CompanionSayHiPatch.Before_sayHiTo))
            );
        }
    }
}
