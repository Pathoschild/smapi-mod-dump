/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ArsVenefici.Framework.GUI.DragNDrop
{
    public abstract class DragSourceArea<T> : DragArea<T>
    {
        protected int maxDisplay;

        public DragSourceArea(Rectangle bounds, int maxDisplay, string name) : base(bounds, name)
        {
            this.maxDisplay = maxDisplay;
        }

        public DragSourceArea(Rectangle bounds, int maxDisplay, string name, string lable) : base(bounds, name, lable)
        {
            this.maxDisplay = maxDisplay;
        }

        public override bool CanPick(T draggable, int mouseX, int mouseY)
        {
            return true;
        }

        public override bool CanDrop(T draggable, int mouseX, int mouseY)
        {
            return false;
        }

        public override List<T> GetVisible()
        {
            //return getAll().stream().limit(maxDisplay).toList();

            List<T> visible = GetAll();
            //visible.Resize(maxDisplay);

            //T[] visible = new T[maxDisplay];

            //for (int i = 0; i < visible.Length; i++)
            //{
            //    visible[i] = getAll()[i];
            //}

            return visible.ToList();
        }
    }
}
