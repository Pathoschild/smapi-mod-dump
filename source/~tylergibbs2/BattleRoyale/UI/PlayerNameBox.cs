/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace BattleRoyale
{
    public class PlayerNameBox
    {
        public static SpriteFont MessageFont(LocalizedContentManager.LanguageCode language)
        {
            return Game1.content.Load<SpriteFont>("Fonts\\SmallFont", language);
        }

        public static void draw(SpriteBatch b, Farmer farmer)
        {
            SpriteFont font = MessageFont(LocalizedContentManager.CurrentLanguageCode);
            int width = (int)font.MeasureString(farmer.name).X;
            int height = (int)font.MeasureString(farmer.name).Y;

            int x = farmer.getStandingX() - width / 2 - Game1.viewport.X;
            int y = farmer.getStandingY() - (farmer.GetBoundingBox().Height * 2 + height * 2) - 16 - Game1.viewport.Y;

            float drawLayer = farmer.getDrawLayer();

            b.Draw(Game1.staminaRect, new Rectangle(x, y, width, height), null, Color.Black * 0.6f, 0f, Vector2.Zero, SpriteEffects.None, drawLayer);
            b.DrawString(font, farmer.name, new Vector2(x, y), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, drawLayer + 0.01f);
        }
    }
}
