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
using StardewValley;
using StardewValley.Menus;

namespace ResetTerrainFeaturesRedux.Framework.Menu
{
    public class Buttons : MenuComponent
    {
        private Action action;
        public bool HeldDown = false;
        public Color DisabledTint = new Color(200, 200, 200);

        public Buttons(string name, Action action) : base(name, false)
        {
            this.action = action;
            this.Bound = new Rectangle(32, 0, (int) Game1.dialogueFont.MeasureString(name).X + 64, 80);
        }

        public override void HoldLeftClick(int x, int y)
        {
            if (this.Disabled)
                this.HeldDown = true;
        }

        public override void ReleaseLeftClick(int x, int y)
        {
            this.HeldDown = false;
        }

        public override void ReceiveLeftClick(int x, int y)
        {
            if (!(this.Bound.Contains(x, y) || this.action == null || this.Disabled))
            {
                Game1.playSound("Ship");
                this.action();
            }
        }

        public override void Draw(SpriteBatch b, int slotX, int slotY)
        {
            IClickableMenu.drawTextureBox(b, 
                Game1.mouseCursors, 
                new Rectangle(432, 439, 9, 9), 
                slotX + this.Bound.X, 
                slotY + this.Bound.Y, 
                this.Bound.Width, 
                this.Bound.Height, 
                (this.Disabled ? this.DisabledTint : Color.White) * (this.HeldDown ? 0.4f : 1f), 
                4f, 
                true);
            Utility.drawTextWithShadow(b, 
                this.Name, 
                Game1.dialogueFont, 
                new Vector2((float)(slotX + this.Bound.Center.X), 
                    (float)(slotY + this.Bound.Center.Y + 4)) - Game1.dialogueFont.MeasureString(this.Name) / 2f, 
                Game1.textColor * (this.Disabled ? 0.5f : 1f), 
                1f, 
                1f, 
                -1, 
                -1, 
                0f, 
                3);
            base.Draw(b, slotX, slotY);
        }
    }
}
