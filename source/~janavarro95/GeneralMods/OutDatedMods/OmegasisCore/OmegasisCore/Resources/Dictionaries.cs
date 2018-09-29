using Microsoft.Xna.Framework.Graphics;
using OmegasisCore.Menus.MenuComponentsAndResources;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegasisCore.Resources
{
   public class Dictionaries
    {
        public static Dictionary<string, TextureDataNode> spriteFontList;
        public static string vanillaFontPath;

        public static void initalizeDictionaries()
        {
            vanillaFontPath = Path.Combine(@"assets", "Fonts", "colorlessSpriteFont", "vanilla");
            spriteFontList = new Dictionary<string, TextureDataNode>();
            fillAllDictionaries();
        }

        public static void fillAllDictionaries()
        {
            fillSpriteFontList();
        }

        public static void fillSpriteFontList()
        {
           
            spriteFontList.Add("0", new TextureDataNode(ModCore.HELPER.Content.Load<Texture2D>(Path.Combine(vanillaFontPath, "0.xnb")), Path.Combine(vanillaFontPath, "0")));

            spriteFontList.Add("leftArrow", new TextureDataNode(ModCore.HELPER.Content.Load<Texture2D>(Path.Combine(vanillaFontPath, "leftArrow")), Path.Combine(vanillaFontPath, "leftArrow")));
            spriteFontList.Add("rightArrow", new TextureDataNode(ModCore.HELPER.Content.Load<Texture2D>(Path.Combine(vanillaFontPath, "rightArrow")), Path.Combine(vanillaFontPath, "rightArrow")));
        }
    }
}
