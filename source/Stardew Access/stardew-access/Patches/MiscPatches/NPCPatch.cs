/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using HarmonyLib;
using StardewValley;

namespace stardew_access.Patches
{
    internal class NPCPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.drawAboveAlwaysFrontLayer)),
                postfix: new HarmonyMethod(typeof(NPCPatch), nameof(NPCPatch.DrawAboveAlwaysFrontLayerPatch))
            );
        }

        private static void DrawAboveAlwaysFrontLayerPatch(NPC __instance, string ___textAboveHead, int ___textAboveHeadTimer)
        {
            try
            {
                if (___textAboveHeadTimer > 2900 && ___textAboveHead != null)
                {
                    MainClass.ScreenReader.SayWithChecker($"{__instance.displayName} says {___textAboveHead}", true);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error in patch:NPCShowTextAboveHeadPatch \n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
