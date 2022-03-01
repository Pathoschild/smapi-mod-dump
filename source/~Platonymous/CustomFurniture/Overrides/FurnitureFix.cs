/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Reflection;

namespace CustomFurniture.Overrides
{
    [HarmonyPatch]
    public class FurnitureFix
    {
        internal static MethodInfo TargetMethod()
        {
            if (Type.GetType("StardewValley.Objects.Furniture, Stardew Valley") != null)
                return AccessTools.Method(Type.GetType("StardewValley.Objects.Furniture, Stardew Valley"), "drawAtNonTileSpot");
            else
                return AccessTools.Method(Type.GetType("StardewValley.Objects.Furniture, StardewValley"), "drawAtNonTileSpot");
        }

        internal static bool Prefix(Furniture __instance, SpriteBatch spriteBatch, Vector2 location, float layerDepth, float alpha = 1f)
        {
 
            if (__instance is CustomFurniture ho)
            {
                CustomFurnitureMod.harmonyDraw(ho.texture, location, new Rectangle?(ho.sourceRect), Color.White * alpha, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, ho.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch]
    public class FurnitureFix2
    {
        internal static MethodInfo TargetMethod()
        {
            if (Type.GetType("StardewValley.Objects.Furniture, Stardew Valley") != null)
                return AccessTools.Method(Type.GetType("StardewValley.Objects.Furniture, Stardew Valley"), "rotate");
            else
                return AccessTools.Method(Type.GetType("StardewValley.Objects.Furniture, StardewValley"), "rotate");
        }

        internal static bool Prefix(Furniture __instance)
        {
            if (__instance is CustomFurniture cf)
                cf.customRotate();
            else
                return true;

            return false;
        }
    }
}
