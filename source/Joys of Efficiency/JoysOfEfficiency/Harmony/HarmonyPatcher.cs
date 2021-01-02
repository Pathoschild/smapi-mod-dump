/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

using Harmony;

namespace JoysOfEfficiency.Harmony
{
    internal class HarmonyPatcher
    {
        private static readonly HarmonyInstance Harmony = HarmonyInstance.Create("com.pome.joe");

        public static void DoPatching()
        {
            Harmony.PatchAll();
        }

    }
}
