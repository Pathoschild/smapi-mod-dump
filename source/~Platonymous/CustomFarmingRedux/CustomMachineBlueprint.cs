/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PyTK.Extensions;
using PyTK.Types;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;


namespace CustomFarmingRedux
{
    public class CustomMachineBlueprint
    {
        internal IModHelper Helper = CustomFarmingReduxMod._helper;
        internal IMonitor Monitor = CustomFarmingReduxMod._monitor;

        public int id { get; set; }
        public string useid => pack.useid;
        public string fullid
        {
            get
            {
                string folderid = (useid != "") ? useid : folder;
                return $"{folderid}.{file}.{id}";
            }
        }
        public string name { get; set; }

        public string loadconditions { get; set; } = "";
        public bool asdisplay { get; set; } = false;
        public string description { get; set; }
        public string category { get; set; } = "Crafting";
        public string legacy { get; set; } = "";
        public List<RecipeBlueprint> production { get; set; }
        public Texture2D _texture { get; set; }
        public string texture { get; set; }
        public int tileindex { get; set; } = 0;
        public int readyindex { get; set; } = -1;
        public int frames { get; set; } = 0;
        public int fps { get; set; } = 6;
        public bool showitem { get; set; } = false;
        public int[] itempos { get; set; } = new int[] { 0, 0 };
        public float itemzoom { get; set; } = 1;
        public int index { get; set; } = -1;
        public int tilewidth { get; set; } = 16;
        public int tileheight { get; set; } = 32;
        public bool water { get; set; } = false;
        public bool pulsate { get; set; } = true;
        public CustomFarmingPack pack;
        public string folder => pack.folderName;
        public string file => pack.fileName;
        public IngredientBlueprint starter { get; set; }
        public bool forsale { get; set; } = false;
        public string shop { get; set; } = "Robin";
        public int price { get; set; } = 100;
        public string condition { get; set; }
        public string crafting { get; set; }
        public Texture2D texture2d { get; set; }
        public string workconditions { get; set; } = "";
        public bool conditionaldropin { get; set; } = false;
        public bool conditionalanimation { get; set; } = false;
        public bool lightsource { get; set; } = false;
        public int[] lightcolor { get; set; } = new int[] { 0, 139, 139, 255 };
        public bool worklight { get; set; } = true;
        public float lightradius { get; set; } = 1.5f;
        public bool scaleup { get; set; } = false;
        public int originalwidth { get; set; } = 16;

        public Texture2D getTexture(IModHelper helper = null)
        {
            if (texture2d != null)
                return texture2d;

            if (helper == null)
                helper = Helper;

            if (texture2d == null)
                if (texture == null || texture == "")
                    texture2d = Game1.objectSpriteSheet;
                else
                {
                    if (pack.baseFolder != "ContentPack")
                        texture2d = helper.ModContent.Load<Texture2D>($"{pack.baseFolder}/{folder}/{texture}");
                    else
                        texture2d = pack.contentPack.ModContent.Load<Texture2D>(texture);
                }

            if (scaleup)
            {
                float scale = (float)(Convert.ToDouble(texture2d.Width) / Convert.ToDouble(originalwidth));
                int height = (int)(texture2d.Height / scale);
                texture2d = ScaledTexture2D.FromTexture(texture2d.getArea(new Rectangle(0, 0, originalwidth, height)), texture2d, scale);
            }
            return texture2d;
        }

    }
}
