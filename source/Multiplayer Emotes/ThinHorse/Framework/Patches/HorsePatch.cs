/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/

using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.Characters;

namespace ThinHorse.Framework.Patches {

  internal static class HorsePatch {

    internal class GetBoundingBoxPatch : ClassPatch {

      public override MethodInfo Original { get; } = AccessTools.Method(typeof(Horse), nameof(Horse.GetBoundingBox));
      public override MethodInfo Prefix { get; } = AccessTools.Method(typeof(GetBoundingBoxPatch), nameof(GetBoundingBoxPatch.GetBoundingBoxPatch_Prefix));
      public override MethodInfo Postfix { get; } = AccessTools.Method(typeof(GetBoundingBoxPatch), nameof(GetBoundingBoxPatch.GetBoundingBoxPatch_Postfix));

      private static IReflectionHelper Reflection { get; set; }

      public static GetBoundingBoxPatch Instance { get; } = new GetBoundingBoxPatch();

      // Explicit static constructor to avoid the compiler to mark type as beforefieldinit.
      static GetBoundingBoxPatch() { }

      private GetBoundingBoxPatch() { }


      public static GetBoundingBoxPatch CreatePatch(IReflectionHelper reflection) {
        Reflection = reflection;
        return Instance;
      }

      private static void GetBoundingBoxPatch_Prefix(Horse __instance, ref bool ___squeezingThroughGate) {
        if (!Instance.PrefixEnabled) {
          return;
        }

        ___squeezingThroughGate = false;
      }

      private static void GetBoundingBoxPatch_Postfix(Horse __instance, ref Rectangle __result) {
        if (!Instance.PostfixEnabled) {
          return;
        }

        if (__instance.rider != null) {
          __result = new Rectangle(
            x: __result.X + 16,
            y: __result.Y,
            width: __result.Width / 2,
            height: __result.Height
          );
        }
      }

    }

  }

}
