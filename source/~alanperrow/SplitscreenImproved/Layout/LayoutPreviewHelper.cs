/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace SplitscreenImproved.Layout
{
    internal static class LayoutPreviewHelper
    {
        private static Color P1Color { get; } = new Color(60, 90, 220);

        private static Color P2Color { get; } = new Color(255, 50, 50);

        private static Color P3Color { get; } = new Color(50, 230, 50);

        private static Color P4Color { get; } = new Color(255, 220, 30);

        public static int PlayerCount { get; set; } = 2;

        public static SplitscreenLayout Layout { get; set; }

        public static LayoutPreset? Preset { get; set; }

        public static bool IsModEnabled { get; set; } = true;

        public static bool IsLayoutFeatureEnabled { get; set; } = true;

        /// <summary>
        /// Draws a preview of the current layout with the number of players provided by <see cref="PlayerCount"/>
        /// (or optionally, <paramref name="playerCount"/>).
        /// </summary>
        /// <param name="sb">SpriteBatch</param>
        /// <param name="p">Vector2</param>
        /// <param name="playerCount">Optional value for player count, to bypass <see cref="PlayerCount"/>.</param>
        public static void DrawPreview(SpriteBatch sb, Vector2 p, int? playerCount = null)
        {
            playerCount ??= PlayerCount;

            int px = (int)p.X;
            int py = (int)p.Y;

            if (!(IsModEnabled && IsLayoutFeatureEnabled) || Layout is null)
            {
                sb.DrawString(Game1.dialogueFont, "Preview Disabled", new Vector2(px, py + 80), Color.Black);
                return;
            }

            // Border
            sb.Draw(Game1.fadeToBlackRect, new Rectangle(px, py, 208, 208), Color.Black);

            // Iterate for each player
            for (int i = 1; i <= playerCount.Value; i++)
            {
                (Rectangle rectangle, Vector2 textPos) = GetPreviewRectangleAndTextPosition(p, i, playerCount.Value, Layout);

                // Player screen location
                Color color = i switch
                {
                    1 => P1Color,
                    2 => P2Color,
                    3 => P3Color,
                    _ => P4Color,
                };
                sb.Draw(Game1.fadeToBlackRect, rectangle, color);

                // Player indicator
                string text = $"P{i}";
                sb.DrawString(Game1.dialogueFont, text, textPos, Color.Black);
            }
        }

        private static (Rectangle PreviewRectangle, Vector2 TextPosition) GetPreviewRectangleAndTextPosition(
            Vector2 p,
            int playerNum,
            int playerCount,
            SplitscreenLayout layoutToPreview)
        {
            Vector4 v = layoutToPreview.GetScreenSplits(playerCount)[playerNum - 1];
            int px = (int)p.X + 4;
            int py = (int)p.Y + 4;

            int x1 = px + (int)(v.X * 200);
            int y1 = py + (int)(v.Y * 200);
            int w = (int)(v.Z * 200);
            int h = (int)(v.W * 200);
            Rectangle rect = new(x1, y1, w, h);

            int tx = x1 + (w / 2) - 20;
            int ty = y1 + (h / 2) - 20;
            Vector2 textPos = new(tx, ty);

            return (rect, textPos);
        }
    }
}
