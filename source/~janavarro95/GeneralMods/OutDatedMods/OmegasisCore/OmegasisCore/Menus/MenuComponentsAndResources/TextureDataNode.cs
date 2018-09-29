using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegasisCore.Menus.MenuComponentsAndResources
{
    public class TextureDataNode
    {
        public Texture2D texture;
        public string path;

        public TextureDataNode(Texture2D Texture, string Path)
        {
            texture = Texture;
            path = Path;
        }

    }
}
