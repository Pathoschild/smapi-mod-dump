using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.UIUtilities
{
    public class TextureManager
    {
        public Dictionary<string,Texture2DExtended> textures;

        public TextureManager()
        {
            this.textures = new Dictionary<string,Texture2DExtended>();
        }

        public void addTexture(string name,Texture2DExtended texture)
        {
            this.textures.Add(name,texture);
        }

        /// <summary>
        /// Returns a Texture2DExtended held by the manager.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Texture2DExtended getTexture(string name)
        {
            foreach(var v in textures)
            {
                if (v.Key == name) return v.Value;
            }
            return null;
        }

    }
}
