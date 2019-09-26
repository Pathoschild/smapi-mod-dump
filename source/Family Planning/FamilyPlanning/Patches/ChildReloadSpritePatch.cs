using System;
using Harmony;
using StardewValley;
using StardewValley.Characters;

namespace FamilyPlanning.Patches
{
    [HarmonyPatch(typeof(Child))]
    [HarmonyPatch("reloadSprite")]
    class ChildReloadSpritePatch
    {
        public static void Postfix(Child __instance)
        {
            if(__instance.Sprite == null || __instance.Sprite.textureName.Contains("Characters\\") || (__instance.Age >= 3 && __instance.Sprite.CurrentFrame == 0))
            {
                //Try to load the child sprite from a content pack
                Tuple<string, string> assetNames = ModEntry.GetChildSpriteData(__instance.Name);
                if (assetNames != null)
                {
                    string assetKey = __instance.Age >= 3 ? assetNames.Item2 : assetNames.Item1;
                    __instance.Sprite = new AnimatedSprite(assetKey);
                }
                //If that fails, load the vanilla sprite
                if (__instance.Sprite == null)
                    __instance.Sprite = new AnimatedSprite(__instance.Age >= 3 ? "Characters\\Toddler" + (__instance.Gender == 0 ? "" : "_girl") + (__instance.darkSkinned ? "_dark" : "") : "Characters\\Baby" + (__instance.darkSkinned ? "_dark" : ""));
            }
            
            //This is default behavior, applies to anyone
            __instance.HideShadow = true;
            switch (__instance.Age)
            {
                case 0:
                    __instance.Sprite.CurrentFrame = 0;
                    __instance.Sprite.SpriteWidth = 22;
                    __instance.Sprite.SpriteHeight = 16;
                    break;
                case 1:
                    __instance.Sprite.CurrentFrame = 4;
                    __instance.Sprite.SpriteWidth = 22;
                    __instance.Sprite.SpriteHeight = 32;
                    break;
                case 2:
                    __instance.Sprite.CurrentFrame = 32;
                    __instance.Sprite.SpriteWidth = 22;
                    __instance.Sprite.SpriteHeight = 16;
                    break;
                case 3:
                    __instance.Sprite.CurrentFrame = 0;
                    __instance.Sprite.SpriteWidth = 16;
                    __instance.Sprite.SpriteHeight = 32;
                    __instance.HideShadow = false;
                    break;
            }

            __instance.Sprite.UpdateSourceRect();
            __instance.Breather = false;
        }
    }
}