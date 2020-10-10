/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium/Stardew_Valley_Mods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace EiTK.Gui.Option
{
    public class GuiOptionsElements
    {
        private string label;
        
        public GuiOptionsElements(string label)
        {
            this.label = label;
        }
        public virtual void receiveLeftClick()
        {
        }

        public virtual void draw(SpriteBatch b, int slotX, int slotY)
        {
            GuiHelper.drawString(b, this.label, slotX, slotY);
        }
    }
}