/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gunnargolf/DynamicChecklist
**
*************************************************/

namespace DynamicChecklist
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DynamicChecklist.ObjectLists;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.Menus;

    internal class DynamicSelectableCheckbox : OptionsCheckbox
    {
        private bool isDone = true;
        private ObjectList objectList;
        private Vector2 labelSize;

        public DynamicSelectableCheckbox(ObjectList objectList, int x = -1, int y = -1)
            : base(objectList.OptionMenuLabel, 1, x, y)
        {
            this.objectList = objectList;
            this.isDone = objectList.TaskDone;

            this.labelSize = Game1.dialogueFont.MeasureString(this.label);
        }

        public override void draw(SpriteBatch b, int slotX, int slotY)
        {
            this.isChecked = this.objectList.OverlayActive;
            base.draw(b, slotX, slotY);
            var whitePixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            whitePixel.SetData(new Color[] { Color.White });
            var destRect = new Rectangle(slotX + this.bounds.X + this.bounds.Width + Game1.pixelZoom * 2, slotY + this.bounds.Y + (int)this.labelSize.Y / 3, (int)this.labelSize.X, Game1.pixelZoom);
            if (this.isDone)
            {
                b.Draw(whitePixel, destRect, Color.Red);
            }
        }

        public override void receiveLeftClick(int x, int y)
        {
            base.receiveLeftClick(x, y);
            if (this.objectList != null)
            {
                this.objectList.OverlayActive = this.isChecked;
            }
        }
    }
}