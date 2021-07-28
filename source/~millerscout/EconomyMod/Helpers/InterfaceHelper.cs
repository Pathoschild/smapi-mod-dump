/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/millerscout/StardewMillerMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace EconomyMod.Helpers
{
    public static class InterfaceHelper
    {
        public enum InterfaceHelperType
        {
            Red,
            Yellow,
            Black,
            Cyan
        }

        private static Color GetColorByType(InterfaceHelperType type)
        {
            switch (type)
            {
                case InterfaceHelperType.Red:
                    return Color.Red;
                case InterfaceHelperType.Yellow:
                    return Color.Yellow;
                case InterfaceHelperType.Black:
                    return Color.Black;
                case InterfaceHelperType.Cyan:
                    return Color.LightCyan;
                default:
                    return Color.Black;
            }

        }
        public static Rectangle GetButtonSizeForPage(IClickableMenu menu, bool margin = true)
        {
            var rec = new Rectangle(menu.xPositionOnScreen + 16, menu.yPositionOnScreen + 80 + 4 + ((menu.height - 128) / 7), menu.width - 32, (menu.height - 128) / 7 + 4);
            rec.X += 8 * Game1.pixelZoom;
            rec.Width -= 8 * Game1.pixelZoom * 2;
            return rec;

        }

        public static Rectangle GetSideTabSizeForPage(IClickableMenu menu, int count)
        {
            return new Rectangle(menu.xPositionOnScreen - 48 + (count == 0 ? Constants.sideTab_widthToMoveActiveTab : 0), menu.yPositionOnScreen + 64 * (2 + count), 64, 64);
        }

        private static Texture2D texture;
        private static SpriteBatch batch;

        public static bool DrawGuidelines = false;

        public static void Draw(Rectangle rectangle, InterfaceHelperType type = InterfaceHelperType.Black, bool center = false)
        {
            PrepareDraw();
            if (!DrawGuidelines) return;
            Color color = GetColorByType(type);

            batch.Draw(texture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, 3), color);
            batch.Draw(texture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width, 3), color);
            batch.Draw(texture, new Rectangle(rectangle.X, rectangle.Y, 3, rectangle.Height), color);
            batch.Draw(texture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, 3, rectangle.Height), color);

            if (center)
                Draw(new Vector2(rectangle.Center.X, rectangle.Center.Y), InterfaceHelperType.Cyan);

        }

        private static void PrepareDraw()
        {
            if (batch == null) batch = Game1.spriteBatch;
            if (texture == null)
            {
                texture = new Texture2D(batch.GraphicsDevice, 1, 1);
                texture.SetData(new Color[] { Color.White });
            }

        }

        internal static void Draw(Vector2 btnPosition, InterfaceHelperType type)
        {
            PrepareDraw();
            if (!DrawGuidelines) return;
            batch.Draw(texture, new Rectangle(Convert.ToInt32(btnPosition.X), Convert.ToInt32(btnPosition.Y), 5, 5), GetColorByType(type));
        }
        public static bool ClickOnTriggerArea(int x, int y, Rectangle bound) {

            return (x >= bound.X && x <= bound.X + bound.Width && y >= bound.Y && y <= bound.Y + bound.Height);
        }
    }
}
