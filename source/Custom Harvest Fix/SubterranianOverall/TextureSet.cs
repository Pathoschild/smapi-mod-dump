using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubterranianOverhaul
{
    class TextureSet
    {
        private static Texture2D sporeTexture;
        private static Texture2D treeTexture;

        private static IModHelper helper;

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
            if(helper == null)
            {
                helper = ModEntry.GetHelper();
            }

            sporeTexture = helper.Content.Load<Texture2D>(Path.Combine("assets", "voidshroom_spore.png"), ContentSource.ModFolder);
            treeTexture = helper.Content.Load<Texture2D>(Path.Combine("assets", "voidshroom_tree.png"), ContentSource.ModFolder);
        }

    }
}
