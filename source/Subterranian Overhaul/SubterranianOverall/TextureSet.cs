using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubterranianOverhaul
{
    class TextureSet
    {
        private static Texture2D sporeTexture;
        private static Texture2D treeTexture;

        public static Texture2D voidShroomSpore
        {
            get {
                if(sporeTexture == null)
                {
                    loadAllTextures();
                }

                return sporeTexture;
            }
        }
        public static Texture2D voidShroomTree
        {
            get {
                if (sporeTexture == null)
                {
                    loadAllTextures();
                }

                return treeTexture;
            }
        }

        private static void loadAllTextures()
        {

        }

    }
}
