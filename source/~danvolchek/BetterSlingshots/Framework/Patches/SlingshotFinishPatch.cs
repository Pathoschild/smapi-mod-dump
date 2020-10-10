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
using StardewValley.Tools;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace BetterSlingshots.Framework.Patches
{
    /// <summary>Allows for automatic firing by disabling the finish fire event.</summary>
    [HarmonyPatch]
    internal class SlingshotFinishPatch
    {
        private static bool shouldRun = true;
        private static Slingshot instanceToControl;

        public static void ShouldRun(Slingshot instance, bool shouldRun)
        {
            SlingshotFinishPatch.shouldRun = shouldRun;
            SlingshotFinishPatch.instanceToControl = instance;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Method names are defined by Harmony.")]
        private static MethodBase TargetMethod()
        {
            return typeof(Slingshot).GetMethod("doFinish", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Argument names are defined by Harmony.")]
        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Method names are defined by Harmony.")]
        private static bool Prefix(Slingshot __instance)
        {
            if (__instance != SlingshotFinishPatch.instanceToControl || SlingshotFinishPatch.instanceToControl == null)
                return true;

            return SlingshotFinishPatch.shouldRun;
        }
    }
}
