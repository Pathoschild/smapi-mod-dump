/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace NoItemIcons.Patches
{
    [HarmonyPatch(typeof(SObject), nameof(SObject.drawInMenu))]
    internal class ObjectDrawInMenuPatch
    {
        public static bool Prefix(SObject __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color)
        {
            if (!ModEntry.IsActive)
                return true;

            ModEntry.DrawWhiteIcon(spriteBatch, location, transparency, layerDepth);

            bool shouldDrawStackNumber = ((drawStackNumber == StackDrawType.Draw && __instance.maximumStackSize() > 1 && __instance.Stack > 1) || drawStackNumber == StackDrawType.Draw_OneInclusive) && (double)scaleSize > 0.3 && __instance.Stack != int.MaxValue;
            if (shouldDrawStackNumber)
                Utility.drawTinyDigits(__instance.Stack, spriteBatch, location + new Vector2((64 - Utility.getWidthOfTinyDigitString(__instance.Stack, 3f * scaleSize)) + 3f * scaleSize, 64f - 18f * scaleSize + 2f), 3f * scaleSize, 1f, color);

            return false;
        }
    }
}
