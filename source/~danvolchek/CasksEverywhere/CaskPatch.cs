/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Harmony;
using StardewValley.Objects;
using System.Reflection;

namespace CasksEverywhere
{
    [HarmonyPatch]
    internal class CaskPatch
    {
        private static MethodBase TargetMethod()
        {
            return typeof(Cask).GetMethod(nameof(Cask.IsValidCaskLocation));
        }

        private static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}
