/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Interfaces.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.GUI.DragNDrop
{
    public class SpellGrammarArea : DragTargetArea<SpellPartDraggable>
    {
        private static int X_PADDING = 4;
        private Action<SpellPartDraggable, int> onDropAction;

        public SpellGrammarArea(Rectangle bounds, Action<SpellPartDraggable, int> onDrop, string name) : base(bounds, 8, name)
        {
            onDropAction = onDrop;
        }

        public SpellGrammarArea(Rectangle bounds, Action<SpellPartDraggable, int> onDrop, string name, string lable) : base(bounds, 8, name, lable)
        {
            onDropAction = onDrop;
        }

        public override SpellPartDraggable ElementAt(int mouseX, int mouseY)
        {
            if (mouseX < x + X_PADDING || mouseX >= x + maxElements * SpellPartDraggable.SIZE + X_PADDING || mouseY < y || mouseY >= y + SpellPartDraggable.SIZE)
                return null;

            int index = (mouseX - x - X_PADDING) / SpellPartDraggable.SIZE;

            return contents.Count() > index ? contents[index] : null;
        }

        public override bool CanPick(SpellPartDraggable draggable, int mouseX, int mouseY)
        {
            return contents.Count() < 2 || draggable.GetPart().GetType() == SpellPartType.MODIFIER || contents[0].GetPart() != draggable.GetPart() || contents[1].GetPart().GetType() != SpellPartType.MODIFIER;
        }

        public override bool CanDrop(SpellPartDraggable draggable, int mouseX, int mouseY)
        {
            return CanStore() && (draggable.GetPart().GetType() == SpellPartType.COMPONENT || draggable.GetPart().GetType() == SpellPartType.MODIFIER && contents.Any() && contents[0].GetPart().GetType() == SpellPartType.COMPONENT);
            //return canStore();
        }

        public override void Draw(SpriteBatch spriteBatch, int positionX, int positionY, float pPartialTick)
        {
            IClickableMenu.drawTextureBox(spriteBatch, x, y, width, height, Color.White);

            for (int i = 0; i < contents.Count(); i++)
            {
                contents[i].Draw(spriteBatch, x + i * SpellPartDraggable.SIZE + X_PADDING, y, pPartialTick);
            }
        }

        public override void OnDrop(SpellPartDraggable draggable, int index)
        {
            onDropAction.Invoke(draggable, index);
        }
    }
}
