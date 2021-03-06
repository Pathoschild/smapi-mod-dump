/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace ResetTerrainFeaturesRedux.Framework.Menu
{
    public class MenuComponent
    {
        //Variables
        public string Name;
        public bool Title = false;
        public bool Disabled = false;
        public const int DefaultX = 8;
        public const int DefaultY = 4;
        public const int DefaultPixelWidth = 9;
        public Rectangle Bound;


        public MenuComponent(string name, bool title = false)
        {
            this.Name = name;
            this.Title = title;
            this.Bound = new Rectangle(32, 16, 36, 36);
        }

        public MenuComponent(string name, int x, int y, int width, int height)
        {
            this.Bound = new Rectangle(x == -1 ? 32 : x, y == -1 ? 16 : y, width, height);
            this.Name = name;
        }

        public MenuComponent(string name, Rectangle bounds)
        {
            this.Name = name;
            this.Bound = bounds;
        }

        public virtual void ReceiveLeftClick(int x, int y)
        {

        }

        public virtual void HoldLeftClick(int x, int y)
        {

        }

        public virtual void ReleaseLeftClick(int x, int y)
        {

        }

        public virtual void ReceiveKeyPress(Keys key)
        {

        }

        public virtual void Draw(SpriteBatch b, int slotX, int slotY)
        {
            Utility.drawTextWithShadow(b, this.Name, Game1.dialogueFont, new Vector2((float)(slotX + this.Bound.X + this.Bound.Width + 8), (float)(slotY + this.Bound.Y)), Game1.textColor * (this.Title ? 0.75f : 1f) * (this.Disabled ? 0.5f : 1f), this.Title ? 1.5f : 1f, 0.1f, -1, -1, 1f, 3);
        }

    }
}
