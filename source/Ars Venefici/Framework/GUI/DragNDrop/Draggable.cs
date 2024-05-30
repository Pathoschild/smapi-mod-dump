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
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.GUI.DragNDrop
{
    public abstract class Draggable<T> : IRenderable
    {

        protected int width;
        protected int height;
        protected T content;

        protected Draggable(int width, int height, T content)
        {
            this.width = width;
            this.height = height;
            this.content = content;
        }

        public abstract void Draw(SpriteBatch spriteBatch, int positionX, int positionY, float pPartialTick);
    }
}
