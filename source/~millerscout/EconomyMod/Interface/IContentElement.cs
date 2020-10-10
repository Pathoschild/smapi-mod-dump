using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace EconomyMod.Interface
{
    public interface IContentElement
    {
        void draw(SpriteBatch spriteBatch, int x, int y);
    }
}
