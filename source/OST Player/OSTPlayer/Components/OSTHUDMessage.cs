/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ProfeJavix/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Extensions;

namespace OSTPlayer
{
    public class OSTHUDMessage : HUDMessage
    {
        private static Texture2D playIcon = null, stopIcon = null;
        private bool playing;
        public OSTHUDMessage(string message, bool playing = false) : base(message)
        {
            this.playing = playing;
            noIcon = false;
            if (playIcon == null || stopIcon == null)
                loadIcons();
        }

        private void loadIcons()
        {
            if (playIcon == null)
            {
                playIcon = ModEntry.context.Helper.ModContent.Load<Texture2D>("assets/playIcon.png");
            }

            if (stopIcon == null)
            {
                stopIcon = ModEntry.context.Helper.ModContent.Load<Texture2D>("assets/stopIcon.png");
            }
        }

        public override void draw(SpriteBatch b, int i, ref int heightUsed)
        {
            Rectangle titleSafeArea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();
            int num = 112;
            Vector2 vector = new Vector2(titleSafeArea.Left + 16, titleSafeArea.Bottom - num - heightUsed - 64);
            heightUsed += num;
            if (Game1.isOutdoorMapSmallerThanViewport())
            {
                vector.X = Math.Max(titleSafeArea.Left + 16, -Game1.uiViewport.X + 16);
            }

            if (Game1.uiViewport.Width < 1400)
            {
                vector.Y -= 48f;
            }
            b.Draw(Game1.mouseCursors, vector, new Rectangle(293, 360, 26, 24), Color.White * transparency, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            float x = Game1.smallFont.MeasureString(message ?? "").X;
            b.Draw(Game1.mouseCursors, new Vector2(vector.X + 104f, vector.Y), new Rectangle(319, 360, 1, 24), Color.White * transparency, 0f, Vector2.Zero, new Vector2(x, 4f), SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2(vector.X + 104f + x, vector.Y), new Rectangle(323, 360, 6, 24), Color.White * transparency, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

            vector.X += 16f;
            vector.Y += 16f;
            Rectangle iconRect = new Rectangle((int)vector.X-4, (int)vector.Y-4, 18*4, 18*4);
            b.Draw(playing? playIcon : stopIcon, iconRect, (playing? Color.MediumSeaGreen: Color.Red) * transparency);

            vector.X += 83f;
            vector.Y += 18f;
            Utility.drawTextWithShadow(b, message ?? "", Game1.smallFont, vector, Game1.textColor * transparency, 1f, 1f, -1, -1, transparency);

        }
    }
}
