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
    /// <summary>Lets the farmer move while aiming if that is enabled.</summary>
    [HarmonyPatch]
    internal class SlingshotBeginUsingPatch
    {
        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Method names are defined by Harmony.")]
        private static bool Prepare()
        {
            return BetterSlingshotsMod.Instance.Config.CanMoveWhileFiring;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Method names are defined by Harmony.")]
        private static MethodBase TargetMethod()
        {
            return typeof(Slingshot).GetMethod(nameof(Slingshot.beginUsing));
        }

        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Argument names are defined by Harmony.")]
        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Method names are defined by Harmony.")]
        private static void Postfix(Slingshot __instance)
        {
            __instance.getLastFarmerToUse().CanMove = true;
        }
    }
}
