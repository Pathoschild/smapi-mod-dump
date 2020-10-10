/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/loe2run/FamilyPlanningMod
**
*************************************************/

using System;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;

namespace FamilyPlanning.Patches
{
    class ChildReloadSpritePatch
    {
        public static void Postfix(Child __instance)
        {
            //Added in the 1.4 update
            //(Presumably this fixes the multiplayer glitches)
            if (Game1.IsMasterGame && __instance.idOfParent.Value == 0L)
            {
                int uniqueMultiplayerId = (int)Game1.MasterPlayer.UniqueMultiplayerID;
                if (Game1.currentLocation is FarmHouse)
                {
                    FarmHouse currentLocation = Game1.currentLocation as FarmHouse;
                    if (currentLocation.owner != null)
                        uniqueMultiplayerId = (int)currentLocation.owner.UniqueMultiplayerID;
                }
                __instance.idOfParent.Value = uniqueMultiplayerId;
            }

            if (__instance.Sprite == null || __instance.Sprite.textureName.Contains("Characters\\") || (__instance.Age >= 3 && __instance.Sprite.CurrentFrame == 0))
            {
                //Try to load the child sprite from a content pack
                Tuple<string, string> assetNames = ModEntry.GetChildSpriteData(__instance.Name);
                if (assetNames != null)
                {
                    string assetKey = __instance.Age >= 3 ? assetNames.Item2 : assetNames.Item1;
                    __instance.Sprite = new AnimatedSprite(assetKey);
                }
                //If that fails, try to load the child sprite from a Content Patcher content pack
                try
                {
                    __instance.Sprite = new AnimatedSprite("Characters\\Child_" + __instance.Name);
                }
                catch (Exception) { }

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
 