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
using CryptOfTheNecrodancerEnemies.Framework.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;

namespace CryptOfTheNecrodancerEnemies.Framework.Patches {

  internal static class MonsterPatch {

    internal class ParseMonsterInfoPatch : ClassPatch {

      public override MethodInfo[] Original { get; } = { AccessTools.Method(typeof(Monster), "parseMonsterInfo", new[] { typeof(string) }) };
      public override MethodInfo Postfix { get; } = AccessTools.Method(typeof(ParseMonsterInfoPatch), nameof(ParseMonsterInfoPatch.ParseMonsterInfoPatch_Postfix));// AccessTools.Method(typeof(ParseMonsterInfoPatch), nameof(ParseMonsterInfoPatch.ParseMonsterInfoPatch_Postfix));

      private static IReflectionHelper Reflection { get; set; }

      public static ParseMonsterInfoPatch Instance { get; } = new ParseMonsterInfoPatch();

      // Explicit static constructor to avoid the compiler to mark type as beforefieldinit
      static ParseMonsterInfoPatch() { }

      private ParseMonsterInfoPatch() { }

      public static ParseMonsterInfoPatch CreatePatch(IReflectionHelper reflection) {
        Reflection = reflection;
        return Instance;
      }

      private static void ParseMonsterInfoPatch_Postfix(Monster __instance, string name) {
#if DEBUG
        ModEntry.ModMonitor.VerboseLog($"{MethodBase.GetCurrentMethod().Name} (enabled: {Instance.PostfixEnabled})");
#endif
        if (!Instance.PostfixEnabled) {
          return;
        }

        PrepareCustomSprite(__instance);

        //var assetName = __instance.Sprite.Texture?.Name;
        //if (assetName != null && Sprites.Assets.TryGetValue(assetName, out SpriteAsset spriteAsset)) {
        //  __instance.Scale = spriteAsset.Scale;
        //}
      }
    }

    internal class ReloadSpritePatch : ClassPatch {

      public override MethodInfo[] Original { get; } = {
        OriginalReloadSprite<Bat>(),
        OriginalReloadSprite<Leaper>(),
      };

      public override MethodInfo Postfix { get; } = AccessTools.Method(typeof(ReloadSpritePatch), nameof(ReloadSpritePatch.ReloadSprite_Postfix));

      private static IReflectionHelper Reflection { get; set; }

      public static ReloadSpritePatch Instance { get; } = new ReloadSpritePatch();

      // Explicit static constructor to avoid the compiler to mark type as beforefieldinit
      static ReloadSpritePatch() { }

      private ReloadSpritePatch() { }

      public static ReloadSpritePatch CreatePatch(IReflectionHelper reflection) {
        Reflection = reflection;
        return Instance;
      }

      private static MethodInfo OriginalReloadSprite<T>() where T : Monster {
        return AccessTools.Method(typeof(T), nameof(Monster.reloadSprite));
      }

      private static void ReloadSprite_Postfix(Monster __instance) {
#if DEBUG
        ModEntry.ModMonitor.VerboseLog($"{MethodBase.GetCurrentMethod().Name} (enabled: {Instance.PostfixEnabled})");
#endif
        if (!Instance.PostfixEnabled) {
          return;
        }

        PrepareCustomSprite(__instance);
      }
    }

    internal static void PrepareCustomSprite(Monster instance) {
      var assetName = instance.Sprite.Texture?.Name;
      if (Sprites.Assets.TryGetFromNullableKey(assetName, out SpriteAsset spriteAsset) && spriteAsset.ShouldResize) {
        //instance.CreateCustomSprite();
        //instance.Sprite = instance.Sprite;

        instance.Scale = spriteAsset.Scale;
        instance.Sprite.SpriteWidth = spriteAsset.SourceRectangle.Width;
        instance.Sprite.SpriteHeight = spriteAsset.SourceRectangle.Height;
        instance.Sprite.UpdateSourceRect();
      }
    }
  }

}
