/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Spells.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.GUI.Menus
{
    public class MagicAltarTabButton : ClickableComponent
    {
        private static readonly int SIZE = 22;
        private readonly int index;
        private readonly int xOffset;
        private readonly int yOffset;
        private readonly MagicAltarTab tab;
        public bool isHovered;

        public MagicAltarTabButton(int index, int x, int y, MagicAltarTab tab, string name) : base(new Rectangle(x, y, SIZE, SIZE), name)
        {
            this.index = index;
            this.xOffset = 0;
            this.yOffset = 0;
            this.tab = tab;
        }
        public MagicAltarTabButton(int index, int x, int y, int xOffset, int yOffset, MagicAltarTab tab, string name) : base(new Rectangle(x, y, SIZE, SIZE), name)
        {
            this.index = index;
            this.xOffset = xOffset;
            this.yOffset = yOffset;
            this.tab = tab;
        }

        public void Draw(SpriteBatch spriteBatch, int positionX, int positionY)
        {
            int scale = 2;

            IClickableMenu.drawTextureBox(spriteBatch, bounds.X - 5, bounds.Y - 5, tab.GetIcon().Width + 40, tab.GetIcon().Height + 40, Color.White);
            spriteBatch.Draw(tab.GetIcon(), new Vector2(this.bounds.X + 2f * scale, this.bounds.Y + 2f * scale), null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            
            //isHovered = bounds.Contains(positionX, positionY);

            if(isHovered)
            {
                string tabNameText = tab.GetName();

                int val1 = 272;
                if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr)
                    val1 = 384;
                if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.tr)
                    val1 = 336;

                int value = Math.Max(val1, (int)Game1.dialogueFont.MeasureString(tabNameText == null ? "" : tabNameText).X);

                if (tabNameText != null && tabNameText != null)
                    IClickableMenu.drawToolTip(spriteBatch, tabNameText, null, null);
            }
        }
        public int GetIndex()
        {
            return index;
        }

        public void IsHovered(int positionX, int positionY)
        {
            Rectangle rectangle = new Rectangle(bounds.X - 5, bounds.Y - 5, tab.GetIcon().Width + 40, tab.GetIcon().Height + 40);

            if(rectangle.Contains(positionX, positionY))
            {
                isHovered = true;
            }
            else
            {
                isHovered = false;
            }
        }

        public MagicAltarTab GetTab()
        {
            return tab;
        }
    }
}
