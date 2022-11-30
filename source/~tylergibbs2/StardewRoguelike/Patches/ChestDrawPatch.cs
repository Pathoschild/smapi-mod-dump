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
using StardewValley.Menus;
using StardewValley.Objects;
using System;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(Chest), "draw", new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) })]
    internal class ChestDrawPatch
    {
        public static void Postfix(Chest __instance, SpriteBatch spriteBatch, int x, int y)
        {
            if (__instance is not TimedChest chest)
                return;

            var span = TimeSpan.FromSeconds(Math.Abs(chest.SecondsLeft));
            string sign = chest.SecondsLeft < 0 ? "-" : "";
            string timeStr = $"{sign}{span.Minutes:0}:{span.Seconds:00}";
            Vector2 textSize = Game1.smallFont.MeasureString(timeStr);

            int width = (int)textSize.X + 32;

            Vector2 drawPos = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + 32 - (width / 2), y * 64 - 48));

            float base_sort_order = Math.Max(0f, ((y + 1f) * 64f - 24f) / 10000f) + x * 1E-05f;
            IClickableMenu.drawTextureBox(
                spriteBatch,
                Game1.menuTexture,
                new Rectangle(0, 256, 60, 60),
                (int)drawPos.X,
                (int)drawPos.Y,
                width,
                (int)textSize.Y + 16,
                Color.White,
                1f,
                drawShadow: false,
                draw_layer: base_sort_order + 0.001f
            );

            Utility.drawTextWithShadow(
                spriteBatch,
                timeStr,
                Game1.smallFont,
                new(drawPos.X + (width / 2) - (textSize.X / 2), drawPos.Y + 10),
                Game1.textColor,
                layerDepth: base_sort_order + 0.0011f
            );
        }
    }
}
