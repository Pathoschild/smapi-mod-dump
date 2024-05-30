/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.GUI.Menus;
using ArsVenefici.Framework.Interfaces.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.GUI.DragNDrop
{
    public class ShapeGroupListArea : DragTargetArea<SpellPartDraggable>
    {
        public List<ShapeGroupArea> shapeGroups;
        private SpellBookMenu menu;

        public ShapeGroupListArea(int x, int y, SpellBookMenu screen, Action<SpellPartDraggable, int, int> onDrop, string name)
            : base(new Rectangle(x, y, ShapeGroupArea.WIDTH * screen.AllowedShapeGroups(), ShapeGroupArea.HEIGHT), 20, name)
        {
            menu = screen;
            shapeGroups = new List<ShapeGroupArea>();

            for (int i = 0; i < screen.AllowedShapeGroups(); i++)
            {
                int finalI = i;

                //(part, index)-> onDrop.accept(part, finalI, index)

                shapeGroups.Add(new ShapeGroupArea(x + i * ShapeGroupArea.WIDTH, y, (part, index) => onDrop.Invoke(part, finalI, index), name));
            }

            SetLocks();
        }

        public ShapeGroupListArea(int x, int y, SpellBookMenu screen, Action<SpellPartDraggable, int, int> onDrop, string name, string lable)
            : base(new Rectangle(x, y, ShapeGroupArea.WIDTH * screen.AllowedShapeGroups(), ShapeGroupArea.HEIGHT), 20, name, lable)
        {
            menu = screen;
            shapeGroups = new List<ShapeGroupArea>();

            for (int i = 0; i < screen.AllowedShapeGroups(); i++)
            {
                int finalI = i;

                //(part, index)-> onDrop.accept(part, finalI, index)

                shapeGroups.Add(new ShapeGroupArea(x + i * ShapeGroupArea.WIDTH, y, (part, index) => onDrop.Invoke(part, finalI, index), name));
            }

            SetLocks();
        }

        public override SpellPartDraggable ElementAt(int mouseX, int mouseY)
        {
            ShapeGroupArea area = GetHoveredArea(mouseX, mouseY);
            return area == null ? null : area.ElementAt(mouseX, mouseY);
        }

        public override List<SpellPartDraggable> GetAll()
        {
            List<SpellPartDraggable> list = new List<SpellPartDraggable>();

            shapeGroups.ForEach(e => list.AddRange(e.GetAll()));

            return list;
        }

        public override void SetAll(int index, List<SpellPartDraggable> list)
        {
            shapeGroups[index].SetAll(list);
        }

        public override bool CanPick(SpellPartDraggable draggable, int mouseX, int mouseY)
        {
            ShapeGroupArea area = GetHoveredArea(mouseX, mouseY);
            return area != null && area.CanPick(draggable, mouseX, mouseY);
        }

        public override bool CanDrop(SpellPartDraggable draggable, int mouseX, int mouseY)
        {
            ShapeGroupArea area = GetHoveredArea(mouseX, mouseY);
            return area != null && area.CanDrop(draggable, mouseX, mouseY);
        }


        public override void Pick(SpellPartDraggable draggable, int mouseX, int mouseY)
        {
            ShapeGroupArea area = GetHoveredArea(mouseX, mouseY);

            if (area != null && area.CanPick(draggable, mouseX, mouseY))
            {
                area.Pick(draggable, mouseX, mouseY);
            }

            SetLocks();
        }

        public override void Drop(SpellPartDraggable draggable, int mouseX, int mouseY)
        {
            ShapeGroupArea area = GetHoveredArea(mouseX, mouseY);

            if (area != null && area.CanDrop(draggable, mouseX, mouseY))
            {
                area.Drop(draggable, mouseX, mouseY);
            }

            SetLocks();
        }

        public override bool CanStore()
        {
            return shapeGroups.Exists(e => e.CanStore());
        }

        public override void Draw(SpriteBatch spriteBatch, int positionX, int positionY, float pPartialTick)
        {
            //IClickableMenu.drawTextureBox(spriteBatch, x, y, width, height, Color.White);

            foreach (ShapeGroupArea area in shapeGroups)
            {
                area.Draw(spriteBatch, area.x, area.y, pPartialTick);
            }
        }

        public bool IsValid()
        {
            return shapeGroups.Exists(e => ShapeGroupArea.IsValid(e.GetAll()));
        }

        //public List<List<ResourceLocation>> getShapeGroupData()
        //{
        //    return shapeGroups.stream()
        //            .map(e->e.getAll().stream()
        //                    .map(f->f.getPart().getId())
        //                    .toList())
        //            .toList();
        //}

        public ShapeGroupArea Get(int index)
        {
            return shapeGroups[index];
        }

        private ShapeGroupArea GetHoveredArea(int mouseX, int mouseY)
        {
            foreach (ShapeGroupArea area in shapeGroups)
            {
                //if (area.isAbove(mouseX, mouseY)) return area;
                if (area.bounds.Contains(mouseX, mouseY)) return area;
            }

            return null;
        }

        public void SetLocks()
        {
            //shapeGroups.ForEach(e => e.setLockState(ShapeGroupArea.LockState.ALL));

            for (int i = 0; i < menu.AllowedShapeGroups(); i++)
            {
                shapeGroups[i].SetLockState(ShapeGroupArea.LockState.NONE);
            }

            //for (int i = 0; i < shapeGroups.Count; i++)
            //{
            //    ShapeGroupArea area = shapeGroups[i];

            //    //if (area.getAll().Count() == 1 && shapeGroups.Count() > i + 1 && shapeGroups[i + 1].getAll().Any())
            //    //{
            //    //    area.setLockState(ShapeGroupArea.LockState.FIRST);
            //    //}

            //    //if (i > 0 && !(shapeGroups[i - 1].getAll().Any()))
            //    //{
            //    //    area.setLockState(ShapeGroupArea.LockState.ALL);
            //    //}

            //    if (area.getAll().Count == 1 && shapeGroups.Count > i + 1 && !(shapeGroups[i + 1].getAll().Count == 0))
            //    {
            //        area.setLockState(ShapeGroupArea.LockState.FIRST);
            //    }

            //    if (i > 0 && shapeGroups[i - 1].getAll().Count == 0)
            //    {
            //        area.setLockState(ShapeGroupArea.LockState.ALL);
            //    }
            //}
        }

        public List<ShapeGroupArea> GetShapeGroups()
        {
            return shapeGroups;
        }
    }
}
