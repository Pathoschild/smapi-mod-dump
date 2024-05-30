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
using ArsVenefici.Framework.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.GUI.DragNDrop
{
    public class ShapeGroupArea : DragTargetArea<SpellPartDraggable>
    {
        [JsonIgnore]
        private static int ROWS = 2;

        [JsonIgnore]
        private static int COLUMNS = 2;

        [JsonIgnore]
        private static int X_PADDING = 2;

        [JsonIgnore]
        private static int Y_PADDING = 1;

        [JsonIgnore]
        public static int WIDTH = 128;

        [JsonIgnore]
        public static int HEIGHT = 128;

        [JsonIgnore]
        private Action<SpellPartDraggable, int> onDropAction;

        private LockState lockState = LockState.NONE;

        public ShapeGroupArea(int x, int y, Action<SpellPartDraggable, int> onDrop, string name) : base(new Rectangle(x, y, WIDTH, HEIGHT), ROWS * COLUMNS, name)
        {
            onDropAction = onDrop;
        }

        public ShapeGroupArea(int x, int y, Action<SpellPartDraggable, int> onDrop, string name, string lable) : base(new Rectangle(x, y, WIDTH, HEIGHT), ROWS * COLUMNS, name, lable)
        {
            onDropAction = onDrop;
        }

        public void SetLockState(LockState lockState)
        {
            this.lockState = lockState;
        }

        public override SpellPartDraggable ElementAt(int mouseX, int mouseY)
        {
            mouseX -= x;
            mouseY -= y;
            mouseX -= X_PADDING;
            mouseY -= Y_PADDING;
            if (mouseX < 0 || mouseX >= ROWS * SpellPartDraggable.SIZE || mouseY < 0 || mouseY >= COLUMNS * SpellPartDraggable.SIZE) return null;
            int index = 0;
            index += mouseX / SpellPartDraggable.SIZE;
            index += mouseY / SpellPartDraggable.SIZE * COLUMNS;

            return contents.Count() > index ? contents[index] : null;
        }

        public override bool CanPick(SpellPartDraggable draggable, int mouseX, int mouseY)
        {
            if (lockState == LockState.ALL)
                return false;

            //if (lockState == LockState.FIRST && contents.Any() &&  contents[0].getPart() == draggable.getPart()) 
            //    return false;

            if (lockState == LockState.FIRST && !(contents.Count() == 0) && contents[0].GetPart() == draggable.GetPart())
                return false;

            List<SpellPartDraggable> list = new List<SpellPartDraggable>(contents);
            list.Remove(draggable);

            return IsValid(list);
        }

        public override void Draw(SpriteBatch spriteBatch, int positionX, int positionY, float pPartialTick)
        {
            if (lockState == LockState.ALL)
            {
                IClickableMenu.drawTextureBox(spriteBatch, x, y, WIDTH, HEIGHT, Color.Gray);
            }
            else
            {
                IClickableMenu.drawTextureBox(spriteBatch, x, y, WIDTH, HEIGHT, Color.White);
            }

            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLUMNS; j++)
                {
                    int index = i * COLUMNS + j;

                    if (index >= contents.Count())
                        return;

                    contents[index].Draw(spriteBatch, x + j * SpellPartDraggable.SIZE + X_PADDING, y + i * SpellPartDraggable.SIZE + Y_PADDING, pPartialTick);
                }
            }
        }

        public override bool CanDrop(SpellPartDraggable draggable, int mouseX, int mouseY)
        {
            if (lockState == LockState.ALL)
                return false;

            if (!CanStore() || draggable.GetPart().GetType() == SpellPartType.COMPONENT)
                return false;

            List<SpellPartDraggable> list = new List<SpellPartDraggable>(contents);
            list.Add(draggable);

            return IsValid(list);
        }

        public static bool IsValid(List<SpellPartDraggable> list)
        {
            if (!list.Any())
                return true;

            SpellPartDraggable first = list[0];

            if (first.GetPart().GetType() != SpellPartType.SHAPE)
                return false;

            if (((ISpellShape)first.GetPart()).NeedsPrecedingShape())
                return false;

            Predicate<SpellPartDraggable> p = IsShape;

            SpellPartDraggable last = Utils.GetLastMatching(list, p);

            if (last != null)
            {
                for (int i = 1; i < list.Count(); i++)
                {
                    SpellPartDraggable part = list[i];
                    if (part.GetPart().GetType() == SpellPartType.MODIFIER)
                        continue;

                    if (((ISpellShape)part.GetPart()).NeedsToComeFirst())
                        return false;

                    if (part != last && ((ISpellShape)part.GetPart()).IsEndShape())
                        return false;
                }
            }

            return true;
        }

        private static bool IsShape(SpellPartDraggable e)
        {
            return e.GetPart().GetType() == SpellPartType.SHAPE;
        }

        public enum LockState
        {
            NONE, FIRST, ALL
        }
    }
}
