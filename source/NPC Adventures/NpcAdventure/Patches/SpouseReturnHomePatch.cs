using Harmony;
using Microsoft.Xna.Framework.Graphics;
using NpcAdventure.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Patches
{
    internal class SpouseReturnHomePatch
    {
        internal static List<string> recruitedSpouses;

        internal static bool Prefix(ref NPC __instance)
        {
            // Ignore home return when spouse is recruited
            if (recruitedSpouses.IndexOf(__instance.Name) >= 0)
                return false;

            return true;
        }

        internal static void Setup(HarmonyInstance harmony)
        {
            recruitedSpouses = new List<string>();

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.returnHomeFromFarmPosition)),
                prefix: new HarmonyMethod(typeof(SpouseReturnHomePatch), nameof(SpouseReturnHomePatch.Prefix))
            );
        }
    }
}
