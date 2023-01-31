/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ribeena/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

using StardewValley.Locations;
using StardewValley.Characters;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley.Objects;
using System.IO;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Color = Microsoft.Xna.Framework.Color;
using System.Drawing;
using Force.DeepCloner;
using DynamicBodies.Framework;


namespace DynamicBodies.Patches
{
    internal class BootsPatched
    {
        //Colours for changing
        static Color bootc0 = new Color(61,17,35,255);
        static Color bootc1 = new Color(91,31,36, 255);
        static Color bootc2 = new Color(105,36,31, 255);//interpolated
        static Color bootc3 = new Color(119,41,26, 255);
        static Color bootc4 = new Color(146,56,26, 255);//interpolated
        static Color bootc5 = new Color(173, 71, 27, 255);

        static BootsPatched bootsPatched;
        public static Dictionary<string, Texture2D> DGATextures = new Dictionary<string, Texture2D>();

        private static Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();
        public BootsPatched(Harmony harmony)
        {
            bootsPatched = this;
            //Add new drawing method, done over the previous (possibly fix for artisanal products conflict?)
            harmony.Patch(
                original: AccessTools.Method(typeof(Boots), nameof(Boots.drawInMenu), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }),
                postfix: new HarmonyMethod(GetType(), nameof(Post_drawInMenu))
            );
        }

