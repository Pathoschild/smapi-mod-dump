/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Interfaces.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.GUI.DragNDrop
{
    public abstract class DragArea<T> : ClickableComponent, IRenderable
    {
        public int x;
        public int y;
        public int width;
        public int height;

        protected List<T> contents = new List<T>();

        public DragArea(Rectangle bounds, string name) : base(bounds, name)
        {
            this.x = bounds.X;
            this.y = bounds.Y;
            this.width = bounds.Width;
            this.height = bounds.Height;
        }

        public DragArea(Rectangle bounds, string name, string lable) : base(bounds, name, lable)
        {
            this.x = bounds.X;
            this.y = bounds.Y;
            this.width = bounds.Width;
            this.height = bounds.Height;
        }

        public abstract T ElementAt(int mouseX, int mouseY);

        //public bool isAbove(int mouseX, int mouseY)
        //{
        //    return mouseX >= x && mouseX < x + width && mouseY >= y && mouseY < y + height;
        //}

        public virtual List<T> GetAll()
        {
            return contents;
        }

        public virtual void SetAll(List<T> list)
        {
            contents = list;
        }

        public virtual void SetAll(int index, List<T> list)
        {
            contents = list;
        }

        public virtual List<T> GetVisible()
        {
            return GetAll();
        }

        public virtual bool CanPick(T draggable, int mouseX, int mouseY)
        {
            return true;
        }

        public virtual bool CanDrop(T draggable, int mouseX, int mouseY)
        {
            return true;
        }

        public virtual void Pick(T draggable, int mouseX, int mouseY)
        {
            contents.Remove(draggable);
        }

        public virtual void Drop(T draggable, int mouseX, int mouseY)
        {
            contents.Add(draggable);
            int index = contents.Count() - 1;

            if (mouseX != int.MaxValue && mouseY != int.MaxValue)
            {
                OnDrop(draggable, index);
            }
        }

        public virtual void OnDrop(T draggable, int index) { }

        public abstract void Draw(SpriteBatch spriteBatch, int positionX, int positionY, float pPartialTick);
    }
}
