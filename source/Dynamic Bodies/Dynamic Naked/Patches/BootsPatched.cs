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

        private static Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();
        public BootsPatched(Harmony harmony)
        {
            //Add new drawing method, done over the previous (possibly fix for artisanal products conflict?)
            harmony.Patch(
                original: AccessTools.Method(typeof(Boots), nameof(Boots.drawInMenu), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }),
                postfix: new HarmonyMethod(GetType(), nameof(Post_drawInMenu))
            );
        }
        public static void Post_drawInMenu(Boots __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            if (!textureCache.ContainsKey($"{__instance.indexInTileSheet}_{__instance.indexInColorSheet}")){
                CreateTextureCache(__instance.indexInTileSheet, __instance.indexInColorSheet);
                ModEntry.debugmsg($"Made boots for {__instance.indexInTileSheet}_{__instance.indexInColorSheet}", LogLevel.Debug);
            }
            if (textureCache.ContainsKey($"{__instance.indexInTileSheet}_{__instance.indexInColorSheet}")) {
                spriteBatch.Draw(textureCache[$"{__instance.indexInTileSheet}_{__instance.indexInColorSheet}"], location + new Vector2(32f, 32f) * scaleSize, new Rectangle(0, 0, 16, 16), color * transparency, 0f, new Vector2(8f, 8f) * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
            }//return true;
        }

        private static void CreateTextureCache(int indexInTileSheet, int indexInColorSheet)
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
            editor.PatchImage(Game1.objectSpriteSheet, rect);
            img.GetData(data);

            Texture2D shoeColors = Game1.content.Load<Texture2D>("Characters\\Farmer\\shoeColors");
            Color[] shoeColorsData = new Color[shoeColors.Width * shoeColors.Height];
            if (indexInColorSheet < 0)
            {
                indexInColorSheet = shoeColors.Height - 1;
            }
            if (indexInColorSheet > shoeColors.Height - 1)
            {
                indexInColorSheet = 0;
            }
            shoeColors.GetData(shoeColorsData);

            //Store what the colours are
            Color c0 = shoeColorsData[indexInColorSheet * 4 % (shoeColors.Height * 4)];
            Color c1 = shoeColorsData[indexInColorSheet * 4 % (shoeColors.Height * 4) + 1];
            Color c3 = shoeColorsData[indexInColorSheet * 4 % (shoeColors.Height * 4) + 2];
            Color c5 = shoeColorsData[indexInColorSheet * 4 % (shoeColors.Height * 4) + 3];
            //Interpolated colours
            Color c2 = Color.Lerp(c1, c3, 0.5f);
            Color c4 = Color.Lerp(c3, c5, 0.5f);

            

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].Equals(bootc0))
                {
                    data[i] = c0;
                }
                else if (data[i].Equals(bootc1))
                {
                    data[i] = c1;
                }
                else if (data[i].Equals(bootc2))
                {
                    data[i] = c2;
                }
                else if (data[i].Equals(bootc3))
                {
                    data[i] = c3;
                }
                else if (data[i].Equals(bootc4))
                {
                    data[i] = c4;
                }
                else if (data[i].Equals(bootc5))
                {
                    data[i] = c5;
                }
            }
            img.SetData(data);

            //Save the image
            textureCache[$"{indexInTileSheet}_{indexInColorSheet}"] = img;
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
