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
using CryptOfTheNecrodancerEnemies.Framework.Constants;
using CryptOfTheNecrodancerEnemies.Framework.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;

namespace CryptOfTheNecrodancerEnemies.Framework.Patches {

  internal static class CharacterPatch {

    internal class GetShadowOffsetPatch : ClassPatch {

      public override MethodInfo[] Original { get; } = { AccessTools.Method(typeof(Monster), nameof(Monster.GetBoundingBox)) };
      public override MethodInfo Postfix { get; } = AccessTools.Method(typeof(GetShadowOffsetPatch), nameof(GetShadowOffsetPatch.GetShadowOffsetPatch_Postfix));

      private static IReflectionHelper Reflection { get; set; }

      public static GetShadowOffsetPatch Instance { get; } = new GetShadowOffsetPatch();

      // Explicit static constructor to avoid the compiler to mark type as beforefieldinit
      static GetShadowOffsetPatch() { }

      private GetShadowOffsetPatch() { }

      public static GetShadowOffsetPatch CreatePatch(IReflectionHelper reflection) {
        Reflection = reflection;
        return Instance;
      }

      private static void GetShadowOffsetPatch_Postfix(Monster __instance, ref Rectangle __result) {
#if DEBUG
        ModEntry.ModMonitor.VerboseLog($"{MethodBase.GetCurrentMethod().Name} (enabled: {Instance.PostfixEnabled})");
#endif
        if (!Instance.PostfixEnabled) {
          return;
        }

        var assetName = __instance.Sprite.Texture?.Name;
        if (assetName != null && Sprites.Assets.TryGetValue(assetName, out SpriteAsset spriteAsset)) {
          __result = new Rectangle(__result.X - 8, __result.Y - 16, __result.Width, __result.Height);
        }
      }

    }

    internal class SpriteSetterPatch : ClassPatch {

      public override MethodInfo[] Original { get; } = { AccessTools.PropertySetter(typeof(Character), nameof(Character.Sprite)) };
      public override MethodInfo Postfix { get; } = AccessTools.Method(typeof(SpriteSetterPatch), nameof(SpriteSetterPatch.SpriteSetterPatch_Postfix));

      private static IReflectionHelper Reflection { get; set; }

      public static SpriteSetterPatch Instance { get; } = new SpriteSetterPatch();

      // Explicit static constructor to avoid the compiler to mark type as beforefieldinit
      static SpriteSetterPatch() { }

      private SpriteSetterPatch() { }

      public static SpriteSetterPatch CreatePatch(IReflectionHelper reflection) {
        Reflection = reflection;
        return Instance;
      }

      private static void SpriteSetterPatch_Postfix(Character __instance, ref AnimatedSprite value) {
#if DEBUG
        ModEntry.ModMonitor.VerboseLog($"{MethodBase.GetCurrentMethod().Name} (enabled: {Instance.PostfixEnabled})");
#endif
        if (!Instance.PostfixEnabled) {
          return;
        }

        var assetName = value?.Texture?.Name;
        if (__instance is Monster && assetName != null && Sprites.Assets.TryGetValue(assetName, out SpriteAsset spriteAsset)) {
          __instance.Scale = spriteAsset.Scale;
          value.SpriteWidth = spriteAsset.SourceRectangle.Width;
          value.SpriteHeight = spriteAsset.SourceRectangle.Height;
          value.UpdateSourceRect();
        }
      }

    }

    internal class SpriteGetterPatch : ClassPatch {

      public override MethodInfo[] Original { get; } = { AccessTools.PropertyGetter(typeof(Character), nameof(Character.Sprite)) };
      public override MethodInfo Prefix { get; } = AccessTools.Method(typeof(SpriteGetterPatch), nameof(SpriteGetterPatch.SpriteGetterPatch_Postfix));

      private static IReflectionHelper Reflection { get; set; }

      public static SpriteGetterPatch Instance { get; } = new SpriteGetterPatch();

      // Explicit static constructor to avoid the compiler to mark type as beforefieldinit.
      static SpriteGetterPatch() { }

      private SpriteGetterPatch() { }

      public static SpriteGetterPatch CreatePatch(IReflectionHelper reflection) {
        Reflection = reflection;
        return Instance;
      }

      private static void SpriteGetterPatch_Postfix(Monster __instance, ref AnimatedSprite __result) {
        if (!Instance.PostfixEnabled) {
          return;
        }

        var assetName = __result?.Texture?.Name;
        if (Sprites.Assets.TryGetFromNullableKey(assetName, out SpriteAsset spriteAsset)) {
          switch (__instance) {
            case BigSlime bigSlime:
              Instance.BigSlime(bigSlime);
              break;
            default:
              break;
          }

        }
      }

      private static bool UseCustomSprite(Monster character) {
        return character is Monster && (character as Monster).Health > 0;
      }

      private void BigSlime(BigSlime instance) {
        instance.flip = instance.FacingDirection == (int)CharacterDirection.Right;
      }

    }
  }

}
