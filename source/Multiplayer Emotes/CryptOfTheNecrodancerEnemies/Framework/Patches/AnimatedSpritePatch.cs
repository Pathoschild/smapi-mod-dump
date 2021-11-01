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
using CryptOfTheNecrodancerEnemies.Framework.Constants;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace CryptOfTheNecrodancerEnemies.Framework.Patches {

  internal static class AnimatedSpritePatch {

    internal class LoadTexturePatch : ClassPatch {

      public override MethodInfo[] Original { get; } = { AccessTools.Method(typeof(AnimatedSprite), nameof(AnimatedSprite.LoadTexture), new[] { typeof(string) }) };
      public override MethodInfo Postfix { get; } = AccessTools.Method(typeof(LoadTexturePatch), nameof(LoadTexturePatch.LoadTexture_Postfix));

      private static IReflectionHelper Reflection { get; set; }

      public static LoadTexturePatch Instance { get; } = new LoadTexturePatch();

      // Explicit static constructor to avoid the compiler to mark type as beforefieldinit
      static LoadTexturePatch() { }

      private LoadTexturePatch() { }

      public static LoadTexturePatch CreatePatch(IReflectionHelper reflection) {
        Reflection = reflection;
        return Instance;
      }

      private static void LoadTexture_Postfix(AnimatedSprite __instance, string textureName) {
#if DEBUG
        ModEntry.ModMonitor.VerboseLog($"{MethodBase.GetCurrentMethod().Name} (enabled: {Instance.PostfixEnabled})");
#endif
        if (!Instance.PostfixEnabled) {
          return;
        }

        if (textureName != null && Sprites.Assets.TryGetValue(textureName, out SpriteAsset spriteAsset)) {
          //__instance.SpriteWidth = spriteAsset.SourceRectangle.Width;
          //__instance.SpriteHeight = spriteAsset.SourceRectangle.Height;
          //__instance.UpdateSourceRect();
        }
      }

    }

    /*internal class AnimatePatch : ClassPatch {

      public override MethodInfo Original { get; } = AccessTools.Method(typeof(AnimatedSprite), nameof(AnimatedSprite.Animate), new[] { typeof(GameTime), typeof(int), typeof(int), typeof(float) });
      public override MethodInfo Prefix { get; } = AccessTools.Method(typeof(AnimatePatch), nameof(AnimatePatch.Animate_Prefix));

      private static IReflectionHelper Reflection { get; set; }

      public static AnimatePatch Instance { get; } = new AnimatePatch();

      // Explicit static constructor to avoid the compiler to mark type as beforefieldinit
      static AnimatePatch() { }

      private AnimatePatch() { }

      public static AnimatePatch CreatePatch(IReflectionHelper reflection) {
        Reflection = reflection;
        return Instance;
      }


      private static void Animate_Prefix(AnimatedSprite __instance, GameTime gameTime, int startFrame, ref int numberOfFrames, ref float interval) {
#if DEBUG
        ModEntry.ModMonitor.VerboseLog($"{MethodBase.GetCurrentMethod().Name} (enabled: {Instance.PostfixEnabled})");
#endif
        if (!Instance.PostfixEnabled) {
          return;
        }

        var textureName = __instance.Texture.Name;
        if (textureName != null && Sprites.assets.TryGetValue(textureName, out SpriteAsset spriteAsset)) {
          numberOfFrames = spriteAsset.Frames;
          interval = spriteAsset.Interval;
          __instance.CurrentFrame = spriteAsset.CurrentFrame++;
        }
      }

    }*/

  }

}
