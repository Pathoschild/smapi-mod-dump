/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace SeasonalSaveSlots
{
    [HarmonyPatch(typeof(LoadGameMenu.SaveFileSlot), "Draw")]
    internal class SaveFileSlotPatch
    {
        private static void DrawSeasonIcon(SpriteBatch b, LoadGameMenu menu, LoadGameMenu.SaveFileSlot slot, int slotIndex, int season)
        {
            MethodInfo slotSubName = typeof(LoadGameMenu.SaveFileSlot).GetMethod("slotSubName", BindingFlags.NonPublic | BindingFlags.Instance)!;
            string subName = (string)slotSubName.Invoke(slot, null)!;

            Utility.drawWithShadow(
                b,
                Game1.mouseCursors,
                new Vector2(
                    (menu.slotButtons[slotIndex].bounds.X + menu.width) - Game1.dialogueFont.MeasureString(subName).X - 192,
                    menu.slotButtons[slotIndex].bounds.Y + 49
                ),
                new Rectangle(406, 441 + season * 8, 12, 8),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                flipped: false,
                layerDepth: 1f
            );
        }

        private static void DrawSeasonColoredLine(SpriteBatch b, LoadGameMenu menu, int slotIndex, int season)
        {
            Color color = season switch
            {
                0 => new(5, 122, 13),
                1 => new(255, 255, 23),
                2 => new(209, 52, 0),
                _ => new(103, 183, 255)
            };

            int padding = 20;
            b.Draw(
                Game1.fadeToBlackRect,
                new Rectangle(
                    menu.slotButtons[slotIndex].bounds.X + padding,
                    menu.slotButtons[slotIndex].bounds.Y + 12,
                    menu.slotButtons[slotIndex].bounds.Width - padding * 2,
                    5
                ),
                null,
                color,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                1f
            );
        }

        public static void Postfix(LoadGameMenu.SaveFileSlot __instance, SpriteBatch b, int i)
        {
            LoadGameMenu menu = (LoadGameMenu)typeof(LoadGameMenu.SaveFileSlot).GetField("menu", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(__instance)!;
            int season = __instance.Farmer.seasonForSaveGame ?? 0;

            if (ModEntry.Config.ShowSeasonIcon)
                DrawSeasonIcon(b, menu, __instance, i, season);
            if (ModEntry.Config.ShowSeasonColoredLine)
                DrawSeasonColoredLine(b, menu, i, season);
        }
    }
}