        public static void PatchDGA(Harmony harmony)
        {
            //Dyanmically edit DGA by specifying the class (in the namesapce) and the assembly dll (eg "Namespace.Class, Assembly")
            var CustomBootsClass = Type.GetType("DynamicGameAssets.Game.CustomBoots, DynamicGameAssets", true, true);
            ModEntry.debugmsg($"Patching DGA boots: {CustomBootsClass}", LogLevel.Debug);
            harmony.Patch(
                original: AccessTools.Method(CustomBootsClass, "drawInMenu", new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }),
                prefix: new HarmonyMethod(bootsPatched.GetType(), nameof(Pre_drawInMenu_CustomBoots))
            );
        }
        public static void Post_drawInMenu(Boots __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            string shoesPaletteID = __instance.indexInColorSheet + "";
            if (__instance.modData.ContainsKey("DGA.FarmerColors")) shoesPaletteID = __instance.modData["DGA.FarmerColors"];

            if (!textureCache.ContainsKey($"{__instance.indexInTileSheet}_{shoesPaletteID}")){
                CreateTextureCache(__instance.indexInTileSheet, shoesPaletteID);
                ModEntry.debugmsg($"Made boots for {__instance.indexInTileSheet}_{shoesPaletteID}", LogLevel.Debug);
            }
            if (textureCache.ContainsKey($"{__instance.indexInTileSheet}_{shoesPaletteID}")) {
                spriteBatch.Draw(textureCache[$"{__instance.indexInTileSheet}_{shoesPaletteID}"], location + new Vector2(32f, 32f) * scaleSize, new Rectangle(0, 0, 16, 16), color * transparency, 0f, new Vector2(8f, 8f) * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
            }
        }

        public static bool Pre_drawInMenu_CustomBoots(Boots __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            string id = ModEntry.dga.GetDGAItemId(__instance);
            string colorindex = id;
            if (__instance.modData.ContainsKey("DGA.FarmerColors")) colorindex = __instance.modData["DGA.FarmerColors"];

            if (!textureCache.ContainsKey($"{id}_{colorindex}"))
            {
                CreateTextureCache(DGATextures[id], ShoesPalette.GetColors(colorindex), id+"_"+colorindex);
                ModEntry.debugmsg($"Made boots for {id}_{colorindex}", LogLevel.Debug);
            }
            if (textureCache.ContainsKey($"{id}_{colorindex}"))
            {
                spriteBatch.Draw(textureCache[$"{id}_{colorindex}"], location + new Vector2(32f, 32f) * scaleSize, new Rectangle(0, 0, 16, 16), color * transparency, 0f, new Vector2(8f, 8f) * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
                return false;
            }
            return true;
        }

        public static void CreateDGATexture(Texture2D tileSheet, int indexInTileSheet, string id)
        {
            Texture2D img = new Texture2D(Game1.graphics.GraphicsDevice, 16, 16);
            Color[] data = new Color[img.Width * img.Height];
            for (int i = 0; i < data.Length; i++) data[i] = Color.Transparent;
            img.SetData(data);//blank texture

            var editor = ModEntry.context.Helper.ModContent.GetPatchHelper(img).AsImage();
            
            //Draw just the one icon
            editor.PatchImage(tileSheet, Game1.getSourceRectForStandardTileSheet(tileSheet, indexInTileSheet, 16, 16));

            //Save the image
            DGATextures[id] = img;
        }

        private static void CreateTextureCache(int indexInTileSheet, string shoesPaletteID)
        {
            Texture2D img = new Texture2D(Game1.graphics.GraphicsDevice, 16, 16);
            Color[] data = new Color[img.Width * img.Height];
            for (int i = 0; i < data.Length; i++) data[i] = Color.Transparent;
            img.SetData(data);//blank texture

            var editor = ModEntry.context.Helper.ModContent.GetPatchHelper(img).AsImage();
            //Safety check for JsonAssets
            Rectangle rect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, indexInTileSheet, 16, 16);
            if(Game1.objectSpriteSheet.Width < rect.X+rect.Width || Game1.objectSpriteSheet.Height < rect.Y + rect.Height)
            {
                //Default shoe
                rect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 21*24+2, 16, 16);
            }
            //Draw just the one icon
            editor.PatchImage(Game1.objectSpriteSheet, rect);
            
            //Save the image
            CreateTextureCache(img, ShoesPalette.GetColors(shoesPaletteID), $"{indexInTileSheet}_{shoesPaletteID}");
        }

        private static void CreateTextureCache(Texture2D texture, Color[] colors, string id)
        {
            Texture2D img = new Texture2D(Game1.graphics.GraphicsDevice, 16, 16);
            Color[] data = new Color[img.Width * img.Height];
            texture.GetData(data);
            img.SetData(data);//copy of texture
            
            img.GetData(data);

            //Interpolated colours
            Color c1_2 = Color.Lerp(colors[1], colors[2], 0.5f);
            Color c2_3 = Color.Lerp(colors[2], colors[3], 0.5f);

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].Equals(bootc0))
                {
                    data[i] = colors[0];
                }
                else if (data[i].Equals(bootc1))
                {
                    data[i] = colors[1];
                }
                else if (data[i].Equals(bootc2))
                {
                    data[i] = c1_2;
                }
                else if (data[i].Equals(bootc3))
                {
                    data[i] = colors[2];
                }
                else if (data[i].Equals(bootc4))
                {
                    data[i] = c2_3;
                }
                else if (data[i].Equals(bootc5))
                {
                    data[i] = colors[3];
                }
            }
            img.SetData(data);

            //Save the image
            textureCache[id] = img;
        }

        public void PatchImage(IAssetData asset)
        {
            //Add colourable versions of the boots
            var editor = asset.AsImage();
            Texture2D boots = Game1.content.Load<Texture2D>("Mods/ribeena.dynamicbodies/assets/Interface/springobjects_boots.png");
            editor.PatchImage(boots, sourceArea: new Rectangle(0, 0, 192, 16), targetArea: new Rectangle(0, 336, 192, 16));
            editor.PatchImage(boots, sourceArea: new Rectangle(0, 16, 16, 16), targetArea: new Rectangle(192, 528, 16, 16));
            editor.PatchImage(boots, sourceArea: new Rectangle(16, 16, 16, 16), targetArea: new Rectangle(224, 528, 16, 16));
            editor.PatchImage(boots, sourceArea: new Rectangle(32, 16, 48, 16), targetArea: new Rectangle(208, 560, 48, 16));
            editor.PatchImage(boots, sourceArea: new Rectangle(80, 16, 16, 16), targetArea: new Rectangle(224, 576, 16, 16));
        }
    }
}
