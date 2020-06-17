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
        private static Texture2D caveCarrotSeedTexture;
        private static Texture2D caveCarrotCropTexture;
        private static Texture2D caveCarrotFlowerCropTexture;

        private static IModHelper helper;

        public static Texture2D caveCarrotFlowerCrop
        {
            get {
                if (caveCarrotFlowerCropTexture == null)
                {
                    loadAllTextures();
                }
                return caveCarrotFlowerCropTexture;
            }
        }

        public static Texture2D caveCarrotSeed
        {
            get {
                if (caveCarrotSeedTexture == null)
                {
                    loadAllTextures();
                }

                return caveCarrotSeedTexture;
            }
        }

        public static Texture2D caveCarrotCrop
        {
            get {
                if (caveCarrotCropTexture == null)
                {
                    loadAllTextures();
                }

                return caveCarrotCropTexture;
            }
        }

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

            if (sporeTexture == null)
            {
                sporeTexture = helper.Content.Load<Texture2D>(Path.Combine("assets", "voidshroom_spore.png"), ContentSource.ModFolder);
            }
            
            if (treeTexture == null)
            {
                treeTexture = helper.Content.Load<Texture2D>(Path.Combine("assets", "voidshroom_tree.png"), ContentSource.ModFolder);
            }
            
            if (caveCarrotSeedTexture == null)
            {
                caveCarrotSeedTexture = helper.Content.Load<Texture2D>(Path.Combine("assets", "cavecarrot_seed.png"), ContentSource.ModFolder);
            }

            if (caveCarrotCropTexture == null)
            {
                caveCarrotCropTexture = helper.Content.Load<Texture2D>(Path.Combine("assets", "cavecarrot_crop.png"), ContentSource.ModFolder);
            }

            if (caveCarrotFlowerCropTexture == null)
            {
                caveCarrotFlowerCropTexture = helper.Content.Load<Texture2D>(Path.Combine("assets", "cavecarrot_flower_crop.png"), ContentSource.ModFolder);
            }
        }

    }
}
