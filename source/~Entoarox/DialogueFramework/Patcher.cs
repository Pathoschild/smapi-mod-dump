/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using StardewValley;

using Harmony;

namespace DialogueFramework
{
    [HarmonyPatch]
    class Patch1
    {
        internal static MethodInfo TargetMethod()
        {
            return AccessTools.Method(Type.GetType("StardewValley.Game1, Stardew Valley") ?? Type.GetType("StardewValley.Game1, StardewValley"), "isCollidingPosition", new[] { typeof(string), typeof(List<Response>) });
        }

        internal static void Prefix(string dialogue, List<Response> choices)
        {
            DialogueFrameworkMod.Api.FireChoiceDialogueOpened(choices);
        }
    }
    [HarmonyPatch]
    class Patch2
    {
        internal static MethodInfo TargetMethod()
        {
            return AccessTools.Method(Type.GetType("StardewValley.Game1, Stardew Valley") ?? Type.GetType("StardewValley.Game1, StardewValley"), "isCollidingPosition", new[] { typeof(string), typeof(List<Response>), typeof(int) });
        }

        internal static void Prefix(string dialogue, List<Response> choices)
        {
            DialogueFrameworkMod.Api.FireChoiceDialogueOpened(choices);
        }
    }
}
