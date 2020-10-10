/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium/Stardew_Valley_Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace EiTK.Gui
{
    public class GuiHelper
    {
        
        public static void drawBox(SpriteBatch b,int x,int y,int width,int height,Color color)
        {
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), x, y, width, height + 32, color, 4f, true);
        }
        
        public static void drawString(SpriteBatch b,string text, int x, int y)
        {
            SpriteText.drawString(b,text,x,y);
        }
        
        public static void drawString(SpriteBatch b,string text, int x, int y,int color)
        {
            SpriteText.drawString(b,text,x,y,color:color);
        }

        public static void openGui(IClickableMenu i)
        {
            Game1.activeClickableMenu = i;
        }

        public static int getStringWidth(string text)
        {
            return SpriteText.getWidthOfString(text);
        }
        
        public static int getStringHeight(string text)
        {
            return SpriteText.getHeightOfString(text);
        }
        
    }
}