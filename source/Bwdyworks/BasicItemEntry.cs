using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bwdyworks
{
    public class BasicItemEntry
    {
        public int IntegerId;
        public string GlobalId;
        public string ModId;

        public string InternalName;
        public int Price;
        public int Edibility;
        public string CategoryName;
        public int Category = -1; //-1 for Basic
        public string DisplayName;
        public string Description;

        [Newtonsoft.Json.JsonIgnore]
        public StardewModdingAPI.Mod Mod;

        public BasicItemEntry(StardewModdingAPI.Mod mod, string internal_name, int price, int edibility, string cat_name, int category, string display_name, string desc)
        {
            ModId = mod.ModManifest.UniqueID;
            InternalName = internal_name;
            Price = price;
            Edibility = edibility;
            CategoryName = cat_name;
            Category = category;
            DisplayName = display_name;
            Description = desc;
            Mod = mod;
        }

        public Texture2D LoadTexture()
        {
            return Mod.Helper.Content.Load<Texture2D>("./Assets/" + InternalName + ".png", StardewModdingAPI.ContentSource.ModFolder);
        }

        public string Compile()
        {
            return DisplayName + "/" + Price + "/" + Edibility + "/" + CategoryName + (Category >= 0 ? " " + Category + "/" : "") + "/" + InternalName + "/" + Description;
        }
    }
}
