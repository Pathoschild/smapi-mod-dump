/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/

using System;
using System.Reflection;
using System.Reflection.Emit;
using CryptOfTheNecrodancerEnemies.Framework.Constants;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;

namespace CryptOfTheNecrodancerEnemies.Framework.Patches.Monsters {

  internal static class BatPatch {

    internal class ReloadSpritePatch : ClassPatch {

      public override MethodInfo[] Original { get; } = { AccessTools.Method(typeof(Bat), nameof(Bat.reloadSprite)) };
      public override MethodInfo Postfix { get; } = AccessTools.Method(typeof(ReloadSpritePatch), nameof(ReloadSpritePatch.ReloadSpritePatch_Postfix));

      private static IReflectionHelper Reflection { get; set; }

      public static ReloadSpritePatch Instance { get; } = new ReloadSpritePatch();

      // Explicit static constructor to avoid the compiler to mark type as beforefieldinit
      static ReloadSpritePatch() { }

      private ReloadSpritePatch() { }

      public static ReloadSpritePatch CreatePatch(IReflectionHelper reflection) {
        Reflection = reflection;
        return Instance;
      }

      private static void ReloadSpritePatch_Postfix(Bat __instance) {
#if DEBUG
        ModEntry.ModMonitor.VerboseLog($"{MethodBase.GetCurrentMethod().Name} (enabled: {Instance.PostfixEnabled})");
#endif
        if (!Instance.PostfixEnabled) {
          return;
        }

        var assetName = __instance.Sprite.Texture?.Name;
        if (assetName != null && Sprites.Assets.TryGetValue(assetName, out SpriteAsset spriteAsset)) {
          __instance.Sprite.LoadTexture(assetName);
        }
      }
    }

  }

}
